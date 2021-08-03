using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Webhooks
{
    [Singletone]
    public class WebhookHostedService : IHostedService
    {
        private WorkerService workerService;
        private BuildQueueService buildQueueService;

        public WebhookHostedService(WorkerService workerService,
            BuildQueueService buildQueueService)
        {
            this.workerService = workerService;
            this.buildQueueService = buildQueueService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            workerService.Start(cancellationToken);
            buildQueueService.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            if (workerService != null)
            {
                workerService.Stop();
                workerService = null;
            }

            if (buildQueueService != null)
            {
                buildQueueService.Stop();
            }

            return Task.CompletedTask;
        }
    }
}
