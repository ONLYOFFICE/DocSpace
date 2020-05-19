using System;
using System.Diagnostics;

using Microsoft.Extensions.Caching.Memory;

namespace AppLimit.CloudComputing.SharpBox.Common.Cache
{
    public sealed class CachedDictionary<T> : CachedDictionaryBase<T>
    {
        private readonly TimeSpan _absoluteExpirationPeriod;

        private DateTime AbsoluteExpiration
        {
            get
            {
                if (_absoluteExpirationPeriod == TimeSpan.Zero)
                    return DateTime.MaxValue;
                return DateTime.Now + _absoluteExpirationPeriod;
            }
        }

        private TimeSpan SlidingExpiration { get; set; }
        private MemoryCache MemoryCache { get; set; }

        public CachedDictionary(string baseKey, TimeSpan absoluteExpirationPeriod, TimeSpan slidingExpiration, Func<T, bool> cacheCodition)
        {
            MemoryCache = new MemoryCache(new MemoryCacheOptions());
            _baseKey = baseKey;
            _absoluteExpirationPeriod = absoluteExpirationPeriod;
            SlidingExpiration = slidingExpiration;
            _cacheCodition = cacheCodition ?? throw new ArgumentNullException("cacheCodition");
            InsertRootKey(_baseKey);
        }

        public CachedDictionary(string baseKey)
            : this(baseKey, x => true)
        {
        }

        public CachedDictionary(string baseKey, Func<T, bool> cacheCodition)
            : this(baseKey, TimeSpan.Zero, TimeSpan.Zero, cacheCodition)
        {
        }

        protected override void InsertRootKey(string rootKey)
        {
#if (DEBUG)
            Debug.Print("inserted root key {0}", rootKey);
#endif
            MemoryCache.Remove(rootKey);
            MemoryCache.Set(rootKey, DateTime.UtcNow.Ticks, new MemoryCacheEntryOptions() { AbsoluteExpiration = DateTime.MaxValue, SlidingExpiration = TimeSpan.Zero, Priority = CacheItemPriority.NeverRemove });
        }

        public override void Reset(string rootKey, string key)
        {
            MemoryCache.Remove(BuildKey(key, rootKey));
        }

        protected override object GetObjectFromCache(string fullKey)
        {
            return MemoryCache.Get(fullKey);
        }

        public override void Add(string rootkey, string key, T newValue)
        {
            var builtrootkey = BuildKey(string.Empty, string.IsNullOrEmpty(rootkey) ? "root" : rootkey);
            if (!MemoryCache.TryGetValue(builtrootkey, out _))
            {
#if (DEBUG)
                Debug.Print("added root key {0}", builtrootkey);
#endif
                //Insert root if no present
                MemoryCache.Remove(builtrootkey);
                MemoryCache.Set(builtrootkey, DateTime.UtcNow.Ticks, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = AbsoluteExpiration,
                    SlidingExpiration = SlidingExpiration
                });

                MemoryCache.Remove(BuildKey(key, rootkey));
            }

            if (newValue != null)
            {
                var buildKey = BuildKey(key, rootkey);
                MemoryCache.Remove(buildKey);
                var options = new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.Normal,
                    AbsoluteExpiration = AbsoluteExpiration,
                    SlidingExpiration = SlidingExpiration,
                };
                MemoryCache.Set(BuildKey(key, rootkey), newValue, new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.Normal,
                    AbsoluteExpiration = AbsoluteExpiration,
                    SlidingExpiration = SlidingExpiration
                });
                //TODO
                //options.AddExpirationToken(Microsoft.Extensions.Primitives.CancellationChangeToken);
                //new CacheDependency(null, new[] { _baseKey, builtrootkey }),
            }
            else
            {
                MemoryCache.Remove(BuildKey(key, rootkey)); //Remove if null
            }
        }
    }
}