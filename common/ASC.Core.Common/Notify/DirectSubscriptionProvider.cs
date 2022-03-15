namespace ASC.Core.Notify;

class DirectSubscriptionProvider : ISubscriptionProvider
{
    private readonly IRecipientProvider _recipientProvider;
    private readonly SubscriptionManager _subscriptionManager;
    private readonly string _sourceId;


    public DirectSubscriptionProvider(string sourceID, SubscriptionManager subscriptionManager, IRecipientProvider recipientProvider)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(sourceID);
        _sourceId = sourceID;
        _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        _recipientProvider = recipientProvider ?? throw new ArgumentNullException(nameof(recipientProvider));
    }


    public object GetSubscriptionRecord(INotifyAction action, IRecipient recipient, string objectID)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        return _subscriptionManager.GetSubscriptionRecord(_sourceId, action.ID, recipient.ID, objectID);
    }

    public string[] GetSubscriptions(INotifyAction action, IRecipient recipient, bool checkSubscribe = true)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        return _subscriptionManager.GetSubscriptions(_sourceId, action.ID, recipient.ID, checkSubscribe);
    }

    public IRecipient[] GetRecipients(INotifyAction action, string objectID)
    {
        ArgumentNullException.ThrowIfNull(action);

        return _subscriptionManager.GetRecipients(_sourceId, action.ID, objectID)
            .Select(r => _recipientProvider.GetRecipient(r))
            .Where(r => r != null)
            .ToArray();
    }

    public string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        return _subscriptionManager.GetSubscriptionMethod(_sourceId, action.ID, recipient.ID);
    }

    public void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        _subscriptionManager.UpdateSubscriptionMethod(_sourceId, action.ID, recipient.ID, senderNames);
    }

    public bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(action);

        return _subscriptionManager.IsUnsubscribe(_sourceId, recipient.ID, action.ID, objectID);
    }

    public void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        _subscriptionManager.Subscribe(_sourceId, action.ID, objectID, recipient.ID);
    }

    public void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        _subscriptionManager.Unsubscribe(_sourceId, action.ID, objectID, recipient.ID);
    }

    public void UnSubscribe(INotifyAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _subscriptionManager.UnsubscribeAll(_sourceId, action.ID);
    }

    public void UnSubscribe(INotifyAction action, string objectID)
    {
        ArgumentNullException.ThrowIfNull(action);

        _subscriptionManager.UnsubscribeAll(_sourceId, action.ID, objectID);
    }

    [Obsolete("Use UnSubscribe(INotifyAction, string, IRecipient)", true)]
    public void UnSubscribe(INotifyAction action, IRecipient recipient)
    {
        throw new NotSupportedException("use UnSubscribe(INotifyAction, string, IRecipient )");
    }
}
