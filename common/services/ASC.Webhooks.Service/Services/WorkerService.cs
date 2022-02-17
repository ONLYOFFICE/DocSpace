namespace ASC.Webhooks.Service.Services;

[Singletone]
public class WorkerService : BackgroundService
{
    private readonly ILog _logger;
    private readonly ConcurrentQueue<WebhookRequest> _queue;
    private readonly int? _threadCount = 10;
    private readonly WebhookSender _webhookSender;
    private readonly TimeSpan _waitingPeriod;

    public WorkerService(WebhookSender webhookSender,
        ILog logger,
        BuildQueueService buildQueueService,
        Settings settings)
    {
        _logger = logger;
        _webhookSender = webhookSender;
        _queue = buildQueueService.Queue;
        _threadCount = settings.ThreadCount;
        _waitingPeriod = TimeSpan.FromSeconds(5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var queueSize = _queue.Count;

            if (queueSize == 0) // change to "<= threadCount"
            {
                _logger.TraceFormat("Procedure: Waiting for data. Sleep {0}.", _waitingPeriod);

                await Task.Delay(_waitingPeriod, stoppingToken);

                continue;
            }

            var tasks = new List<Task>();
            var counter = 0;

            for (int i = 0; i < queueSize; i++)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                if (!_queue.TryDequeue(out var entry))
                {
                    break;
                }

                tasks.Add(_webhookSender.Send(entry, stoppingToken));
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
        }
    }
}