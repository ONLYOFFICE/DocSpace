using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ASC.Core;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Auth
{
    public class ConfirmAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ConfirmAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var emailValidationKeyModel = EmailValidationKeyModel.FromRequest(Context.Request);

            if (SecurityContext.IsAuthenticated && emailValidationKeyModel.Type != ConfirmType.EmailChange)
            {
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
            }

            var checkKeyResult = emailValidationKeyModel.Validate();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, emailValidationKeyModel.Type.ToString())
            };

            if (!SecurityContext.IsAuthenticated)
            {
                if (emailValidationKeyModel.UiD.HasValue)
                {
                    SecurityContext.AuthenticateMe(CoreContext.TenantManager.GetCurrentTenant().TenantId, emailValidationKeyModel.UiD.Value, claims);
                }
                else
                {
                    SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem, claims);
                }
            }
            else
            {
                SecurityContext.AuthenticateMe(SecurityContext.CurrentAccount, claims);
            }

            var result = checkKeyResult switch
            {
                EmailValidationKeyProvider.ValidationResult.Ok => AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)),
                _ => AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString()))
            };

            return Task.FromResult(result);
        }
    }
}
