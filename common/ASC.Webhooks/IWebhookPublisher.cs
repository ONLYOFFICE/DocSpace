using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;

namespace ASC.Webhooks
{
    [Scope]
    public interface IWebhookPublisher
    {
        public void Publish(string eventName, object data);
    }
}
