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

/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Files.Core;
using ASC.Web.Files.Services.DocumentService;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.CRM.Classes
{
    [Scope]
    public class ReportHelper
    {
        private readonly CurrencyProvider _currencyProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SecurityContext _securityContext;
        private readonly DocbuilderReportsUtilityHelper _docbuilderReportsUtilityHelper;
        private readonly DaoFactory _daoFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Global _global;
        private readonly SettingsManager _settingsManager;
        private readonly TenantUtil _tenantUtil;
        private readonly TenantManager _tenantManager;
        private readonly IHttpClientFactory _clientFactory;

        public ReportHelper(TenantManager tenantManager,
                            TenantUtil tenantUtil,
                            Global global,
                            DocbuilderReportsUtilityHelper docbuilderReportsUtilityHelper,
                            SettingsManager settingsManager,
                            DaoFactory daoFactory,
                            SecurityContext securityContext,
                            IServiceProvider serviceProvider,
                            IHttpContextAccessor httpContextAccessor,
                            CurrencyProvider currencyProvider,
                            IHttpClientFactory clientFactory,
                            IServiceScopeFactory serviceScopeFactory
                          )
        {
            _tenantManager = tenantManager;
            _tenantUtil = tenantUtil;
            _global = global;
            _settingsManager = settingsManager;
            _serviceProvider = serviceProvider;
            _daoFactory = daoFactory;
            _docbuilderReportsUtilityHelper = docbuilderReportsUtilityHelper;
            _securityContext = securityContext;
            _httpContext = httpContextAccessor;
            _currencyProvider = currencyProvider;
            _clientFactory = clientFactory;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private string GetFileName(ReportType reportType)
        {
            string reportName;

            switch (reportType)
            {
                case ReportType.SalesByManagers:
                    reportName = CRMReportResource.SalesByManagersReport;
                    break;
                case ReportType.SalesForecast:
                    reportName = CRMReportResource.SalesForecastReport;
                    break;
                case ReportType.SalesFunnel:
                    reportName = CRMReportResource.SalesFunnelReport;
                    break;
                case ReportType.WorkloadByContacts:
                    reportName = CRMReportResource.WorkloadByContactsReport;
                    break;
                case ReportType.WorkloadByTasks:
                    reportName = CRMReportResource.WorkloadByTasksReport;
                    break;
                case ReportType.WorkloadByDeals:
                    reportName = CRMReportResource.WorkloadByDealsReport;
                    break;
                case ReportType.WorkloadByInvoices:
                    reportName = CRMReportResource.WorkloadByInvoicesReport;
                    break;
                case ReportType.SummaryForThePeriod:
                    reportName = CRMReportResource.SummaryForThePeriodReport;
                    break;
                case ReportType.SummaryAtThisMoment:
                    reportName = CRMReportResource.SummaryAtThisMomentReport;
                    break;
                default:
                    reportName = string.Empty;
                    break;
            }

            return $"{reportName} ({_tenantUtil.DateTimeNow().ToShortDateString()} {_tenantUtil.DateTimeNow().ToShortTimeString()}).xlsx";
        }

        public bool CheckReportData(ReportType reportType, ReportTimePeriod timePeriod, Guid[] managers)
        {
            var reportDao = _daoFactory.GetReportDao();

            throw new NotImplementedException();

            //switch (reportType)
            //{
            //    case ReportType.SalesByManagers:
            //        return reportDao.CheckSalesByManagersReportData(timePeriod, managers);
            //    case ReportType.SalesForecast:
            //        return reportDao.CheckSalesForecastReportData(timePeriod, managers);
            //    case ReportType.SalesFunnel:
            //        return reportDao.CheckSalesFunnelReportData(timePeriod, managers);
            //    case ReportType.WorkloadByContacts:
            //        return reportDao.CheckWorkloadByContactsReportData(timePeriod, managers);
            //    case ReportType.WorkloadByTasks:
            //        return reportDao.CheckWorkloadByTasksReportData(timePeriod, managers);
            //    case ReportType.WorkloadByDeals:
            //        return reportDao.CheckWorkloadByDealsReportData(timePeriod, managers);
            //    case ReportType.WorkloadByInvoices:
            //        return reportDao.CheckWorkloadByInvoicesReportData(timePeriod, managers);
            //    case ReportType.SummaryForThePeriod:
            //        return reportDao.CheckSummaryForThePeriodReportData(timePeriod, managers);
            //    case ReportType.SummaryAtThisMoment:
            //        return reportDao.CheckSummaryAtThisMomentReportData(timePeriod, managers);
            //    default:
            //        return false;
            //}
        }

        public List<string> GetMissingRates(ReportType reportType)
        {
            var reportDao = _daoFactory.GetReportDao();

            if (reportType == ReportType.WorkloadByTasks || reportType == ReportType.WorkloadByInvoices ||
                reportType == ReportType.WorkloadByContacts) return null;

            var crmSettings = _settingsManager.Load<CrmSettings>();
            var defaultCurrency = _currencyProvider.Get(crmSettings.DefaultCurrency);

            return reportDao.GetMissingRates(defaultCurrency.Abbreviation);
        }

        private object GetReportData(ReportType reportType, ReportTimePeriod timePeriod, Guid[] managers)
        {
            var crmSettings = _settingsManager.Load<CrmSettings>();

            var reportDao = _daoFactory.GetReportDao();
            var defaultCurrency = _currencyProvider.Get(crmSettings.DefaultCurrency).Abbreviation;

            switch (reportType)
            {
                case ReportType.SalesByManagers:
                    return reportDao.GetSalesByManagersReportData(timePeriod, managers, defaultCurrency);
                case ReportType.SalesForecast:
                    return reportDao.GetSalesForecastReportData(timePeriod, managers, defaultCurrency);
                case ReportType.SalesFunnel:
                    return reportDao.GetSalesFunnelReportData(timePeriod, managers, defaultCurrency);
                case ReportType.WorkloadByContacts:
                    return reportDao.GetWorkloadByContactsReportData(timePeriod, managers);
                case ReportType.WorkloadByTasks:
                    return reportDao.GetWorkloadByTasksReportData(timePeriod, managers);
                case ReportType.WorkloadByDeals:
                    return reportDao.GetWorkloadByDealsReportData(timePeriod, managers, defaultCurrency);
                case ReportType.WorkloadByInvoices:
                    return reportDao.GetWorkloadByInvoicesReportData(timePeriod, managers);
                case ReportType.SummaryForThePeriod:
                    return reportDao.GetSummaryForThePeriodReportData(timePeriod, managers, defaultCurrency);
                case ReportType.SummaryAtThisMoment:
                    return reportDao.GetSummaryAtThisMomentReportData(timePeriod, managers, defaultCurrency);
                default:
                    return null;
            }
        }

        private string GetReportScript(object data, ReportType type, string fileName)
        {
            var script =
                FileHelper.ReadTextFromEmbeddedResource(string.Format("ASC.Web.CRM.ReportTemplates.{0}.docbuilder", type));

            if (string.IsNullOrEmpty(script))
                throw new Exception(CRMReportResource.BuildErrorEmptyDocbuilderTemplate);

            return script.Replace("${outputFilePath}", fileName)
                         .Replace("${reportData}", JsonSerializer.Serialize(data));
        }

        private async Task SaveReportFile(ReportState state, string url)
        {
            var httpClient = _clientFactory.CreateClient();
            var responseData = await httpClient.GetByteArrayAsync(url);

            using (var stream = new System.IO.MemoryStream(responseData))
            {
                var document = _serviceProvider.GetService<File<int>>();

                document.Title = state.FileName;
                document.ParentId = await _daoFactory.GetFileDao().GetRootAsync();
                document.ContentLength = stream.Length;

                var file = await _daoFactory.GetFileDao().SaveFileAsync(document, stream);

                _daoFactory.GetReportDao().SaveFile((int)file.Id, state.ReportType);

                state.FileId = (int)file.Id;
            }

        }

        public ReportState RunGenareteReport(ReportType reportType, ReportTimePeriod timePeriod, Guid[] managers)
        {
            var reportData = GetReportData(reportType, timePeriod, managers);

            if (reportData == null)
                throw new Exception(CRMReportResource.ErrorNullReportData);

            var tmpFileName = DocbuilderReportsUtility.TmpFileName;

            var script = GetReportScript(reportData, reportType, tmpFileName);

            if (string.IsNullOrEmpty(script))
                throw new Exception(CRMReportResource.ErrorNullReportScript);

            var reportStateData = new ReportStateData(
                GetFileName(reportType),
                tmpFileName,
                script,
                (int)reportType,
                ReportOrigin.CRM,
                SaveReportFile,
                null,
                _tenantManager.GetCurrentTenant().Id,
                _securityContext.CurrentAccount.ID);

            var state = new ReportState(_serviceScopeFactory, reportStateData, _httpContext);

            _docbuilderReportsUtilityHelper.Enqueue(state);

            return state;
        }
    }
}