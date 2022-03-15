namespace ASC.MessagingSystem.Core;

[Scope]
public class MessageService
{
    private readonly ILog _logger;
    private readonly IMessageSender _sender;
    private readonly HttpRequest _request;
    private readonly MessageFactory _messageFactory;
    private readonly MessagePolicy _messagePolicy;

    public MessageService(
        IConfiguration configuration,
        MessageFactory messageFactory,
        DbMessageSender sender,
        MessagePolicy messagePolicy,
        IOptionsMonitor<ILog> options)
    {
        if (configuration["messaging:enabled"] != "true")
        {
            return;
        }

        _sender = sender;
        _messagePolicy = messagePolicy;
        _messageFactory = messageFactory;
        _logger = options.CurrentValue;
    }

    public MessageService(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        MessageFactory messageFactory,
        DbMessageSender sender,
        MessagePolicy messagePolicy,
        IOptionsMonitor<ILog> options)
        : this(configuration, messageFactory, sender, messagePolicy, options)
    {
        _request = httpContextAccessor?.HttpContext?.Request;
    }

    #region HttpRequest

    public void Send(MessageAction action)
    {
        SendRequestMessage(null, action, null);
    }

    public void Send(MessageAction action, string d1)
    {
        SendRequestMessage(null, action, null, d1);
    }

    public void Send(MessageAction action, string d1, string d2)
    {
        SendRequestMessage(null, action, null, d1, d2);
    }

    public void Send(MessageAction action, string d1, string d2, string d3)
    {
        SendRequestMessage(null, action, null, d1, d2, d3);
    }

    public void Send(MessageAction action, string d1, string d2, string d3, string d4)
    {
        SendRequestMessage(null, action, null, d1, d2, d3, d4);
    }

    public void Send(MessageAction action, IEnumerable<string> d1, string d2)
    {
        SendRequestMessage(null, action, null, string.Join(", ", d1), d2);
    }

    public void Send(MessageAction action, string d1, IEnumerable<string> d2)
    {
        SendRequestMessage(null, action, null, d1, string.Join(", ", d2));
    }

    public void Send(MessageAction action, string d1, string d2, IEnumerable<string> d3)
    {
        SendRequestMessage(null, action, null, d1, d2, string.Join(", ", d3));
    }

    public void Send(MessageAction action, IEnumerable<string> d1)
    {
        SendRequestMessage(null, action, null, string.Join(", ", d1));
    }

    public void Send(string loginName, MessageAction action)
    {
        SendRequestMessage(loginName, action, null);
    }

    public void Send(string loginName, MessageAction action, string d1)
    {
        SendRequestMessage(loginName, action, null, d1);
    }

    #endregion

    #region HttpRequest & Target

    public void Send(MessageAction action, MessageTarget target)
    {
        SendRequestMessage(null, action, target);
    }

    public void Send(MessageAction action, MessageTarget target, string d1)
    {
        SendRequestMessage(null, action, target, d1);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2)
    {
        SendRequestMessage(null, action, target, d1, d2);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2, string d3)
    {
        SendRequestMessage(null, action, target, d1, d2, d3);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2, string d3, string d4)
    {
        SendRequestMessage(null, action, target, d1, d2, d3, d4);
    }

    public void Send(MessageAction action, MessageTarget target, IEnumerable<string> d1, string d2)
    {
        SendRequestMessage(null, action, target, string.Join(", ", d1), d2);
    }

    public void Send(MessageAction action, MessageTarget target, string d1, IEnumerable<string> d2)
    {
        SendRequestMessage(null, action, target, d1, string.Join(", ", d2));
    }

    public void Send(MessageAction action, MessageTarget target, string d1, string d2, IEnumerable<string> d3)
    {
        SendRequestMessage(null, action, target, d1, d2, string.Join(", ", d3));
    }

    public void Send(MessageAction action, MessageTarget target, IEnumerable<string> d1)
    {
        SendRequestMessage(null, action, target, string.Join(", ", d1));
    }

    public void Send(string loginName, MessageAction action, MessageTarget target)
    {
        SendRequestMessage(loginName, action, target);
    }

    public void Send(string loginName, MessageAction action, MessageTarget target, string d1)
    {
        SendRequestMessage(loginName, action, target, d1);
    }

    #endregion

    private void SendRequestMessage(string loginName, MessageAction action, MessageTarget target, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        if (_request == null)
        {
            _logger.Debug(string.Format("Empty Http Request for \"{0}\" type of event", action));

            return;
        }

        var message = _messageFactory.Create(_request, loginName, action, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        _sender.Send(message);
    }

    #region HttpHeaders

    public void Send(MessageUserData userData, IDictionary<string, StringValues> httpHeaders, MessageAction action)
    {
        SendHeadersMessage(userData, httpHeaders, action, null);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action)
    {
        SendHeadersMessage(null, httpHeaders, action, null);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, string d1)
    {
        SendHeadersMessage(null, httpHeaders, action, null, d1);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, IEnumerable<string> d1)
    {
        SendHeadersMessage(null, httpHeaders, action, null, d1?.ToArray());
    }

    public void Send(MessageUserData userData, IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target)
    {
        SendHeadersMessage(userData, httpHeaders, action, target);
    }

    #endregion

    #region HttpHeaders & Target

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target)
    {
        SendHeadersMessage(null, httpHeaders, action, target);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target, string d1)
    {
        SendHeadersMessage(null, httpHeaders, action, target, d1);
    }

    public void Send(IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target, IEnumerable<string> d1)
    {
        SendHeadersMessage(null, httpHeaders, action, target, d1?.ToArray());
    }

    #endregion

    private void SendHeadersMessage(MessageUserData userData, IDictionary<string, StringValues> httpHeaders, MessageAction action, MessageTarget target, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        var message = _messageFactory.Create(userData, httpHeaders, action, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        _sender.Send(message);
    }

    #region Initiator

    public void Send(MessageInitiator initiator, MessageAction action, params string[] description)
    {
        SendInitiatorMessage(initiator.ToString(), action, null, description);
    }

    #endregion

    #region Initiator & Target

    public void Send(MessageInitiator initiator, MessageAction action, MessageTarget target, params string[] description)
    {
        SendInitiatorMessage(initiator.ToString(), action, target, description);
    }

    #endregion

    private void SendInitiatorMessage(string initiator, MessageAction action, MessageTarget target, params string[] description)
    {
        if (_sender == null)
        {
            return;
        }

        var message = _messageFactory.Create(_request, initiator, action, target, description);
        if (!_messagePolicy.Check(message))
        {
            return;
        }

        _sender.Send(message);
    }
}
