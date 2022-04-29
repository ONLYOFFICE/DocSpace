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
    private readonly RequestDelegate _next;

    public SsoHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, SsoHandlerService ssoHandlerService)
    {
        await ssoHandlerService.InvokeAsync(context).ConfigureAwait(false);
        await _next.Invoke(context).ConfigureAwait(false);
    }

}

[Scope]
public class SsoHandlerService
{
    private readonly ILog _log;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly CookiesManager _cookiesManager;
    private readonly Signature _signature;
    private readonly SecurityContext _securityContext;
    private readonly TenantExtra _tenantExtra;
    private readonly TenantStatisticsProvider _tenantStatisticsProvider;
    private readonly UserFormatter _userFormatter;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly MessageService _messageService;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;


    public SsoHandlerService(
        IOptionsMonitor<ILog> optionsMonitor,
        CoreBaseSettings coreBaseSettings,
        UserManager userManager,
        TenantManager tenantManager,
        SettingsManager settingsManager,
        CookiesManager cookiesManager,
        Signature signature,
        SecurityContext securityContext,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticsProvider,
        UserFormatter userFormatter,
        UserManagerWrapper userManagerWrapper,
        MessageService messageService,
        DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        _log = optionsMonitor.CurrentValue;
        _coreBaseSettings = coreBaseSettings;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _cookiesManager = cookiesManager;
        _signature = signature;
        _securityContext = securityContext;
        _tenantExtra = tenantExtra;
        _tenantStatisticsProvider = tenantStatisticsProvider;
        _userFormatter = userFormatter;
        _userManagerWrapper = userManagerWrapper;
        _messageService = messageService;
        _displayUserSettingsHelper = displayUserSettingsHelper;

    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (!SetupInfo.IsVisibleSettings(ManagementType.SingleSignOnSettings.ToString()) && !_coreBaseSettings.Standalone)
            {
                await WriteErrorToResponse(context, "Single sign-on settings are disabled");
                return;
            }
            if (!(_coreBaseSettings.Standalone || _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).Sso))
            {
                await WriteErrorToResponse(context, "Single sign-on settings are not paid");
                return;
            }
            var settings = _settingsManager.Load<SsoSettingsV2>();

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
                await WriteErrorToResponse(context, "Single sign-on is disabled");
                return;
            }

            var data = context.Request.Query["data"];

            if (string.IsNullOrEmpty(data))
            {
                await WriteErrorToResponse(context, "SAML response is null or empty");
                return;
            }

            if (context.Request.Query["auth"] == "true")
            {
                var userData = _signature.Read<SsoUserData>(data);

                if (userData == null)
                {
                    await WriteErrorToResponse(context, "SAML response is not valid");
                    return;
                }

                var userInfo = userData.ToUserInfo(true);

                if (Equals(userInfo, Constants.LostUser))
                {
                    await WriteErrorToResponse(context, "Can't create userInfo using current SAML response (fields Email, FirstName, LastName are required)");
                    return;
                }

                if (userInfo.Status == EmployeeStatus.Terminated)
                {
                    await WriteErrorToResponse(context, "Current user is terminated");
                    return;
                }

                if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
                {
                    var authenticatedUserInfo = _userManager.GetUsers(((IUserAccount)context.User.Identity).ID);

                    if (!Equals(userInfo, authenticatedUserInfo))
                    {
                        var loginName = authenticatedUserInfo.DisplayUserName(false, _displayUserSettingsHelper);
                        _messageService.Send(loginName, MessageAction.Logout);
                        _cookiesManager.ResetUserCookie();
                        _securityContext.Logout();
                    }
                    else
                    {
                        _log.DebugFormat("User {0} already authenticated", context.User.Identity);
                    }
                }

                userInfo = AddUser(userInfo);

                var authKey = _securityContext.AuthenticateMe(userInfo.Id);
                _cookiesManager.SetCookies(CookiesType.AuthKey, authKey);
                _messageService.Send(MessageAction.LoginSuccessViaSSO);

                await context.Response.WriteAsync($"Authenticated with token: {authKey}");
            }
            else if (context.Request.Query["logout"] == "true")
            {
                var logoutSsoUserData = _signature.Read<LogoutSsoUserData>(data);

                if (logoutSsoUserData == null)
                {
                    await WriteErrorToResponse(context, "SAML Logout response is not valid");
                    return;
                }

                var userInfo = _userManager.GetSsoUserByNameId(logoutSsoUserData.NameId);

                if (Equals(userInfo, Constants.LostUser))
                {
                    await WriteErrorToResponse(context, "Can't logout userInfo using current SAML response");
                    return;
                }

                if (userInfo.Status == EmployeeStatus.Terminated)
                {
                    await WriteErrorToResponse(context, "Current user is terminated");
                    return;
                }

                _securityContext.AuthenticateMeWithoutCookie(userInfo.Id);

                var loginName = userInfo.DisplayUserName(false, _displayUserSettingsHelper);
                _messageService.Send(loginName, MessageAction.Logout);

                _cookiesManager.ResetUserCookie();
                _securityContext.Logout();
            }
        }
        catch (Exception e)
        {
            await WriteErrorToResponse(context, $"Unexpected error. {e}");
        }
        finally
        {
            await context.Response.CompleteAsync();
            //context.ApplicationInstance.CompleteRequest();
        }
    }

    private async Task WriteErrorToResponse(HttpContext context, string errorText)
    {
        _log.ErrorFormat(errorText);
        await context.Response.WriteAsync(errorText);
    }

    private UserInfo AddUser(UserInfo userInfo)
    {
        UserInfo newUserInfo;

        try
        {
            newUserInfo = userInfo.Clone() as UserInfo;

            if (newUserInfo == null)
                return Constants.LostUser;

            _log.DebugFormat("Adding or updating user in database, userId={0}", userInfo.Id);

            _securityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);

            if (string.IsNullOrEmpty(newUserInfo.UserName))
            {
                var limitExceeded = _tenantStatisticsProvider.GetUsersCount() >= _tenantExtra.GetTenantQuota().ActiveUsers;

                newUserInfo = _userManagerWrapper.AddUser(newUserInfo, UserManagerWrapper.GeneratePassword(), true,
                    false, isVisitor: limitExceeded);
            }
            else
            {
                if (!_userFormatter.IsValidUserName(userInfo.FirstName, userInfo.LastName))
                    throw new Exception(Resource.ErrorIncorrectUserName);

                _userManager.SaveUserInfo(newUserInfo);
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
}

public static class SsoHandlerExtensions
{
    public static IApplicationBuilder UseSsoHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SsoHandler>();
    }
}
