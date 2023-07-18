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

using ASC.Core.Data;

using Microsoft.Net.Http.Headers;

using Constants = ASC.Core.Users.Constants;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace ASC.Web.Core;

public enum CookiesType
{
    AuthKey,
    SocketIO
}

[Scope]
public class CookiesManager
{
    private const string AuthCookiesName = "asc_auth_key";
    private const string SocketIOCookiesName = "socketio.sid";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly TenantCookieSettingsHelper _tenantCookieSettingsHelper;
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly DbLoginEventsManager _dbLoginEventsManager;
    private readonly MessageService _messageService;
    private readonly SameSiteMode? _sameSiteMode;

    public CookiesManager(
        IHttpContextAccessor httpContextAccessor,
        UserManager userManager,
        SecurityContext securityContext,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        DbLoginEventsManager dbLoginEventsManager,
        MessageService messageService,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _securityContext = securityContext;
        _tenantCookieSettingsHelper = tenantCookieSettingsHelper;
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _dbLoginEventsManager = dbLoginEventsManager;
        _messageService = messageService;

        if (Enum.TryParse<SameSiteMode>(configuration["web:samesite"], out var sameSiteMode))
        {
            _sameSiteMode = sameSiteMode;
        }
    }

    public void SetCookies(CookiesType type, string value, bool session = false)
    {
        if (_httpContextAccessor?.HttpContext == null)
        {
            return;
        }

        var options = new CookieOptions
        {
            Expires = GetExpiresDate(session)
        };

        if (type == CookiesType.AuthKey)
        {
            options.HttpOnly = true;

            if (_sameSiteMode.HasValue && _sameSiteMode.Value != SameSiteMode.None)
            {
                options.SameSite = _sameSiteMode.Value;
            }

            var urlRewriter = _httpContextAccessor.HttpContext.Request.Url();
            if (urlRewriter.Scheme == "https")
            {
                options.Secure = true;

                if (_sameSiteMode.HasValue && _sameSiteMode.Value == SameSiteMode.None)
                {
                    options.SameSite = _sameSiteMode.Value;
                }
            }

            if (FromCors())
            {
                options.Domain = $".{_coreBaseSettings.Basedomain}";
            }
        }

        _httpContextAccessor.HttpContext.Response.Cookies.Append(GetCookiesName(type), value, options);
    }

    public string GetCookies(CookiesType type)
    {
        if (_httpContextAccessor?.HttpContext != null)
        {
            var cookieName = GetCookiesName(type);

            if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookieName))
            {
                return _httpContextAccessor.HttpContext.Request.Cookies[cookieName] ?? "";
            }
        }
        return "";
    }

    public void ClearCookies(CookiesType type)
    {
        if (_httpContextAccessor?.HttpContext == null)
        {
            return;
        }

        if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(GetCookiesName(type)))
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(GetCookiesName(type), new CookieOptions() { Expires = DateTime.Now.AddDays(-3) });
        }
    }

    private DateTime? GetExpiresDate(bool session)
    {
        DateTime? expires = null;

        if (!session)
        {
            var tenant = _tenantManager.GetCurrentTenant().Id;
            expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);
        }

        return expires;
    }

    public async Task SetLifeTime(int lifeTime, bool enabled)
    {
        var tenant = _tenantManager.GetCurrentTenant();
        if (!_userManager.IsUserInGroup(_securityContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
        {
            throw new SecurityException();
        }

        var settings = _tenantCookieSettingsHelper.GetForTenant(tenant.Id);
        settings.Enabled = enabled;

        if (lifeTime > 0)
        {
            settings.Index += 1;
            settings.LifeTime = lifeTime;
        }
        else
        {
            settings.LifeTime = 0;
        }

        _tenantCookieSettingsHelper.SetForTenant(tenant.Id, settings);

        if (enabled && lifeTime > 0)
        {
            await _dbLoginEventsManager.LogOutAllActiveConnectionsForTenant(tenant.Id);
        }

        AuthenticateMeAndSetCookies(tenant.Id, _securityContext.CurrentAccount.ID, MessageAction.LoginSuccess);
    }

    public TenantCookieSettings GetLifeTime(int tenantId)
    {
        return _tenantCookieSettingsHelper.GetForTenant(tenantId);
    }

    public async Task ResetUserCookie(Guid? userId = null)
    {
        var currentUserId = _securityContext.CurrentAccount.ID;
        var tenant = _tenantManager.GetCurrentTenant().Id;
        var settings = _tenantCookieSettingsHelper.GetForUser(userId ?? currentUserId);
        settings.Index = settings.Index + 1;
        _tenantCookieSettingsHelper.SetForUser(userId ?? currentUserId, settings);

        await _dbLoginEventsManager.LogOutAllActiveConnections(tenant, userId ?? currentUserId);

        if (!userId.HasValue)
        {
            AuthenticateMeAndSetCookies(tenant, currentUserId, MessageAction.LoginSuccess);
        }
    }

    public async Task ResetTenantCookie()
    {
        var tenant = _tenantManager.GetCurrentTenant();

        if (!_userManager.IsUserInGroup(_securityContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
        {
            throw new SecurityException();
        }

        var settings = _tenantCookieSettingsHelper.GetForTenant(tenant.Id);
        settings.Index += 1;
        _tenantCookieSettingsHelper.SetForTenant(tenant.Id, settings);

        await _dbLoginEventsManager.LogOutAllActiveConnectionsForTenant(tenant.Id);
    }

    public string AuthenticateMeAndSetCookies(int tenantId, Guid userId, MessageAction action, bool session = false)
    {
        var isSuccess = true;
        var cookies = string.Empty;
        Func<int> funcLoginEvent = () => { return GetLoginEventId(action); };

        try
        {
            cookies = _securityContext.AuthenticateMe(userId, funcLoginEvent);
        }
        catch (Exception)
        {
            isSuccess = false;
            throw;
        }
        finally
        {
            if (isSuccess)
            {
                SetCookies(CookiesType.AuthKey, cookies, session);
                _dbLoginEventsManager.ResetCache(tenantId, userId);
            }
        }

        return cookies;
    }

    public void AuthenticateMeAndSetCookies(string login, string passwordHash, MessageAction action, bool session = false)
    {
        var isSuccess = true;
        var cookies = string.Empty;
        Func<int> funcLoginEvent = () => { return GetLoginEventId(action); };

        try
        {
            cookies = _securityContext.AuthenticateMe(login, passwordHash, funcLoginEvent);
        }
        catch (Exception)
        {
            isSuccess = false;
            throw;
        }
        finally
        {
            if (isSuccess)
            {
                SetCookies(CookiesType.AuthKey, cookies, session);
                _dbLoginEventsManager.ResetCache();
            }
        }
    }

    public int GetLoginEventId(MessageAction action)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;
        var userId = _securityContext.CurrentAccount.ID;
        var data = new MessageUserData(tenantId, userId);

        return _messageService.SendLoginMessage(data, action);
    }

    public string GetAscCookiesName()
    {
        return GetCookiesName(CookiesType.AuthKey);
    }

    private string GetCookiesName(CookiesType type)
    {
        var result = type switch
        {
            CookiesType.AuthKey => AuthCookiesName,
            CookiesType.SocketIO => SocketIOCookiesName,

            _ => string.Empty,
        };

        if (FromCors())
        {
            var origin = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Origin].FirstOrDefault();
            if (!string.IsNullOrEmpty(origin))
            {
                var originUri = new Uri(origin);
                var host = originUri.Host;
                var alias = host.Substring(0, host.Length - _coreBaseSettings.Basedomain.Length - 1);
                result = $"{result}_{alias}";
            }
        }

        return result;
    }

    private bool FromCors()
    {
        var urlRewriter = _httpContextAccessor.HttpContext.Request.Url();
        var origin = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Origin].FirstOrDefault();
        var baseDomain = _coreBaseSettings.Basedomain;

        try
        {
            if (!string.IsNullOrEmpty(origin))
            {
                var originUri = new Uri(origin);

                if (!string.IsNullOrEmpty(baseDomain) &&
                urlRewriter.Host != originUri.Host &&
                originUri.Host.EndsWith(baseDomain))
                {
                    return true;
                }
            }
        }
        catch (Exception)
        {

        }

        return false;
    }
}
