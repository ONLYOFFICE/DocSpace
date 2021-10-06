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
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

using Google.Protobuf;

using Microsoft.Extensions.Caching.Memory;

namespace ASC.Common.Caching
{
    [Singletone]
    public class AscCacheNotify
    {
        private ICacheNotify<AscCacheItem> CacheNotify { get; }

        public AscCacheNotify(ICacheNotify<AscCacheItem> cacheNotify)
        {
            CacheNotify = cacheNotify;

            CacheNotify.Subscribe((item) => { OnClearCache(); }, CacheNotifyAction.Any);
        }

        public void ClearCache()
        {
            CacheNotify.Publish(new AscCacheItem { Id = ByteString.CopyFrom(Guid.NewGuid().ToByteArray()) }, CacheNotifyAction.Any);
        }

        public static void OnClearCache()
        {
            var keys = MemoryCache.Default.Select(r => r.Key).ToList();

            foreach (var k in keys)
            {
                MemoryCache.Default.Remove(k);
            }
        }
    }

    [Singletone]
    public class AscCache : ICache
    {
        private IMemoryCache MemoryCache { get; }
        private ConcurrentDictionary<string, object> MemoryCacheKeys { get; }

        public AscCache(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
            MemoryCacheKeys = new ConcurrentDictionary<string, object>();
        }

        public T Get<T>(string key) where T : class
        {
            return MemoryCache.Get<T>(key);
        }

        public void Insert(string key, object value, TimeSpan sligingExpiration)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(sligingExpiration)
                .RegisterPostEvictionCallback(EvictionCallback);

            MemoryCache.Set(key, value, options);
            MemoryCacheKeys.TryAdd(key, null);
        }

        public void Insert(string key, object value, DateTime absolutExpiration)
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(absolutExpiration == DateTime.MaxValue ? DateTimeOffset.MaxValue : new DateTimeOffset(absolutExpiration))
                .RegisterPostEvictionCallback(EvictionCallback);

            MemoryCache.Set(key, value, options);
            MemoryCacheKeys.TryAdd(key, null);
        }

        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            MemoryCacheKeys.TryRemove(key.ToString(), out _);
        }

        public void Remove(string key)
        {
            MemoryCache.Remove(key);
        }

        public void Remove(Regex pattern)
        {
            var copy = MemoryCacheKeys.ToDictionary(p => p.Key, p => p.Value);
            var keys = copy.Select(p => p.Key).Where(k => pattern.IsMatch(k)).ToArray();

            foreach (var key in keys)
            {
                MemoryCache.Remove(key);
            }
        }


        public ConcurrentDictionary<string, T> HashGetAll<T>(string key)
        {
            return MemoryCache.GetOrCreate(key, r => new ConcurrentDictionary<string, T>());
        }

        public T HashGet<T>(string key, string field)
        {
            if (MemoryCache.TryGetValue<ConcurrentDictionary<string, T>>(key, out var dic) && dic.TryGetValue(field, out var value))
            {
                return value;
            }
            return default;
        }

        public void HashSet<T>(string key, string field, T value)
        {
            var dic = HashGetAll<T>(key);
            if (value != null)
            {
                dic.AddOrUpdate(field, value, (k, v) => value);
                MemoryCache.Set(key, dic, DateTime.MaxValue);
            }
            else if (dic != null)
            {
                dic.TryRemove(field, out _);
                if (dic.Count == 0)
                {
                    MemoryCache.Remove(key);
                }
                else
                {
                    MemoryCache.Set(key, dic, DateTime.MaxValue);
                }
            }
        }
    }
}