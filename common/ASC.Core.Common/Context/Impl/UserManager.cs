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

    public async Task<UserInfo[]> GetUsersAsync()
    {
        return await GetUsersAsync(EmployeeStatus.Default);
    }

    public async Task<UserInfo[]> GetUsersAsync(EmployeeStatus status)
    {
        return await GetUsersAsync(status, EmployeeType.All);
    }

    public async Task<UserInfo[]> GetUsersAsync(EmployeeStatus status, EmployeeType type)
    {
        var users = (await GetUsersInternalAsync()).Where(u => (u.Status & status) == u.Status).ToAsyncEnumerable();
        switch (type)
        {
            case EmployeeType.RoomAdmin:
                users = users.WhereAwait(async u => !await this.IsUserAsync(u) && !await this.IsCollaboratorAsync(u) && !await this.IsDocSpaceAdminAsync(u));
                break;
            case EmployeeType.DocSpaceAdmin:
                users = users.WhereAwait(async u => await this.IsDocSpaceAdminAsync(u));
                break;
            case EmployeeType.Collaborator:
                users = users.WhereAwait(async u => await this.IsCollaboratorAsync(u));
                break;
            case EmployeeType.User:
                users = users.WhereAwait(async u => await this.IsUserAsync(u));
                break;
        }

        return await users.ToArrayAsync();
    }
    
    public Task<int> GetUsersCountAsync(
        bool isDocSpaceAdmin,
        EmployeeStatus? employeeStatus,
        List<List<Guid>> includeGroups,
        List<Guid> excludeGroups,
        List<Tuple<List<List<Guid>>, List<Guid>>> combinedGroups,
        EmployeeActivationStatus? activationStatus,
        AccountLoginType? accountLoginType,
        string text)
    {
        return _userService.GetUsersCountAsync(Tenant.Id, isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, combinedGroups, activationStatus, accountLoginType, text);
    }

    public IAsyncEnumerable<UserInfo> GetUsers(
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
        long offset)
    {
        return _userService.GetUsers(Tenant.Id, isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, combinedGroups, activationStatus, accountLoginType, text, sortBy, sortOrderAsc, limit, offset);
    }

    public async Task<string[]> GetUserNamesAsync(EmployeeStatus status)
    {
        return (await GetUsersAsync(status))
            .Select(u => u.UserName)
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();
    }

    public async Task<UserInfo> GetUserByUserNameAsync(string username)
    {
        var u = await _userService.GetUserByUserName(await _tenantManager.GetCurrentTenantIdAsync(), username);

        return u ?? Constants.LostUser;
    }

    public async Task<UserInfo> GetUserBySidAsync(string sid)
    {
        return (await GetUsersInternalAsync())
                .FirstOrDefault(u => u.Sid != null && string.Equals(u.Sid, sid, StringComparison.CurrentCultureIgnoreCase)) ?? Constants.LostUser;
    }

    public async Task<UserInfo> GetSsoUserByNameIdAsync(string nameId)
    {
        return (await GetUsersInternalAsync())
            .FirstOrDefault(u => !string.IsNullOrEmpty(u.SsoNameId) && string.Equals(u.SsoNameId, nameId, StringComparison.CurrentCultureIgnoreCase)) ?? Constants.LostUser;
    }
    public async Task<bool> IsUserNameExistsAsync(string username)
    {
        return (await GetUserNamesAsync(EmployeeStatus.All))
            .Contains(username, StringComparer.CurrentCultureIgnoreCase);
    }

    public async Task<UserInfo> GetUsersAsync(Guid id)
    {
        if (IsSystemUser(id))
        {
            return SystemUsers[id];
        }

        var u = await _userService.GetUserAsync(Tenant.Id, id);

        return u != null && !u.Removed ? u : Constants.LostUser;
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

    public async Task<UserInfo> GetUserAsync(Guid id, Expression<Func<User, UserInfo>> exp)
    {
        if (IsSystemUser(id))
        {
            return SystemUsers[id];
        }

        var u = await _userService.GetUserAsync(Tenant.Id, id, exp);

        return u != null && !u.Removed ? u : Constants.LostUser;
    }

    public async Task<UserInfo> GetUsersByPasswordHashAsync(int tenant, string login, string passwordHash)
    {
        var u = await _userService.GetUserByPasswordHashAsync(tenant, login, passwordHash);

        return u != null && !u.Removed ? u : Constants.LostUser;
    }

    public async Task<bool> UserExistsAsync(Guid id)
    {
        return UserExists(await GetUsersAsync(id));
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

    public async Task<UserInfo> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return Constants.LostUser;
        }

        var u = await _userService.GetUserAsync(Tenant.Id, email);

        return u != null && !u.Removed ? u : Constants.LostUser;
    }

    public async Task<UserInfo[]> SearchAsync(string text, EmployeeStatus status)
    {
        return await SearchAsync(text, status, Guid.Empty);
    }

    public async Task<UserInfo[]> SearchAsync(string text, EmployeeStatus status, Guid groupId)
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
            await GetUsersAsync(status) :
            (await GetUsersByGroupAsync(groupId)).Where(u => (u.Status & status) == status);

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

    public async Task<UserInfo> UpdateUserInfoAsync(UserInfo u)
    {
        if (IsSystemUser(u.Id))
        {
            return SystemUsers[u.Id];
        }

        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(u.Id), Constants.Action_EditUser);

        var tenant = await _tenantManager.GetCurrentTenantAsync();

        if (u.Status == EmployeeStatus.Terminated && u.Id == tenant.OwnerId)
        {
            throw new InvalidOperationException("Can not disable tenant owner.");
        }

        var oldUserData = await _userService.GetUserByUserName(tenant.Id, u.UserName);

        if (oldUserData == null || Equals(oldUserData, Constants.LostUser))
        {
            throw new InvalidOperationException("User not found.");
        }

        var (name, value) = ("", -1);

        if (!IsUserInGroup(oldUserData.Id, Constants.GroupUser.ID) &&
            oldUserData.Status != u.Status)
        {
            (name, value) = await _tenantQuotaFeatureStatHelper.GetStatAsync<CountPaidUserFeature, int>();
            value = oldUserData.Status > u.Status ? ++value : --value;//crutch: data race
        }

        var newUserData = await _userService.SaveUserAsync(tenant.Id, u);

        if (value > 0)
        {
            _ = _quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
        }

        return newUserData;
    }

    public async Task<UserInfo> UpdateUserInfoWithSyncCardDavAsync(UserInfo u)
    {
        var oldUserData = await _userService.GetUserByUserName(await _tenantManager.GetCurrentTenantIdAsync(), u.UserName);

        var newUser = await UpdateUserInfoAsync(u);

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

        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(u.Id, type), Constants.Action_AddRemoveUser);

        if (!_coreBaseSettings.Personal)
        {
            if (_constants.MaxEveryoneCount <= (await GetUsersByGroupAsync(Constants.GroupEveryone.ID)).Length)
            {
                throw new TenantQuotaException("Maximum number of users exceeded");
            }
        }

        var oldUserData = await _userService.GetUserByUserName(await _tenantManager.GetCurrentTenantIdAsync(), u.UserName);

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

        var newUser = await _userService.SaveUserAsync(await _tenantManager.GetCurrentTenantIdAsync(), u);
        if (syncCardDav)
        {
            await SyncCardDavAsync(u, oldUserData, newUser);
        }

        return newUser;
    }

    private async Task SyncCardDavAsync(UserInfo u, UserInfo oldUserData, UserInfo newUser)
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var myUri = (_accessor?.HttpContext != null) ? _accessor.HttpContext.Request.GetDisplayUrl() :
                    (_cache.Get<string>("REWRITE_URL" + tenant.Id) != null) ?
                    new Uri(_cache.Get<string>("REWRITE_URL" + tenant.Id)).ToString() : tenant.GetTenantDomain(_coreSettings);

        var rootAuthorization = _cardDavAddressbook.GetSystemAuthorization();

        if (rootAuthorization != null)
        {
            var allUserEmails = (await GetDavUserEmailsAsync()).ToList();

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
                        await _cardDavAddressbook.UpdateItemForAllAddBooks(allUserEmails, myUri, cardDavUser, await _tenantManager.GetCurrentTenantIdAsync(), oldUserData != null && oldUserData.Email != newUser.Email ? oldUserData.Email : null);
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

    public async Task<IEnumerable<string>> GetDavUserEmailsAsync()
    {
        return await _userService.GetDavUserEmailsAsync(await _tenantManager.GetCurrentTenantIdAsync());
    }

    public async Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime from)
    {
        return await _userService.GetTenantsWithFeedsAsync(from);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        if (IsSystemUser(id))
        {
            return;
        }

        await _permissionContext.DemandPermissionsAsync(Constants.Action_AddRemoveUser);
        if (id == Tenant.OwnerId)
        {
            throw new InvalidOperationException("Can not remove tenant owner.");
        }

        var delUser = await GetUsersAsync(id);
        await _userService.RemoveUserAsync(Tenant.Id, id);
        var tenant = await _tenantManager.GetCurrentTenantAsync();

        try
        {
            var curreMail = delUser.Email.ToLower();
            var currentAccountPaswd = _instanceCrypto.Encrypt(curreMail);
            var userAuthorization = curreMail + ":" + currentAccountPaswd;
            var rootAuthorization = _cardDavAddressbook.GetSystemAuthorization();
            var myUri = (_accessor?.HttpContext != null) ? _accessor.HttpContext.Request.GetDisplayUrl() :
                (_cache.Get<string>("REWRITE_URL" + tenant.Id) != null) ?
                new Uri(_cache.Get<string>("REWRITE_URL" + tenant.Id)).ToString() : tenant.GetTenantDomain(_coreSettings);
            var davUsersEmails = await GetDavUserEmailsAsync();
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

    public async Task SaveUserPhotoAsync(Guid id, byte[] photo)
    {
        if (IsSystemUser(id))
        {
            return;
        }

        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(id), Constants.Action_EditUser);

        await _userService.SetUserPhotoAsync(Tenant.Id, id, photo);
    }

    public async Task<byte[]> GetUserPhotoAsync(Guid id)
    {
        if (IsSystemUser(id))
        {
            return null;
        }

        return await _userService.GetUserPhotoAsync(Tenant.Id, id);
    }

    public async Task<List<GroupInfo>> GetUserGroupsAsync(Guid id)
    {
        return await GetUserGroupsAsync(id, IncludeType.Distinct, Guid.Empty);
    }

    public async Task<List<GroupInfo>> GetUserGroupsAsync(Guid id, Guid categoryID)
    {
        return await GetUserGroupsAsync(id, IncludeType.Distinct, categoryID);
    }

    public async Task<List<GroupInfo>> GetUserGroupsAsync(Guid userID, IncludeType includeType)
    {
        return await GetUserGroupsAsync(userID, includeType, null);
    }

    internal async Task<List<GroupInfo>> GetUserGroupsAsync(Guid userID, IncludeType includeType, Guid? categoryId)
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

        var refs = await GetRefsInternalAsync();
        IEnumerable<UserGroupRef> userRefs = null;
        if (refs is UserGroupRefStore store)
        {
            userRefs = store.GetRefsByUser(userID);
        }

        var userRefsContainsNotRemoved = userRefs?.Where(r => !r.Removed && r.RefType == UserGroupRefType.Contains).ToList();

        foreach (var g in (await GetGroupsInternalAsync()).Where(g => !categoryId.HasValue || g.CategoryID == categoryId))
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

    public async Task<bool> IsUserInGroupAsync(Guid userId, Guid groupId)
    {
        return IsUserInGroupInternal(userId, groupId, await GetRefsInternalAsync());
    }

    public bool IsUserInGroup(Guid userId, Guid groupId)
    {
        return IsUserInGroupInternal(userId, groupId, GetRefsInternal());
    }

    public async Task<UserInfo[]> GetUsersByGroupAsync(Guid groupId, EmployeeStatus employeeStatus = EmployeeStatus.Default)
    {
        var refs = await GetRefsInternalAsync();

        return (await GetUsersAsync(employeeStatus)).Where(u => IsUserInGroupInternal(u.Id, groupId, refs)).ToArray();
    }

    public async Task AddUserIntoGroupAsync(Guid userId, Guid groupId, bool dontClearAddressBook = false, bool notifyWebSocket = true)
    {
        if (Constants.LostUser.Id == userId || Constants.LostGroupInfo.ID == groupId)
        {
            return;
        }

        var user = await GetUsersAsync(userId);
        var isUser = await this.IsUserAsync(user);
        var isPaidUser = await IsPaidUserAsync(user);

        await _permissionContext.DemandPermissionsAsync(new UserGroupObject(new UserAccount(user, await _tenantManager.GetCurrentTenantIdAsync(), _userFormatter), groupId),
            Constants.Action_EditGroups);

        await _userService.SaveUserGroupRefAsync(Tenant.Id, new UserGroupRef(userId, groupId, UserGroupRefType.Contains));

        ResetGroupCache(userId);

        if (groupId == Constants.GroupUser.ID)
        {
            var tenant = await _tenantManager.GetCurrentTenantAsync();
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
            var (name, value) = await _tenantQuotaFeatureStatHelper.GetStatAsync<CountPaidUserFeature, int>();
            _ = _quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
        }
    }

    public async Task RemoveUserFromGroupAsync(Guid userId, Guid groupId)
    {
        if (Constants.LostUser.Id == userId || Constants.LostGroupInfo.ID == groupId)
        {
            return;
        }

        var user = await GetUsersAsync(userId);
        var isUserBefore = await this.IsUserAsync(user);
        var isPaidUserBefore = await IsPaidUserAsync(user);

        await _permissionContext.DemandPermissionsAsync(new UserGroupObject(new UserAccount(user, await _tenantManager.GetCurrentTenantIdAsync(), _userFormatter), groupId),
            Constants.Action_EditGroups);

        await _userService.RemoveUserGroupRefAsync(Tenant.Id, userId, groupId, UserGroupRefType.Contains);

        ResetGroupCache(userId);

        var isUserAfter = await this.IsUserAsync(user);
        var isPaidUserAfter = await IsPaidUserAsync(user);

        if (isPaidUserBefore && !isPaidUserAfter && isUserAfter ||
            isUserBefore && !isUserAfter)
        {
            var (name, value) = await _tenantQuotaFeatureStatHelper.GetStatAsync<CountPaidUserFeature, int>();
            _ = _quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
        }
    }

    internal void ResetGroupCache(Guid userID)
    {
        new HttpRequestDictionary<List<GroupInfo>>(_accessor?.HttpContext, "GroupInfo").Reset(userID.ToString());
        new HttpRequestDictionary<List<Guid>>(_accessor?.HttpContext, "GroupInfoID").Reset(userID.ToString());
    }

    #endregion Users


    #region Company

    public async Task<GroupInfo[]> GetDepartmentsAsync()
    {
        return await GetGroupsAsync();
    }

    public async Task<Guid> GetDepartmentManagerAsync(Guid deparmentID)
    {
        var groupRef = await _userService.GetUserGroupRefAsync(Tenant.Id, deparmentID, UserGroupRefType.Manager);

        if (groupRef == null)
        {
            return Guid.Empty;
        }

        return groupRef.UserId;
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

    public async Task SetDepartmentManagerAsync(Guid deparmentID, Guid userID)
    {
        var managerId = await GetDepartmentManagerAsync(deparmentID);
        if (managerId != Guid.Empty)
        {
            await _userService.RemoveUserGroupRefAsync(
                Tenant.Id,
                managerId, deparmentID, UserGroupRefType.Manager);
        }
        if (userID != Guid.Empty)
        {
            await _userService.SaveUserGroupRefAsync(
                Tenant.Id,
                new UserGroupRef(userID, deparmentID, UserGroupRefType.Manager));
        }
    }

    public async Task<UserInfo> GetCompanyCEOAsync()
    {
        var id = await GetDepartmentManagerAsync(Guid.Empty);

        return id != Guid.Empty ? await GetUsersAsync(id) : null;
    }

    public async Task SetCompanyCEOAsync(Guid userId)
    {
        await SetDepartmentManagerAsync(Guid.Empty, userId);
    }

    #endregion Company


    #region Groups

    public async Task<GroupInfo[]> GetGroupsAsync()
    {
        return await GetGroupsAsync(Guid.Empty);
    }

    public async Task<GroupInfo[]> GetGroupsAsync(Guid categoryID)
    {
        return (await GetGroupsInternalAsync())
            .Where(g => g.CategoryID == categoryID)
            .ToArray();
    }

    public async Task<GroupInfo> GetGroupInfoAsync(Guid groupID)
    {
        var group = await _userService.GetGroupAsync(Tenant.Id, groupID);

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

    public async Task<GroupInfo> GetGroupInfoBySidAsync(string sid)
    {
        return (await GetGroupsInternalAsync())
            .SingleOrDefault(g => g.Sid == sid) ?? Constants.LostGroupInfo;
    }

    public async Task<GroupInfo> SaveGroupInfoAsync(GroupInfo g)
    {
        if (Constants.LostGroupInfo.Equals(g))
        {
            return Constants.LostGroupInfo;
        }

        if (Constants.BuildinGroups.Any(b => b.ID == g.ID))
        {
            return Constants.BuildinGroups.Single(b => b.ID == g.ID);
        }

        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups);

        var newGroup = await _userService.SaveGroupAsync(Tenant.Id, ToGroup(g));

        return new GroupInfo(newGroup.CategoryId) { ID = newGroup.Id, Name = newGroup.Name, Sid = newGroup.Sid };
    }

    public async Task DeleteGroupAsync(Guid id)
    {
        if (Constants.LostGroupInfo.Equals(id))
        {
            return;
        }

        if (Constants.BuildinGroups.Any(b => b.ID == id))
        {
            return;
        }

        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups);

        await _userService.RemoveGroupAsync(Tenant.Id, id);
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


    private async Task<IEnumerable<UserInfo>> GetUsersInternalAsync()
    {
        return (await _userService.GetUsersAsync(Tenant.Id))
            .Where(u => !u.Removed);
    }

    private async Task<IEnumerable<GroupInfo>> GetGroupsInternalAsync()
    {
        return (await _userService.GetGroupsAsync(Tenant.Id))
            .Where(g => !g.Removed)
            .Select(g => new GroupInfo(g.CategoryId) { ID = g.Id, Name = g.Name, Sid = g.Sid })
            .Concat(Constants.BuildinGroups)
            .ToList();
    }

    private async Task<IDictionary<string, UserGroupRef>> GetRefsInternalAsync()
    {
        return await _userService.GetUserGroupRefsAsync(Tenant.Id);
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

    private async Task<bool> IsPaidUserAsync(UserInfo userInfo)
    {
        return await this.IsCollaboratorAsync(userInfo) || await this.IsDocSpaceAdminAsync(userInfo);
    }
}
