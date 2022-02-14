namespace ASC.EventBus.Events;

public record IntegrationEvent
{        
    private IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreateOn = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid createBy, int tenantId) : base()
    {
        CreateBy = createBy;
        TenantId = tenantId;
    }

    [JsonInclude]
    public Guid Id { get; private init; }

    [JsonInclude]
    public DateTime CreateOn { get; private init; }

    [JsonInclude]
    public Guid CreateBy { get; private init; }

    [JsonInclude]
    public int TenantId { get; private init; }

}
