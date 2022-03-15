namespace ASC.Notify;

public sealed class Context : INotifyRegistry
{
    public const string SysRecipient = "_#" + SysRecipientId + "#_";
    internal const string SysRecipientId = "SYS_RECIPIENT_ID";
    internal const string SysRecipientName = "SYS_RECIPIENT_NAME";
    internal const string SysRecipientAddress = "SYS_RECIPIENT_ADDRESS";

    private readonly Dictionary<string, ISenderChannel> _channels = new Dictionary<string, ISenderChannel>(2);

    public NotifyEngine NotifyEngine { get; private set; }
    public INotifyRegistry NotifyService => this;
    public DispatchEngine DispatchEngine { get; private set; }

    public event Action<Context, INotifyClient> NotifyClientRegistration;

    private ILog Logger { get; set; }

    public Context(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetService<IOptionsMonitor<ILog>>();
        Logger = options.CurrentValue;
        NotifyEngine = new NotifyEngine(this, serviceProvider);
        DispatchEngine = new DispatchEngine(this, serviceProvider.GetService<IConfiguration>(), options);
    }

    void INotifyRegistry.RegisterSender(string senderName, ISink senderSink)
    {
        lock (_channels)
        {
            _channels[senderName] = new SenderChannel(this, senderName, null, senderSink);
        }
    }

    void INotifyRegistry.UnregisterSender(string senderName)
    {
        lock (_channels)
        {
            _channels.Remove(senderName);
        }
    }

    ISenderChannel INotifyRegistry.GetSender(string senderName)
    {
        lock (_channels)
        {
            _channels.TryGetValue(senderName, out var channel);

            return channel;
        }
    }

    INotifyClient INotifyRegistry.RegisterClient(INotifySource source, IServiceScope serviceScope)
    {
        //ValidateNotifySource(source);
        var client = new NotifyClientImpl(this, source, serviceScope);
        NotifyClientRegistration?.Invoke(this, client);

        return client;
    }

    private void ValidateNotifySource(INotifySource source)
    {
        foreach (var a in source.GetActionProvider().GetActions())
        {
            IEnumerable<string> senderNames;
            lock (_channels)
            {
                senderNames = _channels.Values.Select(s => s.SenderName);
            }
            foreach (var s in senderNames)
            {
                try
                {
                    var pattern = source.GetPatternProvider().GetPattern(a, s);
                    if (pattern == null)
                    {
                        throw new NotifyException($"In notify source {source.Id} pattern not found for action {a.ID} and sender {s}");
                    }
                }
                catch (Exception error)
                {
                    Logger.ErrorFormat("Source: {0}, action: {1}, sender: {2}, error: {3}", source.Id, a.ID, s, error);
                }
            }
        }
    }
}
