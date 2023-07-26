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

    public bool DecryptCookie(string cookie, out int tenant, out Guid userid, out int indexTenant, out DateTime expire, out int indexUser, out int loginEventId)
    {
        tenant = Tenant.DefaultTenant;
        userid = Guid.Empty;
        indexTenant = 0;
        expire = DateTime.MaxValue;
        indexUser = 0;
        loginEventId = 0;

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
            if (8 < s.Length)
            {
                loginEventId = !string.IsNullOrEmpty(s[8]) ? int.Parse(s[8]) : 0;
            }
            return true;
        }
        catch (Exception err)
        {
            _logger.AuthenticateError(cookie, tenant, userid, indexTenant, expire.ToString(DateTimeFormat), loginEventId, err);
        }

        return false;
    }


    public int GetLoginEventIdFromCookie(string cookie)
    {
        var loginEventId = 0;
        if (string.IsNullOrEmpty(cookie))
        {
            return loginEventId;
        }

        try
        {
            cookie = (HttpUtility.UrlDecode(cookie) ?? "").Replace(' ', '+');
            var s = _instanceCrypto.Decrypt(cookie).Split('$');
            if (8 < s.Length)
            {
                loginEventId = !string.IsNullOrEmpty(s[8]) ? int.Parse(s[8]) : 0;
            }
        }
        catch (Exception err)
        {
            _logger.ErrorLoginEvent(cookie, loginEventId, err);
        }
        return loginEventId;
    }

    public async Task<string> EncryptCookieAsync(int tenant, Guid userid, int loginEventId)
    {
        var settingsTenant = await _tenantCookieSettingsHelper.GetForTenantAsync(tenant);
        var expires = await _tenantCookieSettingsHelper.GetExpiresTimeAsync(tenant);
        var settingsUser = await _tenantCookieSettingsHelper.GetForUserAsync(tenant, userid);

        return EncryptCookie(tenant, userid, settingsTenant.Index, expires, settingsUser.Index, loginEventId);
    }

    public string EncryptCookie(int tenant, Guid userid, int indexTenant, DateTime expires, int indexUser, int loginEventId)
    {
        var s = string.Format("{0}${1}${2}${3}${4}${5}${6}${7}${8}",
            string.Empty, //login
            tenant,
            string.Empty, //password
            GetUserDepenencySalt(),
            userid.ToString("N"),
            indexTenant,
            expires.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
            indexUser,
            loginEventId != 0 ? loginEventId.ToString() : null);

        return _instanceCrypto.Encrypt(s);
    }

    private string GetUserDepenencySalt()
    {
        var data = string.Empty;
        try
        {
            if (_httpContext?.Request != null)
            {
                data = _httpContext.Connection.RemoteIpAddress.ToString();
            }
        }
        catch { }

        return Hasher.Base64Hash(data ?? string.Empty, HashAlg.SHA256);
    }
}
