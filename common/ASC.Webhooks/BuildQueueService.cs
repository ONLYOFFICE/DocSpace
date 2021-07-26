using ASC.Common;
using ASC.Common.Caching;
using ASC.Web.Webhooks;

namespace ASC.Webhooks
{
    [Singletone]
    public class BuildQueueService
    {
        private ICacheNotify<WebhookRequest> WebhookNotify { get; }
        public BuildQueueService(ICacheNotify<WebhookRequest> webhookNotify)
        {
            WebhookNotify = webhookNotify;
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
            WebhookHostedService.Queue.Enqueue(request);
        }
    }
}
