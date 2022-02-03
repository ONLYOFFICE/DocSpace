using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Web.Webhooks;

using Microsoft.Extensions.Hosting;

namespace ASC.Webhooks.Service.Services;

[Singletone]
public class BuildQueueService : IHostedService
{
    internal ConcurrentQueue<WebhookRequest> Queue { get; }
    private readonly ICacheNotify<WebhookRequest> _webhookNotify;

    public BuildQueueService(ICacheNotify<WebhookRequest> webhookNotify)
    {
        _webhookNotify = webhookNotify;
        Queue = new ConcurrentQueue<WebhookRequest>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _webhookNotify.Subscribe(BuildWebhooksQueue, CacheNotifyAction.Update);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _webhookNotify.Unsubscribe(CacheNotifyAction.Update);
        return Task.CompletedTask;
    }

    public void BuildWebhooksQueue(WebhookRequest request)
    {
        Queue.Enqueue(request);
    }
}
