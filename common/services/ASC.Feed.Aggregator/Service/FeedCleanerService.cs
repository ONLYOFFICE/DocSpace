namespace ASC.Feed.Aggregator.Service;

[Singletone]
public class FeedCleanerService : FeedBaseService
{
    protected override string LoggerName { get; set; } = "ASC.Feed.Cleaner";

    public FeedCleanerService(
        FeedSettings feedSettings,
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<ILog> optionsMonitor)
        : base(feedSettings, serviceScopeFactory, optionsMonitor)
    {
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.Info("Feed Cleaner Service running.");

        var cfg = FeedSettings;

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(cfg.RemovePeriod, stoppingToken);

            RemoveFeeds(cfg.AggregateInterval);
        }

        Logger.Info("Feed Cleaner Service stopping.");
    }

    private void RemoveFeeds(object interval)
    {
        try
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var feedAggregateDataProvider = scope.ServiceProvider.GetService<FeedAggregateDataProvider>();

            Logger.DebugFormat("Start of removing old news");

            feedAggregateDataProvider.RemoveFeedAggregate(DateTime.UtcNow.Subtract((TimeSpan)interval));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
}
