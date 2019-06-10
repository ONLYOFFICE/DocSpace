using ASC.Common.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace ASC.Common
{
    public static class HttpContext
    {
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                var currentContext = CommonServiceProvider.GetService<IHttpContextAccessor>();
                return currentContext?.HttpContext;
            }
        }
    }
}