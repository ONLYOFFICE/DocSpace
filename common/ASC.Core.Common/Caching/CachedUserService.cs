// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Core.Caching;

[Singletone]
public class UserServiceCache
{
    public const string Users = "users";
    public const string Refs = "refs";
    private const string Groups = "groups";

    internal readonly ICache Cache;
    internal readonly CoreBaseSettings CoreBaseSettings;
    internal readonly ICacheNotify<UserInfoCacheItem> CacheUserInfoItem;
    internal readonly ICacheNotify<UserPhotoCacheItem> CacheUserPhotoItem;
    internal readonly ICacheNotify<GroupCacheItem> CacheGroupCacheItem;
    internal readonly ICacheNotify<UserGroupRefCacheItem> CacheUserGroupRefItem;

    public UserServiceCache(ICacheNotify<GroupCacheItem> cacheGroupCacheItem)
    {
        CacheGroupCacheItem = cacheGroupCacheItem;
    }

    public UserServiceCache(
        CoreBaseSettings coreBaseSettings,
        ICacheNotify<UserInfoCacheItem> cacheUserInfoItem,
        ICacheNotify<UserPhotoCacheItem> cacheUserPhotoItem,
        ICacheNotify<GroupCacheItem> cacheGroupCacheItem,
        ICacheNotify<UserGroupRefCacheItem> cacheUserGroupRefItem,
        ICache cache)
    {
        Cache = cache;
        CoreBaseSettings = coreBaseSettings;
        CacheUserInfoItem = cacheUserInfoItem;
        CacheUserPhotoItem = cacheUserPhotoItem;
        CacheGroupCacheItem = cacheGroupCacheItem;
        CacheUserGroupRefItem = cacheUserGroupRefItem;

        cacheUserInfoItem.Subscribe((u) => InvalidateCache(u), CacheNotifyAction.Any);
        cacheUserPhotoItem.Subscribe((p) => Cache.Remove(p.Key), CacheNotifyAction.Remove);
        cacheGroupCacheItem.Subscribe((g) => InvalidateCache(g), CacheNotifyAction.Any);

        cacheUserGroupRefItem.Subscribe((r) => UpdateUserGroupRefCache(r), CacheNotifyAction.Remove);
        cacheUserGroupRefItem.Subscribe((r) => UpdateUserGroupRefCache(r), CacheNotifyAction.InsertOrUpdate);
    }

    private void InvalidateCache(UserInfoCacheItem userInfo)
    {
        if (userInfo != null)
        {
            var key = GetUserCacheKey(userInfo.Tenant);
            Cache.Remove(key);

            if (Guid.TryParse(userInfo.Id, out var userId))
            {
                var userKey = GetUserCacheKey(userInfo.Tenant, userId);
                Cache.Remove(userKey);
            }
        }
    }
    private void InvalidateCache(GroupCacheItem groupCacheItem)
    {
        if (groupCacheItem != null)
        {
            var key = GetGroupCacheKey(groupCacheItem.Tenant, new Guid(groupCacheItem.Id));
            Cache.Remove(key);
        }
    }

    private void UpdateUserGroupRefCache(UserGroupRef r)
    {
        var key = GetRefCacheKey(r.TenantId);
        var refs = Cache.Get<UserGroupRefStore>(key);
        if (refs != null)
        {
            lock (refs)
            {
                refs[r.CreateKey()] = r;
            }
        }
    }

    public static string GetUserPhotoCacheKey(int tenant, Guid userId)
    {
        return tenant.ToString() + "userphoto" + userId.ToString();
    }

    public static string GetGroupCacheKey(int tenant)
    {
        return tenant.ToString() + Groups;
    }

    public static string GetGroupCacheKey(int tenant, Guid groupId)
    {
        return tenant.ToString() + Groups + groupId;
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
public class CachedUserService : IUserService, ICachedService
{
    internal IUserService Service { get; set; }
    internal ICache Cache { get; set; }
    internal CoreBaseSettings CoreBaseSettings { get; set; }
    internal UserServiceCache UserServiceCache { get; set; }
    internal ICacheNotify<UserInfoCacheItem> CacheUserInfoItem { get; set; }
    internal ICacheNotify<UserPhotoCacheItem> CacheUserPhotoItem { get; set; }
    internal ICacheNotify<GroupCacheItem> CacheGroupCacheItem { get; set; }
    internal ICacheNotify<UserGroupRefCacheItem> CacheUserGroupRefItem { get; set; }

    private readonly TimeSpan _cacheExpiration;
    private readonly TimeSpan _photoExpiration;

    public CachedUserService(ICacheNotify<GroupCacheItem> cacheGroupCacheItem)
    {
        CacheGroupCacheItem = cacheGroupCacheItem;
    }

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
        CacheUserInfoItem = userServiceCache.CacheUserInfoItem;
        CacheUserPhotoItem = userServiceCache.CacheUserPhotoItem;
        CacheGroupCacheItem = userServiceCache.CacheGroupCacheItem;
        CacheUserGroupRefItem = userServiceCache.CacheUserGroupRefItem;
    }

    public IQueryable<UserInfo> GetUsers(
        int tenant,
        bool isDocSpaceAdmin,
        EmployeeStatus? employeeStatus,
        List<List<Guid>> includeGroups,
        List<Guid> excludeGroups,
        List<Tuple<List<List<Guid>>, List<Guid>>> combinedGroups,
        EmployeeActivationStatus? activationStatus,
        AccountLoginType? accountLoginType,
        string text,
        string sortBy,
        bool sortOrderAsc,
        long limit,
        long offset,
        out int total,
        out int count)
    {
        return Service.GetUsers(tenant, isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, combinedGroups, activationStatus, accountLoginType, text, sortBy, sortOrderAsc, limit, offset, out total, out count);
    }

    public async Task<UserInfo> GetUserAsync(int tenant, Guid id)
    {
        var key = UserServiceCache.GetUserCacheKey(tenant, id);
        var user = Cache.Get<UserInfo>(key);

        if (user == null)
        {
            user = await Service.GetUserAsync(tenant, id);

            if (user != null)
            {
                Cache.Insert(key, user, _cacheExpiration);
            }
        }

        return user;
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

    public async Task<UserInfo> GetUserAsync(int tenant, string email)
    {
        return await Service.GetUserAsync(tenant, email);
    }

    public async Task<UserInfo> GetUserByUserName(int tenant, string userName)
    {
        return await Service.GetUserByUserName(tenant, userName);
    }

    public async Task<UserInfo> GetUserByPasswordHashAsync(int tenant, string login, string passwordHash)
    {
        return await Service.GetUserByPasswordHashAsync(tenant, login, passwordHash);
    }
    public async Task<IEnumerable<UserInfo>> GetUsersAllTenantsAsync(IEnumerable<Guid> userIds)
    {
        return await Service.GetUsersAllTenantsAsync(userIds);
    }

    public async Task<UserInfo> SaveUserAsync(int tenant, UserInfo user)
    {
        user = await Service.SaveUserAsync(tenant, user);
        CacheUserInfoItem.Publish(new UserInfoCacheItem { Id = user.Id.ToString(), Tenant = tenant }, CacheNotifyAction.Any);

        return user;
    }

    public async Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime from)
    {
        return await Service.GetTenantsWithFeedsAsync(from);
    }

    public async Task RemoveUserAsync(int tenant, Guid id)
    {
        await Service.RemoveUserAsync(tenant, id);
        CacheUserInfoItem.Publish(new UserInfoCacheItem { Tenant = tenant, Id = id.ToString() }, CacheNotifyAction.Any);
    }

    public async Task<byte[]> GetUserPhotoAsync(int tenant, Guid id)
    {
        var photo = Cache.Get<byte[]>(UserServiceCache.GetUserPhotoCacheKey(tenant, id));
        if (photo == null)
        {
            photo = await Service.GetUserPhotoAsync(tenant, id);
            Cache.Insert(UserServiceCache.GetUserPhotoCacheKey(tenant, id), photo, _photoExpiration);
        }

        return photo;
    }

    public async Task SetUserPhotoAsync(int tenant, Guid id, byte[] photo)
    {
        await Service.SetUserPhotoAsync(tenant, id, photo);
        CacheUserPhotoItem.Publish(new UserPhotoCacheItem { Key = UserServiceCache.GetUserPhotoCacheKey(tenant, id) }, CacheNotifyAction.Remove);
        CacheUserInfoItem.Publish(new UserInfoCacheItem { Id = id.ToString(), Tenant = tenant }, CacheNotifyAction.Any);
    }

    public async Task<DateTime> GetUserPasswordStampAsync(int tenant, Guid id)
    {
        return await Service.GetUserPasswordStampAsync(tenant, id);
    }

    public async Task SetUserPasswordHashAsync(int tenant, Guid id, string passwordHash)
    {
        await Service.SetUserPasswordHashAsync(tenant, id, passwordHash);
    }

    public async Task<Group> GetGroupAsync(int tenant, Guid id)
    {
        var key = UserServiceCache.GetGroupCacheKey(tenant, id);
        var group = Cache.Get<Group>(key);

        if (group == null)
        {
            group = await Service.GetGroupAsync(tenant, id);

            if (group != null)
            {
                Cache.Insert(key, group, _cacheExpiration);
            }
        }

        return group;
    }

    public async Task<Group> SaveGroupAsync(int tenant, Group group)
    {
        group = await Service.SaveGroupAsync(tenant, group);
        CacheGroupCacheItem.Publish(new GroupCacheItem { Id = group.Id.ToString(), Tenant = tenant }, CacheNotifyAction.Any);

        return group;
    }

    public async Task RemoveGroupAsync(int tenant, Guid id)
    {
        await Service.RemoveGroupAsync(tenant, id);
        CacheGroupCacheItem.Publish(new GroupCacheItem { Id = id.ToString(), Tenant = tenant }, CacheNotifyAction.Any);
    }


    public async Task<IDictionary<string, UserGroupRef>> GetUserGroupRefsAsync(int tenant)
    {
        var key = UserServiceCache.GetRefCacheKey(tenant);
        if (Cache.Get<UserGroupRefStore>(key) is not IDictionary<string, UserGroupRef> refs)
        {
            refs = await Service.GetUserGroupRefsAsync(tenant);
            Cache.Insert(key, new UserGroupRefStore(refs), _cacheExpiration);
        }

        return refs;
    }

    public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant)
    {
        var key = UserServiceCache.GetRefCacheKey(tenant);
        if (Cache.Get<UserGroupRefStore>(key) is not IDictionary<string, UserGroupRef> refs)
        {
            refs = Service.GetUserGroupRefs(tenant);
            Cache.Insert(key, new UserGroupRefStore(refs), _cacheExpiration);
        }

        return refs;
    }

    public async Task<UserGroupRef> GetUserGroupRefAsync(int tenant, Guid groupId, UserGroupRefType refType)
    {
        var key = UserServiceCache.GetRefCacheKey(tenant, groupId, refType);
        var groupRef = Cache.Get<UserGroupRef>(key);

        if (groupRef == null)
        {
            groupRef = await Service.GetUserGroupRefAsync(tenant, groupId, refType);

            if (groupRef != null)
            {
                Cache.Insert(key, groupRef, _cacheExpiration);
            }
        }

        return groupRef;
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

    public async Task<UserGroupRef> SaveUserGroupRefAsync(int tenant, UserGroupRef r)
    {
        r = await Service.SaveUserGroupRefAsync(tenant, r);
        CacheUserGroupRefItem.Publish(r, CacheNotifyAction.InsertOrUpdate);

        return r;
    }

    public async Task RemoveUserGroupRefAsync(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
    {
        await Service.RemoveUserGroupRefAsync(tenant, userId, groupId, refType);

        var r = new UserGroupRef(userId, groupId, refType) { TenantId = tenant, Removed = true };
        CacheUserGroupRefItem.Publish(r, CacheNotifyAction.Remove);
    }


    public async Task<IEnumerable<UserInfo>> GetUsersAsync(int tenant)
    {
        var key = UserServiceCache.GetUserCacheKey(tenant);
        var users = Cache.Get<IEnumerable<UserInfo>>(key);
        if (users == null)
        {
            users = await Service.GetUsersAsync(tenant);

            Cache.Insert(key, users, _cacheExpiration);
        }

        return users;
    }

    public async Task<IEnumerable<Group>> GetGroupsAsync(int tenant)
    {
        var key = UserServiceCache.GetGroupCacheKey(tenant);
        var groups = Cache.Get<IEnumerable<Group>>(key);
        if (groups == null)
        {
            groups = await Service.GetGroupsAsync(tenant);
            Cache.Insert(key, groups, _cacheExpiration);
        }

        return groups;
    }

    public void InvalidateCache()
    {
    }

    public async Task<UserInfo> GetUserAsync(int tenant, Guid id, Expression<Func<User, UserInfo>> exp)
    {
        if (exp == null)
        {
            return await GetUserAsync(tenant, id);
        }

        return await Service.GetUserAsync(tenant, id, exp);
    }

    public async Task<IEnumerable<string>> GetDavUserEmailsAsync(int tenant)
    {
        return await Service.GetDavUserEmailsAsync(tenant);
    }
}
