using AuthenticationException = System.Security.Authentication.AuthenticationException;
using Constants = ASC.Core.Users.Constants;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Api.Controllers.Authentication;

[Scope]
[DefaultRoute]
[ApiController]
[AllowAnonymous]
[ControllerName("authentication")]
public class BaseAuthenticationController : ControllerBase
{
    internal readonly UserManager _userManager;
    internal readonly TenantManager _tenantManager;
    internal readonly SecurityContext _securityContext;
    internal readonly TenantCookieSettingsHelper _tenantCookieSettingsHelper;
    internal readonly CookiesManager _cookiesManager;
    internal readonly PasswordHasher _passwordHasher;
    internal readonly EmailValidationKeyModelHelper _emailValidationKeyModelHelper;
    internal readonly ICache _cache;
    internal readonly SetupInfo _setupInfo;
    internal readonly MessageService _messageService;
    internal readonly ProviderManager _providerManager;
    internal readonly IOptionsSnapshot<AccountLinker> _accountLinker;
    internal readonly CoreBaseSettings _coreBaseSettings;
    internal readonly PersonalSettingsHelper _personalSettingsHelper;
    internal readonly StudioNotifyService _studioNotifyService;
    internal readonly UserHelpTourHelper _userHelpTourHelper;
    internal readonly Signature _signature;
    internal readonly InstanceCrypto _instanceCrypto;
    internal readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    internal readonly MessageTarget _messageTarget;
    internal readonly StudioSmsNotificationSettingsHelper _studioSmsNotificationSettingsHelper;
    internal readonly SettingsManager _settingsManager;
    internal readonly SmsManager _smsManager;
    internal readonly TfaManager _tfaManager;
    internal readonly TimeZoneConverter _timeZoneConverter;
    internal readonly SmsKeyStorage _smsKeyStorage;
    internal readonly CommonLinkUtility _commonLinkUtility;
    internal readonly ApiContext _apiContext;
    internal readonly AuthContext _authContext;
    internal readonly UserManagerWrapper _userManagerWrapper;

    public BaseAuthenticationController(
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

    internal UserInfo GetUser(AuthDto memberModel, out bool viaEmail)
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
                    _tenantManager.GetCurrentTenant().TenantId,
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
                if (_userManager.UserExists(userInfo.ID) && SetupInfo.IsSecretEmail(userInfo.Email))
                {
                    try
                    {
                        _securityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                        _userManager.DeleteUser(userInfo.ID);
                        userInfo = Constants.LostUser;
                    }
                    finally
                    {
                        _securityContext.Logout();
                    }
                }

                if (!_userManager.UserExists(userInfo.ID))
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
        if (!_userManager.UserExists(userInfo.ID))
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
        linker.AddLink(userInfo.ID.ToString(), loginProfile);

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

public class AuthenticationTokenData
{
    public string Token { get; set; }

    public DateTime Expires { get; set; }

    public bool Sms { get; set; }

    public string PhoneNoise { get; set; }

    public bool Tfa { get; set; }

    public string TfaKey { get; set; }

    public string ConfirmUrl { get; set; }

    public static AuthenticationTokenData GetSample()
    {
        return new AuthenticationTokenData
        {
            Expires = DateTime.UtcNow,
            Token = "abcde12345",
            Sms = false,
            PhoneNoise = null,
            Tfa = false,
            TfaKey = null
        };
    }
}