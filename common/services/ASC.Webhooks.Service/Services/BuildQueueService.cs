namespace ASC.Webhooks.Service.Services;

[Singletone]
public class BuildQueueService : IHostedService
{
    internal readonly ConcurrentQueue<WebhookRequest> _queue;
    private readonly ICacheNotify<WebhookRequest> _webhookNotify;

    public BuildQueueService(ICacheNotify<WebhookRequest> webhookNotify)
    {
        _webhookNotify = webhookNotify;
        _queue = new ConcurrentQueue<WebhookRequest>();
    }
    public void BuildWebhooksQueue(WebhookRequest request)
    {
        _queue.Enqueue(request);
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
}