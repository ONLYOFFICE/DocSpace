using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class GreetingSettingsController: BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;

    public GreetingSettingsController(
        ApiContext apiContext,
        TenantManager tenantManager,
        PermissionContext permissionContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _tenantManager = tenantManager;
        _permissionContext = permissionContext;
    }

    [Read("greetingsettings")]
    public ContentResult GetGreetingSettings()
    {
        return new ContentResult { Content = Tenant.Name };
    }

    [Create("greetingsettings")]
    public ContentResult SaveGreetingSettingsFromBody([FromBody] GreetingSettingsDto model)
    {
        return SaveGreetingSettings(model);
    }

    [Create("greetingsettings")]
    [Consumes("application/x-www-form-urlencoded")]
    public ContentResult SaveGreetingSettingsFromForm([FromForm] GreetingSettingsDto model)
    {
        return SaveGreetingSettings(model);
    }

    private ContentResult SaveGreetingSettings(GreetingSettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        Tenant.Name = model.Title;
        _tenantManager.SaveTenant(Tenant);

        _messageService.Send(MessageAction.GreetingSettingsUpdated);

        return new ContentResult { Content = Resource.SuccessfullySaveGreetingSettingsMessage };
    }

    [Create("greetingsettings/restore")]
    public ContentResult RestoreGreetingSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantInfoSettingsHelper.RestoreDefaultTenantName();

        return new ContentResult
        {
            Content = Tenant.Name
        };
    }
}
