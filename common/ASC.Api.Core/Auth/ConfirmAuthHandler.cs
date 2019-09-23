using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ASC.Core;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Auth
{
    public class ConfirmAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ConfirmAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }
        public ConfirmAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock, 
            SecurityContext securityContext,
            EmailValidationKeyProvider emailValidationKeyProvider,
            SetupInfo setupInfo) : 
            base(options, logger, encoder, clock)
        {
            SecurityContext = securityContext;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            SetupInfo = setupInfo;
        }

        public SecurityContext SecurityContext { get; }
        public EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        public SetupInfo SetupInfo { get; }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var Request = QueryHelpers.ParseQuery(Context.Request.Headers["confirm"]);
            _ = Request.TryGetValue("type", out var type);
            var _type = typeof(ConfirmType).TryParseEnum(type, ConfirmType.EmpInvite);

            if (SecurityContext.IsAuthenticated && _type != ConfirmType.EmailChange)
            {
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
            }

            _ = Request.TryGetValue("key", out var key);
            _ = Request.TryGetValue("emplType", out var emplType);
            _ = Request.TryGetValue("email", out var _email);
            var validInterval = SetupInfo.ValidEmailKeyInterval;

            EmailValidationKeyProvider.ValidationResult checkKeyResult;
            switch (_type)
            {
                case ConfirmType.EmpInvite:
                    checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(_email + _type + emplType, key, validInterval);
                    break;
                case ConfirmType.LinkInvite:
                    checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(_type + emplType, key, validInterval);
                    break;
                case ConfirmType.EmailChange:
                    checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(_email + _type + SecurityContext.CurrentAccount.ID, key, validInterval);
                    break;
                default:
                    checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(_email + _type, key, validInterval);
                    break;
            }

            var claims = new List<Claim>() 
            {
                new Claim(ClaimTypes.Role, _type.ToString())
            };

            if (!SecurityContext.IsAuthenticated)
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem, claims);
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
