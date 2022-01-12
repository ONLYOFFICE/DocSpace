using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
                foreach (var a in onchange)
                {
                    a(obj);
                }
            }
        }

        public void Subscribe(Action<T> onchange, CacheNotifyAction notifyAction)
        {
            if (onchange != null)
            {
                var key = GetKey(notifyAction);
                _actions.TryAdd(key, new List<Action<T>>());
                _actions[key].Add(onchange);
            }
        }

        public void Unsubscribe(CacheNotifyAction action)
        {
            _actions.TryRemove(GetKey(action), out _);
        }

        private string GetKey(CacheNotifyAction cacheNotifyAction)
        {
            return $"{typeof(T).Name}{cacheNotifyAction}";
        }
    }
}