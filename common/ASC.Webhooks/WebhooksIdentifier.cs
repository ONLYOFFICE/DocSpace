using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Utils;
using ASC.Webhooks.Dao.Models;

namespace ASC.Webhooks
{
    [Singletone]
    public class WebhooksIdentifier
    {
        private Dictionary<EventName, List<string>> dictionary;
        public WebhooksIdentifier(ConfigurationExtension configuration)
        {
            dictionary = configuration.GetSetting<Dictionary<EventName, List<string>>>("webhooksDictionary");
        }

        public EventName Identify(string method)
        {
            foreach (var d in dictionary)
            {
                if (d.Value.Contains(method))
                {
                    return d.Key;
                }
            }
            return EventName.UntrackedAction;
        }
    }
}
