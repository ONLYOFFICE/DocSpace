namespace ASC.Web.Api.Controllers.Authentication;
public class AuthenticationController: BaseAuthenticationController
{
    public AuthenticationController(UserManager userManager,
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

    [Read]
    public bool GetIsAuthentificated()
    {
        return _securityContext.IsAuthenticated;
    }

    [Create("{code}", false, order: int.MaxValue)]
    public AuthenticationTokenData AuthenticateMeFromBodyWithCode([FromBody] AuthDto auth)
    {
        return AuthenticateMeWithCode(auth);
    }

    [Create("{code}", false, order: int.MaxValue)]
    [Consumes("application/x-www-form-urlencoded")]
    public AuthenticationTokenData AuthenticateMeFromFormWithCode([FromForm] AuthDto auth)
    {
        return AuthenticateMeWithCode(auth);
    }

    [Create(false)]
    public AuthenticationTokenData AuthenticateMeFromBody([FromBody] AuthDto auth)
    {
        return AuthenticateMe(auth);
    }

    [Create(false)]
    [Consumes("application/x-www-form-urlencoded")]
    public AuthenticationTokenData AuthenticateMeFromForm([FromForm] AuthDto auth)
    {
        return AuthenticateMe(auth);
    }

    [Create("logout")]
    [Read("logout")]// temp fix
    public void Logout()
    {
        if (_securityContext.IsAuthenticated)
            _cookiesManager.ResetUserCookie(_securityContext.CurrentAccount.ID);

        _cookiesManager.ClearCookies(CookiesType.AuthKey);
        _cookiesManager.ClearCookies(CookiesType.SocketIO);

        _securityContext.Logout();
    }

    [Create("confirm", false)]
    public ValidationResult CheckConfirmFromBody([FromBody] EmailValidationKeyModel model)
    {
        return _emailValidationKeyModelHelper.Validate(model);
    }

    [Create("confirm", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public ValidationResult CheckConfirmFromForm([FromForm] EmailValidationKeyModel model)
    {
        return _emailValidationKeyModelHelper.Validate(model);
    }

    private AuthenticationTokenData AuthenticateMe(AuthDto auth)
    {
        bool viaEmail;
        var user = GetUser(auth, out viaEmail);

        if (_studioSmsNotificationSettingsHelper.IsVisibleSettings() && _studioSmsNotificationSettingsHelper.Enable)
        {
            if (string.IsNullOrEmpty(user.MobilePhone) || user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
                return new AuthenticationTokenData
                {
                    Sms = true,
                    ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneActivation)
                };

            _smsManager.PutAuthCode(user, false);

            return new AuthenticationTokenData
            {
                Sms = true,
                PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
                Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval)),
                ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneAuth)
            };
        }

        if (TfaAppAuthSettings.IsVisibleSettings && _settingsManager.Load<TfaAppAuthSettings>().EnableSetting)
        {
            if (!TfaAppUserSettings.EnableForUser(_settingsManager, user.ID))
                return new AuthenticationTokenData
                {
                    Tfa = true,
                    TfaKey = _tfaManager.GenerateSetupCode(user).ManualEntryKey,
                    ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaActivation)
                };

            return new AuthenticationTokenData
            {
                Tfa = true,
                ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaAuth)
            };
        }

        try
        {
            var token = _securityContext.AuthenticateMe(user.ID);
            _cookiesManager.SetCookies(CookiesType.AuthKey, token, auth.Session);

            _messageService.Send(viaEmail ? MessageAction.LoginSuccessViaApi : MessageAction.LoginSuccessViaApiSocialAccount);

            var tenant = _tenantManager.GetCurrentTenant().TenantId;
            var expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);

            return new AuthenticationTokenData
            {
                Token = token,
                Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, expires)
            };
        }
        catch
        {
            _messageService.Send(user.DisplayUserName(false, _displayUserSettingsHelper), viaEmail ? MessageAction.LoginFailViaApi : MessageAction.LoginFailViaApiSocialAccount);
            throw new AuthenticationException("User authentication failed");
        }
        finally
        {
            _securityContext.Logout();
        }
    }

    private AuthenticationTokenData AuthenticateMeWithCode(AuthDto auth)
    {
        var tenant = _tenantManager.GetCurrentTenant().TenantId;
        var user = GetUser(auth, out _);

        var sms = false;
        try
        {
            if (_studioSmsNotificationSettingsHelper.IsVisibleSettings() && _studioSmsNotificationSettingsHelper.Enable)
            {
                sms = true;
                _smsManager.ValidateSmsCode(user, auth.Code);
            }
            else if (TfaAppAuthSettings.IsVisibleSettings && _settingsManager.Load<TfaAppAuthSettings>().EnableSetting)
            {
                if (_tfaManager.ValidateAuthCode(user, auth.Code))
                {
                    _messageService.Send(MessageAction.UserConnectedTfaApp, _messageTarget.Create(user.ID));
                }
            }
            else
            {
                throw new System.Security.SecurityException("Auth code is not available");
            }

            var token = _securityContext.AuthenticateMe(user.ID);

            _messageService.Send(sms ? MessageAction.LoginSuccessViaApiSms : MessageAction.LoginSuccessViaApiTfa);

            var expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);

            var result = new AuthenticationTokenData
            {
                Token = token,
                Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, expires)
            };

            if (sms)
            {
                result.Sms = true;
                result.PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone);
            }
            else
            {
                result.Tfa = true;
            }

            return result;
        }
        catch
        {
            _messageService.Send(user.DisplayUserName(false, _displayUserSettingsHelper), sms
                                                                            ? MessageAction.LoginFailViaApiSms
                                                                            : MessageAction.LoginFailViaApiTfa,
                                _messageTarget.Create(user.ID));
            throw new AuthenticationException("User authentication failed");
        }
        finally
        {
            _securityContext.Logout();
        }
    }
}
