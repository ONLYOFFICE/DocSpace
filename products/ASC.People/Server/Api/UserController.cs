using SecurityContext = ASC.Core.SecurityContext;
using Module = ASC.Api.Core.Module;

namespace ASC.People.Api;

public class UserController : BasePeopleController
{
    private readonly Constants _constants;
    private readonly CookiesManager _cookiesManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly ILog _logger;
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
    private readonly EmployeeFullDtoHelper _employeeFullDtoHelper;
    private readonly EmployeeDtoHelper _employeeDtoHelper;

    public UserController(
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        MessageService messageService,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService,
        Constants constants,
        CookiesManager cookiesManager,
        CoreBaseSettings coreBaseSettings,
        CustomNamingPeople customNamingPeople,
        ILog logger,
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
        UserPhotoManager userPhotoManager,
        IHttpClientFactory httpClientFactory,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        SetupInfo setupInfo,
        EmployeeFullDtoHelper employeeFullDtoHelper,
        EmployeeDtoHelper employeeDtoHelper)
        : base(
            userManager,
            authContext,
            apiContext,
            permissionContext,
            securityContext,
            messageService,
            messageTarget,
            studioNotifyService,
            userPhotoManager,
            httpClientFactory,
            displayUserSettingsHelper,
            setupInfo)
    {
        _constants = constants;
        _cookiesManager = cookiesManager;
        _coreBaseSettings = coreBaseSettings;
        _customNamingPeople = customNamingPeople;
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
        _employeeDtoHelper = employeeDtoHelper;
        _employeeFullDtoHelper = employeeFullDtoHelper;
    }

    [Create("active")]
    public EmployeeFullDto AddMemberAsActivatedFromBody([FromBody] MemberRequestDto memberModel)
    {
        return AddMemberAsActivated(memberModel);
    }

    [Create("active")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto AddMemberAsActivatedFromForm([FromForm] MemberRequestDto memberModel)
    {
        return AddMemberAsActivated(memberModel);
    }

    [Create]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    public EmployeeFullDto AddMemberFromBody([FromBody] MemberRequestDto memberModel)
    {
        return AddMember(memberModel);
    }

    [Create]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto AddMemberFromForm([FromForm] MemberRequestDto memberModel)
    {
        return AddMember(memberModel);
    }

    [Update("{userid}/password")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
    public EmployeeFullDto ChangeUserPasswordFromBody(Guid userid, [FromBody] MemberRequestDto memberModel)
    {
        return ChangeUserPassword(userid, memberModel);
    }

    [Update("{userid}/password")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto ChangeUserPasswordFromForm(Guid userid, [FromForm] MemberRequestDto memberModel)
    {
        return ChangeUserPassword(userid, memberModel);
    }

    [Delete("{userid}")]
    public EmployeeFullDto DeleteMember(string userid)
    {
        PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID) || user.IsLDAP())
        {
            throw new SecurityException();
        }

        if (user.Status != EmployeeStatus.Terminated)
        {
            throw new Exception("The user is not suspended");
        }

        CheckReassignProccess(new[] { user.ID });

        var userName = user.DisplayUserName(false, DisplayUserSettingsHelper);
        UserPhotoManager.RemovePhoto(user.ID);
        UserManager.DeleteUser(user.ID);
        _queueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, false);

        MessageService.Send(MessageAction.UserDeleted, MessageTarget.Create(user.ID), userName);

        return _employeeFullDtoHelper.GetFull(user);
    }

    [Delete("@self")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "ProfileRemove")]
    public EmployeeFullDto DeleteProfile()
    {
        ApiContext.AuthByClaim();

        if (UserManager.IsSystemUser(SecurityContext.CurrentAccount.ID))
        {
            throw new SecurityException();
        }

        var user = GetUserInfo(SecurityContext.CurrentAccount.ID.ToString());

        if (!UserManager.UserExists(user))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (user.IsLDAP())
        {
            throw new SecurityException();
        }

        SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
        user.Status = EmployeeStatus.Terminated;

        UserManager.SaveUserInfo(user);
        var userName = user.DisplayUserName(false, DisplayUserSettingsHelper);
        MessageService.Send(MessageAction.UsersUpdatedStatus, MessageTarget.Create(user.ID), userName);

        _cookiesManager.ResetUserCookie(user.ID);
        MessageService.Send(MessageAction.CookieSettingsUpdated);

        if (_coreBaseSettings.Personal)
        {
            UserPhotoManager.RemovePhoto(user.ID);
            UserManager.DeleteUser(user.ID);
            MessageService.Send(MessageAction.UserDeleted, MessageTarget.Create(user.ID), userName);
        }
        else
        {
            //StudioNotifyService.Instance.SendMsgProfileHasDeletedItself(user);
            //StudioNotifyService.SendMsgProfileDeletion(Tenant.TenantId, user);
        }

        return _employeeFullDtoHelper.GetFull(user);
    }

    [Read("status/{status}/search")]
    public IEnumerable<EmployeeFullDto> GetAdvanced(EmployeeStatus status, [FromQuery] string query)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }
        try
        {
            var list = UserManager.GetUsers(status).AsEnumerable();

            if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
            {
                var groupId = new Guid(ApiContext.FilterValue);
                //Filter by group
                list = list.Where(x => UserManager.IsUserInGroup(x.ID, groupId));
                ApiContext.SetDataFiltered();
            }

            list = list.Where(x => x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1 || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                                   (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.ContactsList != null && x.ContactsList.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1)));

            return list.Select(u => _employeeFullDtoHelper.GetFull(u));
        }
        catch (Exception error)
        {
            _logger.Error(error);
        }

        return null;
    }

    [Read]
    public IEnumerable<EmployeeDto> GetAll()
    {
        return GetByStatus(EmployeeStatus.Active);
    }

    [Read("email")]
    public EmployeeFullDto GetByEmail([FromQuery] string email)
    {
        if (_coreBaseSettings.Personal && !UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOwner(Tenant))
        {
            throw new MethodAccessException("Method not available");
        }

        var user = UserManager.GetUserByEmail(email);
        if (user.ID == Constants.LostUser.ID)
        {
            throw new ItemNotFoundException("User not found");
        }

        return _employeeFullDtoHelper.GetFull(user);
    }

    [Read("{username}", order: int.MaxValue)]
    public EmployeeFullDto GetById(string username)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var user = UserManager.GetUserByUserName(username);
        if (user.ID == Constants.LostUser.ID)
        {
            if (Guid.TryParse(username, out var userId))
            {
                user = UserManager.GetUsers(userId);
            }
            else
            {
                _logger.Error(string.Format("Account {0} сould not get user by name {1}", SecurityContext.CurrentAccount.ID, username));
            }
        }

        if (user.ID == Constants.LostUser.ID)
        {
            throw new ItemNotFoundException("User not found");
        }

        return _employeeFullDtoHelper.GetFull(user);
    }

    [Read("status/{status}")]
    public IEnumerable<EmployeeDto> GetByStatus(EmployeeStatus status)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new Exception("Method not available");
        }

        Guid? groupId = null;
        if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
        {
            groupId = new Guid(ApiContext.FilterValue);
            ApiContext.SetDataFiltered();
        }

        return GetFullByFilter(status, groupId, null, null, null);
    }

    [Read("filter")]
    public IEnumerable<EmployeeFullDto> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator).AsEnumerable();

        return users.Select(u => _employeeFullDtoHelper.GetFull(u));
    }

    [Read("info")]
    public Module GetModule()
    {
        var product = new PeopleProduct();
        product.Init();

        return new Module(product);
    }
    [Read("search")]
    public IEnumerable<EmployeeFullDto> GetPeopleSearch([FromQuery] string query)
    {
        return GetSearch(query);
    }

    [Read("@search/{query}")]
    public IEnumerable<EmployeeFullDto> GetSearch(string query)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        try
        {
            var groupId = Guid.Empty;
            if ("group".Equals(ApiContext.FilterBy, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ApiContext.FilterValue))
            {
                groupId = new Guid(ApiContext.FilterValue);
            }

            var users = UserManager.Search(query, EmployeeStatus.Active, groupId);

            return users.Select(u => _employeeFullDtoHelper.GetFull(u));
        }
        catch (Exception error)
        {
            _logger.Error(error);
        }

        return null;
    }

    [Read("simple/filter")]
    public IEnumerable<EmployeeDto> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        var users = GetByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);

        return users.Select(u => _employeeDtoHelper.Get(u));
    }

    [AllowAnonymous]
    [Create(@"register")]
    public Task<string> RegisterUserOnPersonalAsync(RegisterPersonalUserRequestDto model)
    {
        if (!_coreBaseSettings.Personal) throw new MethodAccessException("Method is only available on personal.onlyoffice.com");

        return InternalRegisterUserOnPersonalAsync(model);
    }

    [Update("delete", Order = -1)]
    public IEnumerable<EmployeeFullDto> RemoveUsersFromBody([FromBody] UpdateMembersRequestDto model)
    {
        return RemoveUsers(model);
    }

    [Update("delete", Order = -1)]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> RemoveUsersFromForm([FromForm] UpdateMembersRequestDto model)
    {
        return RemoveUsers(model);
    }

    [Update("invite")]
    public IEnumerable<EmployeeFullDto> ResendUserInvitesFromBody([FromBody] UpdateMembersRequestDto model)
    {
        return ResendUserInvites(model);
    }

    [Update("invite")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> ResendUserInvitesFromForm([FromForm] UpdateMembersRequestDto model)
    {
        return ResendUserInvites(model);
    }

    [Read("@self")]
    public EmployeeDto Self()
    {
        var user = UserManager.GetUser(SecurityContext.CurrentAccount.ID, EmployeeFullDtoHelper.GetExpression(ApiContext));

        return _employeeFullDtoHelper.GetFull(user);
    }

    [Create("email", false)]
    public object SendEmailChangeInstructionsFromBody([FromBody] UpdateMemberRequestDto model)
    {
        return SendEmailChangeInstructions(model);
    }

    [Create("email", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendEmailChangeInstructionsFromForm([FromForm] UpdateMemberRequestDto model)
    {
        return SendEmailChangeInstructions(model);
    }

    [AllowAnonymous]
    [Create("password", false)]
    public object SendUserPasswordFromBody([FromBody] MemberRequestDto memberModel)
    {
        return SendUserPassword(memberModel);
    }

    [AllowAnonymous]
    [Create("password", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendUserPasswordFromForm([FromForm] MemberRequestDto memberModel)
    {
        return SendUserPassword(memberModel);
    }

    [Update("activationstatus/{activationstatus}")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
    public IEnumerable<EmployeeFullDto> UpdateEmployeeActivationStatusFromBody(EmployeeActivationStatus activationstatus, [FromBody] UpdateMembersRequestDto model)
    {
        return UpdateEmployeeActivationStatus(activationstatus, model);
    }

    [Update("activationstatus/{activationstatus}")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> UpdateEmployeeActivationStatusFromForm(EmployeeActivationStatus activationstatus, [FromForm] UpdateMembersRequestDto model)
    {
        return UpdateEmployeeActivationStatus(activationstatus, model);
    }

    [Update("{userid}/culture")]
    public EmployeeFullDto UpdateMemberCultureFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return UpdateMemberCulture(userid, memberModel);
    }

    [Update("{userid}/culture")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto UpdateMemberCultureFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return UpdateMemberCulture(userid, memberModel);
    }

    [Update("{userid}")]
    public EmployeeFullDto UpdateMemberFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return UpdateMember(userid, memberModel);
    }

    [Update("{userid}")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto UpdateMemberFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return UpdateMember(userid, memberModel);
    }

    [Update("status/{status}")]
    public IEnumerable<EmployeeFullDto> UpdateUserStatusFromBody(EmployeeStatus status, [FromBody] UpdateMembersRequestDto model)
    {
        return UpdateUserStatus(status, model);
    }

    [Update("status/{status}")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> UpdateUserStatusFromForm(EmployeeStatus status, [FromForm] UpdateMembersRequestDto model)
    {
        return UpdateUserStatus(status, model);
    }

    [Update("type/{type}")]
    public IEnumerable<EmployeeFullDto> UpdateUserTypeFromBody(EmployeeType type, [FromBody] UpdateMembersRequestDto model)
    {
        return UpdateUserType(type, model);
    }

    [Update("type/{type}")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> UpdateUserTypeFromForm(EmployeeType type, [FromForm] UpdateMembersRequestDto model)
    {
        return UpdateUserType(type, model);
    }

    private EmployeeFullDto AddMember(MemberRequestDto memberModel)
    {
        ApiContext.AuthByClaim();

        PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

        memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();
        if (string.IsNullOrEmpty(memberModel.PasswordHash))
        {
            memberModel.Password = (memberModel.Password ?? "").Trim();

            if (string.IsNullOrEmpty(memberModel.Password))
            {
                memberModel.Password = UserManagerWrapper.GeneratePassword();
            }
            else
            {
                _userManagerWrapper.CheckPasswordPolicy(memberModel.Password);
            }
            memberModel.PasswordHash = _passwordHasher.GetClientPassword(memberModel.Password);
        }

        var user = new UserInfo();

        //Validate email
        var address = new MailAddress(memberModel.Email);
        user.Email = address.Address;
        //Set common fields
        user.FirstName = memberModel.Firstname;
        user.LastName = memberModel.Lastname;
        user.Title = memberModel.Title;
        user.Location = memberModel.Location;
        user.Notes = memberModel.Comment;
        user.Sex = "male".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase)
                       ? true
                       : ("female".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);

        user.BirthDate = memberModel.Birthday != null && memberModel.Birthday != DateTime.MinValue ? _tenantUtil.DateTimeFromUtc(memberModel.Birthday) : null;
        user.WorkFromDate = memberModel.Worksfrom != null && memberModel.Worksfrom != DateTime.MinValue ? _tenantUtil.DateTimeFromUtc(memberModel.Worksfrom) : DateTime.UtcNow.Date;

        UpdateContacts(memberModel.Contacts, user);

        user = _userManagerWrapper.AddUser(user, memberModel.PasswordHash, memberModel.FromInviteLink, true, memberModel.IsVisitor, memberModel.FromInviteLink);

        var messageAction = memberModel.IsVisitor ? MessageAction.GuestCreated : MessageAction.UserCreated;
        MessageService.Send(messageAction, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

        UpdateDepartments(memberModel.Department, user);

        if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            UpdatePhotoUrl(memberModel.Files, user);
        }

        return _employeeFullDtoHelper.GetFull(user);
    }

    private EmployeeFullDto AddMemberAsActivated(MemberRequestDto memberModel)
    {
        PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

        var user = new UserInfo();

        memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();
        if (string.IsNullOrEmpty(memberModel.PasswordHash))
        {
            memberModel.Password = (memberModel.Password ?? "").Trim();

            if (string.IsNullOrEmpty(memberModel.Password))
            {
                memberModel.Password = UserManagerWrapper.GeneratePassword();
            }
            else
            {
                _userManagerWrapper.CheckPasswordPolicy(memberModel.Password);
            }

            memberModel.PasswordHash = _passwordHasher.GetClientPassword(memberModel.Password);
        }

        //Validate email
        var address = new MailAddress(memberModel.Email);
        user.Email = address.Address;
        //Set common fields
        user.FirstName = memberModel.Firstname;
        user.LastName = memberModel.Lastname;
        user.Title = memberModel.Title;
        user.Location = memberModel.Location;
        user.Notes = memberModel.Comment;
        user.Sex = "male".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase)
                       ? true
                       : ("female".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);

        user.BirthDate = memberModel.Birthday != null ? _tenantUtil.DateTimeFromUtc(memberModel.Birthday) : null;
        user.WorkFromDate = memberModel.Worksfrom != null ? _tenantUtil.DateTimeFromUtc(memberModel.Worksfrom) : DateTime.UtcNow.Date;

        UpdateContacts(memberModel.Contacts, user);

        user = _userManagerWrapper.AddUser(user, memberModel.PasswordHash, false, false, memberModel.IsVisitor);

        user.ActivationStatus = EmployeeActivationStatus.Activated;

        UpdateDepartments(memberModel.Department, user);

        if (memberModel.Files != UserPhotoManager.GetDefaultPhotoAbsoluteWebPath())
        {
            UpdatePhotoUrl(memberModel.Files, user);
        }

        return _employeeFullDtoHelper.GetFull(user);
    }

    private EmployeeFullDto ChangeUserPassword(Guid userid, MemberRequestDto memberModel)
    {
        ApiContext.AuthByClaim();
        PermissionContext.DemandPermissions(new UserSecurityProvider(userid), Constants.Action_EditUser);

        var user = UserManager.GetUsers(userid);

        if (!UserManager.UserExists(user))
        {
            return null;
        }

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        if (!string.IsNullOrEmpty(memberModel.Email))
        {
            var address = new MailAddress(memberModel.Email);
            if (!string.Equals(address.Address, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = address.Address.ToLowerInvariant();
                user.ActivationStatus = EmployeeActivationStatus.Activated;
                UserManager.SaveUserInfo(user);
            }
        }

        memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();
        if (string.IsNullOrEmpty(memberModel.PasswordHash))
        {
            memberModel.Password = (memberModel.Password ?? "").Trim();

            if (!string.IsNullOrEmpty(memberModel.Password))
            {
                memberModel.PasswordHash = _passwordHasher.GetClientPassword(memberModel.Password);
            }
        }

        if (!string.IsNullOrEmpty(memberModel.PasswordHash))
        {
            SecurityContext.SetUserPasswordHash(userid, memberModel.PasswordHash);
            MessageService.Send(MessageAction.UserUpdatedPassword);

            _cookiesManager.ResetUserCookie(userid);
            MessageService.Send(MessageAction.CookieSettingsUpdated);
        }

        return _employeeFullDtoHelper.GetFull(GetUserInfo(userid.ToString()));
    }

    private void CheckReassignProccess(IEnumerable<Guid> userIds)
    {
        foreach (var userId in userIds)
        {
            var reassignStatus = _queueWorkerReassign.GetProgressItemStatus(Tenant.TenantId, userId);
            if (reassignStatus == null || reassignStatus.IsCompleted)
            {
                continue;
            }

            var userName = UserManager.GetUsers(userId).DisplayUserName(DisplayUserSettingsHelper);

            throw new Exception(string.Format(Resource.ReassignDataRemoveUserError, userName));
        }
    }

    private IQueryable<UserInfo> GetByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        if (_coreBaseSettings.Personal)
        {
            throw new MethodAccessException("Method not available");
        }

        var isAdmin = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin(UserManager) ||
                      _webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);

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

        var users = UserManager.GetUsers(isAdmin, employeeStatus, includeGroups, excludeGroups, activationStatus, ApiContext.FilterValue, ApiContext.SortBy, !ApiContext.SortDescending, ApiContext.Count, ApiContext.StartIndex, out var total, out var count);

        ApiContext.SetTotalCount(total).SetCount(count);

        return users;
    }

    private async Task<string> InternalRegisterUserOnPersonalAsync(RegisterPersonalUserRequestDto model)
    {
        try
        {
            if (_coreBaseSettings.CustomMode) model.Lang = "ru-RU";

            var cultureInfo = SetupInfo.GetPersonalCulture(model.Lang).Value;

            if (cultureInfo != null)
            {
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }

            model.Email.ThrowIfNull(new ArgumentException(Resource.ErrorEmailEmpty, "email"));

            if (!model.Email.TestEmailRegex()) throw new ArgumentException(Resource.ErrorNotCorrectEmail, "email");

            if (!SetupInfo.IsSecretEmail(model.Email)
                && !string.IsNullOrEmpty(SetupInfo.RecaptchaPublicKey) && !string.IsNullOrEmpty(SetupInfo.RecaptchaPrivateKey))
            {
                var ip = Request.Headers["X-Forwarded-For"].ToString() ?? Request.GetUserHostAddress();

                if (string.IsNullOrEmpty(model.RecaptchaResponse)
                    || !await _recaptcha.ValidateRecaptchaAsync(model.RecaptchaResponse, ip))
                {
                    throw new RecaptchaException(Resource.RecaptchaInvalid);
                }
            }

            var newUserInfo = UserManager.GetUserByEmail(model.Email);

            if (UserManager.UserExists(newUserInfo.ID))
            {
                if (!SetupInfo.IsSecretEmail(model.Email) || SecurityContext.IsAuthenticated)
                {
                    StudioNotifyService.SendAlreadyExist(model.Email);
                    return string.Empty;
                }

                try
                {
                    SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                    UserManager.DeleteUser(newUserInfo.ID);
                }
                finally
                {
                    SecurityContext.Logout();
                }
            }
            if (!model.Spam)
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
                    _logger.Debug($"ERROR write to template_unsubscribe {ex.Message}, email:{model.Email.ToLowerInvariant()}");
                }
            }

            StudioNotifyService.SendInvitePersonal(model.Email);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return string.Empty;
    }
    private IEnumerable<EmployeeFullDto> RemoveUsers(UpdateMembersRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_AddRemoveUser);

        CheckReassignProccess(model.UserIds);

        var users = model.UserIds.Select(userId => UserManager.GetUsers(userId))
            .Where(u => !UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
            .ToList();

        var userNames = users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)).ToList();

        foreach (var user in users)
        {
            if (user.Status != EmployeeStatus.Terminated)
            {
                continue;
            }

            UserPhotoManager.RemovePhoto(user.ID);
            UserManager.DeleteUser(user.ID);
            _queueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, false);
        }

        MessageService.Send(MessageAction.UsersDeleted, MessageTarget.Create(users.Select(x => x.ID)), userNames);

        return users.Select(u => _employeeFullDtoHelper.GetFull(u));
    }

    private IEnumerable<EmployeeFullDto> ResendUserInvites(UpdateMembersRequestDto model)
    {
        var users = model.UserIds
            .Where(userId => !UserManager.IsSystemUser(userId))
            .Select(userId => UserManager.GetUsers(userId))
            .ToList();

        foreach (var user in users)
        {
            if (user.IsActive)
            {
                continue;
            }

            var viewer = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            if (viewer == null)
            {
                throw new Exception(Resource.ErrorAccessDenied);
            }

            if (viewer.IsAdmin(UserManager) || viewer.ID == user.ID)
            {
                if (user.ActivationStatus == EmployeeActivationStatus.Activated)
                {
                    user.ActivationStatus = EmployeeActivationStatus.NotActivated;
                }
                if (user.ActivationStatus == (EmployeeActivationStatus.AutoGenerated | EmployeeActivationStatus.Activated))
                {
                    user.ActivationStatus = EmployeeActivationStatus.AutoGenerated;
                }

                UserManager.SaveUserInfo(user);
            }

            if (user.ActivationStatus == EmployeeActivationStatus.Pending)
            {
                if (user.IsVisitor(UserManager))
                {
                    StudioNotifyService.GuestInfoActivation(user);
                }
                else
                {
                    StudioNotifyService.UserInfoActivation(user);
                }
            }
            else
            {
                StudioNotifyService.SendEmailActivationInstructions(user, user.Email);
            }
        }

        MessageService.Send(MessageAction.UsersSentActivationInstructions, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)));

        return users.Select(u => _employeeFullDtoHelper.GetFull(u));
    }

    private object SendEmailChangeInstructions(UpdateMemberRequestDto model)
    {
        Guid.TryParse(model.UserId, out var userid);

        if (userid == Guid.Empty)
        {
            throw new ArgumentNullException("userid");
        }

        var email = (model.Email ?? "").Trim();

        if (string.IsNullOrEmpty(email))
        {
            throw new Exception(Resource.ErrorEmailEmpty);
        }

        if (!email.TestEmailRegex())
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }

        var viewer = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
        var user = UserManager.GetUsers(userid);

        if (user == null)
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (viewer == null || (user.IsOwner(Tenant) && viewer.ID != user.ID))
        {
            throw new Exception(Resource.ErrorAccessDenied);
        }

        var existentUser = UserManager.GetUserByEmail(email);

        if (existentUser.ID != Constants.LostUser.ID)
        {
            throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
        }

        if (!viewer.IsAdmin(UserManager))
        {
            StudioNotifyService.SendEmailChangeInstructions(user, email);
        }
        else
        {
            if (email == user.Email)
            {
                throw new Exception(Resource.ErrorEmailsAreTheSame);
            }

            user.Email = email;
            user.ActivationStatus = EmployeeActivationStatus.NotActivated;
            UserManager.SaveUserInfo(user);
            StudioNotifyService.SendEmailActivationInstructions(user, email);
        }

        MessageService.Send(MessageAction.UserSentEmailChangeInstructions, user.DisplayUserName(false, DisplayUserSettingsHelper));

        return string.Format(Resource.MessageEmailChangeInstuctionsSentOnEmail, email);
    }

    private object SendUserPassword(MemberRequestDto memberModel)
    {
        string error = _userManagerWrapper.SendUserPassword(memberModel.Email);
        if (!string.IsNullOrEmpty(error))
        {
            _logger.ErrorFormat("Password recovery ({0}): {1}", memberModel.Email, error);
        }

        return string.Format(Resource.MessageYourPasswordSendedToEmail, memberModel.Email);
    }

    private void UpdateDepartments(IEnumerable<Guid> department, UserInfo user)
    {
        if (!PermissionContext.CheckPermissions(Constants.Action_EditGroups))
        {
            return;
        }

        if (department == null)
        {
            return;
        }

        var groups = UserManager.GetUserGroups(user.ID);
        var managerGroups = new List<Guid>();
        foreach (var groupInfo in groups)
        {
            UserManager.RemoveUserFromGroup(user.ID, groupInfo.ID);
            var managerId = UserManager.GetDepartmentManager(groupInfo.ID);
            if (managerId == user.ID)
            {
                managerGroups.Add(groupInfo.ID);
                UserManager.SetDepartmentManager(groupInfo.ID, Guid.Empty);
            }
        }
        foreach (var guid in department)
        {
            var userDepartment = UserManager.GetGroupInfo(guid);
            if (userDepartment != Constants.LostGroupInfo)
            {
                UserManager.AddUserIntoGroup(user.ID, guid);
                if (managerGroups.Contains(guid))
                {
                    UserManager.SetDepartmentManager(guid, user.ID);
                }
            }
        }
    }

    private IEnumerable<EmployeeFullDto> UpdateEmployeeActivationStatus(EmployeeActivationStatus activationstatus, UpdateMembersRequestDto model)
    {
        ApiContext.AuthByClaim();

        var retuls = new List<EmployeeFullDto>();
        foreach (var id in model.UserIds.Where(userId => !UserManager.IsSystemUser(userId)))
        {
            PermissionContext.DemandPermissions(new UserSecurityProvider(id), Constants.Action_EditUser);
            var u = UserManager.GetUsers(id);
            if (u.ID == Constants.LostUser.ID || u.IsLDAP())
            {
                continue;
            }

            u.ActivationStatus = activationstatus;
            UserManager.SaveUserInfo(u);
            retuls.Add(_employeeFullDtoHelper.GetFull(u));
        }

        return retuls;
    }

    private EmployeeFullDto UpdateMember(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);
        var self = SecurityContext.CurrentAccount.ID.Equals(user.ID);
        var resetDate = new DateTime(1900, 01, 01);

        //Update it

        var isLdap = user.IsLDAP();
        var isSso = user.IsSSO();
        var isAdmin = _webItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, SecurityContext.CurrentAccount.ID);

        if (!isLdap && !isSso)
        {
            //Set common fields

            user.FirstName = memberModel.Firstname ?? user.FirstName;
            user.LastName = memberModel.Lastname ?? user.LastName;
            user.Location = memberModel.Location ?? user.Location;

            if (isAdmin)
            {
                user.Title = memberModel.Title ?? user.Title;
            }
        }

        if (!_userFormatter.IsValidUserName(user.FirstName, user.LastName))
        {
            throw new Exception(Resource.ErrorIncorrectUserName);
        }

        user.Notes = memberModel.Comment ?? user.Notes;
        user.Sex = ("male".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase)
            ? true
            : ("female".Equals(memberModel.Sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null)) ?? user.Sex;

        user.BirthDate = memberModel.Birthday != null ? _tenantUtil.DateTimeFromUtc(memberModel.Birthday) : user.BirthDate;

        if (user.BirthDate == resetDate)
        {
            user.BirthDate = null;
        }

        user.WorkFromDate = memberModel.Worksfrom != null ? _tenantUtil.DateTimeFromUtc(memberModel.Worksfrom) : user.WorkFromDate;

        if (user.WorkFromDate == resetDate)
        {
            user.WorkFromDate = null;
        }

        //Update contacts
        UpdateContacts(memberModel.Contacts, user);
        UpdateDepartments(memberModel.Department, user);

        if (memberModel.Files != UserPhotoManager.GetPhotoAbsoluteWebPath(user.ID))
        {
            UpdatePhotoUrl(memberModel.Files, user);
        }
        if (memberModel.Disable.HasValue)
        {
            user.Status = memberModel.Disable.Value ? EmployeeStatus.Terminated : EmployeeStatus.Active;
            user.TerminatedDate = memberModel.Disable.Value ? DateTime.UtcNow : null;
        }
        if (self && !isAdmin)
        {
            StudioNotifyService.SendMsgToAdminAboutProfileUpdated();
        }

        // change user type
        var canBeGuestFlag = !user.IsOwner(Tenant) && !user.IsAdmin(UserManager) && user.GetListAdminModules(_webItemSecurity).Count == 0 && !user.IsMe(AuthContext);

        if (memberModel.IsVisitor && !user.IsVisitor(UserManager) && canBeGuestFlag)
        {
            UserManager.AddUserIntoGroup(user.ID, Constants.GroupVisitor.ID);
            _webItemSecurityCache.ClearCache(Tenant.TenantId);
        }

        if (!self && !memberModel.IsVisitor && user.IsVisitor(UserManager))
        {
            var usersQuota = _tenantExtra.GetTenantQuota().ActiveUsers;
            if (_tenantStatisticsProvider.GetUsersCount() < usersQuota)
            {
                UserManager.RemoveUserFromGroup(user.ID, Constants.GroupVisitor.ID);
                _webItemSecurityCache.ClearCache(Tenant.TenantId);
            }
            else
            {
                throw new TenantQuotaException(string.Format("Exceeds the maximum active users ({0})", usersQuota));
            }
        }

        UserManager.SaveUserInfo(user);
        MessageService.Send(MessageAction.UserUpdated, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

        if (memberModel.Disable.HasValue && memberModel.Disable.Value)
        {
            _cookiesManager.ResetUserCookie(user.ID);
            MessageService.Send(MessageAction.CookieSettingsUpdated);
        }

        return _employeeFullDtoHelper.GetFull(user);
    }
    private EmployeeFullDto UpdateMemberCulture(string userid, UpdateMemberRequestDto memberModel)
    {
        var user = GetUserInfo(userid);

        if (UserManager.IsSystemUser(user.ID))
        {
            throw new SecurityException();
        }

        PermissionContext.DemandPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

        var curLng = user.CultureName;

        if (SetupInfo.EnabledCultures.Find(c => string.Equals(c.Name, memberModel.CultureName, StringComparison.InvariantCultureIgnoreCase)) != null)
        {
            if (curLng != memberModel.CultureName)
            {
                user.CultureName = memberModel.CultureName;

                try
                {
                    UserManager.SaveUserInfo(user);
                }
                catch
                {
                    user.CultureName = curLng;
                    throw;
                }

                MessageService.Send(MessageAction.UserUpdatedLanguage, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper));

            }
        }

        return _employeeFullDtoHelper.GetFull(user);
    }
    private IEnumerable<EmployeeFullDto> UpdateUserStatus(EmployeeStatus status, UpdateMembersRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        var users = model.UserIds.Select(userId => UserManager.GetUsers(userId))
            .Where(u => !UserManager.IsSystemUser(u.ID) && !u.IsLDAP())
            .ToList();

        foreach (var user in users)
        {
            if (user.IsOwner(Tenant) || user.IsMe(AuthContext))
            {
                continue;
            }

            switch (status)
            {
                case EmployeeStatus.Active:
                    if (user.Status == EmployeeStatus.Terminated)
                    {
                        if (_tenantStatisticsProvider.GetUsersCount() < _tenantExtra.GetTenantQuota().ActiveUsers || user.IsVisitor(UserManager))
                        {
                            user.Status = EmployeeStatus.Active;
                            UserManager.SaveUserInfo(user);
                        }
                    }
                    break;
                case EmployeeStatus.Terminated:
                    user.Status = EmployeeStatus.Terminated;
                    UserManager.SaveUserInfo(user);

                    _cookiesManager.ResetUserCookie(user.ID);
                    MessageService.Send(MessageAction.CookieSettingsUpdated);
                    break;
            }
        }

        MessageService.Send(MessageAction.UsersUpdatedStatus, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)));

        return users.Select(u => _employeeFullDtoHelper.GetFull(u));
    }

    private IEnumerable<EmployeeFullDto> UpdateUserType(EmployeeType type, UpdateMembersRequestDto model)
    {
        var users = model.UserIds
            .Where(userId => !UserManager.IsSystemUser(userId))
            .Select(userId => UserManager.GetUsers(userId))
            .ToList();

        foreach (var user in users)
        {
            if (user.IsOwner(Tenant) || user.IsAdmin(UserManager)
                || user.IsMe(AuthContext) || user.GetListAdminModules(_webItemSecurity).Count > 0)
            {
                continue;
            }

            switch (type)
            {
                case EmployeeType.User:
                    if (user.IsVisitor(UserManager))
                    {
                        if (_tenantStatisticsProvider.GetUsersCount() < _tenantExtra.GetTenantQuota().ActiveUsers)
                        {
                            UserManager.RemoveUserFromGroup(user.ID, Constants.GroupVisitor.ID);
                            _webItemSecurityCache.ClearCache(Tenant.TenantId);
                        }
                    }
                    break;
                case EmployeeType.Visitor:
                    if (_coreBaseSettings.Standalone || _tenantStatisticsProvider.GetVisitorsCount() < _tenantExtra.GetTenantQuota().ActiveUsers * _constants.CoefficientOfVisitors)
                    {
                        UserManager.AddUserIntoGroup(user.ID, Constants.GroupVisitor.ID);
                        _webItemSecurityCache.ClearCache(Tenant.TenantId);
                    }
                    break;
            }
        }

        MessageService.Send(MessageAction.UsersUpdatedType, MessageTarget.Create(users.Select(x => x.ID)), users.Select(x => x.DisplayUserName(false, DisplayUserSettingsHelper)));

        return users.Select(u => _employeeFullDtoHelper.GetFull(u));
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
    //[Create("import/save")]
    //public void SaveUsers(string userList, bool importUsersAsCollaborators)
    //{
    //    lock (progressQueue.SynchRoot)
    //    {
    //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
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

    //[Read("import/status")]
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
