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
        private EncryptionWorker EncryptionWorker { get; }
        private IServiceProvider ServiceProvider { get; }

        public EncryptionServiceListener(IServiceProvider serviceProvider, ICacheNotify<EncryptionSettingsProto> notifySettings, ICacheNotify<EncryptionStop> notifyStop, EncryptionWorker encryptionWorker)
        {
            NotifySettings = notifySettings;
            ServiceProvider = serviceProvider;
            NotifyStop = notifyStop;
            EncryptionWorker = encryptionWorker;
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
            EncryptionWorker.Start(encryptionSettings);
        }

        public void StopEncryption()
        {
            EncryptionWorker.Stop();
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
