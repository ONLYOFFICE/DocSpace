namespace ASC.Feed.Aggregator.Service;

public abstract class FeedBaseService : BackgroundService
{
    protected virtual string LoggerName { get; set; } = "ASC.Feed";

    protected readonly ILog Logger;
    protected readonly FeedSettings FeedSettings;
    protected readonly IServiceScopeFactory ServiceScopeFactory;

    public FeedBaseService(
        FeedSettings feedSettings,
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<ILog> optionsMonitor)
    {
        FeedSettings = feedSettings;
        ServiceScopeFactory = serviceScopeFactory;
        Logger = optionsMonitor.Get(LoggerName);
    }
}
