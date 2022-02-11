namespace ASC.Feed.Aggregator.Service;

public class FeedCleanerService : FeedBaseService
{
    protected override string LoggerName { get; set; } = "ASC.Feed.Cleaner";

    public FeedCleanerService(
        FeedSettings feedSettings,
        IServiceProvider serviceProvider,
        IOptionsMonitor<ILog> optionsMonitor)
        : base(feedSettings, serviceProvider, optionsMonitor)
    {
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.Info("Feed Cleaner Service running.");

        var cfg = FeedSettings;
        IsStopped = false;

        Timer = new Timer(RemoveFeeds, cfg.AggregateInterval, cfg.RemovePeriod, cfg.RemovePeriod);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.Info("Feed Cleaner Service stopping.");

        IsStopped = true;

        Timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private void RemoveFeeds(object interval)
    {
        if (!Monitor.TryEnter(LockObj))
        {
            return;
        }

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var feedAggregateDataProvider = scope.ServiceProvider.GetService<FeedAggregateDataProvider>();

            Logger.DebugFormat("Start of removing old news");

            feedAggregateDataProvider.RemoveFeedAggregate(DateTime.UtcNow.Subtract((TimeSpan)interval));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
        finally
        {
            Monitor.Exit(LockObj);
        }
    }
}
