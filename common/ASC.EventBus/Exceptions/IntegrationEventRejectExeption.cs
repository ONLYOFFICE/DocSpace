namespace ASC.EventBus.Exceptions;

public class IntegrationEventRejectExeption : Exception
{
    public IntegrationEventRejectExeption()
    {
    }

    public IntegrationEventRejectExeption(string message)
        : base(message)
    {
    }

    public IntegrationEventRejectExeption(string message, Exception inner)
        : base(message, inner)
    {
    }
}
