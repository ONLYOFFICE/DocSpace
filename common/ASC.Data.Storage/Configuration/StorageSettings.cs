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


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Settings;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Data.Storage.Configuration
{
    public class BaseStorageSettingsListener
    {
        public BaseStorageSettingsListener(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            serviceProvider.GetService<ICacheNotify<ConsumerCacheItem>>().Subscribe((i) =>
            {
                using var scope = ServiceProvider.CreateScope();

                var storageSettingsHelper = scope.ServiceProvider.GetService<StorageSettingsHelper>();
                var storageSettings = scope.ServiceProvider.GetService<SettingsManager>();
                var settings = storageSettings.LoadForTenant<StorageSettings>(i.TenantId);
                if (i.Name == settings.Module)
                {
                    storageSettingsHelper.Clear(settings);
                }

                var cdnStorageSettings = scope.ServiceProvider.GetService<CdnStorageSettings>();
                var cdnSettings = storageSettings.LoadForTenant<CdnStorageSettings>(i.TenantId);
                if (i.Name == cdnSettings.Module)
                {
                    storageSettingsHelper.Clear(cdnSettings);
                }
            }, CacheNotifyAction.Remove);
        }

        public IServiceProvider ServiceProvider { get; }
    }

    [Serializable]
    [DataContract]
    public abstract class BaseStorageSettings<T> : ISettings where T : class, ISettings, new()
    {
        [DataMember(Name = "Module")]
        public string Module { get; set; }

        [DataMember(Name = "Props")]
        public Dictionary<string, string> Props { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider) => new T();
        public virtual Func<DataStoreConsumer, DataStoreConsumer> Switch { get { return d => d; } }

        public ICacheNotify<DataStoreCacheItem> Cache { get; internal set; }

        public abstract Guid ID { get; }
    }

    [Serializable]
    [DataContract]
    public class StorageSettings : BaseStorageSettings<StorageSettings>
    {
        public override Guid ID
        {
            get { return new Guid("F13EAF2D-FA53-44F1-A6D6-A5AEDA46FA2B"); }
        }
    }

    [Serializable]
    [DataContract]
    public class CdnStorageSettings : BaseStorageSettings<CdnStorageSettings>
    {
        public override Guid ID
        {
            get { return new Guid("0E9AE034-F398-42FE-B5EE-F86D954E9FB2"); }
        }

        public override Func<DataStoreConsumer, DataStoreConsumer> Switch { get { return d => d.Cdn; } }
    }

    public class StorageSettingsHelper
    {
        public BaseStorageSettingsListener BaseStorageSettingsListener { get; }
        public StorageFactoryConfig StorageFactoryConfig { get; }
        public PathUtils PathUtils { get; }
        public EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        public ICacheNotify<DataStoreCacheItem> Cache { get; }
        public IOptionsMonitor<ILog> Options { get; }
        public TenantManager TenantManager { get; }
        public SettingsManager SettingsManager { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public ConsumerFactory ConsumerFactory { get; }

        public StorageSettingsHelper(
            BaseStorageSettingsListener baseStorageSettingsListener,
            StorageFactoryConfig storageFactoryConfig,
            PathUtils pathUtils,
            EmailValidationKeyProvider emailValidationKeyProvider,
            ICacheNotify<DataStoreCacheItem> cache,
            IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            IHttpContextAccessor httpContextAccessor,
            ConsumerFactory consumerFactory)
        {
            BaseStorageSettingsListener = baseStorageSettingsListener;
            StorageFactoryConfig = storageFactoryConfig;
            PathUtils = pathUtils;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            Cache = cache;
            Options = options;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            HttpContextAccessor = httpContextAccessor;
            ConsumerFactory = consumerFactory;
        }

        public bool Save<T>(BaseStorageSettings<T> baseStorageSettings) where T : class, ISettings, new()
        {
            ClearDataStoreCache();
            dataStoreConsumer = null;
            return SettingsManager.Save(baseStorageSettings);
        }

        internal void ClearDataStoreCache()
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId.ToString();
            var path = TennantPath.CreatePath(tenantId);
            foreach (var module in StorageFactoryConfig.GetModuleList("", true))
            {
                Cache.Publish(new DataStoreCacheItem() { TenantId = path, Module = module }, CacheNotifyAction.Remove);
            }
        }

        public void Clear<T>(BaseStorageSettings<T> baseStorageSettings) where T : class, ISettings, new()
        {
            baseStorageSettings.Module = null;
            baseStorageSettings.Props = null;
            Save(baseStorageSettings);
        }

        private DataStoreConsumer dataStoreConsumer;
        public DataStoreConsumer DataStoreConsumer<T>(BaseStorageSettings<T> baseStorageSettings) where T : class, ISettings, new()
        {
            if (string.IsNullOrEmpty(baseStorageSettings.Module) || baseStorageSettings.Props == null) return dataStoreConsumer = new DataStoreConsumer();

            var consumer = ConsumerFactory.GetByKey<DataStoreConsumer>(baseStorageSettings.Module);

            if (!consumer.IsSet) return dataStoreConsumer = new DataStoreConsumer();

            dataStoreConsumer = (DataStoreConsumer)consumer.Clone();

            foreach (var prop in baseStorageSettings.Props)
            {
                dataStoreConsumer[prop.Key] = prop.Value;
            }

            return dataStoreConsumer;
        }

        private IDataStore dataStore;
        public IDataStore DataStore<T>(BaseStorageSettings<T> baseStorageSettings) where T : class, ISettings, new()
        {
            if (dataStore != null) return dataStore;

            if (DataStoreConsumer(baseStorageSettings).HandlerType == null) return null;

            return dataStore = ((IDataStore)
                Activator.CreateInstance(DataStoreConsumer(baseStorageSettings).HandlerType, TenantManager, PathUtils, HttpContextAccessor, Options))
                .Configure(TenantManager.GetCurrentTenant().TenantId.ToString(), null, null, DataStoreConsumer(baseStorageSettings));
        }
    }

    public static class StorageSettingsExtension
    {
        public static IServiceCollection AddBaseStorageSettingsService(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(ICacheNotify<>), typeof(KafkaCache<>));
            services.TryAddSingleton<BaseStorageSettingsListener>();

            return services
                .AddStorageFactoryConfigService()
                .AddPathUtilsService()
                .AddEmailValidationKeyProviderService()
                .AddHttpContextAccessor();
        }

        public static IServiceCollection AddCdnStorageSettingsService(this IServiceCollection services)
        {
            services.TryAddScoped<StorageSettingsHelper>();

            return services
                .AddSettingsManagerService()
                .AddBaseStorageSettingsService()
                .AddConsumerFactoryService();
        }

        public static IServiceCollection AddStorageSettingsService(this IServiceCollection services)
        {
            services.TryAddScoped<StorageSettingsHelper>();

            return services
                .AddSettingsManagerService()
                .AddBaseStorageSettingsService()
                .AddConsumerFactoryService();
        }
    }
}
