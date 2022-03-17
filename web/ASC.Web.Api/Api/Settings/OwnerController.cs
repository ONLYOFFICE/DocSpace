using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class OwnerController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly PermissionContext _permissionContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly MessageTarget _messageTarget;

    public OwnerController(
        MessageService messageService,
        CommonLinkUtility commonLinkUtility,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        AuthContext authContext,
        PermissionContext permissionContext,
        WebItemManager webItemManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        MessageTarget messageTarget,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _messageService = messageService;
        _commonLinkUtility = commonLinkUtility;
        _studioNotifyService = studioNotifyService;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _permissionContext = permissionContext;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
    }

    [Create("owner")]
    public object SendOwnerChangeInstructionsFromBody([FromBody] SettingsRequestsDto inDto)
    {
        return SendOwnerChangeInstructions(inDto);
    }

    [Create("owner")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendOwnerChangeInstructionsFromForm([FromForm] SettingsRequestsDto inDto)
    {
        return SendOwnerChangeInstructions(inDto);
    }

    private object SendOwnerChangeInstructions(SettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var curTenant = _tenantManager.GetCurrentTenant();
        var owner = _userManager.GetUsers(curTenant.OwnerId);
        var newOwner = _userManager.GetUsers(inDto.OwnerId);

        if (newOwner.IsVisitor(_userManager))
        {
            throw new System.Security.SecurityException("Collaborator can not be an owner");
        }

        if (!owner.Id.Equals(_authContext.CurrentAccount.ID) || Guid.Empty.Equals(newOwner.Id))
        {
            return new { Status = 0, Message = Resource.ErrorAccessDenied };
        }

        var confirmLink = _commonLinkUtility.GetConfirmationUrl(owner.Email, ConfirmType.PortalOwnerChange, newOwner.Id, newOwner.Id);
        _studioNotifyService.SendMsgConfirmChangeOwner(owner, newOwner, confirmLink);

        _messageService.Send(MessageAction.OwnerSentChangeOwnerInstructions, _messageTarget.Create(owner.Id), owner.DisplayUserName(false, _displayUserSettingsHelper));

        var emailLink = $"<a href=\"mailto:{owner.Email}\">{owner.Email}</a>";
        return new { Status = 1, Message = Resource.ChangePortalOwnerMsg.Replace(":email", emailLink) };
    }

    [Update("owner")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalOwnerChange")]
    public void OwnerFromBody([FromBody] SettingsRequestsDto inDto)
    {
        Owner(inDto);
    }

    [Update("owner")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalOwnerChange")]
    [Consumes("application/x-www-form-urlencoded")]
    public void OwnerFromForm([FromForm] SettingsRequestsDto inDto)
    {
        Owner(inDto);
    }

    private void Owner(SettingsRequestsDto inDto)
    {
        var newOwner = Constants.LostUser;
        try
        {
            newOwner = _userManager.GetUsers(inDto.OwnerId);
        }
        catch
        {
        }
        if (Constants.LostUser.Equals(newOwner))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (_userManager.IsUserInGroup(newOwner.Id, Constants.GroupVisitor.ID))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        var curTenant = _tenantManager.GetCurrentTenant();
        curTenant.OwnerId = newOwner.Id;
        _tenantManager.SaveTenant(curTenant);

        _messageService.Send(MessageAction.OwnerUpdated, newOwner.DisplayUserName(false, _displayUserSettingsHelper));
    }
}
