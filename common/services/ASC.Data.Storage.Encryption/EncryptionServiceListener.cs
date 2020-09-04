using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Data.Storage.Encryption;

using ASC.Common.Caching;
using Microsoft.Extensions.DependencyInjection;
using ASC.Common;

namespace ASC.Data.Storage.Encryption
{
    public class EncryptionServiceListener
    {
        private ICacheNotify<EncryptionSettingsProto> NotifySettings { get; }
        private ICacheNotify<EncryptionStop> NotifyStop { get; }
        private IServiceProvider ServiceProvider { get; }
        private ICache Cache { get; }
        private string Key { get; set; }

        public EncryptionServiceListener(IServiceProvider serviceProvider, ICacheNotify<EncryptionSettingsProto> notifySettings, ICacheNotify<EncryptionStop> notifyStop)
        {
            NotifySettings = notifySettings;
            ServiceProvider = serviceProvider;
            NotifyStop = notifyStop;
            Cache = AscCache.Memory;
        }

        public void Start()
        {
            NotifySettings.Subscribe((n) => StartEncryption(n), CacheNotifyAction.Insert);
            NotifyStop.Subscribe((n) => StopEncryption(), CacheNotifyAction.Insert);
        }

        public void Stop()
        {
            NotifySettings.Unsubscribe(CacheNotifyAction.Insert);
            NotifySettings.Unsubscribe(CacheNotifyAction.Insert);
        }

        public void StartEncryption(EncryptionSettingsProto encryptionSettings)
        {
            using var scope = ServiceProvider.CreateScope();
            var encryptionWorker = scope.ServiceProvider.GetService<EncryptionWorker>();
            Key = encryptionWorker.GetCacheKey();
            encryptionWorker.Start(encryptionSettings);
        }

        public void StopEncryption()
        {
            var encryptionWorker = Cache.Get<EncryptionWorker>(Key);
            if(encryptionWorker != null)  encryptionWorker.Stop();
        }
    }

    public static class EncryptionServiceListenerExtension
    {
        public static DIHelper AddEncryptionServiceListener(this DIHelper services)
        {
            services.TryAddSingleton<EncryptionServiceListener>();
            return services.AddEncryptionWorkerService();
        }
    }
}   
