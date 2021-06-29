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
    }
}
