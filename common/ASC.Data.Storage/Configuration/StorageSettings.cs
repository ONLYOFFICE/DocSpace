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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Settings;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Storage.Configuration
{
    [Singletone(Additional = typeof(StorageSettingsExtension))]
    public class BaseStorageSettingsListener
    {
        private IServiceProvider ServiceProvider { get; }
        private volatile bool Subscribed;
        private readonly object locker;

        public BaseStorageSettingsListener(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            locker = new object();
        }

        public void Subscribe()
        {
            if (Subscribed) return;

            lock (locker)
            {
                if (Subscribed) return;

                Subscribed = true;

                ServiceProvider.GetService<ICacheNotify<ConsumerCacheItem>>().Subscribe((i) =>
                {
                    using var scope = ServiceProvider.CreateScope();

                    var scopeClass = scope.ServiceProvider.GetService<BaseStorageSettingsListenerScope>();
                    var (storageSettingsHelper, settingsManager, cdnStorageSettings) = scopeClass;
                    var settings = settingsManager.LoadForTenant<StorageSettings>(i.TenantId);
                    if (i.Name == settings.Module)
                    {
                        storageSettingsHelper.Clear(settings);
                    }

                    var cdnSettings = settingsManager.LoadForTenant<CdnStorageSettings>(i.TenantId);
                    if (i.Name == cdnSettings.Module)
                    {
                        storageSettingsHelper.Clear(cdnSettings);
                    }
                }, CacheNotifyAction.Remove);
            }
        }
    }

    [Serializable]
    public abstract class BaseStorageSettings<T> : ISettings where T : class, ISettings, new()
    {
        public string Module { get; set; }

        public Dictionary<string, string> Props { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new T();
        }

        public virtual Func<DataStoreConsumer, DataStoreConsumer> Switch { get { return d => d; } }

        internal ICacheNotify<DataStoreCacheItem> Cache { get; set; }

        public abstract Guid ID { get; }
    }

    [Serializable]
    public class StorageSettings : BaseStorageSettings<StorageSettings>
    {
        public override Guid ID
        {
            get { return new Guid("F13EAF2D-FA53-44F1-A6D6-A5AEDA46FA2B"); }
        }
    }

    [Scope]
    [Serializable]
    public class CdnStorageSettings : BaseStorageSettings<CdnStorageSettings>
    {
        public override Guid ID
        {
            get { return new Guid("0E9AE034-F398-42FE-B5EE-F86D954E9FB2"); }
        }

        public override Func<DataStoreConsumer, DataStoreConsumer> Switch { get { return d => d.Cdn; } }
    }

    [Scope]
    public class StorageSettingsHelper
    {
        private StorageFactoryConfig StorageFactoryConfig { get; }
        private PathUtils PathUtils { get; }
        private ICacheNotify<DataStoreCacheItem> Cache { get; }
        private IOptionsMonitor<ILog> Options { get; }
        private TenantManager TenantManager { get; }
        private SettingsManager SettingsManager { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private ConsumerFactory ConsumerFactory { get; }
        private IServiceProvider ServiceProvider { get; }

        public StorageSettingsHelper(
            BaseStorageSettingsListener baseStorageSettingsListener,
            StorageFactoryConfig storageFactoryConfig,
            PathUtils pathUtils,
            ICacheNotify<DataStoreCacheItem> cache,
            IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            ConsumerFactory consumerFactory,
            IServiceProvider serviceProvider)
        {
            baseStorageSettingsListener.Subscribe();
            StorageFactoryConfig = storageFactoryConfig;
            PathUtils = pathUtils;
            Cache = cache;
            Options = options;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            ConsumerFactory = consumerFactory;
            ServiceProvider = serviceProvider;
        }
        public StorageSettingsHelper(
            BaseStorageSettingsListener baseStorageSettingsListener,
            StorageFactoryConfig storageFactoryConfig,
            PathUtils pathUtils,
            ICacheNotify<DataStoreCacheItem> cache,
            IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            IHttpContextAccessor httpContextAccessor,
            ConsumerFactory consumerFactory,
            IServiceProvider serviceProvider)
            : this(baseStorageSettingsListener, storageFactoryConfig, pathUtils, cache, options, tenantManager, settingsManager, consumerFactory, serviceProvider)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public bool Save<T>(BaseStorageSettings<T> baseStorageSettings) where T : class, ISettings, new()
        {
            ClearDataStoreCache();
            return SettingsManager.Save(baseStorageSettings);
        }

        internal void ClearDataStoreCache()
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId.ToString();
            var path = TenantPath.CreatePath(tenantId);
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

        public DataStoreConsumer DataStoreConsumer<T>(BaseStorageSettings<T> baseStorageSettings) where T : class, ISettings, new()
        {
            if (string.IsNullOrEmpty(baseStorageSettings.Module) || baseStorageSettings.Props == null) return new DataStoreConsumer();

            var consumer = ConsumerFactory.GetByKey<DataStoreConsumer>(baseStorageSettings.Module);

            if (!consumer.IsSet) return new DataStoreConsumer();

            var dataStoreConsumer = (DataStoreConsumer)consumer.Clone();

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

            return dataStore = ((IDataStore)ServiceProvider.GetService(DataStoreConsumer(baseStorageSettings).HandlerType))
                .Configure(TenantManager.GetCurrentTenant().TenantId.ToString(), null, null, DataStoreConsumer(baseStorageSettings));
        }
    }

    [Scope]
    public class BaseStorageSettingsListenerScope
    {
        private StorageSettingsHelper StorageSettingsHelper { get; }
        private SettingsManager SettingsManager { get; }
        private CdnStorageSettings CdnStorageSettings { get; }

        public BaseStorageSettingsListenerScope(StorageSettingsHelper storageSettingsHelper, SettingsManager settingsManager, CdnStorageSettings cdnStorageSettings)
        {
            StorageSettingsHelper = storageSettingsHelper;
            SettingsManager = settingsManager;
            CdnStorageSettings = cdnStorageSettings;
        }

        public void Deconstruct(out StorageSettingsHelper storageSettingsHelper, out SettingsManager settingsManager, out CdnStorageSettings cdnStorageSettings)
        {
            storageSettingsHelper = StorageSettingsHelper;
            settingsManager = SettingsManager;
            cdnStorageSettings = CdnStorageSettings;
        }
    }

    public static class StorageSettingsExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<BaseStorageSettingsListenerScope>();
        }
    }
}
