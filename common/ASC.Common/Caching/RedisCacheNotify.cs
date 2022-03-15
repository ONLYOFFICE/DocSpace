namespace ASC.Common.Caching;

[Singletone]
public class RedisCacheNotify<T> : ICacheNotify<T> where T : IMessage<T>, new()
{
    private readonly IRedisDatabase _redis;

    public RedisCacheNotify(IRedisCacheClient redisCacheClient)
    {
        _redis = redisCacheClient.GetDbFromConfiguration();
    }

    public void Publish(T obj, CacheNotifyAction action)
    {
        Task.Run(() => _redis.PublishAsync(GetChannelName(action), new RedisCachePubSubItem<T>() { Object = obj, Action = action }))
            .GetAwaiter()
            .GetResult();
    }

    public async Task PublishAsync(T obj, CacheNotifyAction action)
    {
        await Task.Run(() => _redis.PublishAsync(GetChannelName(action), new RedisCachePubSubItem<T>() { Object = obj, Action = action }));
    }

    public void Subscribe(Action<T> onchange, CacheNotifyAction action)
    {
        Task.Run(() => _redis.SubscribeAsync<RedisCachePubSubItem<T>>(GetChannelName(action), (i) =>
        {
            onchange(i.Object);

            return Task.FromResult(true);
        })).GetAwaiter()
          .GetResult();
    }

    public void Unsubscribe(CacheNotifyAction action)
    {
        Task.Run(() => _redis.UnsubscribeAsync<RedisCachePubSubItem<T>>(GetChannelName(action), (i) =>
        {
            return Task.FromResult(true);
        })).GetAwaiter()
          .GetResult();
    }

    private string GetChannelName(CacheNotifyAction action)
    {
        return $"asc:channel:{action}:{typeof(T).FullName}".ToLower();
    }

    class RedisCachePubSubItem<T0>
    {
        public T0 Object { get; set; }

        public CacheNotifyAction Action { get; set; }
    }
}