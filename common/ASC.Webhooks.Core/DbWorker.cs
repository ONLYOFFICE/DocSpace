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
public class DbWorker
{
    private readonly IDbContextFactory<WebhooksDbContext> _dbContextFactory;
    private readonly TenantManager _tenantManager;
    public DbWorker(IDbContextFactory<WebhooksDbContext> dbContextFactory, TenantManager tenantManager)
    {
        _dbContextFactory = dbContextFactory;
        _tenantManager = tenantManager;
    }
    public void AddWebhookConfig(WebhooksConfig webhooksConfig)
    {
        webhooksConfig.TenantId = _tenantManager.GetCurrentTenant().Id;

        using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var addObj = webhooksDbContext.WebhooksConfigs.Where(it =>
        it.SecretKey == webhooksConfig.SecretKey &&
        it.TenantId == webhooksConfig.TenantId &&
        it.Uri == webhooksConfig.Uri).FirstOrDefault();

        if (addObj != null)
        {
            return;
        }

        webhooksDbContext.WebhooksConfigs.Add(webhooksConfig);
        webhooksDbContext.SaveChanges();
    }

    public int ConfigsNumber()
    {
        using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        return webhooksDbContext.WebhooksConfigs.Count();
    }

    public List<WebhooksLog> GetTenantWebhooks()
    {
        var tenant = _tenantManager.GetCurrentTenant().Id;

        using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        return webhooksDbContext.WebhooksLogs.Where(it => it.TenantId == tenant)
                .Select(t => new WebhooksLog
                {
                    Uid = t.Uid,
                    CreationTime = t.CreationTime,
                    RequestPayload = t.RequestPayload,
                    RequestHeaders = t.RequestHeaders,
                    ResponsePayload = t.ResponsePayload,
                    ResponseHeaders = t.ResponseHeaders,
                    Status = t.Status
                }).ToList();
    }

    public List<WebhooksConfig> GetWebhookConfigs(int tenant)
    {
        using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        return webhooksDbContext.WebhooksConfigs.Where(t => t.TenantId == tenant).ToList();
    }

    public WebhookEntry ReadFromJournal(int id)
    {
        using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        return webhooksDbContext.WebhooksLogs
            .Where(it => it.Id == id)
            .Join(webhooksDbContext.WebhooksConfigs, t => t.ConfigId, t => t.ConfigId, (payload, config) => new { payload, config })
            .Select(t => new WebhookEntry { Id = t.payload.Id, Payload = t.payload.RequestPayload, SecretKey = t.config.SecretKey, Uri = t.config.Uri })
            .OrderBy(t => t.Id).FirstOrDefault();
    }

    public void RemoveWebhookConfig(WebhooksConfig webhooksConfig)
    {
        webhooksConfig.TenantId = _tenantManager.GetCurrentTenant().Id;

        using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        var removeObj = webhooksDbContext.WebhooksConfigs.Where(it =>
        it.SecretKey == webhooksConfig.SecretKey &&
        it.TenantId == webhooksConfig.TenantId &&
        it.Uri == webhooksConfig.Uri).FirstOrDefault();

        webhooksDbContext.WebhooksConfigs.Remove(removeObj);
        webhooksDbContext.SaveChanges();
    }

    public void UpdateWebhookConfig(WebhooksConfig webhooksConfig)
    {
        webhooksConfig.TenantId = _tenantManager.GetCurrentTenant().Id;

        using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        var updateObj = webhooksDbContext.WebhooksConfigs.Where(it =>
        it.SecretKey == webhooksConfig.SecretKey &&
        it.TenantId == webhooksConfig.TenantId &&
        it.Uri == webhooksConfig.Uri).FirstOrDefault();

        webhooksDbContext.WebhooksConfigs.Update(updateObj);
        webhooksDbContext.SaveChanges();
    }

    public async Task UpdateWebhookJournal(int id, ProcessStatus status, string responsePayload, string responseHeaders)
    {
        using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var webhook = await webhooksDbContext.WebhooksLogs.Where(t => t.Id == id).FirstOrDefaultAsync();
        webhook.Status = status;
        webhook.ResponsePayload = responsePayload;
        webhook.ResponseHeaders = responseHeaders;

        webhooksDbContext.WebhooksLogs.Update(webhook);
        await webhooksDbContext.SaveChangesAsync();
    }

    public int WriteToJournal(WebhooksLog webhook)
    {
        using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        var entity = webhooksDbContext.WebhooksLogs.Add(webhook);
        webhooksDbContext.SaveChanges();
        return entity.Entity.Id;
    }
}