// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Common.Caching;

[Singletone]
public class KafkaCacheNotify<T> : IDisposable, ICacheNotify<T> where T : IMessage<T>, new()
{
    private IProducer<AscCacheItem, T> _producer;

    private bool _disposedValue; // To detect redundant calls
    private readonly ClientConfig _clientConfig;
    private readonly AdminClientConfig _adminClientConfig;
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancelationToken;
    private readonly ConcurrentDictionary<string, Action<T>> _actions;
    private readonly ProtobufSerializer<T> _valueSerializer = new ProtobufSerializer<T>();
    private readonly ProtobufDeserializer<T> _valueDeserializer = new ProtobufDeserializer<T>();
    private readonly ProtobufSerializer<AscCacheItem> _keySerializer = new ProtobufSerializer<AscCacheItem>();
    private readonly ProtobufDeserializer<AscCacheItem> _keyDeserializer = new ProtobufDeserializer<AscCacheItem>();
    private readonly Guid _key;

    public KafkaCacheNotify(ConfigurationExtension configuration, ILogger<KafkaCacheNotify<T>> logger)
    {
        _logger = logger;
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
                .SetErrorHandler((_, e) => _logger.Error(e.ToString()))
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
            _logger.ErrorKafkaCacheNotifyPublish(e);
        }
        catch (Exception e)
        {
            _logger.ErrorKafkaCacheNotifyPublish(e);
        }
    }

    public async Task PublishAsync(T obj, CacheNotifyAction cacheNotifyAction)
    {
        try
        {
            if (_producer == null)
            {
                _producer = new ProducerBuilder<AscCacheItem, T>(new ProducerConfig(_clientConfig))
                .SetErrorHandler((_, e) => _logger.Error(e.ToString()))
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
            _logger.ErrorKafkaCacheNotifyPublishAsync(e);
        }
        catch (Exception e)
        {
            _logger.ErrorKafkaCacheNotifyPublishAsync(e);
        }
    }

    public void Subscribe(Action<T> onchange, CacheNotifyAction notifyAction)
    {
        var channelName = GetChannelName(notifyAction);

        _cancelationToken[channelName] = new CancellationTokenSource();
        _actions[channelName] = onchange;

        async Task actionAsync()
        {
            var conf = new ConsumerConfig(_clientConfig)
            {
                GroupId = Guid.NewGuid().ToString()
            };


            using (var adminClient = new AdminClientBuilder(_adminClientConfig)
                .SetErrorHandler((_, e) => _logger.Error(e.ToString()))
                .Build())
            {
                try
                {
                    //TODO: must add checking exist
                    await adminClient.CreateTopicsAsync(
                        new TopicSpecification[]
                        {
                                new TopicSpecification
                                {
                                    Name = channelName,
                                    NumPartitions = 1,
                                    ReplicationFactor = 1
                                }
                        });
                }
                catch (AggregateException) { }
            }

            using var c = new ConsumerBuilder<AscCacheItem, T>(conf)
                .SetErrorHandler((_, e) => _logger.Error(e.ToString()))
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
                                _logger.ErrorKafkaOnmessage(e);
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.ErrorSubscribe(e);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                c.Close();
            }
        }

        Task.Run(actionAsync);
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
        return $"ascchannel{notifyAction}{typeof(T).FullName}".ToLowerInvariant();
    }
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
}