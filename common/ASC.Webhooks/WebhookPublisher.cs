using System;
using System.Collections.Generic;
using System.Text.Json;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Web.Webhooks;
using ASC.Webhooks.Dao.Models;

using Microsoft.Extensions.Options;

namespace ASC.Webhooks
{
    [Scope]
    public class WebhookPublisher
    {
        private DbWorker DbWorker { get; }
        private TenantManager TenantManager { get; }
        private ICacheNotify<WebhookRequest> WebhookNotify { get; }
        private ILog Log { get; }
        public WebhookPublisher(
            DbWorker dbWorker, 
            TenantManager tenantManager,
            IOptionsMonitor<ILog> options,
            ICacheNotify<WebhookRequest> webhookNotify)
        {
            DbWorker = dbWorker;
            TenantManager = tenantManager;
            Log = options.Get("ASC.Webhooks");
            WebhookNotify = webhookNotify;
        }

        public void Publish(EventName eventName, object data)
        {
            var content = JsonSerializer.Serialize(data);
            var tenantId = TenantManager.GetCurrentTenant().TenantId;

            var webhooksPayload = new WebhooksPayload
            {
                TenantId = tenantId,
                Event = eventName,
                CreationTime = DateTime.UtcNow,
                Data = content,
                Status = ProcessStatus.InProcess
            };
            var DbId = DbWorker.WriteToJournal(webhooksPayload);

            var webhookConfigs = DbWorker.GetWebhookConfigs(tenantId);
            foreach (var config in webhookConfigs)
            {
                var request = new WebhookRequest()
                {
                    Id = DbId,
                    SecretKey = config.SecretKey,
                    Data = content,
                    URI = config.Uri
                };

                WebhookNotify.Publish(request, CacheNotifyAction.Update);
            }
        }
    }

    public class WebhooksExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<WebhookPublisher>();
            services.TryAdd<WebhooksIdentifier>();
        }
    }

    public enum EventName
    {
        UntrackedAction,
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
