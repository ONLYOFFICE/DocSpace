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
    public IEnumerable<string> SaveIpRestrictionsFromBody([FromBody] IpRestrictionsRequestsDto inDto)
    {
        return SaveIpRestrictions(inDto);
    }

    [Update("iprestrictions")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<string> SaveIpRestrictionsFromForm([FromForm] IpRestrictionsRequestsDto inDto)
    {
        return SaveIpRestrictions(inDto);
    }

    private IEnumerable<string> SaveIpRestrictions(IpRestrictionsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        return _iPRestrictionsService.Save(inDto.Ips, Tenant.Id);
    }

    [Update("iprestrictions/settings")]
    public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromBody([FromBody] IpRestrictionsRequestsDto inDto)
    {
        return UpdateIpRestrictionsSettings(inDto);
    }

    [Update("iprestrictions/settings")]
    [Consumes("application/x-www-form-urlencoded")]
    public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromForm([FromForm] IpRestrictionsRequestsDto inDto)
    {
        return UpdateIpRestrictionsSettings(inDto);
    }

    private IPRestrictionsSettings UpdateIpRestrictionsSettings(IpRestrictionsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = new IPRestrictionsSettings { Enable = inDto.Enable };
        _settingsManager.Save(settings);

        return settings;
    }
}