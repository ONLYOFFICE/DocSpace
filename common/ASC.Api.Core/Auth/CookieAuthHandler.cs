using System.Net;
using System.Security.Authentication;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ASC.Core;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Auth
{
    public class CookieAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }
        //
        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, SecurityContext securityContext)
            : this(options, logger, encoder, clock)
        {
            SecurityContext = securityContext;
        }

        private SecurityContext SecurityContext { get; }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Context.Request.Cookies["asc_auth_key"] ?? Context.Request.Headers["Authorization"];
            var result = SecurityContext.AuthenticateMe(token);

            return Task.FromResult(
                     result ?
                     AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)) :
                     AuthenticateResult.Fail(new AuthenticationException(HttpStatusCode.Unauthorized.ToString()))
                     );
        }
    }
}
