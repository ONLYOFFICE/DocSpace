using System;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Webhooks.Dao.Models;

using Microsoft.Extensions.Hosting;

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

        public void Publish(EventName eventName, object data)
        {
            var content = JsonSerializer.Serialize(data);

            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            var webhookConfigs = DbWorker.GetWebhookConfigs(tenantId);
            foreach (var config in webhookConfigs)
            {
                var webhooksPayload = new WebhooksPayload
                {
                    TenantId = tenantId,
                    Event = eventName,
                    CreationTime = DateTime.UtcNow,
                    Data = content,
                    Status = ProcessStatus.InProcess
                };

                DbWorker.WriteToJournal(webhooksPayload);
            }
        }
    }
    public enum EventName
    {
        NewFileCreated,
        FileUpdated
    }

    public enum ProcessStatus
    {
        InProcess,
        Success,
        Failed
    }
}
