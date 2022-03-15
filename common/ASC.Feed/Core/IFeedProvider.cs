namespace ASC.Feed;

public interface IFeedProvider<T>
{
    IEnumerable<T> GetFeed(FeedFilter filter);
}
