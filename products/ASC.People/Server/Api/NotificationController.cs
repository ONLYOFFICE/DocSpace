using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

public class NotificationController : BaseApiController
{
    private readonly CommonLinkUtility _commonLinkUtility;

    public NotificationController(
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        MessageService messageService,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService,
        CommonLinkUtility commonLinkUtility)
        : base(
            userManager,
            authContext,
            apiContext,
            permissionContext,
            securityContext,
            messageService,
            messageTarget,
            studioNotifyService)
    {
        _commonLinkUtility = commonLinkUtility;
    }

    [Create("phone")]
    public object SendNotificationToChangeFromBody([FromBody] UpdateMemberRequestDto model)
    {
        return SendNotificationToChange(model.UserId);
    }

    [Create("phone")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendNotificationToChangeFromForm([FromForm] UpdateMemberRequestDto model)
    {
        return SendNotificationToChange(model.UserId);
    }

    private object SendNotificationToChange(string userId)
    {
        var user = UserManager.GetUsers(
            string.IsNullOrEmpty(userId)
                ? SecurityContext.CurrentAccount.ID
                : new Guid(userId));
        var canChange =
        user.IsMe(AuthContext)
                    || PermissionContext.CheckPermissions(new UserSecurityProvider(user.ID), Constants.Action_EditUser);

        if (!canChange)
        {
            throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);
        }

        user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.NotActivated;
        UserManager.SaveUserInfo(user);
        if (user.IsMe(AuthContext))
        {
            return _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneActivation);
        }

        StudioNotifyService.SendMsgMobilePhoneChange(user);

        return string.Empty;
    }
}
