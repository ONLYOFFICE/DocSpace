namespace ASC.Core.Notify.Senders;

public interface INotifySender
{
    void Init(IDictionary<string, string> properties);
    NoticeSendResult Send(NotifyMessage m);
}
