using System;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static ASC.Security.Cryptography.EmailValidationKeyProvider;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private SecurityContext SecurityContext { get; }
        private TenantCookieSettingsHelper TenantCookieSettingsHelper { get; }
        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        private AuthContext AuthContext { get; }
        private AuthManager AuthManager { get; }
        private CookiesManager CookiesManager { get; }
        public PasswordHasher PasswordHasher { get; }

        public AuthenticationController(
            UserManager userManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            TenantCookieSettingsHelper tenantCookieSettingsHelper,
            EmailValidationKeyProvider emailValidationKeyProvider,
            AuthContext authContext,
            AuthManager authManager,
            CookiesManager cookiesManager,
            PasswordHasher passwordHasher)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            TenantCookieSettingsHelper = tenantCookieSettingsHelper;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            AuthContext = authContext;
            AuthManager = authManager;
            CookiesManager = cookiesManager;
            PasswordHasher = passwordHasher;
        }

        [Create(false)]
        public AuthenticationTokenData AuthenticateMe([FromBody] AuthModel auth)
        {
            var tenant = TenantManager.GetCurrentTenant();
            var user = GetUser(tenant.TenantId, auth.UserName, auth.Password);

            try
            {
                var token = SecurityContext.AuthenticateMe(user.ID);
                CookiesManager.SetCookies(CookiesType.AuthKey, token);
                var expires = TenantCookieSettingsHelper.GetExpiresTime(tenant.TenantId);

                return new AuthenticationTokenData
                {
                    Token = token,
                    Expires = expires
                };
            }
            catch
            {
                throw new Exception("User authentication failed");
            }
        }

        [Create("logout")]
        public void Logout()
        {
            CookiesManager.ClearCookies(CookiesType.AuthKey);
            CookiesManager.ClearCookies(CookiesType.SocketIO);
        }

        [AllowAnonymous]
        [Create("confirm", false)]
        public ValidationResult CheckConfirm([FromBody] EmailValidationKeyModel model)
        {
            return model.Validate();
        }

        private UserInfo GetUser(int tenantId, string userName, string password)
        {
            var passwordHash = PasswordHasher.GetClientPassword(password);
            var user = UserManager.GetUsersByPasswordHash(
                tenantId,
                userName,
                passwordHash);

            if (user == null || !UserManager.UserExists(user))
            {
                throw new Exception("user not found");
            }

            return user;
        }
    }

    public class AuthenticationTokenData
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public bool Sms { get; set; }

        public string PhoneNoise { get; set; }

        public bool Tfa { get; set; }

        public string TfaKey { get; set; }

        public static AuthenticationTokenData GetSample()
        {
            return new AuthenticationTokenData
            {
                Expires = DateTime.UtcNow,
                Token = "abcde12345",
                Sms = false,
                PhoneNoise = null,
                Tfa = false,
                TfaKey = null
            };
        }
    }

    public static class AuthenticationControllerExtension
    {
        public static DIHelper AddAuthenticationController(this DIHelper services)
        {
            return services
                .AddUserManagerService()
                .AddTenantManagerService()
                .AddSecurityContextService()
                .AddTenantCookieSettingsService()
                .AddEmailValidationKeyProviderService()
                .AddAuthContextService()
                .AddAuthManager()
                .AddPasswordHasherService();
        }
    }
}