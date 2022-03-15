namespace ASC.Feed;

public struct TimeInterval
{
    public DateTime From { get; }
    public DateTime To => _toTime != default ? _toTime : DateTime.MaxValue;

    private readonly DateTime _toTime;

    public TimeInterval(DateTime fromTime, DateTime toTime)
    {
        From = fromTime;
        _toTime = toTime;
    }
}
