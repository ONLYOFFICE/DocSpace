// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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
    private readonly AuditReportCreator _auditReportCreator;
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
        this._auditReportCreator = auditReportCreator;
        _settingsManager = settingsManager;
    }

    [HttpGet("audit/login/last")]
    public IEnumerable<ApiModel.ResponseDto.LoginEventDto> GetLastLoginEvents()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _loginEventsRepository.GetLast(_tenantManager.GetCurrentTenant().Id, 20).Select(x => new ApiModel.ResponseDto.LoginEventDto(x));
    }

    [HttpGet("audit/events/last")]
    public IEnumerable<ApiModel.ResponseDto.AuditEventDto> GetLastAuditEvents()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _auditEventsRepository.GetLast(_tenantManager.GetCurrentTenant().Id, 20).Select(x => new ApiModel.ResponseDto.AuditEventDto(x));
    }

    [HttpPost("audit/login/report")]
    public object CreateLoginHistoryReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var tenantId = _tenantManager.GetCurrentTenant().Id;

        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.LoginHistoryLifeTime));

        var reportName = string.Format(AuditReportResource.LoginHistoryReportName + ".csv", from.ToShortDateString(), to.ToShortDateString());
        var events = _loginEventsRepository.Get(tenantId, from, to);
        var result = _auditReportCreator.CreateCsvReport(events, reportName);

        _messageService.Send(MessageAction.LoginHistoryReportDownloaded);
        return result;
    }

    [HttpPost("audit/events/report")]
    public object CreateAuditTrailReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var tenantId = _tenantManager.GetCurrentTenant().Id;

        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.AuditTrail)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.AuditTrailLifeTime));

        var reportName = string.Format(AuditReportResource.AuditTrailReportName + ".csv", from.ToString("MM.dd.yyyy"), to.ToString("MM.dd.yyyy"));

        var events = _auditEventsRepository.Get(tenantId, from, to);
        var result = _auditReportCreator.CreateCsvReport(events, reportName);

        _messageService.Send(MessageAction.AuditTrailReportDownloaded);
        return result;
    }

    [HttpGet("audit/settings/lifetime")]
    public TenantAuditSettings GetAuditSettings()
    {
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);
    }

    [HttpPost("audit/settings/lifetime")]
    public TenantAuditSettings SetAuditSettings(TenantAuditSettingsWrapper wrapper)
    {
        if (!_tenantExtra.GetTenantQuota().Audit || !SetupInfo.IsVisibleSettings(nameof(ManagementType.LoginHistory)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (wrapper.Settings.LoginHistoryLifeTime <= 0 || wrapper.Settings.LoginHistoryLifeTime > TenantAuditSettings.MaxLifeTime)
        {
            throw new ArgumentException("LoginHistoryLifeTime");
        }

        if (wrapper.Settings.AuditTrailLifeTime <= 0 || wrapper.Settings.AuditTrailLifeTime > TenantAuditSettings.MaxLifeTime)
        {
            throw new ArgumentException("AuditTrailLifeTime");
        }

        _settingsManager.SaveForTenant(wrapper.Settings, _tenantManager.GetCurrentTenant().Id);
        _messageService.Send(MessageAction.AuditSettingsUpdated);

        return wrapper.Settings;
    }
}
