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
using System.Linq;
using System.Threading.Tasks;

using ASC.Api.CRM;
using ASC.Api.Documents;
using ASC.Common.Web;
using ASC.Core.Common.Settings;
using ASC.CRM.ApiModels;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Web.Api.Routing;
using ASC.Web.CRM.Classes;
using ASC.Web.Files.Services.DocumentService;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    public class ReportsController : BaseApiController
    {
        private readonly DocbuilderReportsUtilityHelper _docbuilderReportsUtilityHelper;
        private readonly FileWrapperHelper _fileWrapperHelper;
        private readonly ReportHelper _reportHelper;
        private readonly Global _global;
        private readonly SettingsManager _settingsManager;

        public ReportsController(CrmSecurity crmSecurity,
                     DaoFactory daoFactory,
                     SettingsManager settingsManager,
                     Global global,
                     ReportHelper reportHelper,
                     FileWrapperHelper fileWrapperHelper,
                     DocbuilderReportsUtilityHelper docbuilderReportsUtilityHelper,
                     IMapper mapper
                     )
            : base(daoFactory, crmSecurity, mapper)
        {
            _settingsManager = settingsManager;
            _global = global;
            _reportHelper = reportHelper;
            _fileWrapperHelper = fileWrapperHelper;
            _docbuilderReportsUtilityHelper = docbuilderReportsUtilityHelper;
        }


        /// <summary>Returns a list of all user report files</summary>
        /// <short>Get report files</short>
        /// <category>Reports</category>
        /// <returns>Report files</returns>
        /// <exception cref="SecurityException">if user can't create reports</exception>
        [Read(@"report/files")]
        public Task<IEnumerable<FileWrapper<int>>> GetFilesAsync()
        {
            if (!_global.CanCreateReports)
                throw _crmSecurity.CreateSecurityException();

            return InternalGetFilesAsync();
            }

        private async Task<IEnumerable<FileWrapper<int>>> InternalGetFilesAsync()
        {
            var reportDao = _daoFactory.GetReportDao();

            var files = reportDao.GetFiles();

            if (files.Count == 0)
            {
                var settings = _settingsManager.Load<CrmReportSampleSettings>();

                if (settings.NeedToGenerate)
                {
                    files = await reportDao.SaveSampleReportFilesAsync();

                    settings.NeedToGenerate = false;

                    _settingsManager.Save<CrmReportSampleSettings>(settings);
                }
            }

            List<FileWrapper<int>> result = new List<FileWrapper<int>>(files.Count);
            foreach (var file in files)
            {
                result.Add(await _fileWrapperHelper.GetAsync<int>(file));
            }

            return result.OrderByDescending(file => file.Id);
        }

        /// <summary>Delete the report file with the ID specified in the request</summary>
        /// <param name="fileid">File ID</param>
        /// <short>Delete report file</short>
        /// <category>Reports</category>
        /// <exception cref="SecurityException">if user can't create reports</exception>
        /// <exception cref="ArgumentException">if fileid les than 0</exception>
        /// <exception cref="ItemNotFoundException">if file not found</exception>
        [Delete(@"report/file/{fileid:int}")]
        public void DeleteFile(int fileid)
        {
            if (!_global.CanCreateReports)
                throw _crmSecurity.CreateSecurityException();

            if (fileid < 0) throw new ArgumentException();

            var file = _daoFactory.GetReportDao().GetFile(fileid);

            if (file == null) throw new ItemNotFoundException("File not found");

            _daoFactory.GetReportDao().DeleteFile(fileid);
        }

        [Delete(@"report/fileAsync/{fileid:int}")]
        public Task DeleteFileAsync(int fileid)
        {
            if (!_global.CanCreateReports)
                throw _crmSecurity.CreateSecurityException();

            if (fileid < 0) throw new ArgumentException();

            return InternalDeleteFileAsync(fileid);
        }

        private async Task InternalDeleteFileAsync(int fileid)
        {
            var file = await _daoFactory.GetReportDao().GetFileAsync(fileid);

            if (file == null) throw new ItemNotFoundException("File not found");

            _daoFactory.GetReportDao().DeleteFile(fileid);
        }

        /// <summary>Get the state of the report generation task</summary>
        /// <short>Get report generation state</short>
        /// <category>Reports</category>
        /// <returns>Report state</returns>
        /// <exception cref="SecurityException">if user can't create reports</exception>
        [Read(@"report/status")]
        public ReportState GetStatus()
        {
            if (!_global.CanCreateReports)
                throw _crmSecurity.CreateSecurityException();

            return _docbuilderReportsUtilityHelper.Status(ReportOrigin.CRM);

        }

        /// <summary>Terminate the report generation task</summary>
        /// <short>Terminate report generation</short>
        /// <category>Reports</category>
        /// <exception cref="SecurityException">if user can't create reports</exception>
        [Read(@"report/terminate")]
        public void Terminate()
        {
            if (!_global.CanCreateReports)
                throw _crmSecurity.CreateSecurityException();

            _docbuilderReportsUtilityHelper.Terminate(ReportOrigin.CRM);
        }

        /// <summary>Check data availability for a report</summary>
        /// <param name="type">Report type</param>
        /// <param name="timePeriod">Time period</param>
        /// <param name="managers">Managers</param>
        /// <short>Check report data</short>
        /// <category>Reports</category>
        /// <returns>Object</returns>
        /// <exception cref="SecurityException">if user can't create reports</exception>
        [Create(@"report/check")]
        public object CheckReportData([FromBody] ReportDataRequestDto inDto)
        {
            var type = inDto.Type;
            var timePeriod = inDto.TimePeriod;
            var managers = inDto.Managers;

            if (!_global.CanCreateReports)
                throw _crmSecurity.CreateSecurityException();

            return new
            {
                hasData = _reportHelper.CheckReportData(type, timePeriod, managers),
                missingRates = _reportHelper.GetMissingRates(type)
            };
        }

        /// <summary>Run the report generation task</summary>
        /// <param name="type">Report type</param>
        /// <param name="timePeriod">Time period</param>
        /// <param name="managers">Managers</param>
        /// <short>Generate report</short>
        /// <category>Reports</category>
        /// <returns>Report state</returns>
        /// <exception cref="SecurityException">if user can't create reports</exception>
        [Create(@"report/generate")]
        public ReportState GenerateReport([FromBody] ReportDataRequestDto inDto)
        {
            var type = inDto.Type;
            var timePeriod = inDto.TimePeriod;
            var managers = inDto.Managers;

            if (!_global.CanCreateReports)
                throw _crmSecurity.CreateSecurityException();

            return _reportHelper.RunGenareteReport(type, timePeriod, managers);
        }
    }
}