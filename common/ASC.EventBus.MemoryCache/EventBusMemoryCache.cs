using ASC.Common.Logging;
using ASC.EventBus.Extensions;

using Microsoft.Extensions.Options;

namespace ASC.EventBus.MemoryCache;

public class EventBusMemoryCache : IEventBus, IDisposable
{
    public IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly ILog _logger;
    const string AUTOFAC_SCOPE_NAME = "asc_event_bus";

    public EventBusMemoryCache(IOptionsMonitor<ILog> options , ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager)
    {
        _logger = options.CurrentValue ?? throw new ArgumentNullException(nameof(options.CurrentValue));
        _autofac =  autofac ??  throw new ArgumentNullException(nameof(autofac));
        _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager)); 
    }

    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name;
        var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());

        ProcessEvent(eventName, jsonMessage)
            .GetAwaiter()
            .GetResult();
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        _logger.InfoFormat("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        _subsManager.AddSubscription<T, TH>();
    }

    public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        _logger.InfoFormat("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

        _subsManager.AddDynamicSubscription<TH>(eventName);
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        _logger.InfoFormat("Unsubscribing from event {EventName}", eventName);

        _subsManager.RemoveSubscription<T, TH>();
    }

    public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        _logger.InfoFormat("Unsubscribing from dynamic event {EventName}", eventName);

        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        _logger.TraceFormat("Processing RabbitMQ event: {EventName}", eventName);

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
                        using dynamic eventData = JsonDocument.Parse(message);
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
        else
        {
            _logger.WarnFormat("No subscription for RabbitMQ event: {EventName}", eventName);
        }
    }

    public void Dispose()
    {
        _subsManager.Clear();
    }
}

