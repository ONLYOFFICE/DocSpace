namespace ASC.Feed.Aggregator;

public class Feed
{
    public string Item { get; set; }
    public string ItemId { get; set; }
    public Guid AuthorId { get; private set; }
    public Guid ModifiedBy { get; set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime ModifiedDate { get; set; }
    public string Product { get; set; }
    public string Module { get; set; }
    public string ExtraLocation { get; set; }
    public string ExtraLocationUrl { get; set; }
    public FeedAction Action { get; set; }
    public string Title { get; set; }
    public string ItemUrl { get; set; }
    public string Description { get; set; }
    public string AdditionalInfo { get; set; }
    public string AdditionalInfo2 { get; set; }
    public string AdditionalInfo3 { get; set; }
    public string AdditionalInfo4 { get; set; }
    public bool HasPreview { get; set; }
    public bool CanComment { get; set; }
    public object Target { get; set; }
    public string CommentApiUrl { get; set; }
    public IEnumerable<FeedComment> Comments { get; set; }
    public string GroupId { get; set; }
    public string Keywords { get; set; }
    public string Id => $"{Item}_{ItemId}";

    // это значит, что новость может обновляться (пр. добавление комментариев);
    // следовательно и права доступа могут устаревать
    public bool Variate { get; private set; }

    public Feed() { }

    public Feed(Guid author, DateTime date, bool variate = false)
    {
        AuthorId = author;
        ModifiedBy = author;

        CreatedDate = date;
        ModifiedDate = date;

        Action = FeedAction.Created;
        Variate = variate;
    }
}
