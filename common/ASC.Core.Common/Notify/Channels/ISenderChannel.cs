namespace ASC.Notify.Channels;

public interface ISenderChannel
{
    string SenderName { get; }
    SendResponse DirectSend(INoticeMessage message);
    void SendAsync(INoticeMessage message);
}
