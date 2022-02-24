using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Logging;


using Google.Protobuf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ASC.Common.Caching
{
    [Singletone]
    public class RabbitMQCache<T> : IDisposable, ICacheNotify<T> where T : IMessage<T>, new()
    {
        private IConnection _connection;
        private readonly IConnectionFactory _connectionFactory;
       
        private IModel _consumerChannel;
        private readonly Guid _instanceId;
        private readonly string _exchangeName;
        private readonly string _queueName;

        private readonly ILog _logger;
        private readonly ConcurrentDictionary<string, List<Action<T>>> _actions;

        private readonly object _lock = new object();
        private bool _disposed;

        public RabbitMQCache(IConfiguration configuration, IOptionsMonitor<ILog> options)
        {
            _logger = options.CurrentValue;
            _instanceId = Guid.NewGuid();
            _exchangeName = $"asc:cache_notify:event_bus:{typeof(T).FullName}";
            _queueName = $"asc:cache_notify:queue:{typeof(T).FullName}:{_instanceId}";
            _actions = new ConcurrentDictionary<string, List<Action<T>>>();

            var rabbitMQConfiguration = configuration.GetSection("rabbitmq").Get<RabbitMQSettings>();

            _connectionFactory = new ConnectionFactory {
                   HostName = rabbitMQConfiguration.HostName,
                   UserName = rabbitMQConfiguration.UserName,
                   Password = rabbitMQConfiguration.Password
            };

            _connection = _connectionFactory.CreateConnection();
            _consumerChannel = CreateConsumerChannel();

            StartBasicConsume();
        }

        private IModel CreateConsumerChannel()
        {
            TryConnect();

            _logger.Trace("Creating RabbitMQ consumer channel");

            var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);
            channel.QueueDeclare(queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: null);

            channel.QueueBind(_queueName, _exchangeName, string.Empty, null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.Warn("Recreating RabbitMQ consumer channel", ea.Exception);

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();

                StartBasicConsume();

            };

            return channel;
        }

        private void StartBasicConsume()
        {
            _logger.Trace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(_consumerChannel);

                consumer.Received += OnMessageReceived;

                _consumerChannel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            }
            else
            {
                _logger.Error("StartBasicConsume can't call on _consumerChannel == null");
            }
        }


        private void TryConnect()
        {
            lock (_lock)
            {
                if (IsConnected)
                    return;

                _connection = _connectionFactory.CreateConnection();
                _connection.ConnectionShutdown += (s, e) => TryConnect();
                _connection.CallbackException += (s, e) => TryConnect();
                _connection.ConnectionBlocked += (s, e) => TryConnect();
            }
        }


        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        private void OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.Span.ToArray();

            var parser = new MessageParser<T>(() => new T());

            var data = body.Take(body.Length - 1);

            var obj = parser.ParseFrom(data.ToArray());

            var action = (CacheNotifyAction)body[body.Length - 1];

            if (_actions.TryGetValue(GetKey(action), out var onchange) && onchange != null)
            {
                Parallel.ForEach(onchange, a => a(obj));
            }
        }

        public void Publish(T obj, CacheNotifyAction action)
        {
            var objAsByteArray = obj.ToByteArray();

            var body = new byte[objAsByteArray.Length + 1];

            objAsByteArray.CopyTo(body, 0);

            body[body.Length - 1] = (byte)action;

            _consumerChannel.BasicPublish(
                                 exchange: _exchangeName,
                                 routingKey: string.Empty,
                                 mandatory: true,
                                 basicProperties: _consumerChannel.CreateBasicProperties(),
                                 body: body);
        }

        public Task PublishAsync(T obj, CacheNotifyAction action)
        {
            Publish(obj, action);

            return Task.CompletedTask;
        }


        public void Subscribe(Action<T> onchange, CacheNotifyAction action)
        {
            _actions.GetOrAdd(GetKey(action), new List<Action<T>>())
                .Add(onchange);
        }

        public void Unsubscribe(CacheNotifyAction action)
        {
            _actions.TryRemove(GetKey(action), out _);
        }

        private string GetKey(CacheNotifyAction cacheNotifyAction)
        {
            return $"asc:channel:{cacheNotifyAction}:{typeof(T).FullName}".ToLower();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _consumerChannel?.Dispose();
            _connection.Dispose();

            _disposed = true;
        }
    }

    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
