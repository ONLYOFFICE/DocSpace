namespace ASC.Feed;

public class FeedApiFilter
{
    public string Product { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int Offset { get; set; }
    public int Max { get; set; }
    public Guid Author { get; set; }
    public string[] SearchKeys { get; set; }
    public bool OnlyNew { get; set; }
}
