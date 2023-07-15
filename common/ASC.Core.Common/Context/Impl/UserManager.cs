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

using Microsoft.AspNetCore.Http.Extensions;

using Constants = ASC.Core.Users.Constants;

namespace ASC.Core;

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

    private readonly IHttpContextAccessor _accessor;
    private readonly IUserService _userService;
    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;
    private readonly UserManagerConstants _userManagerConstants;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly RadicaleClient _radicaleClient;
    private readonly CardDavAddressbook _cardDavAddressbook;
    private readonly ILogger<UserManager> _log;
    private readonly ICache _cache;
    private readonly TenantQuotaFeatureCheckerCount<CountPaidUserFeature> _countPaidUserChecker;
    private readonly TenantQuotaFeatureCheckerCount<CountUserFeature> _activeUsersFeatureChecker;
    private readonly Constants _constants;
    private readonly UserFormatter _userFormatter;
    private readonly QuotaSocketManager _quotaSocketManager;
    private readonly TenantQuotaFeatureStatHelper _tenantQuotaFeatureStatHelper;
    private Tenant Tenant => _tenantManager.GetCurrentTenant();

    public UserManager()
    {

    }

    public UserManager(
        IUserService service,
        TenantManager tenantManager,
        PermissionContext permissionContext,
        UserManagerConstants userManagerConstants,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        InstanceCrypto instanceCrypto,
        RadicaleClient radicaleClient,
        CardDavAddressbook cardDavAddressbook,
        ILogger<UserManager> log,
        ICache cache,
        TenantQuotaFeatureCheckerCount<CountPaidUserFeature> countPaidUserChecker,
        TenantQuotaFeatureCheckerCount<CountUserFeature> activeUsersFeatureChecker,
        UserFormatter userFormatter,
        QuotaSocketManager quotaSocketManager,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper
        )
    {
        _userService = service;
        _tenantManager = tenantManager;
        _permissionContext = permissionContext;
        _userManagerConstants = userManagerConstants;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
        _instanceCrypto = instanceCrypto;
        _radicaleClient = radicaleClient;
        _cardDavAddressbook = cardDavAddressbook;
        _log = log;
        _cache = cache;
        _countPaidUserChecker = countPaidUserChecker;
        _activeUsersFeatureChecker = activeUsersFeatureChecker;
        _constants = _userManagerConstants.Constants;
        _userFormatter = userFormatter;
        _quotaSocketManager = quotaSocketManager;
        _tenantQuotaFeatureStatHelper = tenantQuotaFeatureStatHelper;
    }

    public UserManager(
        IUserService service,
        TenantManager tenantManager,
        PermissionContext permissionContext,
        UserManagerConstants userManagerConstants,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        InstanceCrypto instanceCrypto,
        RadicaleClient radicaleClient,
        CardDavAddressbook cardDavAddressbook,
        ILogger<UserManager> log,
        ICache cache,
        TenantQuotaFeatureCheckerCount<CountPaidUserFeature> tenantQuotaFeatureChecker,
        TenantQuotaFeatureCheckerCount<CountUserFeature> activeUsersFeatureChecker,
        IHttpContextAccessor httpContextAccessor,
        UserFormatter userFormatter,
        QuotaSocketManager quotaSocketManager,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper)
        : this(service, tenantManager, permissionContext, userManagerConstants, coreBaseSettings, coreSettings, instanceCrypto, radicaleClient, cardDavAddressbook, log, cache, tenantQuotaFeatureChecker, activeUsersFeatureChecker, userFormatter, quotaSocketManager, tenantQuotaFeatureStatHelper)
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
            case EmployeeType.RoomAdmin:
                users = users.Where(u => !this.IsUser(u) && !this.IsCollaborator(u) && !this.IsDocSpaceAdmin(u));
                break;
            case EmployeeType.DocSpaceAdmin:
                users = users.Where(this.IsDocSpaceAdmin);
                break;
            case EmployeeType.Collaborator:
                users = users.Where(this.IsCollaborator);
                break;
            case EmployeeType.User:
                users = users.Where(this.IsUser);
                break;
        }

        return users.ToArray();
    }

    public IQueryable<UserInfo> GetUsers(
        bool isDocSpaceAdmin,
        EmployeeStatus? employeeStatus,
        List<List<Guid>> includeGroups,
        List<Guid> excludeGroups,
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
        return _userService.GetUsers(Tenant.Id, isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, accountLoginType, text, sortBy, sortOrderAsc, limit, offset, out total, out count);
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
                .FirstOrDefault(u => u.Sid != null && string.Equals(u.Sid, sid, StringComparison.CurrentCultureIgnoreCase)) ?? Constants.LostUser;
    }

    public UserInfo GetSsoUserByNameId(string nameId)
    {
        return GetUsersInternal()
            .FirstOrDefault(u => !string.IsNullOrEmpty(u.SsoNameId) && string.Equals(u.SsoNameId, nameId, StringComparison.CurrentCultureIgnoreCase)) ?? Constants.LostUser;
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
        if (text == null || text.Trim().Length == 0)
        {
            return new UserInfo[0];
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

    public async Task<UserInfo> UpdateUserInfo(UserInfo u)
    {
        if (IsSystemUser(u.Id))
        {
            return SystemUsers[u.Id];
        }

        _permissionContext.DemandPermissions(new UserSecurityProvider(u.Id), Constants.Action_EditUser);

        if (u.Status == EmployeeStatus.Terminated && u.Id == _tenantManager.GetCurrentTenant().OwnerId)
        {
            throw new InvalidOperationException("Can not disable tenant owner.");
        }

        var oldUserData = _userService.GetUserByUserName(_tenantManager.GetCurrentTenant().Id, u.UserName);

        if (oldUserData == null || Equals(oldUserData, Constants.LostUser))
        {
            throw new InvalidOperationException("User not found.");
        }

        var (name, value) = ("", -1);

        if (!IsUserInGroup(oldUserData.Id, Constants.GroupUser.ID) &&
            oldUserData.Status != u.Status)
        {
            (name, value) = await _tenantQuotaFeatureStatHelper.GetStat<CountPaidUserFeature, int>();
            value = oldUserData.Status > u.Status ? ++value : --value;//crutch: data race
        }

        var newUserData = _userService.SaveUser(_tenantManager.GetCurrentTenant().Id, u);

        if (value > 0)
        {
            _ = _quotaSocketManager.ChangeQuotaUsedValue(name, value);
        }

        return newUserData;
    }

    public async Task<UserInfo> UpdateUserInfoWithSyncCardDavAsync(UserInfo u)
    {
        var oldUserData = _userService.GetUserByUserName(_tenantManager.GetCurrentTenant().Id, u.UserName);

        var newUser = await UpdateUserInfo(u);

        if (_coreBaseSettings.DisableDocSpace)
        {
            await SyncCardDavAsync(u, oldUserData, newUser);
        }

        return newUser;
    }

    public async Task<UserInfo> SaveUserInfo(UserInfo u, EmployeeType type = EmployeeType.RoomAdmin, bool syncCardDav = false, bool paidUserQuotaCheck = true)
    {
        if (IsSystemUser(u.Id))
        {
            return SystemUsers[u.Id];
        }

        _permissionContext.DemandPermissions(new UserSecurityProvider(u.Id, type), Constants.Action_AddRemoveUser);

        if (!_coreBaseSettings.Personal)
        {
            if (_constants.MaxEveryoneCount <= GetUsersByGroup(Constants.GroupEveryone.ID).Length)
            {
                throw new TenantQuotaException("Maximum number of users exceeded");
            }
        }

        var oldUserData = _userService.GetUserByUserName(_tenantManager.GetCurrentTenant().Id, u.UserName);

        if (oldUserData != null && !Equals(oldUserData, Constants.LostUser))
        {
            throw new InvalidOperationException("User already exist.");
        }

        if (type is EmployeeType.User)
        {
            await _activeUsersFeatureChecker.CheckAppend();
        }
        else if (paidUserQuotaCheck)
        {
            await _countPaidUserChecker.CheckAppend();
        }

        var newUser = _userService.SaveUser(_tenantManager.GetCurrentTenant().Id, u);
        if (syncCardDav)
        {
            await SyncCardDavAsync(u, oldUserData, newUser);
        }

        return newUser;
    }

    private async Task SyncCardDavAsync(UserInfo u, UserInfo oldUserData, UserInfo newUser)
    {
        var tenant = _tenantManager.GetCurrentTenant();
        var myUri = (_accessor?.HttpContext != null) ? _accessor.HttpContext.Request.GetDisplayUrl() :
                    (_cache.Get<string>("REWRITE_URL" + tenant.Id) != null) ?
                    new Uri(_cache.Get<string>("REWRITE_URL" + tenant.Id)).ToString() : tenant.GetTenantDomain(_coreSettings);

        var rootAuthorization = _cardDavAddressbook.GetSystemAuthorization();

        if (rootAuthorization != null)
        {
            var allUserEmails = GetDavUserEmails().ToList();

            if (oldUserData != null && oldUserData.Status != newUser.Status && newUser.Status == EmployeeStatus.Terminated)
            {
                var userAuthorization = oldUserData.Email.ToLower() + ":" + _instanceCrypto.Encrypt(oldUserData.Email);
                var requestUrlBook = _cardDavAddressbook.GetRadicaleUrl(myUri, newUser.Email.ToLower(), true, true);
                var collection = await _cardDavAddressbook.GetCollection(requestUrlBook, userAuthorization, myUri.ToString());
                if (collection.Completed && collection.StatusCode != 404)
                {
                    await _cardDavAddressbook.Delete(myUri, newUser.Id, newUser.Email, tenant.Id);
                }
                foreach (var email in allUserEmails)
                {
                    var requestUrlItem = _cardDavAddressbook.GetRadicaleUrl(myUri.ToString(), email.ToLower(), true, true, itemID: newUser.Id.ToString());
                    try
                    {
                        var davItemRequest = new DavRequest()
                        {
                            Url = requestUrlItem,
                            Authorization = rootAuthorization,
                            Header = myUri
                        };
                        await _radicaleClient.RemoveAsync(davItemRequest).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _log.ErrorWithException(ex);
                    }
                }
            }
            else
            {
                try
                {
                    var cardDavUser = new CardDavItem(u.Id, u.FirstName, u.LastName, u.UserName, u.BirthDate, u.Sex, u.Title, u.Email, u.ContactsList, u.MobilePhone);
                    try
                    {
                        await _cardDavAddressbook.UpdateItemForAllAddBooks(allUserEmails, myUri, cardDavUser, _tenantManager.GetCurrentTenant().Id, oldUserData != null && oldUserData.Email != newUser.Email ? oldUserData.Email : null);
                    }
                    catch (Exception ex)
                    {
                        _log.ErrorWithException(ex);
                    }
                }
                catch (Exception ex)
                {
                    _log.ErrorWithException(ex);
                }
            }
        }
    }

    public IEnumerable<string> GetDavUserEmails()
    {
        return _userService.GetDavUserEmails(_tenantManager.GetCurrentTenant().Id);
    }

    public IEnumerable<int> GetTenantsWithFeeds(DateTime from)
    {
        return _userService.GetTenantsWithFeeds(from);
    }

    public async Task DeleteUser(Guid id)
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

        var delUser = GetUsers(id);
        _userService.RemoveUser(Tenant.Id, id);
        var tenant = _tenantManager.GetCurrentTenant();

        try
        {
            var curreMail = delUser.Email.ToLower();
            var currentAccountPaswd = _instanceCrypto.Encrypt(curreMail);
            var userAuthorization = curreMail + ":" + currentAccountPaswd;
            var rootAuthorization = _cardDavAddressbook.GetSystemAuthorization();
            var myUri = (_accessor?.HttpContext != null) ? _accessor.HttpContext.Request.GetDisplayUrl() :
                (_cache.Get<string>("REWRITE_URL" + tenant.Id) != null) ?
                new Uri(_cache.Get<string>("REWRITE_URL" + tenant.Id)).ToString() : tenant.GetTenantDomain(_coreSettings);
            var davUsersEmails = GetDavUserEmails();
            var requestUrlBook = _cardDavAddressbook.GetRadicaleUrl(myUri, delUser.Email.ToLower(), true, true);
            
            if(rootAuthorization != null)
            {
                var addBookCollection = await _cardDavAddressbook.GetCollection(requestUrlBook, userAuthorization, myUri.ToString());
                if (addBookCollection.Completed && addBookCollection.StatusCode != 404)
                {
                    var davbookRequest = new DavRequest()
                    {
                        Url = requestUrlBook,
                        Authorization = rootAuthorization,
                        Header = myUri
                    };
                    await _radicaleClient.RemoveAsync(davbookRequest).ConfigureAwait(false);
                }

                foreach (var email in davUsersEmails)
                {
                    var requestUrlItem = _cardDavAddressbook.GetRadicaleUrl(myUri.ToString(), email.ToLower(), true, true, itemID: delUser.Id.ToString());
                    try
                    {
                        var davItemRequest = new DavRequest()
                        {
                            Url = requestUrlItem,
                            Authorization = rootAuthorization,
                            Header = myUri
                        };
                        await _radicaleClient.RemoveAsync(davItemRequest).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _log.ErrorWithException(ex);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            _log.ErrorWithException(ex);
        }
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

    internal List<GroupInfo> GetUserGroups(Guid userID, IncludeType includeType, Guid? categoryId)
    {
        if (_coreBaseSettings.Personal)
        {
            return new List<GroupInfo> { Constants.GroupManager, Constants.GroupEveryone };
        }

        var httpRequestDictionary = new HttpRequestDictionary<List<GroupInfo>>(_accessor?.HttpContext, "GroupInfo");
        var result = httpRequestDictionary.Get(userID.ToString());
        if (result != null && result.Count > 0)
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

    public bool IsUserInGroup(Guid userId, Guid groupId)
    {
        return IsUserInGroupInternal(userId, groupId, GetRefsInternal());
    }

    public UserInfo[] GetUsersByGroup(Guid groupId, EmployeeStatus employeeStatus = EmployeeStatus.Default)
    {
        var refs = GetRefsInternal();

        return GetUsers(employeeStatus).Where(u => IsUserInGroupInternal(u.Id, groupId, refs)).ToArray();
    }

    public async Task AddUserIntoGroup(Guid userId, Guid groupId, bool dontClearAddressBook = false, bool notifyWebSocket = true)
    {
        if (Constants.LostUser.Id == userId || Constants.LostGroupInfo.ID == groupId)
        {
            return;
        }

        var user = GetUsers(userId);
        var isUser = this.IsUser(user);
        var isPaidUser = IsPaidUser(user);

        _permissionContext.DemandPermissions(new UserGroupObject(new UserAccount(user, _tenantManager.GetCurrentTenant().Id, _userFormatter), groupId),
            Constants.Action_EditGroups);

        _userService.SaveUserGroupRef(Tenant.Id, new UserGroupRef(userId, groupId, UserGroupRefType.Contains));

        ResetGroupCache(userId);

        if (groupId == Constants.GroupUser.ID)
        {
            var tenant = _tenantManager.GetCurrentTenant();
            var myUri = (_accessor?.HttpContext != null) ? _accessor.HttpContext.Request.GetDisplayUrl() :
                       (_cache.Get<string>("REWRITE_URL" + tenant.Id) != null) ?
                       new Uri(_cache.Get<string>("REWRITE_URL" + tenant.Id)).ToString() : tenant.GetTenantDomain(_coreSettings);

            if (!dontClearAddressBook)
            {
                await _cardDavAddressbook.Delete(myUri, user.Id, user.Email, tenant.Id);
            }
        }

        if (!notifyWebSocket)
        {
            return;
        }

        if (isUser && groupId != Constants.GroupUser.ID ||
            !isUser && !isPaidUser && groupId != Constants.GroupUser.ID)
        {
            var (name, value) = await _tenantQuotaFeatureStatHelper.GetStat<CountPaidUserFeature, int>();
            _ = _quotaSocketManager.ChangeQuotaUsedValue(name, value);
        }
    }

    public async Task RemoveUserFromGroup(Guid userId, Guid groupId)
    {
        if (Constants.LostUser.Id == userId || Constants.LostGroupInfo.ID == groupId)
        {
            return;
        }

        var user = GetUsers(userId);
        var isUserBefore = this.IsUser(user);
        var isPaidUserBefore = IsPaidUser(user);

        _permissionContext.DemandPermissions(new UserGroupObject(new UserAccount(user, _tenantManager.GetCurrentTenant().Id, _userFormatter), groupId),
            Constants.Action_EditGroups);

        _userService.RemoveUserGroupRef(Tenant.Id, userId, groupId, UserGroupRefType.Contains);

        ResetGroupCache(userId);

        var isUserAfter = this.IsUser(user);
        var isPaidUserAfter = IsPaidUser(user);

        if (isPaidUserBefore && !isPaidUserAfter && isUserAfter ||
            isUserBefore && !isUserAfter)
        {
            var (name, value) = await _tenantQuotaFeatureStatHelper.GetStat<CountPaidUserFeature, int>();
            _ = _quotaSocketManager.ChangeQuotaUsedValue(name, value);
        }
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

        if (groupRef == null)
        {
            return Guid.Empty;
        }

        return groupRef.UserId;
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

        if (group == null)
        {
            group = ToGroup(Constants.BuildinGroups.FirstOrDefault(r => r.ID == groupID) ?? Constants.LostGroupInfo);
        }

        if (group == null)
        {
            return Constants.LostGroupInfo;
        }

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

        var newGroup = _userService.SaveGroup(Tenant.Id, ToGroup(g));

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

    private IDictionary<string, UserGroupRef> GetRefsInternal()
    {
        return _userService.GetUserGroupRefs(Tenant.Id);
    }

    private bool IsUserInGroupInternal(Guid userId, Guid groupId, IDictionary<string, UserGroupRef> refs)
    {
        if (groupId == Constants.GroupEveryone.ID)
        {
            return true;
        }
        if (groupId == Constants.GroupAdmin.ID && (Tenant.OwnerId == userId || userId == Configuration.Constants.CoreSystem.ID || userId == _constants.NamingPoster.Id))
        {
            return true;
        }
        if (groupId == Constants.GroupUser.ID && userId == Constants.OutsideUser.Id)
        {
            return true;
        }

        UserGroupRef r;
        if (groupId == Constants.GroupManager.ID || groupId == Constants.GroupUser.ID || groupId == Constants.GroupCollaborator.ID)
        {
            var isUser = refs.TryGetValue(UserGroupRef.CreateKey(Tenant.Id, userId, Constants.GroupUser.ID, UserGroupRefType.Contains), out r) && !r.Removed;
            if (groupId == Constants.GroupUser.ID)
            {
                return isUser;
            }

            var isCollaborator = refs.TryGetValue(UserGroupRef.CreateKey(Tenant.Id, userId, Constants.GroupCollaborator.ID, UserGroupRefType.Contains), out r) && !r.Removed;
            if (groupId == Constants.GroupCollaborator.ID)
            {
                return isCollaborator;
            }

            if (groupId == Constants.GroupManager.ID)
            {
                return !isUser && !isCollaborator;
            }
        }

        return refs.TryGetValue(UserGroupRef.CreateKey(Tenant.Id, userId, groupId, UserGroupRefType.Contains), out r) && !r.Removed;
    }

    private Group ToGroup(GroupInfo g)
    {
        if (g == null)
        {
            return null;
        }

        return new Group
        {
            Id = g.ID,
            Name = g.Name,
            ParentId = g.Parent != null ? g.Parent.ID : Guid.Empty,
            CategoryId = g.CategoryID,
            Sid = g.Sid
        };
    }

    private bool IsPaidUser(UserInfo userInfo)
    {
        return this.IsCollaborator(userInfo) || this.IsDocSpaceAdmin(userInfo);
    }
}
