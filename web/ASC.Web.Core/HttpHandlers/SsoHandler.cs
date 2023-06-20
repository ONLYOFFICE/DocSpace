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

using Constants = ASC.Core.Users.Constants;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Web.Core.HttpHandlers;
public class SsoHandler
{
    public SsoHandler(RequestDelegate next)
    {
    }

    public async Task Invoke(HttpContext context, SsoHandlerService ssoHandlerService)
    {
        await ssoHandlerService.InvokeAsync(context).ConfigureAwait(false);
    }

}

[Scope]
public class SsoHandlerService
{
    private readonly ILogger<SsoHandlerService> _log;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly CookiesManager _cookiesManager;
    private readonly Signature _signature;
    private readonly SecurityContext _securityContext;
    private readonly UserFormatter _userFormatter;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly MessageService _messageService;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly TenantUtil _tenantUtil;
    private readonly Action<string> _signatureResolver;
    private readonly CountPaidUserChecker _countPaidUserChecker;
    private const string MOB_PHONE = "mobphone";
    private const string EXT_MOB_PHONE = "extmobphone";

    private const int MAX_NUMBER_OF_SYMBOLS = 64;


    public SsoHandlerService(
        ILogger<SsoHandlerService> log,
        CoreBaseSettings coreBaseSettings,
        UserManager userManager,
        TenantManager tenantManager,
        SettingsManager settingsManager,
        CommonLinkUtility commonLinkUtility,
        CookiesManager cookiesManager,
        Signature signature,
        SecurityContext securityContext,
        UserFormatter userFormatter,
        UserManagerWrapper userManagerWrapper,
        MessageService messageService,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        TenantUtil tenantUtil,
        CountPaidUserChecker countPaidUserChecker)
    {
        _log = log;
        _coreBaseSettings = coreBaseSettings;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _commonLinkUtility = commonLinkUtility;
        _cookiesManager = cookiesManager;
        _signature = signature;
        _securityContext = securityContext;
        _userFormatter = userFormatter;
        _userManagerWrapper = userManagerWrapper;
        _messageService = messageService;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _tenantUtil = tenantUtil;
        _countPaidUserChecker = countPaidUserChecker;
        _signatureResolver = signature =>
        {
            int.TryParse(signature.Substring(signature.Length - 1), out var lastSignChar);
            signature = signature.Remove(signature.Length - 1);

            while (lastSignChar > 0)
            {
                signature = signature + "=";
                lastSignChar--;
            }
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (!SetupInfo.IsVisibleSettings(ManagementType.SingleSignOnSettings.ToString()) && !_coreBaseSettings.Standalone)
            {
                throw new SSOException("Single sign-on settings are disabled", MessageKey.SsoSettingsDisabled);
            }

            if (!(_coreBaseSettings.Standalone || (await _tenantManager.GetTenantQuotaAsync(await _tenantManager.GetCurrentTenantIdAsync())).Sso))
            {
                throw new SSOException("Single sign-on settings are not paid", MessageKey.ErrorNotAllowedOption);
            }

            var settings = await _settingsManager.LoadAsync<SsoSettingsV2>();

            if (context.Request.Query["config"] == "saml")
            {
                context.Response.StatusCode = 200;
                var signedSettings = _signature.Create(settings);
                var ssoConfig = JsonSerializer.Serialize(signedSettings);
                await context.Response.WriteAsync(ssoConfig.Replace("\"", ""));
                return;
            }

            if (!settings.EnableSso)
            {
                throw new SSOException("Single sign-on is disabled", MessageKey.SsoSettingsDisabled);
            }

            var data = context.Request.Query["data"];

            if (string.IsNullOrEmpty(data))
            {
                throw new SSOException("SAML response is null or empty", MessageKey.SsoSettingsEmptyToken);
            }

            if (context.Request.Query["auth"] == "true")
            {
                var userData = _signature.Read<SsoUserData>(data, _signatureResolver);

                if (userData == null)
                {
                    await _messageService.SendAsync(MessageAction.LoginFailViaSSO);
                    throw new SSOException("SAML response is not valid", MessageKey.SsoSettingsNotValidToken);
                }

                var userInfo = await ToUserInfoAsync(userData, true);

                if (Equals(userInfo, Constants.LostUser))
                {
                    throw new SSOException("Can't create userInfo using current SAML response (fields Email, FirstName, LastName are required)", MessageKey.SsoSettingsCantCreateUser);
                }

                if (userInfo.Status == EmployeeStatus.Terminated)
                {
                    throw new SSOException("Current user is terminated", MessageKey.SsoSettingsUserTerminated);
                }

                if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
                {
                    var authenticatedUserInfo = await _userManager.GetUsersAsync(((IUserAccount)context.User.Identity).ID);

                    if (!Equals(userInfo, authenticatedUserInfo))
                    {
                        var loginName = authenticatedUserInfo.DisplayUserName(false, _displayUserSettingsHelper);
                        await _messageService.SendAsync(loginName, MessageAction.Logout);
                        await _cookiesManager.ResetUserCookieAsync();
                        _securityContext.Logout();
                    }
                    else
                    {
                        _log.DebugUserAlreadyAuthenticated(context.User.Identity);
                    }
                }
                try
                {
                    userInfo = await AddUserAsync(userInfo);
                }
                catch(Exception ex)
                {
                    _log.WarningWithException("Failed to save user", ex);
                }

                var authKey = await _cookiesManager.AuthenticateMeAndSetCookiesAsync(userInfo.TenantId, userInfo.Id, MessageAction.LoginSuccessViaSSO);

                context.Response.Redirect(_commonLinkUtility.GetDefault() + "?token=" + HttpUtility.UrlEncode(authKey), false);

            }
            else if (context.Request.Query["logout"] == "true")
            {
                var logoutSsoUserData = _signature.Read<LogoutSsoUserData>(data, _signatureResolver);

                if (logoutSsoUserData == null)
                {
                    throw new SSOException("SAML Logout response is not valid", MessageKey.SsoSettingsNotValidToken);
                }

                var userInfo = await _userManager.GetSsoUserByNameIdAsync(logoutSsoUserData.NameId);

                if (Equals(userInfo, Constants.LostUser))
                {
                    await _messageService.SendAsync(MessageAction.LoginFailViaSSO);
                    throw new SSOException("Can't logout userInfo using current SAML response", MessageKey.SsoSettingsNotValidToken);
                }

                if (userInfo.Status == EmployeeStatus.Terminated)
                {
                    throw new SSOException("Current user is terminated", MessageKey.SsoSettingsUserTerminated);
                }

                await _securityContext.AuthenticateMeWithoutCookieAsync(userInfo.Id);

                var loginName = userInfo.DisplayUserName(false, _displayUserSettingsHelper);
                await _messageService.SendAsync(loginName, MessageAction.Logout);

                await _cookiesManager.ResetUserCookieAsync();
                _securityContext.Logout();
            }
        }
        catch (SSOException e)
        {
            _log.ErrorWithException(e);
            RedirectToLogin(context, (int)e.MessageKey);
        }
        catch (Exception e)
        {
            _log.ErrorWithException(e);
            RedirectToLogin(context, (int)MessageKey.Error);
        }
        finally
        {
            await context.Response.CompleteAsync();
            //context.ApplicationInstance.CompleteRequest();
        }
    }
    private void RedirectToLogin(HttpContext context, int messageKey)
    {
        context.Response.Redirect("/login/error?messageKey=" + messageKey, false);
    }

    //TODO
    private async Task WriteErrorToResponse(HttpContext context, MessageKey messageKey)
    {
         context.Response.StatusCode = 500;
         context.Response.ContentType = "text/plain";
         await context.Response.WriteAsync(((int)messageKey).ToString());
    }

    private async Task<UserInfo> AddUserAsync(UserInfo userInfo)
    {
        UserInfo newUserInfo;

        try
        {
            newUserInfo = userInfo.Clone() as UserInfo;

            if (newUserInfo == null)
            {
                return Constants.LostUser;
            }

            _log.DebugAddingOrUpdatingUser(userInfo.Id);

            await _securityContext.AuthenticateMeWithoutCookieAsync(ASC.Core.Configuration.Constants.CoreSystem);

            if (string.IsNullOrEmpty(newUserInfo.UserName))
            {
                var type = EmployeeType.RoomAdmin;

                try
                {
                    await _countPaidUserChecker.CheckAppend();
                }
                catch (Exception)
                {
                    type = EmployeeType.User;
                }

                newUserInfo = await _userManagerWrapper.AddUserAsync(newUserInfo, UserManagerWrapper.GeneratePassword(), true,
                  false, type);
            }
            else
            {
                if (!_userFormatter.IsValidUserName(userInfo.FirstName, userInfo.LastName))
                {
                    throw new Exception(Resource.ErrorIncorrectUserName);
                }

                await _userManager.UpdateUserInfoAsync(newUserInfo);
            }

            /*var photoUrl = samlResponse.GetRemotePhotoUrl();
            if (!string.IsNullOrEmpty(photoUrl))
            {
                var photoLoader = new UserPhotoLoader();
                photoLoader.SaveOrUpdatePhoto(photoUrl, userInfo.ID);
            }*/
        }
        finally
        {
            _securityContext.Logout();
        }

        return newUserInfo;

    }

    private async Task<UserInfo> ToUserInfoAsync(SsoUserData UserData, bool checkExistance = false)
    {
        var firstName = TrimToLimit(UserData.FirstName);
        var lastName = TrimToLimit(UserData.LastName);
        var email = UserData.Email;
        var nameId = UserData.NameId;
        var sessionId = UserData.SessionId;
        var location = UserData.Location;
        var title = UserData.Title;
        var phone = UserData.Phone;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            return Constants.LostUser;
        }

        var userInfo = Constants.LostUser;

        if (checkExistance)
        {
            userInfo = await _userManager.GetSsoUserByNameIdAsync(nameId);

            if (Equals(userInfo, Constants.LostUser))
            {
                userInfo = await _userManager.GetUserByEmailAsync(email);
            }
        }

        if (Equals(userInfo, Constants.LostUser))
        {
            userInfo = new UserInfo
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                SsoNameId = nameId,
                SsoSessionId = sessionId,
                Location = location,
                Title = title,
                ActivationStatus = EmployeeActivationStatus.NotActivated,
                WorkFromDate = _tenantUtil.DateTimeNow()
            };

            if (string.IsNullOrEmpty(phone))
            {
                return userInfo;
            }

            var contacts = new List<string> { EXT_MOB_PHONE, phone };
            userInfo.ContactsList = contacts;
        }
        else
        {
            userInfo.Email = email;
            userInfo.FirstName = firstName;
            userInfo.LastName = lastName;
            userInfo.SsoNameId = nameId;
            userInfo.SsoSessionId = sessionId;
            userInfo.Location = location;
            userInfo.Title = title;

            var portalUserContacts = userInfo.ContactsList == null ? new List<string>() : userInfo.ContactsList;

            var newContacts = new List<string>();
            var phones = new List<string>();
            var otherContacts = new List<string>();

            for (int i = 0, n = portalUserContacts.Count; i < n; i += 2)
            {
                if (i + 1 >= portalUserContacts.Count)
                {
                    continue;
                }

                var type = portalUserContacts[i];
                var value = portalUserContacts[i + 1];

                switch (type)
                {
                    case EXT_MOB_PHONE:
                        break;
                    case MOB_PHONE:
                        phones.Add(value);
                        break;
                    default:
                        otherContacts.Add(type);
                        otherContacts.Add(value);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(phone))
            {
                if (phones.Exists(p => p.Equals(phone)))
                {
                    phones.Remove(phone);
                }

                newContacts.Add(EXT_MOB_PHONE);
                newContacts.Add(phone);
            }

            phones.ForEach(p =>
            {
                newContacts.Add(MOB_PHONE);
                newContacts.Add(p);
            });

            newContacts.AddRange(otherContacts);

            userInfo.ContactsList = newContacts;
        }

        return userInfo;
    }

    private static string TrimToLimit(string str, int limit = MAX_NUMBER_OF_SYMBOLS)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        var newStr = str.Trim();

        return newStr.Length > limit
                ? newStr.Substring(0, MAX_NUMBER_OF_SYMBOLS)
                : newStr;
    }
}
public enum MessageKey
{
    None,
    Error,
    ErrorUserNotFound,
    ErrorExpiredActivationLink,
    ErrorInvalidActivationLink,
    ErrorConfirmURLError,
    ErrorNotCorrectEmail,
    LoginWithBruteForce,
    RecaptchaInvalid,
    LoginWithAccountNotFound,
    InvalidUsernameOrPassword,
    SsoSettingsDisabled,
    ErrorNotAllowedOption,
    SsoSettingsEmptyToken,
    SsoSettingsNotValidToken,
    SsoSettingsCantCreateUser,
    SsoSettingsUserTerminated,
    SsoError,
    SsoAuthFailed,
    SsoAttributesNotFound,
}

public class SSOException : Exception
{
    public MessageKey MessageKey { get; }

    public SSOException(string message, MessageKey messageKey) : base(message)
    {
        MessageKey = messageKey;
    }
}

public static class SsoHandlerExtensions
{
    public static IApplicationBuilder UseSsoHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SsoHandler>();
    }
}
