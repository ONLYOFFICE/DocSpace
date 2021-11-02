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
        private ICacheNotify<WebhookRequest> WebhookNotify { get; }

        public BuildQueueService(ICacheNotify<WebhookRequest> webhookNotify)
        {
            WebhookNotify = webhookNotify;
            Queue = new ConcurrentQueue<WebhookRequest>();
        }

        public void Start()
        {
            WebhookNotify.Subscribe(BuildWebhooksQueue, CacheNotifyAction.Update);
        }

        public void Stop()
        {
            WebhookNotify.Unsubscribe(CacheNotifyAction.Update);
        }

        public void BuildWebhooksQueue(WebhookRequest request)
        {
            Queue.Enqueue(request);
        }
    }
}
