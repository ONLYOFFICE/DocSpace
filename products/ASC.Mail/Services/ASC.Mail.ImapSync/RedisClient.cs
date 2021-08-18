using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Mail.Configuration;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;

namespace ASC.Mail.ImapSync
{
    [Singletone]
    public class RedisClient
    {
        private readonly MailSettings _mailSettings;
        private readonly StackExchangeRedisCacheClient redis;

        private ILog _log;

        public const string RedisClientPrefix = "ASC.MailAction:";
        public const string RedisClientQueuesKey = RedisClientPrefix + "Queues";


        public RedisClient(IOptionsMonitor<ILog> options, MailSettings mailSettings)
        {
            _log = options.Get("ASC.Mail.RedisClient");

            _mailSettings = mailSettings;

            redis = new StackExchangeRedisCacheClient(new Serializer(), _mailSettings.ImapSync.RedisConnectionString);
        }

        public string CreateQueueKey(int MailBoxId)
        {
            return RedisClientPrefix + MailBoxId.ToString("000000");
        }

        public T Get<T>(string key) where T : class
        {
            return redis.Get<T>(key);
        }

        public void Insert(string key, object value, TimeSpan sligingExpiration)
        {
            redis.Replace(key, value, sligingExpiration);
        }

        public void Insert(string key, object value, DateTime absolutExpiration)
        {
            redis.Replace(key, value, absolutExpiration == DateTime.MaxValue ? DateTimeOffset.MaxValue : new DateTimeOffset(absolutExpiration));
        }

        public void Remove(string key)
        {
            redis.Remove(key);
        }

        public void Remove(Regex pattern)
        {
            var glob = pattern.ToString().Replace(".*", "*").Replace(".", "?");
            var keys = redis.SearchKeys(glob);
            if (keys.Any())
            {
                redis.RemoveAll(keys);
            }
        }

        public IDictionary<string, T> HashGetAll<T>(string key)
        {
            var dic = redis.Database.HashGetAll(key);
            return dic
                .Select(e =>
                {
                    var val = default(T);
                    try
                    {
                        val = (string)e.Value != null ? JsonConvert.DeserializeObject<T>(e.Value) : default(T);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("RedisClient HashGetAll key: {0}", key), ex);
                    }

                    return new { Key = (string)e.Name, Value = val };
                })
                .Where(e => e.Value != null && !e.Value.Equals(default(T)))
                .ToDictionary(e => e.Key, e => e.Value);
        }

        public T HashGet<T>(string key, string field)
        {
            var value = (string)redis.Database.HashGet(key, field);
            try
            {
                return value != null ? JsonConvert.DeserializeObject<T>(value) : default(T);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("RedisClient HashGet key: {0}, field: {1}", key, field), ex);
                return default(T);
            }
        }

        public void HashSet<T>(string key, string field, T value)
        {
            if (value != null)
            {
                redis.Database.HashSet(key, field, JsonConvert.SerializeObject(value));
            }
            else
            {
                redis.Database.HashDelete(key, field);
            }
        }

        public async Task PushToQueue<T>(string QueueName, T value) where T : class
        {
            if (value != null)
            {
                await redis.ListAddToLeftAsync<T>(QueueName, value);
            }
        }

        public async Task<T> PopFromQueue<T>(string QueueName) where T : class
        {
            return await redis.ListGetFromRightAsync<T>(QueueName);
        }

        public async Task<List<string>> GetQueues()
        {
            return (await redis.SetMembersAsync<string>(RedisClientQueuesKey)).ToList();
        }

        public void SubscribeQueueKey<T>(Action<T> onNewKey)
        {
            redis.Subscribe(RedisClientQueuesKey, onNewKey);
        }

        public void PublishQueueKey<T>(List<T> queueKey)
        {
            queueKey.ForEach(x => redis.Publish<T>(RedisClientQueuesKey, x));
        }

        class Serializer : ISerializer
        {
            private readonly Encoding enc = Encoding.UTF8;


            public byte[] Serialize(object item)
            {
                try
                {
                    var s = JsonConvert.SerializeObject(item);
                    return enc.GetBytes(s);
                }
                catch (Exception e)
                {
                    //LogManager.GetLogger("ASC").Error("RedisClient Serialize", e);
                    throw;
                }
            }

            public object Deserialize(byte[] obj)
            {
                try
                {
                    var resolver = new ContractResolver();
                    var settings = new JsonSerializerSettings { ContractResolver = resolver };
                    var s = enc.GetString(obj);
                    return JsonConvert.DeserializeObject(s, typeof(object), settings);
                }
                catch (Exception e)
                {
                    //LogManager.GetLogger("ASC").Error("RedisClient Deserialize", e);
                    throw;
                }
            }

            public T Deserialize<T>(byte[] obj)
            {
                try
                {
                    var resolver = new ContractResolver();
                    var settings = new JsonSerializerSettings { ContractResolver = resolver };
                    var s = enc.GetString(obj);
                    return JsonConvert.DeserializeObject<T>(s, settings);
                }
                catch (Exception e)
                {
                    //LogManager.GetLogger("ASC").Error("RedisClient Deserialize<T>", e);
                    throw;
                }
            }

            public async Task<byte[]> SerializeAsync(object item)
            {
                return await Task.Factory.StartNew(() => Serialize(item));
            }

            public Task<object> DeserializeAsync(byte[] obj)
            {
                return Task.Factory.StartNew(() => Deserialize(obj));
            }

            public Task<T> DeserializeAsync<T>(byte[] obj)
            {
                return Task.Factory.StartNew(() => Deserialize<T>(obj));
            }



            class ContractResolver : DefaultContractResolver
            {
                protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
                {
                    var prop = base.CreateProperty(member, memberSerialization);
                    if (!prop.Writable)
                    {
                        var property = member as PropertyInfo;
                        if (property != null)
                        {
                            var hasPrivateSetter = property.GetSetMethod(true) != null;
                            prop.Writable = hasPrivateSetter;
                        }
                    }
                    return prop;
                }
            }
        }
    }
}
