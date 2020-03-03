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
using ASC.Web.Studio.Core;

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
        public ConfirmAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            SecurityContext securityContext,
            EmailValidationKeyProvider emailValidationKeyProvider,
            SetupInfo setupInfo,
            TenantManager tenantManager,
            UserManager userManager,
            AuthManager authManager,
            AuthContext authContext) :
            base(options, logger, encoder, clock)
        {
            SecurityContext = securityContext;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            SetupInfo = setupInfo;
            TenantManager = tenantManager;
            UserManager = userManager;
            AuthManager = authManager;
            AuthContext = authContext;
        }

        public SecurityContext SecurityContext { get; }
        public EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        public SetupInfo SetupInfo { get; }
        public TenantManager TenantManager { get; }
        public UserManager UserManager { get; }
        public AuthManager AuthManager { get; }
        public AuthContext AuthContext { get; }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var emailValidationKeyModel = EmailValidationKeyModel.FromRequest(Context.Request);

            if (!emailValidationKeyModel.Type.HasValue)
            {
                return SecurityContext.IsAuthenticated
                    ? Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)))
                    : Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString())));
            }

            EmailValidationKeyProvider.ValidationResult checkKeyResult;
            try
            {
                checkKeyResult = emailValidationKeyModel.Validate(EmailValidationKeyProvider, AuthContext, TenantManager, UserManager, AuthManager);
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

    public static class ConfirmAuthHandlerExtension
    {
        public static DIHelper AddConfirmAuthHandler(this DIHelper services)
        {
            return services
                .AddSecurityContextService()
                .AddEmailValidationKeyProviderService()
                .AddSetupInfo()
                .AddTenantManagerService()
                .AddUserManagerService()
                .AddAuthManager()
                .AddAuthContextService();
        }
    }
}
