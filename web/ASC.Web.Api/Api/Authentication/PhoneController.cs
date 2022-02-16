namespace ASC.Web.Api.Controllers.Authentication;

public class PhoneController: BaseAuthenticationController
{
    public PhoneController(UserManager userManager,
        TenantManager tenantManager,
        ASC.Core.SecurityContext securityContext,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        CookiesManager cookiesManager,
        PasswordHasher passwordHasher,
        EmailValidationKeyModelHelper emailValidationKeyModelHelper, 
        ICache cache, SetupInfo setupInfo,
        MessageService messageService, 
        ProviderManager providerManager, 
        IOptionsSnapshot<AccountLinker> accountLinker, 
        CoreBaseSettings coreBaseSettings,
        PersonalSettingsHelper personalSettingsHelper,
        StudioNotifyService studioNotifyService, 
        UserManagerWrapper userManagerWrapper, 
        UserHelpTourHelper userHelpTourHelper, 
        Signature signature, 
        InstanceCrypto instanceCrypto,
        DisplayUserSettingsHelper displayUserSettingsHelper, 
        MessageTarget messageTarget, 
        StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper,
        SettingsManager settingsManager,
        SmsManager smsManager, 
        TfaManager tfaManager, 
        TimeZoneConverter timeZoneConverter, 
        SmsKeyStorage smsKeyStorage,
        CommonLinkUtility commonLinkUtility,
        ApiContext apiContext,
        AuthContext authContext) : base(userManager, tenantManager, securityContext, tenantCookieSettingsHelper, cookiesManager, passwordHasher, emailValidationKeyModelHelper, cache, setupInfo, messageService, providerManager, accountLinker, coreBaseSettings, personalSettingsHelper, studioNotifyService, userManagerWrapper, userHelpTourHelper, signature, instanceCrypto, displayUserSettingsHelper, messageTarget, studioSmsNotificationSettingsHelper, settingsManager, smsManager, tfaManager, timeZoneConverter, smsKeyStorage, commonLinkUtility, apiContext, authContext)
    {
    }

    [Authorize(AuthenticationSchemes = "confirm", Roles = "PhoneActivation")]
    [Create("setphone", false)]
    public AuthenticationTokenData SaveMobilePhoneFromBody([FromBody] MobileDto model)
    {
        return SaveMobilePhone(model);
    }

    [Authorize(AuthenticationSchemes = "confirm", Roles = "PhoneActivation")]
    [Create("setphone", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public AuthenticationTokenData SaveMobilePhoneFromForm([FromForm] MobileDto model)
    {
        return SaveMobilePhone(model);
    }

    [Create(@"sendsms", false)]
    public AuthenticationTokenData SendSmsCodeFromBody([FromBody] AuthDto model)
    {
        return SendSmsCode(model);
    }

    [Create(@"sendsms", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public AuthenticationTokenData SendSmsCodeFromForm([FromForm] AuthDto model)
    {
        return SendSmsCode(model);
    }

    private AuthenticationTokenData SendSmsCode(AuthDto model)
    {
        var user = GetUser(model, out _);
        _smsManager.PutAuthCode(user, true);

        return new AuthenticationTokenData
        {
            Sms = true,
            PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
            Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval))
        };
    }

    private AuthenticationTokenData SaveMobilePhone(MobileDto model)
    {
        _apiContext.AuthByClaim();
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        model.MobilePhone = _smsManager.SaveMobilePhone(user, model.MobilePhone);
        _messageService.Send(MessageAction.UserUpdatedMobileNumber, _messageTarget.Create(user.ID), user.DisplayUserName(false, _displayUserSettingsHelper), model.MobilePhone);

        return new AuthenticationTokenData
        {
            Sms = true,
            PhoneNoise = SmsSender.BuildPhoneNoise(model.MobilePhone),
            Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval))
        };
    }
}
