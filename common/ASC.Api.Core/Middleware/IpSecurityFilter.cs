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
    [Scope]
    public class IpSecurityFilter : IResourceFilter
    {
        private readonly ILog log;

        public IpSecurityFilter(
            IOptionsMonitor<ILog> options,
            AuthContext authContext,
            IPSecurity.IPSecurity IPSecurity,
            SettingsManager settingsManager)
        {
            log = options.CurrentValue;
            AuthContext = authContext;
            this.IPSecurity = IPSecurity;
            SettingsManager = settingsManager;
        }

        private AuthContext AuthContext { get; }
        private IPSecurity.IPSecurity IPSecurity { get; }
        private SettingsManager SettingsManager { get; }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {

            if (AuthContext.IsAuthenticated)
            {
                var enable = SettingsManager.Load<IPRestrictionsSettings>().Enable;

                if (enable && !IPSecurity.Verify())
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                    log.WarnFormat("IPSecurity: user {0}", AuthContext.CurrentAccount.ID);
                    return;
                }
            }
        }
    }
}
