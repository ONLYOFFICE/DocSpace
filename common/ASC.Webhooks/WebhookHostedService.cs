using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Web.Webhooks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASC.Webhooks
{
    public class WebhookHostedService : BackgroundService
    {
        private ILog Log { get; }
        private DbWorker DbWorker { get; }
        private TenantManager TenantManager { get; }
        private WebhookSender WebhookSender { get; }

        private ICacheNotify<WebhookRequest> CacheNotify { get; }

        public WebhookHostedService(IOptionsMonitor<ILog> option, 
            DbWorker dbWorker, 
            TenantManager tenantManager, 
            WebhookSender webhookSender,
            ICacheNotify<WebhookRequest> cacheNotify)
        {
            Log = option.Get("ASC.Webhooks");
            DbWorker = dbWorker;
            TenantManager = tenantManager;
            WebhookSender = webhookSender;
            CacheNotify = cacheNotify;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Debug(
               $"WebhookHostedService is starting.");

            stoppingToken.Register(() =>
            Log.Debug($"WebhookHostedService is stopping."));

            CacheNotify.Subscribe(BackgroundProcessing, CacheNotifyAction.Update);
        }

        public void Stop()
        {
            CacheNotify.Unsubscribe(CacheNotifyAction.Update);
        }

        private void BackgroundProcessing(WebhookRequest request)// горизонтальная, вертикальная кластеризация, добавление в очередь из паблишера
        {
            WebhookSender.Send(request);
        }
    }
}
