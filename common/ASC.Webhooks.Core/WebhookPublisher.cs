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

namespace ASC.Webhooks.Core;

[Scope]
public class WebhookPublisher : IWebhookPublisher
{
    private readonly DbWorker _dbWorker;
    private readonly IEventBus _eventBus;
    private readonly SecurityContext _securityContext;
    private readonly TenantManager _tenantManager;

    public WebhookPublisher(
        DbWorker dbWorker,
        IEventBus eventBus,
        SecurityContext securityContext,
        TenantManager tenantManager)
    {
        _dbWorker = dbWorker;
        _eventBus = eventBus;
        _securityContext = securityContext;
        _tenantManager = tenantManager;
    }

    public async Task PublishAsync(int webhookId, string requestPayload)
    {
        if (string.IsNullOrEmpty(requestPayload))
        {
            return;
        }

        var webhookConfigs = _dbWorker.GetWebhookConfigs();

        await foreach (var config in webhookConfigs.Where(r => r.Enabled))
        {
            _ = await PublishAsync(webhookId, requestPayload, config.Id);
        }
    }

    public async Task<WebhooksLog> PublishAsync(int webhookId, string requestPayload, int configId)
    {
        if (string.IsNullOrEmpty(requestPayload))
        {
            return null;
        }

        var webhooksLog = new WebhooksLog
        {
            WebhookId = webhookId,
            CreationTime = DateTime.UtcNow,
            RequestPayload = requestPayload,
            ConfigId = configId
        };

        var webhook = await _dbWorker.WriteToJournal(webhooksLog);

        _eventBus.Publish(new WebhookRequestIntegrationEvent(
            _securityContext.CurrentAccount.ID,
            _tenantManager.GetCurrentTenant().Id)
        {
            WebhookId = webhook.Id
        });

        return webhook;
    }
}
