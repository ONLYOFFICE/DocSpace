using System.Net;
using System.Security.Authentication;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.Helpers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Auth
{
    [Scope]
    public class CookieAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private AuthorizationHelper AuthorizationHelper { get; }
        private SecurityContext SecurityContext { get; }
        private CookiesManager CookiesManager { get; }

        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }
        //
        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            AuthorizationHelper authorizationHelper,
            SecurityContext securityContext,
            CookiesManager cookiesManager)
            : this(options, logger, encoder, clock)
        {
            AuthorizationHelper = authorizationHelper;
            SecurityContext = securityContext;
            CookiesManager = cookiesManager;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = AuthorizationHelper.ProcessBasicAuthorization(out _);

            if (!result)
            {
                SecurityContext.Logout();
                CookiesManager.ClearCookies(CookiesType.AuthKey);
                CookiesManager.ClearCookies(CookiesType.SocketIO);
            }

            return Task.FromResult(
                     result ?
                     AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)) :
                     AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString()))
                     );
        }
    }
}
