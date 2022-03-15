namespace ASC.Core.Notify.Senders;

public class TelegramSender : INotifySender
{
    private readonly ILog _logger;

    public TelegramSender(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider)
    {
        _logger = options.Get("ASC");
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }

    public void Init(IDictionary<string, string> properties) { }

    public NoticeSendResult Send(NotifyMessage m)
    {
        if (!string.IsNullOrEmpty(m.Content))
        {
            m.Content = m.Content.Replace("\r\n", "\n").Trim('\n', '\r', ' ');
            m.Content = Regex.Replace(m.Content, "\n{3,}", "\n\n");
        }
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var TelegramHelper = scope.ServiceProvider.GetService<TelegramHelper>();
            TelegramHelper.SendMessage(m);
        }
        catch (Exception e)
        {
            _logger.ErrorFormat("Unexpected error, {0}, {1}, {2}",
                   e.Message, e.StackTrace, e.InnerException != null ? e.InnerException.Message : string.Empty);
        }

        return NoticeSendResult.OK;
    }
}
