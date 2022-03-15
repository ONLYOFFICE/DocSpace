namespace ASC.Feed.Aggregator.Modules;

public interface IFeedModule
{
    string Name { get; }

    string Product { get; }

    IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime);

    IEnumerable<Tuple<Feed, object>> GetFeeds(FeedFilter filter);

    bool VisibleFor(Feed feed, object data, Guid userId);

    void VisibleFor(List<Tuple<FeedRow, object>> feed, Guid userId);
}
