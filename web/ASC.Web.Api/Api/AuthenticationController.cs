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

using AuthenticationException = System.Security.Authentication.AuthenticationException;
using Constants = ASC.Core.Users.Constants;

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
    private readonly SetupInfo _setupInfo;
    private readonly MessageService _messageService;
    private readonly ProviderManager _providerManager;
    private readonly AccountLinker _accountLinker;
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
    private readonly CookieStorage _cookieStorage;
    private readonly DbLoginEventsManager _dbLoginEventsManager;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly TfaAppAuthSettingsHelper _tfaAppAuthSettingsHelper;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly BruteForceLoginManager _bruteForceLoginManager;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly InvitationLinkService _invitationLinkService;

    public AuthenticationController(
        UserManager userManager,
        TenantManager tenantManager,
        SecurityContext securityContext,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        CookiesManager cookiesManager,
        PasswordHasher passwordHasher,
        EmailValidationKeyModelHelper emailValidationKeyModelHelper,
        SetupInfo setupInfo,
        MessageService messageService,
        ProviderManager providerManager,
        AccountLinker accountLinker,
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
        AuthContext authContext,
        CookieStorage cookieStorage,
        DbLoginEventsManager dbLoginEventsManager,
        BruteForceLoginManager bruteForceLoginManager,
        TfaAppAuthSettingsHelper tfaAppAuthSettingsHelper,
        EmailValidationKeyProvider emailValidationKeyProvider,
        ILogger<AuthenticationController> logger,
        InvitationLinkService invitationLinkService)
    {
        _userManager = userManager;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _tenantCookieSettingsHelper = tenantCookieSettingsHelper;
        _cookiesManager = cookiesManager;
        _passwordHasher = passwordHasher;
        _emailValidationKeyModelHelper = emailValidationKeyModelHelper;
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
        _cookieStorage = cookieStorage;
        _dbLoginEventsManager = dbLoginEventsManager;
        _userManagerWrapper = userManagerWrapper;
        _bruteForceLoginManager = bruteForceLoginManager;
        _tfaAppAuthSettingsHelper = tfaAppAuthSettingsHelper;
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _logger = logger;
        _invitationLinkService = invitationLinkService;
    }

    [AllowNotPayment]
    [HttpGet]
    public bool GetIsAuthentificated()
    {
        return _securityContext.IsAuthenticated;
    }

    [AllowNotPayment]
    [HttpPost("{code}", Order = 1)]
    public async Task<AuthenticationTokenDto> AuthenticateMeFromBodyWithCode(AuthRequestsDto inDto)
    {
        var tenant = _tenantManager.GetCurrentTenant().Id;
        var user = (await GetUser(inDto)).UserInfo;
        var sms = false;

        try
        {
            if (_studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettings() && _studioSmsNotificationSettingsHelper.TfaEnabledForUser(user.Id))
            {
                sms = true;
                await _smsManager.ValidateSmsCode(user, inDto.Code, true);
            }
            else if (_tfaAppAuthSettingsHelper.IsVisibleSettings && _tfaAppAuthSettingsHelper.TfaEnabledForUser(user.Id))
            {
                if (_tfaManager.ValidateAuthCode(user, inDto.Code, true, true))
                {
                    _messageService.Send(MessageAction.UserConnectedTfaApp, _messageTarget.Create(user.Id));
                }
            }
            else
            {
                throw new SecurityException("Auth code is not available");
            }

            var token = _cookiesManager.AuthenticateMeAndSetCookies(user.Tenant, user.Id, MessageAction.LoginSuccess);
            var expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);

            var result = new AuthenticationTokenDto
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
        catch (Exception ex)
        {
            _messageService.Send(user.DisplayUserName(false, _displayUserSettingsHelper), sms
                                                                          ? MessageAction.LoginFailViaApiSms
                                                                          : MessageAction.LoginFailViaApiTfa,
                                _messageTarget.Create(user.Id));
            _logger.ErrorWithException(ex);
            throw new AuthenticationException("User authentication failed");
        }
        finally
        {
            _securityContext.Logout();
        }
    }

    [AllowNotPayment]
    [HttpPost]
    public async Task<AuthenticationTokenDto> AuthenticateMeAsync(AuthRequestsDto inDto)
    {
        var wrapper = await GetUser(inDto);
        var viaEmail = wrapper.ViaEmail;
        var user = wrapper.UserInfo;
        var session = inDto.Session;

        if (user == null || Equals(user, Constants.LostUser))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (_studioSmsNotificationSettingsHelper.IsVisibleAndAvailableSettings() && _studioSmsNotificationSettingsHelper.TfaEnabledForUser(user.Id))
        {
            if (string.IsNullOrEmpty(user.MobilePhone) || user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
            {
                return new AuthenticationTokenDto
                {
                    Sms = true,
                    ConfirmUrl = _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.PhoneActivation)
                };
            }

            await _smsManager.PutAuthCodeAsync(user, false);

            return new AuthenticationTokenDto
            {
                Sms = true,
                PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
                Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval)),
                ConfirmUrl = _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.PhoneAuth)
            };
        }

        if (_tfaAppAuthSettingsHelper.IsVisibleSettings && _tfaAppAuthSettingsHelper.TfaEnabledForUser(user.Id))
        {
            if (!TfaAppUserSettings.EnableForUser(_settingsManager, user.Id))
            {
                return new AuthenticationTokenDto
                {
                    Tfa = true,
                    TfaKey = _tfaManager.GenerateSetupCode(user).ManualEntryKey,
                    ConfirmUrl = _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.TfaActivation)
                };
            }

            return new AuthenticationTokenDto
            {
                Tfa = true,
                ConfirmUrl = _commonLinkUtility.GetConfirmationEmailUrl(user.Email, ConfirmType.TfaAuth)
            };
        }

        try
        {
            var action = viaEmail ? MessageAction.LoginSuccessViaApi : MessageAction.LoginSuccessViaApiSocialAccount;
            var token = _cookiesManager.AuthenticateMeAndSetCookies(user.Tenant, user.Id, action, session);

            var outDto = new AuthenticationTokenDto
            {
                Token = token
            };

            if (!session)
            {
                var tenant = _tenantManager.GetCurrentTenant().Id;
                var expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);

                outDto.Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, expires);
            }

            return outDto;
        }
        catch (Exception ex)
        {
            _messageService.Send(user.DisplayUserName(false, _displayUserSettingsHelper), viaEmail ? MessageAction.LoginFailViaApi : MessageAction.LoginFailViaApiSocialAccount);
            _logger.ErrorWithException(ex);
            throw new AuthenticationException("User authentication failed");
        }
        finally
        {
            _securityContext.Logout();
        }
    }

    [AllowNotPayment]
    [HttpPost("logout")]
    [HttpGet("logout")]// temp fix
    public async Task Logout()
    {
        var cookie = _cookiesManager.GetCookies(CookiesType.AuthKey);
        var loginEventId = _cookieStorage.GetLoginEventIdFromCookie(cookie);
        await _dbLoginEventsManager.LogOutEvent(loginEventId);

        var user = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        var loginName = user.DisplayUserName(false, _displayUserSettingsHelper);
        _messageService.Send(loginName, MessageAction.Logout);

        _cookiesManager.ClearCookies(CookiesType.AuthKey);
        _cookiesManager.ClearCookies(CookiesType.SocketIO);

        _securityContext.Logout();
    }

    [AllowNotPayment, AllowSuspended]
    [HttpPost("confirm")]
    public async Task<ValidationResult> CheckConfirm(EmailValidationKeyModel inDto)
    {
        if (inDto.Type != ConfirmType.LinkInvite)
        {
            return _emailValidationKeyModelHelper.Validate(inDto);
        }

        var linkData = await _invitationLinkService.GetProcessedLinkDataAsync(inDto.Key, inDto.Email, inDto.EmplType ?? default, inDto.UiD ?? default);

        return linkData.Result;
    }

    [AllowNotPayment]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PhoneActivation")]
    [HttpPost("setphone")]
    public async Task<AuthenticationTokenDto> SaveMobilePhoneAsync(MobileRequestsDto inDto)
    {
        _apiContext.AuthByClaim();
        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        inDto.MobilePhone = await _smsManager.SaveMobilePhoneAsync(user, inDto.MobilePhone);
        _messageService.Send(MessageAction.UserUpdatedMobileNumber, _messageTarget.Create(user.Id), user.DisplayUserName(false, _displayUserSettingsHelper), inDto.MobilePhone);

        return new AuthenticationTokenDto
        {
            Sms = true,
            PhoneNoise = SmsSender.BuildPhoneNoise(inDto.MobilePhone),
            Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval))
        };
    }

    [AllowNotPayment]
    [HttpPost("sendsms")]
    public async Task<AuthenticationTokenDto> SendSmsCodeAsync(AuthRequestsDto inDto)
    {
        var user = (await GetUser(inDto)).UserInfo;
        await _smsManager.PutAuthCodeAsync(user, true);

        return new AuthenticationTokenDto
        {
            Sms = true,
            PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
            Expires = new ApiDateTime(_tenantManager, _timeZoneConverter, DateTime.UtcNow.Add(_smsKeyStorage.StoreInterval))
        };
    }

    private async Task<UserInfoWrapper> GetUser(AuthRequestsDto inDto)
    {
        var wrapper = new UserInfoWrapper
        {
            ViaEmail = true
        };

        var action = MessageAction.LoginFailViaApi;
        UserInfo user = null;

        try
        {
            if (inDto.ConfirmData != null)
            {
                var email = inDto.ConfirmData.Email;

                var checkKeyResult = _emailValidationKeyProvider.ValidateEmailKey(email + ConfirmType.Auth + inDto.ConfirmData.First + inDto.ConfirmData.Module + inDto.ConfirmData.Sms, inDto.ConfirmData.Key, _setupInfo.ValidAuthKeyInterval);

                if (checkKeyResult == ValidationResult.Ok)
                {
                    user = email.Contains("@")
                                   ? _userManager.GetUserByEmail(email)
                                   : _userManager.GetUsers(new Guid(email));

                    if (_securityContext.IsAuthenticated && _securityContext.CurrentAccount.ID != user.Id)
                    {
                        _securityContext.Logout();
                        _cookiesManager.ClearCookies(CookiesType.AuthKey);
                        _cookiesManager.ClearCookies(CookiesType.SocketIO);
                    }
                }
            }
            else if ((string.IsNullOrEmpty(inDto.Provider) && string.IsNullOrEmpty(inDto.SerializedProfile)) || inDto.Provider == "email")
            {
                inDto.UserName.ThrowIfNull(new ArgumentException(@"userName empty", "userName"));
                if (!string.IsNullOrEmpty(inDto.Password))
                {
                    inDto.Password.ThrowIfNull(new ArgumentException(@"password empty", "password"));
                }
                else
                {
                    inDto.PasswordHash.ThrowIfNull(new ArgumentException(@"PasswordHash empty", "PasswordHash"));
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

                var requestIp = MessageSettings.GetIP(Request);

                user = _bruteForceLoginManager.Attempt(inDto.UserName, inDto.PasswordHash, requestIp, out _);
            }
            else
            {
                if (!(_coreBaseSettings.Standalone || _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).Oauth))
                {
                    throw new Exception(Resource.ErrorNotAllowedOption);
                }
                wrapper.ViaEmail = false;
                action = MessageAction.LoginFailViaApiSocialAccount;
                LoginProfile thirdPartyProfile;
                if (!string.IsNullOrEmpty(inDto.SerializedProfile))
                {
                    thirdPartyProfile = new LoginProfile(_signature, _instanceCrypto, inDto.SerializedProfile);
                }
                else
                {
                    thirdPartyProfile = _providerManager.GetLoginProfile(inDto.Provider, inDto.AccessToken, inDto.CodeOAuth);
                }

                inDto.UserName = thirdPartyProfile.EMail;

                user = await GetUserByThirdParty(thirdPartyProfile);
            }
        }
        catch (BruteForceCredentialException)
        {
            _messageService.Send(!string.IsNullOrEmpty(inDto.UserName) ? inDto.UserName : AuditResource.EmailNotSpecified, MessageAction.LoginFailBruteForce);
            throw new AuthenticationException("Login Fail. Too many attempts");
        }
        catch (Exception ex)
        {
            _messageService.Send(!string.IsNullOrEmpty(inDto.UserName) ? inDto.UserName : AuditResource.EmailNotSpecified, action);
            _logger.ErrorWithException(ex);
            throw new AuthenticationException("User authentication failed");
        }
        wrapper.UserInfo = user;
        return wrapper;
    }

    private async Task<UserInfo> GetUserByThirdParty(LoginProfile loginProfile)
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
                        await _userManager.DeleteUser(userInfo.Id);
                        userInfo = Constants.LostUser;
                    }
                    finally
                    {
                        _securityContext.Logout();
                    }
                }

                if (!_userManager.UserExists(userInfo.Id))
                {
                    userInfo = await JoinByThirdPartyAccount(loginProfile);

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

    private async Task<UserInfo> JoinByThirdPartyAccount(LoginProfile loginProfile)
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
                userInfo = await _userManagerWrapper.AddUser(newUserInfo, UserManagerWrapper.GeneratePassword());
            }
            finally
            {
                _securityContext.Logout();
            }
        }

        _accountLinker.AddLink(userInfo.Id.ToString(), loginProfile);

        return userInfo;
    }

    private UserInfo ProfileToUserInfo(LoginProfile loginProfile)
    {
        if (string.IsNullOrEmpty(loginProfile.EMail))
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }

        var firstName = loginProfile.FirstName;
        if (string.IsNullOrEmpty(firstName))
        {
            firstName = loginProfile.DisplayName;
        }

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
        if (string.IsNullOrEmpty(hashId))
        {
            return false;
        }

        var linkedProfiles = _accountLinker.GetLinkedObjectsByHashId(hashId);
        var tmp = Guid.Empty;
        if (linkedProfiles.Any(profileId => Guid.TryParse(profileId, out tmp) && _userManager.UserExists(tmp)))
        {
            userId = tmp;
        }

        return true;
    }
}

class UserInfoWrapper
{
    public UserInfo UserInfo { get; set; }
    public bool ViaEmail { get; set; }
}