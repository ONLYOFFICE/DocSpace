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
using System.Text;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

using Microsoft.Extensions.Options;

namespace ASC.Core.Data
{
    [Scope]
    public class ConfigureEFUserService : IConfigureNamedOptions<EFUserService>
    {
        private DbContextManager<UserDbContext> DbContextManager { get; }
        public string DbId { get; set; }

        public ConfigureEFUserService(DbContextManager<UserDbContext> dbContextManager)
        {
            DbContextManager = dbContextManager;
        }

        public void Configure(string name, EFUserService options)
        {
            DbId = name;
            options.LazyUserDbContext = new Lazy<UserDbContext>(() => DbContextManager.Get(name));
            options.UserDbContextManager = DbContextManager;
        }

        public void Configure(EFUserService options)
        {
            options.LazyUserDbContext = new Lazy<UserDbContext>(() => DbContextManager.Value);
            options.UserDbContextManager = DbContextManager;
        }
    }

    [Scope]
    public class EFUserService : IUserService
    {
        private static Expression<Func<User, UserInfo>> FromUserToUserInfo { get; set; }
        private static Func<UserInfo, User> FromUserInfoToUser { get; set; }
        private static Expression<Func<DbGroup, Group>> FromDbGroupToGroup { get; set; }
        private static Func<Group, DbGroup> FromGroupToDbGroup { get; set; }
        private static Expression<Func<UserGroup, UserGroupRef>> FromUserGroupToUserGroupRef { get; set; }
        private static Func<UserGroupRef, UserGroup> FromUserGroupRefToUserGroup { get; set; }

        internal UserDbContext UserDbContext { get => LazyUserDbContext.Value; }
        internal Lazy<UserDbContext> LazyUserDbContext { get; set; }
        internal DbContextManager<UserDbContext> UserDbContextManager { get; set; }
        private PasswordHasher PasswordHasher { get; }
        public MachinePseudoKeys MachinePseudoKeys { get; }
        internal string DbId { get; set; }

        static EFUserService()
        {
            FromUserToUserInfo = user => new UserInfo
            {
                ActivationStatus = user.ActivationStatus,
                BirthDate = user.Birthdate,
                CreateDate = user.CreateOn,
                CultureName = user.Culture,
                Email = user.Email,
                FirstName = user.FirstName,
                ID = user.Id,
                LastModified = user.LastModified,
                LastName = user.LastName,
                Location = user.Location,
                MobilePhone = user.Phone,
                MobilePhoneActivationStatus = user.PhoneActivation,
                Notes = user.Notes,
                Removed = user.Removed,
                Sex = user.Sex,
                Sid = user.Sid,
                SsoNameId = user.SsoNameId,
                SsoSessionId = user.SsoSessionId,
                Status = user.Status,
                Tenant = user.Tenant,
                TerminatedDate = user.TerminatedDate,
                Title = user.Title,
                UserName = user.UserName,
                WorkFromDate = user.WorkFromDate,
                Contacts = user.Contacts
            };

            FromUserInfoToUser = user => new User
            {
                ActivationStatus = user.ActivationStatus,
                Birthdate = user.BirthDate,
                CreateOn = user.CreateDate,
                Culture = user.CultureName,
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.ID,
                LastModified = user.LastModified,
                LastName = user.LastName,
                Location = user.Location,
                Phone = user.MobilePhone,
                PhoneActivation = user.MobilePhoneActivationStatus,
                Notes = user.Notes,
                Removed = user.Removed,
                Sex = user.Sex,
                Sid = user.Sid,
                SsoNameId = user.SsoNameId,
                SsoSessionId = user.SsoSessionId,
                Status = user.Status,
                Tenant = user.Tenant,
                TerminatedDate = user.TerminatedDate,
                Title = user.Title,
                UserName = user.UserName,
                WorkFromDate = user.WorkFromDate,
                Contacts = user.Contacts
            };

            FromDbGroupToGroup = group => new Group
            {
                Id = group.Id,
                Name = group.Name,
                CategoryId = group.CategoryId ?? Guid.Empty,
                ParentId = group.ParentId ?? Guid.Empty,
                Sid = group.Sid,
                Removed = group.Removed,
                LastModified = group.LastModified,
                Tenant = group.Tenant
            };

            FromGroupToDbGroup = group => new DbGroup
            {
                Id = group.Id,
                Name = group.Name,
                CategoryId = group.CategoryId,
                ParentId = group.ParentId,
                Sid = group.Sid,
                Removed = group.Removed,
                LastModified = group.LastModified,
                Tenant = group.Tenant
            };

            FromUserGroupToUserGroupRef = userGroup => new UserGroupRef
            {
                GroupId = userGroup.GroupId,
                UserId = userGroup.UserId,
                Tenant = userGroup.Tenant,
                RefType = userGroup.RefType,
                LastModified = userGroup.LastModified,
                Removed = userGroup.Removed
            };

            FromUserGroupRefToUserGroup = userGroup => new UserGroup
            {
                GroupId = userGroup.GroupId,
                UserId = userGroup.UserId,
                Tenant = userGroup.Tenant,
                RefType = userGroup.RefType,
                LastModified = userGroup.LastModified,
                Removed = userGroup.Removed
            };
        }

        public EFUserService()
        {

        }

        public EFUserService(DbContextManager<UserDbContext> userDbContextManager, PasswordHasher passwordHasher, MachinePseudoKeys machinePseudoKeys)
        {
            UserDbContextManager = userDbContextManager;
            PasswordHasher = passwordHasher;
            MachinePseudoKeys = machinePseudoKeys;
            LazyUserDbContext = new Lazy<UserDbContext>(() => UserDbContextManager.Value);
        }

        public Group GetGroup(int tenant, Guid id)
        {
            return GetGroupQuery(UserDbContext, tenant, default)
                .Where(r => r.Id == id)
                .Select(FromDbGroupToGroup)
                .FirstOrDefault();
        }

        public IDictionary<Guid, Group> GetGroups(int tenant, DateTime from)
        {
            return GetGroupQuery(UserDbContext, tenant, from)
                .Select(FromDbGroupToGroup)
                .ToDictionary(r => r.Id, r => r);
        }

        public UserInfo GetUser(int tenant, Guid id)
        {
            return GetUserQuery(UserDbContext, tenant, default)
                .Where(r => r.Id == id)
                .Select(FromUserToUserInfo)
                .FirstOrDefault();
        }

        public UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("login");

            if (Guid.TryParse(login, out var userId))
            {
                RegeneratePassword(tenant, userId);

                var pwdHash = GetPasswordHash(userId, passwordHash);
                var oldHash = Hasher.Base64Hash(passwordHash, HashAlg.SHA256);

                var q = UserDbContext.Users
                    .Where(r => !r.Removed)
                    .Where(r => r.Id == userId)
                    .Join(UserDbContext.UserSecurity, r => r.Id, r => r.UserId, (user, security) => new DbUserSecurity
                    {
                        User = user,
                        UserSecurity = security
                    })
                    .Where(r => r.UserSecurity.PwdHash == pwdHash || r.UserSecurity.PwdHash == oldHash)  //todo: remove old scheme
                    ;

                if (tenant != Tenant.DEFAULT_TENANT)
                {
                    q = q.Where(r => r.User.Tenant == tenant);
                }

                return q.Select(r => r.User).Select(FromUserToUserInfo).FirstOrDefault();
            }
            else
            {
                var q = UserDbContext.Users
                    .Where(r => !r.Removed)
                    .Where(r => r.Email == login)
                    ;
                if (tenant != Tenant.DEFAULT_TENANT)
                {
                    q = q.Where(r => r.Tenant == tenant);
                }
                var user = q.Select(FromUserToUserInfo).FirstOrDefault();
                if (user != null)
                {
                    RegeneratePassword(tenant, user.ID);

                    var pwdHash = GetPasswordHash(user.ID, passwordHash);
                    var oldHash = Hasher.Base64Hash(passwordHash, HashAlg.SHA256);

                    var count = UserDbContext.UserSecurity
                        .Where(r => r.UserId == user.ID)
                        .Where(r => r.PwdHash == pwdHash || r.PwdHash == oldHash)
                        .Count();//todo: remove old scheme

                    if (count > 0) return user;
                }

                return null;
            }
        }

        //todo: remove
        private void RegeneratePassword(int tenant, Guid userId)
        {
            var h2 = UserDbContext.UserSecurity
                .Where(r => r.Tenant == tenant)
                .Where(r => r.UserId == userId)
                .Select(r => r.PwdHashSha512)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(h2)) return;

            var password = Crypto.GetV(h2, 1, false);
            var passwordHash = PasswordHasher.GetClientPassword(password);
            SetUserPasswordHash(tenant, userId, passwordHash);
        }

        public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            IQueryable<UserGroup> q = UserDbContext.UserGroups;

            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q = q.Where(r => r.Tenant == tenant);
            }

            if (from != default)
            {
                q = q.Where(r => r.LastModified >= from);
            }

            return q.Select(FromUserGroupToUserGroupRef).ToDictionary(r => r.CreateKey(), r => r);
        }

        public DateTime GetUserPasswordStamp(int tenant, Guid id)
        {
            var stamp = UserDbContext.UserSecurity
                .Where(r => r.Tenant == tenant)
                .Where(r => r.UserId == id)
                .Select(r => r.LastModified)
                .FirstOrDefault();

            return stamp ?? DateTime.MinValue;
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            var photo = UserDbContext.Photos
                .Where(r => r.Tenant == tenant)
                .Where(r => r.UserId == id)
                .Select(r => r.Photo)
                .FirstOrDefault();

            return photo ?? new byte[0];
        }

        public IDictionary<Guid, UserInfo> GetUsers(int tenant, DateTime from)
        {
            return GetUserQuery(UserDbContext, tenant, from)
                .Select(FromUserToUserInfo)
                .ToDictionary(r => r.ID, r => r);
        }

        public IQueryable<UserInfo> GetUsers(int tenant, bool isAdmin, EmployeeStatus? employeeStatus, List<List<Guid>> includeGroups, List<Guid> excludeGroups, EmployeeActivationStatus? activationStatus, string text, string sortBy, bool sortOrderAsc, long limit, long offset, out int total, out int count)
        {
            var userDbContext = UserDbContextManager.GetNew(DbId);
            var totalQuery = userDbContext.Users.Where(r => r.Tenant == tenant);
            totalQuery = GetUserQueryForFilter(totalQuery, isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, text);
            total = totalQuery.Count();

            var q = GetUserQuery(userDbContext, tenant, default);

            q = GetUserQueryForFilter(q, isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, text);

            if (!string.IsNullOrEmpty(sortBy))
            {
                q = q.OrderBy(sortBy, sortOrderAsc);
            }

            if (offset != 0)
            {
                q = q.Skip((int)offset);
            }

            if (limit != 0)
            {
                q = q.Take((int)limit);
            }

            count = q.Count();

            return q.Select(FromUserToUserInfo);
        }

        public IQueryable<UserInfo> GetUsers(int tenant, out int total)
        {
            var userDbContext = UserDbContextManager.GetNew(DbId);
            total = userDbContext.Users.Where(r => r.Tenant == tenant).Count();
            return userDbContext.Users.Where(r => r.Tenant == tenant).Select(FromUserToUserInfo);
        }


        public void RemoveGroup(int tenant, Guid id)
        {
            RemoveGroup(tenant, id, false);
        }

        public void RemoveGroup(int tenant, Guid id, bool immediate)
        {
            var ids = CollectGroupChilds(tenant, id);
            var stringIds = ids.Select(r => r.ToString()).ToList();

            using var tr = UserDbContext.Database.BeginTransaction();

            UserDbContext.Acl.RemoveRange(UserDbContext.Acl.Where(r => r.Tenant == tenant && ids.Any(i => i == r.Subject)));
            UserDbContext.Subscriptions.RemoveRange(UserDbContext.Subscriptions.Where(r => r.Tenant == tenant && stringIds.Any(i => i == r.Recipient)));
            UserDbContext.SubscriptionMethods.RemoveRange(UserDbContext.SubscriptionMethods.Where(r => r.Tenant == tenant && stringIds.Any(i => i == r.Recipient)));

            var userGroups = UserDbContext.UserGroups.Where(r => r.Tenant == tenant && ids.Any(i => i == r.GroupId));
            var groups = UserDbContext.Groups.Where(r => r.Tenant == tenant && ids.Any(i => i == r.Id));

            if (immediate)
            {
                UserDbContext.UserGroups.RemoveRange(userGroups);
                UserDbContext.Groups.RemoveRange(groups);
            }
            else
            {
                foreach (var ug in userGroups)
                {
                    ug.Removed = true;
                    ug.LastModified = DateTime.UtcNow;
                }
                foreach (var g in groups)
                {
                    g.Removed = true;
                    g.LastModified = DateTime.UtcNow;
                }
            }

            UserDbContext.SaveChanges();
            tr.Commit();
        }

        public void RemoveUser(int tenant, Guid id)
        {
            RemoveUser(tenant, id, false);
        }

        public void RemoveUser(int tenant, Guid id, bool immediate)
        {
            using var tr = UserDbContext.Database.BeginTransaction();

            UserDbContext.Acl.RemoveRange(UserDbContext.Acl.Where(r => r.Tenant == tenant && r.Subject == id));
            UserDbContext.Subscriptions.RemoveRange(UserDbContext.Subscriptions.Where(r => r.Tenant == tenant && r.Recipient == id.ToString()));
            UserDbContext.SubscriptionMethods.RemoveRange(UserDbContext.SubscriptionMethods.Where(r => r.Tenant == tenant && r.Recipient == id.ToString()));
            UserDbContext.Photos.RemoveRange(UserDbContext.Photos.Where(r => r.Tenant == tenant && r.UserId == id));

            var userGroups = UserDbContext.UserGroups.Where(r => r.Tenant == tenant && r.UserId == id);
            var users = UserDbContext.Users.Where(r => r.Tenant == tenant && r.Id == id);
            var userSecurity = UserDbContext.UserSecurity.Where(r => r.Tenant == tenant && r.UserId == id);

            if (immediate)
            {
                UserDbContext.UserGroups.RemoveRange(userGroups);
                UserDbContext.Users.RemoveRange(users);
                UserDbContext.UserSecurity.RemoveRange(userSecurity);
            }
            else
            {
                foreach (var ug in userGroups)
                {
                    ug.Removed = true;
                    ug.LastModified = DateTime.UtcNow;
                }

                foreach (var u in users)
                {
                    u.Removed = true;
                    u.Status = EmployeeStatus.Terminated;
                    u.TerminatedDate = DateTime.UtcNow;
                    u.LastModified = DateTime.UtcNow;
                }
            }

            UserDbContext.SaveChanges();

            tr.Commit();
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            RemoveUserGroupRef(tenant, userId, groupId, refType, false);
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType, bool immediate)
        {
            using var tr = UserDbContext.Database.BeginTransaction();

            var userGroups = UserDbContext.UserGroups.Where(r => r.Tenant == tenant && r.UserId == userId && r.GroupId == groupId && r.RefType == refType);
            if (immediate)
            {
                UserDbContext.UserGroups.RemoveRange(userGroups);
            }
            else
            {
                foreach (var u in userGroups)
                {
                    u.LastModified = DateTime.UtcNow;
                    u.Removed = true;
                }
            }
            var user = UserDbContext.Users.First(r => r.Tenant == tenant && r.Id == userId);
            user.LastModified = DateTime.UtcNow;
            UserDbContext.SaveChanges();

            tr.Commit();
        }

        public Group SaveGroup(int tenant, Group group)
        {
            if (group == null) throw new ArgumentNullException("user");

            if (group.Id == default) group.Id = Guid.NewGuid();
            group.LastModified = DateTime.UtcNow;
            group.Tenant = tenant;

            var dbGroup = FromGroupToDbGroup(group);
            UserDbContext.AddOrUpdate(r => r.Groups, dbGroup);
            UserDbContext.SaveChanges();

            return group;
        }

        public UserInfo SaveUser(int tenant, UserInfo user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.UserName)) throw new ArgumentOutOfRangeException("Empty username.");

            if (user.ID == default) user.ID = Guid.NewGuid();
            if (user.CreateDate == default) user.CreateDate = DateTime.UtcNow;
            user.LastModified = DateTime.UtcNow;
            user.Tenant = tenant;
            user.UserName = user.UserName.Trim();
            user.Email = user.Email.Trim();

            using var tx = UserDbContext.Database.BeginTransaction();
            var count = UserDbContext.Users.Where(r => r.UserName == user.UserName && r.Id != user.ID && !r.Removed).Count();

            if (count != 0)
            {
                throw new ArgumentOutOfRangeException("Duplicate username.");
            }

            count = UserDbContext.Users.Where(r => r.Email == user.Email && r.Id != user.ID && !r.Removed).Count();

            if (count != 0)
            {
                throw new ArgumentOutOfRangeException("Duplicate email.");
            }

            UserDbContext.AddOrUpdate(r => r.Users, FromUserInfoToUser(user));
            UserDbContext.SaveChanges();
            tx.Commit();

            return user;
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            if (r == null) throw new ArgumentNullException("userGroupRef");

            r.LastModified = DateTime.UtcNow;
            r.Tenant = tenant;

            using var tr = UserDbContext.Database.BeginTransaction();

            UserDbContext.AddOrUpdate(r => r.UserGroups, FromUserGroupRefToUserGroup(r));

            var user = UserDbContext.Users.FirstOrDefault(a => a.Tenant == tenant && a.Id == r.UserId);
            if (user != null)
            {
                user.LastModified = r.LastModified;
            }

            UserDbContext.SaveChanges();
            tr.Commit();

            return r;
        }

        public void SetUserPasswordHash(int tenant, Guid id, string passwordHash)
        {
            var h1 = GetPasswordHash(id, passwordHash);

            var us = new UserSecurity
            {
                Tenant = tenant,
                UserId = id,
                PwdHash = h1,
                PwdHashSha512 = null,//todo: remove
                LastModified = DateTime.UtcNow
            };

            UserDbContext.AddOrUpdate(r => r.UserSecurity, us);
            UserDbContext.SaveChanges();
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            using var tr = UserDbContext.Database.BeginTransaction();

            var userPhoto = UserDbContext.Photos.FirstOrDefault(r => r.UserId == id && r.Tenant == tenant);
            if (photo != null && photo.Length != 0)
            {
                if (userPhoto == null)
                {
                    userPhoto = new UserPhoto
                    {
                        Tenant = tenant,
                        UserId = id,
                        Photo = photo
                    };
                }
                else
                {
                    userPhoto.Photo = photo;
                }

                UserDbContext.AddOrUpdate(r => r.Photos, userPhoto);
            }
            else if (userPhoto != null)
            {
                UserDbContext.Photos.Remove(userPhoto);
            }

            UserDbContext.SaveChanges();
            tr.Commit();
        }

        private IQueryable<User> GetUserQuery(UserDbContext UserDbContext, int tenant, DateTime from)
        {
            var q = UserDbContext.Users.Where(r => true);

            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q = q.Where(r => r.Tenant == tenant);
            }

            if (from != default)
            {
                q = q.Where(r => r.LastModified >= from);
            }

            return q;
        }

        private IQueryable<DbGroup> GetGroupQuery(UserDbContext UserDbContext, int tenant, DateTime from)
        {
            var q = UserDbContext.Groups.Where(r => true);

            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q = q.Where(r => r.Tenant == tenant);
            }

            if (from != default)
            {
                q = q.Where(r => r.LastModified >= from);
            }

            return q;
        }

        private IQueryable<User> GetUserQueryForFilter(
            IQueryable<User> q,
            bool isAdmin,
            EmployeeStatus? employeeStatus,
            List<List<Guid>> includeGroups,
            List<Guid> excludeGroups,
            EmployeeActivationStatus? activationStatus,
            string text)
        {

            q = q.Where(r => !r.Removed);

            if (includeGroups != null && includeGroups.Any())
            {
                Expression or = Expression.Empty();

                foreach (var ig in includeGroups)
                {
                    q = q.Where(r => r.Groups.Any(a => !a.Removed && a.Tenant == r.Tenant && a.UserId == r.Id && ig.Any(r => r == a.GroupId)));
                }
            }

            if (excludeGroups != null && excludeGroups.Any())
            {
                foreach (var eg in excludeGroups)
                {
                    q = q.Where(r => !r.Groups.Any(a => !a.Removed && a.Tenant == r.Tenant && a.UserId == r.Id && a.GroupId == eg));
                }
            }

            if (!isAdmin && employeeStatus == null)
            {
                q = q.Where(r => r.Status != EmployeeStatus.Terminated);
            }

            if (employeeStatus != null)
            {
                switch (employeeStatus)
                {
                    case EmployeeStatus.LeaveOfAbsence:
                    case EmployeeStatus.Terminated:
                        if (isAdmin)
                        {
                            q = q.Where(u => u.Status == EmployeeStatus.Terminated);
                        }
                        else
                        {
                            q = q.Where(u => false);
                        }
                        break;
                    case EmployeeStatus.All:
                        if (!isAdmin) q = q.Where(r => r.Status != EmployeeStatus.Terminated);
                        break;
                    case EmployeeStatus.Default:
                    case EmployeeStatus.Active:
                        q = q.Where(u => u.Status == EmployeeStatus.Active);
                        break;
                }
            }

            if (activationStatus != null)
            {
                q = q.Where(r => r.ActivationStatus == activationStatus.Value);
            }

            if (!string.IsNullOrEmpty(text))
            {
                q = q.Where(
                    u => u.FirstName.Contains(text, StringComparison.InvariantCultureIgnoreCase) ||
                    u.LastName.Contains(text, StringComparison.InvariantCultureIgnoreCase) ||
                    u.Title.Contains(text, StringComparison.InvariantCultureIgnoreCase) ||
                    u.Location.Contains(text, StringComparison.InvariantCultureIgnoreCase) ||
                    u.Email.Contains(text, StringComparison.InvariantCultureIgnoreCase));
            }

            return q;
        }

        private List<Guid> CollectGroupChilds(int tenant, Guid id)
        {
            var result = new List<Guid>();

            var childs = UserDbContext.Groups
                .Where(r => r.Tenant == tenant)
                .Where(r => r.ParentId == id)
                .Select(r => r.Id);
            foreach (var child in childs)
            {
                result.Add(child);
                result.AddRange(CollectGroupChilds(tenant, child));
            }

            result.Add(id);
            return result.Distinct().ToList();
        }

        public UserInfo GetUser(int tenant, Guid id, Expression<Func<User, UserInfo>> exp)
        {
            return GetUserQuery(UserDbContext, tenant, default)
                    .Where(r => r.Id == id)
                    .Select(exp ?? FromUserToUserInfo)
                    .FirstOrDefault();
        }

        protected string GetPasswordHash(Guid userId, string password)
        {
            return Hasher.Base64Hash(password + userId + Encoding.UTF8.GetString(MachinePseudoKeys.GetMachineConstant()), HashAlg.SHA512);
        }
    }

    public class DbUserSecurity
    {
        public User User { get; set; }
        public UserSecurity UserSecurity { get; set; }
    }
}