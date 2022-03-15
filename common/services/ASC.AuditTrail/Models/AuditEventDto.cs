namespace ASC.AuditTrail.Models;

public class AuditEventDto : BaseEvent, IMapFrom<AuditEventQuery>
{
    public string Initiator { get; set; }

    [Event("ActionIdCol", 33)]
    public int Action { get; set; }

    [Event("ActionTypeCol", 30)]
    public string ActionTypeText { get; set; }

    [Event("ProductCol", 31)]
    public string Product { get; set; }

    [Event("ModuleCol", 32)]
    public string Module { get; set; }

    [Event("TargetIdCol", 34)]
    public MessageTarget Target { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AuditEvent, AuditEventDto>();

        profile.CreateMap<AuditEventQuery, AuditEventDto>()
            .ConvertUsing<EventTypeConverter>();
    }
}