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

using Module = ASC.Api.Core.Module;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

public class UserController : PeopleControllerBase
{
    private Tenant Tenant => _apiContext.Tenant;

    private readonly ICache _cache;
    private readonly TenantManager _tenantManager;
    private readonly GlobalSpace _globalSpace;
    private readonly Constants _constants;
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
    private readonly TenantExtra _tenantExtra;
    private readonly TenantStatisticsProvider _tenantStatisticsProvider;
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
    private readonly RoomLinkService _roomLinkService;
    private readonly FileSecurity _fileSecurity;
    private readonly IQuotaService _quotaService;

    public UserController(
        ICache cache,
        TenantManager tenantManager,
        GlobalSpace globalSpace,
        Constants constants,
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
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticsProvider,
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
        RoomLinkService roomLinkService,
        FileSecurity fileSecurity,
        IQuotaService quotaService)
        : base(userManager, permissionContext, apiContext, userPhotoManager, httpClientFactory, httpContextAccessor)
    {
        _cache = cache;
        _tenantManager = tenantManager;
        _globalSpace = globalSpace;
        _constants = constants;
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
        _tenantExtra = tenantExtra;
        _tenantStatisticsProvider = tenantStatisticsProvider;
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
        _roomLinkService = roomLinkService;
        _fileSecurity = fileSecurity;
        _quotaService = quotaService;
    }

    [HttpPost("active")]
    public async Task<EmployeeFullDto> AddMemberAsActivated(MemberRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

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
                       : ("female".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);

        user.BirthDate = inDto.Birthday != null ? _tenantUtil.DateTimeFromUtc(inDto.Birthday) : null;
        user.WorkFromDate = inDto.Worksfrom != null ? _tenantUtil.DateTimeFromUtc(inDto.Worksfrom) : DateTime.UtcNow.Date;

        UpdateContacts(inDto.Contacts, user);

        _cache.Insert("REWRITE_URL" + _tenantManager.GetCurrentTenant().Id, HttpContext.Request.GetUrlRewriter().ToString(), TimeSpan.FromMinutes(5));
        user = _userManagerWrapper.AddUser(user, inDto.PasswordHash, false, false, inDto.IsVisitor, false, true, true);

        user.ActivationStatus = EmployeeActivationStatus.Activated;

        UpdateDepartments(inDto.Department, user);

        if (inDto.Files != _userPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            await UpdatePhotoUrl(inDto.Files, user);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    public async Task<EmployeeFullDto> AddMember(MemberRequestDto inDto)
    {
        _apiContext.AuthByClaim();

        _permissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

        var options = inDto.FromInviteLink ? await _roomLinkService.GetOptionsAsync(inDto.Key, inDto.Email) : null;

        if (options != null && !options.IsCorrect)
        {
            throw new SecurityException(FilesCommonResource.ErrorMessage_InvintationLink);
        }

        var user = new UserInfo();

        var byEmail = options != null && options.Type == LinkType.InvintationByEmail;

        if (byEmail)
        {
            user = _userManager.GetUserByEmail(inDto.Email);

            if (user == Constants.LostUser || user.ActivationStatus != EmployeeActivationStatus.Pending)
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
                       : ("female".Equals(inDto.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);

        user.BirthDate = inDto.Birthday != null && inDto.Birthday != DateTime.MinValue ? _tenantUtil.DateTimeFromUtc(inDto.Birthday) : null;
        user.WorkFromDate = inDto.Worksfrom != null && inDto.Worksfrom != DateTime.MinValue ? _tenantUtil.DateTimeFromUtc(inDto.Worksfrom) : DateTime.UtcNow.Date;

        UpdateContacts(inDto.Contacts, user);
        _cache.Insert("REWRITE_URL" + _tenantManager.GetCurrentTenant().Id, HttpContext.Request.GetUrlRewriter().ToString(), TimeSpan.FromMinutes(5));
        user = _userManagerWrapper.AddUser(user, inDto.PasswordHash, inDto.FromInviteLink, true, inDto.IsVisitor, inDto.FromInviteLink, true, true, byEmail);

        UpdateDepartments(inDto.Department, user);

        if (inDto.Files != _userPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            await UpdatePhotoUrl(inDto.Files, user);
        }

        if (options != null && options.Type == LinkType.InvintationToRoom)
        {
            var success = int.TryParse(options.RoomId, out var id);

            if (success)
            {
                await _fileSecurity.ShareAsync(id, Files.Core.FileEntryType.Folder, user.Id, options.Share);
            }
            else
            {
                await _fileSecurity.ShareAsync(options.RoomId, Files.Core.FileEntryType.Folder, user.Id, options.Share);
            }
        }

        var messageAction = inDto.IsVisitor ? MessageAction.GuestCreated : MessageAction.UserCreated;
        _messageService.Send(messageAction, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        return await _employeeFullDtoHelper.GetFull(user);
    }

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
                _userManager.SaveUserInfo(user, syncCardDav: true);
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
        _userPhotoManager.RemovePhoto(user.Id);
        _userManager.DeleteUser(user.Id);
        _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, false);

        _messageService.Send(MessageAction.UserDeleted, _messageTarget.Create(user.Id), userName);

        return await _employeeFullDtoHelper.GetFull(user);
    }

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

        _userManager.SaveUserInfo(user);
        var userName = user.DisplayUserName(false, _displayUserSettingsHelper);
        _messageService.Send(MessageAction.UsersUpdatedStatus, _messageTarget.Create(user.Id), userName);

        await _cookiesManager.ResetUserCookie(user.Id);
        _messageService.Send(MessageAction.CookieSettingsUpdated);

        if (_coreBaseSettings.Personal)
        {
            _userPhotoManager.RemovePhoto(user.Id);
            _userManager.DeleteUser(user.Id);
            _messageService.Send(MessageAction.UserDeleted, _messageTarget.Create(user.Id), userName);
        }
        else
        {
            //StudioNotifyService.Instance.SendMsgProfileHasDeletedItself(user);
            //StudioNotifyService.SendMsgProfileDeletion(Tenant.TenantId, user);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

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

    [HttpGet]
    public IAsyncEnumerable<EmployeeFullDto> GetAll()
    {
        return GetByStatus(EmployeeStatus.Active);
    }

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
            return _employeeFullDtoHelper.GetSimple(user);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

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

        return GetFullByFilter(status, groupId, null, null, null);
    }

    [HttpGet("filter")]
    public async IAsyncEnumerable<EmployeeFullDto> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

    [HttpGet("info")]
    public Module GetModule()
    {
        var product = new PeopleProduct();
        product.Init();

        return new Module(product);
    }

    [HttpGet("search")]
    public IAsyncEnumerable<EmployeeDto> GetPeopleSearch([FromQuery] string query)
    {
        return GetSearch(query);
    }

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

    [HttpGet("simple/filter")]
    public async IAsyncEnumerable<EmployeeDto> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

        foreach (var user in users)
        {
            yield return await _employeeDtoHelper.Get(user);
        }
    }

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

            _userPhotoManager.RemovePhoto(user.Id);
            _userManager.DeleteUser(user.Id);
            _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, false);
        }

        _messageService.Send(MessageAction.UsersDeleted, _messageTarget.Create(users.Select(x => x.Id)), userNames);

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

    [HttpPut("invite")]
    public async IAsyncEnumerable<EmployeeFullDto> ResendUserInvites(UpdateMembersRequestDto inDto)
    {
        var users = inDto.UserIds
             .Where(userId => !_userManager.IsSystemUser(userId))
             .Select(userId => _userManager.GetUsers(userId))
             .ToList();

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

            if (_userManager.IsAdmin(viewer) || viewer.Id == user.Id)
            {
                if (user.ActivationStatus == EmployeeActivationStatus.Activated)
                {
                    user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                }
                if (user.ActivationStatus == (EmployeeActivationStatus.AutoGenerated | EmployeeActivationStatus.Activated))
                {
                    user.ActivationStatus = EmployeeActivationStatus.AutoGenerated;
                }

                _userManager.SaveUserInfo(user, syncCardDav: true);
            }

            if (user.ActivationStatus == EmployeeActivationStatus.Pending)
            {
                if (_userManager.IsVisitor(user))
                {
                    _studioNotifyService.GuestInfoActivation(user);
                }
                else
                {
                    _studioNotifyService.UserInfoActivation(user);
                }
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

    [HttpGet("theme")]
    public DarkThemeSettings GetTheme()
    {
        return _settingsManager.LoadForCurrentUser<DarkThemeSettings>();
    }

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

    [HttpGet("@self")]
    public async Task<EmployeeFullDto> Self()
    {
        var user = _userManager.GetUser(_securityContext.CurrentAccount.ID, EmployeeFullDtoHelper.GetExpression(_apiContext));

        var result = await _employeeFullDtoHelper.GetFull(user);

        result.Theme = _settingsManager.LoadForCurrentUser<DarkThemeSettings>().Theme;

        return result;
    }

    [AllowNotPayment]
    [HttpPost("email")]
    public object SendEmailChangeInstructions(UpdateMemberRequestDto inDto)
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

        if (!_userManager.IsAdmin(viewer))
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
            _userManager.SaveUserInfo(user, syncCardDav: true);
            _studioNotifyService.SendEmailActivationInstructions(user, email);
        }

        _messageService.Send(MessageAction.UserSentEmailChangeInstructions, user.DisplayUserName(false, _displayUserSettingsHelper));

        return string.Format(Resource.MessageEmailChangeInstuctionsSentOnEmail, email);
    }

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
            _userManager.SaveUserInfo(u);
            yield return await _employeeFullDtoHelper.GetFull(u);
        }
    }

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
                    _userManager.SaveUserInfo(user);
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
        var isAdmin = _webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, _securityContext.CurrentAccount.ID);

        if (!isLdap && !isSso)
        {
            //Set common fields

            user.FirstName = inDto.Firstname ?? user.FirstName;
            user.LastName = inDto.Lastname ?? user.LastName;
            user.Location = inDto.Location ?? user.Location;

            if (isAdmin)
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
        UpdateDepartments(inDto.Department, user);

        if (inDto.Files != await _userPhotoManager.GetPhotoAbsoluteWebPath(user.Id))
        {
            await UpdatePhotoUrl(inDto.Files, user);
        }
        if (inDto.Disable.HasValue)
        {
            user.Status = inDto.Disable.Value ? EmployeeStatus.Terminated : EmployeeStatus.Active;
            user.TerminatedDate = inDto.Disable.Value ? DateTime.UtcNow : null;
        }
        if (self && !isAdmin)
        {
            _studioNotifyService.SendMsgToAdminAboutProfileUpdated();
        }

        // change user type
        var canBeGuestFlag = !user.IsOwner(Tenant) && !_userManager.IsAdmin(user) && user.GetListAdminModules(_webItemSecurity, _webItemManager).Count == 0 && !user.IsMe(_authContext);

        if (inDto.IsVisitor && !_userManager.IsVisitor(user) && canBeGuestFlag)
        {
            _userManager.AddUserIntoGroup(user.Id, Constants.GroupVisitor.ID);
            _webItemSecurityCache.ClearCache(Tenant.Id);
        }

        if (!self && !inDto.IsVisitor && _userManager.IsVisitor(user))
        {
            var usersQuota = _tenantExtra.GetTenantQuota().ActiveUsers;
            if (_tenantStatisticsProvider.GetUsersCount() < usersQuota)
            {
                _userManager.RemoveUserFromGroup(user.Id, Constants.GroupVisitor.ID);
                _webItemSecurityCache.ClearCache(Tenant.Id);
            }
            else
            {
                throw new TenantQuotaException(string.Format("Exceeds the maximum active users ({0})", usersQuota));
            }
        }

        _userManager.SaveUserInfo(user, inDto.IsVisitor, true);
        _messageService.Send(MessageAction.UserUpdated, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper));

        if (inDto.Disable.HasValue && inDto.Disable.Value)
        {
            await _cookiesManager.ResetUserCookie(user.Id);
            _messageService.Send(MessageAction.CookieSettingsUpdated);
        }

        return await _employeeFullDtoHelper.GetFull(user);
    }

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
                        if (_tenantStatisticsProvider.GetUsersCount() < _tenantExtra.GetTenantQuota().ActiveUsers || _userManager.IsVisitor(user))
                        {
                            user.Status = EmployeeStatus.Active;
                            _userManager.SaveUserInfo(user, syncCardDav: true);
                        }
                    }
                    break;
                case EmployeeStatus.Terminated:
                    user.Status = EmployeeStatus.Terminated;
                    _userManager.SaveUserInfo(user, syncCardDav: true);

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

    [HttpPut("type/{type}")]
    public async IAsyncEnumerable<EmployeeFullDto> UpdateUserType(EmployeeType type, UpdateMembersRequestDto inDto)
    {
        var users = inDto.UserIds
            .Where(userId => !_userManager.IsSystemUser(userId))
            .Select(userId => _userManager.GetUsers(userId))
            .ToList();

        foreach (var user in users)
        {
            if (user.IsOwner(Tenant) || _userManager.IsAdmin(user)
                || user.IsMe(_authContext) || user.GetListAdminModules(_webItemSecurity, _webItemManager).Count > 0)
            {
                continue;
            }

            switch (type)
            {
                case EmployeeType.User:
                    if (_userManager.IsVisitor(user))
                    {
                        if (_tenantStatisticsProvider.GetUsersCount() < _tenantExtra.GetTenantQuota().ActiveUsers)
                        {
                            _userManager.RemoveUserFromGroup(user.Id, Constants.GroupVisitor.ID);
                            _webItemSecurityCache.ClearCache(Tenant.Id);
                        }
                    }
                    break;
                case EmployeeType.Visitor:
                    if (_coreBaseSettings.Standalone || _tenantStatisticsProvider.GetVisitorsCount() < _tenantExtra.GetTenantQuota().ActiveUsers * _constants.CoefficientOfVisitors)
                    {
                        _userManager.AddUserIntoGroup(user.Id, Constants.GroupVisitor.ID);
                        _webItemSecurityCache.ClearCache(Tenant.Id);
                    }
                    break;
            }
        }

        _messageService.Send(MessageAction.UsersUpdatedType, _messageTarget.Create(users.Select(x => x.Id)), users.Select(x => x.DisplayUserName(false, _displayUserSettingsHelper)));

        foreach (var user in users)
        {
            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }

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

                var tenanSpaceQuota = _tenantExtra.GetTenantQuota().MaxTotalSize;

                if (tenanSpaceQuota < inDto.Quota || usedSpace > inDto.Quota)
                {
                    continue;
                }
            }

            var quotaSettings = _settingsManager.Load<TenantUserQuotaSettings>();

            _settingsManager.SaveForUser(new UserQuotaSettings { UserQuota = inDto.Quota }, user);

            yield return await _employeeFullDtoHelper.GetFull(user);
        }
    }


    private void UpdateDepartments(IEnumerable<Guid> department, UserInfo user)
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
                _userManager.AddUserIntoGroup(user.Id, guid);
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
                    _userManager.DeleteUser(newUserInfo.Id);
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

    private IQueryable<UserInfo> GetByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var isAdmin = _userManager.IsAdmin(_securityContext.CurrentAccount.ID) ||
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
                case EmployeeType.User:
                    excludeGroups.Add(Constants.GroupVisitor.ID);
                    break;
                case EmployeeType.Visitor:
                    includeGroups.Add(new List<Guid> { Constants.GroupVisitor.ID });
                    break;
            }
        }

        if (isAdministrator.HasValue && isAdministrator.Value)
        {
            var adminGroups = new List<Guid>
            {
                    Constants.GroupAdmin.ID
            };
            var products = _webItemManager.GetItemsAll().Where(i => i is IProduct || i.ID == WebItemManager.MailProductID);
            adminGroups.AddRange(products.Select(r => r.ID));

            includeGroups.Add(adminGroups);
        }

        var users = _userManager.GetUsers(isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, _apiContext.FilterValue, _apiContext.SortBy, !_apiContext.SortDescending, _apiContext.Count, _apiContext.StartIndex, out var total, out var count);

        _apiContext.SetTotalCount(total).SetCount(count);

        return users;
    }

    ///// <summary>
    ///// Adds a new portal user from import with the first and last name, email address
    ///// </summary>
    ///// <short>
    ///// Add new import user
    ///// </short>
    ///// <param name="userList">The list of users to add</param>
    ///// <param name="importUsersAsCollaborators" optional="true">Add users as guests (bool type: false|true)</param>
    ///// <returns>Newly created users</returns>
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
