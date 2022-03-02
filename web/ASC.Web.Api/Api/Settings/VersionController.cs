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
    public TenantVersionResponseDto GetVersions()
    {
        return new TenantVersionResponseDto(Tenant.Version, _tenantManager.GetTenantVersions());
    }

    [Update("version")]
    public TenantVersionResponseDto SetVersionFromBody([FromBody] SettingsDto model)
    {
        return SetVersion(model);
    }

    [Update("version")]
    [Consumes("application/x-www-form-urlencoded")]
    public TenantVersionResponseDto SetVersionFromForm([FromForm] SettingsDto model)
    {
        return SetVersion(model);
    }

    private TenantVersionResponseDto SetVersion(SettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantManager.GetTenantVersions().FirstOrDefault(r => r.Id == model.VersionId).NotFoundIfNull();
        _tenantManager.SetTenantVersion(Tenant, model.VersionId);

        return GetVersions();
    }
}
