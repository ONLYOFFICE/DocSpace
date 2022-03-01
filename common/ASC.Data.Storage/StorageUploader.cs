/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Data.Storage;

[Singletone]
public class StorageUploader
{
    protected readonly DistributedTaskQueue Queue;

    private static readonly object _locker;
    private readonly IServiceProvider _serviceProvider;
    private readonly TempStream _tempStream;
    private readonly ICacheNotify<MigrationProgress> _cacheMigrationNotify;

    static StorageUploader()
    {
        _locker = new object();
    }

    public StorageUploader(IServiceProvider serviceProvider, TempStream tempStream, ICacheNotify<MigrationProgress> cacheMigrationNotify, DistributedTaskQueueOptionsManager options)
    {
        _serviceProvider = serviceProvider;
        _tempStream = tempStream;
        _cacheMigrationNotify = cacheMigrationNotify;
        Queue = options.Get(nameof(StorageUploader));
    }

    public void Start(int tenantId, StorageSettings newStorageSettings, StorageFactoryConfig storageFactoryConfig)
    {
        lock (_locker)
        {
            var id = GetCacheKey(tenantId);
            var migrateOperation = Queue.GetTask<MigrateOperation>(id);
            if (migrateOperation != null)
            {
                return;
            }

            migrateOperation = new MigrateOperation(_serviceProvider, _cacheMigrationNotify, id, tenantId, newStorageSettings, storageFactoryConfig, _tempStream);
            Queue.QueueTask(migrateOperation);
        }
    }

    public MigrateOperation GetProgress(int tenantId)
    {
        lock (_locker)
        {
            return Queue.GetTask<MigrateOperation>(GetCacheKey(tenantId));
        }
    }

    public void Stop()
    {
        foreach (var task in Queue.GetTasks<MigrateOperation>().Where(r => r.Status == DistributedTaskStatus.Running))
        {
            Queue.CancelTask(task.Id);
        }
    }

    private static string GetCacheKey(int tenantId)
    {
        return typeof(MigrateOperation).FullName + tenantId;
    }
}

[Transient]
public class MigrateOperation : DistributedTaskProgress
{
    private static readonly string _configPath;
    private readonly ILog _logger;
    private readonly IEnumerable<string> _modules;
    private readonly StorageSettings _settings;
    private readonly int _tenantId;
    private readonly IServiceProvider _serviceProvider;
    private readonly StorageFactoryConfig _storageFactoryConfig;
    private readonly TempStream _tempStream;
    private readonly ICacheNotify<MigrationProgress> _cacheMigrationNotify;

    static MigrateOperation()
    {
        _configPath = string.Empty;
    }

    public MigrateOperation(
        IServiceProvider serviceProvider,
        ICacheNotify<MigrationProgress> cacheMigrationNotify,
        string id,
        int tenantId,
        StorageSettings settings,
        StorageFactoryConfig storageFactoryConfig,
        TempStream tempStream)
    {
        Id = id;
        Status = DistributedTaskStatus.Created;

        _serviceProvider = serviceProvider;
        _cacheMigrationNotify = cacheMigrationNotify;
        _tenantId = tenantId;
        _settings = settings;
        _storageFactoryConfig = storageFactoryConfig;
        _tempStream = tempStream;
        _modules = storageFactoryConfig.GetModuleList(_configPath, true);
        StepCount = _modules.Count();
        _logger = serviceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    protected override void DoJob()
    {
        try
        {
            _logger.DebugFormat("Tenant: {0}", _tenantId);
            Status = DistributedTaskStatus.Running;

            using var scope = _serviceProvider.CreateScope();
            var tempPath = scope.ServiceProvider.GetService<TempPath>();
            var scopeClass = scope.ServiceProvider.GetService<MigrateOperationScope>();
            var (tenantManager, securityContext, storageFactory, options, storageSettingsHelper, settingsManager) = scopeClass;
            var tenant = tenantManager.GetTenant(_tenantId);
            tenantManager.SetCurrentTenant(tenant);

            securityContext.AuthenticateMeWithoutCookie(tenant.OwnerId);

            foreach (var module in _modules)
            {
                var oldStore = storageFactory.GetStorage(_configPath, _tenantId.ToString(), module);
                var store = storageFactory.GetStorageFromConsumer(_configPath, _tenantId.ToString(), module, storageSettingsHelper.DataStoreConsumer(_settings));
                var domains = _storageFactoryConfig.GetDomainList(_configPath, module).ToList();

                var crossModuleTransferUtility = new CrossModuleTransferUtility(options, _tempStream, tempPath, oldStore, store);

                string[] files;
                foreach (var domain in domains)
                {
                    //Status = module + domain;
                    _logger.DebugFormat("Domain: {0}", domain);
                        files = oldStore.ListFilesRelativeAsync(domain, "\\", "*.*", true).ToArrayAsync().Result;

                    foreach (var file in files)
                    {
                        _logger.DebugFormat("File: {0}", file);
                            crossModuleTransferUtility.CopyFileAsync(domain, file, domain, file).Wait();
                    }
                }

                _logger.Debug("Domain:");

                    files = oldStore.ListFilesRelativeAsync(string.Empty, "\\", "*.*", true).ToArrayAsync().Result
                    .Where(path => domains.All(domain => !path.Contains(domain + "/")))
                    .ToArray();

                foreach (var file in files)
                {
                    _logger.DebugFormat("File: {0}", file);
                        crossModuleTransferUtility.CopyFileAsync("", file, "", file).Wait();
                }

                StepDone();

                MigrationPublish();
            }

            settingsManager.Save(_settings);
            tenant.SetStatus(TenantStatus.Active);
            tenantManager.SaveTenant(tenant);

            Status = DistributedTaskStatus.Completed;
        }
        catch (Exception e)
        {
            Status = DistributedTaskStatus.Failted;
            Exception = e;
            _logger.Error(e);
        }

        MigrationPublish();
    }

    private void MigrationPublish()
    {
        _cacheMigrationNotify.Publish(new MigrationProgress
        {
            TenantId = _tenantId,
            Progress = Percentage,
            Error = Exception.ToString(),
            IsCompleted = IsCompleted
        },
            Common.Caching.CacheNotifyAction.Insert);
    }
}

public class MigrateOperationScope
{
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly StorageFactory _storageFactory;
    private readonly IOptionsMonitor<ILog> _options;
    private readonly StorageSettingsHelper _storageSettingsHelper;
    private readonly SettingsManager _settingsManager;

    public MigrateOperationScope(TenantManager tenantManager,
        SecurityContext securityContext,
        StorageFactory storageFactory,
        IOptionsMonitor<ILog> options,
        StorageSettingsHelper storageSettingsHelper,
        SettingsManager settingsManager)
    {
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _storageFactory = storageFactory;
        _options = options;
        _storageSettingsHelper = storageSettingsHelper;
        _settingsManager = settingsManager;
    }

    public void Deconstruct(out TenantManager tenantManager,
        out SecurityContext securityContext,
        out StorageFactory storageFactory,
        out IOptionsMonitor<ILog> options,
        out StorageSettingsHelper storageSettingsHelper,
        out SettingsManager settingsManager)
    {
        tenantManager = _tenantManager;
        securityContext = _securityContext;
        storageFactory = _storageFactory;
        options = _options;
        storageSettingsHelper = _storageSettingsHelper;
        settingsManager = _settingsManager;
    }
}
