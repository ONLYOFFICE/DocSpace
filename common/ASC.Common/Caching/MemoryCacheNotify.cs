using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using Google.Protobuf;

namespace ASC.Common.Caching
{
    [Singletone]
    public class MemoryCacheNotify<T> : ICacheNotify<T> where T : IMessage<T>, new()
    {
        private readonly ConcurrentDictionary<string, List<Action<T>>> _actions;

        public MemoryCacheNotify()
        {
            _actions = new ConcurrentDictionary<string, List<Action<T>>>();
        }

        public void Publish(T obj, CacheNotifyAction action)
        {
            if (_actions.TryGetValue(GetKey(action), out var onchange) && onchange != null)
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

        public void Unsubscribe(CacheNotifyAction action)
        {
            _actions.TryRemove(GetKey(action), out _);
        }

        private string GetKey(CacheNotifyAction cacheNotifyAction)
        {
            return $"asc:channel:{cacheNotifyAction}:{typeof(T).FullName}".ToLower();
        }
    }
}