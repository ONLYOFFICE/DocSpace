using System.Collections.Concurrent;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Web.Webhooks;

namespace ASC.Webhooks
{
    [Singletone]
    public class BuildQueueService
    {
        internal readonly ConcurrentQueue<WebhookRequest> queue;
        private ICacheNotify<WebhookRequest> WebhookNotify { get; }     
        public BuildQueueService(ICacheNotify<WebhookRequest> webhookNotify)
        {
            WebhookNotify = webhookNotify;
            queue = new ConcurrentQueue<WebhookRequest>();
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
            queue.Enqueue(request);
        }
    }
}
