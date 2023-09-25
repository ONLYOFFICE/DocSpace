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

namespace ASC.Web.Api;

/// <summary>
/// Security API.
/// </summary>
/// <name>security</name>
[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("security")]
public class ConnectionsController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly DbLoginEventsManager _dbLoginEventsManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly ILogger<ConnectionsController> _logger;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly MessageService _messageService;
    private readonly MessageTarget _messageTarget;
    private readonly CookiesManager _cookiesManager;
    private readonly CookieStorage _cookieStorage;
    private readonly GeolocationHelper _geolocationHelper;

    public ConnectionsController(
        UserManager userManager,
        SecurityContext securityContext,
        DbLoginEventsManager dbLoginEventsManager,
        IHttpContextAccessor httpContextAccessor,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        CommonLinkUtility commonLinkUtility,
        ILogger<ConnectionsController> logger,
        WebItemSecurity webItemSecurity,
        MessageService messageService,
        MessageTarget messageTarget,
        CookiesManager cookiesManager,
        CookieStorage cookieStorage,
        GeolocationHelper geolocationHelper)
    {
        _userManager = userManager;
        _securityContext = securityContext;
        _dbLoginEventsManager = dbLoginEventsManager;
        _httpContextAccessor = httpContextAccessor;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _commonLinkUtility = commonLinkUtility;
        _logger = logger;
        _webItemSecurity = webItemSecurity;
        _messageService = messageService;
        _messageTarget = messageTarget;
        _cookiesManager = cookiesManager;
        _cookieStorage = cookieStorage;
        _geolocationHelper = geolocationHelper;
    }

    /// <summary>
    /// Returns all the active connections to the portal.
    /// </summary>
    /// <short>
    /// Get active connections
    /// </short>
    /// <category>Active connections</category>
    /// <returns type="System.Object, System">Active portal connections</returns>
    /// <path>api/2.0/security/activeconnections</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("activeconnections")]
    public async Task<object> GetAllActiveConnections()
    {
        var user = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
        var loginEvents = await _dbLoginEventsManager.GetLoginEventsAsync(user.TenantId, user.Id);
        var tasks = loginEvents.ConvertAll(async r => await _geolocationHelper.AddGeolocationAsync(r));
        var listLoginEvents = (await Task.WhenAll(tasks)).ToList();
        var loginEventId = GetLoginEventIdFromCookie();
        if (loginEventId != 0)
        {
            var loginEvent = listLoginEvents.FirstOrDefault(x => x.Id == loginEventId);
            if (loginEvent != null)
            {
                listLoginEvents.Remove(loginEvent);
                listLoginEvents.Insert(0, loginEvent);
            }
        }
        else
        {
            if (listLoginEvents.Count == 0)
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var uaHeader = MessageSettings.GetUAHeader(request);
                var clientInfo = MessageSettings.GetClientInfo(uaHeader);
                var platformAndDevice = MessageSettings.GetPlatformAndDevice(clientInfo);
                var browser = MessageSettings.GetBrowser(clientInfo);
                var ip = MessageSettings.GetIP(request);

                var baseEvent = new BaseEvent
                {
                    Id = 0,
                    Platform = platformAndDevice,
                    Browser = browser,
                    Date = DateTime.Now,
                    IP = ip
                };

                listLoginEvents.Add(await _geolocationHelper.AddGeolocationAsync(baseEvent));
            }
        }

        var result = new
        {
            Items = listLoginEvents,
            LoginEvent = loginEventId
        };
        return result;
    }

    /// <summary>
    /// Logs out from all the active connections of the current user and changes their password.
    /// </summary>
    /// <short>
    /// Log out and change password
    /// </short>
    /// <category>Active connections</category>
    /// <returns type="System.Object, System">URL to the confirmation message for changing a password</returns>
    /// <path>api/2.0/security/activeconnections/logoutallchangepassword</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("activeconnections/logoutallchangepassword")]
    public async Task<object> LogOutAllActiveConnectionsChangePassword()
    {
        try
        {
            var user = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
            var userName = user.DisplayUserName(false, _displayUserSettingsHelper);

            await LogOutAllActiveConnections(user.Id);

            _securityContext.Logout();

            var auditEventDate = DateTime.UtcNow;
            auditEventDate = auditEventDate.AddTicks(-(auditEventDate.Ticks % TimeSpan.TicksPerSecond));

            var hash = auditEventDate.ToString("s");
            var confirmationUrl = await _commonLinkUtility.GetConfirmationEmailUrlAsync(user.Email, ConfirmType.PasswordChange, hash, user.Id);

            await _messageService.SendAsync(MessageAction.UserSentPasswordChangeInstructions, _messageTarget.Create(user.Id), auditEventDate, userName);

            return confirmationUrl;
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
            return null;
        }
    }

    /// <summary>
    /// Logs out from all the active connections of the user with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Log out for the user by ID
    /// </short>
    /// <category>Active connections</category>
    /// <param type="System.Guid, System" method="url" name="userId">User ID</param>
    /// <path>api/2.0/security/activeconnections/logoutall/{userId}</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns></returns>
    [HttpPut("activeconnections/logoutall/{userId}")]
    public async Task LogOutAllActiveConnectionsForUserAsync(Guid userId)
    {
        if (!await _userManager.IsDocSpaceAdminAsync(_securityContext.CurrentAccount.ID)
            && !await _webItemSecurity.IsProductAdministratorAsync(WebItemManager.PeopleProductID, _securityContext.CurrentAccount.ID))
        {
            throw new SecurityException("Method not available");
        }

        await LogOutAllActiveConnections(userId);
    }

    /// <summary>
    /// Logs out from all the active connections except the current connection.
    /// </summary>
    /// <short>
    /// Log out from all connections
    /// </short>
    /// <category>Active connections</category>
    /// <returns type="System.Object, System">Current user name</returns>
    /// <path>api/2.0/security/activeconnections/logoutallexceptthis</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("activeconnections/logoutallexceptthis")]
    public async Task<object> LogOutAllExceptThisConnection()
    {
        try
        {
            var user = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
            var userName = user.DisplayUserName(false, _displayUserSettingsHelper);
            var loginEventFromCookie = GetLoginEventIdFromCookie();

            await _dbLoginEventsManager.LogOutAllActiveConnectionsExceptThisAsync(loginEventFromCookie, user.TenantId, user.Id);

            await _messageService.SendAsync(MessageAction.UserLogoutActiveConnections, userName);
            return userName;
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
            return null;
        }
    }

    /// <summary>
    /// Logs out from the connection with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Log out from the connection
    /// </short>
    /// <category>Active connections</category>
    /// <param type="System.Int32, System" method="url" name="loginEventId">Login event ID</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/security/activeconnections/logout/{loginEventId}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("activeconnections/logout/{loginEventId}")]
    public async Task<bool> LogOutActiveConnection(int loginEventId)
    {
        try
        {
            var user = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
            var userName = user.DisplayUserName(false, _displayUserSettingsHelper);

            await _dbLoginEventsManager.LogOutEventAsync(loginEventId);

            await _messageService.SendAsync(MessageAction.UserLogoutActiveConnection, userName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
            return false;
        }
    }

    private async Task LogOutAllActiveConnections(Guid? userId = null)
    {
        var currentUserId = _securityContext.CurrentAccount.ID;
        var user = await _userManager.GetUsersAsync(userId ?? currentUserId);
        var userName = user.DisplayUserName(false, _displayUserSettingsHelper);
        var auditEventDate = DateTime.UtcNow;

        await _messageService.SendAsync(currentUserId.Equals(user.Id) ? MessageAction.UserLogoutActiveConnections : MessageAction.UserLogoutActiveConnectionsForUser, _messageTarget.Create(user.Id), auditEventDate, userName);
        await _cookiesManager.ResetUserCookieAsync(user.Id);
    }

    private int GetLoginEventIdFromCookie()
    {
        var cookie = _cookiesManager.GetCookies(CookiesType.AuthKey);
        var loginEventId = _cookieStorage.GetLoginEventIdFromCookie(cookie);
        return loginEventId;
    }
}
