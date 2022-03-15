namespace ASC.Web.Core.Subscriptions
{
    public enum GroupByType
    {
        Modules,
        Groups,
        Simple
    }
    public interface IProductSubscriptionManager : ISubscriptionManager
    {
        GroupByType GroupByType { get; }
        List<SubscriptionGroup> GetSubscriptionGroups();
    }

}
