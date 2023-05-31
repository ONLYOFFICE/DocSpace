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
public class EFUserService : IUserService
{
    private readonly IDbContextFactory<UserDbContext> _dbContextFactory;
    private readonly MachinePseudoKeys _machinePseudoKeys;
    private readonly IMapper _mapper;

    public EFUserService(
        IDbContextFactory<UserDbContext> dbContextFactory,
        MachinePseudoKeys machinePseudoKeys,
        IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _machinePseudoKeys = machinePseudoKeys;
        _mapper = mapper;
    }

    public async Task<Group> GetGroupAsync(int tenant, Guid id)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return await GetGroupQuery(userDbContext, tenant)
            .Where(r => r.Id == id)
            .ProjectTo<Group>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Group>> GetGroupsAsync(int tenant)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return await GetGroupQuery(userDbContext, tenant)
            .ProjectTo<Group>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<UserInfo> GetUserAsync(int tenant, Guid id)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return await GetUserQuery(userDbContext, tenant)
            .Where(r => r.Id == id)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public UserInfo GetUser(int tenant, Guid id)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return GetUserQuery(userDbContext, tenant)
            .Where(r => r.Id == id)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
    }

    public async Task<UserInfo> GetUserAsync(int tenant, string email)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return await GetUserQuery(userDbContext, tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(r => r.Email == email && !r.Removed);
    }

    public async Task<UserInfo> GetUserByUserName(int tenant, string userName)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return await GetUserQuery(userDbContext, tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(r => r.UserName == userName && !r.Removed);
    }

    public async Task<UserInfo> GetUserByPasswordHashAsync(int tenant, string login, string passwordHash)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(login);

        using var userDbContext = _dbContextFactory.CreateDbContext();
        if (Guid.TryParse(login, out var userId))
        {
            var pwdHash = GetPasswordHash(userId, passwordHash);

            var q = GetUserQuery(userDbContext, tenant)
                .Where(r => !r.Removed)
                .Where(r => r.Id == userId)
                .Join(userDbContext.UserSecurity, r => r.Id, r => r.UserId, (user, security) => new DbUserSecurity
                {
                    User = user,
                    UserSecurity = security
                })
                .Where(r => r.UserSecurity.PwdHash == pwdHash);

            if (tenant != Tenant.DefaultTenant)
            {
                q = q.Where(r => r.UserSecurity.Tenant == tenant);
            }

            return await q.Select(r => r.User)
                .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        else
        {
            var q = GetUserQuery(userDbContext, tenant)
                .Where(r => !r.Removed)
                .Where(r => r.Email == login);

            var users = await q.ProjectTo<UserInfo>(_mapper.ConfigurationProvider).ToListAsync();
            foreach (var user in users)
            {
                var pwdHash = GetPasswordHash(user.Id, passwordHash);

                var any = await userDbContext.UserSecurity
                    .AnyAsync(r => r.UserId == user.Id && (r.PwdHash == pwdHash));

                if (any)
                {
                    return user;
                }
            }

            return null;
        }
    }

    public async Task<IEnumerable<UserInfo>> GetUsersAllTenantsAsync(IEnumerable<Guid> userIds)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        var q = userDbContext.Users
            .Where(r => userIds.Contains(r.Id))
            .Where(r => !r.Removed);

        return await q.ProjectTo<UserInfo>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public UserGroupRef GetUserGroupRef(int tenant, Guid groupId, UserGroupRefType refType)
    {
        return GetUserGroupRefInternal(tenant, groupId, refType).SingleOrDefault();
    }

    public async Task<UserGroupRef> GetUserGroupRefAsync(int tenant, Guid groupId, UserGroupRefType refType)
    {
        return await GetUserGroupRefInternal(tenant, groupId, refType).SingleOrDefaultAsync();
    }

    private IQueryable<UserGroupRef> GetUserGroupRefInternal(int tenant, Guid groupId, UserGroupRefType refType)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        IQueryable<UserGroup> q = userDbContext.UserGroups;

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
        }
        return q.Where(r => r.UserGroupId == groupId && r.RefType == refType && !r.Removed)
            .ProjectTo<UserGroupRef>(_mapper.ConfigurationProvider);
    }

    public async Task<IDictionary<string, UserGroupRef>> GetUserGroupRefsAsync(int tenant)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        IQueryable<UserGroup> q = userDbContext.UserGroups;

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
        }

        return await q.ProjectTo<UserGroupRef>(_mapper.ConfigurationProvider).ToDictionaryAsync(r => r.CreateKey(), r => r);
    }

    public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        IQueryable<UserGroup> q = userDbContext.UserGroups;

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
        }

        return q.ProjectTo<UserGroupRef>(_mapper.ConfigurationProvider)
            .AsEnumerable().ToDictionary(r => r.CreateKey(), r => r);
    }

    public async Task<DateTime> GetUserPasswordStampAsync(int tenant, Guid id)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();

        var stamp = await Queries.GetLastModifiedAsync(userDbContext, tenant, id);

        return stamp ?? DateTime.MinValue;
    }

    public async Task<byte[]> GetUserPhotoAsync(int tenant, Guid id)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();

        var photo = await Queries.GetPhotoAsync(userDbContext, tenant, id);

        return photo ?? Array.Empty<byte>();
    }

    public async Task<IEnumerable<UserInfo>> GetUsersAsync(int tenant)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return await GetUserQuery(userDbContext, tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public IQueryable<UserInfo> GetUsers(int tenant, bool isDocSpaceAdmin, EmployeeStatus? employeeStatus, List<List<Guid>> includeGroups, List<Guid> excludeGroups, EmployeeActivationStatus? activationStatus, string text, string sortBy, bool sortOrderAsc, long limit, long offset, out int total, out int count)
    {
        var userDbContext = _dbContextFactory.CreateDbContext();
        var totalQuery = GetUserQuery(userDbContext, tenant);
        totalQuery = GetUserQueryForFilter(userDbContext, totalQuery, isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, text);
        total = totalQuery.Count();

        var q = GetUserQuery(userDbContext, tenant);

        q = GetUserQueryForFilter(userDbContext, q, isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, text);

        var orderedQuery = q.OrderBy(r => r.ActivationStatus == EmployeeActivationStatus.Pending);
        q = orderedQuery;

        if (!string.IsNullOrEmpty(sortBy))
        {
            if (sortBy == "type")
            {
                var q1 = from user in q
                         join userGroup in userDbContext.UserGroups.Where(g => !g.Removed && (g.UserGroupId == Users.Constants.GroupAdmin.ID || g.UserGroupId == Users.Constants.GroupUser.ID
                                 || g.UserGroupId == Users.Constants.GroupCollaborator.ID))
                         on user.Id equals userGroup.Userid into joinedGroup
                         from @group in joinedGroup.DefaultIfEmpty()
                         select new { user, @group };

                if (sortOrderAsc)
                {
                    q = q1.OrderBy(r => r.user.ActivationStatus == EmployeeActivationStatus.Pending)
                        .ThenBy(r => r.group == null ? 2 :
                            r.group.UserGroupId == Users.Constants.GroupAdmin.ID ? 1 :
                            r.group.UserGroupId == Users.Constants.GroupCollaborator.ID ? 3 : 4)
                        .Select(r => r.user);
                }
                else
                {
                    q = q1.OrderBy(r => r.user.ActivationStatus == EmployeeActivationStatus.Pending)
                        .ThenByDescending(u => u.group == null ? 2 :
                            u.group.UserGroupId == Users.Constants.GroupAdmin.ID ? 1 :
                            u.group.UserGroupId == Users.Constants.GroupCollaborator.ID ? 3 : 4)
                        .Select(r => r.user);
                }
            }
            else
            {
                q = orderedQuery.ThenBy(sortBy, sortOrderAsc);
            }
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
        var userDbContext = _dbContextFactory.CreateDbContext();
        total = userDbContext.Users.Count(r => r.Tenant == tenant);

        return GetUserQuery(userDbContext, tenant)
            .ProjectTo<UserInfo>(_mapper.ConfigurationProvider);
    }

    public async Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime from)
    {
        var userDbContext = _dbContextFactory.CreateDbContext();

        return await Queries.GetTenantIdsAsync(userDbContext, from).ToListAsync();
    }

    public async Task RemoveGroupAsync(int tenant, Guid id)
    {
        await RemoveGroupAsync(tenant, id, false);
    }

    public async Task RemoveGroupAsync(int tenant, Guid id, bool immediate)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        var ids = await CollectGroupChildsAsync(userDbContext, tenant, id);
        var stringIds = ids.Select(r => r.ToString()).ToList();
        
        userDbContext.Acl.RemoveRange(await Queries.GetAclsByIdsAsync(userDbContext, tenant, ids).ToListAsync());
        userDbContext.Subscriptions.RemoveRange(await Queries.GetSubscriptionsByIdsAsync(userDbContext, tenant, stringIds).ToListAsync());
        userDbContext.SubscriptionMethods.RemoveRange(await Queries.GetDbSubscriptionMethodsByIdsAsync(userDbContext, tenant, stringIds).ToListAsync());

        var userGroups = Queries.GetUserGroupsByIdsAsync(userDbContext, tenant, ids);
        var groups = Queries.GetDbGroupsAsync(userDbContext, tenant, ids);

        if (immediate)
        {
            userDbContext.UserGroups.RemoveRange(await userGroups.ToListAsync());
            userDbContext.Groups.RemoveRange(await groups.ToListAsync());
        }
        else
        {
            await userGroups.ForEachAsync(ug =>
            {
                ug.Removed = true;
                ug.LastModified = DateTime.UtcNow;
            });

            await groups.ForEachAsync(g =>
            {
                g.Removed = true;
                g.LastModified = DateTime.UtcNow;
            });
        }

        await userDbContext.SaveChangesAsync();
    }

    public async Task RemoveUserAsync(int tenant, Guid id)
    {
        await RemoveUserAsync(tenant, id, false);
    }

    public async Task RemoveUserAsync(int tenant, Guid id, bool immediate)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();

        userDbContext.Acl.RemoveRange(await Queries.GetAclsAsync(userDbContext, tenant, id).ToListAsync());
        userDbContext.Subscriptions.RemoveRange(await Queries.GetSubscriptionsAsync(userDbContext, tenant, id.ToString()).ToListAsync());
        userDbContext.SubscriptionMethods.RemoveRange(await Queries.GetDbSubscriptionMethodsAsync(userDbContext, tenant, id.ToString()).ToListAsync());
        userDbContext.Photos.RemoveRange(await Queries.GetUserPhotosAsync(userDbContext, tenant, id).ToListAsync());

        var userGroups = Queries.GetUserGroupsAsync(userDbContext, tenant, id);
        var users = Queries.GetUsersAsync(userDbContext, tenant, id);

        if (immediate)
        {
            userDbContext.UserGroups.RemoveRange(await userGroups.ToListAsync());
            userDbContext.Users.RemoveRange(await users.ToListAsync());
            userDbContext.UserSecurity.RemoveRange(await Queries.GetUserSecuritiesAsync(userDbContext, tenant, id).ToListAsync());
        }
        else
        {
            await userGroups.ForEachAsync(ug =>
            {
                ug.Removed = true;
                ug.LastModified = DateTime.UtcNow;
            });

            await users.ForEachAsync(u =>
            {
                u.Removed = true;
                u.LastModified = DateTime.UtcNow;
                u.TerminatedDate = DateTime.UtcNow;
                u.Status = EmployeeStatus.Terminated;
            });
        }
        await userDbContext.SaveChangesAsync();
    }

    public async Task RemoveUserGroupRefAsync(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
    {
        await RemoveUserGroupRefAsync(tenant, userId, groupId, refType, false);
    }

    public async Task RemoveUserGroupRefAsync(int tenant, Guid userId, Guid groupId, UserGroupRefType refType, bool immediate)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();

        var userGroups = Queries.GetUserGroupsByGroupIdAsync(userDbContext, tenant, userId, groupId, refType);
        if (immediate)
        {
            userDbContext.UserGroups.RemoveRange(await userGroups.ToListAsync());
        }
        else
        {
            await userGroups.ForEachAsync(l =>
            {
                l.Removed = true;
                l.LastModified = DateTime.UtcNow;
            });
        }

        var user1 = userDbContext.Users.First(r => r.Tenant == tenant && r.Id == userId);
        var user = await Queries.GetUserAsync(userDbContext, tenant, userId);
        user.LastModified = DateTime.UtcNow;
        await userDbContext.SaveChangesAsync();
    }

    public async Task<Group> SaveGroupAsync(int tenant, Group group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (group.Id == default)
        {
            group.Id = Guid.NewGuid();
        }

        group.LastModified = DateTime.UtcNow;
        group.Tenant = tenant;

        var dbGroup = _mapper.Map<Group, DbGroup>(group);

        using var userDbContext = _dbContextFactory.CreateDbContext();
        await userDbContext.AddOrUpdateAsync(q => q.Groups, dbGroup);
        await userDbContext.SaveChangesAsync();

        return group;
    }

    public async Task<UserInfo> SaveUserAsync(int tenant, UserInfo user)
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

        using var userDbContext = _dbContextFactory.CreateDbContext();

        var any = await Queries.AnyUsersAsync(userDbContext, tenant, user.UserName, user.Id);

        if (any)
        {
            throw new ArgumentOutOfRangeException("Duplicate username.");
        }

        any = await Queries.AnyUsersByEmailAsync(userDbContext, tenant, user.Email, user.Id);

        if (any)
        {
            throw new ArgumentOutOfRangeException("Duplicate email.");
        }

        await userDbContext.AddOrUpdateAsync(q => q.Users, _mapper.Map<UserInfo, User>(user));
        await userDbContext.SaveChangesAsync();

        return user;
    }

    public async Task<UserGroupRef> SaveUserGroupRefAsync(int tenant, UserGroupRef userGroupRef)
    {
        ArgumentNullException.ThrowIfNull(userGroupRef);

        userGroupRef.LastModified = DateTime.UtcNow;
        userGroupRef.Tenant = tenant;

        using var userDbContext = _dbContextFactory.CreateDbContext();

        var user = await Queries.GetFirtsOrDefaultUserAsync(userDbContext, tenant, userGroupRef.UserId);
        if (user != null)
        {
            user.LastModified = userGroupRef.LastModified;
            await userDbContext.AddOrUpdateAsync(q => q.UserGroups, _mapper.Map<UserGroupRef, UserGroup>(userGroupRef));
            await userDbContext.SaveChangesAsync();
        }

        return userGroupRef;
    }

    public async Task SetUserPasswordHashAsync(int tenant, Guid id, string passwordHash)
    {
        var h1 = GetPasswordHash(id, passwordHash);

        var us = new UserSecurity
        {
            Tenant = tenant,
            UserId = id,
            PwdHash = h1,
            LastModified = DateTime.UtcNow
        };

        using var userDbContext = _dbContextFactory.CreateDbContext();
        await userDbContext.AddOrUpdateAsync(q => q.UserSecurity, us);
        await userDbContext.SaveChangesAsync();
    }

    public async Task SetUserPhotoAsync(int tenant, Guid id, byte[] photo)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();

        var userPhoto = await Queries.GetUserPhotoAsync(userDbContext, tenant, id);
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

            await userDbContext.AddOrUpdateAsync(q => q.Photos, userPhoto);
        }
        else if (userPhoto != null)
        {
            userDbContext.Photos.Remove(userPhoto);
        }

        await userDbContext.SaveChangesAsync();
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

    private IQueryable<DbGroup> GetGroupQuery(UserDbContext userDbContext, int tenant)
    {
        var q = userDbContext.Groups.Where(r => true);

        if (tenant != Tenant.DefaultTenant)
        {
            q = q.Where(r => r.Tenant == tenant);
        }

        return q;
    }

    private IQueryable<User> GetUserQueryForFilter(
        UserDbContext userDbContext,
        IQueryable<User> q,
        bool isDocSpaceAdmin,
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
                q = q.Where(r => userDbContext.UserGroups.Any(a => !a.Removed && a.Tenant == r.Tenant && a.Userid == r.Id && ig.Any(r => r == a.UserGroupId)));
            }
        }

        if (excludeGroups != null && excludeGroups.Count > 0)
        {
            foreach (var eg in excludeGroups)
            {
                q = q.Where(r => !userDbContext.UserGroups.Any(a => !a.Removed && a.Tenant == r.Tenant && a.Userid == r.Id && a.UserGroupId == eg));
            }
        }

        if (!isDocSpaceAdmin && employeeStatus == null)
        {
            q = q.Where(r => r.Status != EmployeeStatus.Terminated);
        }

        if (employeeStatus != null)
        {
            switch (employeeStatus)
            {
                case EmployeeStatus.LeaveOfAbsence:
                case EmployeeStatus.Terminated:
                    if (isDocSpaceAdmin)
                    {
                        q = q.Where(u => u.Status == EmployeeStatus.Terminated);
                    }
                    else
                    {
                        q = q.Where(u => false);
                    }
                    break;
                case EmployeeStatus.All:
                    if (!isDocSpaceAdmin)
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

    private async Task<List<Guid>> CollectGroupChildsAsync(UserDbContext userDbContext, int tenant, Guid id)
    {
        var result = new List<Guid>();

        var childs = Queries.GetGroupIdsAsynd(userDbContext, tenant, id);

        await foreach (var child in childs)
        {
            result.Add(child);
            result.AddRange(await CollectGroupChildsAsync(userDbContext, tenant, child));
        }

        result.Add(id);

        return result.Distinct().ToList();
    }

    public async Task<UserInfo> GetUserAsync(int tenant, Guid id, Expression<Func<User, UserInfo>> exp)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        var q = GetUserQuery(userDbContext, tenant).Where(r => r.Id == id);

        if (exp != null)
        {
            return await q.Select(exp).FirstOrDefaultAsync();
        }
        else
        {
            return await q.ProjectTo<UserInfo>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }
    }

    public async Task<IEnumerable<string>> GetDavUserEmailsAsync(int tenant)
    {
        using var userDbContext = _dbContextFactory.CreateDbContext();
        return await Queries.GetEmailsAsync(userDbContext, tenant).ToListAsync();
    }

    protected string GetPasswordHash(Guid userId, string password)
    {
        return Hasher.Base64Hash(password + userId + Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()), HashAlg.SHA512);
    }
}

public class DbUserSecurity
{
    public User User { get; set; }
    public UserSecurity UserSecurity { get; set; }
}

static file class Queries
{
    public static readonly Func<UserDbContext, int, Guid, Task<DateTime?>> GetLastModifiedAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId) =>
        ctx.UserSecurity
            .Where(r => r.Tenant == tenantId)
            .Where(r => r.UserId == userId)
            .Select(r => r.LastModified)
            .FirstOrDefault());
    
    public static readonly Func<UserDbContext, int, Guid, Task<byte[]>> GetPhotoAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId) =>
        ctx.Photos
            .Where(r => r.Tenant == tenantId)
            .Where(r => r.UserId == userId)
            .Select(r => r.Photo)
            .FirstOrDefault());
    
    public static readonly Func<UserDbContext, DateTime, IAsyncEnumerable<int>> GetTenantIdsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, DateTime from) =>
        ctx.Users
            .Where(u => u.LastModified > from)
            .Select(u => u.Tenant)
            .Distinct());
    
    public static readonly Func<UserDbContext, int, Guid, IAsyncEnumerable<Guid>> GetGroupIdsAsynd = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid parentId) =>
        ctx.Groups
            .Where(r => r.Tenant == tenantId)
            .Where(r => r.ParentId == parentId)
            .Select(r => r.Id));
    
    public static readonly Func<UserDbContext, int, IEnumerable<Guid>, IAsyncEnumerable<Acl>> GetAclsByIdsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, IEnumerable<Guid> ids) =>
        ctx.Acl
            .Where(r => r.Tenant == tenantId
                && ids.Any(i => i == r.Subject)));
    
    public static readonly Func<UserDbContext, int, IEnumerable<string>, IAsyncEnumerable<Subscription>> GetSubscriptionsByIdsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, IEnumerable<string> ids) =>
        ctx.Subscriptions
            .Where(r => r.Tenant == tenantId 
                && ids.Any(i => i == r.Recipient)));
    
    public static readonly Func<UserDbContext, int, IEnumerable<string>, IAsyncEnumerable<DbSubscriptionMethod>> GetDbSubscriptionMethodsByIdsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, IEnumerable<string> ids) =>
        ctx.SubscriptionMethods
            .Where(r => r.Tenant == tenantId
                && ids.Any(i => i == r.Recipient)));
    
    public static readonly Func<UserDbContext, int, IEnumerable<Guid>, IAsyncEnumerable<UserGroup>> GetUserGroupsByIdsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, IEnumerable<Guid> ids) =>
        ctx.UserGroups
            .Where(r => r.Tenant == tenantId
            && ids.Any(i => i == r.UserGroupId)));
    
    public static readonly Func<UserDbContext, int, IEnumerable<Guid>, IAsyncEnumerable<DbGroup>> GetDbGroupsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, IEnumerable<Guid> ids) =>
        ctx.Groups
            .Where(r => r.Tenant == tenantId
                && ids.Any(i => i == r.Id)));
    
    public static readonly Func<UserDbContext, int, Guid, IAsyncEnumerable<Acl>> GetAclsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid id) =>
        ctx.Acl
            .Where(r => r.Tenant == tenantId
                && r.Subject == id));
    
    public static readonly Func<UserDbContext, int, string, IAsyncEnumerable<Subscription>> GetSubscriptionsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, string id) =>
        ctx.Subscriptions
            .Where(r => r.Tenant == tenantId 
                && r.Recipient == id));
    
    public static readonly Func<UserDbContext, int, string, IAsyncEnumerable<DbSubscriptionMethod>> GetDbSubscriptionMethodsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, string id) =>
        ctx.SubscriptionMethods
            .Where(r => r.Tenant == tenantId 
                && r.Recipient == id));
    
    public static readonly Func<UserDbContext, int, Guid, IAsyncEnumerable<UserPhoto>> GetUserPhotosAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId) =>
        ctx.Photos
            .Where(r => r.Tenant == tenantId 
                && r.UserId == userId));
    
    public static readonly Func<UserDbContext, int, Guid, IAsyncEnumerable<UserGroup>> GetUserGroupsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId) =>
        ctx.UserGroups
            .Where(r => r.Tenant == tenantId
                && r.Userid == userId));
    
    public static readonly Func<UserDbContext, int, Guid, IAsyncEnumerable<User>> GetUsersAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid Id) =>
        ctx.Users.Where(r => r.Tenant == tenantId
            && r.Id == Id));
    
    public static readonly Func<UserDbContext, int, Guid, IAsyncEnumerable<UserSecurity>> GetUserSecuritiesAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId) =>
        ctx.UserSecurity
            .Where(r => r.Tenant == tenantId
                && r.UserId == userId));
    
    public static readonly Func<UserDbContext, int, Guid, Guid, UserGroupRefType, IAsyncEnumerable<UserGroup>> GetUserGroupsByGroupIdAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId, Guid groupId, UserGroupRefType refType) =>
        ctx.UserGroups
            .Where(r => r.Tenant == tenantId
                && r.Userid == userId 
                && r.UserGroupId == groupId 
                && r.RefType == refType));
    
    public static readonly Func<UserDbContext, int, Guid, Task<User>> GetUserAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId) =>
        ctx.Users
            .First(r => r.Tenant == tenantId
                && r.Id == userId));
    
    public static readonly Func<UserDbContext, int, string, Guid, Task<bool>> AnyUsersAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, string userName, Guid id) =>
        ctx.Users
            .Where(r => r.Tenant != Tenant.DefaultTenant && r.Tenant == tenantId)
            .Any(r => r.UserName == userName && r.Id != id && !r.Removed));
    
    public static readonly Func<UserDbContext, int, string, Guid, Task<bool>> AnyUsersByEmailAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, string email, Guid id) =>
        ctx.Users
            .Where(r => r.Tenant != Tenant.DefaultTenant && r.Tenant == tenantId)
            .Any(r => r.Email == email && r.Id != id && !r.Removed));
    
    public static readonly Func<UserDbContext, int, Guid, Task<User>> GetFirtsOrDefaultUserAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid id) =>
        ctx.Users
            .Where(r => r.Tenant != Tenant.DefaultTenant && r.Tenant == tenantId)
            .FirstOrDefault(a => a.Id == id));
    
    public static readonly Func<UserDbContext, int, Guid, Task<UserPhoto>> GetUserPhotoAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId, Guid userId) =>
        ctx.Photos
            .FirstOrDefault(r => r.UserId == userId
                         && r.Tenant == tenantId));
    
    public static readonly Func<UserDbContext, int, IAsyncEnumerable<string>> GetEmailsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
    (UserDbContext ctx, int tenantId) =>
        (from usersDav in ctx.UsersDav
        join users in ctx.Users on new { tenant = usersDav.TenantId, userId = usersDav.UserId } equals new { tenant = users.Tenant, userId = users.Id }
        where usersDav.TenantId == tenantId
        select users.Email)
            .Distinct());
}