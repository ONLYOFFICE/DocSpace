using System.Collections.Concurrent;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Web.Webhooks;

namespace ASC.Webhooks.Service
{
    [Singletone]
    public class BuildQueueService
    {
        internal ConcurrentQueue<WebhookRequest> Queue { get; }
        private IEventBus<WebhookRequest> WebhookNotify { get; }

        public BuildQueueService(IEventBus<WebhookRequest> webhookNotify)
        {
            WebhookNotify = webhookNotify;
            Queue = new ConcurrentQueue<WebhookRequest>();
        }

        public void Start()
        {
            WebhookNotify.Subscribe(BuildWebhooksQueue, EventType.Update);
        }

        public void Stop()
        {
            WebhookNotify.Unsubscribe(EventType.Update);
        }

        public void BuildWebhooksQueue(WebhookRequest request)
        {
            Queue.Enqueue(request);
        }
    }
}
