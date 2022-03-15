namespace ASC.Notify.Model;

public interface ISubscriptionProvider
{
    bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID);
    IRecipient[] GetRecipients(INotifyAction action, string objectID);
    object GetSubscriptionRecord(INotifyAction action, IRecipient recipient, string objectID);
    string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient);
    string[] GetSubscriptions(INotifyAction action, IRecipient recipient, bool checkSubscribe = true);
    void Subscribe(INotifyAction action, string objectID, IRecipient recipient);
    void UnSubscribe(INotifyAction action);
    void UnSubscribe(INotifyAction action, IRecipient recipient);
    void UnSubscribe(INotifyAction action, string objectID);
    void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient);
    void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames);
}

public static class SubscriptionProviderHelper
{
    public static bool IsSubscribed(this ISubscriptionProvider provider, ILog log, INotifyAction action, IRecipient recipient, string objectID)
    {
        var result = false;

        try
        {
            var subscriptionRecord = provider.GetSubscriptionRecord(action, recipient, objectID);
            if (subscriptionRecord != null)
            {
                var properties = subscriptionRecord.GetType().GetProperties();
                if (properties.Length > 0)
                {
                    var property = properties.Single(p => p.Name == "Subscribed");
                    if (property != null)
                    {
                        result = (bool)property.GetValue(subscriptionRecord, null);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            log.Error(exception);
        }

        return result;
    }
}
