namespace ASC.Common.Caching;

[Singletone]
public class EventBusMemoryCache<T> : IEventBus<T> where T : IMessage<T>, new()
{
    private readonly ConcurrentDictionary<string, List<Action<T>>> _actions;

    public EventBusMemoryCache() => _actions = new ConcurrentDictionary<string, List<Action<T>>>();

    public void Publish(T obj, EventType action)
    {
        if (_actions.TryGetValue(GetKey(action), out var onchange) && onchange != null)
            Parallel.ForEach(onchange, a => a(obj));
    }

    public void Subscribe(Action<T> onchange, EventType notifyAction)
    {
        if (onchange != null)
            _actions.GetOrAdd(GetKey(notifyAction), new List<Action<T>>())
                    .Add(onchange);
    }

    public void Unsubscribe(EventType action) => _actions.TryRemove(GetKey(action), out _);

    private string GetKey(EventType cacheNotifyAction) => $"asc:channel:{cacheNotifyAction}:{typeof(T).FullName}".ToLower();
}