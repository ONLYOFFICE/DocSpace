using ASC.Common.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace ASC.Common
{
    public static class HttpContext
    {
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            Current = httpContextAccessor.HttpContext;
            CommonServiceProvider.Current = httpContextAccessor.HttpContext.RequestServices;
        }
        public static void Configure(Microsoft.AspNetCore.Http.HttpContext context)
        {
            Current = context;
            CommonServiceProvider.Current = context.RequestServices;
        }

        public static Microsoft.AspNetCore.Http.HttpContext Current { get; private set; }
    }
}