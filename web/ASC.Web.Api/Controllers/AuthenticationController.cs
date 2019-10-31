using System;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using static ASC.Security.Cryptography.EmailValidationKeyProvider;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        public UserManager UserManager { get; }
        public TenantManager TenantManager { get; }
        public SecurityContext SecurityContext { get; }
        public TenantCookieSettings TenantCookieSettings { get; }
        public EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        public AuthContext AuthContext { get; }
        public AuthManager AuthManager { get; }

        public AuthenticationController(
            UserManager userManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            TenantCookieSettings tenantCookieSettings,
            EmailValidationKeyProvider emailValidationKeyProvider,
            AuthContext authContext,
            AuthManager authManager)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            TenantCookieSettings = tenantCookieSettings;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            AuthContext = authContext;
            AuthManager = authManager;
        }

        [Create(false)]
        public AuthenticationTokenData AuthenticateMe([FromBody]AuthModel auth)
        {
            var tenant = TenantManager.GetCurrentTenant();
            var user = GetUser(tenant.TenantId, auth.UserName, auth.Password);

            try
            {
                var token = SecurityContext.AuthenticateMe(user.ID);
                var expires = TenantCookieSettings.GetExpiresTime(tenant.TenantId);

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

        [AllowAnonymous]
        [Create("confirm", false)]
        public ValidationResult CheckConfirm([FromBody]EmailValidationKeyModel model)
        {
            return model.Validate(EmailValidationKeyProvider, AuthContext, TenantManager, UserManager, AuthManager);
        }

        private UserInfo GetUser(int tenantId, string userName, string password)
        {
            var user = UserManager.GetUsers(
                        tenantId,
                        userName,
                        Hasher.Base64Hash(password, HashAlg.SHA256));

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
        public static IServiceCollection AddAuthenticationController(this IServiceCollection services)
        {
            return services
                .AddUserManagerService()
                .AddTenantManagerService()
                .AddSecurityContextService()
                .AddTenantCookieSettingsService()
                .AddEmailValidationKeyProviderService()
                .AddAuthContextService()
                .AddAuthManager();
        }
    }
}