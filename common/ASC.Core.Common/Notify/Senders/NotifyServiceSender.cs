namespace ASC.Core.Notify.Senders;

public class NotifyServiceSender : INotifySender
{
    public readonly NotifyServiceClient _notifyServiceClient;

    public NotifyServiceSender(ICacheNotify<NotifyMessage> cacheNotify, ICacheNotify<NotifyInvoke> notifyInvoke)
    {
        _notifyServiceClient = new NotifyServiceClient(cacheNotify, notifyInvoke);
    }

    public void Init(IDictionary<string, string> properties) { }

    public NoticeSendResult Send(NotifyMessage m)
    {
        _notifyServiceClient.SendNotifyMessage(m);

        return NoticeSendResult.OK;
    }
}
