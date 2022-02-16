namespace ASC.Web.Api.Controllers.Security;

public class EventsController: BaseSecurityController
{
    public EventsController(PermissionContext permissionContext,
        TenantExtra tenantExtra,
        TenantManager tenantManager, 
        MessageService messageService, 
        LoginEventsRepository loginEventsRepository, 
        AuditEventsRepository auditEventsRepository, 
        AuditReportCreator auditReportCreator,
        SettingsManager settingsManager) : base(permissionContext, tenantExtra, tenantManager, messageService, loginEventsRepository, auditEventsRepository, auditReportCreator, settingsManager)
    {
    }

    [Read("audit/events/last")]
    public IEnumerable<EventWrapper> GetLastAuditEvents()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _auditEventsRepository.GetLast(_tenantManager.GetCurrentTenant().TenantId, 20).Select(x => new EventWrapper(x));
    }



    [Create("audit/events/report")]
    public object CreateAuditTrailReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var tenantId = _tenantManager.GetCurrentTenant().TenantId;

        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().TenantId);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.AuditTrailLifeTime));

        var reportName = string.Format(AuditReportResource.AuditTrailReportName + ".csv", from.ToString("MM.dd.yyyy"), to.ToString("MM.dd.yyyy"));

        var events = _auditEventsRepository.Get(tenantId, from, to);
        var result = _auditReportCreator.CreateCsvReport(events, reportName);

        _messageService.Send(MessageAction.AuditTrailReportDownloaded);
        return result;
    }
}
