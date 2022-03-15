namespace ASC.Notify;

public delegate void SendNoticeCallback(INotifyAction action, string objectID, IRecipient recipient, NotifyResult result);

public interface INotifyClient
{
    void AddInterceptor(ISendInterceptor interceptor);
    void BeginSingleRecipientEvent(string name);
    void EndSingleRecipientEvent(string name);
    void RemoveInterceptor(string name);
    void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, bool checkSubscription, params ITagValue[] args);
    void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, params ITagValue[] args);
    void SendNoticeAsync(int tenantId, INotifyAction action, string objectID, params ITagValue[] args);
    void SendNoticeToAsync(INotifyAction action, IRecipient[] recipients, string[] senderNames, params ITagValue[] args);
    void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, bool checkSubscription, params ITagValue[] args);
    void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, params ITagValue[] args);
    void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, bool checkSubsciption, params ITagValue[] args);
    void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, params ITagValue[] args);
}
