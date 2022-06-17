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

namespace ASC.Core.Data;

[Scope]
public class ConfigureEFUserService : IConfigureNamedOptions<EFUserService>
{
    private readonly DbContextManager<UserDbContext> _dbContextManager;

    public ConfigureEFUserService(DbContextManager<UserDbContext> dbContextManager)
    {
        _dbContextManager = dbContextManager;
    }

    public void Configure(string name, EFUserService options)
    {
        options.DbId = name;
        options.LazyUserDbContext = new Lazy<UserDbContext>(() => _dbContextManager.Get(name));
        options.UserDbContextManager = _dbContextManager;
    }

    public void Configure(EFUserService options)
    {
        options.LazyUserDbContext = new Lazy<UserDbContext>(() => _dbContextManager.Value);
        options.UserDbContextManager = _dbContextManager;
    }
}

[Scope]
public class EFUserService : IUserService
{
    internal UserDbContext UserDbContext => LazyUserDbContext.Value;
    internal Lazy<UserDbContext> LazyUserDbContext;
    internal DbContextManager<UserDbContext> UserDbContextManager;
    private readonly PasswordHasher _passwordHasher;
    public readonly MachinePseudoKeys MachinePseudoKeys;
    internal string DbId { get; set; }
    private readonly IMapper _mapper;

    public EFUserService(
        DbContextManager<UserDbContext> userDbContextManager,
        PasswordHasher passwordHasher,
        MachinePseudoKeys machinePseudoKeys,
        IMapper mapper)
    {
        UserDbContextManager = userDbContextManager;
        _passwordHasher = passwordHasher;
        MachinePseudoKeys = machinePseudoKeys;
        LazyUserDbContext = new Lazy<UserDbContext>(() => UserDbContextManager.Value);
        _mapper = mapper;
    }

    public Group GetGroup(int tenant, Guid id)
    {
        return GetGroupQuery(tenant)
            .Where(r => r.Id == id)
            .ProjectTo<Group>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
    }

    public IEnumerable<Group> GetGroups(int tenant)
    {
        return GetGroupQuery(tenant)
            .ProjectTo<Group>(_mapper.ConfigurationProvider)
            .ToList();
    }

    public UserInfo GetUser(int tenant, Guid id)
    {
        return GetUserQuery(tenant)
            .Where(r => r.Id == id)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
    }

    public UserInfo GetUser(int tenant, string email)
    {
        return GetUserQuery(tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefault(r => r.Email == email && !r.Removed);
    }

    public UserInfo GetUserByUserName(int tenant, string userName)
    {
        return GetUserQuery(tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefault(r => r.UserName == userName && !r.Removed);
    }

    public UserInfo GetUserByPasswordHash(int tenant, string login, string passwordHash)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(login);

        if (Guid.TryParse(login, out var userId))
        {
            var pwdHash = GetPasswordHash(userId, passwordHash);

            var q = GetUserQuery(tenant)
                .Where(r => !r.Removed)
                .Where(r => r.Id == userId)
                .Join(UserDbContext.UserSecurity, r => r.Id, r => r.UserId, (user, security) => new DbUserSecurity
                {
                    User = user,
                    UserSecurity = security
                })
                .Where(r => r.UserSecurity.PwdHash == pwdHash)
                ;

            if (tenant != Tenant.DefaultTenant)
            {
                q = q.Where(r => r.UserSecurity.Tenant == tenant);
            }

            return q.Select(r => r.User)
                .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }
        else
        {
            var q = GetUserQuery(tenant)
                .Where(r => !r.Removed)
                .Where(r => r.Email == login);

            var users = q.ProjectTo<UserInfo>(_mapper.ConfigurationProvider).ToList();
            foreach (var user in users)
            {
                var pwdHash = GetPasswordHash(user.Id, passwordHash);

                var any = UserDbContext.UserSecurity
                    .Any(r => r.UserId == user.Id && (r.PwdHash == pwdHash));

                if (any)
                {
                    return user;
                }
            }

            return null;
        }
    }

    public IEnumerable<UserInfo> GetUsersAllTenants(IEnumerable<Guid> userIds)
    {
        var q = UserDbContext.Users
            .Where(r => userIds.Contains(r.Id))
            .Where(r => !r.Removed);

        return q.ProjectTo<UserInfo>(_mapper.ConfigurationProvider).ToList();
    }

    public UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType)
    {
        IQueryable<UserGroup> q = UserDbContext.UserGroups;

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
        }

        return q.Where(r => r.GroupId == groupId && r.RefType == refType && !r.Removed)
            .ProjectTo<UserGroupRef>(_mapper.ConfigurationProvider).SingleOrDefault();
    }

    public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant)
    {
        IQueryable<UserGroup> q = UserDbContext.UserGroups;

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
        }

        return q.ProjectTo<UserGroupRef>(_mapper.ConfigurationProvider)
            .AsEnumerable().ToDictionary(r => r.CreateKey(), r => r);
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

        return photo ?? Array.Empty<byte>();
    }

    public IEnumerable<UserInfo> GetUsers(int tenant)
    {
        return GetUserQuery(tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .ToList();
    }

    public IQueryable<UserInfo> GetUsers(int tenant, bool isAdmin, EmployeeStatus? employeeStatus, List<List<Guid>> includeGroups, List<Guid> excludeGroups, EmployeeActivationStatus? activationStatus, string text, string sortBy, bool sortOrderAsc, long limit, long offset, out int total, out int count)
    {
        var userDbContext = UserDbContextManager.GetNew(DbId);
        var totalQuery = GetUserQuery(userDbContext, tenant);
        totalQuery = GetUserQueryForFilter(totalQuery, isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, text);
        total = totalQuery.Count();

        var q = GetUserQuery(userDbContext, tenant);

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

        return q.ProjectTo<UserInfo>(_mapper.ConfigurationProvider);
    }

    public IQueryable<UserInfo> GetUsers(int tenant, out int total)
    {
        var userDbContext = UserDbContextManager.GetNew(DbId);
        total = userDbContext.Users.Count(r => r.Tenant == tenant);

        return GetUserQuery(userDbContext, tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider);
    }

    public void RemoveGroup(int tenant, Guid id)
    {
        RemoveGroup(tenant, id, false);
    }

    public void RemoveGroup(int tenant, Guid id, bool immediate)
    {
        var ids = CollectGroupChilds(tenant, id);
        var stringIds = ids.Select(r => r.ToString()).ToList();

        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
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
        });
    }

    public void RemoveUser(int tenant, Guid id)
    {
        RemoveUser(tenant, id, false);
    }

    public void RemoveUser(int tenant, Guid id, bool immediate)
    {
        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
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
        });
    }

    public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
    {
        RemoveUserGroupRef(tenant, userId, groupId, refType, false);
    }

    public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType, bool immediate)
    {
        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
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
        });
    }

    public Group SaveGroup(int tenant, Group group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (group.Id == default)
        {
            group.Id = Guid.NewGuid();
        }

        group.LastModified = DateTime.UtcNow;
        group.Tenant = tenant;

        var dbGroup = _mapper.Map<Group, DbGroup>(group);
        UserDbContext.AddOrUpdate(r => r.Groups, dbGroup);
        UserDbContext.SaveChanges();

        return group;
    }

    public UserInfo SaveUser(int tenant, UserInfo user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrEmpty(user.UserName))
        {
            throw new ArgumentOutOfRangeException("Empty username.");
        }

        if (user.Id == default)
        {
            user.Id = Guid.NewGuid();
        }

        if (user.CreateDate == default)
        {
            user.CreateDate = DateTime.UtcNow;
        }

        user.LastModified = DateTime.UtcNow;
        user.Tenant = tenant;
        user.UserName = user.UserName.Trim();
        user.Email = user.Email.Trim();

        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = UserDbContext.Database.BeginTransaction();
            var any = GetUserQuery(tenant)
                .Any(r => r.UserName == user.UserName && r.Id != user.Id && !r.Removed);

            if (any)
            {
                throw new ArgumentOutOfRangeException("Duplicate username.");
            }

            any = GetUserQuery(tenant)
                .Any(r => r.Email == user.Email && r.Id != user.Id && !r.Removed);

            if (any)
            {
                throw new ArgumentOutOfRangeException("Duplicate email.");
            }

            UserDbContext.AddOrUpdate(r => r.Users, _mapper.Map<UserInfo, User>(user));
            UserDbContext.SaveChanges();
            tx.Commit();
        });

        return user;
    }

    public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef userGroupRef)
    {
        ArgumentNullException.ThrowIfNull(userGroupRef);

        userGroupRef.LastModified = DateTime.UtcNow;
        userGroupRef.Tenant = tenant;

        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tr = UserDbContext.Database.BeginTransaction();

            var user = GetUserQuery(tenant).FirstOrDefault(a => a.Tenant == tenant && a.Id == userGroupRef.UserId);
            if (user != null)
            {
                user.LastModified = userGroupRef.LastModified;
                UserDbContext.AddOrUpdate(r => r.UserGroups, _mapper.Map<UserGroupRef, UserGroup>(userGroupRef));
            }

            UserDbContext.SaveChanges();
            tr.Commit();
        });



        return userGroupRef;
    }

    public void SetUserPasswordHash(int tenant, Guid id, string passwordHash)
    {
        var h1 = GetPasswordHash(id, passwordHash);

        var us = new UserSecurity
        {
            Tenant = tenant,
            UserId = id,
            PwdHash = h1,
            LastModified = DateTime.UtcNow
        };

        UserDbContext.AddOrUpdate(r => r.UserSecurity, us);
        UserDbContext.SaveChanges();
    }

    public void SetUserPhoto(int tenant, Guid id, byte[] photo)
    {
        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
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
        });
    }

    private IQueryable<User> GetUserQuery(int tenant)
    {
        return GetUserQuery(UserDbContext, tenant);
    }

    private IQueryable<User> GetUserQuery(UserDbContext userDbContext, int tenant)
    {
        var q = userDbContext.Users.AsQueryable();
        var where = false;

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
            where = true;
        }

        if (!where)
        {
            q = q.Where(r => 1 == 0);
        }

        return q;
    }

    private IQueryable<DbGroup> GetGroupQuery(int tenant)
    {
        var q = UserDbContext.Groups.Where(r => true);

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
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

        if (includeGroups != null && includeGroups.Count > 0)
        {
            foreach (var ig in includeGroups)
            {
                q = q.Where(r => r.Groups.Any(a => !a.Removed && a.Tenant == r.Tenant && a.UserId == r.Id && ig.Any(r => r == a.GroupId)));
            }
        }

        if (excludeGroups != null && excludeGroups.Count > 0)
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
                    if (!isAdmin)
                    {
                        q = q.Where(r => r.Status != EmployeeStatus.Terminated);
                    }

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
                u => u.FirstName.Contains(text) ||
                u.LastName.Contains(text) ||
                u.Title.Contains(text) ||
                u.Location.Contains(text) ||
                u.Email.Contains(text));
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
        var q = GetUserQuery(tenant).Where(r => r.Id == id);

        if (exp != null)
        {
            return q.Select(exp).FirstOrDefault();
        }
        else
        {
            return q.ProjectTo<UserInfo>(_mapper.ConfigurationProvider).FirstOrDefault();
        }
    }

    public IEnumerable<string> GetDavUserEmails(int tenant)
    {
        return (from usersDav in UserDbContext.UsersDav
                join users in UserDbContext.Users on new { tenant = usersDav.TenantId, userId = usersDav.UserId } equals new { tenant = users.Tenant, userId = users.Id }
                where usersDav.TenantId == tenant
                select users.Email)
                .Distinct()
                .ToList();
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
