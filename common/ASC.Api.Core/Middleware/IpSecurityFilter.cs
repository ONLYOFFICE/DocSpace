using System.Net;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.IPSecurity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Middleware
{
    [Scope]
    public class IpSecurityFilter : IResourceFilter
    {
        private readonly ILog log;

        public IpSecurityFilter(
            IOptionsMonitor<ILog> options,
            AuthContext authContext,
            IPSecurity.IPSecurity IPSecurity)
        {
            log = options.CurrentValue;
            AuthContext = authContext;
            this.IPSecurity = IPSecurity;
        }

        private AuthContext AuthContext { get; }
        public IPRestrictionsSettings IPRestrictionsSettings { get; }
        private IPSecurity.IPSecurity IPSecurity { get; }

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
