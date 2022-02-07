using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.Core.Auth;

[Scope]
public class CookieAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AuthorizationHelper _authorizationHelper;
    private readonly SecurityContext _securityContext;
    private readonly CookiesManager _cookiesManager;

    public CookieAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
        AuthorizationHelper authorizationHelper,
        SecurityContext securityContext,
        CookiesManager cookiesManager)
        : this(options, logger, encoder, clock)
    {
        _authorizationHelper = authorizationHelper;
        _securityContext = securityContext;
        _cookiesManager = cookiesManager;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = _authorizationHelper.ProcessBasicAuthorization(out _);
        if (!result)
        {
            _securityContext.Logout();
            _cookiesManager.ClearCookies(CookiesType.AuthKey);
            _cookiesManager.ClearCookies(CookiesType.SocketIO);
        }

        return Task.FromResult(
                 result ?
                 AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)) :
                 AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString())));
    }
}