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

namespace ASC.Data.Storage.Migration;

[Singletone]
public class ServiceClientListener
{
    private readonly ICacheNotify<MigrationProgress> _progressMigrationNotify;
    private readonly ICache _cache;

    public ServiceClientListener(
        ICacheNotify<MigrationProgress> progressMigrationNotify,
        ICache cache)
    {
        _progressMigrationNotify = progressMigrationNotify;
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
           CacheNotifyAction.Insert);
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

    public ServiceClient(
        ServiceClientListener serviceClientListener,
        ICacheNotify<MigrationCache> cacheMigrationNotify,
        ICacheNotify<MigrationUploadCdn> uploadCdnMigrationNotify)
    {
        ServiceClientListener = serviceClientListener;
        CacheMigrationNotify = cacheMigrationNotify;
        UploadCdnMigrationNotify = uploadCdnMigrationNotify;
    }

    public void Migrate(int tenant, StorageSettings storageSettings)
    {
        var storSettings = new StorSettings { Id = storageSettings.ID.ToString(), Module = storageSettings.Module };

        CacheMigrationNotify.Publish(new MigrationCache
        {
            TenantId = tenant,
            StorSettings = storSettings
        },
                CacheNotifyAction.Insert);
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
                CacheNotifyAction.Insert);
    }

    public double GetProgress(int tenant)
    {
        var migrationProgress = ServiceClientListener.GetProgress(tenant);

        return migrationProgress.Progress;
    }

    public void StopMigrate()
    {
        CacheMigrationNotify.Publish(new MigrationCache(), CacheNotifyAction.InsertOrUpdate);
    }
}
