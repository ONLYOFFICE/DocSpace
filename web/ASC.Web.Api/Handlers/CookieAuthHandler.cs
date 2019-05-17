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
        protected SecurityContext SecurityContext { get; set; }

        protected IHttpContextAccessor HttpContextAccessor { get; set; }

        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        public CookieAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, SecurityContext securityContext, IHttpContextAccessor httpContextAccessor) 
            : this(options, logger, encoder, clock)
        {
            SecurityContext = securityContext;
            HttpContextAccessor = httpContextAccessor;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            ASC.Common.HttpContext.Configure(HttpContextAccessor);
            SecurityContext.AuthenticateMe(Context.Request.Cookies["asc_auth_key"]);

            return Task.FromResult(
             AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(Thread.CurrentPrincipal),
                    new AuthenticationProperties(),
                    Scheme.Name)));
        }
    }
}
