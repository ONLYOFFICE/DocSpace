using System.Collections.Generic;

using ASC.Common;

using Microsoft.Extensions.Primitives;

namespace ASC.Webhooks.Core
{
    [Scope]
    public interface IWebhookPublisher
    {
        public void Publish(string eventName, string requestPayload);
    }
}
