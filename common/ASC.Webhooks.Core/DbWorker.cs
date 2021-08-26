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

        public int WriteToJournal(WebhooksPayload webhook)
        {
            var entity = webhooksDbContext.WebhooksPayloads.Add(webhook);
            webhooksDbContext.SaveChanges();
            return entity.Entity.Id;
        }

        public WebhookEntry ReadFromJournal(int id)
        {
            return webhooksDbContext.WebhooksPayloads
                .Where(it => it.Id == id)
                .Join(webhooksDbContext.WebhooksConfigs, t => t.ConfigId, t => t.ConfigId, (payload, config) => new { payload, config })
                .Select(t => new WebhookEntry { Id = t.payload.Id, Data = t.payload.Data, SecretKey = t.config.SecretKey, Uri = t.config.Uri })
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

        public void UpdateStatus(int id, ProcessStatus status)
        {
            var webhook = webhooksDbContext.WebhooksPayloads.Where(t => t.Id == id).FirstOrDefault();
            webhook.Status = status;
            webhooksDbContext.WebhooksPayloads.Update(webhook);
            webhooksDbContext.SaveChanges();
        }

        public int ConfigsNumber()
        {
            return webhooksDbContext.WebhooksConfigs.Count();
        }
    }
}
