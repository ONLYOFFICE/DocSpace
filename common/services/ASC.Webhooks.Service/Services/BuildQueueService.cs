namespace ASC.Webhooks.Service.Services;

[Singletone]
public class BuildQueueService : BackgroundService
{
    internal readonly ConcurrentQueue<WebhookRequest> Queue;
    private readonly ICacheNotify<WebhookRequest> _webhookNotify;

    public BuildQueueService(ICacheNotify<WebhookRequest> webhookNotify)
    {
        _webhookNotify = webhookNotify;
        Queue = new ConcurrentQueue<WebhookRequest>();
    }
    public void BuildWebhooksQueue(WebhookRequest request)
    {
        Queue.Enqueue(request);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _webhookNotify.Subscribe(BuildWebhooksQueue, CacheNotifyAction.Update);

        stoppingToken.Register(() =>
        {
            _webhookNotify.Unsubscribe(CacheNotifyAction.Update);
        });

        return Task.CompletedTask;
    }
}