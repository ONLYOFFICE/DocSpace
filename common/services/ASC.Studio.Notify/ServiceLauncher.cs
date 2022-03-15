namespace ASC.Notify;

[Singletone]
public class ServiceLauncher : BackgroundService
{
    private readonly StudioNotifyServiceSender _studioNotifyServiceSender;
    private readonly NotifyConfiguration _notifyConfiguration;

    public ServiceLauncher(
        StudioNotifyServiceSender studioNotifyServiceSender,
        NotifyConfiguration notifyConfiguration)
    {
        _studioNotifyServiceSender = studioNotifyServiceSender;
        _notifyConfiguration = notifyConfiguration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _notifyConfiguration.Configure();
        _studioNotifyServiceSender.RegisterSendMethod();

        return Task.CompletedTask;
    }
}
