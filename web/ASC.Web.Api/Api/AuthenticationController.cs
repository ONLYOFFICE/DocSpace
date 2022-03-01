using AuthenticationException = System.Security.Authentication.AuthenticationException;
using Constants = ASC.Core.Users.Constants;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Api.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly TenantCookieSettingsHelper _tenantCookieSettingsHelper;
    private readonly CookiesManager _cookiesManager;
    private readonly PasswordHasher _passwordHasher;
    private readonly EmailValidationKeyModelHelper _emailValidationKeyModelHelper;
    private readonly ICache _cache;
    private readonly SetupInfo _setupInfo;
    private readonly MessageService _messageService;
    private readonly ProviderManager _providerManager;
    private readonly IOptionsSnapshot<AccountLinker> _accountLinker;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly PersonalSettingsHelper _personalSettingsHelper;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly UserHelpTourHelper _userHelpTourHelper;
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly MessageTarget _messageTarget;
    private readonly StudioSmsNotificationSettingsHelper _studioSmsNotificationSettingsHelper;
    private readonly SettingsManager _settingsManager;
    private readonly SmsManager _smsManager;
    private readonly TfaManager _tfaManager;
    private readonly TimeZoneConverter _timeZoneConverter;
    private readonly SmsKeyStorage _smsKeyStorage;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly ApiContext _apiContext;
    private readonly AuthContext _authContext;
    private readonly UserManagerWrapper _userManagerWrapper;

    public AuthenticationController(
        UserManager userManager,
        TenantManager tenantManager,
        SecurityContext securityContext,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        CookiesManager cookiesManager,
        PasswordHasher passwordHasher,
        EmailValidationKeyModelHelper emailValidationKeyModelHelper,
        ICache cache,
        SetupInfo setupInfo,
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
        AuthContext authContext)
    {
        _userManager = userManager;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _tenantCookieSettingsHelper = tenantCookieSettingsHelper;
        _cookiesManager = cookiesManager;
        _passwordHasher = passwordHasher;
        _emailValidationKeyModelHelper = emailValidationKeyModelHelper;
        _cache = cache;
        _setupInfo = setupInfo;
        _messageService = messageService;
        _providerManager = providerManager;
        _accountLinker = accountLinker;
        _coreBaseSettings = coreBaseSettings;
        _personalSettingsHelper = personalSettingsHelper;
        _studioNotifyService = studioNotifyService;
        _userHelpTourHelper = userHelpTourHelper;
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
        _studioSmsNotificationSettingsHelper = studioSmsNotificationSettingsHelper;
        _settingsManager = settingsManager;
        _smsManager = smsManager;
        _tfaManager = tfaManager;
        _timeZoneConverter = timeZoneConverter;
        _smsKeyStorage = smsKeyStorage;
        _commonLinkUtility = commonLinkUtility;
        _apiContext = apiContext;
        _authContext = authContext;
        _userManagerWrapper = userManagerWrapper;
    }


    [Read]
    public bool GetIsAuthentificated()
    {
        return _securityContext.IsAuthenticated;
    }

    [Create("{code}", false, order: int.MaxValue)]
    public AuthenticationTokenResponseDto AuthenticateMeFromBodyWithCode([FromBody] AuthDto auth)
    {
        return AuthenticateMeWithCode(auth);
    }

    [Create("{code}", false, order: int.MaxValue)]
    [Consumes("application/x-www-form-urlencoded")]
    public AuthenticationTokenResponseDto AuthenticateMeFromFormWithCode([FromForm] AuthDto auth)
    {
        return AuthenticateMeWithCode(auth);
    }

    [Create(false)]
    public Task<AuthenticationTokenResponseDto> AuthenticateMeFromBodyAsync([FromBody] AuthDto auth)
    {
        return AuthenticateMeAsync(auth);
    }

    [Create(false)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<AuthenticationTokenResponseDto> AuthenticateMeFromFormAsync([FromForm] AuthDto auth)
    {
        return AuthenticateMeAsync(auth);
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

    [Authorize(AuthenticationSchemes = "confirm", Roles = "PhoneActivation")]
    [Create("setphone", false)]
    public Task<AuthenticationTokenResponseDto> SaveMobilePhoneFromBodyAsync([FromBody] MobileDto model)
    {
        return SaveMobilePhoneAsync(model);
    }

    [Authorize(AuthenticationSchemes = "confirm", Roles = "PhoneActivation")]
    [Create("setphone", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<AuthenticationTokenResponseDto> SaveMobilePhoneFromFormAsync([FromForm] MobileDto model)
    {
        return SaveMobilePhoneAsync(model);
    }

    private async Task<AuthenticationTokenResponseDto> SaveMobilePhoneAsync(MobileDto model)
    {
        _apiContext.AuthByClaim();
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        model.MobilePhone = await _smsManager.SaveMobilePhoneAsync(user, model.MobilePhone);
        _messageService.Send(MessageAction.UserUpdatedMobileNumber, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper), model.MobilePhone);

        return new AuthenticationTokenResponseDto
        {
            Sms = true,
            PhoneNoise = SmsSender.BuildPhoneNoise(model.MobilePhone),
            Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval))
        };
    }

    [Create(@"sendsms", false)]
    public Task<AuthenticationTokenResponseDto> SendSmsCodeFromBodyAsync([FromBody] AuthDto model)
    {
        return SendSmsCodeAsync(model);
    }

    [Create(@"sendsms", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<AuthenticationTokenResponseDto> SendSmsCodeFromFormAsync([FromForm] AuthDto model)
    {
        return SendSmsCodeAsync(model);
    }

    private async Task<AuthenticationTokenResponseDto> SendSmsCodeAsync(AuthDto model)
    {
        var user = GetUser(model, out _);
        await _smsManager.PutAuthCodeAsync(user, true);

        return new AuthenticationTokenResponseDto
        {
            Sms = true,
            PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
            Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval))
        };
    }

    private async Task<AuthenticationTokenResponseDto> AuthenticateMeAsync(AuthDto auth)
    {
        bool viaEmail;
        var user = GetUser(auth, out viaEmail);

        if (_studioSmsNotificationSettingsHelper.IsVisibleSettings() && _studioSmsNotificationSettingsHelper.Enable)
        {
            if (string.IsNullOrEmpty(user.MobilePhone) || user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
                return new AuthenticationTokenResponseDto
                {
                    Sms = true,
                    ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneActivation)
                };

            await _smsManager.PutAuthCodeAsync(user, false);

            return new AuthenticationTokenResponseDto
            {
                Sms = true,
                PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
                Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval)),
                ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneAuth)
            };
        }

        if (TfaAppAuthSettings.IsVisibleSettings && _settingsManager.Load<TfaAppAuthSettings>().EnableSetting)
        {
            if (!TfaAppUserSettings.EnableForUser(_settingsManager, user.Id))
                return new AuthenticationTokenResponseDto
                {
                    Tfa = true,
                    TfaKey = _tfaManager.GenerateSetupCode(user).ManualEntryKey,
                    ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaActivation)
                };

            return new AuthenticationTokenResponseDto
            {
                Tfa = true,
                ConfirmUrl = _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaAuth)
            };
        }

        try
        {
            var token = _securityContext.AuthenticateMe(user.Id);
            _cookiesManager.SetCookies(CookiesType.AuthKey, token, auth.Session);

            _messageService.Send(viaEmail ? MessageAction.LoginSuccessViaApi : MessageAction.LoginSuccessViaApiSocialAccount);

            var tenant = _tenantManager.GetCurrentTenant().Id;
            var expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);

            return new AuthenticationTokenResponseDto
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

    private AuthenticationTokenResponseDto AuthenticateMeWithCode(AuthDto auth)
    {
        var tenant = _tenantManager.GetCurrentTenant().Id;
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
                    _messageService.Send(MessageAction.UserConnectedTfaApp, _messageTarget.Create(user.Id));
                }
            }
            else
            {
                throw new System.Security.SecurityException("Auth code is not available");
            }

            var token = _securityContext.AuthenticateMe(user.Id);

            _messageService.Send(sms ? MessageAction.LoginSuccessViaApiSms : MessageAction.LoginSuccessViaApiTfa);

            var expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);

            var result = new AuthenticationTokenResponseDto
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
                                _messageTarget.Create(user.Id));
            throw new AuthenticationException("User authentication failed");
        }
        finally
        {
            _securityContext.Logout();
        }
    }

    private UserInfo GetUser(AuthDto memberModel, out bool viaEmail)
    {
        viaEmail = true;
        var action = MessageAction.LoginFailViaApi;
        UserInfo user;
        try
        {
            if ((string.IsNullOrEmpty(memberModel.Provider) && string.IsNullOrEmpty(memberModel.SerializedProfile)) || memberModel.Provider == "email")
            {
                memberModel.UserName.ThrowIfNull(new ArgumentException(@"userName empty", "userName"));
                if (!string.IsNullOrEmpty(memberModel.Password))
                {
                    memberModel.Password.ThrowIfNull(new ArgumentException(@"password empty", "password"));
                }
                else
                {
                    memberModel.PasswordHash.ThrowIfNull(new ArgumentException(@"PasswordHash empty", "PasswordHash"));
                }
                int counter;
                int.TryParse(_cache.Get<string>("loginsec/" + memberModel.UserName), out counter);
                if (++counter > _setupInfo.LoginThreshold && !SetupInfo.IsSecretEmail(memberModel.UserName))
                {
                    throw new BruteForceCredentialException();
                }
                _cache.Insert("loginsec/" + memberModel.UserName, counter.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));


                memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();

                if (string.IsNullOrEmpty(memberModel.PasswordHash))
                {
                    memberModel.Password = (memberModel.Password ?? "").Trim();

                    if (!string.IsNullOrEmpty(memberModel.Password))
                    {
                        memberModel.PasswordHash = _passwordHasher.GetClientPassword(memberModel.Password);
                    }
                }

                user = _userManager.GetUsersByPasswordHash(
                    _tenantManager.GetCurrentTenant().Id,
                    memberModel.UserName,
                    memberModel.PasswordHash);

                if (user == null || !_userManager.UserExists(user))
                {
                    throw new Exception("user not found");
                }

                _cache.Insert("loginsec/" + memberModel.UserName, (--counter).ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));
            }
            else
            {
                viaEmail = false;
                action = MessageAction.LoginFailViaApiSocialAccount;
                LoginProfile thirdPartyProfile;
                if (!string.IsNullOrEmpty(memberModel.SerializedProfile))
                {
                    thirdPartyProfile = new LoginProfile(_signature, _instanceCrypto, memberModel.SerializedProfile);
                }
                else
                {
                    thirdPartyProfile = _providerManager.GetLoginProfile(memberModel.Provider, memberModel.AccessToken);
                }

                memberModel.UserName = thirdPartyProfile.EMail;

                user = GetUserByThirdParty(thirdPartyProfile);
            }
        }
        catch (BruteForceCredentialException)
        {
            _messageService.Send(!string.IsNullOrEmpty(memberModel.UserName) ? memberModel.UserName : AuditResource.EmailNotSpecified, MessageAction.LoginFailBruteForce);
            throw new AuthenticationException("Login Fail. Too many attempts");
        }
        catch
        {
            _messageService.Send(!string.IsNullOrEmpty(memberModel.UserName) ? memberModel.UserName : AuditResource.EmailNotSpecified, action);
            throw new AuthenticationException("User authentication failed");
        }

        return user;
    }

    private UserInfo GetUserByThirdParty(LoginProfile loginProfile)
    {
        try
        {
            if (!string.IsNullOrEmpty(loginProfile.AuthorizationError))
            {
                // ignore cancellation
                if (loginProfile.AuthorizationError != "Canceled at provider")
                {
                    throw new Exception(loginProfile.AuthorizationError);
                }
                return Constants.LostUser;
            }

            var userInfo = Constants.LostUser;

            Guid userId;
            if (TryGetUserByHash(loginProfile.HashId, out userId))
            {
                userInfo = _userManager.GetUsers(userId);
            }

            var isNew = false;
            if (_coreBaseSettings.Personal)
            {
                if (_userManager.UserExists(userInfo.Id) && SetupInfo.IsSecretEmail(userInfo.Email))
                {
                    try
                    {
                        _securityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                        _userManager.DeleteUser(userInfo.Id);
                        userInfo = Constants.LostUser;
                    }
                    finally
                    {
                        _securityContext.Logout();
                    }
                }

                if (!_userManager.UserExists(userInfo.Id))
                {
                    userInfo = JoinByThirdPartyAccount(loginProfile);

                    isNew = true;
                }
            }

            if (isNew)
            {
                //TODO:
                //var spam = HttpContext.Current.Request["spam"];
                //if (spam != "on")
                //{
                //    try
                //    {
                //        const string _databaseID = "com";
                //        using (var db = DbManager.FromHttpContext(_databaseID))
                //        {
                //            db.ExecuteNonQuery(new SqlInsert("template_unsubscribe", false)
                //                                   .InColumnValue("email", userInfo.Email.ToLowerInvariant())
                //                                   .InColumnValue("reason", "personal")
                //                );
                //            Log.Debug(string.Format("Write to template_unsubscribe {0}", userInfo.Email.ToLowerInvariant()));
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Debug(string.Format("ERROR write to template_unsubscribe {0}, email:{1}", ex.Message, userInfo.Email.ToLowerInvariant()));
                //    }
                //}

                _studioNotifyService.UserHasJoin();
                _userHelpTourHelper.IsNewUser = true;
                _personalSettingsHelper.IsNewUser = true;
            }

            return userInfo;
        }
        catch (Exception)
        {
            _cookiesManager.ClearCookies(CookiesType.AuthKey);
            _cookiesManager.ClearCookies(CookiesType.SocketIO);
            _securityContext.Logout();
            throw;
        }
    }

    private UserInfo JoinByThirdPartyAccount(LoginProfile loginProfile)
    {
        if (string.IsNullOrEmpty(loginProfile.EMail))
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }

        var userInfo = _userManager.GetUserByEmail(loginProfile.EMail);
        if (!_userManager.UserExists(userInfo.Id))
        {
            var newUserInfo = ProfileToUserInfo(loginProfile);

            try
            {
                _securityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                userInfo = _userManagerWrapper.AddUser(newUserInfo, UserManagerWrapper.GeneratePassword());
            }
            finally
            {
                _securityContext.Logout();
            }
        }

        var linker = _accountLinker.Get("webstudio");
        linker.AddLink(userInfo.Id.ToString(), loginProfile);

        return userInfo;
    }

    private UserInfo ProfileToUserInfo(LoginProfile loginProfile)
    {
        if (string.IsNullOrEmpty(loginProfile.EMail)) throw new Exception(Resource.ErrorNotCorrectEmail);

        var firstName = loginProfile.FirstName;
        if (string.IsNullOrEmpty(firstName)) firstName = loginProfile.DisplayName;

        var userInfo = new UserInfo
        {
            FirstName = string.IsNullOrEmpty(firstName) ? UserControlsCommonResource.UnknownFirstName : firstName,
            LastName = string.IsNullOrEmpty(loginProfile.LastName) ? UserControlsCommonResource.UnknownLastName : loginProfile.LastName,
            Email = loginProfile.EMail,
            Title = string.Empty,
            Location = string.Empty,
            CultureName = _coreBaseSettings.CustomMode ? "ru-RU" : Thread.CurrentThread.CurrentUICulture.Name,
            ActivationStatus = EmployeeActivationStatus.Activated,
        };

        var gender = loginProfile.Gender;
        if (!string.IsNullOrEmpty(gender))
        {
            userInfo.Sex = gender == "male";
        }

        return userInfo;
    }

    private bool TryGetUserByHash(string hashId, out Guid userId)
    {
        userId = Guid.Empty;
        if (string.IsNullOrEmpty(hashId)) return false;

        var linkedProfiles = _accountLinker.Get("webstudio").GetLinkedObjectsByHashId(hashId);
        var tmp = Guid.Empty;
        if (linkedProfiles.Any(profileId => Guid.TryParse(profileId, out tmp) && _userManager.UserExists(tmp)))
            userId = tmp;
        return true;
    }
}