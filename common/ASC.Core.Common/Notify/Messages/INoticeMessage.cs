namespace ASC.Notify.Messages;

public interface INoticeMessage
{
    IDirectRecipient Recipient { get; }
    IPattern Pattern { get; }
    INotifyAction Action { get; }
    ITagValue[] Arguments { get; }
    string Subject { get; set; }
    string Body { get; set; }
    string ContentType { get; }
    void AddArgument(params ITagValue[] tagValue);
}
