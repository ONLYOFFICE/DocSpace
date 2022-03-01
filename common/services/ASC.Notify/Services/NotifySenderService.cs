namespace ASC.Notify.Services;

[Singletone]
public class NotifySenderService : BackgroundService
{
    private readonly DbWorker _db;
    private readonly ILog _logger;
    private readonly NotifyServiceCfg _notifyServiceCfg;

    public NotifySenderService(
        IOptions<NotifyServiceCfg> notifyServiceCfg, 
        DbWorker dbWorker, 
        IOptionsMonitor<ILog> options)
    {
        _logger = options.Get("ASC.NotifySender");
        _notifyServiceCfg = notifyServiceCfg.Value;
        _db = dbWorker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Info("Notify Sender Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ThreadManagerWork(stoppingToken);
        }

        _logger.Info("Notify Sender Service is stopping.");
    }

    private async Task ThreadManagerWork(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>(_notifyServiceCfg.Process.MaxThreads);

        try
        {
            if (tasks.Count < _notifyServiceCfg.Process.MaxThreads)
            {
                var messages = _db.GetMessages(_notifyServiceCfg.Process.BufferSize);
                if (messages.Count > 0)
                {
                    var t = new Task(() => SendMessages(messages, stoppingToken), stoppingToken, TaskCreationOptions.LongRunning);
                    tasks.Add(t);
                    t.Start(TaskScheduler.Default);
                }
                else
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }
            else
            {
                await Task.WhenAny(tasks.ToArray()).ContinueWith(r => tasks.RemoveAll(a => a.IsCompleted));
            }
        }
        catch (ThreadAbortException)
        {
            return;
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }
    }

    private void SendMessages(object messages, CancellationToken stoppingToken)
    {
        try
        {
            foreach (var m in (IDictionary<int, NotifyMessage>)messages)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                var result = MailSendingState.Sended;
                try
                {
                    var sender = _notifyServiceCfg.Senders.FirstOrDefault(r => r.Name == m.Value.SenderType);
                    if (sender != null)
                    {
                        sender.NotifySender.Send(m.Value);
                    }
                    else
                    {
                        result = MailSendingState.FatalError;
                    }

                    _logger.DebugFormat("Notify #{0} has been sent.", m.Key);
                }
                catch (Exception e)
                {
                    result = MailSendingState.FatalError;
                    _logger.Error(e);
                }

                _db.SetState(m.Key, result);
            }
        }
        catch (ThreadAbortException)
        {
            return;
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }
    }
}