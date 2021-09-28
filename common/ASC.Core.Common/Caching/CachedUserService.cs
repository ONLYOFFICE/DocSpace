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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core.Common.EF;
using ASC.Core.Data;
using ASC.Core.Users;

using Microsoft.Extensions.Options;

namespace ASC.Core.Caching
{
    [Scope]
    public class UserServiceCache
    {
        public const string USERS = "users";
        private const string GROUPS = "groups";
        public const string REFS = "refs";

        public ScopedCache ScopedCache { get; } = new ScopedCache();

        public static string GetUserPhotoCacheKey(int tenant, Guid userId)
        {
            return tenant.ToString() + "userphoto" + userId.ToString();
        }

        public static string GetGroupCacheKey(int tenant)
        {
            return tenant.ToString() + GROUPS;
        }

        public static string GetGroupCacheKey(int tenant, Guid groupId)
        {
            return tenant.ToString() + GROUPS + groupId;
        }

        public static string GetRefCacheKey(int tenant)
        {
            return tenant.ToString() + REFS;
        }

        public static string GetRefCacheKey(int tenant, Guid groupId, UserGroupRefType refType)
        {
            return tenant.ToString() + groupId + (int)refType;
        }

        public static string GetUserCacheKey(int tenant)
        {
            return tenant.ToString() + USERS;
        }

        public static string GetUserCacheKey(int tenant, Guid userId)
        {
            return tenant.ToString() + USERS + userId;
        }

        public static string GetUserCacheKey(int tenant, string emailOrUserName)
        {
            return tenant.ToString() + USERS + emailOrUserName;
        }

        public static string GetUserCacheKeyWithLoginAndHash(int tenant, string login, string passwordHash)
        {
            return tenant.ToString() + USERS + login + passwordHash;
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
        }
    }

    [Scope]
    public class CachedUserService : IUserService, ICachedService
    {
        internal IUserService Service { get; set; }
        internal ScopedCache ScopedCache { get; set; }
        internal CoreBaseSettings CoreBaseSettings { get; set; }
        internal UserServiceCache UserServiceCache { get; set; }

        public CachedUserService() { }

        public CachedUserService(
            EFUserService service,
            CoreBaseSettings coreBaseSettings,
            UserServiceCache userServiceCache
            ) : this()
        {
            Service = service ?? throw new ArgumentNullException("service");
            CoreBaseSettings = coreBaseSettings;
            UserServiceCache = userServiceCache;
            ScopedCache = userServiceCache.ScopedCache;
        }

        public IEnumerable<UserInfo> GetUsers(int tenant, DateTime from)
        {
            var users = Service.GetUsers(tenant, from);
            return users;
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

            var key = UserServiceCache.GetUserCacheKey(tenant, id);
            var user = ScopedCache.Get<UserInfo>(key);

            if (user == null)
            {
                user = Service.GetUser(tenant, id);

                if (user != null) ScopedCache.Insert(key, user);
            }

            return user;
        }

        public UserInfo GetUser(int tenant, string email)
        {
            var key = UserServiceCache.GetUserCacheKey(tenant, email);
            var user = ScopedCache.Get<UserInfo>(key);

            if (user == null)
            {
                user = Service.GetUser(tenant, email);

                if (user != null) ScopedCache.Insert(key, user);
            }

            return user;
        }

        public UserInfo GetUserByUserName(int tenant, string userName)
        {
            var key = UserServiceCache.GetUserCacheKey(tenant, userName);
            var user = ScopedCache.Get<UserInfo>(key);

            if (user == null)
            {
                user = Service.GetUserByUserName(tenant, userName);

                if (user != null) ScopedCache.Insert(key, user);
            }

            return user;
        }

        public UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash)
        {
            var key = UserServiceCache.GetUserCacheKeyWithLoginAndHash(tenant, login, passwordHash);
            var user = ScopedCache.Get<UserInfo>(key);

            if (user == null)
            {
                user = Service.GetUserByPasswordHash(tenant, login, passwordHash);

                if (user != null) ScopedCache.Insert(key, user);
            }

            return user;
        }

        public UserInfo SaveUser(int tenant, UserInfo user)
        {
            user = Service.SaveUser(tenant, user);

            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            Service.RemoveUser(tenant, id);
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            var photo = ScopedCache.Get<byte[]>(UserServiceCache.GetUserPhotoCacheKey(tenant, id));
            if (photo == null)
            {
                photo = Service.GetUserPhoto(tenant, id);
                ScopedCache.Insert(UserServiceCache.GetUserPhotoCacheKey(tenant, id), photo);
            }
            return photo;
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            Service.SetUserPhoto(tenant, id, photo);
        }

        public DateTime GetUserPasswordStamp(int tenant, Guid id)
        {
            return Service.GetUserPasswordStamp(tenant, id);
        }

        public void SetUserPasswordHash(int tenant, Guid id, string passwordHash)
        {
            Service.SetUserPasswordHash(tenant, id, passwordHash);
        }



        public IEnumerable<Group> GetGroups(int tenant, DateTime from)
        {
            var groups = Service.GetGroups(tenant, from);

            return groups;
        }

        public Group GetGroup(int tenant, Guid id)
        {
            var key = UserServiceCache.GetGroupCacheKey(tenant, id);
            var group = ScopedCache.Get<Group>(key);

            if (group == null)
            {
                group = Service.GetGroup(tenant, id);

                if (group != null) ScopedCache.Insert(key, group);
            };

            return group;
        }

        public Group SaveGroup(int tenant, Group group)
        {
            group = Service.SaveGroup(tenant, group);
            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            Service.RemoveGroup(tenant, id);
        }

        public UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType)
        {
            var key = UserServiceCache.GetRefCacheKey(tenant, groupId, refType);
            var groupRef = ScopedCache.Get<UserGroupRef>(key);

            if (groupRef == null)
            {
                groupRef = Service.GetUserGroupRef(tenant, groupId, refType);

                if (groupRef != null) ScopedCache.Insert(key, groupRef);
            }

            return groupRef;
        }

        public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            if (CoreBaseSettings.Personal)
            {
                return new Dictionary<string, UserGroupRef>();
            }

            return Service.GetUserGroupRefs(tenant, from);
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            r = Service.SaveUserGroupRef(tenant, r);
            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            Service.RemoveUserGroupRef(tenant, userId, groupId, refType);

            var r = new UserGroupRef(userId, groupId, refType) { Tenant = tenant };
        }


        public void InvalidateCache() { }

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
