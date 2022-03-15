using Constants = ASC.Core.Users.Constants;

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

        private DateTime? GetExpiresDate(bool session)
        {
            DateTime? expires = null;

            if (!session)
            {
                var tenant = TenantManager.GetCurrentTenant().Id;
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

            var settings = TenantCookieSettingsHelper.GetForTenant(tenant.Id);

            if (lifeTime > 0)
            {
                settings.Index += 1;
                settings.LifeTime = lifeTime;
            }
            else
            {
                settings.LifeTime = 0;
            }

            TenantCookieSettingsHelper.SetForTenant(tenant.Id, settings);

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

            var settings = TenantCookieSettingsHelper.GetForTenant(tenant.Id);
            settings.Index += 1;
            TenantCookieSettingsHelper.SetForTenant(tenant.Id, settings);

            var cookie = SecurityContext.AuthenticateMe(SecurityContext.CurrentAccount.ID);
            SetCookies(CookiesType.AuthKey, cookie);
        }
    }
}