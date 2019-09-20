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

        public IpSecurityFilter(
            LogManager logManager,
            AuthContext authContext,
            IPSecurity.IPSecurity IPSecurity)
        {
            log = logManager.Get("Api");
            AuthContext = authContext;
            this.IPSecurity = IPSecurity;
        }

        public AuthContext AuthContext { get; }
        public IPRestrictionsSettings IPRestrictionsSettings { get; }
        public IPSecurity.IPSecurity IPSecurity { get; }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (AuthContext.IsAuthenticated && !IPSecurity.Verify())
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                log.WarnFormat("IPSecurity: user {0}", AuthContext.CurrentAccount.ID);
                return;
            }
        }
    }
}
