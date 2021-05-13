using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using ASC.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ASC.Api.Core.Middleware
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate next;

        public CultureMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, UserManager userManager, TenantManager tenantManager, AuthContext authContext)
        {
            CultureInfo culture = null;

            if (authContext.IsAuthenticated)
            {
                var user = userManager.GetUsers(authContext.CurrentAccount.ID);

                if (!string.IsNullOrEmpty(user.CultureName))
                {
                    culture = user.GetCulture();
                }
            }

            if (culture == null)
            {
                culture = tenantManager.GetCurrentTenant().GetCulture();
            }

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            await next.Invoke(context);
        }
    }

    public static class CultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseCultureMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureMiddleware>();
        }
    }
}
