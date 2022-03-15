using CacheNotifyAction = ASC.Common.Caching.CacheNotifyAction;

namespace ASC.Core.Notify;

[Scope]
public class NotifyServiceClient : INotifyService
{
    private readonly ICacheNotify<NotifyMessage> _cacheNotify;
    private readonly ICacheNotify<NotifyInvoke> _notifyInvoke;
    public NotifyServiceClient(ICacheNotify<NotifyMessage> cacheNotify, ICacheNotify<NotifyInvoke> notifyInvoke)
    {
        _cacheNotify = cacheNotify;
        _notifyInvoke = notifyInvoke;
    }

    public void SendNotifyMessage(NotifyMessage m)
    {
        _cacheNotify.Publish(m, CacheNotifyAction.InsertOrUpdate);
    }

    public void InvokeSendMethod(NotifyInvoke notifyInvoke)
    {
        _notifyInvoke.Publish(notifyInvoke, CacheNotifyAction.InsertOrUpdate);
    }
}
