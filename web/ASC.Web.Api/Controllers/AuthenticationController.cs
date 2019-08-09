using System;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        [Create(false)]
        public AuthenticationTokenData AuthenticateMe([FromBody]AuthModel auth)
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            var user = GetUser(tenant.TenantId, auth.UserName, auth.Password);

            try
            {
                var token = SecurityContext.AuthenticateMe(tenant.TenantId, user.ID);
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

        private static UserInfo GetUser(int tenantId, string userName, string password)
        {
            var user = CoreContext.UserManager.GetUsers(
                        tenantId,
                        userName,
                        Hasher.Base64Hash(password, HashAlg.SHA256));

            if (user == null || !CoreContext.UserManager.UserExists(tenantId, user.ID))
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