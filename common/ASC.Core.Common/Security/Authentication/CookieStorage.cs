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

namespace ASC.Core.Security.Authentication;

[Scope]
public class CookieStorage
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";

    private readonly InstanceCrypto _instanceCrypto;
    private readonly TenantCookieSettingsHelper _tenantCookieSettingsHelper;
    private readonly HttpContext _httpContext;
    private readonly ILogger<CookieStorage> _logger;

    public CookieStorage(
        InstanceCrypto instanceCrypto,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        ILogger<CookieStorage> logger)
    {
        _instanceCrypto = instanceCrypto;
        _tenantCookieSettingsHelper = tenantCookieSettingsHelper;
        _logger = logger;
    }

    public CookieStorage(
        IHttpContextAccessor httpContextAccessor,
        InstanceCrypto instanceCrypto,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        ILogger<CookieStorage> logger)
        : this(instanceCrypto, tenantCookieSettingsHelper, logger)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public bool DecryptCookie(string cookie, out int tenant, out Guid userid, out int indexTenant, out DateTime expire, out int indexUser)
    {
        tenant = Tenant.DefaultTenant;
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
            var s = _instanceCrypto.Decrypt(cookie).Split('$');

            if (1 < s.Length)
            {
                tenant = int.Parse(s[1]);
            }
            if (4 < s.Length)
            {
                userid = new Guid(s[4]);
            }
            if (5 < s.Length)
            {
                indexTenant = int.Parse(s[5]);
            }
            if (6 < s.Length)
            {
                expire = DateTime.ParseExact(s[6], DateTimeFormat, CultureInfo.InvariantCulture);
            }
            if (7 < s.Length)
            {
                indexUser = int.Parse(s[7]);
            }

            return true;
        }
        catch (Exception err)
        {
            _logger.AuthenticateError(cookie, tenant, userid, indexTenant, expire.ToString(DateTimeFormat), err);
        }

        return false;
    }


    public string EncryptCookie(int tenant, Guid userid)
    {
        var settingsTenant = _tenantCookieSettingsHelper.GetForTenant(tenant);
        var expires = _tenantCookieSettingsHelper.GetExpiresTime(tenant);
        var settingsUser = _tenantCookieSettingsHelper.GetForUser(tenant, userid);

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

        return _instanceCrypto.Encrypt(s);
    }

    private string GetUserDepenencySalt()
    {
        var data = string.Empty;
        try
        {
            if (_httpContext?.Request != null)
            {
                var forwarded = _httpContext.Request.Headers["X-Forwarded-For"].ToString();
                data = string.IsNullOrEmpty(forwarded) ? _httpContext.Request.GetUserHostAddress() : forwarded.Split(':')[0];
            }
        }
        catch { }

        return Hasher.Base64Hash(data ?? string.Empty, HashAlg.SHA256);
    }
}
