// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode


namespace ASC.Web.Api.Controllers.Settings;

public class WebhooksController : BaseSettingsController
{
    private readonly ApiContext _context;
    private readonly PermissionContext _permissionContext;
    private readonly DbWorker _webhookDbWorker;
    private readonly IMapper _mapper;
    private readonly WebhookPublisher _webhookPublisher;
    private readonly SettingsManager _settingsManager;

    public WebhooksController(
        ApiContext context,
        PermissionContext permissionContext,
        ApiContext apiContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        DbWorker dbWorker,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        WebhookPublisher webhookPublisher,
        SettingsManager settingsManager)
        : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _context = context;
        _permissionContext = permissionContext;
        _webhookDbWorker = dbWorker;
        _mapper = mapper;
        _webhookPublisher = webhookPublisher;
        _settingsManager = settingsManager;
    }

    /// <summary>
    /// Returns a list of the tenant webhooks.
    /// </summary>
    /// <short>
    /// Get webhooks
    /// </short>
    /// <category>Webhooks</category>
    /// <path>api/2.0/settings/webhook</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns type="System.Collections.Generic.IAsyncEnumerable{ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto}, System.Collections.Generic">List of tenant webhooks with their config parameters (URI, secret key, enabled or not)</returns>
    /// <collection>list</collection>
    [HttpGet("webhook")]
    public async IAsyncEnumerable<WebhooksConfigDto> GetTenantWebhooks()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        await foreach (var w in _webhookDbWorker.GetTenantWebhooks())
        {
            yield return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(w);
        }
    }

    /// <summary>
    /// Creates a new tenant webhook with the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Create a webhook
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.WebhooksConfigRequestsDto, ASC.Web.Api.ApiModels.RequestsDto" name="model">Webhook request parameters</param>
    /// <path>api/2.0/settings/webhook</path>
    /// <httpMethod>POST</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto, ASC.Web.Api.ApiModels.ResponseDto">Tenant webhook with its config parameters (URI, secret key, enabled or not)</returns>
    [HttpPost("webhook")]
    public async Task<WebhooksConfigDto> CreateWebhook(WebhooksConfigRequestsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        ArgumentNullException.ThrowIfNull(model.Uri);
        ArgumentNullException.ThrowIfNull(model.SecretKey);

        var webhook = await _webhookDbWorker.AddWebhookConfig(model.Uri, model.SecretKey);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    /// <summary>
    /// Updates the tenant webhook with the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Update a webhook
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.WebhooksConfigRequestsDto, ASC.Web.Api.ApiModels.RequestsDto" name="model">New webhook request parameters</param>
    /// <path>api/2.0/settings/webhook</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto, ASC.Web.Api.ApiModels.ResponseDto">Updated tenant webhook with its config parameters (URI, secret key, enabled or not)</returns>
    [HttpPut("webhook")]
    public async Task<WebhooksConfigDto> UpdateWebhook(WebhooksConfigRequestsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        ArgumentNullException.ThrowIfNull(model.Uri);
        ArgumentNullException.ThrowIfNull(model.SecretKey);

        var webhook = await _webhookDbWorker.UpdateWebhookConfig(model.Id, model.Uri, model.SecretKey, model.Enabled);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    /// <summary>
    /// Removes the tenant webhook with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Remove a webhook
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="System.Int32, System" method="url" name="id">Webhook ID</param>
    /// <path>api/2.0/settings/webhook</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto, ASC.Web.Api.ApiModels.ResponseDto">Tenant webhook with its config parameters (URI, secret key, enabled or not)</returns>
    [HttpDelete("webhook/{id}")]
    public async Task<WebhooksConfigDto> RemoveWebhook(int id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var webhook = await _webhookDbWorker.RemoveWebhookConfig(id);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    /// <summary>
    /// Returns the logs of the webhook activities.
    /// </summary>
    /// <short>
    /// Get webhook logs
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="System.Nullable{System.DateTime}, System" name="model">Webhook log request parameters</param>
    /// <path>api/2.0/settings/webhooks/log</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns type="System.Collections.Generic.IAsyncEnumerable{ASC.Web.Api.ApiModels.ResponseDto.WebhooksLogDto}, System.Collections.Generic">Logs of the webhook activities: ID, config name, creation time, method, route, request headers, request payload, response headers, response payload, status, delivery time</returns>
    /// <collection>list</collection>
    [HttpGet("webhooks/log")]
    public async IAsyncEnumerable<WebhooksLogDto> GetJournal(WebhooksLogRequest model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        var startIndex = Convert.ToInt32(_context.StartIndex);
        var count = Convert.ToInt32(_context.Count);

        await foreach (var j in _webhookDbWorker.ReadJournal(startIndex, count, model.Delivery, model.HookUri, model.WebhookId))
        {
            yield return _mapper.Map<WebhooksLog, WebhooksLogDto>(j);
        }
    }

    /// <summary>
    /// Retries a webhook with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Retry a webhook
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="System.Int32, System" method="url" name="id">Webhook ID</param>
    /// <path>api/2.0/settings/webhook/{id}/retry</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksLogDto, ASC.Web.Api.ApiModels.ResponseDto">Logs of the webhook activities: ID, config name, creation time, method, route, request headers, request payload, response headers, response payload, status, delivery time</returns>
    [HttpPut("webhook/{id}/retry")]
    public async Task<WebhooksLogDto> RetryWebhook(int id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (id == 0)
        {
            throw new ArgumentException(nameof(id));
        }

        var item = await _webhookDbWorker.ReadJournal(id);

        if (item == null)
        {
            throw new ItemNotFoundException();
        }

        if (item.Status >= 200 && item.Status <= 299 || item.Status == 0)
        {
            throw new HttpException(HttpStatusCode.Forbidden);
        }

        var result = await _webhookPublisher.PublishAsync(item.Id, item.RequestPayload, item.ConfigId);

        return _mapper.Map<WebhooksLog, WebhooksLogDto>(result);
    }

    /// <summary>
    /// Retries all the webhooks with the IDs specified in the request.
    /// </summary>
    /// <short>
    /// Retry webhooks
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.WebhookRetryRequestsDto, ASC.Web.Api.ApiModels.RequestsDto" name="model">Request parameters to retry webhooks</param>
    /// <path>api/2.0/settings/webhook/retry</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns type="System.Collections.Generic.IAsyncEnumerable{ASC.Web.Api.ApiModels.ResponseDto.WebhooksLogDto}, System.Collections.Generic">Logs of the webhook activities: ID, config name, creation time, method, route, request headers, request payload, response headers, response payload, status, delivery time</returns>
    /// <collection>list</collection>
    [HttpPut("webhook/retry")]
    public async IAsyncEnumerable<WebhooksLogDto> RetryWebhooks(WebhookRetryRequestsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        foreach (var id in model.Ids)
        {
            var item = await _webhookDbWorker.ReadJournal(id);

            if (item == null || item.Status >= 200 && item.Status <= 299 || item.Status == 0)
            {
                continue;
            }

            var result = await _webhookPublisher.PublishAsync(item.Id, item.RequestPayload, item.ConfigId);

            yield return _mapper.Map<WebhooksLog, WebhooksLogDto>(result);
        }
    }

    /// <summary>
    /// Returns the webhook SSL settings.
    /// </summary>
    /// <short>
    /// Get webhook SSL settings
    /// </short>
    /// <category>Webhooks</category>
    /// <path>api/2.0/settings/webhook/ssl</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksSslSettingsDto, ASC.Web.Api.ApiModels.ResponseDto">Webhook SSL settings: SSL certificate is enabled or not</returns>
    [HttpGet("webhook/ssl")]
    public WebhooksSslSettingsDto GetSslSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<WebHooksSettings>();

        return _mapper.Map<WebhooksSslSettingsDto>(settings);
    }

    /// <summary>
    /// Sets the webhook SSL settings.
    /// </summary>
    /// <short>
    /// Set webhook SSL settings
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="System.Boolean, System" method="url" name="isEnabled">Specifies if the SSL certificate is enabled or not</param>
    /// <path>api/2.0/settings/webhook/ssl/{isEnabled}</path>
    /// <httpMethod>POST</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksSslSettingsDto, ASC.Web.Api.ApiModels.ResponseDto">Webhook SSL settings: SSL certificate is enabled or not</returns>
    [HttpPost("webhook/ssl/{isEnabled}")]
    public WebhooksSslSettingsDto SetSslSettings(bool isEnabled)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<WebHooksSettings>();
        settings.EnableSSLVerification = isEnabled;
        _settingsManager.Save(settings);

        return _mapper.Map<WebhooksSslSettingsDto>(settings);
    }

    /// <summary>
    /// Returns settings of all webhooks.
    /// </summary>
    /// <short>
    /// Get webhook settings
    /// </short>
    /// <category>Webhooks</category>
    /// <path>api/2.0/settings/webhooks</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns type="System.Collections.Generic.IAsyncEnumerable{ASC.Webhooks.Core.Webhook}, System.Collections.Generic">List of webhook settings: ID, route, method, disabled or not, name, description, endpoint</returns>
    /// <collection>list</collection>
    [HttpGet("webhooks")]
    public async IAsyncEnumerable<Webhook> Settings()
    {
        var settings = _settingsManager.Load<WebHooksSettings>();

        foreach (var w in await _webhookDbWorker.GetWebhooksAsync())
        {
            w.Disable = settings.Ids.Contains(w.Id);
            yield return w;
        }
    }

    /// <summary>
    /// Disables a webhook with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Disable a webhook
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="System.Int32, System" method="url" name="id">Webhook ID</param>
    /// <path>api/2.0/settings/webhook/{id}</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns type="ASC.Webhooks.Core.Webhook, ASC.Webhooks.Core">Webhook settings: ID, route, method, disabled or not, name, description, endpoint</returns>
    [HttpPut("webhook/{id}")]
    public async Task<Webhook> DisableWebHook(int id)
    {
        var settings = _settingsManager.Load<WebHooksSettings>();

        Webhook result = null;

        if (!settings.Ids.Contains(id) && (result = await _webhookDbWorker.GetWebhookAsync(id)) != null)
        {
            settings.Ids.Add(id);
        }

        if (result != null)
        {
            _settingsManager.Save(settings);
        }

        return result;
    }
}