namespace ASC.Notify.Engine;

interface INotifyEngine
{
    event Action<NotifyEngine, NotifyRequest, IServiceScope> AfterTransferRequest;
    event Action<NotifyEngine, NotifyRequest, IServiceScope> BeforeTransferRequest;
    void QueueRequest(NotifyRequest request, IServiceScope serviceScope);
}
