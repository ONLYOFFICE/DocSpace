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
    [Scope]
    [DefaultRoute]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private SecurityContext SecurityContext { get; }
        private TenantCookieSettingsHelper TenantCookieSettingsHelper { get; }
        private CookiesManager CookiesManager { get; }
        public PasswordHasher PasswordHasher { get; }
        public EmailValidationKeyModelHelper EmailValidationKeyModelHelper { get; }

        public AuthenticationController(
            UserManager userManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            TenantCookieSettingsHelper tenantCookieSettingsHelper,
            CookiesManager cookiesManager,
            PasswordHasher passwordHasher,
            EmailValidationKeyModelHelper emailValidationKeyModelHelper)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            TenantCookieSettingsHelper = tenantCookieSettingsHelper;
            CookiesManager = cookiesManager;
            PasswordHasher = passwordHasher;
            EmailValidationKeyModelHelper = emailValidationKeyModelHelper;
        }


        [Read]
        public bool GetIsAuthentificated()
        {
            return SecurityContext.IsAuthenticated;
        }

        [Create(false)]
        public AuthenticationTokenData AuthenticateMeFromBody([FromBody] AuthModel auth)
        {
            return AuthenticateMe(auth);
        }

        [Create(false)]
        [Consumes("application/x-www-form-urlencoded")]
        public AuthenticationTokenData AuthenticateMeFromForm([FromForm] AuthModel auth)
        {
            return AuthenticateMe(auth);
        }

        [Create("logout")]
        public void Logout()
        {
            CookiesManager.ClearCookies(CookiesType.AuthKey);
            CookiesManager.ClearCookies(CookiesType.SocketIO);
        }

        [Create("confirm", false)]
        public ValidationResult CheckConfirmFromBody([FromBody] EmailValidationKeyModel model)
        {
            return EmailValidationKeyModelHelper.Validate(model);
        }

        [Create("confirm", false)]
        [Consumes("application/x-www-form-urlencoded")]
        public ValidationResult CheckConfirmFromForm([FromForm] EmailValidationKeyModel model)
        {
            return EmailValidationKeyModelHelper.Validate(model);
        }

        private AuthenticationTokenData AuthenticateMe(AuthModel auth)
        {
            var tenant = TenantManager.GetCurrentTenant();
            var user = GetUser(tenant.TenantId, auth);

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

        private UserInfo GetUser(int tenantId, AuthModel memberModel)
        {
            memberModel.PasswordHash = (memberModel.PasswordHash ?? "").Trim();

            if (string.IsNullOrEmpty(memberModel.PasswordHash))
            {
                memberModel.Password = (memberModel.Password ?? "").Trim();

                if (!string.IsNullOrEmpty(memberModel.Password))
                {
                    memberModel.PasswordHash = PasswordHasher.GetClientPassword(memberModel.Password);
                }
            }

            var user = UserManager.GetUsersByPasswordHash(
                tenantId,
                memberModel.UserName,
                memberModel.PasswordHash);

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
}