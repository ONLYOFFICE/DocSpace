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
    private readonly object _locker = new object();
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
        IHostEnvironment hostEnvironment,
        IConfiguration configuration,
        TenantDomainValidator tenantDomainValidator,
        TempStream tempStream,
        DbFactory dbFactory,
        StorageFactory storageFactory,
        StorageFactoryConfig storageFactoryConfig,
        ModuleProvider moduleProvider)
    {
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
        _tenantDomainValidator = tenantDomainValidator;
        _tempStream = tempStream;
        _dbFactory = dbFactory;
        _storageFactory = storageFactory;
        _storageFactoryConfig = storageFactoryConfig;
        _moduleProvider = moduleProvider;
    }

    public string Create(int tenant, string userName, string toRegion)
    {
        Init(tenant, userName, toRegion);

        var id = GetId();
        var fileName = _userName + ".tar.gz";
        var path = Path.Combine(_pathToSave, fileName);
        using (var writer = new ZipWriteOperator(_tempStream, path))
        {
            DoMigrationDb(id, writer);
            DoMigrationStorage(id, writer);
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
    }

    private Guid GetId()
    {
        try
        {
            var userDbContext = _dbFactory.CreateDbContext<UserDbContext>();
            return userDbContext.Users.FirstOrDefault(q => q.Tenant == _tenant && q.Status == EmployeeStatus.Active && q.UserName == _userName).Id;
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
                    if (table.Name == "files_thirdparty_account" || table.Name == "files_thirdparty_id_mapping" || table.Name == "core_subscription" || table.Name == "files_security")
                    {
                        continue;
                    }

                    Console.WriteLine($"backup table {table.Name}");
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
        var q = data.Rows[0];
        data.Rows[0]["alias"] = newAlias;
    }

    private List<string> GetAliases()
    {
        using var dbContext = _dbFactory.CreateDbContext<TenantDbContext>(_toRegion);
        var tenants = dbContext.Tenants.Select(t => t.Alias).ToList();
        var forbidens = dbContext.TenantForbiden.Select(tf => tf.Address).ToList();
        return tenants.Union(forbidens).ToList();
    }

    private void ChangeName(DataTable data)
    {
        data.Rows[0]["name"] = "";
    }

    private void DoMigrationStorage(Guid id, IDataWriteOperator writer)
    {
        Console.WriteLine($"start backup storage");
        var fileGroups = GetFilesGroup(id);
        foreach (var group in fileGroups)
        {
            Console.WriteLine($"start backup fileGroup: {group.Key}");
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
            Console.WriteLine($"end backup fileGroup: {group.Key}");
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
        Console.WriteLine($"end backup storage");
    }

    private List<IGrouping<string, BackupFileInfo>> GetFilesGroup(Guid id)
    {
        var files =   GetFilesToProcess(id).ToList();

        return files.GroupBy(file => file.Module).ToList();
    }

    private IEnumerable<BackupFileInfo> GetFilesToProcess(Guid id)
    {
        var files = new List<BackupFileInfo>();

        var filesDbContext = _dbFactory.CreateDbContext<FilesDbContext>();

        var module = _storageFactoryConfig.GetModuleList().Where(m => m == "files").Single();

        var store = _storageFactory.GetStorage(_tenant, module);

        var dbFiles = filesDbContext.Files.Where(q => q.CreateBy == id && q.TenantId == _tenant).ToList();

        var tasks = new List<Task>(20);
        foreach (var dbFile in dbFiles)
        {
            if (tasks.Count != 20)
            {
                tasks.Add(FindFiles(files, store, dbFile, module));
            }
            else
            {
                Task.WaitAll(tasks.ToArray());
                tasks.Clear();
            }
        }
        Task.WaitAll(tasks.ToArray());
        return files.Distinct();
    }

    private async Task FindFiles(List<BackupFileInfo> list, IDataStore store, DbFile dbFile, string module)
    {
        var files = await store.ListFilesRelativeAsync(string.Empty, $"\\{GetUniqFileDirectory(dbFile.Id)}", "*.*", true)
                 .Select(path => new BackupFileInfo(string.Empty, module, $"{GetUniqFileDirectory(dbFile.Id)}\\{path}", _tenant))
                 .ToListAsync();

        lock (_locker)
        {
            list.AddRange(files);
        }

        if (files.Any()) 
        {
            Console.WriteLine($"file {dbFile.Id} found");
        }
        else
        {
            Console.WriteLine($"file {dbFile.Id} not found");
        }
    }

    private string GetUniqFileDirectory(int fileId)
    {
        if (fileId == 0)
        {
            throw new ArgumentNullException("fileIdObject");
        }

        return string.Format("folder_{0}/file_{1}", (fileId / 1000 + 1) * 1000, fileId);
    }
}
