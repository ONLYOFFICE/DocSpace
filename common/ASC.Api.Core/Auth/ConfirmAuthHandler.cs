using System;
using System.Net;
using System.Security.Authentication;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ASC.Core;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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
            if (SecurityContext.IsAuthenticated)
            {
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
            }

            var Request = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(Context.Request.Headers["confirm"]);
            _ = Request.TryGetValue("type", out var type);
            _ = Request.TryGetValue("key", out var key);
            _ = Request.TryGetValue("emplType", out var emplType);
            _ = Request.TryGetValue("email", out var _email);
            var validInterval = SetupInfo.ValidEmailKeyInterval;

            var _type = typeof(ConfirmType).TryParseEnum(type, ConfirmType.EmpInvite);
            EmailValidationKeyProvider.ValidationResult checkKeyResult;
            switch (_type)
            {
                case ConfirmType.EmpInvite:
                    checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(_email + _type + emplType, key, validInterval);
                    break;

                case ConfirmType.LinkInvite:
                    checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(_type + emplType, key, validInterval);
                    break;

                default:
                    checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(_email + _type, key, validInterval);
                    break;
            }

            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

            var result = checkKeyResult switch
            {
                EmailValidationKeyProvider.ValidationResult.Ok => AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)),
                _ => AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString()))
            };

            return Task.FromResult(result);
        }
    }
}
