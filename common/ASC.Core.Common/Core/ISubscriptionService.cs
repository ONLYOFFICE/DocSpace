namespace ASC.Core;

[Scope(typeof(DbSubscriptionService))]
public interface ISubscriptionService
{
    bool IsUnsubscribe(int tenant, string sourceId, string actionId, string recipientId, string objectId);
    IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId);
    IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId);
    IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId);
    string[] GetRecipients(int tenant, string sourceID, string actionID, string objectID);
    string[] GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, bool checkSubscribe);
    SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId);
    void RemoveSubscriptions(int tenant, string sourceId, string actionId);
    void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId);
    void SaveSubscription(SubscriptionRecord s);
    void SetSubscriptionMethod(SubscriptionMethod m);
}
