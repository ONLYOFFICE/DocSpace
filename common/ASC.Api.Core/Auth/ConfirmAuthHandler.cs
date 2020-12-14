using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Auth
{
    [Scope(Additional = typeof(ConfirmAuthHandlerExtension))]
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
            IServiceProvider serviceProvider) :
            base(options, logger, encoder, clock)
        {
            SecurityContext = securityContext;
            ServiceProvider = serviceProvider;
        }

        private SecurityContext SecurityContext { get; }
        public IServiceProvider ServiceProvider { get; }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            using var scope = ServiceProvider.CreateScope();

            var emailValidationKeyHelper = scope.ServiceProvider.GetService<EmailValidationKeyModelHelper>();
            var emailValidationKeyModel = emailValidationKeyHelper.GetModel();

            if (!emailValidationKeyModel.Type.HasValue)
            {
                return SecurityContext.IsAuthenticated
                    ? Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)))
                    : Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString())));
            }

            EmailValidationKeyProvider.ValidationResult checkKeyResult;
            try
            {
                checkKeyResult = emailValidationKeyHelper.Validate(emailValidationKeyModel);
            }
            catch (ArgumentNullException)
            {
                checkKeyResult = EmailValidationKeyProvider.ValidationResult.Invalid;
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, emailValidationKeyModel.Type.ToString())
            };

            if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Ok)
            {
                if (!SecurityContext.IsAuthenticated)
                {
                    if (emailValidationKeyModel.UiD.HasValue && !emailValidationKeyModel.UiD.Equals(Guid.Empty))
                    {
                        SecurityContext.AuthenticateMe(emailValidationKeyModel.UiD.Value, claims);
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
            }

            var result = checkKeyResult switch
            {
                EmailValidationKeyProvider.ValidationResult.Ok => AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)),
                _ => AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString()))
            };

            return Task.FromResult(result);
        }
    }

    public class ConfirmAuthHandlerExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<EmailValidationKeyModelHelper>();
        }
    }
}
