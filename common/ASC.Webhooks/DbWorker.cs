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
        public WebhooksDbContext webhooksContext { get; }
        public DbWorker(DbContextManager<WebhooksDbContext> dbContext)
        {
            webhooksContext = dbContext.Get("webhooks");
        }

        public void WriteToJournal(WebhooksPayload webhook)
        {
            webhooksContext.WebhooksPayloads.Add(webhook);
            webhooksContext.SaveChanges();
        }

        public void AddWebhookConfig(WebhooksConfig webhooksConfig)
        {
            webhooksContext.WebhooksConfigs.Add(webhooksConfig);
            webhooksContext.SaveChanges();
        }

        public List<string> GetWebhookUri(int tenant)
        {
            return webhooksContext.WebhooksConfigs.Where(t => t.TenantId == tenant).Select(it => it.Uri).ToList();
        }

        public List<WebhooksConfig> GetWebhookConfigs(int tenant)
        {
            return webhooksContext.WebhooksConfigs.Where(t => t.TenantId == tenant).ToList();
        }

        public void UpdateStatus(int id, ProcessStatus status)
        {
            var webhook = webhooksContext.WebhooksPayloads.Where(t => t.Id == id).FirstOrDefault();
            webhook.Status = status;
            webhooksContext.WebhooksPayloads.Update(webhook);
            webhooksContext.SaveChanges();
        }

        public List<WebhooksQueueEntry> GetWebhookQueue()
        {
            return webhooksContext.WebhooksPayloads
                .Where(t => t.Status == ProcessStatus.InProcess)
                .Join(webhooksContext.WebhooksConfigs, t => t.TenantId, t => t.TenantId, (payload, config) => new { payload, config })
                .Select(t => new WebhooksQueueEntry { Id = t.payload.Id, Data = t.payload.Data, SecretKey = t.config.SecretKey, Uri = t.config.Uri })
                .OrderBy(t => t.Id)
                .ToList();
        }
    }
}
