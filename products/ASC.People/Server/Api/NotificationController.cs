namespace ASC.People.Api;

public class NotificationController : ApiControllerBase
{
    private readonly NotificationControllerEngine _notificationControllerEngine;

    public NotificationController(NotificationControllerEngine notificationControllerEngine)
    {
        _notificationControllerEngine = notificationControllerEngine;
    }

    [Create("phone")]
    public object SendNotificationToChangeFromBody([FromBody] UpdateMemberRequestDto model)
    {
        return _notificationControllerEngine.SendNotificationToChange(model.UserId);
    }

    [Create("phone")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendNotificationToChangeFromForm([FromForm] UpdateMemberRequestDto model)
    {
        return _notificationControllerEngine.SendNotificationToChange(model.UserId);
    }
}