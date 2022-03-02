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
    public object SendOwnerChangeInstructionsFromBody([FromBody] SettingsDto model)
    {
        return SendOwnerChangeInstructions(model);
    }

    [Create("owner")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendOwnerChangeInstructionsFromForm([FromForm] SettingsDto model)
    {
        return SendOwnerChangeInstructions(model);
    }

    private object SendOwnerChangeInstructions(SettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var curTenant = _tenantManager.GetCurrentTenant();
        var owner = _userManager.GetUsers(curTenant.OwnerId);
        var newOwner = _userManager.GetUsers(model.OwnerId);

        if (newOwner.IsVisitor(_userManager)) throw new System.Security.SecurityException("Collaborator can not be an owner");

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
    public void OwnerFromBody([FromBody] SettingsDto model)
    {
        Owner(model);
    }

    [Update("owner")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalOwnerChange")]
    [Consumes("application/x-www-form-urlencoded")]
    public void OwnerFromForm([FromForm] SettingsDto model)
    {
        Owner(model);
    }

    private void Owner(SettingsDto model)
    {
        var newOwner = Constants.LostUser;
        try
        {
            newOwner = _userManager.GetUsers(model.OwnerId);
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
