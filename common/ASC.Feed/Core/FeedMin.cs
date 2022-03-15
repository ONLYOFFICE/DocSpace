namespace ASC.Feed;

public class FeedMinUser
{
    public UserInfo UserInfo { get; set; }
}

public class FeedMin : IMapFrom<FeedResultItem>
{
    public string Id { get; set; }
    public Guid AuthorId { get; set; }
    public FeedMinUser Author { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string Product { get; set; }
    public string Item { get; set; }
    public string Title { get; set; }
    public string ItemUrl { get; set; }
    public string Description { get; set; }
    public string AdditionalInfo { get; set; }
    public string AdditionalInfo2 { get; set; }
    public string Module { get; set; }
    public string ExtraLocation { get; set; }
    public IEnumerable<FeedComment> Comments { get; set; }

    public class FeedComment
    {
        public Guid AuthorId { get; set; }
        public FeedMinUser Author { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public FeedMin ToFeedMin()
        {
            return new FeedMin
            {
                Author = Author,
                Title = Description,
                CreatedDate = Date,
                ModifiedDate = Date
            };
        }
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<FeedResultItem, FeedMin>()
            .ConvertUsing<FeedTypeConverter>();
    }
}
