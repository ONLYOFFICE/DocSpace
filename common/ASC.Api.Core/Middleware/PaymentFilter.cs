using System.Linq;
using System.Net;
using System.Web;
using ASC.Common.Logging;
using ASC.Web.Api.Routing;
using ASC.Web.Studio.UserControls.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASC.Api.Core.Middleware
{
    public class PaymentFilter : IResourceFilter
    {
        private readonly ILog log;

        public PaymentFilter(LogManager logManager)
        {
            log = logManager.Get("Api");
        }

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
                if (TenantStatisticsProvider.IsNotPaid())
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.PaymentRequired);
                    log.WarnFormat("Payment Required {0}.", context.HttpContext.Request.Url());
                }
            }
        }
    }
}
