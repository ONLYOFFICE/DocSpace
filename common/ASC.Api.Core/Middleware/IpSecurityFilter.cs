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

        public IpSecurityFilter(LogManager logManager, AuthContext authContext, IPRestrictionsSettings iPRestrictionsSettings)
        {
            log = logManager.Get("Api");
            AuthContext = authContext;
            IPRestrictionsSettings = iPRestrictionsSettings;
        }

        public AuthContext AuthContext { get; }
        public IPRestrictionsSettings IPRestrictionsSettings { get; }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant(context.HttpContext);
            var settings = IPRestrictionsSettings.Load();
            if (settings.Enable && AuthContext.IsAuthenticated && !IPSecurity.IPSecurity.Verify(context.HttpContext, tenant, AuthContext))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                log.WarnFormat("IPSecurity: Tenant {0}, user {1}", tenant.TenantId, AuthContext.CurrentAccount.ID);
                return;
            }
        }
    }
}
