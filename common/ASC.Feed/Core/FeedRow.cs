namespace ASC.Feed.Core;

public class FeedRow
{
    public DateTime AggregatedDate { get; set; }
    public IList<Guid> Users { get; set; }
    public int Tenant { get; set; }
    public string Product { get; set; }
    public Aggregator.Feed Feed { get; private set; }
    public string Id => Feed.Id;
    public bool ClearRightsBeforeInsert => Feed.Variate;
    public string Module => Feed.Module;
    public Guid Author => Feed.AuthorId;
    public Guid ModifiedBy => Feed.ModifiedBy;
    public DateTime CreatedDate => Feed.CreatedDate;
    public DateTime ModifiedDate => Feed.ModifiedDate;
    public string GroupId => Feed.GroupId;
    public string Keywords => Feed.Keywords;
    public string Json
    {
        get
        {
            return JsonConvert.SerializeObject(Feed, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
        }
    }

    public FeedRow(Aggregator.Feed feed)
    {
        Users = new List<Guid>();
        Feed = feed;
    }
}
