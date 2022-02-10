namespace ASC.Feed.Mapping;

public class FeedTypeConverter : ITypeConverter<FeedAggregate, FeedResultItem>, ITypeConverter<FeedResultItem, FeedMin>
{
    private readonly TenantUtil _tenantUtil;
    private readonly UserManager _userManager;

    public FeedTypeConverter(TenantUtil tenantUtil, UserManager userManager)
    {
        _tenantUtil = tenantUtil;
        _userManager = userManager;
    }

    public FeedResultItem Convert(FeedAggregate source, FeedResultItem destination, ResolutionContext context)
    {
        var result = new FeedResultItem(
            source.Json,
            source.Module,
            source.Author,
            source.ModifiedBy,
            source.GroupId,
            _tenantUtil.DateTimeFromUtc(source.CreatedDate),
            _tenantUtil.DateTimeFromUtc(source.ModifiedDate),
            _tenantUtil.DateTimeFromUtc(source.AggregateDate),
            _tenantUtil);

        return result;
    }

    public FeedMin Convert(FeedResultItem source, FeedMin destination, ResolutionContext context)
    {
        var feedMin = JsonConvert.DeserializeObject<FeedMin>(source.Json);
        feedMin.Author = new FeedMinUser { UserInfo = _userManager.GetUsers(feedMin.AuthorId) };
        feedMin.CreatedDate = source.CreatedDate;

        if (feedMin.Comments == null)
        {
            return feedMin;
        }

        foreach (var comment in feedMin.Comments)
        {
            comment.Author = new FeedMinUser
            {
                UserInfo = _userManager.GetUsers(comment.AuthorId)
            };
        }

        return feedMin;
    }
}
