namespace ASC.Notify.Sinks;

class DispatchSink : Sink
{
    private readonly string _senderName;
    private readonly DispatchEngine _dispatcher;

    public DispatchSink(string senderName, DispatchEngine dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _senderName = senderName;
    }

    public override SendResponse ProcessMessage(INoticeMessage message)
    {
        return _dispatcher.Dispatch(message, _senderName);
    }

    public override void ProcessMessageAsync(INoticeMessage message)
    {
        _dispatcher.Dispatch(message, _senderName);
    }
}
