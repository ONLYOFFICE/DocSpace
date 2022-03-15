namespace ASC.Core.Security.Authentication;

[Scope]
public class CookieStorage
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";

    private readonly InstanceCrypto _instanceCrypto;
    private readonly TenantCookieSettingsHelper _tenantCookieSettingsHelper;
    private readonly HttpContext _httpContext;
    private readonly ILog _logger;

    public CookieStorage(
        InstanceCrypto instanceCrypto,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        IOptionsMonitor<ILog> options)
    {
        _instanceCrypto = instanceCrypto;
        _tenantCookieSettingsHelper = tenantCookieSettingsHelper;
        _logger = options.CurrentValue;
    }

    public CookieStorage(
        IHttpContextAccessor httpContextAccessor,
        InstanceCrypto instanceCrypto,
        TenantCookieSettingsHelper tenantCookieSettingsHelper,
        IOptionsMonitor<ILog> options)
        : this(instanceCrypto, tenantCookieSettingsHelper, options)
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
            _logger.ErrorFormat("Authenticate error: cookie {0}, tenant {1}, userid {2}, indexTenant {3}, expire {4}: {5}",
                        cookie, tenant, userid, indexTenant, expire.ToString(DateTimeFormat), err);
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
