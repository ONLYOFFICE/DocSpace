namespace ASC.Webhooks.Service.Services;

[Singletone]
public class BuildQueueService : BackgroundService
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