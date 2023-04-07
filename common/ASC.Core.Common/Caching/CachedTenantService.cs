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

namespace ASC.Core.Caching;

[Singletone]
class TenantServiceCache
{
    private const string Key = "tenants";
    private readonly TimeSpan _cacheExpiration;
    internal readonly ICache Cache;
    internal readonly ICacheNotify<TenantCacheItem> CacheNotifyItem;
    internal readonly ICacheNotify<TenantSetting> CacheNotifySettings;

    public TenantServiceCache(
        CoreBaseSettings coreBaseSettings,
        ICacheNotify<TenantCacheItem> cacheNotifyItem,
        ICacheNotify<TenantSetting> cacheNotifySettings,
        ICache cache)
    {
        CacheNotifyItem = cacheNotifyItem;
        CacheNotifySettings = cacheNotifySettings;
        Cache = cache;
        _cacheExpiration = TimeSpan.FromMinutes(2);

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
        var store = Cache.Get<TenantStore>(Key);
        if (store == null)
        {
            store = new TenantStore();
            Cache.Insert(Key, store, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return store;
    }

    internal class TenantStore
    {
        private readonly Dictionary<int, Tenant> _byId = new Dictionary<int, Tenant>();
        private readonly Dictionary<string, Tenant> _byDomain = new Dictionary<string, Tenant>();
        private readonly object _locker = new object();

        public Tenant Get(int id)
        {
            Tenant t;
            lock (_locker)
            {
                _byId.TryGetValue(id, out t);
            }

            return t;
        }

        public Tenant Get(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            Tenant t;
            lock (_locker)
            {
                _byDomain.TryGetValue(domain, out t);
            }

            return t;
        }

        public void Insert(Tenant t, string ip = null)
        {
            if (t == null)
            {
                return;
            }

            Remove(t.Id);
            lock (_locker)
            {
                _byId[t.Id] = t;
                _byDomain[t.Alias] = t;
                if (!string.IsNullOrEmpty(t.MappedDomain))
                {
                    _byDomain[t.MappedDomain] = t;
                }

                if (!string.IsNullOrEmpty(ip))
                {
                    _byDomain[ip] = t;
                }
            }
        }

        public void Remove(int id)
        {
            var t = Get(id);
            if (t != null)
            {
                lock (_locker)
                {
                    _byId.Remove(id);
                    _byDomain.Remove(t.Alias);
                    if (!string.IsNullOrEmpty(t.MappedDomain))
                    {
                        _byDomain.Remove(t.MappedDomain);
                    }
                }
            }
        }

        internal void Clear(CoreBaseSettings coreBaseSettings)
        {
            if (!coreBaseSettings.Standalone)
            {
                return;
            }

            lock (_locker)
            {
                _byId.Clear();
                _byDomain.Clear();
            }
        }
    }
}

[Scope]
class CachedTenantService : ITenantService
{
    private readonly ITenantService _service;
    private readonly ICacheNotify<TenantSetting> _cacheNotifySettings;
    private readonly ICacheNotify<TenantCacheItem> _cacheNotifyItem;
    private readonly TenantServiceCache _tenantServiceCache;
    private readonly TimeSpan _settingsExpiration;
    private readonly ICache _cache;

    public CachedTenantService()
    {
        _settingsExpiration = TimeSpan.FromMinutes(2);
    }

    public CachedTenantService(DbTenantService service, TenantServiceCache tenantServiceCache, ICache cache) : this()
    {
        _cache = cache;
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _tenantServiceCache = tenantServiceCache;
        _cacheNotifyItem = tenantServiceCache.CacheNotifyItem;
        _cacheNotifySettings = tenantServiceCache.CacheNotifySettings;
    }

    public async Task ValidateDomainAsync(string domain)
    {
        await _service.ValidateDomainAsync(domain);
    }

    public async Task<IEnumerable<Tenant>> GetTenantsAsync(string login, string passwordHash)
    {
        return await _service.GetTenantsAsync(login, passwordHash);
    }

    public async Task<IEnumerable<Tenant>> GetTenantsAsync(DateTime from, bool active = true)
    {
        return await _service.GetTenantsAsync(from, active);
    }

    public async Task<IEnumerable<Tenant>> GetTenantsAsync(List<int> ids)
    {
        return await _service.GetTenantsAsync(ids);
    }

    public async Task<Tenant> GetTenantAsync(int id)
    {
        var tenants = _tenantServiceCache.GetTenantStore();
        var t = tenants.Get(id);
        if (t == null)
        {
            t = await _service.GetTenantAsync(id);
            if (t != null)
            {
                tenants.Insert(t);
            }
        }

        return t;
    }

    public Tenant GetTenant(int id)
    {
        var tenants = _tenantServiceCache.GetTenantStore();
        var t = tenants.Get(id);
        if (t == null)
        {
            t = _service.GetTenant(id);
            if (t != null)
            {
                tenants.Insert(t);
            }
        }

        return t;
    }

    public async Task<Tenant> GetTenantAsync(string domain)
    {
        var tenants = _tenantServiceCache.GetTenantStore();
        var t = tenants.Get(domain);
        if (t == null)
        {
            t = await _service.GetTenantAsync(domain);
            if (t != null)
            {
                tenants.Insert(t);
            }
        }

        return t;
    }

    public Tenant GetTenant(string domain)
    {
        var tenants = _tenantServiceCache.GetTenantStore();
        var t = tenants.Get(domain);
        if (t == null)
        {
            t = _service.GetTenant(domain);
            if (t != null)
            {
                tenants.Insert(t);
            }
        }

        return t;
    }

    public Tenant GetTenantForStandaloneWithoutAlias(string ip)
    {
        var tenants = _tenantServiceCache.GetTenantStore();
        var t = tenants.Get(ip);
        if (t == null)
        {
            t = _service.GetTenantForStandaloneWithoutAlias(ip);
            if (t != null)
            {
                tenants.Insert(t, ip);
            }
        }

        return t;
    }

    public async Task<Tenant> GetTenantForStandaloneWithoutAliasAsync(string ip)
    {
        var tenants = _tenantServiceCache.GetTenantStore();
        var t = tenants.Get(ip);
        if (t == null)
        {
            t = await _service.GetTenantForStandaloneWithoutAliasAsync(ip);
            if (t != null)
            {
                tenants.Insert(t, ip);
            }
        }

        return t;
    }

    public async Task<Tenant> SaveTenantAsync(CoreSettings coreSettings, Tenant tenant)
    {
        tenant = await _service.SaveTenantAsync(coreSettings, tenant);
        _cacheNotifyItem.Publish(new TenantCacheItem() { TenantId = tenant.Id }, CacheNotifyAction.InsertOrUpdate);

        return tenant;
    }

    public async Task RemoveTenantAsync(int id, bool auto = false)
    {
        await _service.RemoveTenantAsync(id, auto);
        _cacheNotifyItem.Publish(new TenantCacheItem() { TenantId = id }, CacheNotifyAction.InsertOrUpdate);
    }

    public async Task<IEnumerable<TenantVersion>> GetTenantVersionsAsync()
    {
        return await _service.GetTenantVersionsAsync();
    }

    public async Task<byte[]> GetTenantSettingsAsync(int tenant, string key)
    {
        var cacheKey = string.Format("settings/{0}/{1}", tenant, key);
        var data = _cache.Get<byte[]>(cacheKey);
        if (data == null)
        {
            data = await _service.GetTenantSettingsAsync(tenant, key);

            _cache.Insert(cacheKey, data ?? Array.Empty<byte>(), DateTime.UtcNow + _settingsExpiration);
        }

        return data == null ? null : data.Length == 0 ? null : data;
    }

    public byte[] GetTenantSettings(int tenant, string key)
    {
        var cacheKey = string.Format("settings/{0}/{1}", tenant, key);
        var data = _cache.Get<byte[]>(cacheKey);
        if (data == null)
        {
            data = _service.GetTenantSettings(tenant, key);

            _cache.Insert(cacheKey, data ?? Array.Empty<byte>(), DateTime.UtcNow + _settingsExpiration);
        }

        return data == null ? null : data.Length == 0 ? null : data;
    }

    public async Task SetTenantSettingsAsync(int tenant, string key, byte[] data)
    {
        await _service.SetTenantSettingsAsync(tenant, key, data);
        var cacheKey = string.Format("settings/{0}/{1}", tenant, key);

        _cacheNotifySettings.Publish(new TenantSetting { Key = cacheKey }, CacheNotifyAction.Remove);
    }

    public void SetTenantSettings(int tenant, string key, byte[] data)
    {
        _service.SetTenantSettings(tenant, key, data);
        var cacheKey = string.Format("settings/{0}/{1}", tenant, key);

        _cacheNotifySettings.Publish(new TenantSetting { Key = cacheKey }, CacheNotifyAction.Remove);
    }
}
