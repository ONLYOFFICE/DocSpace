using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Webhooks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Webhooks
{
    [Singletone]
    public class WebhookHostedService : IHostedService
    {
        private WorkerService workerService;
        internal static readonly ConcurrentQueue<WebhookRequest> Queue = new ConcurrentQueue<WebhookRequest>();
        private BuildQueueService BuildQueueService { get; }
        private WebhookSender WebhookSender { get; }
        private ILog Logger { get; }

        public WebhookHostedService(BuildQueueService buildQueueService,
            WebhookSender webhookSender,
            IOptionsMonitor<ILog> options)
        {
            BuildQueueService = buildQueueService;
            WebhookSender = webhookSender;
            Logger = options.Get("ASC.Webhooks");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            workerService = new WorkerService(cancellationToken, WebhookSender, Logger);
            workerService.Start();
            BuildQueueService.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            if (workerService != null)
            {
                workerService.Stop();
                workerService = null;
            }

            if (BuildQueueService != null)
            {
                BuildQueueService.Stop();
            }

            return Task.CompletedTask;
        }
    }
}
