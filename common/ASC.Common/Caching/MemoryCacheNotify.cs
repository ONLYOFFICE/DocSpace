namespace ASC.Common.Caching;

[Singletone]
public class MemoryCacheNotify<T> : ICacheNotify<T> where T : IMessage<T>, new()
{
    private readonly ConcurrentDictionary<string, List<Action<T>>> _actions;

    public MemoryCacheNotify()
    {
        _actions = new ConcurrentDictionary<string, List<Action<T>>>();
    }

    public void Publish(T obj, CacheNotifyAction notifyAction)
    {
        if (_actions.TryGetValue(GetKey(notifyAction), out var onchange) && onchange != null)
        {
            Parallel.ForEach(onchange, a => a(obj));
        }
    }

        public Task PublishAsync(T obj, CacheNotifyAction action)
        {
            if (_actions.TryGetValue(GetKey(action), out var onchange) && onchange != null)
            {
                Parallel.ForEach(onchange, a => a(obj));
            }

            return Task.CompletedTask;
        }

    public void Subscribe(Action<T> onchange, CacheNotifyAction notifyAction)
    {
        if (onchange != null)
        {
            _actions.GetOrAdd(GetKey(notifyAction), new List<Action<T>>())
                    .Add(onchange);
        }
    }

    public void Unsubscribe(CacheNotifyAction notifyAction)
    {
        _actions.TryRemove(GetKey(notifyAction), out _);
    }

    private string GetKey(CacheNotifyAction notifyAction)
    {
        return $"asc:channel:{notifyAction}:{typeof(T).FullName}".ToLower();
    }
}