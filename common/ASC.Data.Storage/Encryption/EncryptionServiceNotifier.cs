using System;

using ASC.Common;
using ASC.Common.Caching;

namespace ASC.Data.Storage.Encryption
{
    [Singletone]
    public class EncryptionServiceNotifier
    {
        private ICacheNotify<ProgressEncryption> СacheBackupProgress { get; }
        private ICache Cache { get; }

        public EncryptionServiceNotifier(ICacheNotify<ProgressEncryption> сacheBackupProgress)
        {
            Cache = AscCache.Memory;
            СacheBackupProgress = сacheBackupProgress;

            СacheBackupProgress.Subscribe((a) =>
            {
                Cache.Insert(GetCacheKey(a.TenantId), a, DateTime.UtcNow.AddDays(1));
            },
            CacheNotifyAction.InsertOrUpdate);
        }

        public ProgressEncryption GetEncryptionProgress(int tenantId)
        {
            return Cache.Get<ProgressEncryption>(GetCacheKey(tenantId));
        }

        private string GetCacheKey(int tenantId)
        {
            return $"encryption{tenantId}";
        }
    }
}
