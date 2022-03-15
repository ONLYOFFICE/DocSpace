namespace ASC.Collections;

public abstract class CachedDictionaryBase<T>
{
    protected string BaseKey { get; set; }
    protected Func<T, bool> Condition { get; set; }

    public T this[string key] => Get(key);

    public T this[Func<T> @default] => Get(@default);

    protected abstract void InsertRootKey(string rootKey);

    public void Clear()
    {
        InsertRootKey(BaseKey);
    }

    public void Clear(string rootKey)
    {
        InsertRootKey(BuildKey(string.Empty, rootKey));
    }

    public void Reset(string key)
    {
        Reset(string.Empty, key);
    }

    public T Get(string key)
    {
        return Get(string.Empty, key, null);
    }

    public T Get(string key, Func<T> defaults)
    {
        return Get(string.Empty, key, defaults);
    }

    public void Add(string key, T newValue)
    {
        Add(string.Empty, key, newValue);
    }

    public bool HasItem(string key)
    {
        return !Equals(Get(key), default(T));
    }

    public T Get(Func<T> @default)
    {
        var key = string.Format("func {0} {2}.{1}({3})", @default.Method.ReturnType, @default.Method.Name,
                                   @default.Method.DeclaringType.FullName,
                                   string.Join(",",
                                               @default.Method.GetGenericArguments().Select(x => x.FullName).ToArray
                                                   ()));
        return Get(key, @default);
    }

    public virtual T Get(string rootkey, string key, Func<T> defaults)
    {
        var fullKey = BuildKey(key, rootkey);
        var objectCache = GetObjectFromCache(fullKey);

        if (FitsCondition(objectCache))
        {
            OnHit(fullKey);

            return ReturnCached(objectCache);
        }

        if (defaults != null)
        {
            OnMiss(fullKey);
            var newValue = defaults();

            if (Condition == null || Condition(newValue))
                Add(rootkey, key, newValue);

            return newValue;
        }

        return default;
    }

    public abstract void Add(string rootkey, string key, T newValue);

    public abstract void Reset(string rootKey, string key);

    protected virtual bool FitsCondition(object cached)
    {
        return cached is T;
    }

    protected virtual T ReturnCached(object objectCache)
    {
        return (T)objectCache;
    }

    protected string BuildKey(string key, string rootkey)
    {
        return $"{BaseKey}-{rootkey}-{key}";
    }

    protected abstract object GetObjectFromCache(string fullKey);

    [Conditional("DEBUG")]
    protected virtual void OnHit(string fullKey)
    {
        Debug.Print("cache hit:{0}", fullKey);
    }

    [Conditional("DEBUG")]
    protected virtual void OnMiss(string fullKey)
    {
        Debug.Print("cache miss:{0}", fullKey);
    }
}