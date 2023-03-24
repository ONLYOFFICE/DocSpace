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

namespace ASC.Data.Storage;

[Singletone]
public class StorageUploader
{
    protected readonly DistributedTaskQueue _queue;

    private static readonly object _locker;
    private readonly IServiceProvider _serviceProvider;
    private readonly TempStream _tempStream;
    private readonly ICacheNotify<MigrationProgress> _cacheMigrationNotify;
    private readonly ILogger<StorageUploader> _logger;

    static StorageUploader()
    {
        _locker = new object();
    }

    public StorageUploader(
        IServiceProvider serviceProvider,
        TempStream tempStream,
        ICacheNotify<MigrationProgress> cacheMigrationNotify,
        IDistributedTaskQueueFactory queueFactory,
        ILogger<StorageUploader> logger)
    {
        _serviceProvider = serviceProvider;
        _tempStream = tempStream;
        _cacheMigrationNotify = cacheMigrationNotify;
        _logger = logger;
        _queue = queueFactory.CreateQueue();
    }

    public void Start(int tenantId, StorageSettings newStorageSettings, StorageFactoryConfig storageFactoryConfig)
    {
        lock (_locker)
        {
            var id = GetCacheKey(tenantId);

            if (_queue.GetAllTasks().Any(x => x.Id == id))
            {
                return;
            }

            var migrateOperation = new MigrateOperation(_serviceProvider, _cacheMigrationNotify, id, tenantId, newStorageSettings, storageFactoryConfig, _tempStream, _logger);
            _queue.EnqueueTask(migrateOperation);
        }
    }

    public MigrateOperation GetProgress(int tenantId)
    {
        lock (_locker)
        {
            return _queue.PeekTask<MigrateOperation>(GetCacheKey(tenantId));
        }
    }

    public void Stop()
    {
        foreach (var task in _queue.GetAllTasks(DistributedTaskQueue.INSTANCE_ID).Where(r => r.Status == DistributedTaskStatus.Running))
        {
            _queue.DequeueTask(task.Id);
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
    private readonly ILogger<StorageUploader> _logger;
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
        TempStream tempStream,
        ILogger<StorageUploader> logger)
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
        _logger = logger;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    protected override async Task DoJob()
    {
        try
        {
            _logger.DebugTenant(_tenantId);
            Status = DistributedTaskStatus.Running;

            await using var scope = _serviceProvider.CreateAsyncScope();
            var tempPath = scope.ServiceProvider.GetService<TempPath>();
            var scopeClass = scope.ServiceProvider.GetService<MigrateOperationScope>();
            var (tenantManager, securityContext, storageFactory, options, storageSettingsHelper, settingsManager) = scopeClass;
            var tenant = await tenantManager.GetTenantAsync(_tenantId);
            tenantManager.SetCurrentTenant(tenant);

            await securityContext.AuthenticateMeWithoutCookieAsync(tenant.OwnerId);

            foreach (var module in _modules)
            {
                var oldStore = await storageFactory.GetStorageAsync(_tenantId, module);
                var store = storageFactory.GetStorageFromConsumer(_tenantId, module, storageSettingsHelper.DataStoreConsumer(_settings));
                var domains = _storageFactoryConfig.GetDomainList(module).ToList();

                var crossModuleTransferUtility = new CrossModuleTransferUtility(options, _tempStream, tempPath, oldStore, store);

                string[] files;
                foreach (var domain in domains)
                {
                    //Status = module + domain;
                    _logger.DebugDomain(domain);
                    files = await oldStore.ListFilesRelativeAsync(domain, "\\", "*.*", true).ToArrayAsync();

                    foreach (var file in files)
                    {
                        _logger.DebugFile(file);
                        await crossModuleTransferUtility.CopyFileAsync(domain, file, domain, file);
                    }
                }

                files = (await oldStore.ListFilesRelativeAsync(string.Empty, "\\", "*.*", true).ToArrayAsync())
                .Where(path => domains.All(domain => !path.Contains(domain + "/")))
                .ToArray();

                foreach (var file in files)
                {
                    _logger.DebugFile(file);
                    await crossModuleTransferUtility.CopyFileAsync("", file, "", file);
                }

                StepDone();

                MigrationPublish();
            }

            await settingsManager.SaveAsync(_settings);
            tenant.SetStatus(TenantStatus.Active);
            await tenantManager.SaveTenantAsync(tenant);

            Status = DistributedTaskStatus.Completed;
        }
        catch (Exception e)
        {
            Status = DistributedTaskStatus.Failted;
            Exception = e;
            _logger.ErrorMigrateOperation(e);
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
            CacheNotifyAction.Insert);
    }
}

public class MigrateOperationScope
{
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly StorageFactory _storageFactory;
    private readonly ILogger _options;
    private readonly StorageSettingsHelper _storageSettingsHelper;
    private readonly SettingsManager _settingsManager;

    public MigrateOperationScope(TenantManager tenantManager,
        SecurityContext securityContext,
        StorageFactory storageFactory,
        ILogger<MigrateOperationScope> options,
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
        out ILogger options,
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
