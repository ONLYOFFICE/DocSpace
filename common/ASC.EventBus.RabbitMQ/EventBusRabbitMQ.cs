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

namespace ASC.EventBus.RabbitMQ;

public class EventBusRabbitMQ : IEventBus, IDisposable
{
    const string EXCHANGE_NAME = "asc_event_bus";
    const string DEAD_LETTER_EXCHANGE_NAME = "asc_event_bus_dlx";
    const string AUTOFAC_SCOPE_NAME = "asc_event_bus";

    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusRabbitMQ> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly int _retryCount;
    private readonly IIntegrationEventSerializer _serializer;

    private string _consumerTag;
    private IModel _consumerChannel;
    private string _queueName;
    private readonly string _deadLetterQueueName;

    private static ConcurrentQueue<Guid> _rejectedEvents;

    public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,
                            ILogger<EventBusRabbitMQ> logger,
                            ILifetimeScope autofac,
                            IEventBusSubscriptionsManager subsManager,
                            IIntegrationEventSerializer serializer,
                            string queueName = null,
                            int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _queueName = queueName;
        _deadLetterQueueName = $"{_queueName}_dlx";
        _consumerChannel = CreateConsumerChannel();
        _autofac = autofac;
        _retryCount = retryCount;
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        _serializer = serializer;
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
                _logger.WarningCouldNotPublishEvent(@event.Id, time.TotalSeconds, ex);
            });

        var eventName = @event.GetType().Name;

        _logger.TraceCreatingRabbitMQChannel(@event.Id, eventName);

        using (var channel = _persistentConnection.CreateModel())
        {
            _logger.TraceDeclaringRabbitMQChannel(@event.Id);

            channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "direct");

            var body = _serializer.Serialize(@event);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.TracePublishingEvent(@event.Id);

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
        _logger.InformationSubscribingDynamic(eventName, typeof(TH).GetGenericTypeName());

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

        _logger.InformationSubscribing(eventName, typeof(TH).GetGenericTypeName());

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

        _logger.InformationUnsubscribing(eventName);

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
        _logger.TraceStartingBasicConsume();

        if (_consumerChannel != null)
        {
            if (!String.IsNullOrEmpty(_consumerTag))
            {
                _logger.TraceConsumerTagExist(_consumerTag);

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
            _logger.ErrorStartBasicConsumeCantCall();
        }
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;

        // TODO: Need will remove after test
        if (!_subsManager.HasSubscriptionsForEvent(eventName))
        {
            _logger.WarningNoSubscription(eventName);

            // anti-pattern https://github.com/LeanKit-Labs/wascally/issues/36
            _consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);

            return;
        }

        var @event = GetEvent(eventName, eventArgs.Body.Span.ToArray());
        var message = @event.ToString();

        try
        {
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, @event);

            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (IntegrationEventRejectExeption ex)
        {
            _logger.WarningProcessingMessage(message, ex);

            if (_rejectedEvents.TryPeek(out var result) && result.Equals(ex.EventId))
            {
                _rejectedEvents.TryDequeue(out var _);
                _consumerChannel.BasicReject(eventArgs.DeliveryTag, requeue: false);
            }
            else
            {
                _rejectedEvents.Enqueue(ex.EventId);
                _consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        }
        catch (Exception ex)
        {
            _logger.WarningProcessingMessage(message, ex);

            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        _logger.TraceCreatingConsumerChannel();

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


        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", DEAD_LETTER_EXCHANGE_NAME }
        };

        channel.QueueDeclare(queue: _queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: arguments);

        channel.CallbackException += (sender, ea) =>
        {
            _logger.WarningRecreatingConsumerChannel(ea.Exception);

            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
            _consumerTag = String.Empty;

            StartBasicConsume();
        };

        return channel;
    }

    private IntegrationEvent GetEvent(string eventName, byte[] serializedMessage)
    {
        var eventType = _subsManager.GetEventTypeByName(eventName);

        var integrationEvent = (IntegrationEvent)_serializer.Deserialize(serializedMessage, eventType);

        return integrationEvent;
    }

    private void PreProcessEvent(IntegrationEvent @event)
    {
        if (_rejectedEvents.Count == 0)
        {
            return;
        }

        if (_rejectedEvents.TryPeek(out var result) && result.Equals(@event.Id))
        {
            @event.Redelivered = true;
        }
    }

    private async Task ProcessEvent(string eventName, IntegrationEvent @event)
    {
        _logger.TraceProcessingEvent(eventName);

        PreProcessEvent(@event);

        await using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
        {
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);

            foreach (var subscription in subscriptions)
            {
                if (subscription.IsDynamic)
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                    if (handler == null)
                    {
                        continue;
                    }

                    using dynamic eventData = @event;
                    await Task.Yield();
                    await handler.Handle(eventData);
                }
                else
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null)
                    {
                        continue;
                    }

                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                }
            }
        }
    }
}
