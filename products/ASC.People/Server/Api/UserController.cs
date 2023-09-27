﻿// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.People.Api;

public class UserController : PeopleControllerBase
{
    private Tenant Tenant => _apiContext.Tenant;

    private readonly ICache _cache;
    private readonly TenantManager _tenantManager;
    private readonly CookiesManager _cookiesManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly EmployeeDtoHelper _employeeDtoHelper;
    private readonly EmployeeFullDtoHelper _employeeFullDtoHelper;
    private readonly ILogger<UserController> _logger;
    private readonly PasswordHasher _passwordHasher;
    private readonly QueueWorkerReassign _queueWorkerReassign;
    private readonly QueueWorkerRemove _queueWorkerRemove;
    private readonly Recaptcha _recaptcha;
    private readonly TenantUtil _tenantUtil;
    private readonly UserFormatter _userFormatter;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly WebItemManager _webItemManager;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly WebItemSecurityCache _webItemSecurityCache;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly MessageTarget _messageTarget;
    private readonly SecurityContext _securityContext;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly MessageService _messageService;
    private readonly AuthContext _authContext;
    private readonly SetupInfo _setupInfo;
    private readonly SettingsManager _settingsManager;
    private readonly InvitationLinkService _invitationLinkService;
    private readonly FileSecurity _fileSecurity;
    private readonly IQuotaService _quotaService;
    private readonly CountPaidUserChecker _countPaidUserChecker;
    private readonly UsersQuotaSyncOperation _usersQuotaSyncOperation;
    private readonly CountUserChecker _countUserChecker;
    private readonly UsersInRoomChecker _usersInRoomChecker;
    private readonly IUrlShortener _urlShortener;

    public UserController(
        ICache cache,
        TenantManager tenantManager,
        CookiesManager cookiesManager,
        CoreBaseSettings coreBaseSettings,
        CustomNamingPeople customNamingPeople,
        EmployeeDtoHelper employeeDtoHelper,
        EmployeeFullDtoHelper employeeFullDtoHelper,
        ILogger<UserController> logger,
        PasswordHasher passwordHasher,
        QueueWorkerReassign queueWorkerReassign,
        QueueWorkerRemove queueWorkerRemove,
        Recaptcha recaptcha,
        TenantUtil tenantUtil,
        UserFormatter userFormatter,
        UserManagerWrapper userManagerWrapper,
        WebItemManager webItemManager,
        WebItemSecurity webItemSecurity,
        WebItemSecurityCache webItemSecurityCache,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        MessageTarget messageTarget,
        SecurityContext securityContext,
        StudioNotifyService studioNotifyService,
        MessageService messageService,
        AuthContext authContext,
        SetupInfo setupInfo,
        UserManager userManager,
        PermissionContext permissionContext,
        ApiContext apiContext,
        UserPhotoManager userPhotoManager,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        SettingsManager settingsManager,
        InvitationLinkService invitationLinkService,
        FileSecurity fileSecurity,
        UsersQuotaSyncOperation usersQuotaSyncOperation,
        CountPaidUserChecker countPaidUserChecker,
        CountUserChecker activeUsersChecker,
        UsersInRoomChecker usersInRoomChecker,
        IQuotaService quotaService,
        IUrlShortener urlShortener)
        : base(userManager, permissionContext, apiContext, userPhotoManager, httpClientFactory, httpContextAccessor)
    {
        _cache = cache;
        _tenantManager = tenantManager;
        _cookiesManager = cookiesManager;
        _coreBaseSettings = coreBaseSettings;
        _customNamingPeople = customNamingPeople;
        _employeeDtoHelper = employeeDtoHelper;
        _employeeFullDtoHelper = employeeFullDtoHelper;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _queueWorkerReassign = queueWorkerReassign;
        _queueWorkerRemove = queueWorkerRemove;
        _recaptcha = recaptcha;
        _tenantUtil = tenantUtil;
        _userFormatter = userFormatter;
        _userManagerWrapper = userManagerWrapper;
        _webItemManager = webItemManager;
        _webItemSecurity = webItemSecurity;
        _webItemSecurityCache = webItemSecurityCache;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
        _securityContext = securityContext;
        _studioNotifyService = studioNotifyService;
        _messageService = messageService;
        _authContext = authContext;
        _setupInfo = setupInfo;
        _settingsManager = settingsManager;
        _invitationLinkService = invitationLinkService;
        _fileSecurity = fileSecurity;
        _countPaidUserChecker = countPaidUserChecker;
        _countUserChecker = activeUsersChecker;
        _usersInRoomChecker = usersInRoomChecker;
        _quotaService = quotaService;
        _usersQuotaSyncOperation = usersQuotaSyncOperation;
        _urlShortener = urlShortener;
    }

    /// <summary>
    /// Adds an activated portal user with the first name, last name, email address, and several optional parameters specified in the request.
    /// </summary>
    /// <short>
    /// Add an activated user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People" name="inDto">Member request parameters</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Newly added user with the detailed information</returns>
    /// <path>api/2.0/people/active</path>
    /// <httpMethod>POST</httpMethod>
    /// <visible>false</visible>
    [HttpPost("active")]
    public async Task<EmployeeFullDto> AddMemberAsActivatedAsync(MemberRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(inDto.Type), Constants.Action_AddRemoveUser);

        var user = new UserInfo();

        inDto.PasswordHash = (inDto.PasswordHash ?? "").Trim();
        if (string.IsNullOrEmpty(inDto.PasswordHash))
        {
            inDto.Password = (inDto.Password ?? "").Trim();

            if (string.IsNullOrEmpty(inDto.Password))
            {
                inDto.Password = UserManagerWrapper.GeneratePassword();
            }
            else
            {
                await _userManagerWrapper.CheckPasswordPolicyAsync(inDto.Password);
            }

            inDto.PasswordHash = _passwordHasher.GetClientPassword(inDto.Password);
        }

        //Validate email
        var address = new MailAddress(inDto.Email);
        user.Email = address.Address;
        //Set common fields
        user.FirstName = inDto.Firstname;
        user.LastName = inDto.Lastname;
        user.Title = inDto.Title;
        user.Location = inDto.Location;
        user.Notes = inDto.Comment;
        user.Sex = "male".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase)
                       ? true
                       : ("female".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase) ? false : null);

        user.BirthDate = inDto.Birthday != null ? _tenantUtil.DateTimeFromUtc(inDto.Birthday) : null;
        user.WorkFromDate = inDto.Worksfrom != null ? _tenantUtil.DateTimeFromUtc(inDto.Worksfrom) : DateTime.UtcNow.Date;

        await UpdateContactsAsync(inDto.Contacts, user);

        _cache.Insert("REWRITE_URL" + await _tenantManager.GetCurrentTenantIdAsync(), HttpContext.Request.GetDisplayUrl(), TimeSpan.FromMinutes(5));
        user = await _userManagerWrapper.AddUserAsync(user, inDto.PasswordHash, true, false, inDto.Type,
            false, true, true);

        user.ActivationStatus = EmployeeActivationStatus.Activated;

        await UpdateDepartmentsAsync(inDto.Department, user);

        if (inDto.Files != _userPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            await UpdatePhotoUrlAsync(inDto.Files, user);
        }

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Adds a new portal user with the first name, last name, email address, and several optional parameters specified in the request.
    /// </summary>
    /// <short>
    /// Add a user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People" name="inDto">Member request parameters</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Newly added user with the detailed information</returns>
    /// <path>api/2.0/people</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    public async Task<EmployeeFullDto> AddMember(MemberRequestDto inDto)
    {
        await _apiContext.AuthByClaimAsync();

        var linkData = inDto.FromInviteLink ? await _invitationLinkService.GetProcessedLinkDataAsync(inDto.Key, inDto.Email, inDto.Type) : null;
        if (linkData is { IsCorrect: false })
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
        }

        if (linkData != null)
        {
            await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(Guid.Empty, linkData.EmployeeType), Constants.Action_AddRemoveUser);
        }
        else
        {
            await _permissionContext.DemandPermissionsAsync(Constants.Action_AddRemoveUser);
        }

        inDto.Type = linkData?.EmployeeType ?? inDto.Type;

        var user = new UserInfo();

        var byEmail = linkData?.LinkType == InvitationLinkType.Individual;

        if (byEmail)
        {
            user = await _userManager.GetUserByEmailAsync(inDto.Email);

            if (user.Equals(Constants.LostUser) || user.ActivationStatus != EmployeeActivationStatus.Pending)
            {
                throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
            }
        }

        inDto.PasswordHash = (inDto.PasswordHash ?? "").Trim();
        if (string.IsNullOrEmpty(inDto.PasswordHash))
        {
            inDto.Password = (inDto.Password ?? "").Trim();

            if (string.IsNullOrEmpty(inDto.Password))
            {
                inDto.Password = UserManagerWrapper.GeneratePassword();
            }
            else
            {
                await _userManagerWrapper.CheckPasswordPolicyAsync(inDto.Password);
            }
            inDto.PasswordHash = _passwordHasher.GetClientPassword(inDto.Password);
        }

        //Validate email
        var address = new MailAddress(inDto.Email);
        user.Email = address.Address;
        //Set common fields
        user.CultureName = inDto.CultureName;
        user.FirstName = inDto.Firstname;
        user.LastName = inDto.Lastname;
        user.Title = inDto.Title;
        user.Location = inDto.Location;
        user.Notes = inDto.Comment;
        user.Sex = "male".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase)
                       ? true
                       : ("female".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase) ? false : null);

        user.BirthDate = inDto.Birthday != null && inDto.Birthday != DateTime.MinValue ? _tenantUtil.DateTimeFromUtc(inDto.Birthday) : null;
        user.WorkFromDate = inDto.Worksfrom != null && inDto.Worksfrom != DateTime.MinValue ? _tenantUtil.DateTimeFromUtc(inDto.Worksfrom) : DateTime.UtcNow.Date;

        await UpdateContactsAsync(inDto.Contacts, user, !inDto.FromInviteLink);

        _cache.Insert("REWRITE_URL" + await _tenantManager.GetCurrentTenantIdAsync(), HttpContext.Request.GetDisplayUrl(), TimeSpan.FromMinutes(5));

        user = await _userManagerWrapper.AddUserAsync(user, inDto.PasswordHash, inDto.FromInviteLink, true, inDto.Type,
            inDto.FromInviteLink && linkData is { IsCorrect: true }, true, true, byEmail);

        await UpdateDepartmentsAsync(inDto.Department, user);

        if (inDto.Files != _userPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            await UpdatePhotoUrlAsync(inDto.Files, user);
        }

        if (linkData is { LinkType: InvitationLinkType.CommonWithRoom })
        {
            var success = int.TryParse(linkData.RoomId, out var id);

            if (success)
            {
                await _usersInRoomChecker.CheckAppend();
                await _fileSecurity.ShareAsync(id, FileEntryType.Folder, user.Id, linkData.Share);
            }
            else
            {
                await _usersInRoomChecker.CheckAppend();
                await _fileSecurity.ShareAsync(linkData.RoomId, FileEntryType.Folder, user.Id, linkData.Share);
            }
        }

        if (inDto.IsUser.GetValueOrDefault(false))
        {
            await _messageService.SendAsync(MessageAction.GuestCreated, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));
        }
        else
        {
            await _messageService.SendAsync(MessageAction.UserCreated, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper), user.Id);
        }

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Invites users specified in the request to the current portal.
    /// </summary>
    /// <short>
    /// Invite users
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.InviteUsersRequestDto, ASC.People" name="inDto">Request parameters for inviting users</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeDto, ASC.Api.Core">List of users</returns>
    /// <path>api/2.0/people/invite</path>
    /// <httpMethod>POST</httpMethod>
    /// <collection>list</collection>
    [HttpPost("invite")]
    public async Task<List<EmployeeDto>> InviteUsersAsync(InviteUsersRequestDto inDto)
    {
        var currentUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);

        foreach (var invite in inDto.Invitations)
        {
            if ((invite.Type == EmployeeType.DocSpaceAdmin && !currentUser.IsOwner(await _tenantManager.GetCurrentTenantAsync())) ||
                !await _permissionContext.CheckPermissionsAsync(new UserSecurityProvider(Guid.Empty, invite.Type), Constants.Action_AddRemoveUser))
            {
                continue;
            }

            var user = await _userManagerWrapper.AddInvitedUserAsync(invite.Email, invite.Type);
            var link = await _invitationLinkService.GetInvitationLinkAsync(user.Email, invite.Type, _authContext.CurrentAccount.ID);
            var shortenLink = await _urlShortener.GetShortenLinkAsync(link);

            await _studioNotifyService.SendDocSpaceInviteAsync(user.Email, shortenLink);
        }

        var result = new List<EmployeeDto>();

        var users = (await _userManager.GetUsersAsync()).Where(u => u.ActivationStatus == EmployeeActivationStatus.Pending);

        foreach (var user in users)
        {
            result.Add(await _employeeDtoHelper.GetAsync(user));
        }

        return result;
    }

    /// <summary>
    /// Sets a new password to the user with the ID specified in the request.
    /// </summary>
    /// <short>Change a user password</short>
    /// <category>Password</category>
    /// <param type="System.Guid, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People" name="inDto">Request parameters for setting new password</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Detailed user information</returns>
    /// <path>api/2.0/people/{userid}/password</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}/password")]
    [EnableRateLimiting("sensitive_api")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
    public async Task<EmployeeFullDto> ChangeUserPassword(Guid userid, MemberRequestDto inDto)
    {
        await _apiContext.AuthByClaimAsync();
        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(userid), Constants.Action_EditUser);

        var user = await _userManager.GetUsersAsync(userid);

        if (!_userManager.UserExists(user))
        {
            return null;
        }

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        if (!string.IsNullOrEmpty(inDto.Email))
        {
            var address = new MailAddress(inDto.Email);
            if (!string.Equals(address.Address, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = address.Address.ToLowerInvariant();
                user.ActivationStatus = EmployeeActivationStatus.Activated;
                await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);
            }
        }

        inDto.PasswordHash = (inDto.PasswordHash ?? "").Trim();

        if (string.IsNullOrEmpty(inDto.PasswordHash))
        {
            inDto.Password = (inDto.Password ?? "").Trim();

            if (!string.IsNullOrEmpty(inDto.Password))
            {
                inDto.PasswordHash = _passwordHasher.GetClientPassword(inDto.Password);
            }
        }

        if (!string.IsNullOrEmpty(inDto.PasswordHash))
        {
            await _securityContext.SetUserPasswordHashAsync(userid, inDto.PasswordHash);
            await _messageService.SendAsync(MessageAction.UserUpdatedPassword);

            await _cookiesManager.ResetUserCookieAsync(userid);
            await _messageService.SendAsync(MessageAction.CookieSettingsUpdated);
        }

        await _cookiesManager.AuthenticateMeAndSetCookiesAsync(Tenant.Id, userid);
        return await _employeeFullDtoHelper.GetFullAsync(await GetUserInfoAsync(userid.ToString()));
    }

    /// <summary>
    /// Deletes a user with the ID specified in the request from the portal.
    /// </summary>
    /// <short>
    /// Delete a user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Deleted user detailed information</returns>
    /// <path>api/2.0/people/{userid}</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("{userid}")]
    public async Task<EmployeeFullDto> DeleteMemberAsync(string userid)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_AddRemoveUser);

        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id) || user.IsLDAP())
        {
            throw new SecurityException();
        }

        if (user.Status != EmployeeStatus.Terminated)
        {
            throw new Exception("The user is not suspended");
        }

        await CheckReassignProccessAsync(new[] { user.Id });

        var userName = user.DisplayUserName(false, _displayUserSettingsHelper);
        await _userPhotoManager.RemovePhotoAsync(user.Id);
        await _userManager.DeleteUserAsync(user.Id);
        _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, false, false);

        await _messageService.SendAsync(MessageAction.UserDeleted, _messageTarget.Create(user.Id), userName);

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Deletes the current user profile.
    /// </summary>
    /// <short>
    /// Delete my profile
    /// </short>
    /// <category>Profiles</category>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Detailed information about my profile</returns>
    /// <path>api/2.0/people/@self</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("@self")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "ProfileRemove")]
    public async Task<EmployeeFullDto> DeleteProfile()
    {
        await _apiContext.AuthByClaimAsync();

        if (_userManager.IsSystemUser(_securityContext.CurrentAccount.ID))
        {
            throw new SecurityException();
        }

        var user = await GetUserInfoAsync(_securityContext.CurrentAccount.ID.ToString());

        if (!_userManager.UserExists(user))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (user.IsLDAP() || user.IsOwner(Tenant))
        {
            throw new SecurityException();
        }

        await _securityContext.AuthenticateMeWithoutCookieAsync(Core.Configuration.Constants.CoreSystem);
        user.Status = EmployeeStatus.Terminated;

        await _userManager.UpdateUserInfoAsync(user);
        var userName = user.DisplayUserName(false, _displayUserSettingsHelper);
        await _messageService.SendAsync(MessageAction.UsersUpdatedStatus, _messageTarget.Create(user.Id), userName);

        await _cookiesManager.ResetUserCookieAsync(user.Id);
        await _messageService.SendAsync(MessageAction.CookieSettingsUpdated);

        if (_coreBaseSettings.Personal)
        {
            await _userPhotoManager.RemovePhotoAsync(user.Id);
            await _userManager.DeleteUserAsync(user.Id);
            await _messageService.SendAsync(MessageAction.UserDeleted, _messageTarget.Create(user.Id), userName);
        }
        else
        {
            //StudioNotifyService.Instance.SendMsgProfileHasDeletedItself(user);
            //StudioNotifyService.SendMsgProfileDeletion(Tenant.TenantId, user);
        }

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Returns a list of users matching the status filter and search query.
    /// </summary>
    /// <short>
    /// Search users by status filter
    /// </short>
    /// <category>Search</category>
    /// <param type="ASC.Core.Users.EmployeeStatus, ASC.Core.Common" method="url" name="status">User status</param>
    /// <param type="System.String, System" name="query">Search query</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/status/{status}/search</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("status/{status}/search")]
    public async IAsyncEnumerable<EmployeeFullDto> GetAdvanced(EmployeeStatus status, [FromQuery] string query)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var list = (await _userManager.GetUsersAsync(status)).ToAsyncEnumerable();

        if ("group".Equals(_apiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            var groupId = new Guid(_apiContext.FilterValue);
            //Filter by group
            list = list.WhereAwait(async x => await _userManager.IsUserInGroupAsync(x.Id, groupId));
            _apiContext.SetDataFiltered();
        }

        list = list.Where(x => x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1 || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                                (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.ContactsList != null && x.ContactsList.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)));

        await foreach (var item in list)
        {
            yield return await _employeeFullDtoHelper.GetFullAsync(item);
        }
    }

    /// <summary>
    /// Returns a list of profiles for all the portal users.
    /// </summary>
    /// <short>
    /// Get profiles
    /// </short>
    /// <category>Profiles</category>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet]
    public IAsyncEnumerable<EmployeeFullDto> GetAll()
    {
        return GetByStatus(EmployeeStatus.Active);
    }

    /// <summary>
    /// Returns the detailed information about a profile of the user with the email specified in the request.
    /// </summary>
    /// <short>
    /// Get a profile by user email
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" method="url" name="email">User email address</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Detailed profile information</returns>
    /// <path>api/2.0/people/email</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("email")]
    public async Task<EmployeeFullDto> GetByEmailAsync([FromQuery] string email)
    {
        if (_coreBaseSettings.Personal && !(await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID)).IsOwner(Tenant))
        {
            throw new MethodAccessException("Method not available");
        }

        var user = await _userManager.GetUserByEmailAsync(email);
        if (user.Id == Constants.LostUser.Id)
        {
            throw new ItemNotFoundException("User not found");
        }

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Returns the detailed information about a profile of the user with the name specified in the request.
    /// </summary>
    /// <short>
    /// Get a profile by user name
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" method="url" name="username">User name</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Detailed profile information</returns>
    /// <path>api/2.0/people/{username}</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    [HttpGet("{username}", Order = 1)]
    public async Task<EmployeeFullDto> GetById(string username)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var isInvite = _httpContextAccessor.HttpContext.User.Claims
               .Any(role => role.Type == ClaimTypes.Role && ConfirmTypeExtensions.TryParse(role.Value, out var confirmType) && confirmType == ConfirmType.LinkInvite);

        await _apiContext.AuthByClaimAsync();

        var user = await _userManager.GetUserByUserNameAsync(username);
        if (user.Id == Constants.LostUser.Id)
        {
            if (Guid.TryParse(username, out var userId))
            {
                user = await _userManager.GetUsersAsync(userId);
            }
            else
            {
                _logger.ErrorCouldNotGetUserByName(_securityContext.CurrentAccount.ID, username);
            }
        }

        if (user.Id == Constants.LostUser.Id)
        {
            throw new ItemNotFoundException("User not found");
        }

        if (isInvite)
        {
            return await _employeeFullDtoHelper.GetSimple(user);
        }

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Returns a list of profiles filtered by user status.
    /// </summary>
    /// <short>
    /// Get profiles by status
    /// </short>
    /// <param type="ASC.Core.Users.EmployeeStatus, ASC.Core.Common" method="url" name="status">User status</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <category>User status</category>
    /// <path>api/2.0/people/status/{status}</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("status/{status}")]
    public IAsyncEnumerable<EmployeeFullDto> GetByStatus(EmployeeStatus status)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new Exception("Method not available");
        }

        Guid? groupId = null;
        if ("group".Equals(_apiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            groupId = new Guid(_apiContext.FilterValue);
            _apiContext.SetDataFiltered();
        }

        return GetFullByFilter(status, groupId, null, null, null, null, null, null);
    }

    /// <summary>
    /// Returns a list of users with full information about them matching the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Search users and their information by extended filter
    /// </short>
    /// <category>Search</category>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeStatus}, System" name="employeeStatus">User status</param>
    /// <param type="System.Nullable{System.Guid}, System" name="groupId">Group ID</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeActivationStatus}, System" name="activationStatus">Activation status</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeType}, System" name="employeeType">User type</param>
    /// <param type="ASC.Core.Users.EmployeeType, ASC.Core.Common" name="employeeTypes"></param>
    /// <param type="System.Nullable{System.Boolean}, System" name="isAdministrator">Specifies if the user is an administrator or not</param>
    /// <param type="System.Nullable{ASC.Core.Payments}, System" name="payments">User payment status</param>
    /// <param type="System.Nullable{ASC.Core.AccountLoginType}, System" name="accountLoginType">Account login type</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/filter</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("filter")]
    public async IAsyncEnumerable<EmployeeFullDto> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, [FromQuery] EmployeeType[] employeeTypes, bool? isAdministrator, Payments? payments, AccountLoginType? accountLoginType)
    {
        var users = GetByFilterAsync(employeeStatus, groupId, activationStatus, employeeType, employeeTypes, isAdministrator, payments, accountLoginType);

        await foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFullAsync(user);
        }
    }

    /// <summary>
    /// Returns the information about the People module.
    /// </summary>
    /// <short>Get the People information</short>
    /// <category>Module</category>
    /// <returns type="ASC.Api.Core.Module, ASC.Api.Core">Module information</returns>
    /// <path>api/2.0/people/info</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("info")]
    public Module GetModule()
    {
        var product = new PeopleProduct();
        product.Init();

        return new Module(product);
    }

    /// <summary>
    /// Returns a list of users matching the search query. This method uses the query parameters.
    /// </summary>
    /// <short>Search users (using query parameters)</short>
    /// <category>Search</category>
    /// <param type="System.String, System" name="query">Search query</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeDto, ASC.Api.Core">List of users</returns>
    /// <path>api/2.0/people/search</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("search")]
    public IAsyncEnumerable<EmployeeDto> GetPeopleSearch([FromQuery] string query)
    {
        return GetSearch(query);
    }

    /// <summary>
    /// Returns a list of users matching the search query.
    /// </summary>
    /// <short>Search users</short>
    /// <category>Search</category>
    /// <param type="System.String, System" method="url" name="query">Search query</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/@search/{query}</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("@search/{query}")]
    public async IAsyncEnumerable<EmployeeFullDto> GetSearch(string query)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var groupId = Guid.Empty;
        if ("group".Equals(_apiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            groupId = new Guid(_apiContext.FilterValue);
        }

        var users = await _userManager.SearchAsync(query, EmployeeStatus.Active, groupId);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFullAsync(user);
        }
    }

    /// <summary>
    /// Returns a list of users matching the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Search users by extended filter
    /// </short>
    /// <category>Search</category>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeStatus}, System" name="employeeStatus">User status</param>
    /// <param type="System.Nullable{System.Guid}, System" name="groupId">Group ID</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeActivationStatus}, System" name="activationStatus">Activation status</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeType}, System" name="employeeType">User type</param>
    /// <param type="ASC.Core.Users.EmployeeType, ASC.Core.Common" name="employeeTypes"></param>
    /// <param type="System.Nullable{System.Boolean}, System" name="isAdministrator">Specifies if the user is an administrator or not</param>
    /// <param type="System.Nullable{ASC.Core.Payments}, System" name="payments">User payment status</param>
    /// <param type="System.Nullable{ASC.Core.AccountLoginType}, System" name="accountLoginType">Account login type</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeDto, ASC.Api.Core">List of users</returns>
    /// <path>api/2.0/people/simple/filter</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("simple/filter")]
    public async IAsyncEnumerable<EmployeeDto> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, [FromQuery] EmployeeType[] employeeTypes, bool? isAdministrator, Payments? payments, AccountLoginType? accountLoginType)
    {
        var users = GetByFilterAsync(employeeStatus, groupId, activationStatus, employeeType, employeeTypes, isAdministrator, payments, accountLoginType);

        await foreach (var user in users)
        {
            yield return await _employeeDtoHelper.GetAsync(user);
        }
    }

    /// <summary>
    /// Registers a user on the Personal portal.
    /// </summary>
    /// <short>
    /// Register a Personal account
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.RegisterPersonalUserRequestDto, ASC.People" name="inDto">Request parameters for registering a Personal account</param>
    /// <returns type="System.String, System">Error message or empty string</returns>
    /// <path>api/2.0/people/register</path>
    /// <httpMethod>POST</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<string> RegisterUserOnPersonalAsync(RegisterPersonalUserRequestDto inDto)
    {
        if (!_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method is only available on personal.onlyoffice.com");
        }

        try
        {
            if (_coreBaseSettings.CustomMode)
            {
                inDto.Lang = "ru-RU";
            }

            var cultureInfo = _setupInfo.GetPersonalCulture(inDto.Lang).Value;

            if (cultureInfo != null)
            {
                CultureInfo.CurrentUICulture = cultureInfo;
            }

            inDto.Email.ThrowIfNull(new ArgumentException(Resource.ErrorEmailEmpty, "email"));

            if (!inDto.Email.TestEmailRegex())
            {
                throw new ArgumentException(Resource.ErrorNotCorrectEmail, "email");
            }

            if (!SetupInfo.IsSecretEmail(inDto.Email)
                && !string.IsNullOrEmpty(_setupInfo.RecaptchaPublicKey) && !string.IsNullOrEmpty(_setupInfo.RecaptchaPrivateKey))
            {
                var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();

                if (string.IsNullOrEmpty(inDto.RecaptchaResponse)
                    || !await _recaptcha.ValidateRecaptchaAsync(inDto.RecaptchaResponse, ip))
                {
                    throw new RecaptchaException(Resource.RecaptchaInvalid);
                }
            }

            var newUserInfo = await _userManager.GetUserByEmailAsync(inDto.Email);

            if (await _userManager.UserExistsAsync(newUserInfo.Id))
            {
                if (!SetupInfo.IsSecretEmail(inDto.Email) || _securityContext.IsAuthenticated)
                {
                    await _studioNotifyService.SendAlreadyExistAsync(inDto.Email);
                    return string.Empty;
                }

                try
                {
                    await _securityContext.AuthenticateMeAsync(Core.Configuration.Constants.CoreSystem);
                    await _userManager.DeleteUserAsync(newUserInfo.Id);
                }
                finally
                {
                    _securityContext.Logout();
                }
            }
            if (!inDto.Spam)
            {
                try
                {
                    //TODO
                    //const string _databaseID = "com";
                    //using (var db = DbManager.FromHttpContext(_databaseID))
                    //{
                    //    db.ExecuteNonQuery(new SqlInsert("template_unsubscribe", false)
                    //                           .InColumnValue("email", email.ToLowerInvariant())
                    //                           .InColumnValue("reason", "personal")
                    //        );
                    //    Log.Debug(String.Format("Write to template_unsubscribe {0}", email.ToLowerInvariant()));
                    //}
                }
                catch (Exception ex)
                {
                    _logger.DebugWriteToTemplateUnsubscribe(inDto.Email.ToLowerInvariant(), ex);
                }
            }

            await _studioNotifyService.SendInvitePersonalAsync(inDto.Email);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return string.Empty;
    }

    /// <summary>
    /// Deletes a list of the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Delete users
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People" name="inDto">Request parameters for updating portal users</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/delete</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("delete", Order = -1)]
    public async IAsyncEnumerable<EmployeeFullDto> RemoveUsers(UpdateMembersRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_AddRemoveUser);

        await CheckReassignProccessAsync(inDto.UserIds);

        var users = await inDto.UserIds.ToAsyncEnumerable().SelectAwait(async userId => await _userManager.GetUsersAsync(userId))
            .Where(u => !_userManager.IsSystemUser(u.Id) && !u.IsLDAP()).ToListAsync();

        var userNames = users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)).ToList();

        foreach (var user in users)
        {
            if (user.Status != EmployeeStatus.Terminated)
            {
                continue;
            }

            await _userPhotoManager.RemovePhotoAsync(user.Id);
            await _userManager.DeleteUserAsync(user.Id);
            _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, false, false);
        }

        await _messageService.SendAsync(MessageAction.UsersDeleted, _messageTarget.Create(users.Select(x => x.Id)), userNames);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFullAsync(user);
        }
    }

    /// <summary>
    /// Resends emails to the users who have not activated their emails.
    /// </summary>
    /// <short>
    /// Resend activation emails
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People" name="inDto">Request parameters for updating portal users</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/invite</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [AllowNotPayment]
    [HttpPut("invite")]
    public async IAsyncEnumerable<EmployeeFullDto> ResendUserInvitesAsync(UpdateMembersRequestDto inDto)
    {
        IEnumerable<UserInfo> users = null;

        if (inDto.ResendAll)
        {
            await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(Guid.Empty, EmployeeType.User), Constants.Action_AddRemoveUser);
            users = (await _userManager.GetUsersAsync()).Where(u => u.ActivationStatus == EmployeeActivationStatus.Pending);
        }
        else
        {
            users = await inDto.UserIds.ToAsyncEnumerable()
                .Where(userId => !_userManager.IsSystemUser(userId))
                .SelectAwait(async userId => await _userManager.GetUsersAsync(userId)).ToListAsync();
        }

        foreach (var user in users)
        {
            if (user.IsActive)
            {
                continue;
            }

            var viewer = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);

            if (viewer == null)
            {
                throw new Exception(Resource.ErrorAccessDenied);
            }

            if (await _userManager.IsDocSpaceAdminAsync(viewer) || viewer.Id == user.Id)
            {
                if (user.ActivationStatus == EmployeeActivationStatus.Activated)
                {
                    user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                }
                if (user.ActivationStatus == (EmployeeActivationStatus.AutoGenerated | EmployeeActivationStatus.Activated))
                {
                    user.ActivationStatus = EmployeeActivationStatus.AutoGenerated;
                }

                await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);
            }

            if (user.ActivationStatus == EmployeeActivationStatus.Pending)
            {
                var type = await _userManager.GetUserTypeAsync(user.Id);

                if (!await _permissionContext.CheckPermissionsAsync(new UserSecurityProvider(type), Constants.Action_AddRemoveUser))
                {
                    continue;
                }

                var link = await _invitationLinkService.GetInvitationLinkAsync(user.Email, type, _authContext.CurrentAccount.ID);
                var shortenLink = await _urlShortener.GetShortenLinkAsync(link);

                await _studioNotifyService.SendDocSpaceInviteAsync(user.Email, shortenLink);
            }
            else
            {
                await _studioNotifyService.SendEmailActivationInstructionsAsync(user, user.Email);
            }
        }

        await _messageService.SendAsync(MessageAction.UsersSentActivationInstructions, _messageTarget.Create(users.Select(x => x.Id)), users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)));

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFullAsync(user);
        }
    }

    /// <summary>
    /// Returns a theme which is set to the current portal.
    /// </summary>
    /// <short>
    /// Get portal theme
    /// </short>
    /// <category>Theme</category>
    /// <returns type="ASC.Web.Core.Users.DarkThemeSettings, ASC.Web.Core">Theme</returns>
    /// <path>api/2.0/people/theme</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("theme")]
    public async Task<DarkThemeSettings> GetThemeAsync()
    {
        return await _settingsManager.LoadForCurrentUserAsync<DarkThemeSettings>();
    }

    /// <summary>
    /// Changes the current portal theme.
    /// </summary>
    /// <short>
    /// Change portal theme
    /// </short>
    /// <category>Theme</category>
    /// <param type="ASC.People.ApiModels.RequestDto.DarkThemeSettingsRequestDto, ASC.People" name="inDto">Theme settings request parameters</param>
    /// <returns type="ASC.Web.Core.Users.DarkThemeSettings, ASC.Web.Core">Theme</returns>
    /// <path>api/2.0/people/theme</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("theme")]
    public async Task<DarkThemeSettings> ChangeThemeAsync(DarkThemeSettingsRequestDto inDto)
    {
        var darkThemeSettings = new DarkThemeSettings
        {
            Theme = inDto.Theme
        };

        await _settingsManager.SaveForCurrentUserAsync(darkThemeSettings);

        return darkThemeSettings;
    }

    /// <summary>
    /// Returns the detailed information about the current user profile.
    /// </summary>
    /// <short>
    /// Get my profile
    /// </short>
    /// <category>Profiles</category>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Detailed information about my profile</returns>
    /// <path>api/2.0/people/@self</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("@self")]
    public async Task<EmployeeFullDto> SelfAsync()
    {
        var user = await _userManager.GetUserAsync(_securityContext.CurrentAccount.ID, EmployeeFullDtoHelper.GetExpression(_apiContext));

        var result = await _employeeFullDtoHelper.GetFullAsync(user);

        result.Theme = (await _settingsManager.LoadForCurrentUserAsync<DarkThemeSettings>()).Theme;

        return result;
    }

    /// <summary>
    /// Sends a message to the user email with the instructions to change the email address connected to the portal.
    /// </summary>
    /// <short>
    /// Send instructions to change email
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People" name="inDto">Request parameters for updating user information</param>
    /// <returns type="System.Object, System">Message text</returns>
    /// <path>api/2.0/people/email</path>
    /// <httpMethod>POST</httpMethod>
    [AllowNotPayment]
    [HttpPost("email")]
    public async Task<object> SendEmailChangeInstructionsAsync(UpdateMemberRequestDto inDto)
    {
        Guid.TryParse(inDto.UserId, out var userid);

        if (userid == Guid.Empty)
        {
            throw new ArgumentNullException("userid");
        }

        var email = (inDto.Email ?? "").Trim();

        if (string.IsNullOrEmpty(email))
        {
            throw new Exception(Resource.ErrorEmailEmpty);
        }

        if (!email.TestEmailRegex())
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }

        var viewer = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
        var viewerIsAdmin = await _userManager.IsDocSpaceAdminAsync(viewer);
        var user = await _userManager.GetUsersAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (!viewerIsAdmin && viewer.Id != user.Id)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        if (user.IsOwner(Tenant) && viewer.Id != user.Id)
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        var existentUser = await _userManager.GetUserByEmailAsync(email);

        if (existentUser.Id != Constants.LostUser.Id)
        {
            throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
        }

        if (!viewerIsAdmin)
        {
            await _studioNotifyService.SendEmailChangeInstructionsAsync(user, email);
        }
        else
        {
            if (email == user.Email)
            {
                throw new Exception(Resource.ErrorEmailsAreTheSame);
            }

            user.Email = email;
            user.ActivationStatus = EmployeeActivationStatus.NotActivated;
            await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);
            await _studioNotifyService.SendEmailActivationInstructionsAsync(user, email);
        }

        await _messageService.SendAsync(MessageAction.UserSentEmailChangeInstructions, user.DisplayUserName(false, _displayUserSettingsHelper));

        return string.Format(Resource.MessageEmailChangeInstuctionsSentOnEmail, email);
    }

    /// <summary>
    /// Reminds a password to the user using the email address specified in the request.
    /// </summary>
    /// <short>
    /// Remind a user password
    /// </short>
    /// <category>Password</category>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People" name="inDto">Member request parameters</param>
    /// <returns type="System.Object, System">Email with the password</returns>
    /// <path>api/2.0/people/password</path>
    /// <httpMethod>POST</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowNotPayment]
    [AllowAnonymous]
    [HttpPost("password")]
    [EnableRateLimiting("sensitive_api")]
    public async Task<object> SendUserPasswordAsync(MemberRequestDto inDto)
    {
        if (_authContext.IsAuthenticated)
        {
            var currentUser = await _userManager.GetUserByEmailAsync(inDto.Email);
            if (currentUser.Id != _authContext.CurrentAccount.ID && !(await _userManager.IsDocSpaceAdminAsync(_authContext.CurrentAccount.ID)))
            {
                throw new Exception(Resource.ErrorAccessDenied);
            }
        }

        var error = await _userManagerWrapper.SendUserPasswordAsync(inDto.Email);
        if (!string.IsNullOrEmpty(error))
        {
            _logger.ErrorPasswordRecovery(inDto.Email, error);
        }

        return string.Format(Resource.MessageYourPasswordSendedToEmail, inDto.Email);
    }

    /// <summary>
    /// Sets the required activation status to the list of users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Set an activation status to the users
    /// </short>
    /// <category>User status</category>
    /// <param type="ASC.Core.Users.EmployeeActivationStatus, ASC.Core.Common" method="url" name="activationstatus">Activation status</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People" name="inDto">Request parameters for updating user information</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/activationstatus/{activationstatus}</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [AllowNotPayment]
    [HttpPut("activationstatus/{activationstatus}")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateEmployeeActivationStatus(EmployeeActivationStatus activationstatus, UpdateMembersRequestDto inDto)
    {
        await _apiContext.AuthByClaimAsync();

        foreach (var id in inDto.UserIds.Where(userId => !_userManager.IsSystemUser(userId)))
        {
            await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(id), Constants.Action_EditUser);
            var u = await _userManager.GetUsersAsync(id);
            if (u.Id == Constants.LostUser.Id || u.IsLDAP())
            {
                continue;
            }

            u.ActivationStatus = activationstatus;
            await _userManager.UpdateUserInfoAsync(u);

            if (activationstatus == EmployeeActivationStatus.Activated
                && u.IsOwner(_tenantManager.GetCurrentTenant()))
            {
                var settings = _settingsManager.Load<FirstEmailConfirmSettings>();

                if (settings.IsFirst)
                {
                    await _studioNotifyService.SendAdminWelcomeAsync(u);

                    settings.IsFirst = false;
                    _settingsManager.Save(settings);
                }
            }

            yield return await _employeeFullDtoHelper.GetFullAsync(u);
        }
    }

    /// <summary>
    /// Updates the user language with the parameter specified in the request.
    /// </summary>
    /// <short>
    /// Update user language
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People" name="inDto">Request parameters for updating user information</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Detailed user information</returns>
    /// <path>api/2.0/people/{userid}/culture</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}/culture")]
    public async Task<EmployeeFullDto> UpdateMemberCulture(string userid, UpdateMemberRequestDto inDto)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        var curLng = user.CultureName;

        if (_setupInfo.EnabledCultures.Find(c => string.Equals(c.Name, inDto.CultureName, StringComparison.InvariantCultureIgnoreCase)) != null)
        {
            if (curLng != inDto.CultureName)
            {
                user.CultureName = inDto.CultureName;

                try
                {
                    await _userManager.UpdateUserInfoAsync(user);
                }
                catch
                {
                    user.CultureName = curLng;
                    throw;
                }

                await _messageService.SendAsync(MessageAction.UserUpdatedLanguage, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

            }
        }

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Updates the data for the selected portal user with the first name, last name, email address, and/or optional parameters specified in the request.
    /// </summary>
    /// <short>
    /// Update a user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" method="url" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People" name="inDto">Member request parameters</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">Updated user with the detailed information</returns>
    /// <path>api/2.0/people/{userid}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}", Order = 1)]
    public async Task<EmployeeFullDto> UpdateMember(string userid, UpdateMemberRequestDto inDto)
    {
        var user = await GetUserInfoAsync(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(user.Id), Constants.Action_EditUser);
        var self = _securityContext.CurrentAccount.ID.Equals(user.Id);
        var resetDate = new DateTime(1900, 01, 01);

        //Update it

        var isLdap = user.IsLDAP();
        var isSso = user.IsSSO();
        var isDocSpaceAdmin = await _webItemSecurity.IsProductAdministratorAsync(WebItemManager.PeopleProductID, _securityContext.CurrentAccount.ID);

        if (!isLdap && !isSso)
        {
            //Set common fields

            user.FirstName = inDto.Firstname ?? user.FirstName;
            user.LastName = inDto.Lastname ?? user.LastName;
            user.Location = inDto.Location ?? user.Location;

            if (isDocSpaceAdmin)
            {
                user.Title = inDto.Title ?? user.Title;
            }
        }

        if (!_userFormatter.IsValidUserName(user.FirstName, user.LastName))
        {
            throw new Exception(Resource.ErrorIncorrectUserName);
        }

        user.Notes = inDto.Comment ?? user.Notes;
        user.Sex = ("male".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase)
            ? true
            : ("female".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null)) ?? user.Sex;

        user.BirthDate = inDto.Birthday != null ? _tenantUtil.DateTimeFromUtc(inDto.Birthday) : user.BirthDate;

        if (user.BirthDate == resetDate)
        {
            user.BirthDate = null;
        }

        user.WorkFromDate = inDto.Worksfrom != null ? _tenantUtil.DateTimeFromUtc(inDto.Worksfrom) : user.WorkFromDate;

        if (user.WorkFromDate == resetDate)
        {
            user.WorkFromDate = null;
        }

        //Update contacts
        await UpdateContactsAsync(inDto.Contacts, user);
        await UpdateDepartmentsAsync(inDto.Department, user);

        if (inDto.Files != await _userPhotoManager.GetPhotoAbsoluteWebPath(user.Id))
        {
            await UpdatePhotoUrlAsync(inDto.Files, user);
        }
        if (inDto.Disable.HasValue)
        {
            user.Status = inDto.Disable.Value ? EmployeeStatus.Terminated : EmployeeStatus.Active;
            user.TerminatedDate = inDto.Disable.Value ? DateTime.UtcNow : null;
        }

        // change user type
        var canBeGuestFlag = !user.IsOwner(Tenant) && !await _userManager.IsDocSpaceAdminAsync(user) && (await user.GetListAdminModulesAsync(_webItemSecurity, _webItemManager)).Count == 0 && !user.IsMe(_authContext);

        if (inDto.IsUser.HasValue)
        {
            var isUser = inDto.IsUser.Value;
            if (isUser && !await _userManager.IsUserAsync(user) && canBeGuestFlag)
            {
                await _countUserChecker.CheckAppend();
                await _userManager.AddUserIntoGroupAsync(user.Id, Constants.GroupUser.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
            }

            if (!self && !isUser && await _userManager.IsUserAsync(user))
            {
                await _countPaidUserChecker.CheckAppend();
                await _userManager.RemoveUserFromGroupAsync(user.Id, Constants.GroupUser.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
            }
        }
        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);

        await _messageService.SendAsync(MessageAction.UserUpdated, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper), user.Id);

        if (inDto.Disable.HasValue && inDto.Disable.Value)
        {
            await _cookiesManager.ResetUserCookieAsync(user.Id);
            await _messageService.SendAsync(MessageAction.CookieSettingsUpdated);
        }

        return await _employeeFullDtoHelper.GetFullAsync(user);
    }

    /// <summary>
    /// Changes a status for the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Change a user status
    /// </short>
    /// <category>User status</category>
    /// <param type="ASC.Core.Users.EmployeeStatus, ASC.Core.Common" method="url" name="status">New user status</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People" name="inDto">Request parameters for updating user information</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/status/{status}</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("status/{status}")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateUserStatus(EmployeeStatus status, UpdateMembersRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        var users = await inDto.UserIds.ToAsyncEnumerable().SelectAwait(async userId => await _userManager.GetUsersAsync(userId))
            .Where(u => !_userManager.IsSystemUser(u.Id) && !u.IsLDAP()).ToListAsync();

        foreach (var user in users)
        {
            if (user.IsOwner(Tenant) || user.IsMe(_authContext))
            {
                continue;
            }

            switch (status)
            {
                case EmployeeStatus.Active:
                    if (user.Status == EmployeeStatus.Terminated)
                    {
                        if (!await _userManager.IsUserAsync(user))
                        {
                            await _countPaidUserChecker.CheckAppend();
                        }
                        else
                        {
                            await _countUserChecker.CheckAppend();
                        }

                        user.Status = EmployeeStatus.Active;

                        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);
                    }
                    break;
                case EmployeeStatus.Terminated:
                    user.Status = EmployeeStatus.Terminated;

                    await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);

                    await _cookiesManager.ResetUserCookieAsync(user.Id);
                    await _messageService.SendAsync(MessageAction.CookieSettingsUpdated);
                    break;
            }
        }

        await _messageService.SendAsync(MessageAction.UsersUpdatedStatus, _messageTarget.Create(users.Select(x => x.Id)), users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)));

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFullAsync(user);
        }
    }

    /// <summary>
    /// Changes a type for the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Change a user type
    /// </short>
    /// <category>User type</category>
    /// <param type="ASC.Core.Users.EmployeeType, ASC.Core.Common" method="url" name="type">New user type</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People" name="inDto">Request parameters for updating user information</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/type/{type}</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("type/{type}")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateUserTypeAsync(EmployeeType type, UpdateMembersRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(new UserSecurityProvider(type), Constants.Action_AddRemoveUser);

        var users = await inDto.UserIds
            .ToAsyncEnumerable()
            .Where(userId => !_userManager.IsSystemUser(userId))
            .SelectAwait(async userId => await _userManager.GetUsersAsync(userId))
            .ToListAsync();

        var updatedUsers = new List<UserInfo>(users.Count());

        foreach (var user in users)
        {
            if (await _userManagerWrapper.UpdateUserTypeAsync(user, type))
            {
                updatedUsers.Add(user);
            }
        }

        await _messageService.SendAsync(MessageAction.UsersUpdatedType, _messageTarget.CreateFromGroupValues(users.Select(x => x.Id.ToString())),
        users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)), users.Select(x => x.Id).ToList(), type);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFullAsync(user);
        }
    }

    ///<summary>
    /// Starts the process of recalculating quota.
    /// </summary>
    /// <short>
    /// Recalculate quota 
    /// </short>
    /// <category>Quota</category>
    /// <path>api/2.0/people/recalculatequota</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns></returns>
    [HttpGet("recalculatequota")]
    public async Task RecalculateQuotaAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        _usersQuotaSyncOperation.RecalculateQuota(await _tenantManager.GetCurrentTenantAsync());
    }

    /// <summary>
    /// Checks the process of recalculating quota.
    /// </summary>
    /// <short>
    /// Check quota recalculation
    /// </short>
    /// <category>Quota</category>
    /// <returns type="ASC.Api.Core.Model.TaskProgressDto, ASC.Api.Core.Model">Task progress</returns>
    /// <path>api/2.0/people/checkrecalculatequota</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("checkrecalculatequota")]
    public async Task<TaskProgressDto> CheckRecalculateQuotaAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        return _usersQuotaSyncOperation.CheckRecalculateQuota(await _tenantManager.GetCurrentTenantAsync());
    }

    /// <summary>
    /// Changes a quota limit for the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Change a user quota limit
    /// </short>
    /// <category>Quota</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People" name="inDto">Request parameters for updating user information</param>
    /// <returns type="ASC.Web.Api.Models.EmployeeFullDto, ASC.Api.Core">List of users with the detailed information</returns>
    /// <path>api/2.0/people/quota</path>
    /// <httpMethod>PUT</httpMethod>
    /// <collection>list</collection>
    [HttpPut("quota")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateUserQuotaAsync(UpdateMembersQuotaRequestDto inDto)
    {
        var users = inDto.UserIds.ToAsyncEnumerable()
            .Where(userId => !_userManager.IsSystemUser(userId))
            .SelectAwait(async userId => await _userManager.GetUsersAsync(userId));

        await foreach (var user in users)
        {
            if (inDto.Quota != -1)
            {
                var usedSpace = Math.Max(0,
                    (await _quotaService.FindUserQuotaRowsAsync(
                            await _tenantManager.GetCurrentTenantIdAsync(),
                            user.Id
                        ))
                .Where(r => !string.IsNullOrEmpty(r.Tag)).Sum(r => r.Counter));

                var tenanSpaceQuota = (await _quotaService.GetTenantQuotaAsync(Tenant.Id)).MaxTotalSize;

                if (tenanSpaceQuota < inDto.Quota || usedSpace > inDto.Quota)
                {
                    continue;
                }
            }

            var quotaSettings = await _settingsManager.LoadAsync<TenantUserQuotaSettings>();

            await _settingsManager.SaveAsync(new UserQuotaSettings { UserQuota = inDto.Quota }, user);

            yield return await _employeeFullDtoHelper.GetFullAsync(user);
        }
    }


    private async Task UpdateDepartmentsAsync(IEnumerable<Guid> department, UserInfo user)
    {
        if (!await _permissionContext.CheckPermissionsAsync(Constants.Action_EditGroups))
        {
            return;
        }

        if (department == null)
        {
            return;
        }

        var groups = await _userManager.GetUserGroupsAsync(user.Id);
        var managerGroups = new List<Guid>();
        foreach (var groupInfo in groups)
        {
            await _userManager.RemoveUserFromGroupAsync(user.Id, groupInfo.ID);
            var managerId = await _userManager.GetDepartmentManagerAsync(groupInfo.ID);
            if (managerId == user.Id)
            {
                managerGroups.Add(groupInfo.ID);
                await _userManager.SetDepartmentManagerAsync(groupInfo.ID, Guid.Empty);
            }
        }
        foreach (var guid in department)
        {
            var userDepartment = await _userManager.GetGroupInfoAsync(guid);
            if (userDepartment != Constants.LostGroupInfo)
            {
                await _userManager.AddUserIntoGroupAsync(user.Id, guid);
                if (managerGroups.Contains(guid))
                {
                    await _userManager.SetDepartmentManagerAsync(guid, user.Id);
                }
            }
        }
    }

    private async Task CheckReassignProccessAsync(IEnumerable<Guid> userIds)
    {
        foreach (var userId in userIds)
        {
            var reassignStatus = _queueWorkerReassign.GetProgressItemStatus(Tenant.Id, userId);
            if (reassignStatus == null || reassignStatus.IsCompleted)
            {
                continue;
            }

            var userName = (await _userManager.GetUsersAsync(userId)).DisplayUserName(_displayUserSettingsHelper);

            throw new Exception(string.Format(Resource.ReassignDataRemoveUserError, userName));
        }
    }

    private async IAsyncEnumerable<UserInfo> GetByFilterAsync(
        EmployeeStatus? employeeStatus,
        Guid? groupId,
        EmployeeActivationStatus? activationStatus,
        EmployeeType? employeeType,
        IEnumerable<EmployeeType> employeeTypes,
        bool? isDocSpaceAdministrator,
        Payments? payments,
        AccountLoginType? accountLoginType)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var isDocSpaceAdmin = (await _userManager.IsDocSpaceAdminAsync(_securityContext.CurrentAccount.ID)) ||
                      await _webItemSecurity.IsProductAdministratorAsync(WebItemManager.PeopleProductID, _securityContext.CurrentAccount.ID);

        var excludeGroups = new List<Guid>();
        var includeGroups = new List<List<Guid>>();
        var combinedGroups = new List<Tuple<List<List<Guid>>, List<Guid>>>();

        if (groupId.HasValue)
        {
            includeGroups.Add(new List<Guid> { groupId.Value });
        }

        if (employeeType.HasValue)
        {
            FilterByUserType(employeeType.Value, includeGroups, excludeGroups);
        }
        else if (employeeTypes != null && employeeTypes.Any())
        {
            foreach (var et in employeeTypes)
            {
                var combinedIncludeGroups = new List<List<Guid>>();
                var combinedExcludeGroups = new List<Guid>();
                FilterByUserType(et, combinedIncludeGroups, combinedExcludeGroups);
                combinedGroups.Add(new(combinedIncludeGroups, combinedExcludeGroups));
            }
        }

        if (payments != null)
        {
            switch (payments)
            {
                case Payments.Paid:
                    excludeGroups.Add(Constants.GroupUser.ID);
                    break;
                case Payments.Free:
                    includeGroups.Add(new List<Guid> { Constants.GroupUser.ID });
                    break;
            }
        }

        if (isDocSpaceAdministrator.HasValue && isDocSpaceAdministrator.Value)
        {
            var adminGroups = new List<Guid>
            {
                    Constants.GroupAdmin.ID
            };
            var products = _webItemManager.GetItemsAll().Where(i => i is IProduct || i.ID == WebItemManager.MailProductID);
            adminGroups.AddRange(products.Select(r => r.ID));

            includeGroups.Add(adminGroups);
        }

        var totalCountTask = _userManager.GetUsersCountAsync(isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, combinedGroups, activationStatus, accountLoginType,
            _apiContext.FilterValue);

        var users = _userManager.GetUsers(isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, combinedGroups, activationStatus, accountLoginType, 
            _apiContext.FilterValue, _apiContext.SortBy, !_apiContext.SortDescending, _apiContext.Count, _apiContext.StartIndex);

        var counter = 0;

        await foreach(var user in users)
        {
            counter++;

            yield return user;
        }

        _apiContext.SetCount(counter).SetTotalCount(await totalCountTask);

        void FilterByUserType(EmployeeType employeeType, List<List<Guid>> includeGroups, List<Guid> excludeGroups)
        {
            switch (employeeType)
            {
                case EmployeeType.DocSpaceAdmin:
                    includeGroups.Add(new List<Guid> { Constants.GroupAdmin.ID });
                    break;
                case EmployeeType.RoomAdmin:
                    excludeGroups.Add(Constants.GroupUser.ID);
                    excludeGroups.Add(Constants.GroupAdmin.ID);
                    excludeGroups.Add(Constants.GroupCollaborator.ID);
                    break;
                case EmployeeType.Collaborator:
                    includeGroups.Add(new List<Guid> { Constants.GroupCollaborator.ID });
                    break;
                case EmployeeType.User:
                    includeGroups.Add(new List<Guid> { Constants.GroupUser.ID });
                    break;
            }
        }
    }

    ///// <summary>
    ///// Imports the new portal users with the first name, last name, and email address.
    ///// </summary>
    ///// <short>
    ///// Import users
    ///// </short>
    ///// <category>Profiles</category>
    ///// <param type="System.String, System" name="userList">List of users</param>
    ///// <param type="System.Boolean, System" name="importUsersAsCollaborators" optional="true">Specifies whether to import users as guests or not</param>
    ///// <returns></returns>
    ///// <path>api/2.0/people/import/save</path>
    ///// <httpMethod>POST</httpMethod>
    //[HttpPost("import/save")]
    //public void SaveUsers(string userList, bool importUsersAsCollaborators)
    //{
    //    lock (progressQueue.SynchRoot)
    //    {
    //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
    //var tenant = CoreContext.TenantManager.GetCurrentTenant();
    //Cache.Insert("REWRITE_URL" + tenant.TenantId, HttpContext.Current.Request.GetUrlRewriter().ToString(), TimeSpan.FromMinutes(5));
    //        if (task != null && task.IsCompleted)
    //        {
    //            progressQueue.Remove(task);
    //            task = null;
    //        }
    //        if (task == null)
    //        {
    //            progressQueue.Add(new ImportUsersTask(userList, importUsersAsCollaborators, GetHttpHeaders(HttpContext.Current.Request))
    //            {
    //                Id = TenantProvider.CurrentTenantID,
    //                UserId = SecurityContext.CurrentAccount.ID,
    //                Percentage = 0
    //            });
    //        }
    //    }
    //}

    // <summary>
    // Returns a status of the current user.
    // </summary>
    // <short>
    // Get a user status
    // </short>
    // <returns tye="System.Object, System">Current user information</returns>
    // <category>User status</category>
    // <path>api/2.0/people/import/status</path>
    // <httpMethod>GET</httpMethod>
    //[HttpGet("import/status")]
    //public object GetStatus()
    //{
    //    lock (progressQueue.SynchRoot)
    //    {
    //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
    //        if (task == null) return null;

    //        return new
    //        {
    //            Completed = task.IsCompleted,
    //            Percents = (int)task.Percentage,
    //            UserCounter = task.GetUserCounter,
    //            Status = (int)task.Status,
    //            Error = (string)task.Error,
    //            task.Data
    //        };
    //    }
    //}
}

[ConstraintRoute("int")]
public class UserControllerAdditionalInternal : UserControllerAdditional<int>
{
    public UserControllerAdditionalInternal(
        EmployeeFullDtoHelper employeeFullDtoHelper, 
        FileSecurity fileSecurity, 
        ApiContext apiContext, 
        IDaoFactory daoFactory) 
        : base(employeeFullDtoHelper, fileSecurity, apiContext, daoFactory)
    {
        
    }
}

public class UserControllerAdditionalThirdParty : UserControllerAdditional<string>
{
    public UserControllerAdditionalThirdParty(
        EmployeeFullDtoHelper employeeFullDtoHelper, 
        FileSecurity fileSecurity, 
        ApiContext apiContext, 
        IDaoFactory daoFactory) 
        : base(employeeFullDtoHelper, fileSecurity, apiContext, daoFactory)
    {
        
    }
}

public class UserControllerAdditional<T> : ApiControllerBase
{
    private readonly EmployeeFullDtoHelper _employeeFullDtoHelper;
    private readonly FileSecurity _fileSecurity;
    private readonly ApiContext _apiContext;
    private readonly IDaoFactory _daoFactory;

    public UserControllerAdditional(
        EmployeeFullDtoHelper employeeFullDtoHelper, 
        FileSecurity fileSecurity, 
        ApiContext apiContext, 
        IDaoFactory daoFactory)
    {
        _employeeFullDtoHelper = employeeFullDtoHelper;
        _fileSecurity = fileSecurity;
        _apiContext = apiContext;
        _daoFactory = daoFactory;
    }

    [HttpGet("room/{id}")]
    public async IAsyncEnumerable<EmployeeFullDto> GetUsersWithRoomSharedAsync(T id, EmployeeStatus? employeeStatus, EmployeeActivationStatus? activationStatus, bool? excludeShared)
    {
        var offset = Convert.ToInt32(_apiContext.StartIndex);
        var count = Convert.ToInt32(_apiContext.Count);

        var room = (await _daoFactory.GetFolderDao<T>().GetFolderAsync(id)).NotFoundIfNull();
        var totalCountTask = _fileSecurity.GetUsersWithSharedCountAsync(room, _apiContext.FilterValue, employeeStatus, activationStatus, excludeShared ?? false);

        var counter = 0;

        await foreach (var u in _fileSecurity.GetUsersWithSharedAsync(room, _apiContext.FilterValue, employeeStatus, activationStatus, excludeShared ?? false, offset, 
                           count))
        {
            counter++;
            
            yield return await _employeeFullDtoHelper.GetFullAsync(u.UserInfo, u.Shared);
        }

        _apiContext.SetCount(counter).SetTotalCount(await totalCountTask);
    }
}