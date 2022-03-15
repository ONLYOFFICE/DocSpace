namespace ASC.Core.Caching;

[Singletone]
class QuotaServiceCache
{
    internal const string KeyQuota = "quota";
    internal const string KeyQuotaRows = "quotarows";

    internal TrustInterval Interval;
    internal readonly ICache Cache;
    internal readonly ICacheNotify<QuotaCacheItem> CacheNotify;
    internal readonly bool QuotaCacheEnabled;

    public QuotaServiceCache(IConfiguration Configuration, ICacheNotify<QuotaCacheItem> cacheNotify, ICache cache)
    {
        if (Configuration["core:enable-quota-cache"] == null)
        {
            QuotaCacheEnabled = true;
        }
        else
        {
            QuotaCacheEnabled = !bool.TryParse(Configuration["core:enable-quota-cache"], out var enabled) || enabled;
        }

        CacheNotify = cacheNotify;
        Cache = cache;
        Interval = new TrustInterval();

        cacheNotify.Subscribe((i) =>
        {
            if (i.Key == KeyQuota)
            {
                Cache.Remove(KeyQuota);
            }
            else
            {
                Cache.Remove(i.Key);
            }
        }, CacheNotifyAction.Any);
    }
}

[Scope]
class ConfigureCachedQuotaService : IConfigureNamedOptions<CachedQuotaService>
{
    private readonly IOptionsSnapshot<DbQuotaService> _service;
    private readonly QuotaServiceCache _quotaServiceCache;

    public ConfigureCachedQuotaService(
        IOptionsSnapshot<DbQuotaService> service,
        QuotaServiceCache quotaServiceCache)
    {
        _service = service;
        _quotaServiceCache = quotaServiceCache;
    }

    public void Configure(string name, CachedQuotaService options)
    {
        Configure(options);
        options.Service = _service.Get(name);
    }

    public void Configure(CachedQuotaService options)
    {
        options.Service = _service.Value;
        options.QuotaServiceCache = _quotaServiceCache;
        options.Cache = _quotaServiceCache.Cache;
        options.CacheNotify = _quotaServiceCache.CacheNotify;
    }
}

[Scope]
class CachedQuotaService : IQuotaService
{
    internal IQuotaService Service;
    internal ICache Cache;
    internal ICacheNotify<QuotaCacheItem> CacheNotify;
    internal TrustInterval Interval;

    internal TimeSpan CacheExpiration;
    internal QuotaServiceCache QuotaServiceCache;

    public CachedQuotaService()
    {
        Interval = new TrustInterval();
        CacheExpiration = TimeSpan.FromMinutes(10);
    }

    public CachedQuotaService(DbQuotaService service, QuotaServiceCache quotaServiceCache) : this()
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        QuotaServiceCache = quotaServiceCache;
        Cache = quotaServiceCache.Cache;
        CacheNotify = quotaServiceCache.CacheNotify;
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        var quotas = Cache.Get<IEnumerable<TenantQuota>>(QuotaServiceCache.KeyQuota);
        if (quotas == null)
        {
            quotas = Service.GetTenantQuotas();
            if (QuotaServiceCache.QuotaCacheEnabled)
            {
                Cache.Insert(QuotaServiceCache.KeyQuota, quotas, DateTime.UtcNow.Add(CacheExpiration));
            }
        }

        return quotas;
    }

    public TenantQuota GetTenantQuota(int tenant)
    {
        return GetTenantQuotas().SingleOrDefault(q => q.Tenant == tenant);
    }

    public TenantQuota SaveTenantQuota(TenantQuota quota)
    {
        var q = Service.SaveTenantQuota(quota);
        CacheNotify.Publish(new QuotaCacheItem { Key = QuotaServiceCache.KeyQuota }, CacheNotifyAction.Any);

        return q;
    }

    public void RemoveTenantQuota(int tenant)
    {
        throw new NotImplementedException();
    }

    public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
    {
        Service.SetTenantQuotaRow(row, exchange);
        CacheNotify.Publish(new QuotaCacheItem { Key = GetKey(row.Tenant) }, CacheNotifyAction.InsertOrUpdate);
    }

    public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
    {
        var key = GetKey(tenantId);
        var result = Cache.Get<IEnumerable<TenantQuotaRow>>(key);

        if (result == null)
        {
            result = Service.FindTenantQuotaRows(tenantId);
            Cache.Insert(key, result, DateTime.UtcNow.Add(CacheExpiration));
        }

        return result;
    }

    public string GetKey(int tenant)
    {
        return QuotaServiceCache.KeyQuotaRows + tenant;
    }
}
