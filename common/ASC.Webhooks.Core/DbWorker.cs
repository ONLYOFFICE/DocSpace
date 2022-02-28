using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Webhooks.Core.Dao;
using ASC.Webhooks.Core.Dao.Models;

namespace ASC.Webhooks.Core
{
    [Scope]
    public class DbWorker
    {
        private Lazy<WebhooksDbContext> LazyWebhooksDbContext { get; }
        private WebhooksDbContext webhooksDbContext { get => LazyWebhooksDbContext.Value; }
        private TenantManager TenantManager { get; }

        public DbWorker(DbContextManager<WebhooksDbContext> webhooksDbContext, TenantManager tenantManager)
        {
            LazyWebhooksDbContext = new Lazy<WebhooksDbContext>(() => webhooksDbContext.Value);
            TenantManager = tenantManager;
        }

        public int WriteToJournal(WebhooksLog webhook)
        {
            var entity = webhooksDbContext.WebhooksLogs.Add(webhook);
            webhooksDbContext.SaveChanges();
            return entity.Entity.Id;
        }

        public WebhookEntry ReadFromJournal(int id)
        {
            return webhooksDbContext.WebhooksLogs
                .Where(it => it.Id == id)
                .Join(webhooksDbContext.WebhooksConfigs, t => t.ConfigId, t => t.ConfigId, (payload, config) => new { payload, config })
                .Select(t => new WebhookEntry { Id = t.payload.Id, Payload = t.payload.RequestPayload, SecretKey = t.config.SecretKey, Uri = t.config.Uri })
                .OrderBy(t => t.Id).FirstOrDefault();
        }

        public void AddWebhookConfig(WebhooksConfig webhooksConfig)
        {
            webhooksConfig.TenantId = TenantManager.GetCurrentTenant().TenantId;

            var addObj = webhooksDbContext.WebhooksConfigs.Where(it =>
            it.SecretKey == webhooksConfig.SecretKey &&
            it.TenantId == webhooksConfig.TenantId &&
            it.Uri == webhooksConfig.Uri).FirstOrDefault();

            if (addObj != null)
                return;

            webhooksDbContext.WebhooksConfigs.Add(webhooksConfig);
            webhooksDbContext.SaveChanges();
        }

        public void RemoveWebhookConfig(WebhooksConfig webhooksConfig)
        {
            webhooksConfig.TenantId = TenantManager.GetCurrentTenant().TenantId;

            var removeObj = webhooksDbContext.WebhooksConfigs.Where(it =>
            it.SecretKey == webhooksConfig.SecretKey &&
            it.TenantId == webhooksConfig.TenantId &&
            it.Uri == webhooksConfig.Uri).FirstOrDefault();

            webhooksDbContext.WebhooksConfigs.Remove(removeObj);
            webhooksDbContext.SaveChanges();
        }

        public void UpdateWebhookConfig(WebhooksConfig webhooksConfig)
        {
            webhooksConfig.TenantId = TenantManager.GetCurrentTenant().TenantId;

            var updateObj = webhooksDbContext.WebhooksConfigs.Where(it =>
            it.SecretKey == webhooksConfig.SecretKey &&
            it.TenantId == webhooksConfig.TenantId &&
            it.Uri == webhooksConfig.Uri).FirstOrDefault();

            webhooksDbContext.WebhooksConfigs.Update(updateObj);
            webhooksDbContext.SaveChanges();
        }

        public List<WebhooksConfig> GetWebhookConfigs(int tenant)
        {
            return webhooksDbContext.WebhooksConfigs.Where(t => t.TenantId == tenant).ToList();
        }

        public void UpdateWebhookJournal(int id, ProcessStatus status, string responsePayload, string responseHeaders, string requestHeaders)
        {
            var webhook = webhooksDbContext.WebhooksLogs.Where(t => t.Id == id).FirstOrDefault();
            webhook.Status = status;
            webhook.ResponsePayload = responsePayload;
            webhook.ResponseHeaders = responseHeaders;
            webhook.RequestHeaders = requestHeaders;
            webhooksDbContext.WebhooksLogs.Update(webhook);
            webhooksDbContext.SaveChanges();
        }

        public List<WebhooksLog> GetTenantWebhooks()
        {
            var tenant = TenantManager.GetCurrentTenant().TenantId;
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

        public int ConfigsNumber()
        {
            return webhooksDbContext.WebhooksConfigs.Count();
        }
    }
}
