namespace ASC.Notify;

public interface INotifyService
{
    void InvokeSendMethod(NotifyInvoke notifyInvoke);
    void SendNotifyMessage(NotifyMessage notifyMessage);
}
