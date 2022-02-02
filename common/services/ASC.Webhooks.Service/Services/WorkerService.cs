using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Webhooks;

using Microsoft.Extensions.Hosting;

namespace ASC.Webhooks.Service.Services
{
    [Singletone]
    public class WorkerService: IHostedService, IDisposable
    {
        private readonly int? _threadCount = 10;
        private readonly WebhookSender _webhookSender;
        private readonly ConcurrentQueue<WebhookRequest> _queue;
        private CancellationToken _cancellationToken;
        private readonly ILog _logger;
        private Timer _timer;

        public WorkerService(WebhookSender webhookSender,
            ILog logger,
            BuildQueueService buildQueueService,
            Settings settings)
        {
            _logger = logger;
            _webhookSender = webhookSender;
            _queue = buildQueueService.Queue;
            _threadCount = settings.ThreadCount;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            _timer = new Timer(DoWork, null, 0, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.CompletedTask;
        }

        private void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }

        private void DoWork(object _)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                Stop();
                return;
            }

            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.Debug("Procedure: Start.");
            var queueSize = _queue.Count;

            if (queueSize == 0)// change to "<= threadCount"
            {
                _logger.TraceFormat("Procedure: Waiting for data. Sleep {0}.", 5);
                _timer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));
                return;
            }

            var tasks = new List<Task>();
            var counter = 0;
            for (int i = 0; i < queueSize; i++)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    Stop();
                    return;
                }

                if (!_queue.TryDequeue(out var entry))
                    break;

                tasks.Add(_webhookSender.Send(entry, _cancellationToken));
                counter++;

                if (counter >= _threadCount)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                    counter = 0;
                }
            }

            Task.WaitAll(tasks.ToArray());
            _logger.Debug("Procedure: Finish.");
            _timer.Change(0, Timeout.Infinite);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
