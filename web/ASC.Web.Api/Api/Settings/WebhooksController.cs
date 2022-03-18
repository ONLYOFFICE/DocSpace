namespace ASC.Web.Api.Controllers.Settings;

public class WebhooksController: BaseSettingsController
{
    private readonly DbWorker _webhookDbWorker;

    public WebhooksController(
        ApiContext apiContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        DbWorker dbWorker) : base(apiContext, memoryCache, webItemManager)
    {
        _webhookDbWorker = dbWorker;
    }

    /// <summary>
    /// Add new config for webhooks
    /// </summary>
    [Create("webhook")]
    public void CreateWebhook(WebhooksConfig model)
    {
        if (model.Uri == null) throw new ArgumentNullException("Uri");
        if (model.SecretKey == null) throw new ArgumentNullException("SecretKey");
        _webhookDbWorker.AddWebhookConfig(model);
    }

    /// <summary>
    /// Update config for webhooks
    /// </summary>
    [Update("webhook")]
    public void UpdateWebhook(WebhooksConfig model)
    {
        if (model.Uri == null) throw new ArgumentNullException("Uri");
        if (model.SecretKey == null) throw new ArgumentNullException("SecretKey");
        _webhookDbWorker.UpdateWebhookConfig(model);
    }

    /// <summary>
    /// Remove config for webhooks
    /// </summary>
    [Delete("webhook")]
    public void RemoveWebhook(WebhooksConfig model)
    {
        if (model.Uri == null) throw new ArgumentNullException("Uri");
        if (model.SecretKey == null) throw new ArgumentNullException("SecretKey");
        _webhookDbWorker.RemoveWebhookConfig(model);
    }

    /// <summary>
    /// Read Webhooks history for actual tenant
    /// </summary>
    [Read("webhooks")]
    public List<WebhooksLog> TenantWebhooks()
    {
        return _webhookDbWorker.GetTenantWebhooks();
    }
}