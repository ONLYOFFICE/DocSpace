using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.Core.Auth;

[Scope(Additional = typeof(ConfirmAuthHandlerExtension))]
public class ConfirmAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ConfirmAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) :
        base(options, logger, encoder, clock)
    { }

    public ConfirmAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        SecurityContext securityContext,
        UserManager userManager,
        IServiceScopeFactory serviceScopeFactory) :
        base(options, logger, encoder, clock)
    {
        _securityContext = securityContext;
        _userManager = userManager;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var emailValidationKeyHelper = scope.ServiceProvider.GetService<EmailValidationKeyModelHelper>();
        var emailValidationKeyModel = emailValidationKeyHelper.GetModel();

        if (!emailValidationKeyModel.Type.HasValue)
        {
            return _securityContext.IsAuthenticated
                ? Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)))
                    : Task.FromResult(AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized))));
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
            if (!_securityContext.IsAuthenticated)
            {
                if (emailValidationKeyModel.UiD.HasValue && !emailValidationKeyModel.UiD.Equals(Guid.Empty))
                {
                    userId = emailValidationKeyModel.UiD.Value;
                }
                else
                {
                    if (emailValidationKeyModel.Type == ConfirmType.EmailActivation
                        || emailValidationKeyModel.Type == ConfirmType.EmpInvite
                        || emailValidationKeyModel.Type == ConfirmType.LinkInvite)
                    {
                        userId = ASC.Core.Configuration.Constants.CoreSystem.ID;
                    }
                    else
                    {
                        userId = _userManager.GetUserByEmail(emailValidationKeyModel.Email).Id;
                    }
                }
            }
            else
            {
                userId = _securityContext.CurrentAccount.ID;
            }

            _securityContext.AuthenticateMeWithoutCookie(userId, claims);
        }

        var result = checkKeyResult switch
        {
            EmailValidationKeyProvider.ValidationResult.Ok => AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)),
            _ => AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized)))
        };

        return Task.FromResult(result);
    }
}

public static class ConfirmAuthHandlerExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EmailValidationKeyModelHelper>();
    }
}