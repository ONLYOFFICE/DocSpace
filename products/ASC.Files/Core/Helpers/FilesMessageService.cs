namespace ASC.Web.Files.Helpers;

[Scope]
public class FilesMessageService
{
    private readonly ILog _logger;
    private readonly MessageTarget _messageTarget;
    private readonly MessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FilesMessageService(
        IOptionsMonitor<ILog> options,
        MessageTarget messageTarget,
        MessageService messageService)
    {
        _logger = options.Get("ASC.Messaging");
        _messageTarget = messageTarget;
        _messageService = messageService;
    }

    public FilesMessageService(
        IOptionsMonitor<ILog> options,
        MessageTarget messageTarget,
        MessageService messageService,
        IHttpContextAccessor httpContextAccessor)
        : this(options, messageTarget, messageService)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Send(IDictionary<string, StringValues> headers, MessageAction action)
    {
        SendHeadersMessage(headers, action, null);
    }

    public void Send<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        // do not log actions in users folder
        if (entry == null || entry.RootFolderType == FolderType.USER)
        {
            return;
        }

        SendHeadersMessage(headers, action, _messageTarget.Create(entry.ID), description);
    }

    public void Send<T1, T2>(FileEntry<T1> entry1, FileEntry<T2> entry2, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        // do not log actions in users folder
        if (entry1 == null || entry2 == null || entry1.RootFolderType == FolderType.USER || entry2.RootFolderType == FolderType.USER)
        {
            return;
        }

        SendHeadersMessage(headers, action, _messageTarget.Create(new[] { entry1.ID.ToString(), entry2.ID.ToString() }), description);
    }

    private void SendHeadersMessage(IDictionary<string, StringValues> headers, MessageAction action, MessageTarget target, params string[] description)
    {
        if (headers == null)
        {
            _logger.Debug(string.Format("Empty Request Headers for \"{0}\" type of event", action));

            return;
        }

        _messageService.Send(headers, action, target, description);
    }

    public void Send<T>(FileEntry<T> entry, MessageAction action, params string[] description)
    {
        // do not log actions in users folder
        if (entry == null || entry.RootFolderType == FolderType.USER)
        {
            return;
        }

        if (_httpContextAccessor == null)
        {
            _logger.Debug(string.Format("Empty Http Request for \"{0}\" type of event", action));

            return;
        }

        _messageService.Send(action, _messageTarget.Create(entry.ID), description);
    }

    public void Send<T>(FileEntry<T> entry, MessageInitiator initiator, MessageAction action, params string[] description)
    {
        if (entry == null || entry.RootFolderType == FolderType.USER)
        {
            return;
        }

        _messageService.Send(initiator, action, _messageTarget.Create(entry.ID), description);
    }
}
