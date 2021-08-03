using System.Collections.Generic;

using ASC.Common;
using ASC.Common.Utils;

using Microsoft.Extensions.Configuration;

namespace ASC.Webhooks
{
    [Singletone]
    public class WebhooksIdentifier
    {
        private WebhooksRoutes routes;
        public WebhooksIdentifier(ConfigurationExtension configuration, IConfiguration configuration1)
        {
            routes = configuration.GetSetting<WebhooksRoutes>("webhooks");
        }

        public bool Identify(string method)
        {
            return routes.routeList.Contains(method);
        }
    }

    public class WebhooksRoutes
    {
        public List<string> routeList { get; set; }
    }
}
