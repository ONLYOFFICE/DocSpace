namespace ASC.Web.Api.Controllers.Settings;

public class VersionController : BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;
    private readonly BuildVersion _buildVersion;

    public VersionController(
        PermissionContext permissionContext,
        ApiContext apiContext,
        TenantManager tenantManager,
        WebItemManager webItemManager,
        BuildVersion buildVersion,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _permissionContext = permissionContext;
        _tenantManager = tenantManager;
        _buildVersion = buildVersion;
    }

    [AllowAnonymous]
    [Read("version/build", false)]
    public Task<BuildVersion> GetBuildVersionsAsync()
    {
        return _buildVersion.GetCurrentBuildVersionAsync();
    }

    [Read("version")]
    public TenantVersionDto GetVersions()
    {
        return new TenantVersionDto(Tenant.Version, _tenantManager.GetTenantVersions());
    }

    [Update("version")]
    public TenantVersionDto SetVersionFromBody([FromBody] SettingsRequestsDto inDto)
    {
        return SetVersion(inDto);
    }

    [Update("version")]
    [Consumes("application/x-www-form-urlencoded")]
    public TenantVersionDto SetVersionFromForm([FromForm] SettingsRequestsDto inDto)
    {
        return SetVersion(inDto);
    }

    private TenantVersionDto SetVersion(SettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantManager.GetTenantVersions().FirstOrDefault(r => r.Id == inDto.VersionId).NotFoundIfNull();
        _tenantManager.SetTenantVersion(Tenant, inDto.VersionId);

        return GetVersions();
    }
}
