namespace ASC.Notify.Model;

class NotifyClientImpl : INotifyClient
{
    private readonly Context _context;
    private readonly InterceptorStorage _interceptors = new InterceptorStorage();
    private readonly INotifySource _notifySource;
    public readonly IServiceScope _serviceScope;

    public NotifyClientImpl(Context context, INotifySource notifySource, IServiceScope serviceScope)
    {
        this._notifySource = notifySource ?? throw new ArgumentNullException(nameof(notifySource));
        _serviceScope = serviceScope;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void SendNoticeToAsync(INotifyAction action, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
    {
        SendNoticeToAsync(action, null, recipients, senderNames, false, args);
    }

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, recipients, senderNames, false, args);
    }

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, recipients, null, false, args);
    }

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, bool checkSubscription, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, recipients, null, checkSubscription, args);
    }

    public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, new[] { recipient }, null, false, args);
    }

    public void SendNoticeAsync(int tenantId, INotifyAction action, string objectID, params ITagValue[] args)
    {
        var subscriptionSource = _notifySource.GetSubscriptionProvider();
        var recipients = subscriptionSource.GetRecipients(action, objectID);
        SendNoticeToAsync(action, objectID, recipients, null, false, args);
    }

    public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient, bool checkSubscription, params ITagValue[] args)
    {
        SendNoticeToAsync(action, objectID, new[] { recipient }, null, checkSubscription, args);
    }

    public void BeginSingleRecipientEvent(string name)
    {
        _interceptors.Add(new SingleRecipientInterceptor(name));
    }

    public void EndSingleRecipientEvent(string name)
    {
        _interceptors.Remove(name);
    }

    public void AddInterceptor(ISendInterceptor interceptor)
    {
        _interceptors.Add(interceptor);
    }

    public void RemoveInterceptor(string name)
    {
        _interceptors.Remove(name);
    }

    public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames, bool checkSubsciption, params ITagValue[] args)
    {
        ArgumentNullException.ThrowIfNull(recipients);

        BeginSingleRecipientEvent("__syspreventduplicateinterceptor");

        foreach (var recipient in recipients)
        {
            var r = CreateRequest(action, objectID, recipient, args, senderNames, checkSubsciption);
            SendAsync(r);
        }
    }

    private void SendAsync(NotifyRequest request)
    {
        request.Interceptors = _interceptors.GetAll();
        _context.NotifyEngine.QueueRequest(request, _serviceScope);
    }

    private NotifyRequest CreateRequest(INotifyAction action, string objectID, IRecipient recipient, ITagValue[] args, string[] senders, bool checkSubsciption)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(recipient);

        var request = new NotifyRequest(_notifySource, action, objectID, recipient)
        {
            SenderNames = senders,
            IsNeedCheckSubscriptions = checkSubsciption
        };

        if (args != null)
        {
            request.Arguments.AddRange(args);
        }

        return request;
    }
}
