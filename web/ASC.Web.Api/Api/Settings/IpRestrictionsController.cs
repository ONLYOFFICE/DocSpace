namespace ASC.Web.Api.Controllers.Settings;

public class IpRestrictionsController: BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly IPRestrictionsService _iPRestrictionsService;

    public IpRestrictionsController(
        ApiContext apiContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        IPRestrictionsService iPRestrictionsService,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _iPRestrictionsService = iPRestrictionsService;
    }

    [Read("iprestrictions")]
    public IEnumerable<IPRestriction> GetIpRestrictions()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        return _iPRestrictionsService.Get(Tenant.Id);
    }

    [Update("iprestrictions")]
    public IEnumerable<string> SaveIpRestrictionsFromBody([FromBody] IpRestrictionsDto model)
    {
        return SaveIpRestrictions(model);
    }

    [Update("iprestrictions")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<string> SaveIpRestrictionsFromForm([FromForm] IpRestrictionsDto model)
    {
        return SaveIpRestrictions(model);
    }

    private IEnumerable<string> SaveIpRestrictions(IpRestrictionsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        return _iPRestrictionsService.Save(model.Ips, Tenant.Id);
    }

    [Update("iprestrictions/settings")]
    public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromBody([FromBody] IpRestrictionsDto model)
    {
        return UpdateIpRestrictionsSettings(model);
    }

    [Update("iprestrictions/settings")]
    [Consumes("application/x-www-form-urlencoded")]
    public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromForm([FromForm] IpRestrictionsDto model)
    {
        return UpdateIpRestrictionsSettings(model);
    }

    private IPRestrictionsSettings UpdateIpRestrictionsSettings(IpRestrictionsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = new IPRestrictionsSettings { Enable = model.Enable };
        _settingsManager.Save(settings);

        return settings;
    }
}