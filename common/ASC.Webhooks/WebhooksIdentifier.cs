using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Webhooks.Dao.Models;

using Microsoft.Extensions.Configuration;

namespace ASC.Webhooks
{
    [Singletone]
    public class WebhooksIdentifier
    {
        private WebhooksDictionary webhooksDictionary;
        public WebhooksIdentifier(ConfigurationExtension configuration, IConfiguration configuration1)
        {
            webhooksDictionary = configuration.GetSetting<WebhooksDictionary>("webhooks");
        }

        public EventName Identify(string method)
        {
            foreach (var d in webhooksDictionary.dictionary)
            {
                if (d.Value.methodsName.Contains(method))
                {
                    return d.Key;
                }
            }
            return EventName.UntrackedAction;
        }
    }

    public class WebhooksDictionary
    {
        public Dictionary<EventName, Methods> dictionary { get; set; }
    }

    public class Methods
    {
        public List<string> methodsName { get; set; }
    }
}
