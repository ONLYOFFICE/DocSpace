namespace ASC.Common.Caching;

[Singletone]
public interface ICacheNotify<T> where T : IMessage<T>, new()
{
    void Publish(T obj, CacheNotifyAction action);

    Task PublishAsync(T obj, CacheNotifyAction action);

    void Subscribe(Action<T> onchange, CacheNotifyAction action);

    void Unsubscribe(CacheNotifyAction action);
}