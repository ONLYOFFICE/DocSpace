namespace ASC.Web.Core.Subscriptions
{

    public interface ISubscriptionManager
    {
        List<SubscriptionObject> GetSubscriptionObjects(Guid subItem);

        List<SubscriptionType> GetSubscriptionTypes();

        ISubscriptionProvider SubscriptionProvider { get; }
    }
}
