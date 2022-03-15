namespace ASC.IPSecurity;

[Singletone]
public class IPRestrictionsServiceCache
{
    public ICache Cache { get; set; }

    private const string CacheKey = "iprestrictions";

    internal readonly ICacheNotify<IPRestrictionItem> Notify;

    public IPRestrictionsServiceCache(ICacheNotify<IPRestrictionItem> notify, ICache cache)
    {
        Cache = cache;
        notify.Subscribe((r) => Cache.Remove(GetCacheKey(r.TenantId)), CacheNotifyAction.Any);
        Notify = notify;
    }

    public static string GetCacheKey(int tenant)
    {
        return CacheKey + tenant;
    }
}

[Scope]
public class IPRestrictionsService
{
    private readonly ICache _cache;
    private readonly ICacheNotify<IPRestrictionItem> _notify;
    private readonly IPRestrictionsRepository _ipRestrictionsRepository;
    private static readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);

    public IPRestrictionsService(
        IPRestrictionsRepository iPRestrictionsRepository,
        IPRestrictionsServiceCache iPRestrictionsServiceCache)
    {
        _ipRestrictionsRepository = iPRestrictionsRepository;
        _cache = iPRestrictionsServiceCache.Cache;
        _notify = iPRestrictionsServiceCache.Notify;
    }

    public IEnumerable<IPRestriction> Get(int tenant)
    {
        var key = IPRestrictionsServiceCache.GetCacheKey(tenant);
        var restrictions = _cache.Get<List<IPRestriction>>(key);
        if (restrictions == null)
        {
            restrictions = _ipRestrictionsRepository.Get(tenant);
            _cache.Insert(key, restrictions, _timeout);
        }

        return restrictions;
    }

    public IEnumerable<string> Save(IEnumerable<string> ips, int tenant)
    {
        var restrictions = _ipRestrictionsRepository.Save(ips, tenant);
        _notify.Publish(new IPRestrictionItem { TenantId = tenant }, CacheNotifyAction.InsertOrUpdate);

        return restrictions;
    }
}
