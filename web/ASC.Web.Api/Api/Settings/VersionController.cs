using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class VersionController : BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;
    private readonly BuildVersion _buildVersion;

    public VersionController(
        ApiContext apiContext,
        TenantManager tenantManager,
        WebItemManager webItemManager,
        BuildVersion buildVersion,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _tenantManager = tenantManager;
        _buildVersion = buildVersion;
    }

    [AllowAnonymous]
    [Read("version/build", false)]
    public BuildVersion GetBuildVersions()
    {
        return _buildVersion.GetCurrentBuildVersion();
    }

    [Read("version")]
    public TenantVersionWrapper GetVersions()
    {
        return new TenantVersionWrapper(Tenant.Version, _tenantManager.GetTenantVersions());
    }

    [Update("version")]
    public TenantVersionWrapper SetVersionFromBody([FromBody] SettingsDto model)
    {
        return SetVersion(model);
    }

    [Update("version")]
    [Consumes("application/x-www-form-urlencoded")]
    public TenantVersionWrapper SetVersionFromForm([FromForm] SettingsDto model)
    {
        return SetVersion(model);
    }

    private TenantVersionWrapper SetVersion(SettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantManager.GetTenantVersions().FirstOrDefault(r => r.Id == model.VersionId).NotFoundIfNull();
        _tenantManager.SetTenantVersion(Tenant, model.VersionId);

        return GetVersions();
    }
}
