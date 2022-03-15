namespace ASC.Common.Caching;

[Singletone(typeof(AscCache))]
public interface ICache
{
    T Get<T>(string key) where T : class;

    void Insert(string key, object value, TimeSpan sligingExpiration);

    void Insert(string key, object value, DateTime absolutExpiration);

    void Remove(string key);

    void Remove(Regex pattern);

    ConcurrentDictionary<string, T> HashGetAll<T>(string key);

    T HashGet<T>(string key, string field);

    void HashSet<T>(string key, string field, T value);
}