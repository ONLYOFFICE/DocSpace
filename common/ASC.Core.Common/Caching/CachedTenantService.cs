namespace ASC.Core.Caching;

[Singletone]
class TenantServiceCache
{
    private const string Key = "tenants";
    private TimeSpan _cacheExpiration;
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
class ConfigureCachedTenantService : IConfigureNamedOptions<CachedTenantService>
{
    private readonly IOptionsSnapshot<DbTenantService> _service;
    private readonly TenantServiceCache _tenantServiceCache;

    public ConfigureCachedTenantService(
        IOptionsSnapshot<DbTenantService> service,
        TenantServiceCache tenantServiceCache)
    {
        _service = service;
        _tenantServiceCache = tenantServiceCache;
    }

    public void Configure(string name, CachedTenantService options)
    {
        Configure(options);
        options.Service = _service.Get(name);
    }

    public void Configure(CachedTenantService options)
    {
        options.Service = _service.Value;
        options.TenantServiceCache = _tenantServiceCache;
        options.CacheNotifyItem = _tenantServiceCache.CacheNotifyItem;
        options.CacheNotifySettings = _tenantServiceCache.CacheNotifySettings;
    }
}

[Scope]
class CachedTenantService : ITenantService
{
    internal ITenantService Service;
    private readonly ICache _cache;
    internal ICacheNotify<TenantSetting> CacheNotifySettings;
    internal ICacheNotify<TenantCacheItem> CacheNotifyItem;
    private TimeSpan _settingsExpiration;
    internal TenantServiceCache TenantServiceCache;

    public CachedTenantService()
    {
        _settingsExpiration = TimeSpan.FromMinutes(2);
    }

    public CachedTenantService(DbTenantService service, TenantServiceCache tenantServiceCache, ICache cache) : this()
    {
        this._cache = cache;
        Service = service ?? throw new ArgumentNullException(nameof(service));
        TenantServiceCache = tenantServiceCache;
        CacheNotifyItem = tenantServiceCache.CacheNotifyItem;
        CacheNotifySettings = tenantServiceCache.CacheNotifySettings;
    }

    public void ValidateDomain(string domain)
    {
        Service.ValidateDomain(domain);
    }

    public IEnumerable<Tenant> GetTenants(string login, string passwordHash)
    {
        return Service.GetTenants(login, passwordHash);
    }

    public IEnumerable<Tenant> GetTenants(DateTime from, bool active = true)
    {
        return Service.GetTenants(from, active);
    }
    public IEnumerable<Tenant> GetTenants(List<int> ids)
    {
        return Service.GetTenants(ids);
    }

    public Tenant GetTenant(int id)
    {
        var tenants = TenantServiceCache.GetTenantStore();
        var t = tenants.Get(id);
        if (t == null)
        {
            t = Service.GetTenant(id);
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
            t = Service.GetTenant(domain);
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
            t = Service.GetTenantForStandaloneWithoutAlias(ip);
            if (t != null)
            {
                tenants.Insert(t, ip);
            }
        }

        return t;
    }

    public Tenant SaveTenant(CoreSettings coreSettings, Tenant tenant)
    {
        tenant = Service.SaveTenant(coreSettings, tenant);
        CacheNotifyItem.Publish(new TenantCacheItem() { TenantId = tenant.Id }, CacheNotifyAction.InsertOrUpdate);

        return tenant;
    }

    public void RemoveTenant(int id, bool auto = false)
    {
        Service.RemoveTenant(id, auto);
        CacheNotifyItem.Publish(new TenantCacheItem() { TenantId = id }, CacheNotifyAction.InsertOrUpdate);
    }

    public IEnumerable<TenantVersion> GetTenantVersions()
    {
        return Service.GetTenantVersions();
    }

    public byte[] GetTenantSettings(int tenant, string key)
    {
        var cacheKey = string.Format("settings/{0}/{1}", tenant, key);
        var data = _cache.Get<byte[]>(cacheKey);
        if (data == null)
        {
            data = Service.GetTenantSettings(tenant, key);

            _cache.Insert(cacheKey, data ?? Array.Empty<byte>(), DateTime.UtcNow + _settingsExpiration);
        }

        return data == null ? null : data.Length == 0 ? null : data;
    }

    public void SetTenantSettings(int tenant, string key, byte[] data)
    {
        Service.SetTenantSettings(tenant, key, data);
        var cacheKey = string.Format("settings/{0}/{1}", tenant, key);

        CacheNotifySettings.Publish(new TenantSetting { Key = cacheKey }, ASC.Common.Caching.CacheNotifyAction.Remove);
    }
}
