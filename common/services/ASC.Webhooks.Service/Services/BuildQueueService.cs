namespace ASC.Webhooks.Service.Services;

[Singletone]
public class BuildQueueService : IHostedService
{
    private readonly ICacheNotify<WebhookRequest> _webhookNotify;
    public BuildQueueService(ICacheNotify<WebhookRequest> webhookNotify)
    {
        _webhookNotify = webhookNotify;
        Queue = new ConcurrentQueue<WebhookRequest>();
    }

    internal ConcurrentQueue<WebhookRequest> Queue { get; }
    public void BuildWebhooksQueue(WebhookRequest request)
    {
        Queue.Enqueue(request);
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