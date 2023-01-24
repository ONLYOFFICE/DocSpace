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

    public WebhooksController(
        ApiContext context,
        PermissionContext permissionContext,
        ApiContext apiContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache,
        DbWorker dbWorker,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        WebhookPublisher webhookPublisher)
        : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _context = context;
        _permissionContext = permissionContext;
        _webhookDbWorker = dbWorker;
        _mapper = mapper;
        _webhookPublisher = webhookPublisher;
    }

    [HttpGet("webhook")]
    public async IAsyncEnumerable<WebhooksConfigDto> GetTenantWebhooks()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        await foreach (var w in _webhookDbWorker.GetTenantWebhooks())
        {
            yield return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(w);
        }
    }

    [HttpPost("webhook")]
    public async Task<WebhooksConfigDto> CreateWebhook(WebhooksConfigRequestsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        ArgumentNullException.ThrowIfNull(model.Uri);
        ArgumentNullException.ThrowIfNull(model.SecretKey);

        var webhook = await _webhookDbWorker.AddWebhookConfig(model.Name, model.Uri, model.SecretKey);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    [HttpPut("webhook")]
    public async Task<WebhooksConfigDto> UpdateWebhook(WebhooksConfigRequestsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        ArgumentNullException.ThrowIfNull(model.Uri);
        ArgumentNullException.ThrowIfNull(model.SecretKey);

        var webhook = await _webhookDbWorker.UpdateWebhookConfig(model.Id, model.Name, model.Uri, model.SecretKey, model.Enabled);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    [HttpDelete("webhook/{id}")]
    public async Task<WebhooksConfigDto> RemoveWebhook(int id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var webhook = await _webhookDbWorker.RemoveWebhookConfig(id);

        return _mapper.Map<WebhooksConfig, WebhooksConfigDto>(webhook);
    }

    [HttpGet("webhooks/log")]
    public async IAsyncEnumerable<WebhooksLogDto> GetJournal(WebhooksLogRequest model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        var startIndex = Convert.ToInt32(_context.StartIndex);
        var count = Convert.ToInt32(_context.Count);

        await foreach (var j in _webhookDbWorker.ReadJournal(startIndex, count, model.Delivery, model.Hookname, model.Route))
        {
            yield return _mapper.Map<WebhooksLog, WebhooksLogDto>(j);
        }
    }

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

        var result = await _webhookPublisher.PublishAsync(item.Method, item.Route, item.RequestPayload, item.ConfigId);

        return _mapper.Map<WebhooksLog, WebhooksLogDto>(result);
    }

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

            var result = await _webhookPublisher.PublishAsync(item.Method, item.Route, item.RequestPayload, item.ConfigId);

            yield return _mapper.Map<WebhooksLog, WebhooksLogDto>(result);
        }
    }
}