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
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;
using Microsoft.AspNetCore.Http;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Core
{
    public enum CookiesType
    {
        AuthKey,
        SocketIO
    }

    public static class CookiesManager
    {
        private const string AuthCookiesName = "asc_auth_key";
        private const string SocketIOCookiesName = "socketio.sid";

        private static string GetCookiesName(CookiesType type)
        {
            switch (type)
            {
                case CookiesType.AuthKey: return AuthCookiesName;
                case CookiesType.SocketIO: return SocketIOCookiesName;
            }

            return string.Empty;
        }

        public static string GetRequestVar(this HttpContext httpContext, CookiesType type)
        {
            if (httpContext == null) return "";

            var cookie = httpContext.Request.Query[GetCookiesName(type)].FirstOrDefault() ?? httpContext.Request.Form[GetCookiesName(type)].FirstOrDefault();

            return string.IsNullOrEmpty(cookie) ? httpContext.GetCookies(type) : cookie;
        }

        public static void SetCookies(this HttpContext httpContext, CookiesType type, string value, bool session = false)
        {
            if (httpContext == null) return;

            var options = new CookieOptions
            {
                Expires = GetExpiresDate(session)
            };

            if (type == CookiesType.AuthKey)
            {
                options.HttpOnly = true;

                if (httpContext.Request.GetUrlRewriter().Scheme == "https")
                {
                    options.Secure = true;
                }
            }

            httpContext.Response.Cookies.Append(GetCookiesName(type), value, options);
        }

        public static void SetCookies(this HttpContext httpContext, CookiesType type, string value, string domain, bool session = false)
        {
            if (httpContext == null) return;

            var options = new CookieOptions
            {
                Expires = GetExpiresDate(session),
                Domain = domain
            };

            if (type == CookiesType.AuthKey)
            {
                options.HttpOnly = true;

                if (httpContext.Request.GetUrlRewriter().Scheme == "https")
                {
                    options.Secure = true;
                }
            }

            httpContext.Response.Cookies.Append(GetCookiesName(type), value, options);
        }

        public static string GetCookies(this HttpContext httpContext, CookiesType type)
        {
            if (httpContext != null)
            {
                var cookieName = GetCookiesName(type);

                if (httpContext.Request.Cookies.ContainsKey(cookieName))
                    return httpContext.Request.Cookies[cookieName] ?? "";
            }
            return "";
        }

        public static void ClearCookies(this HttpContext httpContext, CookiesType type)
        {
            if (httpContext == null) return;

            if (httpContext.Request.Cookies.ContainsKey(GetCookiesName(type)))
            {
                httpContext.Response.Cookies.Delete(GetCookiesName(type), new CookieOptions() { Expires = DateTime.Now.AddDays(-3) });
            }
        }

        private static DateTime GetExpiresDate(bool session)
        {
            var expires = DateTime.MinValue;

            if (!session)
            {
                var tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                expires = TenantCookieSettings.GetExpiresTime(tenant);
            }

            return expires;
        }

        public static void SetLifeTime(this HttpContext httpContext, int lifeTime)
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant(httpContext);
            if (!CoreContext.UserManager.IsUserInGroup(tenant, SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
            {
                throw new SecurityException();
            }

            var settings = TenantCookieSettings.GetForTenant(tenant.TenantId);

            if (lifeTime > 0)
            {
                settings.Index += 1;
                settings.LifeTime = lifeTime;
            }
            else
            {
                settings.LifeTime = 0;
            }

            TenantCookieSettings.SetForTenant(tenant.TenantId, settings);

            var cookie = SecurityContext.AuthenticateMe(tenant.TenantId, SecurityContext.CurrentAccount.ID);

            httpContext.SetCookies(CookiesType.AuthKey, cookie);
        }

        public static int GetLifeTime()
        {
            return TenantCookieSettings.GetForTenant(TenantProvider.CurrentTenantID).LifeTime;
        }

        public static void ResetUserCookie(this HttpContext httpContext, int tenantId, Guid? userId = null)
        {
            var settings = TenantCookieSettings.GetForUser(userId ?? SecurityContext.CurrentAccount.ID);
            settings.Index += 1;
            TenantCookieSettings.SetForUser(userId ?? SecurityContext.CurrentAccount.ID, settings);

            if (!userId.HasValue)
            {
                var cookie = SecurityContext.AuthenticateMe(tenantId, SecurityContext.CurrentAccount.ID);

                httpContext.SetCookies(CookiesType.AuthKey, cookie);
            }
        }

        public static void ResetTenantCookie(this HttpContext httpContext)
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant(httpContext);

            if (!CoreContext.UserManager.IsUserInGroup(tenant, SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID))
            {
                throw new SecurityException();
            }

            var settings = TenantCookieSettings.GetForTenant(tenant.TenantId);
            settings.Index += 1;
            TenantCookieSettings.SetForTenant(tenant.TenantId, settings);

            var cookie = SecurityContext.AuthenticateMe(tenant.TenantId, SecurityContext.CurrentAccount.ID);
            httpContext.SetCookies(CookiesType.AuthKey, cookie);
        }
    }
}