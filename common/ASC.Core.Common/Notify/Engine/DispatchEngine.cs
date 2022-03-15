namespace ASC.Notify.Engine;

public class DispatchEngine
{
    private readonly ILog _logger;
    private readonly ILog _messagesLogger;
    private readonly Context _context;
    private readonly bool _logOnly;

    public DispatchEngine(Context context, IConfiguration configuration, IOptionsMonitor<ILog> options)
    {
        _logger = options.Get("ASC.Notify");
        _messagesLogger = options.Get("ASC.Notify.Messages");
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logOnly = "log".Equals(configuration["core:notify:postman"], StringComparison.InvariantCultureIgnoreCase);
        _logger.DebugFormat("LogOnly: {0}", _logOnly);
    }

    public SendResponse Dispatch(INoticeMessage message, string senderName)
    {
        var response = new SendResponse(message, senderName, SendResult.OK);
        if (!_logOnly)
        {
            var sender = _context.NotifyService.GetSender(senderName);
            if (sender != null)
            {
                response = sender.DirectSend(message);
            }
            else
            {
                response = new SendResponse(message, senderName, SendResult.Impossible);
            }

            LogResponce(message, response, sender != null ? sender.SenderName : string.Empty);
        }

        LogMessage(message, senderName);

        return response;
    }

    private void LogResponce(INoticeMessage message, SendResponse response, string senderName)
    {
        var logmsg = string.Format("[{0}] sended to [{1}] over {2}, status: {3} ", message.Subject, message.Recipient, senderName, response.Result);
        if (response.Result == SendResult.Inprogress)
        {
            _logger.Debug(logmsg, response.Exception);
        }
        else if (response.Result == SendResult.Impossible)
        {
            _logger.Error(logmsg, response.Exception);
        }
        else
        {
            _logger.Debug(logmsg);
        }
    }

    private void LogMessage(INoticeMessage message, string senderName)
    {
        try
        {
            if (_messagesLogger.IsDebugEnabled)
            {
                _messagesLogger.DebugFormat("[{5}]->[{1}] by [{6}] to [{2}] at {0}\r\n\r\n[{3}]\r\n{4}\r\n{7}",
                    DateTime.Now,
                    message.Recipient.Name,
                    0 < message.Recipient.Addresses.Length ? message.Recipient.Addresses[0] : string.Empty,
                    message.Subject,
                    (message.Body ?? string.Empty).Replace(Environment.NewLine, Environment.NewLine + @"   "),
                    message.Action,
                    senderName,
                    new string('-', 80));
            }
        }
        catch { }
    }
}
