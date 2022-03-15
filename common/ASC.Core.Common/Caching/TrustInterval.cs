namespace ASC.Core.Caching;

public class TrustInterval
{
    private TimeSpan _interval;
    public DateTime StartTime { get; private set; }
    public bool Expired => _interval == default || _interval < (DateTime.UtcNow - StartTime).Duration();

    public void Start(TimeSpan interval)
    {
        _interval = interval;
        StartTime = DateTime.UtcNow;
    }

    public void Expire()
    {
        _interval = default;
    }
}
