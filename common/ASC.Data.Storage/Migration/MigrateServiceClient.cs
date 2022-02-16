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
