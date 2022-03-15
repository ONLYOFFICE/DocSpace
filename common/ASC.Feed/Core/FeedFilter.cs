namespace ASC.Feed;

public struct FeedFilter
{
    public int Tenant { get; set; }
    public TimeInterval Time { get; private set; }

    public FeedFilter(TimeInterval time) : this()
    {
        Time = time;
    }

    public FeedFilter(DateTime from, DateTime to)
        : this()
    {
        Time = new TimeInterval(from, to);
    }
}
