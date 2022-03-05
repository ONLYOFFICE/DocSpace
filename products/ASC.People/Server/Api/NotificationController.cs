using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

public class NotificationController : ApiControllerBase
{
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly AuthContext _authContext;
    private readonly PermissionContext _permissionContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly StudioNotifyService _studioNotifyService;

    public NotificationController(
        UserManager userManager,
        SecurityContext securityContext,
        AuthContext authContext,
        PermissionContext permissionContext,
        CommonLinkUtility commonLinkUtility,
        StudioNotifyService studioNotifyService)
    {
        _userManager = userManager;
        _securityContext = securityContext;
        _authContext = authContext;
        _permissionContext = permissionContext;
        _commonLinkUtility = commonLinkUtility;
        _studioNotifyService = studioNotifyService;
    }

    [Create("phone")]
    public object SendNotificationToChangeFromBody([FromBody] UpdateMemberRequestDto inDto)
    {
        return SendNotificationToChange(inDto.UserId);
    }

    [Create("phone")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendNotificationToChangeFromForm([FromForm] UpdateMemberRequestDto inDto)
    {
        return SendNotificationToChange(inDto.UserId);
    }

    private object SendNotificationToChange(string userId)
    {
        var user = _userManager.GetUsers(string.IsNullOrEmpty(userId) 
            ? _securityContext.CurrentAccount.ID : new Guid(userId));

        var canChange = user.IsMe(_authContext) || _permissionContext.CheckPermissions(new UserSecurityProvider(user.Id), Constants.Action_EditUser);

        if (!canChange)
        {
            throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);
        }

        user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.NotActivated;

        _userManager.SaveUserInfo(user);

        if (user.IsMe(_authContext))
        {
            return _commonLinkUtility.GetConfirmationUrl(user.Email, ConfirmType.PhoneActivation);
        }

        _studioNotifyService.SendMsgMobilePhoneChange(user);

        return string.Empty;
    }
}