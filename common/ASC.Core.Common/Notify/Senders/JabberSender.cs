namespace ASC.Core.Notify.Senders;

[Singletone(Additional = typeof(JabberSenderExtension))]
public class JabberSender : INotifySender
{
    private readonly ILog _logger;
    private readonly IServiceProvider _serviceProvider;

    public JabberSender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
    }

    public void Init(IDictionary<string, string> properties) { }

    public NoticeSendResult Send(NotifyMessage m)
    {
        var text = m.Content;
        if (!string.IsNullOrEmpty(text))
        {
            text = text.Replace("\r\n", "\n").Trim('\n', '\r');
            text = Regex.Replace(text, "\n{3,}", "\n\n");
        }
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetService<JabberServiceClient>();
            service.SendMessage(m.TenantId, null, m.Reciever, text, m.Subject);
        }
        catch (Exception e)
        {
            _logger.ErrorFormat("Unexpected error, {0}, {1}, {2}",
                   e.Message, e.StackTrace, e.InnerException != null ? e.InnerException.Message : string.Empty);
        }

        return NoticeSendResult.OK;
    }
}

public static class JabberSenderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<JabberServiceClient>();
    }
}
