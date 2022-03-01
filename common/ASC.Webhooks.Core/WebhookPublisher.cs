namespace ASC.Webhooks.Core;

[Scope]
public class WebhookPublisher : IWebhookPublisher
{
    private readonly DbWorker _dbWorker;
    private readonly TenantManager _tenantManager;
    private readonly ICacheNotify<WebhookRequest> _webhookNotify;

    public WebhookPublisher(
        DbWorker dbWorker,
        TenantManager tenantManager,
        IOptionsMonitor<ILog> options,
        ICacheNotify<WebhookRequest> webhookNotify)
    {
        _dbWorker = dbWorker;
        _tenantManager = tenantManager;
        _webhookNotify = webhookNotify;
    }

    public void Publish(string eventName, string requestPayload)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;
        var webhookConfigs = _dbWorker.GetWebhookConfigs(tenantId);

        foreach (var config in webhookConfigs)
        {
            var webhooksLog = new WebhooksLog
            {
                Uid = Guid.NewGuid().ToString(),
                TenantId = tenantId,
                Event = eventName,
                CreationTime = DateTime.UtcNow,
                RequestPayload = requestPayload,
                Status = ProcessStatus.InProcess,
                ConfigId = config.ConfigId
            };
            var DbId = _dbWorker.WriteToJournal(webhooksLog);

            var request = new WebhookRequest()
            {
                Id = DbId
            };

            _webhookNotify.Publish(request, CacheNotifyAction.Update);
        }
    }
}

public enum ProcessStatus
{
    InProcess,
    Success,
    Failed
}
