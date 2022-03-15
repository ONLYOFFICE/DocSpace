namespace ASC.AuditTrail.Models;

public class LoginEventDto : BaseEvent, IMapFrom<LoginEventQuery>
{
    public string Login { get; set; }
    public int Action { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<LoginEvent, LoginEventDto>();

        profile.CreateMap<LoginEventQuery, LoginEventDto>()
            .ConvertUsing<EventTypeConverter>();
    }
}