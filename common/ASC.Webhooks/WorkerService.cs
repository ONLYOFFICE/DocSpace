using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using ASC.Common.Logging;

namespace ASC.Webhooks
{
    internal class WorkerService
    {
        private readonly int threadCount = 10;
        private readonly CancellationToken cancellationToken;
        private readonly WebhookSender webhookSender;
        private ILog logger;
        private Timer timer;

        public WorkerService(CancellationToken cancellationToken,
            WebhookSender webhookSender,
            ILog logger)
        {
            this.cancellationToken = cancellationToken;
            this.logger = logger;
            this.webhookSender = webhookSender;
        }

        public void Start()
        {
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
            var queueSize = WebhookHostedService.Queue.Count;

            if (queueSize == 0)// change to "<= threadCount"
            {
                logger.TraceFormat("Procedure: Waiting for data. Sleep {0}.", 5);
                timer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));
                return;
            }


            //var queueEntrys = new List<WebhookRequest>();
            //for (int i = 0; i < queueSize; i++)
            //{
            //    WebhookHostedService.Queue.TryDequeue(out var entry);
            //    queueEntrys.Add(entry);
            //}

            var tasks = new List<Task>();
            var counter = 0;
            for (int i = 0; i < queueSize; i++)
            {
                WebhookHostedService.Queue.TryDequeue(out var entry);
                tasks.Add(webhookSender.Send(entry));
                counter++;

                if (counter >= threadCount)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                    counter = 0;
                }
            }

            logger.Debug("Procedure: Finish.");
            timer.Change(0, Timeout.Infinite);
        }
    }
}
