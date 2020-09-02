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
        private IServiceProvider ServiceProvider { get; }

        public EncryptionServiceListener(IServiceProvider serviceProvider, ICacheNotify<EncryptionSettingsProto> notifySettings)
        {
            NotifySettings = notifySettings;
            ServiceProvider = serviceProvider;
        }

        public void Start()
        {
            NotifySettings.Subscribe((n) => StartEncryption(n), CacheNotifyAction.Insert);
        }

        public void Stop()
        {
            NotifySettings.Unsubscribe(CacheNotifyAction.Insert);
        }

        public void StartEncryption(EncryptionSettingsProto encryptionSettings)
        {
            using var scope = ServiceProvider.CreateScope();
            var encryptionService = scope.ServiceProvider.GetService<EncryptionService>();
            encryptionService.Start(encryptionSettings);
        }
    }

    public static class EncryptionServiceListenerExtension
    {
        public static DIHelper AddEncryptionServiceListener(this DIHelper services)
        {
            services.TryAddSingleton<EncryptionServiceListener>();
            return services.AddEncryptionService();
        }
    }
}   
