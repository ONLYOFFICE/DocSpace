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
            var result = SecurityContext.AuthenticateMe(Context.Request.Cookies["asc_auth_key"]);

            return Task.FromResult(
             result ?  
             AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(Thread.CurrentPrincipal), new AuthenticationProperties(), Scheme.Name)) : 
             AuthenticateResult.Fail("fail")
             );
        }
    }
}
