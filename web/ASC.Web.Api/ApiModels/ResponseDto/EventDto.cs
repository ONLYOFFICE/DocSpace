namespace ASC.Web.Api.ApiModel.ResponseDto;

public class EventDto
{
    public EventDto(BaseEvent auditEvent)
    {
        Id = auditEvent.Id;
        Date = new ApiDateTime(auditEvent.Date, TimeSpan.Zero);
        User = auditEvent.UserName;
        Action = auditEvent.ActionText;
    }

    public int Id { get; private set; }
    public ApiDateTime Date { get; private set; }
    public string User { get; private set; }
    public string Action { get; private set; }
}