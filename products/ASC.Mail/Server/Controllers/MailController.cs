using ASC.Api.Core;
using ASC.Common.Logging;
using ASC.Core.Tenants;
using ASC.Web.Api.Routing;
using ASC.Web.Studio.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Mail.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class MailController : ControllerBase
    {
        public Tenant Tenant { get { return ApiContext.Tenant; } }

        public ApiContext ApiContext { get; }

        public ILog Log { get; }

        public MailController(
            ApiContext apiContext, 
            IOptionsMonitor<ILog> option)
        {
            ApiContext = apiContext;
            Log = option.Get("ASC.Api");
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new MailProduct();
            product.Init();
            return new Module(product, false);
        }
    }

    public static class MailControllerExtention
    {
        public static IServiceCollection AddMailController(this IServiceCollection services)
        {
            return services
                .AddApiContextService();
        }
    }
}