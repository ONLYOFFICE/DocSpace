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
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto, ASC.Web.Api">List of tenant webhooks with their config parameters</returns>
    /// <collection>list</collection>
    [HttpGet("webhook")]
    public async IAsyncEnumerable<WebhooksConfigWithStatusDto> GetTenantWebhooks()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await foreach (var webhook in _webhookDbWorker.GetTenantWebhooksWithStatus())
        {
            yield return _mapper.Map<WebhooksConfigWithStatusDto>(webhook);
        }
    }

    /// <summary>
    /// Creates a new tenant webhook with the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Create a webhook
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.WebhooksConfigRequestsDto, ASC.Web.Api" name="inDto">Webhook request parameters</param>
    /// <path>api/2.0/settings/webhook</path>
    /// <httpMethod>POST</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto, ASC.Web.Api">Tenant webhook with its config parameters</returns>
    [HttpPost("webhook")]
    public async Task<WebhooksConfigDto> CreateWebhook(WebhooksConfigRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        ArgumentNullException.ThrowIfNull(inDto.Uri);
        ArgumentNullException.ThrowIfNull(inDto.SecretKey);
        ArgumentNullException.ThrowIfNull(inDto.Name);

        var webhook = await _webhookDbWorker.AddWebhookConfig(inDto.Uri, inDto.Name, inDto.SecretKey, inDto.Enabled, inDto.SSL);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    /// <summary>
    /// Updates the tenant webhook with the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Update a webhook
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.WebhooksConfigRequestsDto, ASC.Web.Api" name="inDto">New webhook request parameters</param>
    /// <path>api/2.0/settings/webhook</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto, ASC.Web.Api">Updated tenant webhook with its config parameters</returns>
    [HttpPut("webhook")]
    public async Task<WebhooksConfigDto> UpdateWebhook(WebhooksConfigRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        ArgumentNullException.ThrowIfNull(inDto.Uri);
        ArgumentNullException.ThrowIfNull(inDto.Name);

        var webhook = await _webhookDbWorker.UpdateWebhookConfig(inDto.Id, inDto.Name, inDto.Uri, inDto.SecretKey, inDto.Enabled, inDto.SSL);

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
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksConfigDto, ASC.Web.Api">Tenant webhook with its config parameters</returns>
    [HttpDelete("webhook/{id}")]
    public async Task<WebhooksConfigDto> RemoveWebhook(int id)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var webhook = await _webhookDbWorker.RemoveWebhookConfigAsync(id);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    /// <summary>
    /// Returns the logs of the webhook activities.
    /// </summary>
    /// <short>
    /// Get webhook logs
    /// </short>
    /// <category>Webhooks</category>
    /// <param type="System.DateTime, System" name="deliveryFrom"></param>
    /// <param type="System.DateTime, System" name="deliveryTo"></param>
    /// <param type="System.String, System" name="hookUri"></param>
    /// <param type="System.Int32, System" name="webhookId"></param>
    /// <param type="System.Int32, System" name="configId"></param>
    /// <param type="System.Int32, System" name="eventId"></param>
    /// <param type="ASC.Webhooks.Core.WebhookGroupStatus, ASC.Webhooks.Core" name="groupStatus"></param>
    /// <path>api/2.0/settings/webhooks/log</path>
    /// <httpMethod>GET</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksLogDto, ASC.Web.Api">Logs of the webhook activities</returns>
    /// <collection>list</collection>
    [HttpGet("webhooks/log")]
    public async IAsyncEnumerable<WebhooksLogDto> GetJournal(DateTime? deliveryFrom, DateTime? deliveryTo, string hookUri, int? webhookId, int? configId, int? eventId, WebhookGroupStatus? groupStatus)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        _context.SetTotalCount(await _webhookDbWorker.GetTotalByQuery(deliveryFrom, deliveryTo, hookUri, webhookId, configId, eventId, groupStatus));

        var startIndex = Convert.ToInt32(_context.StartIndex);
        var count = Convert.ToInt32(_context.Count);

        await foreach (var j in _webhookDbWorker.ReadJournal(startIndex, count, deliveryFrom, deliveryTo, hookUri, webhookId, configId, eventId, groupStatus))
        {
            j.Log.Config = j.Config;
            yield return _mapper.Map<WebhooksLog, WebhooksLogDto>(j.Log);
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
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksLogDto, ASC.Web.Api">Logs of the webhook activities</returns>
    [HttpPut("webhook/{id}/retry")]
    public async Task<WebhooksLogDto> RetryWebhook(int id)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (id == 0)
        {
            throw new ArgumentException(nameof(id));
        }

        var item = await _webhookDbWorker.ReadJournal(id);

        if (item == null)
        {
            throw new ItemNotFoundException();
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
    /// <param type="ASC.Web.Api.ApiModels.RequestsDto.WebhookRetryRequestsDto, ASC.Web.Api" name="inDto">Request parameters to retry webhooks</param>
    /// <path>api/2.0/settings/webhook/retry</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WebhooksLogDto, ASC.Web.Api">Logs of the webhook activities</returns>
    /// <collection>list</collection>
    [HttpPut("webhook/retry")]
    public async IAsyncEnumerable<WebhooksLogDto> RetryWebhooks(WebhookRetryRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        foreach (var id in inDto.Ids)
        {
            var item = await _webhookDbWorker.ReadJournal(id);

            if (item == null)
            {
                continue;
            }

            var result = await _webhookPublisher.PublishAsync(item.Id, item.RequestPayload, item.ConfigId);

            yield return _mapper.Map<WebhooksLog, WebhooksLogDto>(result);
        }
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
    /// <returns type="ASC.Webhooks.Core.Webhook, ASC.Webhooks.Core">List of webhook settings</returns>
    /// <collection>list</collection>
    [HttpGet("webhooks")]
    public async IAsyncEnumerable<Webhook> Settings()
    {
        var settings = await _settingsManager.LoadAsync<WebHooksSettings>();

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
    /// <returns type="ASC.Webhooks.Core.Webhook, ASC.Webhooks.Core">Webhook settings</returns>
    [HttpPut("webhook/{id}")]
    public async Task<Webhook> DisableWebHook(int id)
    {
        var settings = await _settingsManager.LoadAsync<WebHooksSettings>();

        Webhook result = null;

        if (!settings.Ids.Contains(id) && (result = await _webhookDbWorker.GetWebhookAsync(id)) != null)
        {
            settings.Ids.Add(id);
        }

        if (result != null)
        {
            await _settingsManager.SaveAsync(settings);
        }

        return result;
    }
}