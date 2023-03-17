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

using AuditEventDto = ASC.Web.Api.ApiModel.ResponseDto.AuditEventDto;
using LoginEventDto = ASC.Web.Api.ApiModel.ResponseDto.LoginEventDto;

namespace ASC.Web.Api.Controllers;

/// <summary>
/// Security API.
/// </summary>
/// <name>security</name>
[Scope]
[DefaultRoute]
[ApiController]
public class SecurityController : ControllerBase
{
    private readonly PermissionContext _permissionContext;
    private readonly TenantManager _tenantManager;
    private readonly MessageService _messageService;
    private readonly LoginEventsRepository _loginEventsRepository;
    private readonly AuditEventsRepository _auditEventsRepository;
    private readonly AuditReportCreator _auditReportCreator;
    private readonly AuditReportUploader _auditReportSaver;
    private readonly SettingsManager _settingsManager;
    private readonly AuditActionMapper _auditActionMapper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ApiContext _apiContext;

    public SecurityController(
        PermissionContext permissionContext,
        TenantManager tenantManager,
        MessageService messageService,
        LoginEventsRepository loginEventsRepository,
        AuditEventsRepository auditEventsRepository,
        AuditReportCreator auditReportCreator,
        AuditReportUploader auditReportSaver,
        SettingsManager settingsManager,
        AuditActionMapper auditActionMapper,
        CoreBaseSettings coreBaseSettings,
        ApiContext apiContext)
    {
        _permissionContext = permissionContext;
        _tenantManager = tenantManager;
        _messageService = messageService;
        _loginEventsRepository = loginEventsRepository;
        _auditEventsRepository = auditEventsRepository;
        _auditReportCreator = auditReportCreator;
        _auditReportSaver = auditReportSaver;
        _settingsManager = settingsManager;
        _auditActionMapper = auditActionMapper;
        _coreBaseSettings = coreBaseSettings;
        _apiContext = apiContext;
    }

    /// <summary>
    /// Returns all the latest user login activity including successful logins and failed attempts with an indication of reasons.
    /// </summary>
    /// <short>
    /// Get login history
    /// </short>
    /// <category>Login history</category>
    /// <returns>List of login events</returns>
    /// <path>api/2.0/security/audit/login/last</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("audit/login/last")]
    public IEnumerable<LoginEventDto> GetLastLoginEvents()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandBaseAuditPermission();

        return _loginEventsRepository.GetByFilter(startIndex: 0, limit: 20).Select(x => new LoginEventDto(x));
    }

    /// <summary>
    /// Returns a list of the latest changes (creation, modification, deletion, etc.) made by users to the entities (tasks, opportunities, files, etc.) on the portal.
    /// </summary>
    /// <short>
    /// Get audit trail data
    /// </short>
    /// <category>Audit trail data</category>
    /// <returns>List of audit trail data</returns>
    /// <path>api/2.0/security/audit/events/last</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("audit/events/last")]
    public IEnumerable<AuditEventDto> GetLastAuditEvents()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandBaseAuditPermission();

        return _auditEventsRepository.GetByFilter(startIndex: 0, limit: 20).Select(x => new AuditEventDto(x, _auditActionMapper));
    }

    /// <summary>
    /// Returns a list of the login events by the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Get filtered login events
    /// </short>
    /// <category>Login history</category>
    /// <param type="System.Guid, System" name="userId">User ID</param>
    /// <param type="ASC.MessagingSystem.Core.MessageAction, ASC.MessagingSystem.Core" name="action">Action</param>
    /// <param type="ASC.Api.Core.ApiDateTime, ASC.Api.Core" name="from">Start date</param>
    /// <param type="ASC.Api.Core.ApiDateTime, ASC.Api.Core" name="to">End date</param>
    /// <returns>List of filtered login events</returns>
    /// <path>api/2.0/security/audit/login/filter</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("/audit/login/filter")]
    public IEnumerable<LoginEventDto> GetLoginEventsByFilter(Guid userId,
    MessageAction action,
    ApiDateTime from,
    ApiDateTime to)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var startIndex = (int)_apiContext.StartIndex;
        var limit = (int)_apiContext.Count;
        _apiContext.SetDataPaginated();

        action = action == 0 ? MessageAction.None : action;

        if (!_tenantManager.GetCurrentTenantQuota().Audit || !SetupInfo.IsVisibleSettings(ManagementType.LoginHistory.ToString()))
        {
            return GetLastLoginEvents();
        }
        else
        {
            DemandAuditPermission();

            return _loginEventsRepository.GetByFilter(userId, action, from, to, startIndex, limit).Select(x => new LoginEventDto(x));
        }
    }

    /// <summary>
    /// Returns a list of the audit events by the parameters specified in the request.
    /// </summary>
    /// <short>
    /// Get filtered audit trail data
    /// </short>
    /// <category>Audit trail data</category>
    /// <param type="System.Guid, System" name="userId">User ID</param>
    /// <param type="ASC.AuditTrail.Types.ProductType, ASC.AuditTrail.Types" name="productType">Product (None, CRM, Documents, Login, Others, People, Projects, Settings)</param>
    /// <param type="ASC.AuditTrail.Types.ModuleType, ASC.AuditTrail.Types" name="moduleType">Module (None, Files, Folders, DocumentsSettings, Companies, Persons, Contacts, CrmTasks, Opportunities, Invoices, Cases, CommonCrmSettings, ContactsSettings, ContactTypes, InvoiceSettings, OtherCrmSettings, Users, Groups, Projects, Milestones, Tasks, Discussions, TimeTracking, Reports, ProjectsSettings, General, Products, Rooms)</param>
    /// <param type="ASC.AuditTrail.Types.ActionType, ASC.AuditTrail.Types" name="actionType">Action type (None, Create, Update, Delete, Link, Unlink, Attach, Detach, Send, Import, Export, UpdateAccess, Download, Upload, Copy, Move, Reassigns, Follow, Unfollow, Logout)</param>
    /// <param type="ASC.MessagingSystem.Core.MessageAction, ASC.MessagingSystem.Core" name="action">Action</param>
    /// <param type="ASC.AuditTrail.Types.EntryType, ASC.AuditTrail.Types" name="entryType">Entry (None, File, Folder, Project, Contact, Milestone, Task, Comment, SubTask, Message, TimeSpend, ReportTemplate, Template, Relationship, CRMTask, Opportunity, Invoice, Case, ListItem, InvoiceItem, InvoiceTax, FieldDescription, OpportunityMilestone, User, Group, Room, Tag)</param>
    /// <param type="System.String, System" name="target">Target</param>
    /// <param type="ASC.Api.Core.ApiDateTime, ASC.Api.Core" name="from">Start date</param>
    /// <param type="ASC.Api.Core.ApiDateTime, ASC.Api.Core" name="to">End date</param>
    /// <returns>List of filtered audit trail data</returns>
    /// <path>api/2.0/security/audit/events/filter</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("/audit/events/filter")]
    public IEnumerable<AuditEventDto> GetAuditEventsByFilter(Guid userId,
            ProductType productType,
            ModuleType moduleType,
            ActionType actionType,
            MessageAction action,
            EntryType entryType,
            string target,
            ApiDateTime from,
            ApiDateTime to)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var startIndex = (int)_apiContext.StartIndex;
        var limit = (int)_apiContext.Count;
        _apiContext.SetDataPaginated();

        action = action == 0 ? MessageAction.None : action;

        if (!_tenantManager.GetCurrentTenantQuota().Audit || !SetupInfo.IsVisibleSettings(ManagementType.LoginHistory.ToString()))
        {
            return GetLastAuditEvents();
        }
        else
        {
            DemandAuditPermission();

            return _auditEventsRepository.GetByFilter(userId, productType, moduleType, actionType, action, entryType, target, from, to, startIndex, limit).Select(x => new AuditEventDto(x, _auditActionMapper));
        }
    }

    /// <summary>
    /// Returns all the available audit trail types.
    /// </summary>
    /// <short>
    /// Get audit trail types
    /// </short>
    /// <category>Audit trail data</category>
    /// <returns>Audit trail types</returns>
    /// <path>api/2.0/security/audit/types</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [HttpGet("audit/types")]
    public object GetTypes()
    {
        return new
        {
            Actions = MessageActionExtensions.GetNames(),
            ActionTypes = ActionTypeExtensions.GetNames(),
            ProductTypes = ProductTypeExtensions.GetNames(),
            ModuleTypes = ModuleTypeExtensions.GetNames(),
            EntryTypes = EntryTypeExtensions.GetNames()
        };
    }

    /// <summary>
    /// Returns the mappers for the audit trail types.
    /// </summary>
    /// <short>
    /// Get audit trail mappers
    /// </short>
    /// <category>Audit trail data</category>
    /// <param type="System.Nullable{ASC.AuditTrail.Types.ProductType}, System" name="productType">Product</param>
    /// <param type="System.Nullable{ASC.AuditTrail.Types.ModuleType}, System" name="moduleType">Module</param>
    /// <returns>Audit trail mappers</returns>
    /// <path>api/2.0/security/audit/mappers</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    [AllowAnonymous]
    [HttpGet("/audit/mappers")]
    public object GetMappers(ProductType? productType, ModuleType? moduleType)
    {
        return _auditActionMapper.Mappers
            .Where(r => !productType.HasValue || r.Product == productType.Value)
            .Select(r => new
            {
                ProductType = r.Product.ToString(),
                Modules = r.Mappers
                .Where(m => !moduleType.HasValue || m.Module == moduleType.Value)
                .Select(x => new
                {
                    ModuleType = x.Module.ToString(),
                    Actions = x.Actions.Select(a => new
                    {
                        MessageAction = a.Key.ToString(),
                        ActionType = a.Value.ActionType.ToString(),
                        Entity = a.Value.EntryType1.ToString()
                    })
                })
            });
    }

    /// <summary>
    /// Generates the login history report.
    /// </summary>
    /// <short>
    /// Generate the login history report
    /// </short>
    /// <category>Login history</category>
    /// <returns>URL to the xlsx report file</returns>
    /// <path>api/2.0/security/audit/login/report</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("audit/login/report")]
    public async Task<object> CreateLoginHistoryReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandAuditPermission();

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.LoginHistoryLifeTime));

        var reportName = string.Format(AuditReportResource.LoginHistoryReportName + ".csv", from.ToShortDateString(), to.ToShortDateString());
        var events = _loginEventsRepository.GetByFilter(fromDate: from, to: to);

        using var stream = _auditReportCreator.CreateCsvReport(events);
        var result = await _auditReportSaver.UploadCsvReport(stream, reportName);

        _messageService.Send(MessageAction.LoginHistoryReportDownloaded);
        return result;
    }

    /// <summary>
    /// Generates the audit trail report.
    /// </summary>
    /// <short>
    /// Generate the audit trail report
    /// </short>
    /// <category>Audit trail data</category>
    /// <returns>URL to the xlsx report file</returns>
    /// <path>api/2.0/security/audit/events/report</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("audit/events/report")]
    public async Task<object> CreateAuditTrailReport()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandAuditPermission();

        var tenantId = _tenantManager.GetCurrentTenant().Id;

        var settings = _settingsManager.LoadForTenant<TenantAuditSettings>(tenantId);

        var to = DateTime.UtcNow;
        var from = to.Subtract(TimeSpan.FromDays(settings.AuditTrailLifeTime));

        var reportName = string.Format(AuditReportResource.AuditTrailReportName + ".csv", from.ToString("MM.dd.yyyy"), to.ToString("MM.dd.yyyy"));

        var events = _auditEventsRepository.GetByFilter(from: from, to: to);

        using var stream = _auditReportCreator.CreateCsvReport(events);
        var result = await _auditReportSaver.UploadCsvReport(stream, reportName);

        _messageService.Send(MessageAction.AuditTrailReportDownloaded);
        return result;
    }

    /// <summary>
    /// Returns the audit trail settings.
    /// </summary>
    /// <short>
    /// Get the audit trail settings
    /// </short>
    /// <category>Audit trail data</category>
    /// <returns>Audit settings</returns>
    /// <path>api/2.0/security/audit/settings/lifetime</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("audit/settings/lifetime")]
    public TenantAuditSettings GetAuditSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandBaseAuditPermission();

        return _settingsManager.LoadForTenant<TenantAuditSettings>(_tenantManager.GetCurrentTenant().Id);
    }

    /// <summary>
    /// Sets the audit trail settings for the current portal.
    /// </summary>
    /// <short>
    /// Set the audit trail settings
    /// </short>
    /// <category>Audit trail data</category>
    /// <param type="ASC.Core.Tenants.TenantAuditSettingsWrapper, ASC.Core.Tenants" name="wrapper">Audit trail settings: <![CDATA[
    /// <ul>
    ///     <li><b>Settings</b> (TenantAuditSettings) - audit trail settings:</li>
    ///     <ul>
    ///         <li><b>LoginHistoryLifeTime</b> (integer) - login history lifetime,</li>
    ///         <li><b>AuditTrailLifeTime</b> (integer) - audit trail lifetime.</li>
    ///     </ul>
    /// </ul>
    /// ]]></param>
    /// <returns>Audit trail settings</returns>
    /// <path>api/2.0/security/audit/settings/lifetime</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("audit/settings/lifetime")]
    public TenantAuditSettings SetAuditSettings(TenantAuditSettingsWrapper wrapper)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandAuditPermission();

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

    private void DemandAuditPermission()
    {
        if (!_coreBaseSettings.Standalone
            && (!SetupInfo.IsVisibleSettings(ManagementType.LoginHistory.ToString())
                || !_tenantManager.GetCurrentTenantQuota().Audit))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }
    }

    private void DemandBaseAuditPermission()
    {
        if (!_coreBaseSettings.Standalone
            && !SetupInfo.IsVisibleSettings(ManagementType.LoginHistory.ToString()))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Audit");
        }
    }
}
