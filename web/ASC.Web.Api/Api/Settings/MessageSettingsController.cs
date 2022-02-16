using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class MessageSettingsController: BaseSettingsController
{
    public MessageSettingsController(IOptionsMonitor<ILog> option,
        MessageService messageService,
        StudioNotifyService studioNotifyService, 
        ApiContext apiContext, 
        UserManager userManager, 
        TenantManager tenantManager, 
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticsProvider,
        AuthContext authContext,
        CookiesManager cookiesManager, 
        WebItemSecurity webItemSecurity, 
        StudioNotifyHelper studioNotifyHelper,
        LicenseReader licenseReader, 
        PermissionContext permissionContext,
        SettingsManager settingsManager, 
        TfaManager tfaManager, 
        WebItemManager webItemManager, 
        WebItemManagerSecurity webItemManagerSecurity,
        TenantInfoSettingsHelper tenantInfoSettingsHelper, 
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        StorageHelper storageHelper, 
        TenantLogoManager tenantLogoManager, 
        TenantUtil tenantUtil, 
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        ColorThemesSettingsHelper colorThemesSettingsHelper,
        IConfiguration configuration,
        SetupInfo setupInfo, 
        BuildVersion buildVersion, 
        DisplayUserSettingsHelper displayUserSettingsHelper,
        StatisticManager statisticManager,
        IPRestrictionsService iPRestrictionsService, 
        CoreConfiguration coreConfiguration, 
        MessageTarget messageTarget, 
        StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper, 
        CoreSettings coreSettings,
        StorageSettingsHelper storageSettingsHelper,
        IWebHostEnvironment webHostEnvironment,
        IServiceProvider serviceProvider, 
        EmployeeWraperHelper employeeWraperHelper,
        ConsumerFactory consumerFactory, 
        SmsProviderManager smsProviderManager,
        TimeZoneConverter timeZoneConverter,
        CustomNamingPeople customNamingPeople, 
        IPSecurity.IPSecurity ipSecurity,
        IMemoryCache memoryCache,
        ProviderManager providerManager,
        FirstTimeTenantSettings firstTimeTenantSettings, 
        ServiceClient serviceClient, 
        TelegramHelper telegramHelper,
        StorageFactory storageFactory,
        UrlShortener urlShortener,
        EncryptionServiceClient encryptionServiceClient,
        EncryptionSettingsHelper encryptionSettingsHelper,
        BackupAjaxHandler backupAjaxHandler,
        ICacheNotify<DeleteSchedule> cacheDeleteSchedule, 
        EncryptionWorker encryptionWorker,
        PasswordHasher passwordHasher,
        PaymentManager paymentManager, 
        Constants constants, 
        InstanceCrypto instanceCrypto, 
        Signature signature, 
        DbWorker dbWorker,
        IHttpClientFactory clientFactory) : base(option, messageService, studioNotifyService, apiContext, userManager, tenantManager, tenantExtra, tenantStatisticsProvider, authContext, cookiesManager, webItemSecurity, studioNotifyHelper, licenseReader, permissionContext, settingsManager, tfaManager, webItemManager, webItemManagerSecurity, tenantInfoSettingsHelper, tenantWhiteLabelSettingsHelper, storageHelper, tenantLogoManager, tenantUtil, coreBaseSettings, commonLinkUtility, colorThemesSettingsHelper, configuration, setupInfo, buildVersion, displayUserSettingsHelper, statisticManager, iPRestrictionsService, coreConfiguration, messageTarget, studioSmsNotificationSettingsHelper, coreSettings, storageSettingsHelper, webHostEnvironment, serviceProvider, employeeWraperHelper, consumerFactory, smsProviderManager, timeZoneConverter, customNamingPeople, ipSecurity, memoryCache, providerManager, firstTimeTenantSettings, serviceClient, telegramHelper, storageFactory, urlShortener, encryptionServiceClient, encryptionSettingsHelper, backupAjaxHandler, cacheDeleteSchedule, encryptionWorker, passwordHasher, paymentManager, constants, instanceCrypto, signature, dbWorker, clientFactory)
    {
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
            if (!user.ID.Equals(Constants.LostUser.ID))
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
