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

using System.Security.Authentication;

namespace ASC.Common.Caching;

[Singletone]
public class RabbitMQCache<T> : IDisposable, ICacheNotify<T> where T : IMessage<T>, new()
{
    private IConnection _connection;
    private readonly ConnectionFactory _factory;

    private IModel _consumerChannel;
    private readonly Guid _instanceId;
    private readonly string _exchangeName;
    private readonly string _queueName;

    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, List<Action<T>>> _actions;

    private readonly object _lock = new object();
    private bool _disposed;

    public RabbitMQCache(IConfiguration configuration, ILogger<RabbitMQCache<T>> logger)
    {
        _logger = logger;
        _instanceId = Guid.NewGuid();
        _exchangeName = $"asc:cache_notify:event_bus:{typeof(T).FullName}";
        _queueName = $"asc:cache_notify:queue:{typeof(T).FullName}:{_instanceId}";
        _actions = new ConcurrentDictionary<string, List<Action<T>>>();

        var rabbitMQConfiguration = configuration.GetSection("rabbitmq").Get<RabbitMQSettings>();

        _factory = new ConnectionFactory();

        if (!string.IsNullOrEmpty(rabbitMQConfiguration.Uri))
        {
            _factory.Uri = new Uri(rabbitMQConfiguration.Uri);
        }
        else
        {
            _factory.HostName = rabbitMQConfiguration.HostName;
            _factory.UserName = rabbitMQConfiguration.UserName;
            _factory.Password = rabbitMQConfiguration.Password;
            _factory.Port = rabbitMQConfiguration.Port;
            _factory.VirtualHost = rabbitMQConfiguration.VirtualHost;
        }

        if (rabbitMQConfiguration.EnableSsl)
        {
            _factory.Ssl = new SslOption
            {
                Enabled = rabbitMQConfiguration.EnableSsl,
                Version = SslProtocols.Tls12
            };

            if (!string.IsNullOrEmpty(rabbitMQConfiguration.SslCertPath))
            {
                _factory.Ssl.CertPath = rabbitMQConfiguration.SslCertPath;
                _factory.Ssl.ServerName = rabbitMQConfiguration.SslServerName;
            }
        }

        _connection = _factory.CreateConnection();
        _consumerChannel = CreateConsumerChannel();

        StartBasicConsume();
    }

    private IModel CreateConsumerChannel()
    {
        TryConnect();

        _logger.TraceCreatingRabbitMQ();

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
            _logger.WarningRecreatingRabbitMQ(ea.Exception);

            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();

            StartBasicConsume();

        };

        return channel;
    }

    private void StartBasicConsume()
    {
        _logger.TraceStartingRabbitMQ();

        if (_consumerChannel != null)
        {
            var consumer = new EventingBasicConsumer(_consumerChannel);

            consumer.Received += OnMessageReceived;

            _consumerChannel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
        else
        {
            _logger.ErrorStartBasicConsumeCanNotCall();
        }
    }


    private void TryConnect()
    {
        lock (_lock)
        {
            if (IsConnected)
            {
                return;
            }

            _connection = _factory.CreateConnection();
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
        {
            return;
        }

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
    public int Port { get; set; }
    public string VirtualHost { get; set; }
    public string Uri { get; set; }
    public bool EnableSsl { get; set; }
    public string SslServerName { get; set; }
    public string SslCertPath { get; set; }
}
