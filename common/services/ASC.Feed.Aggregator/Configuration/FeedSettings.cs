namespace ASC.Feed.Configuration;

[Singletone]
public class FeedSettings
{
    public string ServerRoot
    {
        get => _serverRoot ?? "http://*/";
        set => _serverRoot = value;
    }
    public TimeSpan AggregatePeriod
    {
        get => _aggregatePeriod == TimeSpan.Zero ? TimeSpan.FromMinutes(5) : _aggregatePeriod;
        set => _aggregatePeriod = value;
    }
    public TimeSpan AggregateInterval
    {
        get => _aggregateInterval == TimeSpan.Zero ? TimeSpan.FromDays(14) : _aggregateInterval;
        set => _aggregateInterval = value;
    }
    public TimeSpan RemovePeriod
    {
        get => _removePeriod == TimeSpan.Zero ? TimeSpan.FromDays(1) : _removePeriod;
        set => _removePeriod = value;
    }

    private string _serverRoot;
    private TimeSpan _aggregatePeriod;
    private TimeSpan _aggregateInterval;
    private TimeSpan _removePeriod;

    public FeedSettings(ConfigurationExtension configuration)
    {
        configuration.GetSetting("feed", this);
    }
}
