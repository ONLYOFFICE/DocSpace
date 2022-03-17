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

namespace ASC.Core.Caching;

[Singletone]
public class UserServiceCache
{
    public const string Users = "users";
    private const string _groups = "groups";
    public const string Refs = "refs";

    internal readonly ICache _cache;
    internal readonly CoreBaseSettings _coreBaseSettings;
    internal readonly ICacheNotify<UserInfoCacheItem> _cacheUserInfoItem;
    internal readonly ICacheNotify<UserPhotoCacheItem> _cacheUserPhotoItem;
    internal readonly ICacheNotify<GroupCacheItem> _cacheGroupCacheItem;

    public UserServiceCache(ICacheNotify<GroupCacheItem> cacheGroupCacheItem)
    {
        _cacheGroupCacheItem = cacheGroupCacheItem;
    }

    internal readonly ICacheNotify<UserGroupRefCacheItem> _cacheUserGroupRefItem;

    public UserServiceCache(
        CoreBaseSettings coreBaseSettings,
        ICacheNotify<UserInfoCacheItem> cacheUserInfoItem,
        ICacheNotify<UserPhotoCacheItem> cacheUserPhotoItem,
        ICacheNotify<GroupCacheItem> cacheGroupCacheItem,
        ICacheNotify<UserGroupRefCacheItem> cacheUserGroupRefItem,
        ICache cache)
    {
        _cache = cache;
        _coreBaseSettings = coreBaseSettings;
        _cacheUserInfoItem = cacheUserInfoItem;
        _cacheUserPhotoItem = cacheUserPhotoItem;
        _cacheGroupCacheItem = cacheGroupCacheItem;
        _cacheUserGroupRefItem = cacheUserGroupRefItem;

        cacheUserInfoItem.Subscribe((u) => InvalidateCache(u), ASC.Common.Caching.CacheNotifyAction.Any);
        cacheUserPhotoItem.Subscribe((p) => _cache.Remove(p.Key), ASC.Common.Caching.CacheNotifyAction.Remove);
        cacheGroupCacheItem.Subscribe((g) => InvalidateCache(), ASC.Common.Caching.CacheNotifyAction.Any);

        cacheUserGroupRefItem.Subscribe((r) => UpdateUserGroupRefCache(r, true), ASC.Common.Caching.CacheNotifyAction.Remove);
        cacheUserGroupRefItem.Subscribe((r) => UpdateUserGroupRefCache(r, false), ASC.Common.Caching.CacheNotifyAction.InsertOrUpdate);
    }

    public void InvalidateCache()
    {
        InvalidateCache(null);
    }

    private void InvalidateCache(UserInfoCacheItem userInfo)
    {
        if (userInfo != null)
        {
            var key = GetUserCacheKey(userInfo.Tenant, new Guid(userInfo.Id));
            _cache.Remove(key);
        }
    }

    private void UpdateUserGroupRefCache(UserGroupRef r, bool remove)
    {
        var key = GetRefCacheKey(r.Tenant);
        var refs = _cache.Get<UserGroupRefStore>(key);
        if (!remove && refs != null)
        {
            lock (refs)
            {
                refs[r.CreateKey()] = r;
            }
        }
        else
        {
            InvalidateCache();
        }
    }

    public static string GetUserPhotoCacheKey(int tenant, Guid userId)
    {
        return tenant.ToString() + "userphoto" + userId.ToString();
    }

    public static string GetGroupCacheKey(int tenant)
    {
        return tenant.ToString() + _groups;
    }

    public static string GetGroupCacheKey(int tenant, Guid groupId)
    {
        return tenant.ToString() + _groups + groupId;
    }

    public static string GetRefCacheKey(int tenant)
    {
        return tenant.ToString() + Refs;
    }
    public static string GetRefCacheKey(int tenant, Guid groupId, UserGroupRefType refType)
    {
        return tenant.ToString() + groupId + (int)refType;
    }

    public static string GetUserCacheKey(int tenant)
    {
        return tenant.ToString() + Users;
    }

    public static string GetUserCacheKey(int tenant, Guid userId)
    {
        return tenant.ToString() + Users + userId;
    }
}

[Scope]
class ConfigureCachedUserService : IConfigureNamedOptions<CachedUserService>
{
    internal readonly IOptionsSnapshot<EFUserService> _service;
    internal readonly UserServiceCache _userServiceCache;
    internal readonly CoreBaseSettings _coreBaseSettings;

    public ConfigureCachedUserService(
        IOptionsSnapshot<EFUserService> service,
        UserServiceCache userServiceCache,
        CoreBaseSettings coreBaseSettings)
    {
        _service = service;
        _userServiceCache = userServiceCache;
        _coreBaseSettings = coreBaseSettings;
    }

    public void Configure(string name, CachedUserService options)
    {
        Configure(options);
        options._service = _service.Get(name);
    }

    public void Configure(CachedUserService options)
    {
        options._service = _service.Value;
        options._coreBaseSettings = _coreBaseSettings;
        options._userServiceCache = _userServiceCache;
        options._cache = _userServiceCache._cache;
        options._cacheUserInfoItem = _userServiceCache._cacheUserInfoItem;
        options._cacheUserPhotoItem = _userServiceCache._cacheUserPhotoItem;
        options._cacheGroupCacheItem = _userServiceCache._cacheGroupCacheItem;
        options._cacheUserGroupRefItem = _userServiceCache._cacheUserGroupRefItem;
    }
}

[Scope]
public class CachedUserService : IUserService, ICachedService
{
    internal IUserService _service;
    internal ICache _cache;
    internal TrustInterval _trustInterval;
    private readonly TimeSpan _cacheExpiration;
    private readonly TimeSpan _photoExpiration;
    internal CoreBaseSettings _coreBaseSettings;
    internal UserServiceCache _userServiceCache;
    internal ICacheNotify<UserInfoCacheItem> _cacheUserInfoItem;
    internal ICacheNotify<UserPhotoCacheItem> _cacheUserPhotoItem;
    internal ICacheNotify<GroupCacheItem> _cacheGroupCacheItem;

    public CachedUserService(ICacheNotify<GroupCacheItem> cacheGroupCacheItem)
    {
        _cacheGroupCacheItem = cacheGroupCacheItem;
    }

    internal ICacheNotify<UserGroupRefCacheItem> _cacheUserGroupRefItem;

    public CachedUserService()
    {
        _cacheExpiration = TimeSpan.FromMinutes(20);
        _photoExpiration = TimeSpan.FromMinutes(10);
    }

    public CachedUserService(
        EFUserService service,
        CoreBaseSettings coreBaseSettings,
        UserServiceCache userServiceCache
        ) : this()
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _coreBaseSettings = coreBaseSettings;
        _userServiceCache = userServiceCache;
        _cache = userServiceCache._cache;
        _cacheUserInfoItem = userServiceCache._cacheUserInfoItem;
        _cacheUserPhotoItem = userServiceCache._cacheUserPhotoItem;
        _cacheGroupCacheItem = userServiceCache._cacheGroupCacheItem;
        _cacheUserGroupRefItem = userServiceCache._cacheUserGroupRefItem;
    }

    public IQueryable<UserInfo> GetUsers(
        int tenant,
        bool isAdmin,
        EmployeeStatus? employeeStatus,
        List<List<Guid>> includeGroups,
        List<Guid> excludeGroups,
        EmployeeActivationStatus? activationStatus,
        string text,
        string sortBy,
        bool sortOrderAsc,
        long limit,
        long offset,
        out int total,
        out int count)
    {
        return _service.GetUsers(tenant, isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, text, sortBy, sortOrderAsc, limit, offset, out total, out count);
    }

    public UserInfo GetUser(int tenant, Guid id)
    {
        var key = UserServiceCache.GetUserCacheKey(tenant, id);
        var user = _cache.Get<UserInfo>(key);

        if (user == null)
        {
            user = _service.GetUser(tenant, id);

            if (user != null)
            {
                _cache.Insert(key, user, _cacheExpiration);
            }
        }

        return user;
    }

    public UserInfo GetUser(int tenant, string email)
    {
        return _service.GetUser(tenant, email);
    }

    public UserInfo GetUserByUserName(int tenant, string userName)
    {
        return _service.GetUserByUserName(tenant, userName);
    }

    public UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash)
    {
        return _service.GetUserByPasswordHash(tenant, login, passwordHash);
    }
    public IEnumerable<UserInfo> GetUsersAllTenants(IEnumerable<Guid> userIds)
    {
        return _service.GetUsersAllTenants(userIds);
    }

    public UserInfo SaveUser(int tenant, UserInfo user)
    {
        user = _service.SaveUser(tenant, user);
        _cacheUserInfoItem.Publish(new UserInfoCacheItem { Id = user.Id.ToString(), Tenant = tenant }, CacheNotifyAction.Any);

        return user;
    }

    public void RemoveUser(int tenant, Guid id)
    {
        _service.RemoveUser(tenant, id);
        _cacheUserInfoItem.Publish(new UserInfoCacheItem { Tenant = tenant, Id = id.ToString() }, CacheNotifyAction.Any);
    }

    public byte[] GetUserPhoto(int tenant, Guid id)
    {
        var photo = _cache.Get<byte[]>(UserServiceCache.GetUserPhotoCacheKey(tenant, id));
        if (photo == null)
        {
            photo = _service.GetUserPhoto(tenant, id);
            _cache.Insert(UserServiceCache.GetUserPhotoCacheKey(tenant, id), photo, _photoExpiration);
        }

        return photo;
    }

    public void SetUserPhoto(int tenant, Guid id, byte[] photo)
    {
        _service.SetUserPhoto(tenant, id, photo);
        _cacheUserPhotoItem.Publish(new UserPhotoCacheItem { Key = UserServiceCache.GetUserPhotoCacheKey(tenant, id) }, ASC.Common.Caching.CacheNotifyAction.Remove);
    }

    public DateTime GetUserPasswordStamp(int tenant, Guid id)
    {
        return _service.GetUserPasswordStamp(tenant, id);
    }

    public void SetUserPasswordHash(int tenant, Guid id, string passwordHash)
    {
        _service.SetUserPasswordHash(tenant, id, passwordHash);
    }

    public Group GetGroup(int tenant, Guid id)
    {
        var key = UserServiceCache.GetGroupCacheKey(tenant, id);
        var group = _cache.Get<Group>(key);

        if (group == null)
        {
            group = _service.GetGroup(tenant, id);

            if (group != null)
            {
                _cache.Insert(key, group, _cacheExpiration);
            }
        }

        return group;
    }

    public Group SaveGroup(int tenant, Group group)
    {
        group = _service.SaveGroup(tenant, group);
        _cacheGroupCacheItem.Publish(new GroupCacheItem { Id = group.Id.ToString() }, CacheNotifyAction.Any);

        return group;
    }

    public void RemoveGroup(int tenant, Guid id)
    {
        _service.RemoveGroup(tenant, id);
        _cacheGroupCacheItem.Publish(new GroupCacheItem { Id = id.ToString() }, CacheNotifyAction.Any);
    }


    public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant)
    {
        var key = UserServiceCache.GetRefCacheKey(tenant);
        if (!(_cache.Get<UserGroupRefStore>(key) is IDictionary<string, UserGroupRef> refs))
        {
            refs = _service.GetUserGroupRefs(tenant);
            _cache.Insert(key, new UserGroupRefStore(refs), _cacheExpiration);
        }

        return refs;
    }

    public UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType)
    {
        var key = UserServiceCache.GetRefCacheKey(tenant, groupId, refType);
        var groupRef = _cache.Get<UserGroupRef>(key);

        if (groupRef == null)
        {
            groupRef = _service.GetUserGroupRef(tenant, groupId, refType);

            if (groupRef != null)
            {
                _cache.Insert(key, groupRef, _cacheExpiration);
            }
        }

        return groupRef;
    }

    public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
    {
        r = _service.SaveUserGroupRef(tenant, r);
        _cacheUserGroupRefItem.Publish(r, CacheNotifyAction.InsertOrUpdate);

        return r;
    }

    public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
    {
        _service.RemoveUserGroupRef(tenant, userId, groupId, refType);

        var r = new UserGroupRef(userId, groupId, refType) { Tenant = tenant };
        _cacheUserGroupRefItem.Publish(r, CacheNotifyAction.Remove);
    }


    public IEnumerable<UserInfo> GetUsers(int tenant)
    {
        var key = UserServiceCache.GetUserCacheKey(tenant);
        var users = _cache.Get<IEnumerable<UserInfo>>(key);
        if (users == null)
        {
            users = _service.GetUsers(tenant);

            _cache.Insert(key, users, _cacheExpiration);
        }

        return users;
    }

    public IEnumerable<Group> GetGroups(int tenant)
    {
        var key = UserServiceCache.GetGroupCacheKey(tenant);
        var groups = _cache.Get<IEnumerable<Group>>(key);
        if (groups == null)
        {
            groups = _service.GetGroups(tenant);
            _cache.Insert(key, groups, _cacheExpiration);
        }

        return groups;
    }

    public void InvalidateCache()
    {
        _userServiceCache.InvalidateCache();
    }

    public UserInfo GetUser(int tenant, Guid id, Expression<Func<User, UserInfo>> exp)
    {
        if (exp == null)
        {
            return GetUser(tenant, id);
        }

        return _service.GetUser(tenant, id, exp);
    }
}
