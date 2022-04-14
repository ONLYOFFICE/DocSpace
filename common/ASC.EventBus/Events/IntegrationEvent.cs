namespace ASC.EventBus.Events;

[ProtoContract]
public record IntegrationEvent
{          
    protected IntegrationEvent()
    {

    }

    public IntegrationEvent(Guid createBy, int tenantId) 
    {
        Id = Guid.NewGuid();
        CreateOn = DateTime.UtcNow;
        CreateBy = createBy;
        TenantId = tenantId;
    }

    [ProtoMember(1)]
    public Guid Id { get; private init; }

    [ProtoMember(2)]
    public DateTime CreateOn { get; private init; }

    [ProtoMember(3)]
    public Guid CreateBy { get; private init; }

    [ProtoMember(4)]
    public bool Redelivered { get; set; }

    [ProtoMember(5)]
    public int TenantId { get; private init; }

}
