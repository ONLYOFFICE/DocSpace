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

using ASC.Api.Core.Security;
using ASC.Common.Log;

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
        IQuotaService quotaService)
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
    }

    /// <summary>
    /// Adds an activated portal user with the first name, last name, email address, and several optional parameters specified in the request.
    /// </summary>
    /// <short>
    /// Add an activated user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Member request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>Type</b> (EmployeeType) - employee type (All, RoomAdmin, User, DocSpaceAdmin, Collaborator),</li>
    ///     <li><b>Email</b> (string) - email,</li>
    ///     <li><b>Firstname</b> (string) - first name,</li>
    ///     <li><b>Lastname</b> (string) - last name,</li>
    ///     <li><b>Department</b> (Guid[]) - list of user departments,</li>
    ///     <li><b>Title</b> (string) - title,</li>
    ///     <li><b>Location</b> (string) - location,</li>
    ///     <li><b>Sex</b> (string) - sex (male or female),</li>
    ///     <li><b>Birthday</b> (ApiDateTime) - birthday,</li>
    ///     <li><b>Worksfrom</b> (ApiDateTime) - registration date (if it is not specified, then the current date will be set),</li>
    ///     <li><b>Comment</b> (string) - comment,</li>
    ///     <li><b>Contacts</b> (IEnumerable&lt;Contact&gt;) - list of user contacts,</li>
    ///     <li><b>Files</b> (string) - avatar photo URL,</li>
    ///     <li><b>Password</b> (string) - user password,</li>
    ///     <li><b>PasswordHash</b> (string) - password hash.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Newly added user with the detailed information</returns>
    /// <path>api/2.0/people/active</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("active")]
    public async Task<EmployeeFullDto> AddMemberAsActivated(MemberRequestDto inDto)
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(inDto.Type), Constants.Action_AddRemoveUser);

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
                _userManagerWrapper.CheckPasswordPolicy(inDto.Password);
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

        UpdateContacts(inDto.Contacts, user);

        _cache.Insert("REWRITE_URL" + _tenantManager.GetCurrentTenant().Id, HttpContext.Request.GetUrlRewriter().ToString(), TimeSpan.FromMinutes(5));
        user = await _userManagerWrapper.AddUser(user, inDto.PasswordHash, true, false, inDto.Type,
            false, true, true);

        user.ActivationStatus = EmployeeActivationStatus.Activated;

        await UpdateDepartments(inDto.Department, user);

        if (inDto.Files != _userPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            await UpdatePhotoUrl(inDto.Files, user);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Adds a new portal user with the first name, last name, email address, and several optional parameters specified in the request.
    /// </summary>
    /// <short>
    /// Add a user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Member request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>Type</b> (EmployeeType) - employee type (All, RoomAdmin, User, DocSpaceAdmin, Collaborator),</li>
    ///     <li><b>IsUser</b> (bool) - specifies if this is a guest or a user,</li>
    ///     <li><b>Email</b> (string) - email,</li>
    ///     <li><b>Firstname</b> (string) - first name,</li>
    ///     <li><b>Lastname</b> (string) - last name,</li>
    ///     <li><b>Department</b> (Guid[]) - list of user departments,</li>
    ///     <li><b>Title</b> (string) - title,</li>
    ///     <li><b>Location</b> (string) - location,</li>
    ///     <li><b>Sex</b> (string) - sex (male or female),</li>
    ///     <li><b>Birthday</b> (ApiDateTime) - birthday,</li>
    ///     <li><b>Worksfrom</b> (ApiDateTime) - registration date (if it is not specified, then the current date will be set),</li>
    ///     <li><b>Comment</b> (string) - comment,</li>
    ///     <li><b>Contacts</b> (IEnumerable&lt;Contact&gt;) - list of user contacts,</li>
    ///     <li><b>Files</b> (string) - avatar photo URL,</li>
    ///     <li><b>Password</b> (string) - password,</li>
    ///     <li><b>PasswordHash</b> (string) - password hash,</li>
    ///     <li><b>FromInviteLink</b> (bool) - specifies if the user is added via the invitation link or not,</li>
    ///     <li><b>Key</b> (string) - key,</li>
    ///     <li><b>CultureName</b> (string) - language.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Newly added user with the detailed information</returns>
    /// <path>api/2.0/people</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    public async Task<EmployeeFullDto> AddMember(MemberRequestDto inDto)
    {
        _apiContext.AuthByClaim();

        var linkData = inDto.FromInviteLink ? await _invitationLinkService.GetProcessedLinkDataAsync(inDto.Key, inDto.Email, inDto.Type) : null;
        if (linkData is { IsCorrect: false })
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
        }

        if (linkData != null)
        { 
            _permissionContext.DemandPermissions(new UserSecurityProvider(Guid.Empty, linkData.EmployeeType) ,Constants.Action_AddRemoveUser);
        }
        else
        {
            _permissionContext.DemandPermissions(Constants.Action_AddRemoveUser);
        }

        inDto.Type = linkData?.EmployeeType ?? inDto.Type;

        var user = new UserInfo();

        var byEmail = linkData?.LinkType == InvitationLinkType.Individual;

        if (byEmail)
        {
            user = _userManager.GetUserByEmail(inDto.Email);

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
                _userManagerWrapper.CheckPasswordPolicy(inDto.Password);
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

        UpdateContacts(inDto.Contacts, user, !inDto.FromInviteLink);

        _cache.Insert("REWRITE_URL" + _tenantManager.GetCurrentTenant().Id, HttpContext.Request.GetUrlRewriter().ToString(), TimeSpan.FromMinutes(5));

        user = await _userManagerWrapper.AddUser(user, inDto.PasswordHash, inDto.FromInviteLink, true, inDto.Type,
            inDto.FromInviteLink && linkData is { IsCorrect: true }, true, true, byEmail);

        await UpdateDepartments(inDto.Department, user);

        if (inDto.Files != _userPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            await UpdatePhotoUrl(inDto.Files, user);
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

        if (inDto.IsUser)
        {
            _messageService.Send(MessageAction.GuestCreated, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));
        }
        else
        {
            _messageService.Send(MessageAction.UserCreated, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper), user.Id);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Invites users specified in the request to the current portal.
    /// </summary>
    /// <short>
    /// Invite users
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.InviteUsersRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for inviting users: <![CDATA[
    /// <ul>
    ///     <li><b>Invitations</b> (IEnumerable&lt;UserInvitation&gt;) - list of user invitations:</li>
    ///     <ul>
    ///         <li><b>Email</b> (string) - email address,</li>
    ///         <li><b>Type</b> (EmployeeType) - employee type (All, RoomAdmin, User, DocSpaceAdmin, Collaborator).</li>
    ///     </ul>
    /// </ul>
    /// ]]></param>
    /// <returns>List of users</returns>
    /// <path>api/2.0/people/invite</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("invite")]
    public async Task<List<EmployeeDto>> InviteUsersAsync(InviteUsersRequestDto inDto)
    {
        foreach (var invite in inDto.Invitations)
        {
            if (!_permissionContext.CheckPermissions(new UserSecurityProvider(Guid.Empty, invite.Type), Constants.Action_AddRemoveUser))
            {
                continue;
            }

            var user = await _userManagerWrapper.AddInvitedUserAsync(invite.Email, invite.Type);
            var link = _invitationLinkService.GetInvitationLink(user.Email, invite.Type, _authContext.CurrentAccount.ID);

            _studioNotifyService.SendDocSpaceInvite(user.Email, link);
            _logger.Debug(link);
        }

        var result = new List<EmployeeDto>();

        var users = _userManager.GetUsers().Where(u => u.ActivationStatus == EmployeeActivationStatus.Pending);

        foreach (var user in users)
        {
            result.Add(await _employeeDtoHelper.Get(user));
        }

        return result;
    }

    /// <summary>
    /// Sets a new password to the user with the ID specified in the request.
    /// </summary>
    /// <short>Change a user password</short>
    /// <category>Password</category>
    /// <param type="System.Guid, System" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for setting new password: <![CDATA[
    /// <ul>
    ///     <li><b>Email</b> (string) - email address,</li>
    ///     <li><b>Password</b> (string) - password,</li>
    ///     <li><b>PasswordHash</b> (string) - password hash.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Detailed user information</returns>
    /// <path>api/2.0/people/{userid}/password</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}/password")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
    public async Task<EmployeeFullDto> ChangeUserPassword(Guid userid, MemberRequestDto inDto)
    {
        _apiContext.AuthByClaim();
        _permissionContext.DemandPermissions(new UserSecurityProvider(userid), Constants.Action_EditUser);

        var user = _userManager.GetUsers(userid);

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
            _securityContext.SetUserPasswordHash(userid, inDto.PasswordHash);
            _messageService.Send(MessageAction.UserUpdatedPassword);

            await _cookiesManager.ResetUserCookie(userid);
            _messageService.Send(MessageAction.CookieSettingsUpdated);
        }

        return await _employeeFullDtoHelper.GetFull(GetUserInfo(userid.ToString()));
    }

    /// <summary>
    /// Deletes a user with the ID specified in the request from the portal.
    /// </summary>
    /// <short>
    /// Delete a user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" name="userid">User ID</param>
    /// <returns>Deleted user detailed information</returns>
    /// <path>api/2.0/people/{userid}</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("{userid}")]
    public async Task<EmployeeFullDto> DeleteMember(string userid)
    {
        _permissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id) || user.IsLDAP())
        {
            throw new SecurityException();
        }

        if (user.Status != EmployeeStatus.Terminated)
        {
            throw new Exception("The user is not suspended");
        }

        CheckReassignProccess(new[] { user.Id });

        var userName = user.DisplayUserName(false, _displayUserSettingsHelper);
        await _userPhotoManager.RemovePhoto(user.Id);
        await _userManager.DeleteUser(user.Id);
        _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, false);

        _messageService.Send(MessageAction.UserDeleted, _messageTarget.Create(user.Id), userName);

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Deletes the current user profile.
    /// </summary>
    /// <short>
    /// Delete my profile
    /// </short>
    /// <category>Profiles</category>
    /// <returns>Detailed information about my profile</returns>
    /// <path>api/2.0/people/@self</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("@self")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "ProfileRemove")]
    public async Task<EmployeeFullDto> DeleteProfile()
    {
        _apiContext.AuthByClaim();

        if (_userManager.IsSystemUser(_securityContext.CurrentAccount.ID))
        {
            throw new SecurityException();
        }

        var user = GetUserInfo(_securityContext.CurrentAccount.ID.ToString());

        if (!_userManager.UserExists(user))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (user.IsLDAP())
        {
            throw new SecurityException();
        }

        _securityContext.AuthenticateMeWithoutCookie(Core.Configuration.Constants.CoreSystem);
        user.Status = EmployeeStatus.Terminated;

        _userManager.UpdateUserInfo(user);
        var userName = user.DisplayUserName(false, _displayUserSettingsHelper);
        _messageService.Send(MessageAction.UsersUpdatedStatus, _messageTarget.Create(user.Id), userName);

        await _cookiesManager.ResetUserCookie(user.Id);
        _messageService.Send(MessageAction.CookieSettingsUpdated);

        if (_coreBaseSettings.Personal)
        {
            await _userPhotoManager.RemovePhoto(user.Id);
            await _userManager.DeleteUser(user.Id);
            _messageService.Send(MessageAction.UserDeleted, _messageTarget.Create(user.Id), userName);
        }
        else
        {
            //StudioNotifyService.Instance.SendMsgProfileHasDeletedItself(user);
            //StudioNotifyService.SendMsgProfileDeletion(Tenant.TenantId, user);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Returns a list of users matching the status filter and search query.
    /// </summary>
    /// <short>
    /// Search users by status filter
    /// </short>
    /// <category>Search</category>
    /// <param type="ASC.Core.Users.EmployeeStatus, ASC.Core.Users" name="status">User status ("Active", "Terminated", "LeaveOfAbsence", "All", or "Default")</param>
    /// <param type="System.String, System" name="query">Search query</param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/status/{status}/search</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("status/{status}/search")]
    public async IAsyncEnumerable<EmployeeFullDto> GetAdvanced(EmployeeStatus status, [FromQuery] string query)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var list = _userManager.GetUsers(status).AsEnumerable();

        if ("group".Equals(_apiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            var groupId = new Guid(_apiContext.FilterValue);
            //Filter by group
            list = list.Where(x => _userManager.IsUserInGroup(x.Id, groupId));
            _apiContext.SetDataFiltered();
        }

        list = list.Where(x => x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1 || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                                (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.ContactsList != null && x.ContactsList.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)));

        foreach (var item in list)
        {
            yield return await _employeeFullDtoHelper.GetFull(item);
        }
    }

    /// <summary>
    /// Returns a list of profiles for all the portal users.
    /// </summary>
    /// <short>
    /// Get profiles
    /// </short>
    /// <category>Profiles</category>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people</path>
    /// <httpMethod>GET</httpMethod>
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
    /// <param type="System.String, System" name="email">User email address</param>
    /// <returns>Detailed profile information</returns>
    /// <path>api/2.0/people/email</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("email")]
    public async Task<EmployeeFullDto> GetByEmail([FromQuery] string email)
    {
        if (_coreBaseSettings.Personal && !_userManager.GetUsers(_securityContext.CurrentAccount.ID).IsOwner(Tenant))
        {
            throw new MethodAccessException("Method not available");
        }

        var user = _userManager.GetUserByEmail(email);
        if (user.Id == Constants.LostUser.Id)
        {
            throw new ItemNotFoundException("User not found");
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Returns the detailed information about a profile of the user with the name specified in the request.
    /// </summary>
    /// <short>
    /// Get a profile by user name
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" name="username">User name</param>
    /// <returns>Detailed profile information</returns>
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

        _apiContext.AuthByClaim();

        var user = _userManager.GetUserByUserName(username);
        if (user.Id == Constants.LostUser.Id)
        {
            if (Guid.TryParse(username, out var userId))
            {
                user = _userManager.GetUsers(userId);
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

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Returns a list of profiles filtered by user status.
    /// </summary>
    /// <short>
    /// Get profiles by status
    /// </short>
    /// <param type="ASC.Core.Users.EmployeeStatus, ASC.Core.Users" name="status">User status ("Active", "Terminated", "LeaveOfAbsence", "All", or "Default")</param>
    /// <returns>List of users with the detailed information</returns>
    /// <category>User status</category>
    /// <path>api/2.0/people/status/{status}</path>
    /// <httpMethod>GET</httpMethod>
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

        return GetFullByFilter(status, groupId, null, null, null, null);
    }

    /// <summary>
    /// Returns a list of users with full information about them matching the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Search users and their information by extended filter
    /// </short>
    /// <category>Search</category>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeStatus}, System" name="employeeStatus">User status ("Active", "Terminated", "LeaveOfAbsence", "All", or "Default")</param>
    /// <param type="System.Nullable{System.Guid}, System" name="groupId">Group ID</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeActivationStatus}, System" name="activationStatus">Activation status ("NotActivated", "Activated", "Pending", or "AutoGenerated")</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeType}, System" name="employeeType">User type ("All", "RoomAdmin", "User", "DocSpaceAdmin", or "Collaborator")</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="isAdministrator">Specifies if the user is an administrator or not</param>
    /// <param type="System.Nullable{ASC.Core.Payments}, System" name="payments">User payment status (Paid, Free)</param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/filter</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("filter")]
    public async IAsyncEnumerable<EmployeeFullDto> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator, Payments? payments)
    {
        var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator, payments);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

    /// <summary>
    /// Returns the information about the People module.
    /// </summary>
    /// <short>Get the People information</short>
    /// <category>Module</category>
    /// <returns>Module information: ID, product class name, title, description, icon URL, large icon URL, start URL, primary or nor, help URL</returns>
    /// <path>api/2.0/people/info</path>
    /// <httpMethod>GET</httpMethod>
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
    /// <returns>List of users</returns>
    /// <path>api/2.0/people/search</path>
    /// <httpMethod>GET</httpMethod>
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
    /// <param type="System.String, System" name="query">Search query</param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/@search/{query}</path>
    /// <httpMethod>GET</httpMethod>
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

        var users = _userManager.Search(query, EmployeeStatus.Active, groupId);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

    /// <summary>
    /// Returns a list of users matching the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Search users by extended filter
    /// </short>
    /// <category>Search</category>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeStatus}, System" name="employeeStatus">User status ("Active", "Terminated", "LeaveOfAbsence", "All", or "Default")</param>
    /// <param type="System.Nullable{System.Guid}, System" name="groupId">Group ID</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeActivationStatus}, System" name="activationStatus">Activation status ("NotActivated", "Activated", "Pending", or "AutoGenerated")</param>
    /// <param type="System.Nullable{ASC.Core.Users.EmployeeType}, System" name="employeeType">User type ("All", "RoomAdmin", "User", "DocSpaceAdmin", or "Collaborator")</param>
    /// <param type="System.Nullable{System.Boolean}, System" name="isAdministrator">Specifies if the user is an administrator or not</param>
    /// <param type="System.Nullable{ASC.Core.Payments}, System" name="payments">User payment status (Paid, Free)</param>
    /// <returns>List of users</returns>
    /// <path>api/2.0/people/simple/filter</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("simple/filter")]
    public async IAsyncEnumerable<EmployeeDto> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator, Payments? payments)
    {
        var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator, payments);

        foreach (var user in users)
        {
            yield return await _employeeDtoHelper.Get(user);
        }
    }

    /// <summary>
    /// Registers a user on the Personal portal.
    /// </summary>
    /// <short>
    /// Register a Personal account
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.RegisterPersonalUserRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for registering a Personal account: <![CDATA[
    /// <ul>
    ///     <li><b>Email</b> (string) - email address,</li>
    ///     <li><b>Lang</b> (string) - language,</li>
    ///     <li><b>Spam</b> (bool) - specifies if the user wants to subscribe to the ONLYOFFICE newsletter or not,</li>
    ///     <li><b>RecaptchaResponse</b> (string) - ReCAPTCHA response.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Error message or empty string</returns>
    /// <path>api/2.0/people/register</path>
    /// <httpMethod>POST</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [HttpPost("register")]
    public Task<string> RegisterUserOnPersonalAsync(RegisterPersonalUserRequestDto inDto)
    {
        if (!_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method is only available on personal.onlyoffice.com");
        }

        return InternalRegisterUserOnPersonalAsync(inDto);
    }

    /// <summary>
    /// Deletes a list of the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Delete users
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating portal users: <![CDATA[UserIds (IEnumerable&lt;Guid&gt;) - list of user IDs]]></param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/delete</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("delete", Order = -1)]
    public async IAsyncEnumerable<EmployeeFullDto> RemoveUsers(UpdateMembersRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

        CheckReassignProccess(inDto.UserIds);

        var users = inDto.UserIds.Select(userId => _userManager.GetUsers(userId))
            .Where(u => !_userManager.IsSystemUser(u.Id) && !u.IsLDAP())
            .ToList();

        var userNames = users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)).ToList();

        foreach (var user in users)
        {
            if (user.Status != EmployeeStatus.Terminated)
            {
                continue;
            }

            await _userPhotoManager.RemovePhoto(user.Id);
            await _userManager.DeleteUser(user.Id);
            _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, false);
        }

        _messageService.Send(MessageAction.UsersDeleted, _messageTarget.Create(users.Select(x => x.Id)), userNames);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

    /// <summary>
    /// Resends emails to the users who have not activated their emails.
    /// </summary>
    /// <short>
    /// Resend activation emails
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating portal users: <![CDATA[
    /// <ul>
    ///     <li><b>UserIds</b> (IEnumerable&lt;Guid&lt;) - list of user IDs,</li>
    ///     <li><b>ResendAll</b> (bool) - specifies whether to resend invitation letters to all the users or not.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/invite</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("invite")]
    public async IAsyncEnumerable<EmployeeFullDto> ResendUserInvites(UpdateMembersRequestDto inDto)
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(Guid.Empty, EmployeeType.User), Constants.Action_AddRemoveUser);

        IEnumerable<UserInfo> users = null;

        if (inDto.ResendAll)
        {
            users = _userManager.GetUsers().Where(u => u.ActivationStatus == EmployeeActivationStatus.Pending);
        }
        else
        {
            users = inDto.UserIds
                .Where(userId => !_userManager.IsSystemUser(userId))
                .Select(userId => _userManager.GetUsers(userId));
        }

        foreach (var user in users)
        {
            if (user.IsActive)
            {
                continue;
            }

            var viewer = _userManager.GetUsers(_securityContext.CurrentAccount.ID);

            if (viewer == null)
            {
                throw new Exception(Resource.ErrorAccessDenied);
            }

            if (_userManager.IsDocSpaceAdmin(viewer) || viewer.Id == user.Id)
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
                var type = _userManager.GetUserType(user.Id);

                if (!_permissionContext.CheckPermissions(new UserSecurityProvider(type), Constants.Action_AddRemoveUser))
                {
                    continue;
                }

                var link = _invitationLinkService.GetInvitationLink(user.Email, type, _authContext.CurrentAccount.ID);
                _studioNotifyService.SendDocSpaceInvite(user.Email, link);
            }
            else
            {
                _studioNotifyService.SendEmailActivationInstructions(user, user.Email);
            }
        }

        _messageService.Send(MessageAction.UsersSentActivationInstructions, _messageTarget.Create(users.Select(x => x.Id)), users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)));

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

    /// <summary>
    /// Returns a theme which is set to the current portal.
    /// </summary>
    /// <short>
    /// Get portal theme
    /// </short>
    /// <category>Theme</category>
    /// <returns>Theme</returns>
    /// <path>api/2.0/people/theme</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("theme")]
    public DarkThemeSettings GetTheme()
    {
        return _settingsManager.LoadForCurrentUser<DarkThemeSettings>();
    }

    /// <summary>
    /// Changes the current portal theme.
    /// </summary>
    /// <short>
    /// Change portal theme
    /// </short>
    /// <category>Theme</category>
    /// <param type="ASC.People.ApiModels.RequestDto.DarkThemeSettingsRequestDto, ASC.People.ApiModels.RequestDto" name="model">Theme settings request parameters: Theme (DarkThemeSettingsEnum) - portal theme (Base, System, or Dark)</param>
    /// <returns>Theme</returns>
    /// <path>api/2.0/people/theme</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("theme")]
    public DarkThemeSettings ChangeTheme(DarkThemeSettingsRequestDto model)
    {
        var darkThemeSettings = new DarkThemeSettings
        {
            Theme = model.Theme
        };

        _settingsManager.SaveForCurrentUser(darkThemeSettings);

        return darkThemeSettings;
    }

    /// <summary>
    /// Returns the detailed information about the current user profile.
    /// </summary>
    /// <short>
    /// Get my profile
    /// </short>
    /// <category>Profiles</category>
    /// <returns>Detailed information about my profile</returns>
    /// <path>api/2.0/people/@self</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("@self")]
    public async Task<EmployeeFullDto> Self()
    {
        var user = _userManager.GetUser(_securityContext.CurrentAccount.ID, EmployeeFullDtoHelper.GetExpression(_apiContext));

        var result = await _employeeFullDtoHelper.GetFull(user);

        result.Theme = _settingsManager.LoadForCurrentUser<DarkThemeSettings>().Theme;

        return result;
    }

    /// <summary>
    /// Sends a message to the user email with the instructions to change the email address connected to the portal.
    /// </summary>
    /// <short>
    /// Send instructions to change email
    /// </short>
    /// <category>Profiles</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating user information: <![CDATA[
    /// <ul>
    ///     <li><b>UserId</b> (string) - user ID,</li>
    ///     <li><b>Email</b> (string) - email address.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Message text</returns>
    /// <path>api/2.0/people/email</path>
    /// <httpMethod>POST</httpMethod>
    [AllowNotPayment]
    [HttpPost("email")]
    public async Task<object> SendEmailChangeInstructions(UpdateMemberRequestDto inDto)
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

        var viewer = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        var user = _userManager.GetUsers(userid);

        if (user == null)
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (viewer == null || (user.IsOwner(Tenant) && viewer.Id != user.Id))
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        var existentUser = _userManager.GetUserByEmail(email);

        if (existentUser.Id != Constants.LostUser.Id)
        {
            throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
        }

        if (!_userManager.IsDocSpaceAdmin(viewer))
        {
            _studioNotifyService.SendEmailChangeInstructions(user, email);
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
            _studioNotifyService.SendEmailActivationInstructions(user, email);
        }

        _messageService.Send(MessageAction.UserSentEmailChangeInstructions, user.DisplayUserName(false, _displayUserSettingsHelper));

        return string.Format(Resource.MessageEmailChangeInstuctionsSentOnEmail, email);
    }

    /// <summary>
    /// Reminds a password to the user using the email address specified in the request.
    /// </summary>
    /// <short>
    /// Remind a user password
    /// </short>
    /// <category>Password</category>
    /// <param type="ASC.People.ApiModels.RequestDto.MemberRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Member request parameters: Email (string) - user email</param>
    /// <returns>Email with the password</returns>
    /// <path>api/2.0/people/password</path>
    /// <httpMethod>POST</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowNotPayment]
    [AllowAnonymous]
    [HttpPost("password")]
    public object SendUserPassword(MemberRequestDto inDto)
    {
        var error = _userManagerWrapper.SendUserPassword(inDto.Email);
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
    /// <param type="ASC.Core.Users.EmployeeActivationStatus, ASC.Core.Users" name="activationstatus">Activation status ("NotActivated", "Activated", "Pending", or "AutoGenerated")</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating user information: <![CDATA[UserIds (IEnumerable&lt;Guid&gt;) - list of user IDs]]></param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/activationstatus/{activationstatus}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("activationstatus/{activationstatus}")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateEmployeeActivationStatus(EmployeeActivationStatus activationstatus, UpdateMembersRequestDto inDto)
    {
        _apiContext.AuthByClaim();

        foreach (var id in inDto.UserIds.Where(userId => !_userManager.IsSystemUser(userId)))
        {
            _permissionContext.DemandPermissions(new UserSecurityProvider(id), Constants.Action_EditUser);
            var u = _userManager.GetUsers(id);
            if (u.Id == Constants.LostUser.Id || u.IsLDAP())
            {
                continue;
            }

            u.ActivationStatus = activationstatus;
            _userManager.UpdateUserInfo(u);
            yield return await _employeeFullDtoHelper.GetFull(u);
        }
    }

    /// <summary>
    /// Updates the user language with the parameter specified in the request.
    /// </summary>
    /// <short>
    /// Update user language
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating user information: CultureName (string) - language</param>
    /// <returns>Detailed user information</returns>
    /// <path>api/2.0/people/{userid}/culture</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}/culture")]
    public async Task<EmployeeFullDto> UpdateMemberCulture(string userid, UpdateMemberRequestDto inDto)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        _permissionContext.DemandPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        var curLng = user.CultureName;

        if (_setupInfo.EnabledCultures.Find(c => string.Equals(c.Name, inDto.CultureName, StringComparison.InvariantCultureIgnoreCase)) != null)
        {
            if (curLng != inDto.CultureName)
            {
                user.CultureName = inDto.CultureName;

                try
                {
                    _userManager.UpdateUserInfo(user);
                }
                catch
                {
                    user.CultureName = curLng;
                    throw;
                }

                _messageService.Send(MessageAction.UserUpdatedLanguage, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

            }
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Updates the data for the selected portal user with the first name, last name, email address, and/or optional parameters specified in the request.
    /// </summary>
    /// <short>
    /// Update a user
    /// </short>
    /// <category>Profiles</category>
    /// <param type="System.String, System" name="userid">User ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMemberRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Member request parameters: <![CDATA[
    /// <ul>
    ///     <li><b>IsUser</b> (bool) - specifies if this is a guest or user,</li>
    ///     <li><b>Firstname</b> (string) - first name,</li>
    ///     <li><b>Lastname</b> (string) - last name,</li>
    ///     <li><b>Department</b> (Guid[]) - list of user departments,</li>
    ///     <li><b>Title</b> (string) - title,</li>
    ///     <li><b>Location</b> (string) - location,</li>
    ///     <li><b>Sex</b> (string) - sex (male or female),</li>
    ///     <li><b>Birthday</b> (ApiDateTime) - birthday,</li>
    ///     <li><b>Worksfrom</b> (ApiDateTime) - registration date (if it is not specified, then the current date will be set),</li>
    ///     <li><b>Comment</b> (string) - comment,</li>
    ///     <li><b>Contacts</b> (IEnumerable&lt;Contact&gt;) - list of user contacts,</li>
    ///     <li><b>Files</b> (string) - avatar photo URL,</li>
    ///     <li><b>Disable</b> (bool?) - specifies whether to disable a user or not.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Updated user with the detailed information</returns>
    /// <path>api/2.0/people/{userid}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{userid}", Order = 1)]
    public async Task<EmployeeFullDto> UpdateMember(string userid, UpdateMemberRequestDto inDto)
    {
        var user = GetUserInfo(userid);

        if (_userManager.IsSystemUser(user.Id))
        {
            throw new SecurityException();
        }

        _permissionContext.DemandPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);
        var self = _securityContext.CurrentAccount.ID.Equals(user.Id);
        var resetDate = new DateTime(1900, 01, 01);

        //Update it

        var isLdap = user.IsLDAP();
        var isSso = user.IsSSO();
        var isDocSpaceAdmin = _webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, _securityContext.CurrentAccount.ID);

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
        UpdateContacts(inDto.Contacts, user);
        await UpdateDepartments(inDto.Department, user);

        if (inDto.Files != await _userPhotoManager.GetPhotoAbsoluteWebPath(user.Id))
        {
            await UpdatePhotoUrl(inDto.Files, user);
        }
        if (inDto.Disable.HasValue)
        {
            user.Status = inDto.Disable.Value ? EmployeeStatus.Terminated : EmployeeStatus.Active;
            user.TerminatedDate = inDto.Disable.Value ? DateTime.UtcNow : null;
        }
        if (self && !isDocSpaceAdmin)
        {
            _studioNotifyService.SendMsgToAdminAboutProfileUpdated();
        }

        // change user type
        var canBeGuestFlag = !user.IsOwner(Tenant) && !_userManager.IsDocSpaceAdmin(user) && user.GetListAdminModules(_webItemSecurity, _webItemManager).Count == 0 && !user.IsMe(_authContext);

        if (inDto.IsUser && !_userManager.IsUser(user) && canBeGuestFlag)
        {
            await _countUserChecker.CheckAppend();
            await _userManager.AddUserIntoGroup(user.Id, Constants.GroupUser.ID);
            _webItemSecurityCache.ClearCache(Tenant.Id);
        }

        if (!self && !inDto.IsUser && _userManager.IsUser(user))
        {
            await _countPaidUserChecker.CheckAppend();
            _userManager.RemoveUserFromGroup(user.Id, Constants.GroupUser.ID);
            _webItemSecurityCache.ClearCache(Tenant.Id);
        }

        await _userManager.UpdateUserInfoWithSyncCardDavAsync(user);

        _messageService.Send(MessageAction.UserUpdated, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper), user.Id);

        if (inDto.Disable.HasValue && inDto.Disable.Value)
        {
            await _cookiesManager.ResetUserCookie(user.Id);
            _messageService.Send(MessageAction.CookieSettingsUpdated);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

    /// <summary>
    /// Changes a status for the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Change a user status
    /// </short>
    /// <category>User status</category>
    /// <param type="ASC.Core.Users.EmployeeStatus, ASC.Core.Users" name="status">New user status ("Active", "Terminated", "LeaveOfAbsence", "All", or "Default"</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating user information: <![CDATA[UserIds (IEnumerable&lt;Guid&gt;) - list of user IDs]]></param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/status/{status}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("status/{status}")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateUserStatus(EmployeeStatus status, UpdateMembersRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        var users = inDto.UserIds.Select(userId => _userManager.GetUsers(userId))
            .Where(u => !_userManager.IsSystemUser(u.Id) && !u.IsLDAP())
            .ToList();

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
                        if (!_userManager.IsUser(user))
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

                    await _cookiesManager.ResetUserCookie(user.Id);
                    _messageService.Send(MessageAction.CookieSettingsUpdated);
                    break;
            }
        }

        _messageService.Send(MessageAction.UsersUpdatedStatus, _messageTarget.Create(users.Select(x => x.Id)), users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)));

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

    /// <summary>
    /// Changes a type for the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Change a user type
    /// </short>
    /// <category>User type</category>
    /// <param type="ASC.Core.Users.EmployeeType, ASC.Core.Users" name="type">New user type ("All", "RoomAdmin", "User", "DocSpaceAdmin", or "Collaborator")</param>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating user information: <![CDATA[UserIds (IEnumerable&lt;Guid&gt;) - list of user IDs]]></param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/type/{type}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("type/{type}")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateUserTypeAsync(EmployeeType type, UpdateMembersRequestDto inDto)
    {
        _permissionContext.DemandPermissions(new UserSecurityProvider(type), Constants.Action_AddRemoveUser);

        var users = inDto.UserIds
            .Where(userId => !_userManager.IsSystemUser(userId))
            .Select(userId => _userManager.GetUsers(userId))
            .ToList();

        var updatedUsers = new List<UserInfo>(users.Count);

        foreach (var user in users)
        {
            if (await _userManagerWrapper.UpdateUserTypeAsync(user, type))
            {
                updatedUsers.Add(user);
            }
                }
        
            _messageService.Send(MessageAction.UsersUpdatedType, _messageTarget.CreateFromGroupValues(users.Select(x => x.Id.ToString())),
            users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)), users.Select(x => x.Id).ToList(), type);
        
        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
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
    public void RecalculateQuota()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        _usersQuotaSyncOperation.RecalculateQuota(_tenantManager.GetCurrentTenant());
    }

    /// <summary>
    /// Checks the process of recalculating quota.
    /// </summary>
    /// <short>
    /// Check quota recalculation
    /// </short>
    /// <category>Quota</category>
    /// <returns>Task progress</returns>
    /// <path>api/2.0/people/checkrecalculatequota</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("checkrecalculatequota")]
    public TaskProgressDto CheckRecalculateQuota()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        return _usersQuotaSyncOperation.CheckRecalculateQuota(_tenantManager.GetCurrentTenant());
    }

    /// <summary>
    /// Changes a quota limit for the users with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Change a user quota limit
    /// </short>
    /// <category>Quota</category>
    /// <param type="ASC.People.ApiModels.RequestDto.UpdateMembersRequestDto, ASC.People.ApiModels.RequestDto" name="inDto">Request parameters for updating user information: <![CDATA[
    /// <ul>
    ///     <li><b>UserIds</b> (IEnumerable&lt;Guid&gt;) - list of user IDs,</li>
    ///     <li><b>Quota</b> (long) - user quota.</li>
    /// </ul>
    /// ]]></param>
    /// <returns>List of users with the detailed information</returns>
    /// <path>api/2.0/people/quota</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("quota")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateUserQuota(UpdateMembersQuotaRequestDto inDto)
    {
        var users = inDto.UserIds
            .Where(userId => !_userManager.IsSystemUser(userId))
            .Select(userId => _userManager.GetUsers(userId))
            .ToList();

        foreach (var user in users)
        {
            if (inDto.Quota != -1)
            {
                var usedSpace = Math.Max(0,
                    _quotaService.FindUserQuotaRows(
                            _tenantManager.GetCurrentTenant().Id,
                            user.Id
                        )
                .Where(r => !string.IsNullOrEmpty(r.Tag)).Sum(r => r.Counter));

                var tenanSpaceQuota = _quotaService.GetTenantQuota(Tenant.Id).MaxTotalSize;

                if (tenanSpaceQuota < inDto.Quota || usedSpace > inDto.Quota)
                {
                    continue;
                }
            }

            var quotaSettings = _settingsManager.Load<TenantUserQuotaSettings>();

            _settingsManager.Save(new UserQuotaSettings { UserQuota = inDto.Quota }, user);

            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }


    private async Task UpdateDepartments(IEnumerable<Guid> department, UserInfo user)
    {
        if (!_permissionContext.CheckPermissions(Constants.Action_EditGroups))
        {
            return;
        }

        if (department == null)
        {
            return;
        }

        var groups = _userManager.GetUserGroups(user.Id);
        var managerGroups = new List<Guid>();
        foreach (var groupInfo in groups)
        {
            _userManager.RemoveUserFromGroup(user.Id, groupInfo.ID);
            var managerId = _userManager.GetDepartmentManager(groupInfo.ID);
            if (managerId == user.Id)
            {
                managerGroups.Add(groupInfo.ID);
                _userManager.SetDepartmentManager(groupInfo.ID, Guid.Empty);
            }
        }
        foreach (var guid in department)
        {
            var userDepartment = _userManager.GetGroupInfo(guid);
            if (userDepartment != Constants.LostGroupInfo)
            {
                await _userManager.AddUserIntoGroup(user.Id, guid);
                if (managerGroups.Contains(guid))
                {
                    _userManager.SetDepartmentManager(guid, user.Id);
                }
            }
        }
    }

    private void CheckReassignProccess(IEnumerable<Guid> userIds)
    {
        foreach (var userId in userIds)
        {
            var reassignStatus = _queueWorkerReassign.GetProgressItemStatus(Tenant.Id, userId);
            if (reassignStatus == null || reassignStatus.IsCompleted)
            {
                continue;
            }

            var userName = _userManager.GetUsers(userId).DisplayUserName(_displayUserSettingsHelper);

            throw new Exception(string.Format(Resource.ReassignDataRemoveUserError, userName));
        }
    }

    private async Task<string> InternalRegisterUserOnPersonalAsync(RegisterPersonalUserRequestDto inDto)
    {
        try
        {
            if (_coreBaseSettings.CustomMode)
            {
                inDto.Lang = "ru-RU";
            }

            var cultureInfo = _setupInfo.GetPersonalCulture(inDto.Lang).Value;

            if (cultureInfo != null)
            {
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }

            inDto.Email.ThrowIfNull(new ArgumentException(Resource.ErrorEmailEmpty, "email"));

            if (!inDto.Email.TestEmailRegex())
            {
                throw new ArgumentException(Resource.ErrorNotCorrectEmail, "email");
            }

            if (!SetupInfo.IsSecretEmail(inDto.Email)
                && !string.IsNullOrEmpty(_setupInfo.RecaptchaPublicKey) && !string.IsNullOrEmpty(_setupInfo.RecaptchaPrivateKey))
            {
                var ip = Request.Headers["X-Forwarded-For"].ToString() ?? Request.GetUserHostAddress();

                if (string.IsNullOrEmpty(inDto.RecaptchaResponse)
                    || !await _recaptcha.ValidateRecaptchaAsync(inDto.RecaptchaResponse, ip))
                {
                    throw new RecaptchaException(Resource.RecaptchaInvalid);
                }
            }

            var newUserInfo = _userManager.GetUserByEmail(inDto.Email);

            if (_userManager.UserExists(newUserInfo.Id))
            {
                if (!SetupInfo.IsSecretEmail(inDto.Email) || _securityContext.IsAuthenticated)
                {
                    _studioNotifyService.SendAlreadyExist(inDto.Email);
                    return string.Empty;
                }

                try
                {
                    _securityContext.AuthenticateMe(Core.Configuration.Constants.CoreSystem);
                    await _userManager.DeleteUser(newUserInfo.Id);
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

            _studioNotifyService.SendInvitePersonal(inDto.Email);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return string.Empty;
    }

    private IQueryable<UserInfo> GetByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isDocSpaceAdministrator, Payments? payments)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var isDocSpaceAdmin = _userManager.IsDocSpaceAdmin(_securityContext.CurrentAccount.ID) ||
                      _webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, _securityContext.CurrentAccount.ID);

        var includeGroups = new List<List<Guid>>();
        if (groupId.HasValue)
        {
            includeGroups.Add(new List<Guid> { groupId.Value });
        }

        var excludeGroups = new List<Guid>();

        if (employeeType != null)
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

        var users = _userManager.GetUsers(isDocSpaceAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, _apiContext.FilterValue, _apiContext.SortBy, !_apiContext.SortDescending, _apiContext.Count, _apiContext.StartIndex, out var total, out var count);

        _apiContext.SetTotalCount(total).SetCount(count);

        return users;
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
    // <returns>Current user information</returns>
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
