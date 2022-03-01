using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class MessageSettingsController: BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly IPSecurity.IPSecurity _ipSecurity;
    private readonly UserManager _userManager;
    private readonly TenantExtra _tenantExtra;
    private readonly TenantStatisticsProvider _tenantStatisticsProvider;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;

    public MessageSettingsController(
        MessageService messageService,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        UserManager userManager,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticsProvider,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        CoreBaseSettings coreBaseSettings,
        CustomNamingPeople customNamingPeople,
        IPSecurity.IPSecurity ipSecurity,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _customNamingPeople = customNamingPeople;
        _ipSecurity = ipSecurity;
        _messageService = messageService;
        _studioNotifyService = studioNotifyService;
        _userManager = userManager;
        _tenantExtra = tenantExtra;
        _tenantStatisticsProvider = tenantStatisticsProvider;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
    }

    [Create("messagesettings")]
    public object EnableAdminMessageSettingsFromBody([FromBody] AdminMessageSettingsDto model)
    {
        return EnableAdminMessageSettings(model);
    }

    [Create("messagesettings")]
    [Consumes("application/x-www-form-urlencoded")]
    public object EnableAdminMessageSettingsFromForm([FromForm] AdminMessageSettingsDto model)
    {
        return EnableAdminMessageSettings(model);
    }

    private object EnableAdminMessageSettings(AdminMessageSettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _settingsManager.Save(new StudioAdminMessageSettings { Enable = model.TurnOn });

        _messageService.Send(MessageAction.AdministratorMessageSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [AllowAnonymous]
    [Create("sendadmmail")]
    public object SendAdmMailFromBody([FromBody] AdminMessageSettingsDto model)
    {
        return SendAdmMail(model);
    }

    [AllowAnonymous]
    [Create("sendadmmail")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendAdmMailFromForm([FromForm] AdminMessageSettingsDto model)
    {
        return SendAdmMail(model);
    }

    private object SendAdmMail(AdminMessageSettingsDto model)
    {
        var studioAdminMessageSettings = _settingsManager.Load<StudioAdminMessageSettings>();
        var enableAdmMess = studioAdminMessageSettings.Enable || _tenantExtra.IsNotPaid();

        if (!enableAdmMess)
            throw new MethodAccessException("Method not available");

        if (!model.Email.TestEmailRegex())
            throw new Exception(Resource.ErrorNotCorrectEmail);

        if (string.IsNullOrEmpty(model.Message))
            throw new Exception(Resource.ErrorEmptyMessage);

        CheckCache("sendadmmail");

        _studioNotifyService.SendMsgToAdminFromNotAuthUser(model.Email, model.Message);
        _messageService.Send(MessageAction.ContactAdminMailSent);

        return Resource.AdminMessageSent;
    }

    [AllowAnonymous]
    [Create("sendjoininvite")]
    public object SendJoinInviteMailFromBody([FromBody] AdminMessageSettingsDto model)
    {
        return SendJoinInviteMail(model);
    }

    [AllowAnonymous]
    [Create("sendjoininvite")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendJoinInviteMailFromForm([FromForm] AdminMessageSettingsDto model)
    {
        return SendJoinInviteMail(model);
    }

    private object SendJoinInviteMail(AdminMessageSettingsDto model)
    {
        try
        {
            var email = model.Email;
            if (!(
                (Tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom &&
                Tenant.TrustedDomains.Count > 0) ||
                Tenant.TrustedDomainsType == TenantTrustedDomainsType.All))
                throw new MethodAccessException("Method not available");

            if (!email.TestEmailRegex())
                throw new Exception(Resource.ErrorNotCorrectEmail);

            CheckCache("sendjoininvite");

            var user = _userManager.GetUserByEmail(email);
            if (!user.Id.Equals(Constants.LostUser.Id))
                throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));

            var settings = _settingsManager.Load<IPRestrictionsSettings>();

            if (settings.Enable && !_ipSecurity.Verify())
                throw new Exception(Resource.ErrorAccessRestricted);

            var trustedDomainSettings = _settingsManager.Load<StudioTrustedDomainSettings>();
            var emplType = trustedDomainSettings.InviteUsersAsVisitors ? EmployeeType.Visitor : EmployeeType.User;
            if (!_coreBaseSettings.Personal)
            {
                var enableInviteUsers = _tenantStatisticsProvider.GetUsersCount() < _tenantExtra.GetTenantQuota().ActiveUsers;

                if (!enableInviteUsers)
                    emplType = EmployeeType.Visitor;
            }

            switch (Tenant.TrustedDomainsType)
            {
                case TenantTrustedDomainsType.Custom:
                {
                    var address = new MailAddress(email);
                    if (Tenant.TrustedDomains.Any(d => address.Address.EndsWith("@" + d.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _studioNotifyService.SendJoinMsg(email, emplType);
                        _messageService.Send(MessageInitiator.System, MessageAction.SentInviteInstructions, email);
                        return Resource.FinishInviteJoinEmailMessage;
                    }

                    throw new Exception(Resource.ErrorEmailDomainNotAllowed);
                }
                case TenantTrustedDomainsType.All:
                {
                    _studioNotifyService.SendJoinMsg(email, emplType);
                    _messageService.Send(MessageInitiator.System, MessageAction.SentInviteInstructions, email);
                    return Resource.FinishInviteJoinEmailMessage;
                }
                default:
                    throw new Exception(Resource.ErrorNotCorrectEmail);
            }
        }
        catch (FormatException)
        {
            return Resource.ErrorNotCorrectEmail;
        }
        catch (Exception e)
        {
            return e.Message.HtmlEncode();
        }
    }
}
