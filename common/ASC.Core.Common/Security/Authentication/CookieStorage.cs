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
using System.Globalization;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Core.Security.Authentication
{
    [Scope]
    public class CookieStorage
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";

        private InstanceCrypto InstanceCrypto { get; }
        private TenantCookieSettingsHelper TenantCookieSettingsHelper { get; }
        private HttpContext HttpContext { get; }
        private ILog Log { get; }

        public CookieStorage(
            InstanceCrypto instanceCrypto,
            TenantCookieSettingsHelper tenantCookieSettingsHelper,
            IOptionsMonitor<ILog> options)
        {
            InstanceCrypto = instanceCrypto;
            TenantCookieSettingsHelper = tenantCookieSettingsHelper;
            Log = options.CurrentValue;
        }

        public CookieStorage(
            IHttpContextAccessor httpContextAccessor,
            InstanceCrypto instanceCrypto,
            TenantCookieSettingsHelper tenantCookieSettingsHelper,
            IOptionsMonitor<ILog> options)
            : this(instanceCrypto, tenantCookieSettingsHelper, options)
        {
            HttpContext = httpContextAccessor.HttpContext;
        }

        public bool DecryptCookie(string cookie, out int tenant, out Guid userid, out int indexTenant, out DateTime expire, out int indexUser)
        {
            tenant = Tenant.DEFAULT_TENANT;
            userid = Guid.Empty;
            indexTenant = 0;
            expire = DateTime.MaxValue;
            indexUser = 0;

            if (string.IsNullOrEmpty(cookie))
            {
                return false;
            }

            try
            {
                cookie = (HttpUtility.UrlDecode(cookie) ?? "").Replace(' ', '+');
                var s = InstanceCrypto.Decrypt(cookie).Split('$');

                if (1 < s.Length) tenant = int.Parse(s[1]);
                if (4 < s.Length) userid = new Guid(s[4]);
                if (5 < s.Length) indexTenant = int.Parse(s[5]);
                if (6 < s.Length) expire = DateTime.ParseExact(s[6], DateTimeFormat, CultureInfo.InvariantCulture);
                if (7 < s.Length) indexUser = int.Parse(s[7]);

                return true;
            }
            catch (Exception err)
            {
                Log.ErrorFormat("Authenticate error: cookie {0}, tenant {1}, userid {2}, indexTenant {3}, expire {4}: {5}",
                            cookie, tenant, userid, indexTenant, expire.ToString(DateTimeFormat), err);
            }
            return false;
        }


        public string EncryptCookie(int tenant, Guid userid)
        {
            var settingsTenant = TenantCookieSettingsHelper.GetForTenant(tenant);
            var expires = TenantCookieSettingsHelper.GetExpiresTime(tenant);
            var settingsUser = TenantCookieSettingsHelper.GetForUser(tenant, userid);
            return EncryptCookie(tenant, userid, settingsTenant.Index, expires, settingsUser.Index);
        }

        public string EncryptCookie(int tenant, Guid userid, int indexTenant, DateTime expires, int indexUser)
        {
            var s = string.Format("{0}${1}${2}${3}${4}${5}${6}${7}",
                string.Empty, //login
                tenant,
                string.Empty, //password
                GetUserDepenencySalt(),
                userid.ToString("N"),
                indexTenant,
                expires.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
                indexUser);

            return InstanceCrypto.Encrypt(s);
        }


        private string GetUserDepenencySalt()
        {
            var data = string.Empty;
            try
            {
                if (HttpContext?.Request != null)
                {
                    var forwarded = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                    data = string.IsNullOrEmpty(forwarded) ? HttpContext.Request.GetUserHostAddress() : forwarded.Split(':')[0];
                }
            }
            catch { }
            return Hasher.Base64Hash(data ?? string.Empty, HashAlg.SHA256);
        }
    }
}