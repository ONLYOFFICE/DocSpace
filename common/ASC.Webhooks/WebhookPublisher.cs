using System;
using System.Text.Json;

using ASC.Common;
using ASC.Core;
using ASC.Webhooks.Dao.Models;

namespace ASC.Webhooks
{
    [Scope]
    public class WebhookPublisher
    {
        private DbWorker DbWorker { get; }
        private TenantManager TenantManager { get; }
        private WebhookSender WebhookSender { get; }

        public WebhookPublisher(DbWorker dbWorker, TenantManager tenantManager, WebhookSender webhookSender)
        {
            DbWorker = dbWorker;
            TenantManager = tenantManager;
            WebhookSender = webhookSender;
        }

        public void Publish(EventName name)
        {
            var result = WebhookSender.Send(name);

            if (result)
            {
                var tenantId = TenantManager.GetCurrentTenant().TenantId;

                var webhooksPayload = new WebhooksPayload
                {
                    TenantId = tenantId,
                    Event = name,
                    CreationTime = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(name),
                };

                DbWorker.WriteToJournal(webhooksPayload);
            }
        }
    }
    public struct EventName
    {
        public const string NewUserRegistered = "NewUserRegistered";
        public const string TenantDeleted = "TenantDeleted";
    }
}
