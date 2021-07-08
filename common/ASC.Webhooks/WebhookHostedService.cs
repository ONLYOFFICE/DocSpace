using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Core;

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

        public WebhookHostedService(IOptionsMonitor<ILog> option, 
            DbWorker dbWorker, 
            TenantManager tenantManager, 
            WebhookSender webhookSender)
        {
            Log = option.Get("ASC.Webhooks");
            DbWorker = dbWorker;
            TenantManager = tenantManager;
            WebhookSender = webhookSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Debug(
               $"WebhookHostedService is starting.{Environment.NewLine}");

            stoppingToken.Register(() =>
            Log.Debug($"WebhookHostedService is stopping."));

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var webhooks = DbWorker.GetWebhookQueue();

                    foreach (var wh in webhooks)
                    {
                        WebhookSender.Send(wh);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    Log.Error("ERROR: " + ex.Message);
                }
            }
        }
    }
}
