namespace ASC.Feed.Aggregator;

public class FeedComment
{
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Guid AuthorId { get; private set; }

    public FeedComment(Guid author)
    {
        AuthorId = author;
    }
}
