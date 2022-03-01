namespace ASC.Web.Api.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class SecurityController : ControllerBase
{
    private readonly PermissionContext _permissionContext;
    private readonly TenantExtra _tenantExtra;
    private readonly TenantManager _tenantManager;
    private readonly MessageService _messageService;
    private readonly LoginEventsRepository _loginEventsRepository;
    private readonly AuditEventsRepository _auditEventsRepository;
    private readonly AuditReportCreator auditReportCreator;
    private readonly SettingsManager _settingsManager;

    public SecurityController(
        PermissionContext permissionContext,
        TenantExtra tenantExtra,
        TenantManager tenantManager,
        MessageService messageService,
        LoginEventsRepository loginEventsRepository,
        AuditEventsRepository auditEventsRepository,
        AuditReportCreator auditReportCreator,
        SettingsManager settingsManager)
    {
        _permissionContext = permissionContext;
        _tenantExtra = tenantExtra;
        _tenantManager = tenantManager;
        _messageService = messageService;
        _loginEventsRepository = loginEventsRepository;
        _auditEventsRepository = auditEventsRepository;
        this.auditReportCreator = auditReportCreator;
        _settingsManager = settingsManager;
    }

    [Read("audit/login/last")]
    public IEnumerable<EventResponseDto> GetLastLoginEvents()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _loginEventsRepository.GetLast(_tenantManager.GetCurrentTenant().Id, 20).Select(x => new EventResponseDto(x));
    }

    [Read("audit/events/last")]
    public IEnumerable<EventResponseDto> GetLastAuditEvents()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _auditEventsRepository.GetLast(_tenantManager.GetCurrentTenant().Id, 20).Select(x => new EventResponseDto(x));
    }

    [Create("audit/login/report")]
    public object CreateLoginHistoryReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var tenantId = _tenantManager.GetCurrentTenant().Id;

        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.LoginHistoryLifeTime));

        var reportName = string.Format(AuditReportResource.LoginHistoryReportName + ".csv", from.ToShortDateString(), to.ToShortDateString());
        var events = _loginEventsRepository.Get(tenantId, from, to);
        var result = auditReportCreator.CreateCsvReport(events, reportName);

        _messageService.Send(MessageAction.LoginHistoryReportDownloaded);
        return result;
    }

    [Create("audit/events/report")]
    public object CreateAuditTrailReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var tenantId = _tenantManager.GetCurrentTenant().Id;

        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.AuditTrailLifeTime));

        var reportName = string.Format(AuditReportResource.AuditTrailReportName + ".csv", from.ToString("MM.dd.yyyy"), to.ToString("MM.dd.yyyy"));

        var events = _auditEventsRepository.Get(tenantId, from, to);
        var result = auditReportCreator.CreateCsvReport(events, reportName);

        _messageService.Send(MessageAction.AuditTrailReportDownloaded);
        return result;
    }

    [Read("audit/settings/lifetime")]
    public TenantAuditSettings GetAuditSettings()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);
    }

    [Create("audit/settings/lifetime")]
    public TenantAuditSettings SetAuditSettingsFromBody([FromBody] TenantAuditSettingsWrapper wrapper)
    {
        return SetAuditSettings(wrapper);
    }

    [Create("audit/settings/lifetime")]
    [Consumes("application/x-www-form-urlencoded")]
    public TenantAuditSettings SetAuditSettingsFromForm([FromForm] TenantAuditSettingsWrapper wrapper)
    {
        return SetAuditSettings(wrapper);
    }

    private TenantAuditSettings SetAuditSettings(TenantAuditSettingsWrapper wrapper)
    {
        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (wrapper.settings.LoginHistoryLifeTime <= 0 || wrapper.settings.LoginHistoryLifeTime > TenantAuditSettings.MaxLifeTime)
            throw new ArgumentException("LoginHistoryLifeTime");

        if (wrapper.settings.AuditTrailLifeTime <= 0 || wrapper.settings.AuditTrailLifeTime > TenantAuditSettings.MaxLifeTime)
            throw new ArgumentException("AuditTrailLifeTime");

        _settingsManager.SaveForTenant(wrapper.settings, _tenantManager.GetCurrentTenant().Id);
        _messageService.Send(MessageAction.AuditSettingsUpdated);

        return wrapper.settings;
    }
}
