namespace ASC.Common.Caching;

[Flags]
public enum CacheNotifyAction
{
    Insert = 1,
    Update = 2,
    Remove = 4,
    InsertOrUpdate = Insert | Update,
    Any = InsertOrUpdate | Remove,
}