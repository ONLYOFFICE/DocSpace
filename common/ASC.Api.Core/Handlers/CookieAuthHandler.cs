using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using ASC.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Web.Api.Handlers
{
    public class CookieAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor) 
            : this(options, logger, encoder, clock)
        {
            Common.HttpContext.Configure(httpContextAccessor);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Context.Request.Cookies["asc_auth_key"] ?? Context.Request.Headers["Authorization"];
            var result = SecurityContext.AuthenticateMe(token);

            if (!result)
            {
                throw new AuthenticationException(HttpStatusCode.Unauthorized.ToString());
            }
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)));
        }
    }
}
