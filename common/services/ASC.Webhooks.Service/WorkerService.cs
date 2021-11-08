using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Webhooks;

namespace ASC.Webhooks.Service
{
    [Singletone]
    public class WorkerService
    {
        private readonly int? threadCount = 10;
        private readonly WebhookSender webhookSender;
        private readonly ConcurrentQueue<WebhookRequest> queue;
        private CancellationToken cancellationToken;
        private ILog logger;
        private Timer timer;

        public WorkerService(WebhookSender webhookSender,
            ILog logger,
            BuildQueueService buildQueueService,
            Settings settings)
        {
            this.logger = logger;
            this.webhookSender = webhookSender;
            queue = buildQueueService.Queue;
            threadCount = settings.ThreadCount;
        }

        public void Start(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            timer = new Timer(Procedure, null, 0, Timeout.Infinite);
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }

        private void Procedure(object _)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Stop();
                return;
            }

            timer.Change(Timeout.Infinite, Timeout.Infinite);
            logger.Debug("Procedure: Start.");
            var queueSize = queue.Count;

            if (queueSize == 0)// change to "<= threadCount"
            {
                logger.TraceFormat("Procedure: Waiting for data. Sleep {0}.", 5);
                timer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));
                return;
            }

            var tasks = new List<Task>();
            var counter = 0;
            for (int i = 0; i < queueSize; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Stop();
                    return;
                }

                if (!queue.TryDequeue(out var entry))
                    break;

                tasks.Add(webhookSender.Send(entry, cancellationToken));
                counter++;

                if (counter >= threadCount)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                    counter = 0;
                }
            }

            Task.WaitAll(tasks.ToArray());
            logger.Debug("Procedure: Finish.");
            timer.Change(0, Timeout.Infinite);
        }
    }
}
