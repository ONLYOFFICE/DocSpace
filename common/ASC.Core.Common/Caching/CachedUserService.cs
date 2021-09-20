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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core.Common.EF;
using ASC.Core.Data;
using ASC.Core.Tenants;
using ASC.Core.Users;

using Microsoft.Extensions.Options;

namespace ASC.Core.Caching
{
    [Singletone]
    public class UserServiceCache
    {
        public const string USERS = "users";
        private const string GROUPS = "groups";
        public const string REFS = "refs";

        public TrustInterval TrustInterval { get; set; }
        internal ICache Cache { get; }
        internal CoreBaseSettings CoreBaseSettings { get; }
        internal ICacheNotify<UserInfoCacheItem> CacheUserInfoItem { get; }
        internal ICacheNotify<UserPhotoCacheItem> CacheUserPhotoItem { get; }
        internal ICacheNotify<GroupCacheItem> CacheGroupCacheItem { get; }
        internal ICacheNotify<UserGroupRefCacheItem> CacheUserGroupRefItem { get; }

        public UserServiceCache(
            CoreBaseSettings coreBaseSettings,
            ICacheNotify<UserInfoCacheItem> cacheUserInfoItem,
            ICacheNotify<UserPhotoCacheItem> cacheUserPhotoItem,
            ICacheNotify<GroupCacheItem> cacheGroupCacheItem,
            ICacheNotify<UserGroupRefCacheItem> cacheUserGroupRefItem, 
            ICache cache)
        {
            TrustInterval = new TrustInterval();
            Cache = cache;
            CoreBaseSettings = coreBaseSettings;
            CacheUserInfoItem = cacheUserInfoItem;
            CacheUserPhotoItem = cacheUserPhotoItem;
            CacheGroupCacheItem = cacheGroupCacheItem;
            CacheUserGroupRefItem = cacheUserGroupRefItem;

            cacheUserInfoItem.Subscribe((u) => InvalidateCache(u), CacheNotifyAction.Any);
            cacheUserPhotoItem.Subscribe((p) => Cache.Remove(p.Key), CacheNotifyAction.Remove);
            cacheGroupCacheItem.Subscribe((g) => InvalidateCache(), CacheNotifyAction.Any);

            cacheUserGroupRefItem.Subscribe((r) => UpdateUserGroupRefCache(r, true), CacheNotifyAction.Remove);
            cacheUserGroupRefItem.Subscribe((r) => UpdateUserGroupRefCache(r, false), CacheNotifyAction.InsertOrUpdate);
        }

        public void InvalidateCache()
        {
            InvalidateCache(null);
        }

        private void InvalidateCache(UserInfoCacheItem userInfo)
        {
            if (CoreBaseSettings.Personal && userInfo != null)
            {
                var key = GetUserCacheKeyForPersonal(userInfo.Tenant, userInfo.ID.FromByteString());
                Cache.Remove(key);
            }

            TrustInterval.Expire();
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
        public static string GetUserPhotoCacheKey(int tenant, Guid userId)
        {
            return tenant.ToString() + "userphoto" + userId.ToString();
        }

        public static string GetGroupCacheKey(int tenant)
        {
            return tenant.ToString() + GROUPS;
        }

        public static string GetRefCacheKey(int tenant)
        {
            return tenant.ToString() + REFS;
        }

        public static string GetUserCacheKey(int tenant)
        {
            return tenant.ToString() + USERS;
        }

        public static string GetUserCacheKeyForPersonal(int tenant, Guid userId)
        {
            return tenant.ToString() + USERS + userId;
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
            options.CacheUserInfoItem = UserServiceCache.CacheUserInfoItem;
            options.CacheUserPhotoItem = UserServiceCache.CacheUserPhotoItem;
            options.CacheGroupCacheItem = UserServiceCache.CacheGroupCacheItem;
            options.CacheUserGroupRefItem = UserServiceCache.CacheUserGroupRefItem;
            options.TrustInterval = UserServiceCache.TrustInterval;
        }
    }

    [Scope]
    public class CachedUserService : IUserService, ICachedService
    {
        internal IUserService Service { get; set; }
        internal ICache Cache { get; set; }

        internal TrustInterval TrustInterval { get; set; }
        private int getchanges;

        private TimeSpan CacheExpiration { get; set; }
        private TimeSpan DbExpiration { get; set; }
        private TimeSpan PhotoExpiration { get; set; }
        internal CoreBaseSettings CoreBaseSettings { get; set; }
        internal UserServiceCache UserServiceCache { get; set; }
        internal ICacheNotify<UserInfoCacheItem> CacheUserInfoItem { get; set; }
        internal ICacheNotify<UserPhotoCacheItem> CacheUserPhotoItem { get; set; }
        internal ICacheNotify<GroupCacheItem> CacheGroupCacheItem { get; set; }
        internal ICacheNotify<UserGroupRefCacheItem> CacheUserGroupRefItem { get; set; }

        public CachedUserService()
        {
            CacheExpiration = TimeSpan.FromMinutes(20);
            DbExpiration = TimeSpan.FromMinutes(1);
            PhotoExpiration = TimeSpan.FromMinutes(10);
        }

        public CachedUserService(
            EFUserService service,
            CoreBaseSettings coreBaseSettings,
            UserServiceCache userServiceCache
            ) : this()
        {
            Service = service ?? throw new ArgumentNullException("service");
            CoreBaseSettings = coreBaseSettings;
            UserServiceCache = userServiceCache;
            Cache = userServiceCache.Cache;
            CacheUserInfoItem = userServiceCache.CacheUserInfoItem;
            CacheUserPhotoItem = userServiceCache.CacheUserPhotoItem;
            CacheGroupCacheItem = userServiceCache.CacheGroupCacheItem;
            CacheUserGroupRefItem = userServiceCache.CacheUserGroupRefItem;
            TrustInterval = userServiceCache.TrustInterval;
        }

        public IDictionary<Guid, UserInfo> GetUsers(int tenant, DateTime from)
        {
            var users = GetUsers(tenant);
            lock (users)
            {
                return (from == default ? users.Values : users.Values.Where(u => u.LastModified >= from)).ToDictionary(u => u.ID);
            }
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
            if (id.Equals(Guid.Empty)) return null;

            if (CoreBaseSettings.Personal)
            {
                return GetUserForPersonal(tenant, id);
            }

            var users = GetUsers(tenant);
            lock (users)
            {
                users.TryGetValue(id, out var u);
                return u;
            }
        }

        public UserInfo GetUser(int tenant, string email)
        {
            return Service.GetUser(tenant, email);
        }

        public UserInfo GetUserByUserName(int tenant, string userName)
        {
            return Service.GetUserByUserName(tenant, userName);
        }


        /// <summary>
        /// For Personal only
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private UserInfo GetUserForPersonal(int tenant, Guid id)
        {
            if (!CoreBaseSettings.Personal) return GetUser(tenant, id);

            var key = UserServiceCache.GetUserCacheKeyForPersonal(tenant, id);
            var user = Cache.Get<UserInfo>(key);

            if (user == null)
            {
                user = Service.GetUser(tenant, id);

                if (user != null)
                {
                    Cache.Insert(key, user, CacheExpiration);
                }
            }

            return user;
        }

        public UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash)
        {
            return Service.GetUserByPasswordHash(tenant, login, passwordHash);
        }

        public UserInfo SaveUser(int tenant, UserInfo user)
        {
            user = Service.SaveUser(tenant, user);
            CacheUserInfoItem.Publish(new UserInfoCacheItem { ID = user.ID.ToByteString(), Tenant = tenant }, CacheNotifyAction.Any);
            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            Service.RemoveUser(tenant, id);
            CacheUserInfoItem.Publish(new UserInfoCacheItem { Tenant = tenant, ID = id.ToByteString() }, CacheNotifyAction.Any);
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            var photo = Cache.Get<byte[]>(UserServiceCache.GetUserPhotoCacheKey(tenant, id));
            if (photo == null)
            {
                photo = Service.GetUserPhoto(tenant, id);
                Cache.Insert(UserServiceCache.GetUserPhotoCacheKey(tenant, id), photo, PhotoExpiration);
            }
            return photo;
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            Service.SetUserPhoto(tenant, id, photo);
            CacheUserPhotoItem.Publish(new UserPhotoCacheItem { Key = UserServiceCache.GetUserPhotoCacheKey(tenant, id) }, CacheNotifyAction.Remove);
        }

        public DateTime GetUserPasswordStamp(int tenant, Guid id)
        {
            return Service.GetUserPasswordStamp(tenant, id);
        }

        public void SetUserPasswordHash(int tenant, Guid id, string passwordHash)
        {
            Service.SetUserPasswordHash(tenant, id, passwordHash);
        }



        public IDictionary<Guid, Group> GetGroups(int tenant, DateTime from)
        {
            var groups = GetGroups(tenant);
            lock (groups)
            {
                return (from == default ? groups.Values : groups.Values.Where(g => g.LastModified >= from)).ToDictionary(g => g.Id);
            }
        }

        public Group GetGroup(int tenant, Guid id)
        {
            var groups = GetGroups(tenant);
            lock (groups)
            {
                groups.TryGetValue(id, out var g);
                return g;
            }
        }

        public Group SaveGroup(int tenant, Group group)
        {
            group = Service.SaveGroup(tenant, group);
            CacheGroupCacheItem.Publish(new GroupCacheItem { ID = group.Id.ToString() }, CacheNotifyAction.Any);
            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            Service.RemoveGroup(tenant, id);
            CacheGroupCacheItem.Publish(new GroupCacheItem { ID = id.ToString() }, CacheNotifyAction.Any);
        }


        public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            if (CoreBaseSettings.Personal)
            {
                return new Dictionary<string, UserGroupRef>();
            }

            GetChangesFromDb();

            var key = UserServiceCache.GetRefCacheKey(tenant);
            if (!(Cache.Get<UserGroupRefStore>(key) is IDictionary<string, UserGroupRef> refs))
            {
                refs = Service.GetUserGroupRefs(tenant, default);
                Cache.Insert(key, new UserGroupRefStore(refs), CacheExpiration);
            }
            lock (refs)
            {
                return from == default ? refs : refs.Values.Where(r => r.LastModified >= from).ToDictionary(r => r.CreateKey());
            }
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            r = Service.SaveUserGroupRef(tenant, r);
            CacheUserGroupRefItem.Publish(r, CacheNotifyAction.InsertOrUpdate);
            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            Service.RemoveUserGroupRef(tenant, userId, groupId, refType);

            var r = new UserGroupRef(userId, groupId, refType) { Tenant = tenant };
            CacheUserGroupRefItem.Publish(r, CacheNotifyAction.Remove);
        }


        private IDictionary<Guid, UserInfo> GetUsers(int tenant)
        {
            GetChangesFromDb();

            var key = UserServiceCache.GetUserCacheKey(tenant);
            var users = Cache.Get<IDictionary<Guid, UserInfo>>(key);
            if (users == null)
            {
                users = Service.GetUsers(tenant, default);

                Cache.Insert(key, users, CacheExpiration);
            }
            return users;
        }

        private IDictionary<Guid, Group> GetGroups(int tenant)
        {
            GetChangesFromDb();

            var key = UserServiceCache.GetGroupCacheKey(tenant);
            var groups = Cache.Get<IDictionary<Guid, Group>>(key);
            if (groups == null)
            {
                groups = Service.GetGroups(tenant, default);
                Cache.Insert(key, groups, CacheExpiration);
            }
            return groups;
        }

        private void GetChangesFromDb()
        {
            if (!TrustInterval.Expired)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref getchanges, 1, 0) == 0)
            {
                try
                {
                    if (!TrustInterval.Expired)
                    {
                        return;
                    }

                    var starttime = TrustInterval.StartTime;
                    if (starttime != default)
                    {
                        var correction = TimeSpan.FromTicks(DbExpiration.Ticks * 3);
                        starttime = TrustInterval.StartTime.Subtract(correction);
                    }

                    TrustInterval.Start(DbExpiration);

                    //get and merge changes in cached tenants
                    foreach (var tenantGroup in Service.GetUsers(Tenant.DEFAULT_TENANT, starttime).Values.GroupBy(u => u.Tenant))
                    {
                        var users = Cache.Get<IDictionary<Guid, UserInfo>>(UserServiceCache.GetUserCacheKey(tenantGroup.Key));
                        if (users != null)
                        {
                            lock (users)
                            {
                                foreach (var u in tenantGroup)
                                {
                                    users[u.ID] = u;
                                }
                            }
                        }
                    }

                    foreach (var tenantGroup in Service.GetGroups(Tenant.DEFAULT_TENANT, starttime).Values.GroupBy(g => g.Tenant))
                    {
                        var groups = Cache.Get<IDictionary<Guid, Group>>(UserServiceCache.GetGroupCacheKey(tenantGroup.Key));
                        if (groups != null)
                        {
                            lock (groups)
                            {
                                foreach (var g in tenantGroup)
                                {
                                    groups[g.Id] = g;
                                }
                            }
                        }
                    }

                    foreach (var tenantGroup in Service.GetUserGroupRefs(Tenant.DEFAULT_TENANT, starttime).Values.GroupBy(r => r.Tenant))
                    {
                        var refs = Cache.Get<UserGroupRefStore>(UserServiceCache.GetRefCacheKey(tenantGroup.Key));
                        if (refs != null)
                        {
                            lock (refs)
                            {
                                foreach (var r in tenantGroup)
                                {
                                    refs[r.CreateKey()] = r;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Volatile.Write(ref getchanges, 0);
                }
            }
        }


        public void InvalidateCache()
        {
            UserServiceCache.InvalidateCache();
        }

        public UserInfo GetUser(int tenant, Guid id, Expression<Func<User, UserInfo>> exp)
        {
            return Service.GetUser(tenant, id, exp);
        }

        [Serializable]
        class UserPhoto
        {
            public string Key { get; set; }
        }
    }
}
