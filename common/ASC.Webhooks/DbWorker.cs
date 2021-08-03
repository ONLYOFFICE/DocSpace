using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Webhooks.Dao;
using ASC.Webhooks.Dao.Models;

namespace ASC.Webhooks
{
    [Scope]
    public class DbWorker
    {
        private Lazy<WebhooksDbContext> LazyWebhooksDbContext { get; }
        private WebhooksDbContext webhooksDbContext { get => LazyWebhooksDbContext.Value; }
        public DbWorker(DbContextManager<WebhooksDbContext> webhooksDbContext)
        {
            LazyWebhooksDbContext = new Lazy<WebhooksDbContext>(() => webhooksDbContext.Value);
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
            webhooksDbContext.WebhooksConfigs.Add(webhooksConfig);
            webhooksDbContext.SaveChanges();
        }

        public List<string> GetWebhookUri(int tenant)
        {
            return webhooksDbContext.WebhooksConfigs.Where(t => t.TenantId == tenant).Select(it => it.Uri).ToList();
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
    }
}
