using System.Net;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.IPSecurity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Middleware
{
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

    public static class IpSecurityFilterExtension
    {
        public static DIHelper AddIpSecurityFilter(this DIHelper services)
        {
            return services
                .AddSettingsManagerService()
                .AddAuthContextService()
                .AddIPSecurityService();
        }
    }
}
