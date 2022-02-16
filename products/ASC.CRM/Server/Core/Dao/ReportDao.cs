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


#region Import

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Classes;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.VoipService;
using ASC.Web.Core.Users;
using ASC.Web.CRM.Classes;
using ASC.Web.Files.Api;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


#endregion

namespace ASC.CRM.Core.Dao
{
    [Scope]
    public class ReportDao : AbstractDao
    {
        const string TimeFormat = "[h]:mm:ss;@";
        const string ShortDateFormat = "M/d/yyyy";

        private IServiceProvider _serviceProvider;
        private TenantManager _tenantManager;
        private UserManager _userManager;
        private Global _global;
        private FilesIntegration _filesIntegration;
        private CurrencyInfo _defaultCurrency;
        private TenantUtil _tenantUtil;
        private DaoFactory _daoFactory;
        private DisplayUserSettingsHelper _displayUserSettings;

        #region Constructor

        public ReportDao(DbContextManager<CrmDbContext> dbContextManager,
                       TenantManager tenantManager,
                       SecurityContext securityContext,
                       FilesIntegration filesIntegration,
                       IOptionsMonitor<ILog> logger,
                       ICache ascCache,
                       TenantUtil tenantUtil,
                       SettingsManager settingsManager,
                       Global global,
                       UserManager userManager,
                       IServiceProvider serviceProvider,
                       CurrencyProvider currencyProvider,
                       DaoFactory daoFactory,
                       DisplayUserSettingsHelper displayUserSettingsHelper,
                       IMapper mapper) :
            base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache,
                 mapper)
        {
            _tenantUtil = tenantUtil;

            _filesIntegration = filesIntegration;
            _global = global;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _serviceProvider = serviceProvider;
            _daoFactory = daoFactory;

            var crmSettings = settingsManager.Load<CrmSettings>();

            _defaultCurrency = currencyProvider.Get(crmSettings.DefaultCurrency);

            _displayUserSettings = displayUserSettingsHelper;

        }


        #endregion



        #region Common Methods

        private void GetTimePeriod(ReportTimePeriod timePeriod, out DateTime fromDate, out DateTime toDate)
        {
            var now = _tenantUtil.DateTimeNow().Date;

            var diff = (int)now.DayOfWeek - (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            var quarter = (now.Month + 2) / 3;

            var year = now.Year;

            switch (timePeriod)
            {
                case ReportTimePeriod.Today:
                    fromDate = now;
                    toDate = now.AddDays(1).AddSeconds(-1);
                    break;
                case ReportTimePeriod.Yesterday:
                    fromDate = now.AddDays(-1);
                    toDate = now.AddSeconds(-1);
                    break;
                case ReportTimePeriod.Tomorrow:
                    fromDate = now.AddDays(1);
                    toDate = now.AddDays(2).AddSeconds(-1);
                    break;
                case ReportTimePeriod.CurrentWeek:
                    fromDate = now.AddDays(-1 * diff);
                    toDate = now.AddDays(-1 * diff + 7).AddSeconds(-1);
                    break;
                case ReportTimePeriod.PreviousWeek:
                    fromDate = now.AddDays(-1 * diff - 7);
                    toDate = now.AddDays(-1 * diff).AddSeconds(-1);
                    break;
                case ReportTimePeriod.NextWeek:
                    fromDate = now.AddDays(-1 * diff + 7);
                    toDate = now.AddDays(-1 * diff + 14).AddSeconds(-1);
                    break;
                case ReportTimePeriod.CurrentMonth:
                    fromDate = new DateTime(now.Year, now.Month, 1);
                    toDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddSeconds(-1);
                    break;
                case ReportTimePeriod.PreviousMonth:
                    toDate = new DateTime(now.Year, now.Month, 1).AddSeconds(-1);
                    fromDate = new DateTime(toDate.Year, toDate.Month, 1);
                    break;
                case ReportTimePeriod.NextMonth:
                    fromDate = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                    toDate = new DateTime(now.Year, now.Month, 1).AddMonths(2).AddSeconds(-1);
                    break;
                case ReportTimePeriod.CurrentQuarter:
                    fromDate = new DateTime(now.Year, quarter * 3 - 2, 1);
                    toDate = new DateTime(now.Year, fromDate.Month, 1).AddMonths(3).AddSeconds(-1);
                    break;
                case ReportTimePeriod.PreviousQuarter:
                    quarter--;
                    if (quarter == 0)
                    {
                        year--;
                        quarter = 4;
                    }
                    fromDate = new DateTime(year, quarter * 3 - 2, 1);
                    toDate = new DateTime(year, fromDate.Month, 1).AddMonths(3).AddSeconds(-1);
                    break;
                case ReportTimePeriod.NextQuarter:
                    quarter++;
                    if (quarter == 5)
                    {
                        year++;
                        quarter = 1;
                    }
                    fromDate = new DateTime(year, quarter * 3 - 2, 1);
                    toDate = new DateTime(year, fromDate.Month, 1).AddMonths(3).AddSeconds(-1);
                    break;
                case ReportTimePeriod.CurrentYear:
                    fromDate = new DateTime(now.Year, 1, 1);
                    toDate = new DateTime(now.Year, 1, 1).AddYears(1).AddSeconds(-1);
                    break;
                case ReportTimePeriod.PreviousYear:
                    toDate = new DateTime(now.Year, 1, 1).AddSeconds(-1);
                    fromDate = new DateTime(toDate.Year, 1, 1);
                    break;
                case ReportTimePeriod.NextYear:
                    fromDate = new DateTime(now.Year, 1, 1).AddYears(1);
                    toDate = new DateTime(now.Year, 1, 1).AddYears(2).AddSeconds(-1);
                    break;
                case ReportTimePeriod.DuringAllTime:
                    fromDate = DateTime.MinValue;
                    toDate = DateTime.MaxValue;
                    break;
                default:
                    fromDate = DateTime.MinValue;
                    toDate = DateTime.MinValue;
                    break;
            }
        }

        private string GetTimePeriodText(ReportTimePeriod timePeriod)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            switch (timePeriod)
            {
                case ReportTimePeriod.Today:
                case ReportTimePeriod.Yesterday:
                case ReportTimePeriod.Tomorrow:
                    return fromDate.ToShortDateString();
                case ReportTimePeriod.CurrentWeek:
                case ReportTimePeriod.PreviousWeek:
                case ReportTimePeriod.NextWeek:
                    return $"{fromDate.ToShortDateString()}-{toDate.ToShortDateString()}";
                case ReportTimePeriod.CurrentMonth:
                case ReportTimePeriod.PreviousMonth:
                case ReportTimePeriod.NextMonth:
                    return fromDate.ToString("Y");
                case ReportTimePeriod.CurrentQuarter:
                case ReportTimePeriod.PreviousQuarter:
                case ReportTimePeriod.NextQuarter:
                    return $"{fromDate.ToString("Y")}-{toDate.ToString("Y")}";
                case ReportTimePeriod.CurrentYear:
                case ReportTimePeriod.PreviousYear:
                case ReportTimePeriod.NextYear:
                    return fromDate.Year.ToString(CultureInfo.InvariantCulture);
                case ReportTimePeriod.DuringAllTime:
                    return CRMReportResource.DuringAllTime;
                default:
                    return string.Empty;
            }
        }

        public List<string> GetMissingRates(string defaultCurrency)
        {
            var existingRatesQuery = CrmDbContext.CurrencyRate
                                    .AsQueryable()
                                    .Where(x => x.ToCurrency == defaultCurrency)
                                    .Select(x => x.FromCurrency).Distinct().ToList();

            var missingRatesQuery = Query(CrmDbContext.Deals)
                                    .Where(x => x.BidCurrency != defaultCurrency && !existingRatesQuery.Contains(x.BidCurrency))
                                    .Select(x => x.BidCurrency)
                                    .Distinct();

            return missingRatesQuery.ToList();
        }

        #endregion

        #region Report Files

        public Task<List<Files.Core.File<int>>> SaveSampleReportFilesAsync()
        {
            var storeTemplate = _global.GetStoreTemplate();

            if (storeTemplate == null) return System.Threading.Tasks.Task.FromResult(new List<Files.Core.File<int>>());

            return InternalSaveSampleReportFilesAsync(storeTemplate);
        }

        private async Task<List<Files.Core.File<int>>> InternalSaveSampleReportFilesAsync(Data.Storage.IDataStore storeTemplate)
        {
            var result = new List<Files.Core.File<int>>();

            var culture = _userManager.GetUsers(_securityContext.CurrentAccount.ID).GetCulture() ??
                          _tenantManager.GetCurrentTenant().GetCulture();

            var path = culture + "/";

            if (!await storeTemplate.IsDirectoryAsync(path))
            {
                path = "default/";
                if (!await storeTemplate.IsDirectoryAsync(path)) return result;
            }

            await foreach (var filePath in storeTemplate.ListFilesRelativeAsync("", path, "*", false).Select(x => path + x))
            {
                using (var stream = await storeTemplate.GetReadStreamAsync("", filePath))
                {

                    var document = _serviceProvider.GetService<Files.Core.File<int>>();

                    document.Title = Path.GetFileName(filePath);
                    document.FolderID = await _daoFactory.GetFileDao().GetRootAsync();
                    document.ContentLength = stream.Length;


                    var file = await _daoFactory.GetFileDao().SaveFileAsync(document, stream);

                    SaveFile(file.ID, -1);

                    result.Add(file);
                }
            }

            return result;
        }

        public List<Files.Core.File<int>> GetFiles()
        {
            return GetFiles(_securityContext.CurrentAccount.ID);
        }

        public List<Files.Core.File<int>> GetFiles(Guid userId)
        {
            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            var fileIds = Query(CrmDbContext.ReportFile).Where(x => x.CreateBy == userId).Select(x => x.FileId).ToArray();

            return fileIds.Length > 0 ? filedao.GetFilesAsync(fileIds).ToListAsync().Result : new List<Files.Core.File<int>>();

        }

        public List<int> GetFileIds(Guid userId)
        {
            return Query(CrmDbContext.ReportFile)
                        .Where(x => x.CreateBy == userId).Select(x => x.FileId).ToList();
        }

        public Files.Core.File<int> GetFile(int fileid)
        {
            return GetFile(fileid, _securityContext.CurrentAccount.ID);
        }

        public async Task<Files.Core.File<int>> GetFileAsync(int fileid)
        {
            return await GetFileAsync(fileid, _securityContext.CurrentAccount.ID);
        }

        public Files.Core.File<int> GetFile(int fileid, Guid userId)
        {
            var exist = Query(CrmDbContext.ReportFile)
                        .Any(x => x.CreateBy == userId && x.FileId == fileid);

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            return exist ? filedao.GetFileAsync(fileid).Result : null;

        }

        public async Task<Files.Core.File<int>> GetFileAsync(int fileid, Guid userId)
        {
            var exist = await Query(CrmDbContext.ReportFile)
                        .AnyAsync(x => x.CreateBy == userId && x.FileId == fileid);

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            return exist ? await filedao.GetFileAsync(fileid) : null;

        }

        public void DeleteFile(int fileid)
        {
            var itemToDelete = Query(CrmDbContext.ReportFile).Where(x => x.FileId == fileid && x.CreateBy == _securityContext.CurrentAccount.ID);

            CrmDbContext.Remove(itemToDelete);
            CrmDbContext.SaveChanges();

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            filedao.DeleteFileAsync(fileid).Wait();
        }

        public void DeleteFiles(Guid userId)
        {
            var fileIds = GetFileIds(userId);

            var itemToDelete = Query(CrmDbContext.ReportFile).Where(x => x.CreateBy == _securityContext.CurrentAccount.ID);

            CrmDbContext.Remove(itemToDelete);
            CrmDbContext.SaveChanges();

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            foreach (var fileId in fileIds)
            {
                filedao.DeleteFileAsync(fileId).Wait();
            }

        }

        public void SaveFile(int fileId, int reportType)
        {

            var itemToInsert = new DbReportFile
            {
                FileId = fileId,
                ReportType = (ReportType)reportType,
                CreateOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()),
                CreateBy = _securityContext.CurrentAccount.ID,
                TenantId = TenantID
            };

            CrmDbContext.Add(itemToInsert);
            CrmDbContext.SaveChanges();
        }

        #endregion


        #region SalesByManagersReport

        public bool CheckSalesByManagersReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);


            return Query(CrmDbContext.Deals).Join(Query(CrmDbContext.DealMilestones),
                                            x => x.DealMilestoneId,
                                            y => y.Id,
                                            (x, y) => new { x, y })
                                          .Where(x => x.y.Status == DealMilestoneStatus.ClosedAndWon)
                                          .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                                          .Where(x => x.x.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                                          .Any();
        }

        public object GetSalesByManagersReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            var reportData = BuildSalesByManagersReport(timePeriod, managers, defaultCurrency);

            return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
        }

        private List<SalesByManager> BuildSalesByManagersReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            Func<DateTime?, DateTime> exp;

            switch (timePeriod)
            {
                case ReportTimePeriod.Today:
                case ReportTimePeriod.Yesterday:
                    exp = x => x ?? x.Value.Date.AddHours(x.Value.Hour);

                    break;
                case ReportTimePeriod.CurrentWeek:
                case ReportTimePeriod.PreviousWeek:
                case ReportTimePeriod.CurrentMonth:
                case ReportTimePeriod.PreviousMonth:
                    exp = x => x ?? x.Value.Date;

                    break;
                case ReportTimePeriod.CurrentQuarter:
                case ReportTimePeriod.PreviousQuarter:
                case ReportTimePeriod.CurrentYear:
                case ReportTimePeriod.PreviousYear:
                    exp = x => x ?? x.Value.Date.AddDays(-(x.Value.Day - 1));

                    break;
                default:
                    return null;
            }

            var result = Query(CrmDbContext.Deals)
                               .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                       x => x.BidCurrency,
                                       y => y.FromCurrency,
                                       (x, y) => new { x, y })
                               .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                               .GroupJoin(Query(CrmDbContext.DealMilestones),
                                     x => x.Deal.DealMilestoneId,
                                     y => y.Id,
                                     (x, y) => new { x, y })
                              .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                             .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                             .Where(x => x.Deal.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                             .Where(x => x.DealMilestone.Status == DealMilestoneStatus.ClosedAndWon)
                             .GroupBy(x => new { x.Deal.ResponsibleId, x.Deal.ActualCloseDate, x.DealMilestone.Status })
                             .Select(x => new
                             {
                                 responsible_id = x.Key.ResponsibleId,
                                 status = x.Key.Status,
                                 count = x.Count(),
                                 deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                     x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate)),
                                 close_date = exp.Invoke(x.Key.ActualCloseDate)
                             }).ToList()
                             .ConvertAll(x => new SalesByManager
                             {
                                 UserId = x.responsible_id,
                                 UserName = _displayUserSettings.GetFullUserName(x.responsible_id),
                                 Value = x.deals_value,
                                 Date = x.close_date == DateTime.MinValue ? DateTime.MinValue : _tenantUtil.DateTimeFromUtc(x.close_date)
                             });

            return result;
        }

        private object GenerateReportData(ReportTimePeriod timePeriod, List<SalesByManager> data)
        {
            switch (timePeriod)
            {
                case ReportTimePeriod.Today:
                case ReportTimePeriod.Yesterday:
                    return GenerateReportDataByHours(timePeriod, data);
                case ReportTimePeriod.CurrentWeek:
                case ReportTimePeriod.PreviousWeek:
                case ReportTimePeriod.CurrentMonth:
                case ReportTimePeriod.PreviousMonth:
                    return GenerateReportDataByDays(timePeriod, data);
                case ReportTimePeriod.CurrentQuarter:
                case ReportTimePeriod.PreviousQuarter:
                case ReportTimePeriod.CurrentYear:
                case ReportTimePeriod.PreviousYear:
                    return GenerateReportByMonths(timePeriod, data);
                default:
                    return null;
            }
        }

        private object GenerateReportDataByHours(ReportTimePeriod timePeriod, List<SalesByManager> data)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var res = new Dictionary<Guid, Dictionary<DateTime, decimal>>();

            var users = data.Select(x => x.UserId).Distinct().ToList();

            foreach (var userId in users)
            {
                var date = fromDate;

                while (date < toDate)
                {
                    if (res.ContainsKey(userId))
                    {
                        res[userId].Add(date, 0);
                    }
                    else
                    {
                        res.Add(userId, new Dictionary<DateTime, decimal> { { date, 0 } });
                    }

                    date = date.AddHours(1);
                }
            }

            foreach (var item in data)
            {
                var itemDate = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day, item.Date.Hour, 0, 0);

                if (itemDate < res[item.UserId].First().Key)
                    itemDate = res[item.UserId].First().Key;

                if (itemDate > res[item.UserId].Last().Key)
                    itemDate = res[item.UserId].Last().Key;

                res[item.UserId][itemDate] += item.Value;
            }

            var body = new List<List<object>>();

            foreach (var resItem in res)
            {
                var bodyItem = new List<object>
                    {
                        data.First(x => x.UserId == resItem.Key).UserName
                    };

                bodyItem.AddRange(resItem.Value.Select(x => new { format = "0.00", value = x.Value.ToString(CultureInfo.InvariantCulture) }));

                body.Add(bodyItem);
            }

            var head = new List<object>();

            foreach (var key in res.First().Value.Keys)
            {
                head.Add(new { format = "H:mm", value = key.ToShortTimeString() });
            }

            return new
            {
                resource = new
                {
                    manager = CRMReportResource.Manager,
                    summary = CRMReportResource.Sum,
                    total = CRMReportResource.Total,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),
                    sheetName = CRMReportResource.SalesByManagersReport,
                    header = CRMReportResource.SalesByManagersReport,
                    header1 = CRMReportResource.SalesByHour + ", " + _defaultCurrency.Symbol,
                    header2 = CRMReportResource.TotalSalesByManagers + ", " + _defaultCurrency.Symbol,
                    chartName1 = CRMReportResource.SalesByHour + ", " + _defaultCurrency.Symbol,
                    chartName2 = CRMReportResource.TotalSalesByManagers + ", " + _defaultCurrency.Symbol
                },
                thead = head,
                tbody = body
            };
        }

        private object GenerateReportDataByDays(ReportTimePeriod timePeriod, List<SalesByManager> data)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var res = new Dictionary<Guid, Dictionary<DateTime, decimal>>();

            var users = data.Select(x => x.UserId).Distinct().ToList();

            foreach (var userId in users)
            {
                var date = fromDate;

                while (date < toDate)
                {
                    if (res.ContainsKey(userId))
                    {
                        res[userId].Add(date, 0);
                    }
                    else
                    {
                        res.Add(userId, new Dictionary<DateTime, decimal> { { date, 0 } });
                    }

                    date = date.AddDays(1);
                }
            }

            foreach (var item in data)
            {
                var itemDate = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day);

                if (itemDate < res[item.UserId].First().Key)
                    itemDate = res[item.UserId].First().Key;

                if (itemDate > res[item.UserId].Last().Key)
                    itemDate = res[item.UserId].Last().Key;

                res[item.UserId][itemDate] += item.Value;
            }

            var body = new List<List<object>>();

            foreach (var resItem in res)
            {
                var bodyItem = new List<object>
                    {
                        data.First(x => x.UserId == resItem.Key).UserName
                    };

                bodyItem.AddRange(resItem.Value.Select(x => new { format = "0.00", value = x.Value.ToString(CultureInfo.InvariantCulture) }));

                body.Add(bodyItem);
            }

            var head = new List<object>();
            var separator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator.ToCharArray();
            var pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.Replace("yyyy", string.Empty).Trim(separator);

            foreach (var key in res.First().Value.Keys)
            {
                head.Add(new { format = pattern, value = key.ToString(ShortDateFormat, CultureInfo.InvariantCulture) });
            }

            return new
            {
                resource = new
                {
                    manager = CRMReportResource.Manager,
                    summary = CRMReportResource.Sum,
                    total = CRMReportResource.Total,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),
                    sheetName = CRMReportResource.SalesByManagersReport,
                    header = CRMReportResource.SalesByManagersReport,
                    header1 = CRMReportResource.SalesByDay + ", " + _defaultCurrency.Symbol,
                    header2 = CRMReportResource.TotalSalesByManagers + ", " + _defaultCurrency.Symbol,
                    chartName1 = CRMReportResource.SalesByDay + ", " + _defaultCurrency.Symbol,
                    chartName2 = CRMReportResource.TotalSalesByManagers + ", " + _defaultCurrency.Symbol
                },
                thead = head,
                tbody = body
            };
        }

        private object GenerateReportByMonths(ReportTimePeriod timePeriod, List<SalesByManager> data)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var res = new Dictionary<Guid, Dictionary<DateTime, decimal>>();

            var users = data.Select(x => x.UserId).Distinct().ToList();

            foreach (var userId in users)
            {
                var date = fromDate;

                while (date < toDate)
                {
                    if (res.ContainsKey(userId))
                    {
                        res[userId].Add(date, 0);
                    }
                    else
                    {
                        res.Add(userId, new Dictionary<DateTime, decimal> { { date, 0 } });
                    }

                    date = date.AddMonths(1);
                }
            }

            foreach (var item in data)
            {
                var itemDate = new DateTime(item.Date.Year, item.Date.Month, 1);

                if (itemDate < res[item.UserId].First().Key)
                    itemDate = res[item.UserId].First().Key;

                if (itemDate > res[item.UserId].Last().Key)
                    itemDate = res[item.UserId].Last().Key;

                res[item.UserId][itemDate] += item.Value;
            }

            var body = new List<List<object>>();

            foreach (var resItem in res)
            {
                var bodyItem = new List<object>
                    {
                        data.First(x => x.UserId == resItem.Key).UserName
                    };

                bodyItem.AddRange(resItem.Value.Select(x => new { format = "0.00", value = x.Value.ToString(CultureInfo.InvariantCulture) }));

                body.Add(bodyItem);
            }

            var head = new List<object>();

            foreach (var key in res.First().Value.Keys)
            {
                head.Add(new { format = "MMM-yy", value = key.ToString(ShortDateFormat, CultureInfo.InvariantCulture) });
            }

            return new
            {
                resource = new
                {
                    manager = CRMReportResource.Manager,
                    summary = CRMReportResource.Sum,
                    total = CRMReportResource.Total,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),
                    sheetName = CRMReportResource.SalesByManagersReport,
                    header = CRMReportResource.SalesByManagersReport,
                    header1 = CRMReportResource.SalesByMonth + ", " + _defaultCurrency.Symbol,
                    header2 = CRMReportResource.TotalSalesByManagers + ", " + _defaultCurrency.Symbol,
                    chartName1 = CRMReportResource.SalesByMonth + ", " + _defaultCurrency.Symbol,
                    chartName2 = CRMReportResource.TotalSalesByManagers + ", " + _defaultCurrency.Symbol
                },
                thead = head,
                tbody = body
            };
        }

        #endregion


        #region SalesForecastReport

        public bool CheckSalesForecastReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            return Query(CrmDbContext.Deals).Join(Query(CrmDbContext.DealMilestones),
                                     x => x.DealMilestoneId,
                                     y => y.Id,
                                     (x, y) => new { x, y })
                                   .Where(x => x.y.Status == DealMilestoneStatus.Open)
                                   .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                                   .Where(x => x.x.ExpectedCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                                   .Any();
        }

        public object GetSalesForecastReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            var reportData = BuildSalesForecastReport(timePeriod, managers, defaultCurrency);

            return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
        }

        private List<SalesForecast> BuildSalesForecastReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            Func<DateTime, DateTime> exp;

            switch (timePeriod)
            {
                case ReportTimePeriod.CurrentWeek:
                case ReportTimePeriod.NextWeek:
                case ReportTimePeriod.CurrentMonth:
                case ReportTimePeriod.NextMonth:
                    exp = x => x.Date;
                    break;
                case ReportTimePeriod.CurrentQuarter:
                case ReportTimePeriod.NextQuarter:
                case ReportTimePeriod.CurrentYear:
                case ReportTimePeriod.NextYear:
                    exp = x => x.Date.AddDays(-(x.Day - 1));
                    break;
                default:
                    return null;
            }

            var result = Query(CrmDbContext.Deals)
                               .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                       x => x.BidCurrency,
                                       y => y.FromCurrency,
                                       (x, y) => new { x, y })
                               .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                               .GroupJoin(Query(CrmDbContext.DealMilestones),
                                     x => x.Deal.DealMilestoneId,
                                     y => y.Id,
                                     (x, y) => new { x, y })
                              .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                             .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                             .Where(x => x.Deal.ExpectedCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ExpectedCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                             .Where(x => x.DealMilestone.Status == DealMilestoneStatus.ClosedAndWon)
                             .GroupBy(x => new { x.Deal.ExpectedCloseDate })
                             .Select(x => new
                             {
                                 deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                     x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate)),

                                 value_with_probability = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) * x.Deal.DealMilestoneProbability / 100.0m :
                                                                     x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) * x.Deal.DealMilestoneProbability / 100.0m),
                                 close_date = exp.Invoke(x.Key.ExpectedCloseDate)
                             }).ToList()
                             .ConvertAll(x => new SalesForecast
                             {
                                 Value = x.deals_value,
                                 ValueWithProbability = x.value_with_probability,
                                 Date = x.close_date == DateTime.MinValue ? DateTime.MinValue : _tenantUtil.DateTimeFromUtc(x.close_date)
                             });


            return result;
        }

        private SalesForecast ToSalesForecast(object[] row)
        {
            return new SalesForecast
            {
                Value = Convert.ToDecimal(row[0]),
                ValueWithProbability = Convert.ToDecimal(row[1]),
                Date = Convert.ToDateTime(row[2]) == DateTime.MinValue ? DateTime.MinValue : _tenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[2]))
            };
        }

        private object GenerateReportData(ReportTimePeriod timePeriod, List<SalesForecast> data)
        {
            switch (timePeriod)
            {
                case ReportTimePeriod.CurrentWeek:
                case ReportTimePeriod.NextWeek:
                case ReportTimePeriod.CurrentMonth:
                case ReportTimePeriod.NextMonth:
                    return GenerateReportDataByDays(timePeriod, data);
                case ReportTimePeriod.CurrentQuarter:
                case ReportTimePeriod.NextQuarter:
                case ReportTimePeriod.CurrentYear:
                case ReportTimePeriod.NextYear:
                    return GenerateReportByMonths(timePeriod, data);
                default:
                    return null;
            }
        }

        private object GenerateReportDataByDays(ReportTimePeriod timePeriod, List<SalesForecast> data)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var res = new Dictionary<DateTime, Tuple<decimal, decimal>>();

            var date = fromDate;

            while (date < toDate)
            {
                res.Add(date, new Tuple<decimal, decimal>(0, 0));
                date = date.AddDays(1);
            }

            foreach (var item in data)
            {
                var key = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day);

                if (key < res.First().Key)
                    key = res.First().Key;

                if (key > res.Last().Key)
                    key = res.Last().Key;

                res[key] = new Tuple<decimal, decimal>(res[key].Item1 + item.ValueWithProbability,
                                                       res[key].Item2 + item.Value);
            }

            var body = new List<List<object>>();
            var separator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator.ToCharArray();
            var pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.Replace("yyyy", string.Empty).Trim(separator);

            foreach (var resItem in res)
            {
                var bodyItem = new List<object>
                    {
                        new {format = pattern, value = resItem.Key.ToString(ShortDateFormat, CultureInfo.InvariantCulture)},
                        new {format = "0.00", value = resItem.Value.Item1.ToString(CultureInfo.InvariantCulture)},
                        new {format = "0.00", value = resItem.Value.Item2.ToString(CultureInfo.InvariantCulture)}
                    };

                body.Add(bodyItem);
            }

            var head = new List<object>
                {
                    CRMReportResource.Day,
                    CRMReportResource.WithRespectToProbability,
                    CRMReportResource.IfAllOpportunitiesWon
                };

            return new
            {
                resource = new
                {
                    total = CRMReportResource.Total,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),
                    sheetName = CRMReportResource.SalesForecastReport,
                    header = CRMReportResource.SalesForecastReport,
                    header1 = CRMReportResource.SalesForecastReport + ", " + _defaultCurrency.Symbol,
                    chartName = CRMReportResource.SalesForecastReport + ", " + _defaultCurrency.Symbol
                },
                thead = head,
                tbody = body
            };
        }

        private object GenerateReportByMonths(ReportTimePeriod timePeriod, List<SalesForecast> data)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var res = new Dictionary<DateTime, Tuple<decimal, decimal>>();

            var date = fromDate;

            while (date < toDate)
            {
                res.Add(date, new Tuple<decimal, decimal>(0, 0));
                date = date.AddMonths(1);
            }

            foreach (var item in data)
            {
                var key = new DateTime(item.Date.Year, item.Date.Month, 1);

                if (key < res.First().Key)
                    key = res.First().Key;

                if (key > res.Last().Key)
                    key = res.Last().Key;

                res[key] = new Tuple<decimal, decimal>(res[key].Item1 + item.ValueWithProbability,
                                                       res[key].Item2 + item.Value);
            }

            var body = new List<List<object>>();

            foreach (var resItem in res)
            {
                var bodyItem = new List<object>
                    {
                        new {format = "MMM-yy", value = resItem.Key.ToString(ShortDateFormat, CultureInfo.InvariantCulture)},
                        new {format = "0.00", value = resItem.Value.Item1.ToString(CultureInfo.InvariantCulture)},
                        new {format = "0.00", value = resItem.Value.Item2.ToString(CultureInfo.InvariantCulture)}
                    };

                body.Add(bodyItem);
            }

            var head = new List<object>
                {
                    CRMReportResource.Month,
                    CRMReportResource.WithRespectToProbability,
                    CRMReportResource.IfAllOpportunitiesWon
                };

            return new
            {
                resource = new
                {
                    total = CRMReportResource.Total,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),
                    sheetName = CRMReportResource.SalesForecastReport,
                    header = CRMReportResource.SalesForecastReport,
                    header1 = CRMReportResource.SalesForecastReport + ", " + _defaultCurrency.Symbol,
                    chartName = CRMReportResource.SalesForecastReport + ", " + _defaultCurrency.Symbol
                },
                thead = head,
                tbody = body
            };
        }

        #endregion


        #region SalesFunnelReport

        public bool CheckSalesFunnelReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            return Query(CrmDbContext.Deals)
                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                        .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                        .Any();
        }

        public object GetSalesFunnelReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            var reportData = BuildSalesFunnelReport(timePeriod, managers, defaultCurrency);

            return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
        }

        private List<SalesFunnel> BuildSalesFunnelReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var result = Query(CrmDbContext.DealMilestones)
                                            .GroupJoin(Query(CrmDbContext.Deals),
                                                   x => x.Id,
                                                   y => y.DealMilestoneId,
                                                   (x, y) => new { x, y })
                                            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = y, DealMilestone = x.x })
                                            .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                                    x => x.Deal.BidCurrency,
                                                    y => y.FromCurrency,
                                                    (x, y) => new { x, y })
                                            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, DealMilestone = x.x.DealMilestone, CurrencyRate = y })
                                          .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                          .Where(x => x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                          .Where(x => x.DealMilestone.Status == DealMilestoneStatus.Open)
                                          .GroupBy(x => new { x.DealMilestone.Id, x.DealMilestone.Title, x.DealMilestone.Status })
                                          .Select(x => new
                                          {
                                              status = x.Key.Status,
                                              title = x.Key.Title,
                                              deals_count = x.Count(),
                                              deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                  x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate)),
                                              deals_duration = x.Average(x => x.DealMilestone.Status == DealMilestoneStatus.ClosedAndWon ? (x.Deal.ActualCloseDate - x.Deal.CreateOn).Value.Days : 0)
                                          })
                                          .ToList()
                                          .ConvertAll(x =>
                                          new SalesFunnel
                                          {
                                              Status = x.status,
                                              Title = x.title,
                                              Count = x.deals_count,
                                              Value = x.deals_value,
                                              Duration = Convert.ToInt32(x.deals_duration)
                                          });

            return result;
        }

        private object GenerateReportData(ReportTimePeriod timePeriod, List<SalesFunnel> data)
        {
            var totalCount = data.Sum(x => x.Count);

            if (totalCount == 0) return null;

            var totalBudget = data.Sum(x => x.Value);

            var closed = data.Where(x => x.Status == DealMilestoneStatus.ClosedAndWon).ToList();

            var reportData = data.Select(item => new List<object>
                    {
                        item.Title,
                        item.Status,
                        item.Count,
                        item.Value
                    }).ToList();

            return new
            {
                resource = new
                {
                    header = CRMReportResource.SalesFunnelReport,
                    sheetName = CRMReportResource.SalesFunnelReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),

                    chartName = CRMReportResource.SalesFunnelByCount,
                    chartName1 = CRMReportResource.SalesFunnelByBudget + ", " + _defaultCurrency.Symbol,
                    chartName2 = CRMReportResource.DealsCount,
                    chartName3 = CRMReportResource.DealsBudget + ", " + _defaultCurrency.Symbol,

                    totalCountLabel = CRMReportResource.TotalDealsCount,
                    totalCountValue = totalCount,

                    totalBudgetLabel = CRMReportResource.TotalDealsBudget + ", " + _defaultCurrency.Symbol,
                    totalBudgetValue = totalBudget,

                    averageBidLabel = CRMReportResource.AverageDealsBudget + ", " + _defaultCurrency.Symbol,
                    averageBidValue = totalBudget / totalCount,

                    averageDurationLabel = CRMReportResource.AverageDealsDuration,
                    averageDurationValue = closed.Sum(x => x.Duration) / closed.Count,

                    header1 = CRMReportResource.ByCount,
                    header2 = CRMReportResource.ByBudget + ", " + _defaultCurrency.Symbol,

                    stage = CRMReportResource.Stage,
                    count = CRMReportResource.Count,
                    budget = CRMReportResource.Budget,
                    conversion = CRMReportResource.Conversion,

                    deals = CRMDealResource.Deals,
                    status0 = DealMilestoneStatus.Open.ToLocalizedString(),
                    status1 = DealMilestoneStatus.ClosedAndWon.ToLocalizedString(),
                    status2 = DealMilestoneStatus.ClosedAndLost.ToLocalizedString()
                },
                data = reportData
            };
        }

        #endregion


        #region WorkloadByContactsReport

        public bool CheckWorkloadByContactsReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            return Query(CrmDbContext.Contacts)
                       .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                       .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                       .Any();
        }

        public object GetWorkloadByContactsReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            var reportData = BuildWorkloadByContactsReport(timePeriod, managers);

            return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
        }

        private List<WorkloadByContacts> BuildWorkloadByContactsReport(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var result = Query(CrmDbContext.Contacts)
                                           .GroupJoin(Query(CrmDbContext.ListItem),
                                               x => x.ContactTypeId,
                                               y => y.Id,
                                               (x, y) => new { x, y })
                                           .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Contact = x.x, ListItem = y })
                                           .GroupJoin(Query(CrmDbContext.Deals),
                                               x => x.Contact.Id,
                                               y => y.ContactId,
                                               (x, y) => new { x, y })
                                           .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Contact = x.x.Contact, ListItem = x.x.ListItem, Deal = y })
                                           .Where(x => x.ListItem.ListType == ListType.ContactType)
                                           .Where(x => managers != null && managers.Any() ? managers.Contains(x.Contact.CreateBy) : true)
                                           .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Contact.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Contact.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                           .GroupBy(x => new { x.Contact.CreateBy, x.ListItem.Id, x.ListItem.Title })
                                           .Select(x => new
                                           {
                                               create_by = x.Key.CreateBy,
                                               id = x.Key.Id,
                                               title = x.Key.Title,
                                               total = x.Count(x => x.Contact.Id > 0),
                                               with_deals = x.Count(x => x.Deal.Id > 0)
                                           })
                                           .OrderBy(x => x.title)
                                           .ToList()
                                           .ConvertAll(x => new WorkloadByContacts
                                           {
                                               UserId = x.create_by,
                                               UserName = _displayUserSettings.GetFullUserName(x.create_by),
                                               CategoryId = x.id,
                                               CategoryName = x.title,
                                               Count = x.total,
                                               WithDeals = x.with_deals
                                           });

            //                    .OrderBy("i.sort_order, i.title", true);

            return result;
        }

        private object GenerateReportData(ReportTimePeriod timePeriod, List<WorkloadByContacts> reportData)
        {
            return new
            {
                resource = new
                {
                    header = CRMReportResource.WorkloadByContactsReport,
                    sheetName = CRMReportResource.WorkloadByContactsReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),

                    header1 = CRMReportResource.NewContacts,
                    header2 = CRMReportResource.NewContactsWithAndWithoutDeals,

                    manager = CRMReportResource.Manager,
                    total = CRMReportResource.Total,

                    noSet = CRMCommonResource.NoSet,
                    withDeals = CRMReportResource.ContactsWithDeals,
                    withouthDeals = CRMReportResource.ContactsWithoutDeals,
                },
                data = reportData
            };
        }

        #endregion


        #region WorkloadByTasksReport

        public bool CheckWorkloadByTasksReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var sqlNewTasksQuery = Query(CrmDbContext.Tasks)
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                        .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Any();

            var sqlClosedTasksQuery = Query(CrmDbContext.Tasks)
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                        .Where(x => x.IsClosed)
                                        .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Any();



            var sqlOverdueTasksQuery = Query(CrmDbContext.Tasks)
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                        .Where(x => x.IsClosed)
                                        .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Deadline >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Where(x => (!x.IsClosed && x.Deadline < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ||
                                                     (x.IsClosed && x.LastModifedOn > x.Deadline))
                                        .Any();

            return sqlNewTasksQuery ||
                   sqlClosedTasksQuery ||
                   sqlOverdueTasksQuery;
        }

        public object GetWorkloadByTasksReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            var reportData = BuildWorkloadByTasksReport(timePeriod, managers);

            if (reportData == null || !reportData.Any()) return null;

            var hasData = reportData.Any(item => item.Value.Count > 0);

            return hasData ? GenerateReportData(timePeriod, reportData) : null;
        }

        private Dictionary<string, List<WorkloadByTasks>> BuildWorkloadByTasksReport(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var sqlNewTasksQuery = Query(CrmDbContext.Tasks)
                                   .GroupJoin(Query(CrmDbContext.ListItem),
                                         x => x.CategoryId,
                                         y => y.Id,
                                         (x, y) => new { x, y })
                                   .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Task = x.x, ListItem = y })
                                   .Where(x => x.ListItem.ListType == ListType.TaskCategory)
                                   .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
                                   .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Task.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                   .OrderBy(x => x.ListItem.SortOrder)
                                   .GroupBy(x => new { x.ListItem.Id, x.ListItem.Title, x.Task.ResponsibleId })
                                   .Select(x => new
                                   {
                                       id = x.Key.Id,
                                       title = x.Key.Title,
                                       responsibleId = x.Key.ResponsibleId,
                                       count = x.Count()
                                   })
                                   .ToList()
                                  .ConvertAll(x => new WorkloadByTasks
                                  {
                                      CategoryId = x.id,
                                      CategoryName = x.title,
                                      UserId = x.responsibleId,
                                      UserName = _displayUserSettings.GetFullUserName(x.responsibleId),
                                      Count = x.count
                                  });

            var sqlClosedTasksQuery = Query(CrmDbContext.Tasks)
                       .GroupJoin(Query(CrmDbContext.ListItem),
                             x => x.CategoryId,
                             y => y.Id,
                             (x, y) => new { x, y })
                       .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Task = x.x, ListItem = y })
                       .Where(x => x.ListItem.ListType == ListType.TaskCategory)
                       .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
                       .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Task.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))
                       .Where(x => x.Task.IsClosed)
                       .OrderBy(x => x.ListItem.SortOrder)
                       .GroupBy(x => new { x.ListItem.Id, x.ListItem.Title, x.Task.ResponsibleId })
                       .Select(x => new
                       {
                           id = x.Key.Id,
                           title = x.Key.Title,
                           responsibleId = x.Key.ResponsibleId,
                           count = x.Count()
                       })
                       .ToList()
                      .ConvertAll(x => new WorkloadByTasks
                      {
                          CategoryId = x.id,
                          CategoryName = x.title,
                          UserId = x.responsibleId,
                          UserName = _displayUserSettings.GetFullUserName(x.responsibleId),
                          Count = x.count
                      });

            var sqlOverdueTasksQuery = Query(CrmDbContext.Tasks)
                                   .GroupJoin(Query(CrmDbContext.ListItem),
                                         x => x.CategoryId,
                                         y => y.Id,
                                         (x, y) => new { x, y })
                                   .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Task = x.x, ListItem = y })
                                   .Where(x => x.ListItem.ListType == ListType.TaskCategory)
                                   .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
                                   .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Task.Deadline >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.Deadline <= _tenantUtil.DateTimeToUtc(toDate))
                                   .Where(x => (!x.Task.IsClosed && x.Task.Deadline < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) || (x.Task.IsClosed && x.Task.LastModifedOn > x.Task.Deadline))
                                   .OrderBy(x => x.ListItem.SortOrder)
                                   .GroupBy(x => new { x.ListItem.Id, x.ListItem.Title, x.Task.ResponsibleId })
                                   .Select(x => new
                                   {
                                       id = x.Key.Id,
                                       title = x.Key.Title,
                                       responsibleId = x.Key.ResponsibleId,
                                       count = x.Count()
                                   })
                                   .ToList()
                                   .ConvertAll(x => new WorkloadByTasks
                                   {
                                       CategoryId = x.id,
                                       CategoryName = x.title,
                                       UserId = x.responsibleId,
                                       UserName = _displayUserSettings.GetFullUserName(x.responsibleId),
                                       Count = x.count
                                   });

            return new Dictionary<string, List<WorkloadByTasks>> {
                {"Created", sqlNewTasksQuery},
                {"Closed", sqlClosedTasksQuery},
                {"Overdue", sqlOverdueTasksQuery}
            };
        }

        private object GenerateReportData(ReportTimePeriod timePeriod, Dictionary<string, List<WorkloadByTasks>> reportData)
        {
            return new
            {
                resource = new
                {
                    header = CRMReportResource.WorkloadByTasksReport,
                    sheetName = CRMReportResource.WorkloadByTasksReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),

                    header1 = CRMReportResource.ClosedTasks,
                    header2 = CRMReportResource.NewTasks,
                    header3 = CRMReportResource.OverdueTasks,

                    manager = CRMReportResource.Manager,
                    total = CRMReportResource.Total
                },
                data = reportData
            };
        }

        #endregion


        #region WorkloadByDealsReport

        public bool CheckWorkloadByDealsReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            return Query(CrmDbContext.Deals)
                   .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                   .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : (x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate)) ||
                                                                                      (x.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate)))
                   .Any();
        }

        public object GetWorkloadByDealsReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            var reportData = BuildWorkloadByDealsReport(timePeriod, managers, defaultCurrency);

            return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
        }

        private List<WorkloadByDeals> BuildWorkloadByDealsReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var result = Query(CrmDbContext.Deals)
                               .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                       x => x.BidCurrency,
                                       y => y.FromCurrency,
                                       (x, y) => new { x, y })
                               .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                               .GroupJoin(Query(CrmDbContext.DealMilestones),
                                     x => x.Deal.DealMilestoneId,
                                     y => y.Id,
                                     (x, y) => new { x, y })
                              .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                             .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                             .Where(x => x.Deal.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                             .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : (x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate) ||
                                                                                                x.Deal.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate)))
                             .GroupBy(x => new { x.Deal.ResponsibleId, x.DealMilestone.Status })
                             .Select(x => new
                             {
                                 responsible_id = x.Key.ResponsibleId,
                                 status = x.Key.Status,
                                 count = x.Count(),
                                 deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                     x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                             }).ToList()
                             .ConvertAll(x => new WorkloadByDeals
                             {
                                 UserId = x.responsible_id,
                                 UserName = _displayUserSettings.GetFullUserName(x.responsible_id),
                                 Status = x.status,
                                 Count = x.count,
                                 Value = x.deals_value
                             });

            return result;
        }

        private object GenerateReportData(ReportTimePeriod timePeriod, List<WorkloadByDeals> data)
        {
            var reportData = data.Select(item => new List<object>
                    {
                        item.UserId,
                        item.UserName,
                        (int)item.Status,
                        item.Count,
                        item.Value
                    }).ToList();

            return new
            {
                resource = new
                {
                    header = CRMReportResource.WorkloadByDealsReport,
                    sheetName = CRMReportResource.WorkloadByDealsReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),

                    chartName = CRMReportResource.DealsCount,
                    chartName1 = CRMReportResource.DealsBudget + ", " + _defaultCurrency.Symbol,

                    header1 = CRMReportResource.ByCount,
                    header2 = CRMReportResource.ByBudget + ", " + _defaultCurrency.Symbol,

                    manager = CRMReportResource.Manager,
                    total = CRMReportResource.Total,

                    status0 = CRMReportResource.New,
                    status1 = CRMReportResource.Won,
                    status2 = CRMReportResource.Lost
                },
                data = reportData
            };
        }

        #endregion


        #region WorkloadByInvoicesReport

        public bool CheckWorkloadByInvoicesReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            return Query(CrmDbContext.Invoices)
                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                        .Where(x => (x.Status != InvoiceStatus.Draft && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.IssueDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.IssueDate <= _tenantUtil.DateTimeToUtc(toDate))) ||
                                    (x.Status == InvoiceStatus.Paid && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))) ||
                                    (x.Status == InvoiceStatus.Rejected && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))) ||
                                    ((timePeriod == ReportTimePeriod.DuringAllTime ? true : x.DueDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DueDate <= _tenantUtil.DateTimeToUtc(toDate))) &&
                                    (x.Status == InvoiceStatus.Sent && x.DueDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()) || x.Status == InvoiceStatus.Paid && x.LastModifedOn > x.DueDate))
                        .Any();
        }

        public object GetWorkloadByInvoicesReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            var reportData = BuildWorkloadByInvoicesReport(timePeriod, managers);

            if (reportData == null || !reportData.Any()) return null;

            var hasData = reportData.Any(item => item.SentCount > 0 || item.PaidCount > 0 || item.RejectedCount > 0 || item.OverdueCount > 0);

            return hasData ? GenerateReportData(timePeriod, reportData) : null;
        }

        private List<WorkloadByInvoices> BuildWorkloadByInvoicesReport(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var result = Query(CrmDbContext.Invoices)
                                .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                .GroupBy(x => x.CreateBy)
                                .Select(x => new
                                {
                                    createBy = x.Key,
                                    sent = x.Sum(x => x.Status != InvoiceStatus.Draft && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.IssueDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.IssueDate <= _tenantUtil.DateTimeToUtc(toDate)) ? 1 : 0),
                                    paid = x.Sum(x => x.Status == InvoiceStatus.Paid && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate)) ? 1 : 0),
                                    rejected = x.Sum(x => x.Status == InvoiceStatus.Rejected && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate)) ? 1 : 0),
                                    overdue = x.Sum(x => (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.DueDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DueDate <= _tenantUtil.DateTimeToUtc(toDate)) && ((x.Status == InvoiceStatus.Sent && x.DueDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) || (x.Status == InvoiceStatus.Paid && x.LastModifedOn > x.DueDate)) ? 1 : 0)
                                })
                                .ToList()
                                .ConvertAll(x => new WorkloadByInvoices
                                {
                                    UserId = x.createBy,
                                    UserName = _displayUserSettings.GetFullUserName(x.createBy),
                                    SentCount = x.sent,
                                    PaidCount = x.paid,
                                    RejectedCount = x.rejected,
                                    OverdueCount = x.overdue
                                });

            return result;
        }


        private object GenerateReportData(ReportTimePeriod timePeriod, List<WorkloadByInvoices> reportData)
        {
            return new
            {
                resource = new
                {
                    header = CRMReportResource.WorkloadByInvoicesReport,
                    sheetName = CRMReportResource.WorkloadByInvoicesReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),

                    chartName = CRMReportResource.BilledInvoices,
                    chartName1 = CRMInvoiceResource.Invoices,

                    header1 = CRMInvoiceResource.Invoices,

                    manager = CRMReportResource.Manager,
                    total = CRMReportResource.Total,

                    billed = CRMReportResource.Billed,
                    paid = CRMReportResource.Paid,
                    rejected = CRMReportResource.Rejected,
                    overdue = CRMReportResource.Overdue
                },
                data = reportData
            };
        }

        #endregion


        #region GetWorkloadByViopReport

        public bool CheckWorkloadByViopReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            return Query(CrmDbContext.VoipCalls)
                        .Where(x => x.ParentCallId == "")
                        .Where(x => managers != null && managers.Any() ? managers.ToList().Contains(x.AnsweredBy) : true)
                        .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ?
                                    true :
                                    x.DialDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DialDate <= _tenantUtil.DateTimeToUtc(toDate))
                        .Any();
        }

        public object GetWorkloadByViopReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            var reportData = BuildWorkloadByViopReport(timePeriod, managers);

            return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
        }

        private List<WorkloadByViop> BuildWorkloadByViopReport(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var result = Query(CrmDbContext.VoipCalls)
                            .Where(x => x.ParentCallId == "")
                            .Where(x => managers != null && managers.Any() ? managers.Contains(x.AnsweredBy) : true)
                            .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.DialDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DialDate <= _tenantUtil.DateTimeToUtc(toDate))
                            .GroupBy(x => new { x.AnsweredBy, x.Status })
                            .Select(x => new
                            {
                                answered_by = x.Key.AnsweredBy,
                                status = x.Key.Status,
                                calls_count = x.Count(),
                                duration = x.Sum(x => x.DialDuration)
                            })
                            .ToList()
                            .ConvertAll(x => new WorkloadByViop
                            {
                                UserId = x.answered_by,
                                UserName = _displayUserSettings.GetFullUserName(x.answered_by),
                                Status = x.status,
                                Count = x.calls_count,
                                Duration = x.duration ?? x.duration.Value
                            });

            return result;
        }

        private WorkloadByViop ToWorkloadByViop(object[] row)
        {
            return new WorkloadByViop
            {
                UserId = string.IsNullOrEmpty(Convert.ToString(row[0])) ? Guid.Empty : new Guid(Convert.ToString(row[0])),
                UserName = Convert.ToString(row[1] ?? string.Empty),
                Status = (VoipCallStatus)Convert.ToInt32(row[2] ?? 0),
                Count = Convert.ToInt32(row[3]),
                Duration = Convert.ToInt32(row[4])
            };
        }

        private object GenerateReportData(ReportTimePeriod timePeriod, List<WorkloadByViop> data)
        {
            var reportData = data.Select(item => new List<object>
                {
                    item.UserId,
                    item.UserName,
                    (int) item.Status,
                    item.Count,
                    new {format = TimeFormat, value = SecondsToTimeFormat(item.Duration)}
                }).ToList();

            return new
            {
                resource = new
                {
                    header = CRMReportResource.WorkloadByVoipReport,
                    sheetName = CRMReportResource.WorkloadByVoipReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),

                    chartName = CRMReportResource.CallsCount,
                    chartName1 = CRMReportResource.CallsDuration,

                    header1 = CRMReportResource.CallsCount,
                    header2 = CRMReportResource.CallsDuration,

                    manager = CRMReportResource.Manager,
                    total = CRMReportResource.Total,

                    incoming = CRMReportResource.Incoming,
                    outcoming = CRMReportResource.Outcoming,

                    timeFormat = TimeFormat
                },
                data = reportData
            };
        }

        private string SecondsToTimeFormat(int duration)
        {
            var timeSpan = TimeSpan.FromSeconds(duration);

            return $"{(timeSpan.TotalHours < 10 ? "0" : "") + (int)timeSpan.TotalHours}:{(timeSpan.Minutes < 10 ? "0" : "") + timeSpan.Minutes}:{(timeSpan.Seconds < 10 ? "0" : "") + timeSpan.Seconds}";
        }

        #endregion


        #region SummaryForThePeriodReport

        public bool CheckSummaryForThePeriodReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var newDealsSqlQuery = Query(CrmDbContext.Deals)
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                        .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Any();

            var closedDealsSqlQuery = Query(CrmDbContext.Deals)
                            .Join(Query(CrmDbContext.DealMilestones),
                                  x => x.DealMilestoneId,
                                  y => y.Id,
                                  (x, y) => new { x, y })
                            .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                            .Where(x => x.x.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                            .Where(x => x.y.Status != DealMilestoneStatus.Open)
                            .Any();

            var overdueDealsSqlQuery = Query(CrmDbContext.Deals)
                            .Join(Query(CrmDbContext.DealMilestones),
                                  x => x.DealMilestoneId,
                                  y => y.Id,
                                  (x, y) => new { x, y })
                            .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                            .Where(x => x.x.ExpectedCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ExpectedCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                            .Where(x => (x.y.Status == DealMilestoneStatus.Open && x.x.ExpectedCloseDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ||
                                        (x.y.Status == DealMilestoneStatus.ClosedAndWon && x.x.ActualCloseDate > x.x.ExpectedCloseDate))
                            .Any();

            var invoicesSqlQuery = Query(CrmDbContext.Invoices)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                      .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();

            var contactsSqlQuery = Query(CrmDbContext.Contacts)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                      .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();


            var tasksSqlQuery = Query(CrmDbContext.Tasks)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                      .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();


            var voipSqlQuery = Query(CrmDbContext.VoipCalls)
                          .Where(x => x.ParentCallId == "")
                          .Where(x => managers != null && managers.Any() ? managers.Contains(x.AnsweredBy) : true)
                          .Where(x => x.DialDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DialDate <= _tenantUtil.DateTimeToUtc(toDate))
                          .Any();

            return newDealsSqlQuery ||
                   closedDealsSqlQuery ||
                   overdueDealsSqlQuery ||
                   invoicesSqlQuery ||
                   contactsSqlQuery ||
                   tasksSqlQuery ||
                   voipSqlQuery;
        }

        public object GetSummaryForThePeriodReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            var reportData = BuildSummaryForThePeriodReport(timePeriod, managers, defaultCurrency);

            if (reportData == null) return null;

            return GenerateSummaryForThePeriodReportData(timePeriod, reportData);
        }

        private object BuildSummaryForThePeriodReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var newDealsSqlQuery = Query(CrmDbContext.Deals)
                                           .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                                   x => x.BidCurrency,
                                                   y => y.FromCurrency,
                                                   (x, y) => new { x, y })
                                           .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                                           .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                           .Where(x => x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                           .GroupBy(x => x.Deal.Id)
                                           .Select(x => new
                                           {
                                               count = x.Count(),
                                               deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                   x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                           }).ToList();

            var wonDealsSqlQuery = Query(CrmDbContext.Deals)
                                           .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                                   x => x.BidCurrency,
                                                   y => y.FromCurrency,
                                                   (x, y) => new { x, y })
                                           .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                                           .GroupJoin(Query(CrmDbContext.DealMilestones),
                                                 x => x.Deal.DealMilestoneId,
                                                 y => y.Id,
                                                 (x, y) => new { x, y })
                                          .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                                          .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                          .Where(x => x.Deal.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                                          .Where(x => x.DealMilestone.Status == DealMilestoneStatus.ClosedAndWon)
                                          .GroupBy(x => x.Deal.Id)
                                          .Select(x => new
                                          {
                                              count = x.Count(),
                                              deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                  x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                          }).ToList();

            var lostDealsSqlQuery = Query(CrmDbContext.Deals)
                                           .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                                   x => x.BidCurrency,
                                                   y => y.FromCurrency,
                                                   (x, y) => new { x, y })
                                           .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                                           .GroupJoin(Query(CrmDbContext.DealMilestones),
                                                 x => x.Deal.DealMilestoneId,
                                                 y => y.Id,
                                                 (x, y) => new { x, y })
                                          .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                                         .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                         .Where(x => x.Deal.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                                         .Where(x => x.DealMilestone.Status == DealMilestoneStatus.ClosedAndLost)
                                         .GroupBy(x => x.Deal.Id)
                                         .Select(x => new
                                         {
                                             count = x.Count(),
                                             deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                 x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                         }).ToList();


            var overdueDealsSqlQuery = Query(CrmDbContext.Deals)
                                           .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                                   x => x.BidCurrency,
                                                   y => y.FromCurrency,
                                                   (x, y) => new { x, y })
                                           .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                                           .GroupJoin(Query(CrmDbContext.DealMilestones),
                                                 x => x.Deal.DealMilestoneId,
                                                 y => y.Id,
                                                 (x, y) => new { x, y })
                                          .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                        .Where(x => x.Deal.ExpectedCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ExpectedCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Where(x => (x.DealMilestone.Status == DealMilestoneStatus.Open && x.Deal.ExpectedCloseDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ||
                                                    (x.DealMilestone.Status == DealMilestoneStatus.ClosedAndWon && x.Deal.ActualCloseDate > x.Deal.ExpectedCloseDate))
                                        .GroupBy(x => x.Deal.Id)
                                        .Select(x => new
                                        {
                                            count = x.Count(),
                                            deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : x.Deal.PerPeriodValue) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                        }).ToList();

            var invoicesSqlQuery = Query(CrmDbContext.Invoices)
                                  .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                  .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                  .GroupBy(x => x.Status)
                                  .Select(x => new
                                  {
                                      sent = x.Sum(x => x.Status != InvoiceStatus.Draft ? 1 : 0),
                                      paid = x.Sum(x => x.Status == InvoiceStatus.Paid ? 1 : 0),
                                      rejected = x.Sum(x => x.Status == InvoiceStatus.Rejected ? 1 : 0),
                                      overdue = x.Sum(x => (x.Status == InvoiceStatus.Sent && x.DueDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ||
                                                           (x.Status == InvoiceStatus.Paid && x.LastModifedOn > x.DueDate)
                                      ? 1 : 0)
                                  }).ToList();

            var contactsSqlQuery = Query(CrmDbContext.Contacts)
                                  .GroupJoin(Query(CrmDbContext.ListItem),
                                      x => x.ContactTypeId,
                                      y => y.Id,
                                      (x, y) => new { x, y })
                                  .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Contact = x.x, ListItem = y })
                                  .Where(x => x.ListItem.ListType == ListType.ContactType)
                                  .Where(x => managers != null && managers.Any() ? managers.Contains(x.Contact.CreateBy) : true)
                                  .Where(x => x.Contact.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Contact.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                  .GroupBy(x => new { x.ListItem.Title })
                                  .Select(x => new
                                  {
                                      title = x.Key.Title,
                                      count = x.Count()
                                  })
                                  .OrderBy(x => x.title)
                                  .ToList();

            var tasksSqlQuery = Query(CrmDbContext.Tasks)
                                .GroupJoin(Query(CrmDbContext.ListItem),
                                      x => x.CategoryId,
                                      y => y.Id,
                                      (x, y) => new { x, y })
                              .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Task = x.x, ListItem = y })
                              .Where(x => x.ListItem.ListType == ListType.TaskCategory)
                              .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
                              .Where(x => x.Task.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                              .GroupBy(x => new { x.ListItem.Title })
                              .Select(x => new
                              {

                                  title = x.Key.Title,
                                  sum1 = x.Sum(x => x.Task.IsClosed && x.Task.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate) ? 1 : 0),
                                  sum2 = x.Sum(x => (!x.Task.IsClosed && x.Task.Deadline < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) || (x.Task.IsClosed && x.Task.LastModifedOn > x.Task.Deadline) ? 1 : 0),
                                  count = x.Count()
                              })
                              .OrderBy(x => x.title)
                              .ToList();
            //    .OrderBy("i.sort_order, i.title", true);

            var voipSqlQuery = Query(CrmDbContext.VoipCalls)
                                .Where(x => String.IsNullOrEmpty(x.ParentCallId))
                                .Where(x => managers != null && managers.Any() ? managers.Contains(x.AnsweredBy) : true)
                                .Where(x => x.DialDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DialDate <= _tenantUtil.DateTimeToUtc(toDate))
                                .GroupBy(x => x.Status)
                                .Select(x => new
                                {
                                    status = x.Key,
                                    calls_count = x.Count(),
                                    duration = x.Sum(x => x.DialDuration)
                                })
                                .ToList();
            return new
            {
                DealsInfo = new
                {
                    Created = newDealsSqlQuery,
                    Won = wonDealsSqlQuery,
                    Lost = lostDealsSqlQuery,
                    Overdue = overdueDealsSqlQuery
                },
                InvoicesInfo = invoicesSqlQuery,
                ContactsInfo = contactsSqlQuery,
                TasksInfo = tasksSqlQuery,
                VoipInfo = voipSqlQuery
            };
        }

        private object GenerateSummaryForThePeriodReportData(ReportTimePeriod timePeriod, object reportData)
        {
            return new
            {
                resource = new
                {
                    header = CRMReportResource.SummaryForThePeriodReport,
                    sheetName = CRMReportResource.SummaryForThePeriodReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),
                    chartName = CRMReportResource.DealsByBudget + ", " + _defaultCurrency.Symbol,
                    chartName1 = CRMReportResource.DealsByCount,
                    chartName2 = CRMReportResource.ContactsByType,
                    chartName3 = CRMReportResource.TasksForThePeriod,
                    chartName4 = CRMReportResource.InvoicesForThePeriod,
                    chartName5 = CRMReportResource.CallsForThePeriod,
                    header1 = CRMDealResource.Deals,
                    header2 = CRMContactResource.Contacts,
                    header3 = CRMTaskResource.Tasks,
                    header4 = CRMInvoiceResource.Invoices,
                    header5 = CRMReportResource.Calls,
                    byBudget = CRMReportResource.ByBudget,
                    currency = _defaultCurrency.Symbol,
                    byCount = CRMReportResource.ByCount,
                    item = CRMReportResource.Item,
                    type = CRMReportResource.Type,
                    won = CRMReportResource.Won,
                    lost = CRMReportResource.Lost,
                    created = CRMReportResource.Created,
                    closed = CRMReportResource.Closed,
                    overdue = CRMReportResource.Overdue,
                    notSpecified = CRMCommonResource.NoSet,
                    total = CRMReportResource.Total,
                    status = CRMReportResource.Status,
                    billed = CRMReportResource.Billed,
                    paid = CRMReportResource.Paid,
                    rejected = CRMReportResource.Rejected,
                    count = CRMReportResource.Count,
                    duration = CRMReportResource.Duration,
                    incoming = CRMReportResource.Incoming,
                    outcoming = CRMReportResource.Outcoming,
                    missed = CRMReportResource.MissedCount,
                    timeFormat = TimeFormat
                },
                data = reportData
            };
        }

        #endregion


        #region SummaryAtThisMomentReport

        public bool CheckSummaryAtThisMomentReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var dealsSqlQuery = Query(CrmDbContext.Deals)
                                        .Join(CrmDbContext.DealMilestones,
                                              x => x.DealMilestoneId,
                                              y => y.Id,
                                              (x, y) => new { x, y })
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                                        .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Where(x => x.y.Status == DealMilestoneStatus.Open)
                                        .Any();


            var contactsSqlQuery = Query(CrmDbContext.Contacts)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                      .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();

            var tasksSqlQuery = Query(CrmDbContext.Tasks)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                      .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();

            var invoicesSqlQuery = Query(CrmDbContext.Invoices)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                      .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();

            return dealsSqlQuery ||
                    contactsSqlQuery ||
                    tasksSqlQuery ||
                    invoicesSqlQuery;

        }

        public object GetSummaryAtThisMomentReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            var reportData = BuildSummaryAtThisMomentReport(timePeriod, managers, defaultCurrency);

            if (reportData == null) return null;

            return GenerateSummaryAtThisMomentReportData(timePeriod, reportData);
        }

        private object BuildSummaryAtThisMomentReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var openDealsSqlQuery = Query(CrmDbContext.Deals)
                                           .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                                   x => x.BidCurrency,
                                                   y => y.FromCurrency,
                                                   (x, y) => new { x, y })
                                           .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                                           .GroupJoin(Query(CrmDbContext.DealMilestones),
                                                 x => x.Deal.DealMilestoneId,
                                                 y => y.Id,
                                                 (x, y) => new { x, y })
                                          .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                                          .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                          .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                          .Where(x => x.DealMilestone.Status == DealMilestoneStatus.Open)
                                          .GroupBy(x => x.Deal.Id)
                                          .Select(x => new
                                          {
                                              count = x.Count(),
                                              deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                  x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                          }).ToList();

            var overdueDealsSqlQuery = Query(CrmDbContext.Deals)
                               .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                       x => x.BidCurrency,
                                       y => y.FromCurrency,
                                       (x, y) => new { x, y })
                               .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                               .GroupJoin(Query(CrmDbContext.DealMilestones),
                                     x => x.Deal.DealMilestoneId,
                                     y => y.Id,
                                     (x, y) => new { x, y })
                              .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                              .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                              .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                              .Where(x => x.DealMilestone.Status == DealMilestoneStatus.Open)
                              .Where(x => x.Deal.ExpectedCloseDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()))
                              .GroupBy(x => x.Deal.Id)
                              .Select(x => new
                              {
                                  count = x.Count(),
                                  deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                      x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                              }).ToList();


            var nearDealsSqlQuery = Query(CrmDbContext.Deals)
                   .GroupJoin(Query(CrmDbContext.CurrencyRate),
                           x => x.BidCurrency,
                           y => y.FromCurrency,
                           (x, y) => new { x, y })
                   .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x, CurrencyRate = y })
                   .GroupJoin(Query(CrmDbContext.DealMilestones),
                         x => x.Deal.DealMilestoneId,
                         y => y.Id,
                         (x, y) => new { x, y })
                  .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, CurrencyRate = x.x.CurrencyRate, DealMilestone = y })
                  .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                  .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                  .Where(x => x.DealMilestone.Status == DealMilestoneStatus.Open)
                  .Where(x => x.Deal.ExpectedCloseDate >= _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()) && x.Deal.ExpectedCloseDate <= _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow().AddDays(30)))
                  .GroupBy(x => x.Deal.Id)
                  .Select(x => new
                  {
                      count = x.Count(),
                      deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                          x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                  }).ToList();

            var dealsByStageSqlQuery = Query(CrmDbContext.DealMilestones)
                                            .GroupJoin(Query(CrmDbContext.Deals),
                                                   x => x.Id,
                                                   y => y.DealMilestoneId,
                                                   (x, y) => new { x, y })
                                            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = y, DealMilestone = x.x })
                                            .GroupJoin(Query(CrmDbContext.CurrencyRate),
                                                    x => x.Deal.BidCurrency,
                                                    y => y.FromCurrency,
                                                    (x, y) => new { x, y })
                                            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Deal = x.x.Deal, DealMilestone = x.x.DealMilestone, CurrencyRate = y })
                                          .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                          .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                          .Where(x => x.DealMilestone.Status == DealMilestoneStatus.Open)
                                          .GroupBy(x => new { x.DealMilestone.Id, x.DealMilestone.Title })
                                          .Select(x => new
                                          {
                                              title = x.Key.Title,
                                              deals_count = x.Count(),
                                              deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                  x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : Convert.ToDecimal(x.Deal.PerPeriodValue)) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                          }).ToList();

            var contactsByTypeSqlQuery = Query(CrmDbContext.Contacts)
                                        .GroupJoin(Query(CrmDbContext.ListItem),
                                            x => x.ContactTypeId,
                                            y => y.Id,
                                            (x, y) => new { x, y })
                                        .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Contact = x.x, ListItem = y })
                                        .Where(x => x.ListItem.ListType == ListType.ContactType)
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.Contact.CreateBy) : true)
                                        .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Contact.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Contact.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .GroupBy(x => new { x.ListItem.Title })
                                        .Select(x => new
                                        {
                                            title = x.Key.Title,
                                            count = x.Count()
                                        })
                                        .OrderBy(x => x.title)
                                        .ToList();

            var contactsByStageSqlQuery = Query(CrmDbContext.Contacts)
                                  .GroupJoin(Query(CrmDbContext.ListItem),
                                      x => x.StatusId,
                                      y => y.Id,
                                      (x, y) => new { x, y })
                                  .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Contact = x.x, ListItem = y })
                                  .Where(x => x.ListItem.ListType == ListType.ContactStatus)
                                  .Where(x => managers != null && managers.Any() ? managers.Contains(x.Contact.CreateBy) : true)
                                  .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Contact.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Contact.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                  .GroupBy(x => new { x.ListItem.Title })
                                  .Select(x => new
                                  {
                                      title = x.Key.Title,
                                      count = x.Count()
                                  })
                                  .OrderBy(x => x.title)
                                  .ToList();

            var tasksSqlQuery = Query(CrmDbContext.Tasks)
                    .GroupJoin(Query(CrmDbContext.ListItem),
                          x => x.CategoryId,
                          y => y.Id,
                          (x, y) => new { x, y })
                  .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { Task = x.x, ListItem = y })
                  .Where(x => x.ListItem.ListType == ListType.TaskCategory)
                  .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
                  .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Task.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                  .GroupBy(x => new { x.ListItem.Title })
                  .Select(x => new
                  {

                      title = x.Key.Title,
                      sum1 = x.Sum(x => !x.Task.IsClosed ? 1 : 0),
                      sum2 = x.Sum(x => (!x.Task.IsClosed && x.Task.Deadline < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ? 1 : 0),
                      count = x.Count()
                  })
                  .OrderBy(x => x.title)
                  .ToList();


            var invoicesSqlQuery = Query(CrmDbContext.Invoices)
                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                      .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                      .GroupBy(x => x.Status)
                      .Select(x => new
                      {
                          sent = x.Sum(x => x.Status == InvoiceStatus.Sent ? 1 : 0),
                          overdue = x.Sum(x => (x.Status == InvoiceStatus.Sent && x.DueDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ? 1 : 0)
                      }).ToList();


            return new
            {
                DealsInfo = new
                {
                    Open = openDealsSqlQuery,
                    Overdue = overdueDealsSqlQuery,
                    Near = nearDealsSqlQuery,
                    ByStage = dealsByStageSqlQuery
                },
                ContactsInfo = new
                {
                    ByType = contactsByTypeSqlQuery,
                    ByStage = contactsByStageSqlQuery
                },
                TasksInfo = tasksSqlQuery,
                InvoicesInfo = invoicesSqlQuery
            };
        }

        private object GenerateSummaryAtThisMomentReportData(ReportTimePeriod timePeriod, object reportData)
        {
            return new
            {
                resource = new
                {
                    header = CRMReportResource.SummaryAtThisMomentReport,
                    sheetName = CRMReportResource.SummaryAtThisMomentReport,
                    dateRangeLabel = CRMReportResource.TimePeriod + ":",
                    dateRangeValue = GetTimePeriodText(timePeriod),
                    chartName = CRMReportResource.DealsByStatus + ", " + _defaultCurrency.Symbol,
                    chartName1 = CRMReportResource.DealsByStage + ", " + _defaultCurrency.Symbol,
                    chartName2 = CRMReportResource.ContactsByType,
                    chartName3 = CRMReportResource.ContactsByStage,
                    chartName4 = CRMReportResource.TasksByStatus,
                    chartName5 = CRMReportResource.InvoicesByStatus,
                    header1 = CRMDealResource.Deals,
                    header2 = CRMContactResource.Contacts,
                    header3 = CRMTaskResource.Tasks,
                    header4 = CRMInvoiceResource.Invoices,
                    budget = CRMReportResource.Budget + ", " + _defaultCurrency.Symbol,
                    count = CRMReportResource.Count,
                    open = CRMReportResource.Opened,
                    overdue = CRMReportResource.Overdue,
                    near = CRMReportResource.Near,
                    stage = CRMReportResource.Stage,
                    temperature = CRMContactResource.ContactStage,
                    type = CRMReportResource.Type,
                    total = CRMReportResource.Total,
                    billed = CRMReportResource.Billed,
                    notSpecified = CRMCommonResource.NoSet,
                    status = CRMReportResource.Status,
                },
                data = reportData
            };
        }

        #endregion
    }
}