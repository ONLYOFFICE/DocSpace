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

using ASC.Core;

namespace ASC.Migration.PersonalToDocspace.Runner;

[Scope]
public class MigrationRunner
{
    private readonly DbFactory _dbFactory;
    private readonly StorageFactory _storageFactory;
    private readonly StorageFactoryConfig _storageFactoryConfig;
    private readonly ModuleProvider _moduleProvider;
    private readonly ILogger<RestoreDbModuleTask> _logger;
    private readonly CreatorDbContext _creatorDbContext;

    private string _backupFile;
    private string _region;
    private List<IModuleSpecifics> _modules;
    private readonly List<ModuleName> _namesModules = new List<ModuleName>()
    {
        ModuleName.Core,
        ModuleName.Files,
        ModuleName.Files2,
        ModuleName.Tenants,
        ModuleName.WebStudio
    };

    public MigrationRunner(
        DbFactory dbFactory,
        StorageFactory storageFactory,
        StorageFactoryConfig storageFactoryConfig,
        ModuleProvider moduleProvider,
        ILogger<RestoreDbModuleTask> logger,
        CreatorDbContext creatorDbContext)
    {
        _dbFactory = dbFactory;
        _storageFactory = storageFactory;
        _storageFactoryConfig = storageFactoryConfig;
        _moduleProvider = moduleProvider;
        _logger = logger;
        _creatorDbContext = creatorDbContext;
    }

    public async Task Run(string backupFile, string region, string fromAlias, string toAlias)
    {
        _region = region;
        _modules = _moduleProvider.AllModules.Where(m => _namesModules.Contains(m.ModuleName)).ToList();
        _backupFile = backupFile;
        var columnMapper = new ColumnMapper();
        if (!string.IsNullOrEmpty(toAlias))
        {
            using var dbContextTenant = _creatorDbContext.CreateDbContext<TenantDbContext>();
            var fromTenant = dbContextTenant.Tenants.SingleOrDefault(q => q.Alias == fromAlias);

            using var dbContextToTenant = _creatorDbContext.CreateDbContext<TenantDbContext>(region);
            var toTenant = dbContextToTenant.Tenants.SingleOrDefault(q => q.Alias == toAlias);

            toTenant.Status = TenantStatus.Restoring;
            toTenant.StatusChanged = DateTime.UtcNow;

            dbContextTenant.Tenants.Update(toTenant);
            dbContextToTenant.SaveChanges();

            columnMapper.SetMapping("tenants_tenants", "id", fromTenant.Id, toTenant.Id);
            columnMapper.Commit();
        }
        using (var dataReader = new ZipReadOperator(_backupFile))
        {
            foreach (var module in _modules)
            {
                Console.WriteLine($"start restore module: {module}");
                var restoreTask = new RestoreDbModuleTask(_logger, module, dataReader, columnMapper, _dbFactory, false, false, _region, _storageFactory, _storageFactoryConfig, _moduleProvider);

                await restoreTask.RunJob();
                Console.WriteLine($"end restore module: {module}");
            }

            await DoRestoreStorage(dataReader, columnMapper);

            SetTenantActiveaAndTenantOwner(columnMapper.GetTenantMapping());
            SetAdmin(columnMapper.GetTenantMapping());
        }
    }

    private async Task DoRestoreStorage(IDataReadOperator dataReader, ColumnMapper columnMapper)
    {
        var fileGroups = GetFilesToProcess(dataReader).GroupBy(file => file.Module).ToList();

        foreach (var group in fileGroups)
        {
            Console.WriteLine($"start restore fileGroup: {group.Key}");
            foreach (var file in group)
            {
                var storage = await _storageFactory.GetStorageAsync(columnMapper.GetTenantMapping(), group.Key, _region);
                var quotaController = storage.QuotaController;
                storage.SetQuotaController(null);

                try
                {
                    var adjustedPath = file.Path;
                    var module = _moduleProvider.GetByStorageModule(file.Module, file.Domain);
                    if (module == null || module.TryAdjustFilePath(false, columnMapper, ref adjustedPath))
                    {
                        var key = file.GetZipKey();
                        using var stream = dataReader.GetEntry(key);

                        await storage.SaveAsync(file.Domain, adjustedPath, module != null ? module.PrepareData(key, stream, columnMapper) : stream);
                    }
                }
                finally
                {
                    if (quotaController != null)
                    {
                        storage.SetQuotaController(quotaController);
                    }
                }
            }
            Console.WriteLine($"end restore fileGroup: {group.Key}");
        }
    }

    private IEnumerable<BackupFileInfo> GetFilesToProcess(IDataReadOperator dataReader)
    {
        using var stream = dataReader.GetEntry(KeyHelper.GetStorageRestoreInfoZipKey());
        if (stream == null)
        {
            return Enumerable.Empty<BackupFileInfo>();
        }

        var restoreInfo = XElement.Load(new StreamReader(stream));

        return restoreInfo.Elements("file").Select(BackupFileInfo.FromXElement).ToList();
    }

    private void SetTenantActiveaAndTenantOwner(int tenantId)
    {
        using var dbContextTenant = _creatorDbContext.CreateDbContext<TenantDbContext>(_region);
        using var dbContextUser = _creatorDbContext.CreateDbContext<UserDbContext>(_region);

        var tenant = dbContextTenant.Tenants.Single(t => t.Id == tenantId);
        tenant.Status = TenantStatus.Active;
        Console.WriteLine("set tenant status");
        tenant.LastModified = DateTime.UtcNow;
        tenant.StatusChanged = DateTime.UtcNow;
        if (!dbContextUser.Users.Any(q => q.Id == tenant.OwnerId))
        {

            var user = dbContextUser.Users.Single(u => u.TenantId == tenantId);
            tenant.OwnerId = user.Id;
            Console.WriteLine($"set ownerId {user.Id}");
        }
        dbContextTenant.Tenants.Update(tenant);
        dbContextTenant.SaveChanges();
    }

    private void SetAdmin(int tenantId)
    {
        using var dbContextTenant = _creatorDbContext.CreateDbContext<TenantDbContext>(_region);
        var tenant = dbContextTenant.Tenants.Single(t => t.Id == tenantId);
        using var dbContextUser = _creatorDbContext.CreateDbContext<UserDbContext>(_region);

        if (!dbContextUser.UserGroups.Any(q => q.TenantId == tenantId))
        {
            var userGroup = new UserGroup()
            {
                TenantId = tenantId,
                LastModified = DateTime.UtcNow,
                RefType = ASC.Core.UserGroupRefType.Contains,
                Removed = false,
                UserGroupId = ASC.Common.Security.Authorizing.Constants.DocSpaceAdmin.ID,
                Userid = tenant.OwnerId.Value
            };

            dbContextUser.UserGroups.Add(userGroup);
            dbContextUser.SaveChanges();
        }
    }
}
