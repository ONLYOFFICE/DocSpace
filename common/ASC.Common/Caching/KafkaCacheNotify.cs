namespace ASC.Common.Caching;

[Singletone]
public class KafkaCacheNotify<T> : IDisposable, ICacheNotify<T> where T : IMessage<T>, new()
{
    private IProducer<AscCacheItem, T> _producer;

    private bool _disposedValue = false; // To detect redundant calls
    private readonly ClientConfig _clientConfig;
    private readonly AdminClientConfig _adminClientConfig;
    private readonly ILog _logger;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancelationToken;
    private readonly ConcurrentDictionary<string, Action<T>> _actions;
    private readonly ProtobufSerializer<T> _valueSerializer = new ProtobufSerializer<T>();
    private readonly ProtobufDeserializer<T> _valueDeserializer = new ProtobufDeserializer<T>();
    private readonly ProtobufSerializer<AscCacheItem> _keySerializer = new ProtobufSerializer<AscCacheItem>();
    private readonly ProtobufDeserializer<AscCacheItem> _keyDeserializer = new ProtobufDeserializer<AscCacheItem>();
    private readonly Guid _key;

    public KafkaCacheNotify(ConfigurationExtension configuration, IOptionsMonitor<ILog> options)
    {
        _logger = options.CurrentValue;
        _cancelationToken = new ConcurrentDictionary<string, CancellationTokenSource>();
        _actions = new ConcurrentDictionary<string, Action<T>>();
        _key = Guid.NewGuid();

        var settings = configuration.GetSetting<KafkaSettings>("kafka");

        _clientConfig = new ClientConfig { BootstrapServers = settings.BootstrapServers };
        _adminClientConfig = new AdminClientConfig { BootstrapServers = settings.BootstrapServers };
    }

    public void Publish(T obj, CacheNotifyAction notifyAction)
    {
        try
        {
            if (_producer == null)
            {
                _producer = new ProducerBuilder<AscCacheItem, T>(new ProducerConfig(_clientConfig))
                .SetErrorHandler((_, e) => _logger.Error(e))
                .SetKeySerializer(_keySerializer)
                .SetValueSerializer(_valueSerializer)
                .Build();
            }

            var channelName = GetChannelName(notifyAction);

            if (_actions.TryGetValue(channelName, out var onchange))
            {
                onchange(obj);
            }

            var message = new Message<AscCacheItem, T>
            {
                Value = obj,
                Key = new AscCacheItem
                {
                    Id = _key.ToString()
                }
            };

            _producer.ProduceAsync(channelName, message);
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.Error(e);
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }
    }

    public async Task PublishAsync(T obj, CacheNotifyAction cacheNotifyAction)
    {
        try
        {
            if (_producer == null)
            {
                _producer = new ProducerBuilder<AscCacheItem, T>(new ProducerConfig(_clientConfig))
                .SetErrorHandler((_, e) => _logger.Error(e))
                .SetKeySerializer(_keySerializer)
                .SetValueSerializer(_valueSerializer)
                .Build();
            }

            var channelName = GetChannelName(cacheNotifyAction);

            if (_actions.TryGetValue(channelName, out var onchange))
            {
                onchange(obj);
            }

            var message = new Message<AscCacheItem, T>
            {
                Value = obj,
                Key = new AscCacheItem
                {
                    Id = _key.ToString()
                }
            };

            await _producer.ProduceAsync(channelName, message);
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.Error(e);
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }
    }

    public void Subscribe(Action<T> onchange, CacheNotifyAction notifyAction)
    {
        var channelName = GetChannelName(notifyAction);

        _cancelationToken[channelName] = new CancellationTokenSource();
        _actions[channelName] = onchange;

        void action()
        {
            var conf = new ConsumerConfig(_clientConfig)
            {
                GroupId = Guid.NewGuid().ToString()
            };


            using (var adminClient = new AdminClientBuilder(_adminClientConfig)
                .SetErrorHandler((_, e) => _logger.Error(e))
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
                catch (AggregateException) { }
            }

            using var c = new ConsumerBuilder<AscCacheItem, T>(conf)
                .SetErrorHandler((_, e) => _logger.Error(e))
                .SetKeyDeserializer(_keyDeserializer)
                .SetValueDeserializer(_valueDeserializer)
                .Build();

            c.Assign(new TopicPartition(channelName, new Partition()));

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume(_cancelationToken[channelName].Token);
                        if (cr != null && cr.Message != null && cr.Message.Value != null && !(new Guid(cr.Message.Key.Id)).Equals(_key) && _actions.TryGetValue(channelName, out var act))
                        {
                            try
                            {
                                act(cr.Message.Value);
                            }
                            catch (Exception e)
                            {
                                _logger.Error("Kafka onmessage", e);
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.Error(e);
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

    public void Unsubscribe(CacheNotifyAction notifyAction)
    {
        _cancelationToken.TryGetValue(GetChannelName(notifyAction), out var source);

        if (source != null)
        {
            source.Cancel();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && _producer != null)
            {
                _producer.Dispose();
            }

            _disposedValue = true;
        }
    }

    ~KafkaCacheNotify()
    {
        Dispose(false);
    }

    private string GetChannelName(CacheNotifyAction notifyAction)
    {
        return $"ascchannel{notifyAction}{typeof(T).FullName}".ToLower();
    }
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
}