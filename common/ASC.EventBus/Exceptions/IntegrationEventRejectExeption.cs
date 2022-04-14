namespace ASC.EventBus.Exceptions;

public class IntegrationEventRejectExeption : Exception
{
    public IntegrationEventRejectExeption(Guid eventId)
    {
        EventId = eventId;
    }

    public IntegrationEventRejectExeption(Guid eventId, string message)
        : base(message)
    {
        EventId = eventId;
    }

    public IntegrationEventRejectExeption(Guid eventId, string message, Exception inner)
        : base(message, inner)
    {
        EventId = eventId;    
    }

    public Guid EventId { get; private set; }
}
