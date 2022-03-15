namespace ASC.Notify.Channels;

public class SenderChannel : ISenderChannel
{
    private readonly ISink _firstSink;
    private readonly ISink _senderSink;

    public string SenderName { get; private set; }


    public SenderChannel(Context context, string senderName, ISink decorateSink, ISink senderSink)
    {
        SenderName = senderName ?? throw new ArgumentNullException(nameof(senderName));
        _firstSink = decorateSink;
        _senderSink = senderSink ?? throw new ApplicationException($"channel with tag {senderName} not created sender sink");


        context = context ?? throw new ArgumentNullException(nameof(context));
        var dispatcherSink = new DispatchSink(SenderName, context.DispatchEngine);
        _firstSink = AddSink(_firstSink, dispatcherSink);
    }

    public void SendAsync(INoticeMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        _firstSink.ProcessMessageAsync(message);
    }

    public SendResponse DirectSend(INoticeMessage message)
    {
        return _senderSink.ProcessMessage(message);
    }

    private ISink AddSink(ISink firstSink, ISink addedSink)
    {
        if (firstSink == null)
        {
            return addedSink;
        }

        if (addedSink == null)
        {
            return firstSink;
        }

        var current = firstSink;
        while (current.NextSink != null)
        {
            current = current.NextSink;
        }
        current.NextSink = addedSink;

        return firstSink;
    }
}
