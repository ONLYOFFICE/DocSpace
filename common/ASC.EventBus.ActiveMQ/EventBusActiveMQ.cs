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

namespace ASC.EventBus.ActiveMQ;

public class EventBusActiveMQ : IEventBus, IDisposable
{
    const string EXCHANGE_NAME = "asc_event_bus";
    const string AUTOFAC_SCOPE_NAME = "asc_event_bus";

    private readonly ILogger<EventBusActiveMQ> _logger;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;

    private static ConcurrentQueue<Guid> _rejectedEvents;
    private readonly IActiveMQPersistentConnection _persistentConnection;
    private readonly IIntegrationEventSerializer _serializer;
    private ISession _consumerSession;

    private readonly List<IMessageConsumer> _consumers;

    private readonly int _retryCount;
    private string _queueName;

    public EventBusActiveMQ(IActiveMQPersistentConnection persistentConnection,
                            ILogger<EventBusActiveMQ> logger,
                            ILifetimeScope autofac,
                            IEventBusSubscriptionsManager subsManager,
                            IIntegrationEventSerializer serializer,
                            string queueName = null,
                            int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _serializer = serializer;
        _queueName = queueName;
        _autofac = autofac;
        _retryCount = retryCount;
        _rejectedEvents = new ConcurrentQueue<Guid>();
        _consumerSession = CreateConsumerSession();
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        _consumers = new List<IMessageConsumer>();
    }

    private void SubsManager_OnEventRemoved(object sender, string eventName)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        using (var session = _persistentConnection.CreateSession())
        {
            var messageSelector = $"eventName='{eventName}'";

            var findedConsumer = _consumers.Find(x => x.MessageSelector == messageSelector);

            if (findedConsumer != null)
            {
                findedConsumer.Close();

                _consumers.Remove(findedConsumer);
            }

            if (_subsManager.IsEmpty)
            {
                _queueName = string.Empty;
                _consumerSession.Close();
            }
        }
    }

    public void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<SocketException>()
                            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                            {
                                _logger.WarningCouldNotPublishEvent(@event.Id, time.TotalSeconds, ex);
                            });

        using (var session = _persistentConnection.CreateSession(AcknowledgementMode.ClientAcknowledge))
        {
            var destination = session.GetQueue(_queueName);

            using (var producer = session.CreateProducer(destination))
            {
                producer.DeliveryMode = MsgDeliveryMode.Persistent;

                var body = _serializer.Serialize(@event);

                var request = session.CreateStreamMessage();
                var eventName = @event.GetType().Name;
                        
                request.Properties["eventName"] = eventName;

                request.WriteBytes(body);

                producer.Send(request);
            }
        }
    }

    public void Subscribe<T, TH>()
    where T : IntegrationEvent
    where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        _logger.InformationSubscribing(eventName, typeof(TH).GetGenericTypeName());

        _subsManager.AddSubscription<T, TH>();

        StartBasicConsume(eventName);
    }

    public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        _logger.InformationSubscribingDynamic(eventName, typeof(TH).GetGenericTypeName());

        _subsManager.AddDynamicSubscription<TH>(eventName);

        StartBasicConsume(eventName);
    }

    private ISession CreateConsumerSession()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        _logger.TraceCreatingConsumerSession();

        _consumerSession = _persistentConnection.CreateSession(AcknowledgementMode.ClientAcknowledge);

        return _consumerSession;
    }

    private void StartBasicConsume(string eventName)
    {
        _logger.TraceStartingBasicConsume();

        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var destination = _consumerSession.GetQueue(_queueName);

        var messageSelector = $"eventName='{eventName}'";

        var consumer = _consumerSession.CreateConsumer(destination, messageSelector);
       
        _consumers.Add(consumer);

        if (_consumerSession != null)
        {
            consumer.Listener += Consumer_Listener;
        }
        else
        {
            _logger.ErrorStartBasicConsumeCantCall();
        }
    }

    private void Consumer_Listener(IMessage objMessage)
    {
        var streamMessage = objMessage as IStreamMessage;

        var eventName = streamMessage.Properties["eventName"].ToString();
    
        var buffer = new byte[4 * 1024];

        byte[] serializedMessage;

        using (var ms = new MemoryStream())
        {
            int read;

            while ((read = streamMessage.ReadBytes(buffer)) > 0)
            {
                ms.Write(buffer, 0, read);

                if (read < buffer.Length)
                {
                    break;
                }
            }

            serializedMessage = ms.ToArray();
        }

        var @event = GetEvent(eventName, serializedMessage);
        var message = @event.ToString();

        try
        {
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            ProcessEvent(eventName, @event)
                .GetAwaiter()
                .GetResult();

            streamMessage.Acknowledge();
        }
        catch (IntegrationEventRejectExeption ex)
        {
            _logger.WarningProcessingMessage(message, ex);

            if (_rejectedEvents.TryPeek(out var result) && result.Equals(ex.EventId))
            {
                _rejectedEvents.TryDequeue(out var _);
                streamMessage.Acknowledge();
            }
            else
            {
                _rejectedEvents.Enqueue(ex.EventId);
            }

        }
        catch (Exception ex)
        {
            _logger.WarningProcessingMessage(message, ex);

            streamMessage.Acknowledge();
        }
    }

    private IntegrationEvent GetEvent(string eventName, byte[] serializedMessage)
    {
        var eventType = _subsManager.GetEventTypeByName(eventName);

        var integrationEvent = (IntegrationEvent)_serializer.Deserialize(serializedMessage, eventType);

        return integrationEvent;
    }


    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        _logger.InformationUnsubscribing(eventName);

        _subsManager.RemoveSubscription<T, TH>();
    }

    public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
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

        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
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
        else
        {
            _logger.WarningNoSubscription(eventName);
        }
    }

    public void Dispose()
    {
        foreach (var consumer in _consumers)
        {
            consumer.Dispose();
        }

        if (_consumerSession != null)
        {
            _consumerSession.Dispose();
        }

        _subsManager.Clear();
    }
}