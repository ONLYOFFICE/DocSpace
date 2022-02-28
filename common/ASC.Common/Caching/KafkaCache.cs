using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;
using ASC.Common.Utils;

using Confluent.Kafka;
using Confluent.Kafka.Admin;

using Google.Protobuf;

using Microsoft.Extensions.Options;

namespace ASC.Common.Caching
{
    [Singletone]
    public class KafkaCache<T> : IDisposable, ICacheNotify<T> where T : IMessage<T>, new()
    {
        private ClientConfig ClientConfig { get; set; }
        private AdminClientConfig AdminClientConfig { get; set; }
        private ILog Log { get; set; }
        private ConcurrentDictionary<string, CancellationTokenSource> Cts { get; set; }
        private ConcurrentDictionary<string, Action<T>> Actions { get; set; }
        private ProtobufSerializer<T> ValueSerializer { get; } = new ProtobufSerializer<T>();
        private ProtobufDeserializer<T> ValueDeserializer { get; } = new ProtobufDeserializer<T>();
        private ProtobufSerializer<AscCacheItem> KeySerializer { get; } = new ProtobufSerializer<AscCacheItem>();
        private ProtobufDeserializer<AscCacheItem> KeyDeserializer { get; } = new ProtobufDeserializer<AscCacheItem>();
        private IProducer<AscCacheItem, T> Producer { get; set; }
        private Guid Key { get; set; }

        public KafkaCache(ConfigurationExtension configuration, IOptionsMonitor<ILog> options)
        {
            Log = options.CurrentValue;
            Cts = new ConcurrentDictionary<string, CancellationTokenSource>();
            Actions = new ConcurrentDictionary<string, Action<T>>();
            Key = Guid.NewGuid();

            var settings = configuration.GetSetting<KafkaSettings>("kafka");

            ClientConfig = new ClientConfig { BootstrapServers = settings.BootstrapServers };
            AdminClientConfig = new AdminClientConfig { BootstrapServers = settings.BootstrapServers };
        }

        public void Publish(T obj, CacheNotifyAction cacheNotifyAction)
        {
            try
            {
                if (Producer == null)
                {
                    Producer = new ProducerBuilder<AscCacheItem, T>(new ProducerConfig(ClientConfig))
                    .SetErrorHandler((_, e) => Log.Error(e))
                    .SetKeySerializer(KeySerializer)
                    .SetValueSerializer(ValueSerializer)
                    .Build();
                }

                var channelName = GetChannelName(cacheNotifyAction);

                if (Actions.TryGetValue(channelName, out var onchange))
                {
                    onchange(obj);
                }

                var message = new Message<AscCacheItem, T>
                {
                    Value = obj,
                    Key = new AscCacheItem
                    {
                        Id = Key.ToString()
                    }
                };

                Producer.ProduceAsync(channelName, message);
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

        public async Task PublishAsync(T obj, CacheNotifyAction cacheNotifyAction)
        {
            try
            {
                if (Producer == null)
                {
                    Producer = new ProducerBuilder<AscCacheItem, T>(new ProducerConfig(ClientConfig))
                    .SetErrorHandler((_, e) => Log.Error(e))
                    .SetKeySerializer(KeySerializer)
                    .SetValueSerializer(ValueSerializer)
                    .Build();
                }

                var channelName = GetChannelName(cacheNotifyAction);

                if (Actions.TryGetValue(channelName, out var onchange))
                {
                    onchange(obj);
                }

                var message = new Message<AscCacheItem, T>
                {
                    Value = obj,
                    Key = new AscCacheItem
                    {
                        Id = Key.ToString()
                    }
                };

                await Producer.ProduceAsync(channelName, message);
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
            var channelName = GetChannelName(cacheNotifyAction);

            Cts[channelName] = new CancellationTokenSource();
            Actions[channelName] = onchange;

            void action()
            {
                var conf = new ConsumerConfig(ClientConfig)
                {
                    GroupId = Guid.NewGuid().ToString()
                };


                using (var adminClient = new AdminClientBuilder(AdminClientConfig)
                    .SetErrorHandler((_, e) => Log.Error(e))
                    .Build())
                {
                    try
                    {
                        //TODO: must add checking exist
                        adminClient.CreateTopicsAsync(
                            new TopicSpecification[]
                            {
                                new TopicSpecification
                                {
                                    Name = channelName,
                                    NumPartitions = 1,
                                    ReplicationFactor = 1
                                }
                            }).Wait();
                    }
                    catch (AggregateException)
                    {

                    }
                }


                using var c = new ConsumerBuilder<AscCacheItem, T>(conf)
                    .SetErrorHandler((_, e) => Log.Error(e))
                    .SetKeyDeserializer(KeyDeserializer)
                    .SetValueDeserializer(ValueDeserializer)
                    .Build();

                c.Assign(new TopicPartition(channelName, new Partition()));

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(Cts[channelName].Token);
                            if (cr != null && cr.Message != null && cr.Message.Value != null && !(new Guid(cr.Message.Key.Id)).Equals(Key) && Actions.TryGetValue(channelName, out var act))
                            {
                                try
                                {
                                    act(cr.Message.Value);
                                }
                                catch (Exception e)
                                {
                                    Log.Error("Kafka onmessage", e);
                                }
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
            return $"ascchannel{cacheNotifyAction}{typeof(T).FullName}".ToLower();
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
                if (disposing && Producer != null)
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
}