namespace ASC.Collections;

public sealed class HttpRequestDictionary<T> : CachedDictionaryBase<T>
{
    private readonly HttpContext _httpContext;

    public HttpRequestDictionary(HttpContext httpContext, string baseKey)
    {
        Condition = (T) => true;
        BaseKey = baseKey;
        _httpContext = httpContext;
    }

    public override void Reset(string rootKey, string key)
    {
        if (_httpContext != null)
        {
            var builtkey = BuildKey(key, rootKey);
            _httpContext.Items[builtkey] = null;
        }
    }

    public override void Add(string rootkey, string key, T newValue)
    {
        if (_httpContext != null)
        {
            var builtkey = BuildKey(key, rootkey);
            _httpContext.Items[builtkey] = new CachedItem(newValue);
        }
    }

    protected override object GetObjectFromCache(string fullKey)
    {
        return _httpContext?.Items[fullKey];
    }

    protected override bool FitsCondition(object cached)
    {
        return cached is CachedItem;
    }

    protected override T ReturnCached(object objectCache)
    {
        return ((CachedItem)objectCache).Value;
    }

    protected override void OnHit(string fullKey) { }

    protected override void OnMiss(string fullKey) { }

    protected override void InsertRootKey(string rootKey)
    {
        //We can't expire in HtppContext in such way
    }

    private sealed class CachedItem
    {
        internal T Value { get; set; }

        internal CachedItem(T value)
        {
            Value = value;
        }
    }
}