namespace ASC.Notify;

public interface INotifyRegistry
{
    INotifyClient RegisterClient(INotifySource source, IServiceScope serviceScope);
    ISenderChannel GetSender(string senderName);
    void RegisterSender(string senderName, ISink senderSink);
    void UnregisterSender(string senderName);
}
