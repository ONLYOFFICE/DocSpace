namespace ASC.Core;

[DebuggerDisplay("{UserId} - {GroupId}")]
public class UserGroupRef : IMapFrom<UserGroup>
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public bool Removed { get; set; }
    public DateTime LastModified { get; set; }
    public UserGroupRefType RefType { get; set; }
    public int Tenant { get; set; }

    public UserGroupRef() { }

    public UserGroupRef(Guid userId, Guid groupId, UserGroupRefType refType)
    {
        UserId = userId;
        GroupId = groupId;
        RefType = refType;
    }

    public static string CreateKey(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
    {
        return tenant.ToString() + userId.ToString("N") + groupId.ToString("N") + ((int)refType).ToString();
    }

    public string CreateKey()
    {
        return CreateKey(Tenant, UserId, GroupId, RefType);
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode() ^ GroupId.GetHashCode() ^ Tenant.GetHashCode() ^ RefType.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is UserGroupRef r && r.Tenant == Tenant && r.UserId == UserId && r.GroupId == GroupId && r.RefType == RefType;
    }

    public static implicit operator UserGroupRef(UserGroupRefCacheItem cache)
    {
        var result = new UserGroupRef
        {
            UserId = new Guid(cache.UserId),
            GroupId = new Guid(cache.GroupId)
        };

        if (Enum.TryParse<UserGroupRefType>(cache.RefType, out var refType))
        {
            result.RefType = refType;
        }

        result.Tenant = cache.Tenant;
        result.LastModified = new DateTime(cache.LastModified);
        result.Removed = cache.Removed;

        return result;
    }

    public static implicit operator UserGroupRefCacheItem(UserGroupRef cache)
    {
        return new UserGroupRefCacheItem
        {
            GroupId = cache.GroupId.ToString(),
            UserId = cache.UserId.ToString(),
            RefType = cache.RefType.ToString(),
            LastModified = cache.LastModified.Ticks,
            Removed = cache.Removed,
            Tenant = cache.Tenant
        };
    }
}
