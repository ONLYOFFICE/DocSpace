using System;
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Security;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.FederatedLogin;
using ASC.FederatedLogin.LoginProviders;
using ASC.FederatedLogin.Profile;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Api.Core;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Sms;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.Core.TFA;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using static ASC.Security.Cryptography.EmailValidationKeyProvider;

namespace ASC.Web.Api.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private SecurityContext SecurityContext { get; }
        private TenantCookieSettingsHelper TenantCookieSettingsHelper { get; }
        private CookiesManager CookiesManager { get; }
        private PasswordHasher PasswordHasher { get; }
        private EmailValidationKeyModelHelper EmailValidationKeyModelHelper { get; }
        private ICache Cache { get; }
        private SetupInfo SetupInfo { get; }
        private MessageService MessageService { get; }
        private ProviderManager ProviderManager { get; }
        private IOptionsSnapshot<AccountLinker> AccountLinker { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private PersonalSettingsHelper PersonalSettingsHelper { get; }
        private StudioNotifyService StudioNotifyService { get; }
        private UserHelpTourHelper UserHelpTourHelper { get; }
        private Signature Signature { get; }
        private InstanceCrypto InstanceCrypto { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private MessageTarget MessageTarget { get; }
        private StudioSmsNotificationSettingsHelper StudioSmsNotificationSettingsHelper { get; }
        private SettingsManager SettingsManager { get; }
        private SmsManager SmsManager { get; }
        private TfaManager TfaManager { get; }
        private TimeZoneConverter TimeZoneConverter { get; }
        private SmsKeyStorage SmsKeyStorage { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private ApiContext ApiContext { get; }
        private AuthContext AuthContext { get; }
        private UserManagerWrapper UserManagerWrapper { get; }

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
            UserManager = userManager;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            TenantCookieSettingsHelper = tenantCookieSettingsHelper;
            CookiesManager = cookiesManager;
            PasswordHasher = passwordHasher;
            EmailValidationKeyModelHelper = emailValidationKeyModelHelper;
            Cache = cache;
            SetupInfo = setupInfo;
            MessageService = messageService;
            ProviderManager = providerManager;
            AccountLinker = accountLinker;
            CoreBaseSettings = coreBaseSettings;
            PersonalSettingsHelper = personalSettingsHelper;
            StudioNotifyService = studioNotifyService;
            UserHelpTourHelper = userHelpTourHelper;
            Signature = signature;
            InstanceCrypto = instanceCrypto;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            MessageTarget = messageTarget;
            StudioSmsNotificationSettingsHelper = studioSmsNotificationSettingsHelper;
            SettingsManager = settingsManager;
            SmsManager = smsManager;
            TfaManager = tfaManager;
            TimeZoneConverter = timeZoneConverter;
            SmsKeyStorage = smsKeyStorage;
            CommonLinkUtility = commonLinkUtility;
            ApiContext = apiContext;
            AuthContext = authContext;
            UserManagerWrapper = userManagerWrapper;
        }


        [Read]
        public bool GetIsAuthentificated()
        {
            return SecurityContext.IsAuthenticated;
        }

        [Create("{code}", false, order: int.MaxValue)]
        public AuthenticationTokenData AuthenticateMeFromBodyWithCode([FromBody] AuthModel auth)
        {
            return AuthenticateMeWithCode(auth);
        }

        [Create("{code}", false, order: int.MaxValue)]
        [Consumes("application/x-www-form-urlencoded")]
        public AuthenticationTokenData AuthenticateMeFromFormWithCode([FromForm] AuthModel auth)
        {
            return AuthenticateMeWithCode(auth);
        }

        [Create(false)]
        public Task<AuthenticationTokenData> AuthenticateMeFromBodyAsync([FromBody] AuthModel auth)
        {
            return AuthenticateMeAsync(auth);
        }

        [Create(false)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<AuthenticationTokenData> AuthenticateMeFromFormAsync([FromForm] AuthModel auth)
        {
            return AuthenticateMeAsync(auth);
        }

        [Create("logout")]
        [Read("logout")]// temp fix
        public void Logout()
        {
            if (SecurityContext.IsAuthenticated)
                CookiesManager.ResetUserCookie(SecurityContext.CurrentAccount.ID);

            CookiesManager.ClearCookies(CookiesType.AuthKey);
            CookiesManager.ClearCookies(CookiesType.SocketIO);

            SecurityContext.Logout();
        }

        [Create("confirm", false)]
        public ValidationResult CheckConfirmFromBody([FromBody] EmailValidationKeyModel model)
        {
            return EmailValidationKeyModelHelper.Validate(model);
        }

        [Create("confirm", false)]
        [Consumes("application/x-www-form-urlencoded")]
        public ValidationResult CheckConfirmFromForm([FromForm] EmailValidationKeyModel model)
        {
            return EmailValidationKeyModelHelper.Validate(model);
        }

        [Authorize(AuthenticationSchemes = "confirm", Roles = "PhoneActivation")]
        [Create("setphone", false)]
        public Task<AuthenticationTokenData> SaveMobilePhoneFromBodyAsync([FromBody] MobileModel model)
        {
            return SaveMobilePhoneAsync(model);
        }

        [Authorize(AuthenticationSchemes = "confirm", Roles = "PhoneActivation")]
        [Create("setphone", false)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<AuthenticationTokenData> SaveMobilePhoneFromFormAsync([FromForm] MobileModel model)
        {
            return SaveMobilePhoneAsync(model);
        }       

        private async Task<AuthenticationTokenData> SaveMobilePhoneAsync(MobileModel model)
        {
            ApiContext.AuthByClaim();
            var user = UserManager.GetUsers(AuthContext.CurrentAccount.ID);
            model.MobilePhone = await SmsManager.SaveMobilePhoneAsync(user, model.MobilePhone);
            MessageService.Send(MessageAction.UserUpdatedMobileNumber, MessageTarget.Create(user.ID), user.DisplayUserName(false, DisplayUserSettingsHelper), model.MobilePhone);

            return new AuthenticationTokenData
            {
                Sms = true,
                PhoneNoise = SmsSender.BuildPhoneNoise(model.MobilePhone),
                Expires = new ApiDateTime(TenantManager, TimeZoneConverter, DateTime.UtcNow.Add(SmsKeyStorage.StoreInterval))
            };
        }

        [Create(@"sendsms", false)]
        public Task<AuthenticationTokenData> SendSmsCodeFromBodyAsync([FromBody] AuthModel model)
        {
            return SendSmsCodeAsync(model);
        }

        [Create(@"sendsms", false)]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<AuthenticationTokenData> SendSmsCodeFromFormAsync([FromForm] AuthModel model)
        {
            return SendSmsCodeAsync(model);
        }

        private async Task<AuthenticationTokenData> SendSmsCodeAsync(AuthModel model)
        {
            var user = GetUser(model, out _);
            await SmsManager.PutAuthCodeAsync(user, true);

            return new AuthenticationTokenData
            {
                Sms = true,
                PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
                Expires = new ApiDateTime(TenantManager, TimeZoneConverter, DateTime.UtcNow.Add(SmsKeyStorage.StoreInterval))
            };
        }

        private async Task<AuthenticationTokenData> AuthenticateMeAsync(AuthModel auth)
        {
            bool viaEmail;
            var user = GetUser(auth, out viaEmail);

            if (StudioSmsNotificationSettingsHelper.IsVisibleSettings() && StudioSmsNotificationSettingsHelper.Enable)
            {
                if (string.IsNullOrEmpty(user.MobilePhone) || user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
                    return new AuthenticationTokenData
                    {
                        Sms = true,
                        ConfirmUrl = CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneActivation)
                    };

                await SmsManager.PutAuthCodeAsync(user, false);

                return new AuthenticationTokenData
                {
                    Sms = true,
                    PhoneNoise = SmsSender.BuildPhoneNoise(user.MobilePhone),
                    Expires = new ApiDateTime(TenantManager, TimeZoneConverter, DateTime.UtcNow.Add(SmsKeyStorage.StoreInterval)),
                    ConfirmUrl = CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneAuth)
                };
            }

            if (TfaAppAuthSettings.IsVisibleSettings && SettingsManager.Load<TfaAppAuthSettings>().EnableSetting)
            {
                if (!TfaAppUserSettings.EnableForUser(SettingsManager, user.ID))
                    return new AuthenticationTokenData
                    {
                        Tfa = true,
                        TfaKey = TfaManager.GenerateSetupCode(user).ManualEntryKey,
                        ConfirmUrl = CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaActivation)
                    };

                return new AuthenticationTokenData
                {
                    Tfa = true,
                    ConfirmUrl = CommonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.TfaAuth)
                };
            }

            try
            {
                var token = SecurityContext.AuthenticateMe(user.ID);
                CookiesManager.SetCookies(CookiesType.AuthKey, token, auth.Session);

                MessageService.Send(viaEmail ? MessageAction.LoginSuccessViaApi : MessageAction.LoginSuccessViaApiSocialAccount);

                var tenant = TenantManager.GetCurrentTenant().TenantId;
                var expires = TenantCookieSettingsHelper.GetExpiresTime(tenant);

                return new AuthenticationTokenData
                {
                    Token = token,
                    Expires = new ApiDateTime(TenantManager, TimeZoneConverter, expires)
                };
            }
            catch
            {
                MessageService.Send(user.DisplayUserName(false, DisplayUserSettingsHelper), viaEmail ? MessageAction.LoginFailViaApi : MessageAction.LoginFailViaApiSocialAccount);
                throw new AuthenticationException("User authentication failed");
            }
            finally
            {
                SecurityContext.Logout();
            }
        }

        private AuthenticationTokenData AuthenticateMeWithCode(AuthModel auth)
        {
            var tenant = TenantManager.GetCurrentTenant().TenantId;
            var user = GetUser(auth, out _);

            var sms = false;
            try
            {
                if (StudioSmsNotificationSettingsHelper.IsVisibleSettings() && StudioSmsNotificationSettingsHelper.Enable)
                {
                    sms = true;
                    SmsManager.ValidateSmsCode(user, auth.Code);
                }
                else if (TfaAppAuthSettings.IsVisibleSettings && SettingsManager.Load<TfaAppAuthSettings>().EnableSetting)
                {
                    if (TfaManager.ValidateAuthCode(user, auth.Code))
                    {
                        MessageService.Send(MessageAction.UserConnectedTfaApp, MessageTarget.Create(user.ID));
                    }
                }
                else
                {
                    throw new System.Security.SecurityException("Auth code is not available");
                }

                var token = SecurityContext.AuthenticateMe(user.ID);

                MessageService.Send(sms ? MessageAction.LoginSuccessViaApiSms : MessageAction.LoginSuccessViaApiTfa);
                
                var expires = TenantCookieSettingsHelper.GetExpiresTime(tenant);

                var result = new AuthenticationTokenData
                {
                    Token = token,
                    Expires = new ApiDateTime(TenantManager, TimeZoneConverter, expires)
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
                MessageService.Send(user.DisplayUserName(false, DisplayUserSettingsHelper), sms
                                                                              ? MessageAction.LoginFailViaApiSms
                                                                              : MessageAction.LoginFailViaApiTfa,
                                    MessageTarget.Create(user.ID));
                throw new AuthenticationException("User authentication failed");
            }
            finally
            {
                SecurityContext.Logout();
            }
        }

        private UserInfo GetUser(AuthModel memberModel, out bool viaEmail)
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
                    int.TryParse(Cache.Get<string>("loginsec/" + memberModel.UserName), out counter);
                    if (++counter > SetupInfo.LoginThreshold && !SetupInfo.IsSecretEmail(memberModel.UserName))
                    {
                        throw new BruteForceCredentialException();
                    }
                    Cache.Insert("loginsec/" + memberModel.UserName, counter.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));


                    memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();

                    if (string.IsNullOrEmpty(memberModel.PasswordHash))
                    {
                        memberModel.Password = (memberModel.Password ?? "").Trim();

                        if (!string.IsNullOrEmpty(memberModel.Password))
                        {
                            memberModel.PasswordHash = PasswordHasher.GetClientPassword(memberModel.Password);
                        }
                    }

                    user = UserManager.GetUsersByPasswordHash(
                        TenantManager.GetCurrentTenant().TenantId,
                        memberModel.UserName,
                        memberModel.PasswordHash);

                    if (user == null || !UserManager.UserExists(user))
                    {
                        throw new Exception("user not found");
                    }

                    Cache.Insert("loginsec/" + memberModel.UserName, (--counter).ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));
                }
                else
                {
                    viaEmail = false;
                    action = MessageAction.LoginFailViaApiSocialAccount;
                    LoginProfile thirdPartyProfile;
                    if (!string.IsNullOrEmpty(memberModel.SerializedProfile))
                    {
                        thirdPartyProfile = new LoginProfile(Signature, InstanceCrypto, memberModel.SerializedProfile);
                    }
                    else
                    {
                        thirdPartyProfile = ProviderManager.GetLoginProfile(memberModel.Provider, memberModel.AccessToken);
                    }

                    memberModel.UserName = thirdPartyProfile.EMail;

                    user = GetUserByThirdParty(thirdPartyProfile);
                }
            }
            catch (BruteForceCredentialException)
            {
                MessageService.Send(!string.IsNullOrEmpty(memberModel.UserName) ? memberModel.UserName : AuditResource.EmailNotSpecified, MessageAction.LoginFailBruteForce);
                throw new AuthenticationException("Login Fail. Too many attempts");
            }
            catch
            {
                MessageService.Send(!string.IsNullOrEmpty(memberModel.UserName) ? memberModel.UserName : AuditResource.EmailNotSpecified, action);
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
                    userInfo = UserManager.GetUsers(userId);
                }

                var isNew = false;
                if (CoreBaseSettings.Personal)
                {
                    if (UserManager.UserExists(userInfo.ID) && SetupInfo.IsSecretEmail(userInfo.Email))
                    {
                        try
                        {
                            SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                            UserManager.DeleteUser(userInfo.ID);
                            userInfo = Constants.LostUser;
                        }
                        finally
                        {
                            SecurityContext.Logout();
                        }
                    }

                    if (!UserManager.UserExists(userInfo.ID))
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

                    StudioNotifyService.UserHasJoin();
                    UserHelpTourHelper.IsNewUser = true;
                    PersonalSettingsHelper.IsNewUser = true;
                }

                return userInfo;
            }
            catch (Exception)
            {
                CookiesManager.ClearCookies(CookiesType.AuthKey);
                CookiesManager.ClearCookies(CookiesType.SocketIO);
                SecurityContext.Logout();
                throw;
            }
        }

        private UserInfo JoinByThirdPartyAccount(LoginProfile loginProfile)
        {
            if (string.IsNullOrEmpty(loginProfile.EMail))
            {
                throw new Exception(Resource.ErrorNotCorrectEmail);
            }

            var userInfo = UserManager.GetUserByEmail(loginProfile.EMail);
            if (!UserManager.UserExists(userInfo.ID))
            {
                var newUserInfo = ProfileToUserInfo(loginProfile);

                try
                {
                    SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                    userInfo = UserManagerWrapper.AddUser(newUserInfo, UserManagerWrapper.GeneratePassword());
                }
                finally
                {
                    SecurityContext.Logout();
                }
            }

            var linker = AccountLinker.Get("webstudio");
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
                CultureName = CoreBaseSettings.CustomMode ? "ru-RU" : Thread.CurrentThread.CurrentUICulture.Name,
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

            var linkedProfiles = AccountLinker.Get("webstudio").GetLinkedObjectsByHashId(hashId);
            var tmp = Guid.Empty;
            if (linkedProfiles.Any(profileId => Guid.TryParse(profileId, out tmp) && UserManager.UserExists(tmp)))
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
}