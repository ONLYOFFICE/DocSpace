namespace ASC.Web.Api.Controllers.Security;

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("security")]
public class BaseSecurityController : ControllerBase
{
    internal readonly PermissionContext _permissionContext;
    internal readonly TenantExtra _tenantExtra;
    internal readonly TenantManager _tenantManager;
    internal readonly MessageService _messageService;
    internal readonly LoginEventsRepository _loginEventsRepository;
    internal readonly AuditEventsRepository _auditEventsRepository;
    internal readonly AuditReportCreator _auditReportCreator;
    internal readonly SettingsManager _settingsManager;

    public BaseSecurityController(
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
        _auditReportCreator = auditReportCreator;
        _settingsManager = settingsManager;
    }
}