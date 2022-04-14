namespace ASC.EventBus.Extensions.Logger;
public class IntegrationEventLogEntry
{
    private IntegrationEventLogEntry() { }
    public IntegrationEventLogEntry(IntegrationEvent @event, Guid transactionId)
    {
        EventId = @event.Id;
        CreateOn = @event.CreateOn;
        CreateBy = @event.CreateBy;
        TenantId = @event.TenantId;
        EventTypeName = @event.GetType().FullName;                     
        Content = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
        State = EventStateEnum.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId.ToString();
    }
    public Guid EventId { get; private set; }

    public string EventTypeName { get; private set; }
    
    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.')?.Last();
    
    [NotMapped]
    public IntegrationEvent IntegrationEvent { get; private set; }
    public EventStateEnum State { get; set; }
    public int TimesSent { get; set; }
    public DateTime CreateOn { get; private set; }
    public Guid CreateBy { get; private set; }
    public string Content { get; private set; }
    public string TransactionId { get; private set; }
    public int TenantId { get; private set; }
    
    public IntegrationEventLogEntry DeserializeJsonContent(Type type)
    {            
        IntegrationEvent = JsonSerializer.Deserialize(Content, type, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as IntegrationEvent;
        return this;
    }
}
