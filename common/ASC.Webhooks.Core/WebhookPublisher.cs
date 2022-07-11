using System;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Web.Webhooks;
using ASC.Webhooks.Core.Dao.Models;

using Microsoft.Extensions.Options;

namespace ASC.Webhooks.Core
{
    [Scope]
    public class WebhookPublisher : IWebhookPublisher
    {
        private DbWorker DbWorker { get; }
        private TenantManager TenantManager { get; }
        private ICacheNotify<WebhookRequest> WebhookNotify { get; }

        public WebhookPublisher(
            DbWorker dbWorker,
            TenantManager tenantManager,
            IOptionsMonitor<ILog> options,
            ICacheNotify<WebhookRequest> webhookNotify)
        {
            DbWorker = dbWorker;
            TenantManager = tenantManager;
            WebhookNotify = webhookNotify;
        }

        public void Publish(string eventName, string requestPayload)
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            var webhookConfigs = DbWorker.GetWebhookConfigs(tenantId);

            foreach (var config in webhookConfigs)
            {
                var webhooksLog = new WebhooksLog
                {
                    Uid = Guid.NewGuid().ToString(),
                    TenantId = tenantId,
                    Event = eventName,
                    CreationTime = DateTime.UtcNow,
                    RequestPayload = requestPayload,
                    Status = ProcessStatus.InProcess,
                    ConfigId = config.ConfigId
                };
                var DbId = DbWorker.WriteToJournal(webhooksLog);

                var request = new WebhookRequest()
                {
                    Id = DbId
                };

                WebhookNotify.Publish(request, CacheNotifyAction.Update);
            }
        }
    }

    public enum ProcessStatus
    {
        InProcess,
        Success,
        Failed
    }
}
