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
    public const string Refs = "refs";
    private const string Groups = "groups";

    internal ICache Cache { get; }
    internal CoreBaseSettings CoreBaseSettings { get; }
    internal ICacheNotify<UserInfoCacheItem> UserInfoCacheNotify { get; }
    internal ICacheNotify<UserPhotoCacheItem> UserPhotoCacheNotify { get; }
    internal ICacheNotify<GroupCacheItem> GroupCacheNotify { get; }
    internal ICacheNotify<UserGroupRefCacheItem> UserGroupRefCacheNotify { get; }

    public UserServiceCache(
        CoreBaseSettings coreBaseSettings,
        ICacheNotify<UserInfoCacheItem> userInfoCacheNotify,
        ICacheNotify<UserPhotoCacheItem> userPhotoCacheNotify,
        ICacheNotify<GroupCacheItem> groupCacheNotify,
        ICacheNotify<UserGroupRefCacheItem> userGroupRefCacheNotify,
        ICache cache)
    {
        Cache = cache;
        CoreBaseSettings = coreBaseSettings;
        UserInfoCacheNotify = userInfoCacheNotify;
        UserPhotoCacheNotify = userPhotoCacheNotify;
        GroupCacheNotify = groupCacheNotify;
        UserGroupRefCacheNotify = userGroupRefCacheNotify;

        userInfoCacheNotify.Subscribe((u) => InvalidateCache(u), CacheNotifyAction.Any);
        userPhotoCacheNotify.Subscribe((p) => Cache.Remove(p.Key), CacheNotifyAction.Remove);
        groupCacheNotify.Subscribe((g) => InvalidateCache(), CacheNotifyAction.Any);

        userGroupRefCacheNotify.Subscribe((r) => UpdateUserGroupRefCache(r, true), CacheNotifyAction.Remove);
        userGroupRefCacheNotify.Subscribe((r) => UpdateUserGroupRefCache(r, false), CacheNotifyAction.InsertOrUpdate);
    }

    public void InvalidateCache()
    {
        InvalidateCache(null);
    }

    public static string GetUserPhotoCacheKey(int tenant, Guid userId)
    {
        return $"{tenant}userphoto{userId}";
    }

    public static string GetGroupCacheKey(int tenant)
    {
        return $"{tenant}{Groups}";
    }

    public static string GetGroupCacheKey(int tenant, Guid groupId)
    {
        return $"{tenant}{Groups}{groupId}";
    }

    public static string GetRefCacheKey(int tenant)
    {
        return $"{tenant}{Refs}";
    }

    public static string GetRefCacheKey(int tenant, Guid groupId, UserGroupRefType refType)
    {
        return $"{tenant}{groupId}{(int)refType}";
    }

    public static string GetUserCacheKey(int tenant)
    {
        return $"{tenant}{Users}";
    }

    public static string GetUserCacheKey(int tenant, Guid userId)
    {
        return $"{tenant}{Users}{userId}";
    }

    private void InvalidateCache(UserInfoCacheItem userInfo)
    {
        if (userInfo != null)
        {
            var key = GetUserCacheKey(userInfo.Tenant, new Guid(userInfo.Id));
            Cache.Remove(key);
        }
    }

    private void UpdateUserGroupRefCache(UserGroupRef r, bool remove)
    {
        var key = GetRefCacheKey(r.Tenant);
        var refs = Cache.Get<UserGroupRefStore>(key);
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
}

[Scope]
class ConfigureCachedUserService : IConfigureNamedOptions<CachedUserService>
{
    internal IOptionsSnapshot<EFUserService> Service { get; }
    internal UserServiceCache UserServiceCache { get; }
    internal CoreBaseSettings CoreBaseSettings { get; }

    public ConfigureCachedUserService(
        IOptionsSnapshot<EFUserService> service,
        UserServiceCache userServiceCache,
        CoreBaseSettings coreBaseSettings)
    {
        Service = service;
        UserServiceCache = userServiceCache;
        CoreBaseSettings = coreBaseSettings;
    }

    public void Configure(string name, CachedUserService options)
    {
        Configure(options);
        options.Service = Service.Get(name);
    }

    public void Configure(CachedUserService options)
    {
        options.Service = Service.Value;
        options.CoreBaseSettings = CoreBaseSettings;
        options.UserServiceCache = UserServiceCache;
        options.Cache = UserServiceCache.Cache;
        options.UserInfoCacheNotify = UserServiceCache.UserInfoCacheNotify;
        options.UserPhotoCacheNotify = UserServiceCache.UserPhotoCacheNotify;
        options.GroupCacheNotify = UserServiceCache.GroupCacheNotify;
        options.UserGroupRefCacheNotify = UserServiceCache.UserGroupRefCacheNotify;
    }
}

[Scope]
public class CachedUserService : IUserService, ICachedService
{
    internal IUserService Service { get; set; }
    internal ICache Cache { get; set; }
    internal TrustInterval TrustInterval { get; set; }
    internal CoreBaseSettings CoreBaseSettings { get; set; }
    internal UserServiceCache UserServiceCache { get; set; }
    internal ICacheNotify<UserInfoCacheItem> UserInfoCacheNotify { get; set; }
    internal ICacheNotify<UserPhotoCacheItem> UserPhotoCacheNotify { get; set; }
    internal ICacheNotify<GroupCacheItem> GroupCacheNotify { get; set; }
    internal ICacheNotify<UserGroupRefCacheItem> UserGroupRefCacheNotify { get; set; }

    private TimeSpan _cacheExpiration;
    private TimeSpan _photoExpiration;

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
        Service = service ?? throw new ArgumentNullException(nameof(service));
        CoreBaseSettings = coreBaseSettings;
        UserServiceCache = userServiceCache;
        Cache = userServiceCache.Cache;
        UserInfoCacheNotify = userServiceCache.UserInfoCacheNotify;
        UserPhotoCacheNotify = userServiceCache.UserPhotoCacheNotify;
        GroupCacheNotify = userServiceCache.GroupCacheNotify;
        UserGroupRefCacheNotify = userServiceCache.UserGroupRefCacheNotify;
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
        return Service.GetUsers(tenant, isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, text, sortBy, sortOrderAsc, limit, offset, out total, out count);
    }

    public UserInfo GetUser(int tenant, Guid id)
    {
        var key = UserServiceCache.GetUserCacheKey(tenant, id);
        var user = Cache.Get<UserInfo>(key);

        if (user == null)
        {
            user = Service.GetUser(tenant, id);

            if (user != null)
            {
                Cache.Insert(key, user, _cacheExpiration);
            }
        }

        return user;
    }

    public UserInfo GetUser(int tenant, string email)
    {
        return Service.GetUser(tenant, email);
    }

    public UserInfo GetUserByUserName(int tenant, string userName)
    {
        return Service.GetUserByUserName(tenant, userName);
    }

    public UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash)
    {
        return Service.GetUserByPasswordHash(tenant, login, passwordHash);
    }

    public IEnumerable<UserInfo> GetUsersAllTenants(IEnumerable<Guid> userIds)
    {
        return Service.GetUsersAllTenants(userIds);
    }

    public UserInfo SaveUser(int tenant, UserInfo user)
    {
        user = Service.SaveUser(tenant, user);
        UserInfoCacheNotify.Publish(new UserInfoCacheItem { Id = user.Id.ToString(), Tenant = tenant }, CacheNotifyAction.Any);

        return user;
    }

    public void RemoveUser(int tenant, Guid id)
    {
        Service.RemoveUser(tenant, id);
        UserInfoCacheNotify.Publish(new UserInfoCacheItem { Tenant = tenant, Id = id.ToString() }, CacheNotifyAction.Any);
    }

    public byte[] GetUserPhoto(int tenant, Guid id)
    {
        var photo = Cache.Get<byte[]>(UserServiceCache.GetUserPhotoCacheKey(tenant, id));
        if (photo == null)
        {
            photo = Service.GetUserPhoto(tenant, id);
            Cache.Insert(UserServiceCache.GetUserPhotoCacheKey(tenant, id), photo, _photoExpiration);
        }

        return photo;
    }

    public void SetUserPhoto(int tenant, Guid id, byte[] photo)
    {
        Service.SetUserPhoto(tenant, id, photo);
        UserPhotoCacheNotify.Publish(new UserPhotoCacheItem { Key = UserServiceCache.GetUserPhotoCacheKey(tenant, id) }, CacheNotifyAction.Remove);
    }

    public DateTime GetUserPasswordStamp(int tenant, Guid id)
    {
        return Service.GetUserPasswordStamp(tenant, id);
    }

    public void SetUserPasswordHash(int tenant, Guid id, string passwordHash)
    {
        Service.SetUserPasswordHash(tenant, id, passwordHash);
    }

    public Group GetGroup(int tenant, Guid id)
    {
        var key = UserServiceCache.GetGroupCacheKey(tenant, id);
        var group = Cache.Get<Group>(key);

        if (group == null)
        {
            group = Service.GetGroup(tenant, id);

            if (group != null)
            {
                Cache.Insert(key, group, _cacheExpiration);
            }
        };

        return group;
    }

    public Group SaveGroup(int tenant, Group group)
    {
        group = Service.SaveGroup(tenant, group);
        GroupCacheNotify.Publish(new GroupCacheItem { Id = group.Id.ToString() }, CacheNotifyAction.Any);

        return group;
    }

    public void RemoveGroup(int tenant, Guid id)
    {
        Service.RemoveGroup(tenant, id);
        GroupCacheNotify.Publish(new GroupCacheItem { Id = id.ToString() }, CacheNotifyAction.Any);
    }


    public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant)
    {
        var key = UserServiceCache.GetRefCacheKey(tenant);
        if (!(Cache.Get<UserGroupRefStore>(key) is IDictionary<string, UserGroupRef> refs))
        {
            refs = Service.GetUserGroupRefs(tenant);
            Cache.Insert(key, new UserGroupRefStore(refs), _cacheExpiration);
        }

        return refs;
    }

    public UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType)
    {
        var key = UserServiceCache.GetRefCacheKey(tenant, groupId, refType);
        var groupRef = Cache.Get<UserGroupRef>(key);

        if (groupRef == null)
        {
            groupRef = Service.GetUserGroupRef(tenant, groupId, refType);

            if (groupRef != null)
            {
                Cache.Insert(key, groupRef, _cacheExpiration);
            }
        }

        return groupRef;
    }

    public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
    {
        r = Service.SaveUserGroupRef(tenant, r);
        UserGroupRefCacheNotify.Publish(r, CacheNotifyAction.InsertOrUpdate);

        return r;
    }

    public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
    {
        Service.RemoveUserGroupRef(tenant, userId, groupId, refType);

        var r = new UserGroupRef(userId, groupId, refType) { Tenant = tenant };
        UserGroupRefCacheNotify.Publish(r, CacheNotifyAction.Remove);
    }


    public IEnumerable<UserInfo> GetUsers(int tenant)
    {
        var key = UserServiceCache.GetUserCacheKey(tenant);
        var users = Cache.Get<IEnumerable<UserInfo>>(key);
        if (users == null)
        {
            users = Service.GetUsers(tenant);

            Cache.Insert(key, users, _cacheExpiration);
        }

        return users;
    }

    public IEnumerable<Group> GetGroups(int tenant)
    {
        var key = UserServiceCache.GetGroupCacheKey(tenant);
        var groups = Cache.Get<IEnumerable<Group>>(key);
        if (groups == null)
        {
            groups = Service.GetGroups(tenant);
            Cache.Insert(key, groups, _cacheExpiration);
        }

        return groups;
    }


    public void InvalidateCache()
    {
        UserServiceCache.InvalidateCache();
    }

    public UserInfo GetUser(int tenant, Guid id, Expression<Func<User, UserInfo>> exp)
    {
        if (exp == null)
        {
            return GetUser(tenant, id);
        }

        return Service.GetUser(tenant, id, exp);
    }
}
