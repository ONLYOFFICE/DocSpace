namespace ASC.Api.Collections;

[CollectionDataContract(Name = "hash", Namespace = "", ItemName = "entry", KeyName = "key", ValueName = "value")]
public class ItemDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    public ItemDictionary() { }

    public ItemDictionary(IDictionary<TKey, TValue> items)
        : base(items) { }
}