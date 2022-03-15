namespace ASC.MessagingSystem.Core.Sender;

[Singletone]
public class DbMessageSender : IMessageSender
{
    private readonly ILog _logger;
    private readonly MessagesRepository _messagesRepository;
    private bool _messagingEnabled;

    public DbMessageSender(IConfiguration configuration, MessagesRepository messagesRepository, IOptionsMonitor<ILog> options)
    {
        var setting = configuration["messaging:enabled"];
        _messagingEnabled = !string.IsNullOrEmpty(setting) && setting == "true";
        _messagesRepository = messagesRepository;
        _logger = options.Get("ASC.Messaging");
    }

    public void Send(EventMessage message)
    {
        try
        {
            if (!_messagingEnabled)
            {
                return;
            }

            if (message == null)
            {
                return;
            }

            _messagesRepository.Add(message);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to send a message", ex);
        }
    }
}
