namespace ASC.Notify.Messages;

[Serializable]
public class NoticeMessage : INoticeMessage
{
    [NonSerialized]
    private readonly List<ITagValue> _arguments = new List<ITagValue>();

    [NonSerialized]
    private IPattern _pattern;

    public NoticeMessage() { }

    public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID)
    {
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Action = action;
        ObjectID = objectID;
    }

    public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID, IPattern pattern)
    {
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Action = action;
        Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        ObjectID = objectID;
        ContentType = pattern.ContentType;
    }

    public NoticeMessage(IDirectRecipient recipient, string subject, string body, string contentType)
    {
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        Subject = subject;
        Body = body ?? throw new ArgumentNullException(nameof(body));
        ContentType = contentType;
    }

    public string ObjectID { get; private set; }

    public IDirectRecipient Recipient { get; private set; }

    public IPattern Pattern
    {
        get => _pattern;
        internal set => _pattern = value;
    }

    public INotifyAction Action { get; private set; }

    public ITagValue[] Arguments => _arguments.ToArray();

    public void AddArgument(params ITagValue[] tagValues)
    {
        ArgumentNullException.ThrowIfNull(tagValues);

        Array.ForEach(tagValues,
            tagValue =>
            {
                if (!_arguments.Exists(tv => Equals(tv.Tag, tagValue.Tag)))
                {
                    _arguments.Add(tagValue);
                }
            });
    }

    public ITagValue GetArgument(string tag)
    {
        return _arguments.Find(r => r.Tag == tag);
    }

    public string Subject { get; set; }
    public string Body { get; set; }
    public string ContentType { get; internal set; }
}
