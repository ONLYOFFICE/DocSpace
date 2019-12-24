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

using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core.Common.EF.Context;
using ASC.Core.Data;
using ASC.Core.Tenants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASC.Core.Caching
{
    class TenantServiceCache
    {
        private const string KEY = "tenants";
        private TimeSpan CacheExpiration { get; set; }
        internal ICache Cache { get; }
        internal ICacheNotify<TenantCacheItem> CacheNotifyItem { get; }
        internal ICacheNotify<TenantSetting> CacheNotifySettings { get; }

        public TenantServiceCache(CoreBaseSettings coreBaseSettings, ICacheNotify<TenantCacheItem> cacheNotifyItem, ICacheNotify<TenantSetting> cacheNotifySettings)
        {
            CacheNotifyItem = cacheNotifyItem;
            CacheNotifySettings = cacheNotifySettings;
            Cache = AscCache.Memory;
            CacheExpiration = TimeSpan.FromMinutes(2);

            cacheNotifyItem.Subscribe((t) =>
            {
                var tenants = GetTenantStore();
                tenants.Remove(t.TenantId);
                tenants.Clear(coreBaseSettings);
            }, CacheNotifyAction.InsertOrUpdate);

            cacheNotifySettings.Subscribe((s) =>
            {
                Cache.Remove(s.Key);
            }, CacheNotifyAction.Remove);
        }

        internal TenantStore GetTenantStore()
        {
            var store = Cache.Get<TenantStore>(KEY);
            if (store == null)
            {
                Cache.Insert(KEY, store = new TenantStore(), DateTime.UtcNow.Add(CacheExpiration));
            }
            return store;
        }


        internal class TenantStore
        {
            private readonly Dictionary<int, Tenant> byId = new Dictionary<int, Tenant>();
            private readonly Dictionary<string, Tenant> byDomain = new Dictionary<string, Tenant>();
            private readonly object locker = new object();


            public Tenant Get(int id)
            {
                Tenant t;
                lock (locker)
                {
                    byId.TryGetValue(id, out t);
                }
                return t;
            }

            public Tenant Get(string domain)
            {
                if (string.IsNullOrEmpty(domain)) return null;

                Tenant t;
                lock (locker)
                {
                    byDomain.TryGetValue(domain, out t);
                }
                return t;
            }

            public void Insert(Tenant t, string ip = null)
            {
                if (t == null)
                {
                    return;
                }

                Remove(t.TenantId);
                lock (locker)
                {
                    byId[t.TenantId] = t;
                    byDomain[t.TenantAlias] = t;
                    if (!string.IsNullOrEmpty(t.MappedDomain)) byDomain[t.MappedDomain] = t;
                    if (!string.IsNullOrEmpty(ip)) byDomain[ip] = t;
                }
            }

            public void Remove(int id)
            {
                var t = Get(id);
                if (t != null)
                {
                    lock (locker)
                    {
                        byId.Remove(id);
                        byDomain.Remove(t.TenantAlias);
                        if (!string.IsNullOrEmpty(t.MappedDomain))
                        {
                            byDomain.Remove(t.MappedDomain);
                        }
                    }
                }
            }

            internal void Clear(CoreBaseSettings coreBaseSettings)
            {
                if (!coreBaseSettings.Standalone) return;
                lock (locker)
                {
                    byId.Clear();
                    byDomain.Clear();
                }
            }
        }
    }

    class CachedTenantService : ITenantService
    {
        private readonly ITenantService service;
        private readonly ICache cache;
        private readonly ICacheNotify<TenantSetting> cacheNotifySettings;
        private readonly ICacheNotify<TenantCacheItem> cacheNotifyItem;

        private TimeSpan SettingsExpiration { get; set; }
        private TenantServiceCache TenantServiceCache { get; }

        public CachedTenantService(DbTenantService service, TenantServiceCache tenantServiceCache)
        {
            this.service = service ?? throw new ArgumentNullException("service");
            cache = AscCache.Memory;
            SettingsExpiration = TimeSpan.FromMinutes(2);

            TenantServiceCache = tenantServiceCache;
            cacheNotifyItem = tenantServiceCache.CacheNotifyItem;
            cacheNotifySettings = tenantServiceCache.CacheNotifySettings;
        }


        public void ValidateDomain(string domain)
        {
            service.ValidateDomain(domain);
        }

        public IEnumerable<Tenant> GetTenants(string login, string passwordHash)
        {
            return service.GetTenants(login, passwordHash);
        }

        public IEnumerable<Tenant> GetTenants(DateTime from, bool active = true)
        {
            return service.GetTenants(from, active);
        }

        public Tenant GetTenant(int id)
        {
            var tenants = TenantServiceCache.GetTenantStore();
            var t = tenants.Get(id);
            if (t == null)
            {
                t = service.GetTenant(id);
                if (t != null)
                {
                    tenants.Insert(t);
                }
            }
            return t;
        }

        public Tenant GetTenant(string domain)
        {
            var tenants = TenantServiceCache.GetTenantStore();
            var t = tenants.Get(domain);
            if (t == null)
            {
                t = service.GetTenant(domain);
                if (t != null)
                {
                    tenants.Insert(t);
                }
            }
            return t;
        }

        public Tenant GetTenantForStandaloneWithoutAlias(string ip)
        {
            var tenants = TenantServiceCache.GetTenantStore();
            var t = tenants.Get(ip);
            if (t == null)
            {
                t = service.GetTenantForStandaloneWithoutAlias(ip);
                if (t != null)
                {
                    tenants.Insert(t, ip);
                }
            }
            return t;
        }

        public Tenant SaveTenant(CoreSettings coreSettings, Tenant tenant)
        {
            tenant = service.SaveTenant(coreSettings, tenant);
            cacheNotifyItem.Publish(new TenantCacheItem() { TenantId = tenant.TenantId }, CacheNotifyAction.InsertOrUpdate);
            return tenant;
        }

        public void RemoveTenant(int id, bool auto = false)
        {
            service.RemoveTenant(id, auto);
            cacheNotifyItem.Publish(new TenantCacheItem() { TenantId = id }, CacheNotifyAction.InsertOrUpdate);
        }

        public IEnumerable<TenantVersion> GetTenantVersions()
        {
            return service.GetTenantVersions();
        }

        public byte[] GetTenantSettings(int tenant, string key)
        {
            var cacheKey = string.Format("settings/{0}/{1}", tenant, key);
            var data = cache.Get<byte[]>(cacheKey);
            if (data == null)
            {
                data = service.GetTenantSettings(tenant, key);
                cache.Insert(cacheKey, data ?? new byte[0], DateTime.UtcNow + SettingsExpiration);
            }
            return data == null ? null : data.Length == 0 ? null : data;
        }

        public void SetTenantSettings(int tenant, string key, byte[] data)
        {
            service.SetTenantSettings(tenant, key, data);
            var cacheKey = string.Format("settings/{0}/{1}", tenant, key);
            cacheNotifySettings.Publish(new TenantSetting { Key = cacheKey }, CacheNotifyAction.Remove);
        }
    }

    public static class TenantConfigExtension
    {
        public static IServiceCollection AddTenantService(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(ICacheNotify<>), typeof(KafkaCache<>));
            services.TryAddSingleton<TenantDomainValidator>();
            services.TryAddSingleton<TimeZoneConverter>();
            services.TryAddSingleton<TenantServiceCache>();
            services.TryAddScoped<DbTenantService>();
            services.TryAddScoped<ITenantService, CachedTenantService>();

            return services
                .AddCoreBaseSettingsService()
                .AddTenantDbContextService();
        }
    }
}
