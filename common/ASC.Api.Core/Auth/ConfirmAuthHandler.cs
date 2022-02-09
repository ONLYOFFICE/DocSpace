﻿using SecurityContext = ASC.Core.SecurityContext;

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
            UserManager userManager,
            IServiceProvider serviceProvider) :
            base(options, logger, encoder, clock)
        {
            SecurityContext = securityContext;
            UserManager = userManager;
            ServiceProvider = serviceProvider;
        }

        private SecurityContext SecurityContext { get; }
        private UserManager UserManager { get; }
        private IServiceProvider ServiceProvider { get; }

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
                Guid userId;
                if (!SecurityContext.IsAuthenticated)
                {
                    if (emailValidationKeyModel.UiD.HasValue && !emailValidationKeyModel.UiD.Equals(Guid.Empty))
                    {
                        userId = emailValidationKeyModel.UiD.Value;
                    }
                    else
                    {
                        if(emailValidationKeyModel.Type == Web.Studio.Utility.ConfirmType.EmailActivation ||
                            emailValidationKeyModel.Type == Web.Studio.Utility.ConfirmType.EmpInvite ||
                            emailValidationKeyModel.Type == Web.Studio.Utility.ConfirmType.LinkInvite)
                        {
                            userId = ASC.Core.Configuration.Constants.CoreSystem.ID;
                        }
                        else
                        {
                            userId = UserManager.GetUserByEmail(emailValidationKeyModel.Email).Id;
                        }
                    }
                }
                else
                {
                    userId = SecurityContext.CurrentAccount.ID;
                }

                SecurityContext.AuthenticateMeWithoutCookie(userId, claims);
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
