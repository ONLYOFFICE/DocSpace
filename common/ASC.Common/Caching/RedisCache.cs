/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Google.Protobuf;

using StackExchange.Redis.Extensions.Core.Abstractions;

namespace ASC.Common.Caching;

[Singletone]
public class RedisCache<T> : ICacheNotify<T> where T : IMessage<T>, new()
{
    private readonly string CacheId = Guid.NewGuid().ToString();
    private readonly IRedisDatabase _redis;
    private readonly ConcurrentDictionary<Type, ConcurrentBag<Action<object, CacheNotifyAction>>> actions = new ConcurrentDictionary<Type, ConcurrentBag<Action<object, CacheNotifyAction>>>();

    public RedisCache(IRedisCacheClient redisCacheClient)
    {
        _redis = redisCacheClient.GetDbFromConfiguration();
    }

    public void Publish(T obj, CacheNotifyAction action)
    {
        Task.Run(() => _redis.PublishAsync("asc:channel:" + typeof(T).FullName, new RedisCachePubSubItem<T>() { CacheId = CacheId, Object = obj, Action = action }))
            .GetAwaiter()
            .GetResult();

        ConcurrentBag<Action<object, CacheNotifyAction>> onchange;
        actions.TryGetValue(typeof(T), out onchange);

        if (onchange != null)
        {
            onchange.ToList().ForEach(r => r(obj, action));
        }
    }

    public void Subscribe(Action<T> onchange, CacheNotifyAction action)
    {
        Task.Run(() => _redis.SubscribeAsync<RedisCachePubSubItem<T>>("asc:channel:" + typeof(T).FullName, (i) =>
        {
            if (i.CacheId != CacheId)
            {
                onchange(i.Object);
            }

            return Task.FromResult(true);
        })).GetAwaiter()
          .GetResult();


        if (onchange != null)
        {
            Action<object, CacheNotifyAction> _action = (o, a) => onchange((T)o);

            actions.AddOrUpdate(typeof(T),
                new ConcurrentBag<Action<object, CacheNotifyAction>> { _action },
                (type, bag) =>
                {
                    bag.Add(_action);
                    return bag;
                });
        }
        else
        {
            ConcurrentBag<Action<object, CacheNotifyAction>> removed;
            actions.TryRemove(typeof(T), out removed);
        }
    }

    public void Unsubscribe(CacheNotifyAction action)
    {
        Task.Run(() => _redis.UnsubscribeAsync<RedisCachePubSubItem<T>>("asc:channel:" + typeof(T).FullName, (i) =>
        {
            return Task.FromResult(true);
        })).GetAwaiter()
          .GetResult();
    }

    class RedisCachePubSubItem<T0>
    {
        public string CacheId { get; set; }

        public T0 Object { get; set; }

        public CacheNotifyAction Action { get; set; }
    }
}



