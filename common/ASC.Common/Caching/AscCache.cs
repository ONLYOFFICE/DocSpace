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


namespace ASC.Common.Caching;

[Singletone]
public class AscCacheNotify
{
    private readonly ICacheNotify<AscCacheItem> _cacheNotify;
    private readonly ICache _cache;

    public AscCacheNotify(ICacheNotify<AscCacheItem> cacheNotify, ICache cache)
    {
        _cacheNotify = cacheNotify;
        _cache = cache;

        _cacheNotify.Subscribe((item) => { OnClearCache(); }, CacheNotifyAction.Any);
    }

    public void ClearCache() => _cacheNotify.Publish(new AscCacheItem { Id = Guid.NewGuid().ToString() }, CacheNotifyAction.Any);

    public void OnClearCache()
    {
        _cache.Reset();
    }
}

[Singletone]
public class AscCache : ICache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, object> _memoryCacheKeys;
    private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

    public AscCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _memoryCacheKeys = new ConcurrentDictionary<string, object>();
    }

    public T Get<T>(string key) where T : class
    {
        return _memoryCache.Get<T>(key);
    }

    public void Insert(string key, object value, TimeSpan sligingExpiration, Action<object, object, EvictionReason, object> evictionCallback = null)
    {
        Insert(key, value, sligingExpiration, null, evictionCallback);
    }

    public void Insert(string key, object value, DateTime absolutExpiration, Action<object, object, EvictionReason, object> evictionCallback = null)
    {
        Insert(key, value, null, absolutExpiration, evictionCallback);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public void Remove(Regex pattern)
    {
        var copy = _memoryCacheKeys.ToDictionary(p => p.Key, p => p.Value);
        var keys = copy.Select(p => p.Key).Where(k => pattern.IsMatch(k));

        foreach (var key in keys)
        {
            _memoryCache.Remove(key);
        }
    }

    public void Reset()
    {
        if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
        }

        _resetCacheToken = new CancellationTokenSource();
    }

    public ConcurrentDictionary<string, T> HashGetAll<T>(string key) =>
        _memoryCache.GetOrCreate(key, r => new ConcurrentDictionary<string, T>());

    public T HashGet<T>(string key, string field)
    {
        if (_memoryCache.TryGetValue<ConcurrentDictionary<string, T>>(key, out var dic)
            && dic.TryGetValue(field, out var value))
        {
            return value;
        }

        return default;
    }

    public void HashSet<T>(string key, string field, T value)
    {
        var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.MaxValue)
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

        var dic = HashGetAll<T>(key);

        if (value != null)
        {
            dic.AddOrUpdate(field, value, (k, v) => value);

            _memoryCache.Set(key, dic, options);
        }
        else if (dic != null)
        {
            dic.TryRemove(field, out _);

            if (dic.IsEmpty)
            {
                _memoryCache.Remove(key);
            }
            else
            {
                _memoryCache.Set(key, dic, options);
            }
        }
    }

    private void Insert(string key, object value, TimeSpan? sligingExpiration = null, DateTime? absolutExpiration = null, Action<object, object, EvictionReason, object> evictionCallback = null)
    {
        var options = new MemoryCacheEntryOptions()
            .RegisterPostEvictionCallback(EvictionCallback)
            .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

        if (sligingExpiration.HasValue)
        {
            options = options.SetSlidingExpiration(sligingExpiration.Value);
        }

        if (absolutExpiration.HasValue)
        {
            options = options.SetAbsoluteExpiration(absolutExpiration.Value == DateTime.MaxValue ? DateTimeOffset.MaxValue : new DateTimeOffset(absolutExpiration.Value));
        }

        if (evictionCallback != null)
        {
            options = options.RegisterPostEvictionCallback(new PostEvictionDelegate(evictionCallback));
        }

        _memoryCache.Set(key, value, options);
        _memoryCacheKeys.TryAdd(key, null);
    }

    private void EvictionCallback(object key, object value, EvictionReason reason, object state)
    {
        _memoryCacheKeys.TryRemove(key.ToString(), out _);
    }
}