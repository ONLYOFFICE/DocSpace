using System.Linq;
using System.Net;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Api.Routing;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Middleware
{
    [Scope]
    public class PaymentFilter : IResourceFilter
    {
        private readonly ILog log;

        public PaymentFilter(IOptionsMonitor<ILog> options, TenantExtra tenantExtra)
        {
            log = options.CurrentValue;
            TenantExtra = tenantExtra;
        }

        private TenantExtra TenantExtra { get; }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor && !controllerActionDescriptor.EndpointMetadata.OfType<CustomHttpMethodAttribute>().FirstOrDefault().Check)
            {
                log.Debug("Payment is not required");
                return;
            }

            var header = context.HttpContext.Request.Headers["Payment-Info"];
            if (string.IsNullOrEmpty(header) || (bool.TryParse(header, out var flag) && flag))
            {
                if (TenantExtra.IsNotPaid())
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.PaymentRequired);
                    log.WarnFormat("Payment Required {0}.", context.HttpContext.Request.Url());
                }
            }
        }
    }
}
