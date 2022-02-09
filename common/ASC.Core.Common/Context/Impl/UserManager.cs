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
using Constants = ASC.Core.Users.Constants;

namespace ASC.Core
{
    [Singletone]
    public class UserManagerConstants
    {
        public IDictionary<Guid, UserInfo> SystemUsers { get; }
        internal Constants Constants { get; }

        public UserManagerConstants(Constants constants)
        {
            SystemUsers = Configuration.Constants.SystemAccounts.ToDictionary(a => a.ID, a => new UserInfo { Id = a.ID, LastName = a.Name });
            SystemUsers[Constants.LostUser.Id] = Constants.LostUser;
            SystemUsers[Constants.OutsideUser.Id] = Constants.OutsideUser;
            SystemUsers[constants.NamingPoster.Id] = constants.NamingPoster;
            Constants = constants;
        }
    }

    [Scope]
    public class UserManager
    {
        private IDictionary<Guid, UserInfo> SystemUsers => _userManagerConstants.SystemUsers;
        private Tenant Tenant => _tenant ??= _tenantManager.GetCurrentTenant();

        private readonly IHttpContextAccessor _accessor;
        private readonly IUserService _userService;
        private readonly TenantManager _tenantManager;
        private readonly PermissionContext _permissionContext;
        private readonly UserManagerConstants _userManagerConstants;
        private readonly CoreBaseSettings _coreBaseSettings;
        private readonly Constants _constants;
        private readonly IMapper _mapper;
        private Tenant _tenant;
        
        public UserManager() { }

        public UserManager(
            IUserService service,
            TenantManager tenantManager,
            PermissionContext permissionContext,
            UserManagerConstants userManagerConstants,
            CoreBaseSettings coreBaseSettings,
            IMapper mapper)
        {
            _userService = service;
            _tenantManager = tenantManager;
            _permissionContext = permissionContext;
            _userManagerConstants = userManagerConstants;
            _coreBaseSettings = coreBaseSettings;
            _constants = _userManagerConstants.Constants;
            _mapper = mapper;
        }

        public UserManager(
            IUserService service,
            TenantManager tenantManager,
            PermissionContext permissionContext,
            UserManagerConstants userManagerConstants,
            CoreBaseSettings coreBaseSettings,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : this(service, tenantManager, permissionContext, userManagerConstants, coreBaseSettings, mapper)
        {
            _accessor = httpContextAccessor;
        }

        public void ClearCache()
        {
            if (_userService is ICachedService service)
            {
                service.InvalidateCache();
            }
        }

        #region Users

        public UserInfo[] GetUsers()
        {
            return GetUsers(EmployeeStatus.Default);
        }

        public UserInfo[] GetUsers(EmployeeStatus status)
        {
            return GetUsers(status, EmployeeType.All);
        }

        public UserInfo[] GetUsers(EmployeeStatus status, EmployeeType type)
        {
            var users = GetUsersInternal().Where(u => (u.Status & status) == u.Status);
            switch (type)
            {
                case EmployeeType.User:
                    users = users.Where(u => !u.IsVisitor(this));
                    break;
                case EmployeeType.Visitor:
                    users = users.Where(u => u.IsVisitor(this));
                    break;
            }

            return users.ToArray();
        }

        public IQueryable<UserInfo> GetUsers(
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
            return _userService.GetUsers(Tenant.Id, isAdmin, employeeStatus, includeGroups, excludeGroups, 
                activationStatus, text, sortBy, sortOrderAsc, limit, offset, out total, out count);
        }

        public string[] GetUserNames(EmployeeStatus status)
        {
            return GetUsers(status)
                .Select(u => u.UserName)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        public UserInfo GetUserByUserName(string username)
        {
            var u = _userService.GetUserByUserName(_tenantManager.GetCurrentTenant().Id, username);

            return u ?? Constants.LostUser;
        }

        public UserInfo GetUserBySid(string sid)
        {
            return GetUsersInternal()
                .FirstOrDefault(u => u.Sid != null && string.Compare(u.Sid, sid, 
                StringComparison.CurrentCultureIgnoreCase) == 0) ?? Constants.LostUser;
        }

        public UserInfo GetSsoUserByNameId(string nameId)
        {
            return GetUsersInternal()
                .FirstOrDefault(u => !string.IsNullOrEmpty(u.SsoNameId) 
                && string.Compare(u.SsoNameId, nameId, StringComparison.CurrentCultureIgnoreCase) == 0) ?? Constants.LostUser;
        }
        public bool IsUserNameExists(string username)
        {
            return GetUserNames(EmployeeStatus.All)
                .Contains(username, StringComparer.CurrentCultureIgnoreCase);
        }

        public UserInfo GetUsers(Guid id)
        {
            if (IsSystemUser(id))
            {
                return SystemUsers[id];
            }

            var u = _userService.GetUser(Tenant.Id, id);

            return u != null && !u.Removed ? u : Constants.LostUser;
        }

        public UserInfo GetUser(Guid id, Expression<Func<User, UserInfo>> exp)
        {
            if (IsSystemUser(id))
            {
                return SystemUsers[id];
            }

            var u = _userService.GetUser(Tenant.Id, id, exp);

            return u != null && !u.Removed ? u : Constants.LostUser;
        }

        public UserInfo GetUsersByPasswordHash(int tenant, string login, string passwordHash)
        {
            var u = _userService.GetUserByPasswordHash(tenant, login, passwordHash);

            return u != null && !u.Removed ? u : Constants.LostUser;
        }

        public bool UserExists(Guid id)
        {
            return UserExists(GetUsers(id));
        }

        public bool UserExists(UserInfo user)
        {
            return !user.Equals(Constants.LostUser);
        }

        public bool IsSystemUser(Guid id)
        {
            return SystemUsers.ContainsKey(id);
        }

        public UserInfo GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Constants.LostUser;
            }

            var u = _userService.GetUser(Tenant.Id, email);

            return u != null && !u.Removed ? u : Constants.LostUser;
        }

        public UserInfo[] Search(string text, EmployeeStatus status)
        {
            return Search(text, status, Guid.Empty);
        }

        public UserInfo[] Search(string text, EmployeeStatus status, Guid groupId)
        {
            if (text == null || text.Trim() == string.Empty)
            {
                return Array.Empty<UserInfo>();
            }

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
            {
                return Array.Empty<UserInfo>();
            }

            var users = groupId == Guid.Empty ?
                GetUsers(status) :
                GetUsersByGroup(groupId).Where(u => (u.Status & status) == status);

            var findUsers = new List<UserInfo>();

            foreach (var user in users)
            {
                var properties = new string[]
                {
                    user.LastName ?? string.Empty,
                    user.FirstName ?? string.Empty,
                    user.Title ?? string.Empty,
                    user.Location ?? string.Empty,
                    user.Email ?? string.Empty,
                };

                if (IsPropertiesContainsWords(properties, words))
                {
                    findUsers.Add(user);
                }
            }

            return findUsers.ToArray();
        }

        public UserInfo SaveUserInfo(UserInfo u, bool isVisitor = false)
        {
            if (IsSystemUser(u.Id))
            {
                return SystemUsers[u.Id];
            }

            if (u.Id == Guid.Empty)
            {
                _permissionContext.DemandPermissions(Constants.Action_AddRemoveUser);
            }
            else
            {
                _permissionContext.DemandPermissions(new UserSecurityProvider(u.Id), Constants.Action_EditUser);
            }

            if (!_coreBaseSettings.Personal)
            {
                if (_constants.MaxEveryoneCount <= GetUsersByGroup(Constants.GroupEveryone.ID).Length)
                {
                    throw new TenantQuotaException("Maximum number of users exceeded");
                }

                if (u.Status == EmployeeStatus.Active)
                {
                    if (isVisitor)
                    {
                        var maxUsers = _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).ActiveUsers;
                        var visitors = _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).Free ? 0 : _constants.CoefficientOfVisitors;

                        if (!_coreBaseSettings.Standalone && GetUsersByGroup(Constants.GroupVisitor.ID).Length > visitors * maxUsers)
                        {
                            throw new TenantQuotaException("Maximum number of visitors exceeded");
                        }
                    }
                    else
                    {
                        var q = _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id);
                        if (q.ActiveUsers < GetUsersByGroup(Constants.GroupUser.ID).Length)
                        {
                            throw new TenantQuotaException(string.Format("Exceeds the maximum active users ({0})", q.ActiveUsers));
                        }
                    }
                }
            }

            if (u.Status == EmployeeStatus.Terminated && u.Id == _tenantManager.GetCurrentTenant().OwnerId)
            {
                throw new InvalidOperationException("Can not disable tenant owner.");
            }

            var newUser = _userService.SaveUser(_tenantManager.GetCurrentTenant().Id, u);

            return newUser;
        }

        public void DeleteUser(Guid id)
        {
            if (IsSystemUser(id))
            {
                return;
            }

            _permissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

            if (id == Tenant.OwnerId)
            {
                throw new InvalidOperationException("Can not remove tenant owner.");
            }

            _userService.RemoveUser(Tenant.Id, id);
        }

        public void SaveUserPhoto(Guid id, byte[] photo)
        {
            if (IsSystemUser(id))
            {
                return;
            }

            _permissionContext.DemandPermissions(new UserSecurityProvider(id), Constants.Action_EditUser);

            _userService.SetUserPhoto(Tenant.Id, id, photo);
        }

        public byte[] GetUserPhoto(Guid id)
        {
            if (IsSystemUser(id))
            {
                return null;
            }

            return _userService.GetUserPhoto(Tenant.Id, id);
        }

        public List<GroupInfo> GetUserGroups(Guid id)
        {
            return GetUserGroups(id, IncludeType.Distinct, Guid.Empty);
        }

        public List<GroupInfo> GetUserGroups(Guid id, Guid categoryID)
        {
            return GetUserGroups(id, IncludeType.Distinct, categoryID);
        }

        public List<GroupInfo> GetUserGroups(Guid userID, IncludeType includeType)
        {
            return GetUserGroups(userID, includeType, null);
        }

        public bool IsUserInGroup(Guid userId, Guid groupId)
        {
            return IsUserInGroupInternal(userId, groupId, GetRefsInternal());
        }

        public UserInfo[] GetUsersByGroup(Guid groupId, EmployeeStatus employeeStatus = EmployeeStatus.Default)
        {
            var refs = GetRefsInternal();

            return GetUsers(employeeStatus).Where(u => IsUserInGroupInternal(u.Id, groupId, refs)).ToArray();
        }

        public void AddUserIntoGroup(Guid userId, Guid groupId)
        {
            if (Constants.LostUser.Id == userId || Constants.LostGroupInfo.ID == groupId)
            {
                return;
            }

            _permissionContext.DemandPermissions(Constants.Action_EditGroups);

            _userService.SaveUserGroupRef(Tenant.Id, new UserGroupRef(userId, groupId, UserGroupRefType.Contains));

            ResetGroupCache(userId);
        }

        public void RemoveUserFromGroup(Guid userId, Guid groupId)
        {
            if (Constants.LostUser.Id == userId || Constants.LostGroupInfo.ID == groupId)
            {
                return;
            }

            _permissionContext.DemandPermissions(Constants.Action_EditGroups);

            _userService.RemoveUserGroupRef(Tenant.Id, userId, groupId, UserGroupRefType.Contains);

            ResetGroupCache(userId);
        }

        internal List<GroupInfo> GetUserGroups(Guid userID, IncludeType includeType, Guid? categoryId)
        {
            if (_coreBaseSettings.Personal)
            {
                return new List<GroupInfo> { Constants.GroupUser, Constants.GroupEveryone };
            }

            var httpRequestDictionary = new HttpRequestDictionary<List<GroupInfo>>(_accessor?.HttpContext, "GroupInfo");
            var result = httpRequestDictionary.Get(userID.ToString());
            if (result != null)
            {
                if (categoryId.HasValue)
                {
                    result = result.Where(r => r.CategoryID.Equals(categoryId.Value)).ToList();
                }

                return result;
            }

            result = new List<GroupInfo>();
            var distinctUserGroups = new List<GroupInfo>();

            var refs = GetRefsInternal();
            IEnumerable<UserGroupRef> userRefs = null;

            if (refs is UserGroupRefStore store)
            {
                userRefs = store.GetRefsByUser(userID);
            }

            var userRefsContainsNotRemoved = userRefs?.Where(r => !r.Removed && r.RefType == UserGroupRefType.Contains).ToList();

            foreach (var g in GetGroupsInternal().Where(g => !categoryId.HasValue || g.CategoryID == categoryId))
            {
                if (((g.CategoryID == Constants.SysGroupCategoryId || userRefs == null) && IsUserInGroupInternal(userID, g.ID, refs)) ||
                    (userRefsContainsNotRemoved != null && userRefsContainsNotRemoved.Any(r => r.GroupId == g.ID)))
                {
                    distinctUserGroups.Add(g);
                }
            }

            if (IncludeType.Distinct == (includeType & IncludeType.Distinct))
            {
                result.AddRange(distinctUserGroups);
            }

            result.Sort((group1, group2) => string.Compare(group1.Name, group2.Name, StringComparison.Ordinal));

            httpRequestDictionary.Add(userID.ToString(), result);

            if (categoryId.HasValue)
            {
                result = result.Where(r => r.CategoryID.Equals(categoryId.Value)).ToList();
            }

            return result;
        }

        internal void ResetGroupCache(Guid userID)
        {
            new HttpRequestDictionary<List<GroupInfo>>(_accessor?.HttpContext, "GroupInfo").Reset(userID.ToString());
            new HttpRequestDictionary<List<Guid>>(_accessor?.HttpContext, "GroupInfoID").Reset(userID.ToString());
        }

        #endregion Users


        #region Company

        public GroupInfo[] GetDepartments()
        {
            return GetGroups();
        }

        public Guid GetDepartmentManager(Guid deparmentID)
        {
            var groupRef = _userService.GetUserGroupRef(Tenant.Id, deparmentID, UserGroupRefType.Manager);

            return groupRef == null ? Guid.Empty : groupRef.UserId;
        }

        public void SetDepartmentManager(Guid deparmentID, Guid userID)
        {
            var managerId = GetDepartmentManager(deparmentID);

            if (managerId != Guid.Empty)
            {
                _userService.RemoveUserGroupRef(
                    Tenant.Id,
                    managerId, deparmentID, UserGroupRefType.Manager);
            }

            if (userID != Guid.Empty)
            {
                _userService.SaveUserGroupRef(
                   Tenant.Id,
                   new UserGroupRef(userID, deparmentID, UserGroupRefType.Manager));
            }
        }

        public UserInfo GetCompanyCEO()
        {
            var id = GetDepartmentManager(Guid.Empty);

            return id != Guid.Empty ? GetUsers(id) : null;
        }

        public void SetCompanyCEO(Guid userId)
        {
            SetDepartmentManager(Guid.Empty, userId);
        }

        #endregion Company


        #region Groups

        public GroupInfo[] GetGroups()
        {
            return GetGroups(Guid.Empty);
        }

        public GroupInfo[] GetGroups(Guid categoryID)
        {
            return GetGroupsInternal()
                .Where(g => g.CategoryID == categoryID)
                .ToArray();
        }

        public GroupInfo GetGroupInfo(Guid groupID)
        {
            var group = _userService.GetGroup(Tenant.Id, groupID);

            return new GroupInfo
            {
                ID = group.Id,
                CategoryID = group.CategoryId,
                Name = group.Name,
                Sid = group.Sid
            };
        }

        public GroupInfo GetGroupInfoBySid(string sid)
        {
            return GetGroupsInternal()
                .SingleOrDefault(g => g.Sid == sid) ?? Constants.LostGroupInfo;
        }

        public GroupInfo SaveGroupInfo(GroupInfo g)
        {
            if (Constants.LostGroupInfo.Equals(g))
            {
                return Constants.LostGroupInfo;
            }

            if (Constants.BuildinGroups.Any(b => b.ID == g.ID))
            {
                return Constants.BuildinGroups.Single(b => b.ID == g.ID);
            }

            _permissionContext.DemandPermissions(Constants.Action_EditGroups);

            var newGroup = _userService.SaveGroup(Tenant.Id, _mapper.Map<GroupInfo, Group>(g));

            return new GroupInfo(newGroup.CategoryId) { ID = newGroup.Id, Name = newGroup.Name, Sid = newGroup.Sid };
        }

        public void DeleteGroup(Guid id)
        {
            if (Constants.LostGroupInfo.Equals(id))
            {
                return;
            }

            if (Constants.BuildinGroups.Any(b => b.ID == id))
            {
                return;
            }

            _permissionContext.DemandPermissions(Constants.Action_EditGroups);

            _userService.RemoveGroup(Tenant.Id, id);
        }

        #endregion Groups


        private bool IsPropertiesContainsWords(IEnumerable<string> properties, IEnumerable<string> words)
        {
            foreach (var w in words)
            {
                var find = false;

                foreach (var p in properties)
                {
                    find = (2 <= w.Length) && (0 <= p.IndexOf(w, StringComparison.CurrentCultureIgnoreCase));
                    if (find)
                    {
                        break;
                    }
                }

                if (!find)
                {
                    return false;
                }
            }

            return true;
        }


        private IEnumerable<UserInfo> GetUsersInternal()
        {
            return _userService.GetUsers(Tenant.Id)
                .Where(u => !u.Removed);
        }

        private IEnumerable<GroupInfo> GetGroupsInternal()
        {
            return _userService.GetGroups(Tenant.Id)
                .Where(g => !g.Removed)
                .Select(g => new GroupInfo(g.CategoryId) { ID = g.Id, Name = g.Name, Sid = g.Sid })
                .Concat(Constants.BuildinGroups)
                .ToList();
        }

        private IDictionary<string, UserGroupRef> GetRefsInternal() => _userService.GetUserGroupRefs(Tenant.Id);

        private bool IsUserInGroupInternal(Guid userId, Guid groupId, IDictionary<string, UserGroupRef> refs)
        {
            if (groupId == Constants.GroupEveryone.ID)
            {
                return true;
            }

            if (groupId == Constants.GroupAdmin.ID && (Tenant.OwnerId == userId 
                || userId == Configuration.Constants.CoreSystem.ID || userId == _constants.NamingPoster.Id))
            {
                return true;
            }

            if (groupId == Constants.GroupVisitor.ID && userId == Constants.OutsideUser.Id)
            {
                return true;
            }

            UserGroupRef r;
            if (groupId == Constants.GroupUser.ID || groupId == Constants.GroupVisitor.ID)
            {
                var visitor = refs.TryGetValue(UserGroupRef.CreateKey(Tenant.Id, userId, Constants.GroupVisitor.ID, 
                    UserGroupRefType.Contains), out r) && !r.Removed;

                if (groupId == Constants.GroupVisitor.ID)
                {
                    return visitor;
                }

                if (groupId == Constants.GroupUser.ID)
                {
                    return !visitor;
                }
            }

            return refs.TryGetValue(UserGroupRef.CreateKey(Tenant.Id, userId, groupId, UserGroupRefType.Contains), out r) && !r.Removed;
        }
    }
}