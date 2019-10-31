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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;

using ASC.Common.Caching;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Logging;
using ASC.Core.Common.Settings;

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
        private DbManager DbManager { get; }

        public DbSettingsManager(
            IServiceProvider serviceProvider,
            DbSettingsManagerCache dbSettingsManagerCache,
            DbOptionsManager optionsDbManager,
            IOptionsMonitor<LogNLog> option) : this(null)
        {
            ServiceProvider = serviceProvider;
            DbSettingsManagerCache = dbSettingsManagerCache;
            Cache = dbSettingsManagerCache.Cache;
            DbManager = optionsDbManager.Value;
            log = option.Get("ASC");
        }

        public DbSettingsManager(ConnectionStringSettings connectionString)
        {
            dbId = connectionString != null ? connectionString.Name : "default";
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

                var db = DbManager;

                var defaultData = Serialize(settings.GetDefault());

                ISqlInstruction i;
                if (data.SequenceEqual(defaultData))
                {
                    // remove default settings
                    i = new SqlDelete("webstudio_settings")
                        .Where("id", settings.ID.ToString())
                        .Where("tenantid", tenantId)
                        .Where("userid", userId.ToString());
                }
                else
                {
                    i = new SqlInsert("webstudio_settings", true)
                        .InColumnValue("id", settings.ID.ToString())
                        .InColumnValue("userid", userId.ToString())
                        .InColumnValue("tenantid", tenantId)
                        .InColumnValue("data", data);
                }

                DbSettingsManagerCache.Remove(key);

                db.ExecuteNonQuery(i);

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
            var settingsInstance = (ISettings)ServiceProvider.GetService<T>();
            var key = settingsInstance.ID.ToString() + tenantId + userId;

            try
            {
                var settings = Cache.Get<T>(key);
                if (settings != null) return settings;

                var db = DbManager;
                var q = new SqlQuery("webstudio_settings")
                    .Select("data")
                    .Where("id", settingsInstance.ID.ToString())
                    .Where("tenantid", tenantId)
                    .Where("userid", userId.ToString());

                var result = db.ExecuteScalar<object>(q);
                if (result != null)
                {
                    var data = result is string ? Encoding.UTF8.GetBytes((string)result) : (byte[])result;
                    settings = Deserialize<T>(data);
                }
                else
                {
                    settings = (T)settingsInstance.GetDefault();
                }

                Cache.Insert(key, settings, expirationTimeout);
                return settings;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return (T)settingsInstance.GetDefault();
        }

        private T Deserialize<T>(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var settings = data[0] == 0
                            ? new BinaryFormatter().Deserialize(stream)
                            : GetJsonSerializer(typeof(T)).ReadObject(stream);
            return (T)settings;
        }

        private byte[] Serialize(ISettings settings)
        {
            using var stream = new MemoryStream();
            GetJsonSerializer(settings.GetType()).WriteObject(stream, settings);
            return stream.ToArray();
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

    public static class DbSettingsManagerFactory
    {
        public static IServiceCollection AddDbSettingsManagerService(this IServiceCollection services)
        {
            services.TryAddSingleton<DbSettingsManagerCache>();
            services.TryAddScoped<DbSettingsManager>();

            return services.AddDbManagerService();
        }
    }
}