namespace ASC.Feed.Aggregator.Modules;

public abstract class FeedModule : IFeedModule
{
    public abstract string Name { get; }
    public abstract string Product { get; }
    public abstract Guid ProductID { get; }
    protected abstract string DbId { get; }
    protected int Tenant => TenantManager.GetCurrentTenant().Id;

    protected readonly TenantManager TenantManager;
    protected readonly WebItemSecurity WebItemSecurity;

    protected FeedModule(TenantManager tenantManager, WebItemSecurity webItemSecurity)
    {
        TenantManager = tenantManager;
        WebItemSecurity = webItemSecurity;
    }

    public abstract IEnumerable<Tuple<Feed, object>> GetFeeds(FeedFilter filter);

    public abstract IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime);

    public virtual void VisibleFor(List<Tuple<FeedRow, object>> feed, Guid userId)
    {
        if (!WebItemSecurity.IsAvailableForUser(ProductID, userId))
        {
            return;
        }

        foreach (var tuple in feed)
        {
            if (VisibleFor(tuple.Item1.Feed, tuple.Item2, userId))
            {
                tuple.Item1.Users.Add(userId);
            }
        }
    }

    public virtual bool VisibleFor(Feed feed, object data, Guid userId)
    {
        return WebItemSecurity.IsAvailableForUser(ProductID, userId);
    }

    protected static Guid ToGuid(object guid)
    {
        try
        {
            var str = guid as string;
            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
        }
        catch (Exception)
        {
            return Guid.Empty;
        }
    }

    protected string GetGroupId(string item, Guid author, string rootId = null, int action = -1)
    {
        const int interval = 2;

        var now = DateTime.UtcNow;
        var hours = now.Hour;
        var groupIdHours = hours - (hours % interval);

        if (rootId == null)
        {
            // groupId = {item}_{author}_{date}
            return string.Format("{0}_{1}_{2}",
                                 item,
                                 author,
                                 now.ToString("yyyy.MM.dd.") + groupIdHours);
        }
        if (action == -1)
        {
            // groupId = {item}_{author}_{date}_{rootId}_{action}
            return string.Format("{0}_{1}_{2}_{3}",
                                 item,
                                 author,
                                 now.ToString("yyyy.MM.dd.") + groupIdHours,
                                 rootId);
        }

        // groupId = {item}_{author}_{date}_{rootId}_{action}
        return string.Format("{0}_{1}_{2}_{3}_{4}",
                             item,
                             author,
                             now.ToString("yyyy.MM.dd.") + groupIdHours,
                             rootId,
                             action);
    }
}
