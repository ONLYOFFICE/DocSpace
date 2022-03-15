namespace ASC.Notify.Patterns;

[DebuggerDisplay("{Tag}: {Value}")]
public class TagValue : ITagValue
{
    public string Tag { get; private set; }
    public object Value { get; private set; }

    public TagValue(string tag, object value)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(tag);

        Tag = tag;
        Value = value;
    }
}

public class AdditionalSenderTag : TagValue
{
    public AdditionalSenderTag(string senderName)
        : base("__AdditionalSender", senderName)
    {
    }
}

public class TagActionValue : ITagValue
{
    private readonly Func<string> _action;

    public string Tag { get; private set; }
    public object Value => _action();

    public TagActionValue(string name, Func<string> action)
    {
        Tag = name;
        _action = action;
    }
}
