namespace ASC.FederatedLogin.Helpers;

public static class DictionaryExtensions
{
    public static TValue Get<T, TValue>(this IDictionary<T, TValue> disct, T key)
    {
        disct.TryGetValue(key, out var def);

        return def;
    }
}
