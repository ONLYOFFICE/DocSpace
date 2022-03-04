using System.Collections.Concurrent;

using ASC.EventBus.Exceptions;

namespace ASC.EventBus.RabbitMQ;

public class EventBusRabbitMQ : IEventBus, IDisposable
{
    const string EXCHANGE_NAME = "asc_event_bus";
    const string DEAD_LETTER_EXCHANGE_NAME = "asc_event_bus_dlx";
    const string AUTOFAC_SCOPE_NAME = "asc_event_bus";

    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly ILog _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly int _retryCount;

    private string _consumerTag;
    private IModel _consumerChannel;
    private string _queueName;
    private string _deadLetterQueueName;

    private static ConcurrentQueue<Guid> _rejectedEvents;

    public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, IOptionsMonitor<ILog> options,
        ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = options.CurrentValue ?? throw new ArgumentNullException(nameof(options.CurrentValue));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _queueName = queueName;
        _deadLetterQueueName = $"{_queueName}_dlx";
        _consumerChannel = CreateConsumerChannel();
        _autofac = autofac;
        _retryCount = retryCount;
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;

        _rejectedEvents = new ConcurrentQueue<Guid>();

    }

    private void SubsManager_OnEventRemoved(object sender, string eventName)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        using (var channel = _persistentConnection.CreateModel())
        {
            channel.QueueUnbind(queue: _queueName,
                exchange: EXCHANGE_NAME,
                routingKey: eventName);

            if (_subsManager.IsEmpty)
            {
                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }
    }

    public void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.Warn(string.Format("Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message), ex);
            });

        var eventName = @event.GetType().Name;

        _logger.TraceFormat("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

        using (var channel = _persistentConnection.CreateModel())
        {
            _logger.TraceFormat("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

            channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "direct");

            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.TraceFormat("Publishing event to RabbitMQ: {EventId}", @event.Id);

                channel.BasicPublish(
                    exchange: EXCHANGE_NAME,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }
    }

    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _logger.InfoFormat("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        DoInternalSubscription(eventName);
        _subsManager.AddDynamicSubscription<TH>(eventName);
        StartBasicConsume();
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        DoInternalSubscription(eventName);

        _logger.InfoFormat("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        _subsManager.AddSubscription<T, TH>();
        StartBasicConsume();
    }

    private void DoInternalSubscription(string eventName)
    {
        var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
        if (!containsKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel.QueueBind(queue: _deadLetterQueueName,
                                exchange: DEAD_LETTER_EXCHANGE_NAME,
                                routingKey: eventName);

            _consumerChannel.QueueBind(queue: _queueName,
                                exchange: EXCHANGE_NAME,
                                routingKey: eventName);
        }
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        _logger.InfoFormat("Unsubscribing from event {EventName}", eventName);

        _subsManager.RemoveSubscription<T, TH>();
    }

    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    public void Dispose()
    {
        if (_consumerChannel != null)
        {
            _consumerChannel.Dispose();
        }

        _subsManager.Clear();
    }

    private void StartBasicConsume()
    {
        _logger.Trace("Starting RabbitMQ basic consume");

        if (_consumerChannel != null)
        {
            if (!String.IsNullOrEmpty(_consumerTag))
            {
                _logger.TraceFormat("Consumer tag {ConsumerTag} already exist. Cancelled BasicConsume again", _consumerTag );

                return;
            }

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += Consumer_Received;

            _consumerTag = _consumerChannel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }
        else
        {
            _logger.Error("StartBasicConsume can't call on _consumerChannel == null");
        }
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
      
        try
        {
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, message);

           _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (IntegrationEventRejectExeption ex)
        {
            _logger.Warn(String.Format("----- ERROR Processing message \"{0}\"", message), ex);

            if (eventArgs.Redelivered)
            {
                if (_rejectedEvents.TryPeek(out Guid result) && result.Equals(ex.EventId))
                {
                    _rejectedEvents.TryDequeue(out Guid _);
                    _consumerChannel.BasicReject(eventArgs.DeliveryTag, requeue: false);
                }
                else
                {
                    _rejectedEvents.Enqueue(ex.EventId);
                }
            }
            else
            {
                _consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        }
        catch (Exception ex)
        {
            _logger.Warn(String.Format("----- ERROR Processing message \"{0}\"", message), ex);

            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        _logger.Trace("Creating RabbitMQ consumer channel");

        var channel = _persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange: EXCHANGE_NAME,
                                type: "direct");

        channel.ExchangeDeclare(exchange: DEAD_LETTER_EXCHANGE_NAME,              
                                type: "direct");

        channel.QueueDeclare(queue: _deadLetterQueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,                          
                        arguments: null);


        var arguments = new Dictionary<string, object>();

        arguments.Add("x-dead-letter-exchange", DEAD_LETTER_EXCHANGE_NAME);

        channel.QueueDeclare(queue: _queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: arguments);

        channel.CallbackException += (sender, ea) =>
        {
            _logger.WarnFormat("Recreating RabbitMQ consumer channel", ea.Exception);

            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
            _consumerTag = String.Empty;
            
            StartBasicConsume();
        };

        return channel;
    }


    public void PreProcessEvent(string eventName, ref string message)
    {
        if (_rejectedEvents.Count == 0) return;

        var eventType = _subsManager.GetEventTypeByName(eventName);

        var integrationEvent = (IntegrationEvent)JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        if (_rejectedEvents.TryPeek(out Guid result) && result.Equals(integrationEvent.Id))
        {
            integrationEvent.Redelivered = true;
        }

        message = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(integrationEvent, eventType, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    private IntegrationEvent PreProcessEvent(string eventName, string message)
    {
        var eventType = _subsManager.GetEventTypeByName(eventName);

        var integrationEvent = (IntegrationEvent)JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        if (_rejectedEvents.Count == 0) return integrationEvent;

        if (_rejectedEvents.TryPeek(out Guid result) && result.Equals(integrationEvent.Id))
        {
            integrationEvent.Redelivered = true;
        }

        return integrationEvent;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        _logger.TraceFormat("Processing RabbitMQ event: {EventName}", eventName);

        var @event = PreProcessEvent(eventName, message);

        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);

                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                        if (handler == null) continue;
                        using dynamic eventData = @event;
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }
            }
        }
        else
        {
            _logger.WarnFormat("No subscription for RabbitMQ event: {EventName}", eventName);
        }
    }
}
