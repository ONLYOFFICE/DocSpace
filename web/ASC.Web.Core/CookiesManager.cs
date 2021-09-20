/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Linq;
using System.Security;
using System.Web;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;

using Microsoft.AspNetCore.Http;


using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Core
{
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

        private IHttpContextAccessor HttpContextAccessor { get; }
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private TenantCookieSettingsHelper TenantCookieSettingsHelper { get; }
        private TenantManager TenantManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }

        public CookiesManager(
            IHttpContextAccessor httpContextAccessor,
            UserManager userManager,
            SecurityContext securityContext,
            TenantCookieSettingsHelper tenantCookieSettingsHelper,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings)
        {
            HttpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            SecurityContext = securityContext;
            TenantCookieSettingsHelper = tenantCookieSettingsHelper;
            TenantManager = tenantManager;
            CoreBaseSettings = coreBaseSettings;
        }

        private static string GetCookiesName(CookiesType type)
        {
            return type switch
            {
                CookiesType.AuthKey => AuthCookiesName,
                CookiesType.SocketIO => SocketIOCookiesName,

                _ => string.Empty,
            };
        }

        public string GetRequestVar(CookiesType type)
        {
            if (HttpContextAccessor?.HttpContext == null) return "";

            var cookie = HttpContextAccessor.HttpContext.Request.Query[GetCookiesName(type)].FirstOrDefault() ?? HttpContextAccessor.HttpContext.Request.Form[GetCookiesName(type)].FirstOrDefault();

            return string.IsNullOrEmpty(cookie) ? GetCookies(type) : cookie;
        }

        public void SetCookies(CookiesType type, string value, bool session = false)
        {
            if (HttpContextAccessor?.HttpContext == null) return;

            var options = new CookieOptions
            {
                Expires = GetExpiresDate(session)
            };

            if (type == CookiesType.AuthKey)
            {
                options.HttpOnly = true;

                if (HttpContextAccessor.HttpContext.Request.GetUrlRewriter().Scheme == "https")
                {
                    options.Secure = true;

                    if (CoreBaseSettings.Personal)
                    {
                        options.SameSite = SameSiteMode.None;
                    }
                }
            }

            HttpContextAccessor.HttpContext.Response.Cookies.Append(GetCookiesName(type), value, options);
        }

        public void SetCookies(CookiesType type, string value, string domain, bool session = false)
        {
            if (HttpContextAccessor?.HttpContext == null) return;

            var options = new CookieOptions
            {
                Expires = GetExpiresDate(session),
                Domain = domain
            };

            if (type == CookiesType.AuthKey)
            {
                options.HttpOnly = true;

                if (HttpContextAccessor.HttpContext.Request.GetUrlRewriter().Scheme == "https")
                {
                    options.Secure = true;

                    if (CoreBaseSettings.Personal)
                    {
                        options.SameSite = SameSiteMode.None;
                    }
                }
            }

            HttpContextAccessor.HttpContext.Response.Cookies.Append(GetCookiesName(type), value, options);
        }

        public string GetCookies(CookiesType type)
        {
            if (HttpContextAccessor?.HttpContext != null)
            {
                var cookieName = GetCookiesName(type);

                if (HttpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookieName))
                    return HttpContextAccessor.HttpContext.Request.Cookies[cookieName] ?? "";
            }
            return "";
        }

        public void ClearCookies(CookiesType type)
        {
            if (HttpContextAccessor?.HttpContext == null) return;

            if (HttpContextAccessor.HttpContext.Request.Cookies.ContainsKey(GetCookiesName(type)))
            {
                HttpContextAccessor.HttpContext.Response.Cookies.Delete(GetCookiesName(type), new CookieOptions() { Expires = DateTime.Now.AddDays(-3) });
            }
        }

        private DateTime GetExpiresDate(bool session)
        {
            var expires = DateTime.MinValue;

            if (!session)
            {
                var tenant = TenantManager.GetCurrentTenant().TenantId;
                expires = TenantCookieSettingsHelper.GetExpiresTime(tenant);
            }

            return expires;
        }

        public void SetLifeTime(int lifeTime)
        {
            var tenant = TenantManager.GetCurrentTenant();
            if (!UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
            {
                throw new SecurityException();
            }

            var settings = TenantCookieSettingsHelper.GetForTenant(tenant.TenantId);

            if (lifeTime > 0)
            {
                settings.Index += 1;
                settings.LifeTime = lifeTime;
            }
            else
            {
                settings.LifeTime = 0;
            }

            TenantCookieSettingsHelper.SetForTenant(tenant.TenantId, settings);

            var cookie = SecurityContext.AuthenticateMe(SecurityContext.CurrentAccount.ID);

            SetCookies(CookiesType.AuthKey, cookie);
        }

        public int GetLifeTime(int tenantId)
        {
            return TenantCookieSettingsHelper.GetForTenant(tenantId).LifeTime;
        }

        public void ResetUserCookie(Guid? userId = null)
        {
            var settings = TenantCookieSettingsHelper.GetForUser(userId ?? SecurityContext.CurrentAccount.ID);
            settings.Index += 1;
            TenantCookieSettingsHelper.SetForUser(userId ?? SecurityContext.CurrentAccount.ID, settings);

            if (!userId.HasValue)
            {
                var cookie = SecurityContext.AuthenticateMe(SecurityContext.CurrentAccount.ID);

                SetCookies(CookiesType.AuthKey, cookie);
            }
        }

        public void ResetTenantCookie()
        {
            var tenant = TenantManager.GetCurrentTenant();

            if (!UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
            {
                throw new SecurityException();
            }

            var settings = TenantCookieSettingsHelper.GetForTenant(tenant.TenantId);
            settings.Index += 1;
            TenantCookieSettingsHelper.SetForTenant(tenant.TenantId, settings);

            var cookie = SecurityContext.AuthenticateMe(SecurityContext.CurrentAccount.ID);
            SetCookies(CookiesType.AuthKey, cookie);
        }
    }
}