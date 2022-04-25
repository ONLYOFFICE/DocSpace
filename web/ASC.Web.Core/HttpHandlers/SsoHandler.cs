using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Security.Authentication;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management.SingleSignOnSettings;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Web.Core.HttpHandlers;
public class SsoHandler
{
    private RequestDelegate _next;

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
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly TenantStatisticsProvider _tenantStatisticsProvider;
    private readonly UserFormatter _userFormatter;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly MessageService _messageService;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;


    private const string AUTH_PAGE = "~/Auth.aspx";

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
        CommonLinkUtility commonLinkUtility,
        TenantStatisticsProvider tenantStatisticsProvider,
        UserFormatter userFormatter,
        UserManagerWrapper userManagerWrapper,
        MessageService messageService,
        DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        _log = optionsMonitor.Get("ASC");
        _coreBaseSettings = coreBaseSettings;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _cookiesManager = cookiesManager;
        _signature = signature;
        _securityContext = securityContext;
        _tenantExtra = tenantExtra;
        _commonLinkUtility = commonLinkUtility;
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
                ErrorWithStatus("Single sign-on settings are disabled");
            }
            if (!(_coreBaseSettings.Standalone || _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId).Sso))
            {
                ErrorWithStatus("Single sign-on settings are not paid");
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
                ErrorWithStatus("Single sign-on is disabled");
            }

            var data = context.Request.Query["data"];

            if (string.IsNullOrEmpty(data))
            {
                ErrorWithStatus("SAML response is null or empty");
            }

            if (context.Request.Query["auth"] == "true")
            {
                var userData = _signature.Read<SsoUserData>(data);

                if (userData == null)
                {
                    ErrorWithStatus("SAML response is not valid");
                }

                var userInfo = userData.ToUserInfo(true);

                if (Equals(userInfo, Constants.LostUser))
                {
                    ErrorWithStatus("Can't create userInfo using current SAML response (fields Email, FirstName, LastName are required)");
                }

                if (userInfo.Status == EmployeeStatus.Terminated)
                {
                    ErrorWithStatus("Current user is terminated");
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

                var authKey = _securityContext.AuthenticateMe(userInfo.ID);
                _cookiesManager.SetCookies(CookiesType.AuthKey, authKey);
                _messageService.Send(MessageAction.LoginSuccessViaSSO);

                context.Response.Redirect(_commonLinkUtility.GetDefault() + "?token=" + HttpUtility.UrlEncode(authKey), false);
            }
            else if (context.Request.Query["logout"] == "true")
            {
                var logoutSsoUserData = _signature.Read<LogoutSsoUserData>(data);

                if (logoutSsoUserData == null)
                {
                    ErrorWithStatus("SAML Logout response is not valid");
                }

                var userInfo = _userManager.GetSsoUserByNameId(logoutSsoUserData.NameId);

                if (Equals(userInfo, Constants.LostUser))
                {
                    ErrorWithStatus("Can't logout userInfo using current SAML response");
                }

                if (userInfo.Status == EmployeeStatus.Terminated)
                {
                    ErrorWithStatus("Current user is terminated");
                }

                _securityContext.AuthenticateMeWithoutCookie(userInfo.ID);

                var loginName = userInfo.DisplayUserName(false, _displayUserSettingsHelper);
                _messageService.Send(loginName, MessageAction.Logout);

                _cookiesManager.ResetUserCookie();
                _securityContext.Logout();

                context.Response.Redirect(AUTH_PAGE, false);
            }
        }
        catch (Exception e)
        {
            ErrorWithStatus("Unexpected error. {0}", e);
        }
        finally
        {
            await context.Response.CompleteAsync();
            //context.ApplicationInstance.CompleteRequest();
        }
    }

    private void ErrorWithStatus(string errorText, object errorArg = null)
    {
        _log.ErrorFormat(errorText, errorArg);
        throw new Exception(string.Format(errorText, errorArg));
    }

    private UserInfo AddUser(UserInfo userInfo)
    {
        UserInfo newUserInfo;

        try
        {
            newUserInfo = userInfo.Clone() as UserInfo;

            if (newUserInfo == null)
                return Constants.LostUser;

            _log.DebugFormat("Adding or updating user in database, userId={0}", userInfo.ID);

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
