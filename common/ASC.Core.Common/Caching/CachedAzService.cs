namespace ASC.Core.Caching;

[Singletone]
class AzServiceCache
{
    internal readonly ICache Cache;
    internal readonly ICacheNotify<AzRecordCache> CacheNotify;

    public AzServiceCache(ICacheNotify<AzRecordCache> cacheNotify, ICache cache)
    {
        CacheNotify = cacheNotify;
        Cache = cache;

        cacheNotify.Subscribe((r) => UpdateCache(r, true), CacheNotifyAction.Remove);
        cacheNotify.Subscribe((r) => UpdateCache(r, false), CacheNotifyAction.InsertOrUpdate);
    }

    private void UpdateCache(AzRecord r, bool remove)
    {
        var aces = Cache.Get<AzRecordStore>(GetKey(r.Tenant));
        if (aces != null)
        {
            lock (aces)
            {
                if (remove)
                {
                    aces.Remove(r);
                }
                else
                {
                    aces.Add(r);
                }
            }
        }
    }

    public static string GetKey(int tenant)
    {
        return "acl" + tenant.ToString();
    }
}

[Scope]
class CachedAzService : IAzService
{
    private readonly IAzService _service;
    private readonly ICacheNotify<AzRecordCache> _cacheNotify;
    private readonly ICache _cache;
    private readonly TimeSpan _cacheExpiration;


    public CachedAzService(DbAzService service, AzServiceCache azServiceCache)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _cache = azServiceCache.Cache;
        _cacheNotify = azServiceCache.CacheNotify;
        _cacheExpiration = TimeSpan.FromMinutes(10);
    }


    public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
    {
        var key = AzServiceCache.GetKey(tenant);
        var aces = _cache.Get<AzRecordStore>(key);
        if (aces == null)
        {
            var records = _service.GetAces(tenant, default);
            aces = new AzRecordStore(records);
            _cache.Insert(key, aces, DateTime.UtcNow.Add(_cacheExpiration));
        }

        return aces;
    }

    public AzRecord SaveAce(int tenant, AzRecord r)
    {
        r = _service.SaveAce(tenant, r);
        _cacheNotify.Publish(r, CacheNotifyAction.InsertOrUpdate);

        return r;
    }

    public void RemoveAce(int tenant, AzRecord r)
    {
        _service.RemoveAce(tenant, r);
        _cacheNotify.Publish(r, CacheNotifyAction.Remove);
    }
}
