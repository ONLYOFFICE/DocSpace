// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Migration.PersonalToDocspace.Creator;

[Scope]
public class MigrationCreator
{
    private readonly IDbContextFactory<UserDbContext> _userDbContext;
    private readonly IDbContextFactory<BackupsContext> _backupsContext;
    private readonly IDbContextFactory<FilesDbContext> _filesDbContext;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IConfiguration _configuration;
    private readonly TenantDomainValidator _tenantDomainValidator;
    private readonly TempStream _tempStream;
    private readonly DbFactory _dbFactory;
    private readonly StorageFactory _storageFactory;
    private readonly StorageFactoryConfig _storageFactoryConfig;
    private readonly ModuleProvider _moduleProvider;

    private List<IModuleSpecifics> _modules;
    private string _pathToSave;
    private string _userName;
    private string _toRegion;
    private int _tenant;
    private readonly int _limit = 1000;
    private readonly List<ModuleName> _namesModules = new List<ModuleName>()
    {
        ModuleName.Core,
        ModuleName.Files,
        ModuleName.Files2,
        ModuleName.Tenants,
        ModuleName.WebStudio
    };

    public MigrationCreator(
        IDbContextFactory<UserDbContext> userDbContext,
        IDbContextFactory<BackupsContext> backupsContext,
        IDbContextFactory<FilesDbContext> filesDbContext,
        IHostEnvironment hostEnvironment,
        IConfiguration configuration,
        TenantDomainValidator tenantDomainValidator,
        TempStream tempStream,
        DbFactory dbFactory,
        StorageFactory storageFactory,
        StorageFactoryConfig storageFactoryConfig,
        ModuleProvider moduleProvider)
    {
        _userDbContext = userDbContext;
        _backupsContext = backupsContext;
        _filesDbContext = filesDbContext;
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
        _tenantDomainValidator = tenantDomainValidator;
        _tempStream = tempStream;
        _dbFactory = dbFactory;
        _storageFactory = storageFactory;
        _storageFactoryConfig = storageFactoryConfig;
        _moduleProvider = moduleProvider;
    }



    public async Task<string> Create(int tenant, string userName, string toRegion)
    {
        Init(tenant, userName, toRegion);

        var id = GetId();
        var fileName = _userName + ".tar.gz";
        var path = Path.Combine(_pathToSave, fileName);
        using (var writer = new ZipWriteOperator(_tempStream, path))
        {
            DoMigrationDb(id, writer);
            await DoMigrationStorage(id, writer);
        }
        return fileName;
    }

    private void Init(int tenant, string userName, string toRegion)
    {
        _modules = _moduleProvider.AllModules.Where(m => _namesModules.Contains(m.ModuleName)).ToList();

        _pathToSave = "";
        _toRegion = toRegion;
        _userName = userName;
        _tenant = tenant;

        CheckExistDataStorage();
    }

    private void CheckExistDataStorage()
    {
        var store = _storageFactory.GetStorage(_tenant, "files");
        if (store is DiscDataStore)
        {
            var path = Path.Combine(_hostEnvironment.ContentRootPath, _configuration[Data.Storage.Constants.StorageRootParam]);
            if (!Directory.Exists(path))
            {
                throw new Exception("Wrong $STORAGE_ROOT, change $STORAGE_ROOT in appsettings.json");
            }
        }
    }

    private Guid GetId()
    {
        try
        {
            using var dbContext = _userDbContext.CreateDbContext();
            return dbContext.Users.FirstOrDefault(q => q.Tenant == _tenant && q.Status == EmployeeStatus.Active && q.UserName == _userName).Id;
        }
        catch (Exception)
        {
            throw new Exception("username was not found");
        }
    }

    private void DoMigrationDb(Guid id, IDataWriteOperator writer)
    {
        foreach (var module in _modules)
        {
            var tablesToProcess = module.Tables.Where(t => t.InsertMethod != InsertMethod.None).ToList();

            using (var connection = _dbFactory.OpenConnection())
            {
                foreach (var table in tablesToProcess)
                {
                    using (var data = new DataTable(table.Name))
                    {
                        ActionInvoker.Try(
                            state =>
                            {
                                data.Clear();
                                int counts;
                                var offset = 0;
                                do
                                {
                                    var t = (TableInfo)state;
                                    var dataAdapter = _dbFactory.CreateDataAdapter();
                                    dataAdapter.SelectCommand = module.CreateSelectCommand(connection.Fix(), _tenant, t, _limit, offset, id).WithTimeout(600);
                                    counts = ((DbDataAdapter)dataAdapter).Fill(data);
                                    offset += _limit;
                                } while (counts == _limit);

                            },
                            table,
                            maxAttempts: 5,
                            onFailure: error => { throw ThrowHelper.CantBackupTable(table.Name, error); });

                        foreach (var col in data.Columns.Cast<DataColumn>().Where(col => col.DataType == typeof(DateTime)))
                        {
                            col.DateTimeMode = DataSetDateTime.Unspecified;
                        }

                        module.PrepareData(data);

                        if (data.TableName == "tenants_tenants")
                        {
                            ChangeAlias(data);
                            ChangeName(data);
                        }

                        using (var file = _tempStream.Create())
                        {
                            data.WriteXml(file, XmlWriteMode.WriteSchema);
                            data.Clear();

                            writer.WriteEntry(KeyHelper.GetTableZipKey(module, data.TableName), file);
                        }
                    }
                }
            }
        }

    }

    private void ChangeAlias(DataTable data)
    {
        var aliases = GetAliases();
        var newAlias = _userName;
        while (true)
        {
            try
            {
                _tenantDomainValidator.ValidateDomainLength(newAlias);
                _tenantDomainValidator.ValidateDomainCharacters(newAlias);
                if (aliases.Contains(newAlias))
                {
                    throw new Exception($"Alias is busy");
                }
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                newAlias = Console.ReadLine();
            }
        }
        data.Rows[0]["alias"] = newAlias;
    }

    private void ChangeName(DataTable data)
    {
        data.Rows[0]["name"] = "";
    }

    private List<string> GetAliases()
    {
        using (var connection = _dbFactory.OpenConnection(region: _toRegion))
        {
            var list = new List<string>();
            var dataAdapter = _dbFactory.CreateDataAdapter();

            var command = connection.CreateCommand();
            command.CommandText = "select alias from tenants_tenants left outer join tenants_forbiden on alias = address";
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetValue(0).ToString());
                    }
                }
                return list;
            }
        }
    }

    private async Task DoMigrationStorage(Guid id, IDataWriteOperator writer)
    {
        var fileGroups = await GetFilesGroup(id);
        foreach (var group in fileGroups)
        {
            foreach (var file in group)
            {
                var storage = _storageFactory.GetStorage(_tenant, group.Key);
                var file1 = file;
                ActionInvoker.Try(state =>
                {
                    var f = (BackupFileInfo)state;
                    using var fileStream = storage.GetReadStreamAsync(f.Domain, f.Path).Result;
                    writer.WriteEntry(file1.GetZipKey(), fileStream);
                }, file, 5);

            }
        }

        var restoreInfoXml = new XElement(
            "storage_restore",
            fileGroups
                .SelectMany(group => group.Select(file => (object)file.ToXElement()))
                .ToArray());

        using (var tmpFile = _tempStream.Create())
        {
            restoreInfoXml.WriteTo(tmpFile);
            writer.WriteEntry(KeyHelper.GetStorageRestoreInfoZipKey(), tmpFile);
        }
    }

    private async Task<List<IGrouping<string, BackupFileInfo>>> GetFilesGroup(Guid id)
    {
        var files = (await GetFilesToProcess(id)).ToList();

        using var dbManager = _backupsContext.CreateDbContext();
        var exclude = dbManager.Backups.AsQueryable().Where(b => b.TenantId == _tenant && b.StorageType == 0 && b.StoragePath != null).ToList();
        files = files.Where(f => !exclude.Any(e => f.Path.Replace('\\', '/').Contains($"/file_{e.StoragePath}/"))).ToList();

        return files.GroupBy(file => file.Module).ToList();
    }

    protected async Task<IEnumerable<BackupFileInfo>> GetFilesToProcess(Guid id)
    {
        var files = new List<BackupFileInfo>();

        foreach (var module in _storageFactoryConfig.GetModuleList().Where(m => m == "files"))
        {
            var store = _storageFactory.GetStorage(_tenant, module);
            var domains = _storageFactoryConfig.GetDomainList(module).ToArray();

            foreach (var domain in domains)
            {
                files.AddRange(await store.ListFilesRelativeAsync(domain, "\\", "*.*", true).Select(path => new BackupFileInfo(domain, module, path, _tenant)).ToListAsync());
            }

            files.AddRange(await
                store.ListFilesRelativeAsync(string.Empty, "\\", "*.*", true)
                .Where(path => domains.All(domain => !path.Contains(domain + "/")))
                .Select(path => new BackupFileInfo(string.Empty, module, path, _tenant))
                .ToListAsync());
        }

        using var filesRecordContext = _filesDbContext.CreateDbContext();
        files = files.Where(f => UserIsFileOwner(id, f, filesRecordContext)).ToList();
        return files.Distinct();
    }

    private bool UserIsFileOwner(Guid id, BackupFileInfo fileInfo, FilesDbContext filesDbContext)
    {
        var stringId = id.ToString();
        return filesDbContext.Files.Any(
            f => f.CreateBy == id &&
            f.TenantId == _tenant &&
            fileInfo.Path.Contains("\\file_" + f.Id + "\\"));
    }
}
