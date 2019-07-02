using System.Net;
using ASC.Common.Logging;
using ASC.Core;
using ASC.IPSecurity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASC.Api.Core.Middleware
{
    public class IpSecurityFilter : IResourceFilter
    {
        private readonly ILog log;

        public IpSecurityFilter(LogManager logManager)
        {
            log = logManager.Get("Api");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            var settings = IPRestrictionsSettings.LoadForTenant(tenant.TenantId);
            if (settings.Enable && SecurityContext.IsAuthenticated && !IPSecurity.IPSecurity.Verify(context.HttpContext, tenant))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                log.WarnFormat("IPSecurity: Tenant {0}, user {1}", tenant.TenantId, SecurityContext.CurrentAccount.ID);
                return;
            }
        }
    }
}
