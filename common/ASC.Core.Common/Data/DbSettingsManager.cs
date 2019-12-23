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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Core.Data
{
    public class DbSettingsManagerCache
    {
        public ICache Cache { get; }
        public ICacheNotify<SettingsCacheItem> Notify { get; }

        public DbSettingsManagerCache(ICacheNotify<SettingsCacheItem> notify)
        {
            Cache = AscCache.Memory;
            Notify = notify;
            Notify.Subscribe((i) => Cache.Remove(i.Key), CacheNotifyAction.Remove);
        }

        public void Remove(string key)
        {
            Notify.Publish(new SettingsCacheItem { Key = key }, CacheNotifyAction.Remove);
        }
    }

    public class DbSettingsManager
    {
        private readonly ILog log;

        private readonly TimeSpan expirationTimeout = TimeSpan.FromMinutes(5);
        private readonly IDictionary<Type, DataContractJsonSerializer> jsonSerializers = new Dictionary<Type, DataContractJsonSerializer>();
        private readonly string dbId;

        private ICache Cache { get; }
        private IServiceProvider ServiceProvider { get; }
        private DbSettingsManagerCache DbSettingsManagerCache { get; }
        public AuthContext AuthContext { get; }
        public TenantManager TenantManager { get; }
        private WebstudioDbContext WebstudioDbContext { get; }

        public DbSettingsManager(
            IServiceProvider serviceProvider,
            DbSettingsManagerCache dbSettingsManagerCache,
            IOptionsMonitor<ILog> option,
            AuthContext authContext,
            TenantManager tenantManager,
            DbContextManager<WebstudioDbContext> dbContextManager) : this(null)
        {
            ServiceProvider = serviceProvider;
            DbSettingsManagerCache = dbSettingsManagerCache;
            AuthContext = authContext;
            TenantManager = tenantManager;
            Cache = dbSettingsManagerCache.Cache;
            log = option.CurrentValue;
            WebstudioDbContext = dbContextManager.Value;
        }

        //TODO: remove
        public DbSettingsManager(ConnectionStringSettings connectionString)
        {
            dbId = connectionString != null ? connectionString.Name : "default";
        }

        private int TenantID
        {
            get { return TenantManager.GetCurrentTenant().TenantId; }
        }
        //
        private Guid CurrentUserID
        {
            get { return AuthContext.CurrentAccount.ID; }
        }

        public bool SaveSettings<T>(T settings, int tenantId) where T : ISettings
        {
            return SaveSettingsFor(settings, tenantId, Guid.Empty);
        }

        public T LoadSettings<T>(int tenantId) where T : class, ISettings
        {
            return LoadSettingsFor<T>(tenantId, Guid.Empty);
        }

        public void ClearCache<T>(int tenantId) where T : class, ISettings
        {
            var settings = LoadSettings<T>(tenantId);
            var key = settings.ID.ToString() + tenantId + Guid.Empty;
            DbSettingsManagerCache.Remove(key);
        }


        public bool SaveSettingsFor<T>(T settings, int tenantId, Guid userId) where T : ISettings
        {
            if (settings == null) throw new ArgumentNullException("settings");
            try
            {
                var key = settings.ID.ToString() + tenantId + userId;
                var data = Serialize(settings);

                var def = (T)settings.GetDefault(ServiceProvider);

                var defaultData = Serialize(def);

                var tr = WebstudioDbContext.Database.BeginTransaction();

                if (data.SequenceEqual(defaultData))
                {
                    // remove default settings
                    var s = WebstudioDbContext.WebstudioSettings
                        .Where(r => r.Id == settings.ID)
                        .Where(r => r.TenantId == tenantId)
                        .Where(r => r.UserId == userId)
                        .FirstOrDefault();

                    WebstudioDbContext.WebstudioSettings.Remove(s);
                }
                else
                {
                    var s = new DbWebstudioSettings
                    {
                        Id = settings.ID,
                        UserId = userId,
                        TenantId = tenantId,
                        Data = data
                    };

                    WebstudioDbContext.AddOrUpdate(r => r.WebstudioSettings, s);
                }

                WebstudioDbContext.SaveChanges();
                tr.Commit();

                DbSettingsManagerCache.Remove(key);

                Cache.Insert(key, settings, expirationTimeout);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        internal T LoadSettingsFor<T>(int tenantId, Guid userId) where T : class, ISettings
        {
            var settingsInstance = Activator.CreateInstance<T>();
            var key = settingsInstance.ID.ToString() + tenantId + userId;
            var def = (T)settingsInstance.GetDefault(ServiceProvider);

            try
            {
                var settings = Cache.Get<T>(key);
                if (settings != null) return settings;

                var result = WebstudioDbContext.WebstudioSettings
                        .Where(r => r.Id == settingsInstance.ID)
                        .Where(r => r.TenantId == tenantId)
                        .Where(r => r.UserId == userId)
                        .Select(r => r.Data)
                        .FirstOrDefault();

                if (result != null)
                {
                    settings = Deserialize<T>(result.ToString());
                }
                else
                {
                    settings = def;
                }

                Cache.Insert(key, settings, expirationTimeout);
                return settings;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return def;
        }

        public T Load<T>() where T : class, ISettings
        {
            return LoadSettings<T>(TenantID);
        }

        public T LoadForCurrentUser<T>() where T : class, ISettings
        {
            return LoadForUser<T>(CurrentUserID);
        }

        public T LoadForUser<T>(Guid userId) where T : class, ISettings
        {
            return LoadSettingsFor<T>(TenantID, userId);
        }

        public T LoadForDefaultTenant<T>() where T : class, ISettings
        {
            return LoadForTenant<T>(Tenant.DEFAULT_TENANT);
        }

        public T LoadForTenant<T>(int tenantId) where T : class, ISettings
        {
            return LoadSettings<T>(tenantId);
        }

        public virtual bool Save<T>(T data) where T : class, ISettings
        {
            return SaveSettings(data, TenantID);
        }

        public bool SaveForCurrentUser<T>(T data) where T : class, ISettings
        {
            return SaveForUser(data, CurrentUserID);
        }

        public bool SaveForUser<T>(T data, Guid userId) where T : class, ISettings
        {
            return SaveSettingsFor(data, TenantID, userId);
        }

        public bool SaveForDefaultTenant<T>(T data) where T : class, ISettings
        {
            return SaveForTenant(data, Tenant.DEFAULT_TENANT);
        }

        public bool SaveForTenant<T>(T data, int tenantId) where T : class, ISettings
        {
            return SaveSettings(data, tenantId);
        }

        public void ClearCache<T>() where T : class, ISettings
        {
            ClearCache<T>(TenantID);
        }

        private T Deserialize<T>(string data)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            var settings = GetJsonSerializer(typeof(T)).ReadObject(stream);
            return (T)settings;
        }

        private string Serialize(ISettings settings)
        {
            using var stream = new MemoryStream();
            GetJsonSerializer(settings.GetType()).WriteObject(stream, settings);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private DataContractJsonSerializer GetJsonSerializer(Type type)
        {
            lock (jsonSerializers)
            {
                if (!jsonSerializers.ContainsKey(type))
                {
                    jsonSerializers[type] = new DataContractJsonSerializer(type);
                }
                return jsonSerializers[type];
            }
        }
    }

    public static class DbSettingsManagerExtension
    {
        public static IServiceCollection AddDbSettingsManagerService(this IServiceCollection services)
        {
            services.TryAddSingleton<DbSettingsManagerCache>();
            services.TryAddScoped<DbSettingsManager>();

            return services.AddWebstudioDbContextService();
        }
    }
}