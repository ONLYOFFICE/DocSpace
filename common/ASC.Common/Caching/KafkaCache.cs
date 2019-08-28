using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Common.Utils;
using Confluent.Kafka;

using Google.Protobuf;

namespace ASC.Common.Caching
{
    public class KafkaCache<T> : IDisposable, ICacheNotify<T> where T : IMessage<T>, new()
    {
        private ClientConfig ClientConfig { get; set; }
        private ILog Log { get; set; }
        private ConcurrentDictionary<string, CancellationTokenSource> Cts { get; set; }
        private MemoryCacheNotify<T> MemoryCacheNotify { get; set; }
        private string ChannelName { get; } = $"ascchannel{typeof(T).Name}";
        private ProtobufSerializer<T> Serializer { get; } = new ProtobufSerializer<T>();
        private ProtobufDeserializer<T> Deserializer { get; } = new ProtobufDeserializer<T>();

        private IProducer<Null, T> Producer { get; }
        public KafkaCache()
        {
            Log = LogManager.GetLogger("ASC");
            Cts = new ConcurrentDictionary<string, CancellationTokenSource>();

            var settings = ConfigurationManager.GetSetting<KafkaSettings>("kafka");
            if (settings != null && !string.IsNullOrEmpty(settings.BootstrapServers))
            {
                ClientConfig = new ClientConfig { BootstrapServers = settings.BootstrapServers };
                var config = new ProducerConfig(ClientConfig);
                Producer = new ProducerBuilder<Null, T>(config)
                .SetErrorHandler((_, e) => Log.Error(e))
                .SetValueSerializer(Serializer)
                .Build();
            }
            else
            {
                MemoryCacheNotify = new MemoryCacheNotify<T>();
            }

        }

        public void Publish(T obj, CacheNotifyAction cacheNotifyAction)
        {
            if (ClientConfig == null)
            {
                MemoryCacheNotify.Publish(obj, cacheNotifyAction);
                return;
            }

            try
            {
                Producer.ProduceAsync(GetChannelName(cacheNotifyAction), new Message<Null, T>() { Value = obj });
            }
            catch (ProduceException<Null, string> e)
            {
                Log.Error(e);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Subscribe(Action<T> onchange, CacheNotifyAction cacheNotifyAction)
        {
            if (ClientConfig == null)
            {
                MemoryCacheNotify.Subscribe(onchange, cacheNotifyAction);
                return;
            }

            Cts[GetChannelName(cacheNotifyAction)] = new CancellationTokenSource();

            void action()
            {
                var conf = new ConsumerConfig(ClientConfig)
                {
                    GroupId = Guid.NewGuid().ToString()
                };

                using var c = new ConsumerBuilder<Ignore, T>(conf)
                    .SetErrorHandler((_, e) => Log.Error(e))
                    .SetValueDeserializer(Deserializer)
                    .Build();

                c.Assign(new TopicPartition(GetChannelName(cacheNotifyAction), new Partition()));

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(Cts[GetChannelName(cacheNotifyAction)].Token);
                            if (cr != null && cr.Value != null)
                            {
                                onchange(cr.Value);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Log.Error(e);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    c.Close();
                }
            }

            var task = new Task(action, TaskCreationOptions.LongRunning);
            task.Start();
        }

        private string GetChannelName(CacheNotifyAction cacheNotifyAction)
        {
            return $"{ChannelName}{cacheNotifyAction}";
        }

        public void Unsubscribe(CacheNotifyAction action)
        {
            Cts.TryGetValue(GetChannelName(action), out var source);
            if (source != null)
            {
                source.Cancel();
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Producer.Dispose();
                }

                disposedValue = true;
            }
        }
        ~KafkaCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
    }

    public class MemoryCacheNotify<T> : ICacheNotify<T> where T : IMessage<T>, new()
    {
        private readonly Dictionary<string, List<Action<T>>> actions = new Dictionary<string, List<Action<T>>>();

        public void Publish(T obj, CacheNotifyAction action)
        {
            if (actions.TryGetValue(GetKey(action), out var onchange) && onchange != null)
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
                actions.TryAdd(key, new List<Action<T>>());
                actions[key].Add(onchange);
            }
        }

        public void Unsubscribe(CacheNotifyAction action)
        {
            actions.Remove(GetKey(action));
        }

        private string GetKey(CacheNotifyAction cacheNotifyAction)
        {
            return $"{typeof(T).Name}{cacheNotifyAction}";
        }
    }
}