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
using System.Threading.Tasks;

using Google.Protobuf;

using StackExchange.Redis.Extensions.Core.Abstractions;

namespace ASC.Common.Caching;

[Singletone]
public class RedisCache<T> : ICacheNotify<T> where T : IMessage<T>, new()
{
    private readonly IRedisDatabase _redis;

    public RedisCache(IRedisCacheClient redisCacheClient)
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

    private string GetChannelName(CacheNotifyAction cacheNotifyAction)
    {
        return $"asc:channel:{cacheNotifyAction}:{typeof(T).FullName}".ToLower();
    }

    class RedisCachePubSubItem<T0>
    {
        public T0 Object { get; set; }

        public CacheNotifyAction Action { get; set; }
    }
}



