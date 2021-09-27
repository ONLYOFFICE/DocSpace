using System.Collections.Generic;

namespace ASC.Common.Caching
{
    [Scope]
    public class ScopedCache
    {
        private Dictionary<string, object> cache
            = new Dictionary<string, object>();

        public T Get<T>(string key)
        {
            cache.TryGetValue(key, out var result);

            return (T)result;
        }
        
        public void Insert(string key, object obj)
        {
            if (cache.ContainsKey(key)) cache.Remove(key);

            cache.Add(key, obj);
        }
        
        public void Remove(string key)
        {
            cache.Remove(key);
        }
    }
}
