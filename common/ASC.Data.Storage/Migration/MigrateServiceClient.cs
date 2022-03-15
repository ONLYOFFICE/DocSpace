namespace ASC.Data.Storage.Migration;

[Singletone]
public class ServiceClientListener
{
    private readonly ICacheNotify<MigrationProgress> _progressMigrationNotify;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICache _cache;

    public ServiceClientListener(
        ICacheNotify<MigrationProgress> progressMigrationNotify,
        IServiceProvider serviceProvider,
        ICache cache)
    {
        _progressMigrationNotify = progressMigrationNotify;
        _serviceProvider = serviceProvider;
        _cache = cache;

        ProgressListening();
    }

    public MigrationProgress GetProgress(int tenantId)
    {
        return _cache.Get<MigrationProgress>(GetCacheKey(tenantId));
    }

    private void ProgressListening()
    {
        _progressMigrationNotify.Subscribe(n =>
        {
            var migrationProgress = new MigrationProgress
            {
                TenantId = n.TenantId,
                Progress = n.Progress,
                IsCompleted = n.IsCompleted,
                Error = n.Error
            };

            _cache.Insert(GetCacheKey(n.TenantId), migrationProgress, DateTime.MaxValue);
        },
           Common.Caching.CacheNotifyAction.Insert);
    }

    private string GetCacheKey(int tenantId)
    {
        return typeof(MigrationProgress).FullName + tenantId;
    }
}

[Scope]
public class ServiceClient : IService
{
    public ServiceClientListener ServiceClientListener { get; }
    public ICacheNotify<MigrationCache> CacheMigrationNotify { get; }
    public ICacheNotify<MigrationUploadCdn> UploadCdnMigrationNotify { get; }
    public IServiceProvider ServiceProvider { get; }

    public ServiceClient(
        ServiceClientListener serviceClientListener,
        ICacheNotify<MigrationCache> cacheMigrationNotify,
        ICacheNotify<MigrationUploadCdn> uploadCdnMigrationNotify,
        IServiceProvider serviceProvider)
    {
        ServiceClientListener = serviceClientListener;
        CacheMigrationNotify = cacheMigrationNotify;
        UploadCdnMigrationNotify = uploadCdnMigrationNotify;
        ServiceProvider = serviceProvider;
    }

    public void Migrate(int tenant, StorageSettings storageSettings)
    {
        var storSettings = new StorSettings { Id = storageSettings.ID.ToString(), Module = storageSettings.Module };

        CacheMigrationNotify.Publish(new MigrationCache
        {
            TenantId = tenant,
            StorSettings = storSettings
        },
                Common.Caching.CacheNotifyAction.Insert);
    }

    public void UploadCdn(int tenantId, string relativePath, string mappedPath, CdnStorageSettings settings = null)
    {
        var cdnStorSettings = new CdnStorSettings { Id = settings.ID.ToString(), Module = settings.Module };

        UploadCdnMigrationNotify.Publish(new MigrationUploadCdn
        {
            Tenant = tenantId,
            RelativePath = relativePath,
            MappedPath = mappedPath,
            CdnStorSettings = cdnStorSettings
        },
                Common.Caching.CacheNotifyAction.Insert);
    }

    public double GetProgress(int tenant)
    {
        var migrationProgress = ServiceClientListener.GetProgress(tenant);

        return migrationProgress.Progress;
    }

    public void StopMigrate()
    {
            CacheMigrationNotify.Publish(new MigrationCache(), Common.Caching.CacheNotifyAction.InsertOrUpdate);
    }
}
