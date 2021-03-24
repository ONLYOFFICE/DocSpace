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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.CRM.Core.EF;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Web.CRM.Classes;
using ASC.Web.Files.Api;

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

        #region Constructor

        public ReportDao(DbContextManager<CRMDbContext> dbContextManager,
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
                       DaoFactory daoFactory) :
            base(dbContextManager,
                 tenantManager,
                 securityContext,
                 logger,
                 ascCache)
        {
            _tenantUtil = tenantUtil;

            _filesIntegration = filesIntegration;
            _global = global;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _serviceProvider = serviceProvider;
            _daoFactory = daoFactory;
            var crmSettings = settingsManager.Load<CRMSettings>();

            _defaultCurrency  = currencyProvider.Get(crmSettings.DefaultCurrency);
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
                    return string.Format("{0}-{1}", fromDate.ToShortDateString(), toDate.ToShortDateString());
                case ReportTimePeriod.CurrentMonth:
                case ReportTimePeriod.PreviousMonth:
                case ReportTimePeriod.NextMonth:
                    return fromDate.ToString("Y");
                case ReportTimePeriod.CurrentQuarter:
                case ReportTimePeriod.PreviousQuarter:
                case ReportTimePeriod.NextQuarter:
                    return string.Format("{0}-{1}", fromDate.ToString("Y"), toDate.ToString("Y"));
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
            var existingRatesQuery = CRMDbContext.CurrencyRate
                                    .Where(x => x.ToCurrency == defaultCurrency)
                                    .Select(x => x.FromCurrency).Distinct().ToList();

            var missingRatesQuery = Query(CRMDbContext.Deals)
                                    .Where(x => x.BidCurrency != defaultCurrency && !existingRatesQuery.Contains(x.BidCurrency))
                                    .Select(x => x.BidCurrency)
                                    .Distinct();

            return missingRatesQuery.ToList();
        }

        #endregion

        #region Report Files

        public List<Files.Core.File<int>> SaveSampleReportFiles()
        {
            var result = new List<Files.Core.File<int>>();

            var storeTemplate = _global.GetStoreTemplate();

            if (storeTemplate == null) return result;

            var culture = _userManager.GetUsers(_securityContext.CurrentAccount.ID).GetCulture() ??
                          _tenantManager.GetCurrentTenant().GetCulture();

            var path = culture + "/";

            if (!storeTemplate.IsDirectory(path))
            {
                path = "default/";
                if (!storeTemplate.IsDirectory(path)) return result;
            }

            foreach (var filePath in storeTemplate.ListFilesRelative("", path, "*", false).Select(x => path + x))
            {
                using (var stream = storeTemplate.GetReadStream("", filePath))
                {

                    var document = _serviceProvider.GetService<Files.Core.File<int>>();

                    document.Title = Path.GetFileName(filePath);
                    document.FolderID = _daoFactory.GetFileDao().GetRoot();
                    document.ContentLength = stream.Length;


                    var file = _daoFactory.GetFileDao().SaveFile(document, stream);

                    SaveFile((int)file.ID, -1);

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

            var fileIds = Query(CRMDbContext.ReportFile).Where(x => x.CreateBy == userId).Select(x => x.FileId).ToArray();

            return fileIds.Length > 0 ? filedao.GetFiles(fileIds) : new List<Files.Core.File<int>>();

        }

        public List<int> GetFileIds(Guid userId)
        {
            return Query(CRMDbContext.ReportFile)
                        .Where(x => x.CreateBy == userId).Select(x => x.FileId).ToList();
        }

        public Files.Core.File<int> GetFile(int fileid)
        {
            return GetFile(fileid, _securityContext.CurrentAccount.ID);
        }

        public Files.Core.File<int> GetFile(int fileid, Guid userId)
        {
            var exist = Query(CRMDbContext.ReportFile)
                        .Any(x => x.CreateBy == userId && x.FileId == fileid);

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            return exist ? filedao.GetFile(fileid) : null;

        }

        public void DeleteFile(int fileid)
        {
            var itemToDelete = Query(CRMDbContext.ReportFile).Where(x => x.FileId == fileid && x.CreateBy == _securityContext.CurrentAccount.ID);

            CRMDbContext.Remove(itemToDelete);
            CRMDbContext.SaveChanges();

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            filedao.DeleteFile(fileid);
        }

        public void DeleteFiles(Guid userId)
        {
            var fileIds = GetFileIds(userId);

            var itemToDelete = Query(CRMDbContext.ReportFile).Where(x => x.CreateBy == _securityContext.CurrentAccount.ID);

            CRMDbContext.Remove(itemToDelete);
            CRMDbContext.SaveChanges();

            var filedao = _filesIntegration.DaoFactory.GetFileDao<int>();

            foreach (var fileId in fileIds)
            {
                filedao.DeleteFile(fileId);
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

            CRMDbContext.Add(itemToInsert);
            CRMDbContext.SaveChanges();
        }

        #endregion


        #region SalesByManagersReport

        public bool CheckSalesByManagersReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);


            return Query(CRMDbContext.Deals).Join(Query(CRMDbContext.DealMilestones),
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

            throw new NotImplementedException();

            //string dateSelector;

            //switch (timePeriod)
            //{
            //    case ReportTimePeriod.Today:
            //    case ReportTimePeriod.Yesterday:
            //        dateSelector = "date_add(date(d.actual_close_date), interval extract(hour from d.actual_close_date) hour) as close_date";
            //        break;
            //    case ReportTimePeriod.CurrentWeek:
            //    case ReportTimePeriod.PreviousWeek:
            //    case ReportTimePeriod.CurrentMonth:
            //    case ReportTimePeriod.PreviousMonth:
            //        dateSelector = "date(d.actual_close_date) as close_date";
            //        break;
            //    case ReportTimePeriod.CurrentQuarter:
            //    case ReportTimePeriod.PreviousQuarter:
            //    case ReportTimePeriod.CurrentYear:
            //    case ReportTimePeriod.PreviousYear:
            //        dateSelector = "date_sub(date(d.actual_close_date), interval (extract(day from d.actual_close_date) - 1) day) as close_date";
            //        break;
            //    default:
            //        return null;
            //}

            //var sqlQuery = Query("crm_deal d")
            //    .Select("d.responsible_id",
            //            "concat(u.firstname, ' ', u.lastname) as full_name",
            //            string.Format(@"sum((case d.bid_type
            //            when 0 then
            //                d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
            //            else
            //                d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
            //            end)) as bid_value", defaultCurrency),
            //            dateSelector)
            //    .LeftOuterJoin("crm_deal_milestone m", Exp.EqColumns("m.id", "d.deal_milestone_id") & Exp.EqColumns("m.tenant_id", "d.tenant_id"))
            //    .LeftOuterJoin("crm_currency_rate r", Exp.EqColumns("r.tenant_id", "d.tenant_id") & Exp.EqColumns("r.from_currency", "d.bid_currency"))
            //    .LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "d.tenant_id") & Exp.EqColumns("u.id", "d.responsible_id"))
            //    .Where("m.status", (int)DealMilestoneStatus.ClosedAndWon)
            //    .Where(managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty)
            //    .Where(Exp.Between("d.actual_close_date", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .GroupBy("responsible_id", "close_date");


            //return Db.ExecuteList(sqlQuery).ConvertAll(ToSalesByManagers);
        }

        private SalesByManager ToSalesByManagers(object[] row)
        {
            return new SalesByManager
            {
                UserId = string.IsNullOrEmpty(Convert.ToString(row[0])) ? Guid.Empty : new Guid(Convert.ToString(row[0])),
                UserName = Convert.ToString(row[1]),
                Value = Convert.ToDecimal(row[2]),
                Date = Convert.ToDateTime(row[3]) == DateTime.MinValue ? DateTime.MinValue : _tenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[3]))
            };
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

            return Query(CRMDbContext.Deals).Join(Query(CRMDbContext.DealMilestones),
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

            throw new NotImplementedException();

            //string dateSelector;

            //switch (timePeriod)
            //{
            //    case ReportTimePeriod.CurrentWeek:
            //    case ReportTimePeriod.NextWeek:
            //    case ReportTimePeriod.CurrentMonth:
            //    case ReportTimePeriod.NextMonth:
            //        dateSelector = "d.expected_close_date as close_date";
            //        break;
            //    case ReportTimePeriod.CurrentQuarter:
            //    case ReportTimePeriod.NextQuarter:
            //    case ReportTimePeriod.CurrentYear:
            //    case ReportTimePeriod.NextYear:
            //        dateSelector = "date_sub(date(d.expected_close_date), interval (extract(day from d.expected_close_date) - 1) day) as close_date";
            //        break;
            //    default:
            //        return null;
            //}

            //var sqlQuery = Query("crm_deal d")
            //    .Select(string.Format(@"sum(case d.bid_type
            //            when 0 then
            //                d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
            //            else
            //                d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
            //            end) as value", defaultCurrency),
            //            string.Format(@"sum(case d.bid_type
            //            when 0 then
            //                d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate)) * d.deal_milestone_probability / 100
            //            else
            //                d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate)) * d.deal_milestone_probability / 100
            //            end) as value_with_probability", defaultCurrency),
            //            dateSelector)
            //    .LeftOuterJoin("crm_deal_milestone m", Exp.EqColumns("m.tenant_id", "d.tenant_id") & Exp.EqColumns("m.id", "d.deal_milestone_id"))
            //    .LeftOuterJoin("crm_currency_rate r", Exp.EqColumns("r.tenant_id", "d.tenant_id") & Exp.EqColumns("r.from_currency", "d.bid_currency"))
            //    .Where("m.status", (int)DealMilestoneStatus.Open)
            //    .Where(managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty)
            //    .Where(Exp.Between("d.expected_close_date", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .GroupBy("close_date");

            //return Db.ExecuteList(sqlQuery).ConvertAll(ToSalesForecast);
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

            return Query(CRMDbContext.Deals)
                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                        .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                        .Any();
        }

        //public object GetSalesFunnelReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
        //{
        //    var reportData = BuildSalesFunnelReport(timePeriod, managers, defaultCurrency);

        //    return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
        //}

    //    private List<SalesFunnel> BuildSalesFunnelReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        var sqlQuery = Query("crm_deal_milestone m")
    //            .Select("m.status", "m.title",
    //                    "count(d.id) as deals_count",
    //                    string.Format(@"sum(case d.bid_type
    //                    when 0 then
    //                        d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
    //                    else
    //                        d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
    //                    end) as deals_value", defaultCurrency),
    //                    "avg(if(m.status = 1, datediff(d.actual_close_date, d.create_on), 0)) as deals_duration")
    //            .LeftOuterJoin("crm_deal d", Exp.EqColumns("d.tenant_id", "m.tenant_id") &
    //                                         Exp.EqColumns("d.deal_milestone_id", "m.id") &
    //                                         (managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty) &
    //                                         Exp.Between("d.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
    //            .LeftOuterJoin("crm_currency_rate r",
    //                           Exp.EqColumns("r.tenant_id", "m.tenant_id") &
    //                           Exp.EqColumns("r.from_currency", "d.bid_currency"))
    //            .GroupBy("m.id")
    //            .OrderBy("m.sort_order", true);


    //        return Db.ExecuteList(sqlQuery).ConvertAll(ToSalesFunnel);
    //    }

    //    private SalesFunnel ToSalesFunnel(object[] row)
    //    {
    //        return new SalesFunnel
    //        {
    //            Status = (DealMilestoneStatus)Convert.ToInt32(row[0]),
    //            Title = Convert.ToString(row[1]),
    //            Count = Convert.ToInt32(row[2]),
    //            Value = Convert.ToDecimal(row[3]),
    //            Duration = Convert.ToInt32(row[4])
    //        };
    //    }

    //    private object GenerateReportData(ReportTimePeriod timePeriod, List<SalesFunnel> data)
    //    {
    //        var totalCount = data.Sum(x => x.Count);

    //        if (totalCount == 0) return null;

    //        var totalBudget = data.Sum(x => x.Value);

    //        var closed = data.Where(x => x.Status == DealMilestoneStatus.ClosedAndWon).ToList();

    //        var reportData = data.Select(item => new List<object>
    //            {
    //                item.Title,
    //                item.Status,
    //                item.Count,
    //                item.Value
    //            }).ToList();

    //        return new
    //        {
    //            resource = new
    //            {
    //                header = CRMReportResource.SalesFunnelReport,
    //                sheetName = CRMReportResource.SalesFunnelReport,
    //                dateRangeLabel = CRMReportResource.TimePeriod + ":",
    //                dateRangeValue = GetTimePeriodText(timePeriod),

    //                chartName = CRMReportResource.SalesFunnelByCount,
    //                chartName1 = CRMReportResource.SalesFunnelByBudget + ", " + _defaultCurrency.Symbol,
    //                chartName2 = CRMReportResource.DealsCount,
    //                chartName3 = CRMReportResource.DealsBudget + ", " + _defaultCurrency.Symbol,

    //                totalCountLabel = CRMReportResource.TotalDealsCount,
    //                totalCountValue = totalCount,

    //                totalBudgetLabel = CRMReportResource.TotalDealsBudget + ", " + _defaultCurrency.Symbol,
    //                totalBudgetValue = totalBudget,

    //                averageBidLabel = CRMReportResource.AverageDealsBudget + ", " + _defaultCurrency.Symbol,
    //                averageBidValue = totalBudget / totalCount,

    //                averageDurationLabel = CRMReportResource.AverageDealsDuration,
    //                averageDurationValue = closed.Sum(x => x.Duration) / closed.Count,

    //                header1 = CRMReportResource.ByCount,
    //                header2 = CRMReportResource.ByBudget + ", " + _defaultCurrency.Symbol,

    //                stage = CRMReportResource.Stage,
    //                count = CRMReportResource.Count,
    //                budget = CRMReportResource.Budget,
    //                conversion = CRMReportResource.Conversion,

    //                deals = CRMDealResource.Deals,
    //                status0 = DealMilestoneStatus.Open.ToLocalizedString(),
    //                status1 = DealMilestoneStatus.ClosedAndWon.ToLocalizedString(),
    //                status2 = DealMilestoneStatus.ClosedAndLost.ToLocalizedString()
    //            },
    //            data = reportData
    //        };
    //    }

    //    #endregion


    //    #region WorkloadByContactsReport

    //    public bool CheckWorkloadByContactsReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        return Query(CRMDbContext.Contacts)
    //                   .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
    //                   .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
    //                   .Any();
    //    }

    //    public object GetWorkloadByContactsReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        var reportData = BuildWorkloadByContactsReport(timePeriod, managers);

    //        return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
    //    }

    //    private List<WorkloadByContacts> BuildWorkloadByContactsReport(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        var sqlQuery = Query(CRMDbContext.Contacts)
    //                        .GroupJoin(Query(CRMDbContext.ListItem),
    //                                   x => x.ContactTypeId,
    //                                   y => y.Id,
    //                                   (x, y) => new { x, y })
    //                        .GroupJoin(Query(CRMDbContext.Deals),
    //                                   x => x.x.Id,
    //                                   y => y.Id,
    //                                   (x, y) => new { x, y })



    //        var sqlQuery = Query("crm_contact c")
    //            .Select("c.create_by",
    //                    "concat(u.firstname, ' ', u.lastname) as full_name",
    //                    "i.id",
    //                    "i.title",
    //                    "count(c.id) as total",
    //                    "count(d.id) as `with deals`")
    //            .LeftOuterJoin("crm_list_item i", Exp.EqColumns("i.tenant_id", "c.tenant_id") & Exp.EqColumns("i.id", "c.contact_type_id") & Exp.Eq("i.list_type", (int)ListType.ContactType))
    //            .LeftOuterJoin("crm_deal d", Exp.EqColumns("d.tenant_id", "c.tenant_id") & Exp.EqColumns("d.contact_id", "c.id"))
    //            .LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "c.tenant_id") & Exp.EqColumns("u.id", "c.create_by"))
    //            .Where(managers != null && managers.Any() ? Exp.In("c.create_by", managers) : Exp.Empty)
    //            .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("c.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
    //            .GroupBy("c.create_by", "i.id")
    //            .OrderBy("i.sort_order, i.title", true);

    //        return Db.ExecuteList(sqlQuery).ConvertAll(ToWorkloadByContacts);
    //    }

    //    private WorkloadByContacts ToWorkloadByContacts(object[] row)
    //    {
    //        return new WorkloadByContacts
    //        {
    //            UserId = string.IsNullOrEmpty(Convert.ToString(row[0])) ? Guid.Empty : new Guid(Convert.ToString(row[0])),
    //            UserName = Convert.ToString(row[1]),
    //            CategoryId = Convert.ToInt32(row[2]),
    //            CategoryName = Convert.ToString(row[3]),
    //            Count = Convert.ToInt32(row[4]),
    //            WithDeals = Convert.ToInt32(row[5])
    //        };
    //    }

    //    private object GenerateReportData(ReportTimePeriod timePeriod, List<WorkloadByContacts> reportData)
    //    {
    //        return new
    //        {
    //            resource = new
    //            {
    //                header = CRMReportResource.WorkloadByContactsReport,
    //                sheetName = CRMReportResource.WorkloadByContactsReport,
    //                dateRangeLabel = CRMReportResource.TimePeriod + ":",
    //                dateRangeValue = GetTimePeriodText(timePeriod),

    //                header1 = CRMReportResource.NewContacts,
    //                header2 = CRMReportResource.NewContactsWithAndWithoutDeals,

    //                manager = CRMReportResource.Manager,
    //                total = CRMReportResource.Total,

    //                noSet = CRMCommonResource.NoSet,
    //                withDeals = CRMReportResource.ContactsWithDeals,
    //                withouthDeals = CRMReportResource.ContactsWithoutDeals,
    //            },
    //            data = reportData
    //        };
    //    }

    //    #endregion


    //    #region WorkloadByTasksReport

    //    public bool CheckWorkloadByTasksReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        var sqlNewTasksQuery = Query(CRMDbContext.Tasks)
    //                                    .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
    //                                    .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
    //                                    .Any();

    //        var sqlClosedTasksQuery = Query(CRMDbContext.Tasks)
    //                                    .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
    //                                    .Where(x => x.IsClosed)
    //                                    .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))
    //                                    .Any();



    //        var sqlOverdueTasksQuery = Query(CRMDbContext.Tasks)
    //                                    .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
    //                                    .Where(x => x.IsClosed)
    //                                    .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Deadline >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))
    //                                    .Where(x => (!x.IsClosed && x.Deadline < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ||
    //                                                 (x.IsClosed && x.LastModifedOn > x.Deadline))
    //                                    .Any();

    //        return sqlNewTasksQuery ||
    //               sqlClosedTasksQuery ||
    //               sqlOverdueTasksQuery;
    //    }

    //    public object GetWorkloadByTasksReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        var reportData = BuildWorkloadByTasksReport(timePeriod, managers);

    //        if (reportData == null || !reportData.Any()) return null;

    //        var hasData = reportData.Any(item => item.Value.Count > 0);

    //        return hasData ? GenerateReportData(timePeriod, reportData) : null;
    //    }

    //    private Dictionary<string, List<WorkloadByTasks>> BuildWorkloadByTasksReport(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        var sqlNewTasksQuery = Query(CRMDbContext.Tasks)
    //                               .Join(Query(CRMDbContext.ListItem).DefaultIfEmpty(),
    //                                     x => x.CategoryId,
    //                                     y => y.Id,
    //                                     (x, y) => new { Task = x, ListItem = y })
    //                               .Join(_userDbContext.Users.DefaultIfEmpty(),
    //                                     x => x.Task.ResponsibleId,
    //                                     y => y.Id,
    //                                     (x, y) => new { Task = x.Task, ListItem = x.ListItem, User = y }
    //                                     )
    //                               .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
    //                               .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Task.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
    //                               .OrderBy(x => x.ListItem.SortOrder)
    //                               .Select(x => new
    //                               {
    //                                   x.ListItem.Id,
    //                                   x.ListItem.Title,
    //                                   x.Task.ResponsibleId,
    //                                   x.User
    //                               });

    //        var sqlNewTasksQuery = Query(CRMDbContext.Tasks)
    //                               .Join(Query(CRMDbContext.ListItem).DefaultIfEmpty(),
    //                                     x => x.CategoryId,
    //                                     y => y.Id,
    //                                     (x, y) => new { Task = x, ListItem = y })
    //                               .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
    //                               .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.Task.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
    //                               .OrderBy(x => x.ListItem.SortOrder)
    //                               .GroupBy(x => new { x.ListItem.Id, x.ListItem.Title, x.Task.ResponsibleId })
    //                               .Select(x => new
    //                               {
    //                                   Id = x.Key.Id,
    //                                   Title = x.Key.Title,
    //                                   ResponsibleId = x.Key.ResponsibleId,
    //                                   Count = x.Count()
    //                               });

    //        throw new NotImplementedException();


    //        var sqlNewTasksQuery = Query("crm_task t")
    //.Select("i.id",
    //        "i.title",
    //        "t.responsible_id",
    //        "concat(u.firstname, ' ', u.lastname) as full_name",
    //        "count(t.id) as count")
    //.LeftOuterJoin("crm_list_item i", Exp.EqColumns("i.tenant_id", "t.tenant_id") & Exp.EqColumns("i.id", "t.category_id") & Exp.Eq("i.list_type", (int)ListType.TaskCategory))
    //.LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "t.tenant_id") & Exp.EqColumns("u.id", "t.responsible_id"))
    //.Where(managers != null && managers.Any() ? Exp.In("t.responsible_id", managers) : Exp.Empty)
    //.Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("t.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
    //.GroupBy("i.id", "t.responsible_id")
    //.OrderBy("i.sort_order", true);

    //        var sqlClosedTasksQuery = Query("crm_task t")
    //            .Select("i.id",
    //                    "i.title",
    //                    "t.responsible_id",
    //                    "concat(u.firstname, ' ', u.lastname) as full_name",
    //                    "count(t.id) as count")
    //            .LeftOuterJoin("crm_list_item i", Exp.EqColumns("i.tenant_id", "t.tenant_id") & Exp.EqColumns("i.id", "t.category_id") & Exp.Eq("i.list_type", (int)ListType.TaskCategory))
    //            .LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "t.tenant_id") & Exp.EqColumns("u.id", "t.responsible_id"))
    //            .Where(managers != null && managers.Any() ? Exp.In("t.responsible_id", managers) : Exp.Empty)
    //            .Where(Exp.Eq("t.is_closed", 1))
    //            .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("t.last_modifed_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
    //            .GroupBy("i.id", "t.responsible_id")
    //            .OrderBy("i.sort_order", true);

    //        var sqlOverdueTasksQuery = Query("crm_task t")
    //            .Select("i.id",
    //                    "i.title",
    //                    "t.responsible_id",
    //                    "concat(u.firstname, ' ', u.lastname) as full_name",
    //                    "count(t.id) as count")
    //            .LeftOuterJoin("crm_list_item i", Exp.EqColumns("i.tenant_id", "t.tenant_id") & Exp.EqColumns("i.id", "t.category_id") & Exp.Eq("i.list_type", (int)ListType.TaskCategory))
    //            .LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "t.tenant_id") & Exp.EqColumns("u.id", "t.responsible_id"))
    //            .Where(managers != null && managers.Any() ? Exp.In("t.responsible_id", managers) : Exp.Empty)
    //            .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("t.deadline", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
    //            .Where(Exp.Or(Exp.Eq("t.is_closed", 0) & Exp.Lt("t.deadline", _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())), (Exp.Eq("t.is_closed", 1) & Exp.Sql("t.last_modifed_on > t.deadline"))))
    //            .GroupBy("i.id", "t.responsible_id")
    //            .OrderBy("i.sort_order", true);

    //        Dictionary<string, List<WorkloadByTasks>> res;

    //        using (var tx = Db.BeginTransaction())
    //        {
    //            res = new Dictionary<string, List<WorkloadByTasks>>
    //                {
    //                    {"Created", Db.ExecuteList(sqlNewTasksQuery).ConvertAll(ToWorkloadByTasks)},
    //                    {"Closed", Db.ExecuteList(sqlClosedTasksQuery).ConvertAll(ToWorkloadByTasks)},
    //                    {"Overdue", Db.ExecuteList(sqlOverdueTasksQuery).ConvertAll(ToWorkloadByTasks)}
    //                };

    //            tx.Commit();
    //        }

    //        return res;
    //    }

    //    private WorkloadByTasks ToWorkloadByTasks(object[] row)
    //    {
    //        return new WorkloadByTasks
    //        {
    //            CategoryId = Convert.ToInt32(row[0]),
    //            CategoryName = Convert.ToString(row[1]),
    //            UserId = string.IsNullOrEmpty(Convert.ToString(row[2])) ? Guid.Empty : new Guid(Convert.ToString(row[2])),
    //            UserName = Convert.ToString(row[3]),
    //            Count = Convert.ToInt32(row[4])
    //        };
    //    }

    //    private object GenerateReportData(ReportTimePeriod timePeriod, Dictionary<string, List<WorkloadByTasks>> reportData)
    //    {
    //        return new
    //        {
    //            resource = new
    //            {
    //                header = CRMReportResource.WorkloadByTasksReport,
    //                sheetName = CRMReportResource.WorkloadByTasksReport,
    //                dateRangeLabel = CRMReportResource.TimePeriod + ":",
    //                dateRangeValue = GetTimePeriodText(timePeriod),

    //                header1 = CRMReportResource.ClosedTasks,
    //                header2 = CRMReportResource.NewTasks,
    //                header3 = CRMReportResource.OverdueTasks,

    //                manager = CRMReportResource.Manager,
    //                total = CRMReportResource.Total
    //            },
    //            data = reportData
    //        };
    //    }

    //    #endregion


    //    #region WorkloadByDealsReport

    //    public bool CheckWorkloadByDealsReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        return Query(CRMDbContext.Deals)
    //               .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
    //               .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : (x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate)) ||
    //                                                                                  (x.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate)))
    //               .Any();
    //    }

    //    public object GetWorkloadByDealsReportData(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
    //    {
    //        var reportData = BuildWorkloadByDealsReport(timePeriod, managers, defaultCurrency);

    //        return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
    //    }

    //    private List<WorkloadByDeals> BuildWorkloadByDealsReport(ReportTimePeriod timePeriod, Guid[] managers, string defaultCurrency)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        var sqlQuery = Query("crm_deal d")
    //            .Select("d.responsible_id",
    //                    "concat(u.firstname, ' ', u.lastname) as full_name",
    //                    "m.status",
    //                    "count(d.id) as deals_count",
    //                    string.Format(@"sum(case d.bid_type
    //                    when 0 then
    //                        d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
    //                    else
    //                        d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
    //                    end) as deals_value", defaultCurrency))
    //            .LeftOuterJoin("crm_deal_milestone m", Exp.EqColumns("m.tenant_id", "d.tenant_id") & Exp.EqColumns("m.id", "d.deal_milestone_id"))
    //            .LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "d.tenant_id") & Exp.EqColumns("u.id", "d.responsible_id"))
    //            .LeftOuterJoin("crm_currency_rate r", Exp.EqColumns("r.tenant_id", "d.tenant_id") & Exp.EqColumns("r.from_currency", "d.bid_currency"))
    //            .Where(managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty)
    //            .Where(timePeriod == ReportTimePeriod.DuringAllTime ?
    //                    Exp.Empty :
    //                    Exp.Or(Exp.Between("d.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)),
    //                            Exp.Between("d.actual_close_date", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate))))
    //            .GroupBy("d.responsible_id", "m.status");


    //        return Db.ExecuteList(sqlQuery).ConvertAll(ToWorkloadByDeals);
    //    }

    //    private WorkloadByDeals ToWorkloadByDeals(object[] row)
    //    {
    //        return new WorkloadByDeals
    //        {
    //            UserId = string.IsNullOrEmpty(Convert.ToString(row[0])) ? Guid.Empty : new Guid(Convert.ToString(row[0])),
    //            UserName = Convert.ToString(row[1]),
    //            Status = (DealMilestoneStatus)Convert.ToInt32(row[2]),
    //            Count = Convert.ToInt32(row[3]),
    //            Value = Convert.ToDecimal(row[4])
    //        };
    //    }

    //    private object GenerateReportData(ReportTimePeriod timePeriod, List<WorkloadByDeals> data)
    //    {
    //        var reportData = data.Select(item => new List<object>
    //            {
    //                item.UserId,
    //                item.UserName,
    //                (int)item.Status,
    //                item.Count,
    //                item.Value
    //            }).ToList();

    //        return new
    //        {
    //            resource = new
    //            {
    //                header = CRMReportResource.WorkloadByDealsReport,
    //                sheetName = CRMReportResource.WorkloadByDealsReport,
    //                dateRangeLabel = CRMReportResource.TimePeriod + ":",
    //                dateRangeValue = GetTimePeriodText(timePeriod),

    //                chartName = CRMReportResource.DealsCount,
    //                chartName1 = CRMReportResource.DealsBudget + ", " + _defaultCurrency.Symbol,

    //                header1 = CRMReportResource.ByCount,
    //                header2 = CRMReportResource.ByBudget + ", " + _defaultCurrency.Symbol,

    //                manager = CRMReportResource.Manager,
    //                total = CRMReportResource.Total,

    //                status0 = CRMReportResource.New,
    //                status1 = CRMReportResource.Won,
    //                status2 = CRMReportResource.Lost
    //            },
    //            data = reportData
    //        };
    //    }

    //    #endregion


    //    #region WorkloadByInvoicesReport

    //    public bool CheckWorkloadByInvoicesReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        return Query(CRMDbContext.Invoices)
    //                    .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
    //                    .Where(x => (x.Status != InvoiceStatus.Draft && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.IssueDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.IssueDate <= _tenantUtil.DateTimeToUtc(toDate))) ||
    //                                (x.Status == InvoiceStatus.Paid && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))) ||
    //                                (x.Status == InvoiceStatus.Rejected && (timePeriod == ReportTimePeriod.DuringAllTime ? true : x.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate))) ||
    //                                ((timePeriod == ReportTimePeriod.DuringAllTime ? true : x.DueDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DueDate <= _tenantUtil.DateTimeToUtc(toDate))) &&
    //                                (x.Status == InvoiceStatus.Sent && x.DueDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()) || x.Status == InvoiceStatus.Paid && x.LastModifedOn > x.DueDate))
    //                    .Any();
    //    }

    //    public object GetWorkloadByInvoicesReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        var reportData = BuildWorkloadByInvoicesReport(timePeriod, managers);

    //        if (reportData == null || !reportData.Any()) return null;

    //        var hasData = reportData.Any(item => item.SentCount > 0 || item.PaidCount > 0 || item.RejectedCount > 0 || item.OverdueCount > 0);

    //        return hasData ? GenerateReportData(timePeriod, reportData) : null;
    //    }

    //    private List<WorkloadByInvoices> BuildWorkloadByInvoicesReport(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        throw new NotImplementedException();

    //        var sent = Exp.Sum(Exp.If(!Exp.Eq("i.status", (int)InvoiceStatus.Draft) & (timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("i.issue_date", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate))), 1, 0));
    //        var paid = Exp.Sum(Exp.If(Exp.Eq("i.status", (int)InvoiceStatus.Paid) & (timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("i.last_modifed_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate))), 1, 0));
    //        var rejected = Exp.Sum(Exp.If(Exp.Eq("i.status", (int)InvoiceStatus.Rejected) & (timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("i.last_modifed_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate))), 1, 0));
    //        var overdue = Exp.Sum(Exp.If((timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("i.due_date", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate))) & Exp.Or(Exp.Eq("i.status", (int)InvoiceStatus.Sent) & Exp.Lt("i.due_date", _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())), Exp.Eq("i.status", (int)InvoiceStatus.Paid) & Exp.Sql("i.last_modifed_on > i.due_date")), 1, 0));

    //        var sqlQuery = Query("crm_invoice i")
    //            .Select("i.create_by", "concat(u.firstname, ' ', u.lastname) as full_name")
    //            .Select(sent)
    //            .Select(paid)
    //            .Select(rejected)
    //            .Select(overdue)
    //            .LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "i.tenant_id") & Exp.EqColumns("u.id", "i.create_by"))
    //            .Where(managers != null && managers.Any() ? Exp.In("i.create_by", managers) : Exp.Empty)
    //            .GroupBy("i.create_by");


    //        return Db.ExecuteList(sqlQuery).ConvertAll(ToWorkloadByInvoices);
    //    }

    //    private WorkloadByInvoices ToWorkloadByInvoices(object[] row)
    //    {
    //        return new WorkloadByInvoices
    //        {
    //            UserId = string.IsNullOrEmpty(Convert.ToString(row[0])) ? Guid.Empty : new Guid(Convert.ToString(row[0])),
    //            UserName = Convert.ToString(row[1]),
    //            SentCount = Convert.ToInt32(row[2]),
    //            PaidCount = Convert.ToInt32(row[3]),
    //            RejectedCount = Convert.ToInt32(row[4]),
    //            OverdueCount = Convert.ToInt32(row[5])
    //        };
    //    }

    //    private object GenerateReportData(ReportTimePeriod timePeriod, List<WorkloadByInvoices> reportData)
    //    {
    //        return new
    //        {
    //            resource = new
    //            {
    //                header = CRMReportResource.WorkloadByInvoicesReport,
    //                sheetName = CRMReportResource.WorkloadByInvoicesReport,
    //                dateRangeLabel = CRMReportResource.TimePeriod + ":",
    //                dateRangeValue = GetTimePeriodText(timePeriod),

    //                chartName = CRMReportResource.BilledInvoices,
    //                chartName1 = CRMInvoiceResource.Invoices,

    //                header1 = CRMInvoiceResource.Invoices,

    //                manager = CRMReportResource.Manager,
    //                total = CRMReportResource.Total,

    //                billed = CRMReportResource.Billed,
    //                paid = CRMReportResource.Paid,
    //                rejected = CRMReportResource.Rejected,
    //                overdue = CRMReportResource.Overdue
    //            },
    //            data = reportData
    //        };
    //    }

    //    #endregion


    //    #region GetWorkloadByViopReport

    //    public bool CheckWorkloadByViopReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        return Query(CRMDbContext.VoipCalls)
    //                    .Where(x => x.ParentCallId == "")
    //                    .Where(x => managers != null && managers.Any() ? managers.ToList().Contains(x.AnsweredBy) : true)
    //                    .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ?
    //                                true :
    //                                x.DialDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DialDate <= _tenantUtil.DateTimeToUtc(toDate))
    //                    .Any();
    //    }

    //    public object GetWorkloadByViopReportData(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        var reportData = BuildWorkloadByViopReport(timePeriod, managers);

    //        return reportData == null || !reportData.Any() ? null : GenerateReportData(timePeriod, reportData);
    //    }

    //    private List<WorkloadByViop> BuildWorkloadByViopReport(ReportTimePeriod timePeriod, Guid[] managers)
    //    {
    //        DateTime fromDate;
    //        DateTime toDate;

    //        GetTimePeriod(timePeriod, out fromDate, out toDate);

    //        var sqlQuery = Query("crm_voip_calls c")
    //            .Select("c.answered_by",
    //                    "concat(u.firstname, ' ', u.lastname) as full_name",
    //                    "c.status",
    //                    "count(c.id) as calls_count",
    //                    "sum(c.dial_duration) as duration")
    //            .LeftOuterJoin("core_user u", Exp.EqColumns("u.tenant", "c.tenant_id") & Exp.EqColumns("u.id", "c.answered_by"))
    //            .Where(Exp.EqColumns("c.parent_call_id", "''"))
    //            .Where(managers != null && managers.Any() ? Exp.In("c.answered_by", managers) : Exp.Empty)
    //            .Where(timePeriod == ReportTimePeriod.DuringAllTime ?
    //                    Exp.Empty :
    //                    Exp.Between("c.dial_date", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
    //            .GroupBy("c.answered_by", "c.status");


    //        return Db.ExecuteList(sqlQuery).ConvertAll(ToWorkloadByViop);
    //    }

    //    private WorkloadByViop ToWorkloadByViop(object[] row)
    //    {
    //        return new WorkloadByViop
    //        {
    //            UserId = string.IsNullOrEmpty(Convert.ToString(row[0])) ? Guid.Empty : new Guid(Convert.ToString(row[0])),
    //            UserName = Convert.ToString(row[1] ?? string.Empty),
    //            Status = (VoipCallStatus)Convert.ToInt32(row[2] ?? 0),
    //            Count = Convert.ToInt32(row[3]),
    //            Duration = Convert.ToInt32(row[4])
    //        };
    //    }

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

            return string.Format("{0}:{1}:{2}",
                ((timeSpan.TotalHours < 10 ? "0" : "") + (int)timeSpan.TotalHours),
                ((timeSpan.Minutes < 10 ? "0" : "") + timeSpan.Minutes),
                ((timeSpan.Seconds < 10 ? "0" : "") + timeSpan.Seconds));
        }

        #endregion


        #region SummaryForThePeriodReport

        public bool CheckSummaryForThePeriodReportData(ReportTimePeriod timePeriod, Guid[] managers)
        {
            DateTime fromDate;
            DateTime toDate;

            GetTimePeriod(timePeriod, out fromDate, out toDate);

            var newDealsSqlQuery = Query(CRMDbContext.Deals)
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                        .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Any();

            var closedDealsSqlQuery = Query(CRMDbContext.Deals)
                            .Join(Query(CRMDbContext.DealMilestones),
                                  x => x.DealMilestoneId,
                                  y => y.Id,
                                  (x, y) => new { x, y })
                            .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                            .Where(x => x.x.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                            .Where(x => x.y.Status != DealMilestoneStatus.Open)
                            .Any();

            var overdueDealsSqlQuery = Query(CRMDbContext.Deals)
                            .Join(Query(CRMDbContext.DealMilestones),
                                  x => x.DealMilestoneId,
                                  y => y.Id,
                                  (x, y) => new { x, y })
                            .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                            .Where(x => x.x.ExpectedCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.ExpectedCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                            .Where(x => (x.y.Status == DealMilestoneStatus.Open && x.x.ExpectedCloseDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ||
                                        (x.y.Status == DealMilestoneStatus.ClosedAndWon && x.x.ActualCloseDate > x.x.ExpectedCloseDate))
                            .Any();

            var invoicesSqlQuery = Query(CRMDbContext.Invoices)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                      .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();

            var contactsSqlQuery = Query(CRMDbContext.Contacts)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                      .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();


            var tasksSqlQuery = Query(CRMDbContext.Tasks)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                      .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();


            var voipSqlQuery = Query(CRMDbContext.VoipCalls)
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

            var newDealsSqlQuery = Query(CRMDbContext.Deals)
                                           .Join(Query(CRMDbContext.CurrencyRate),
                                                   x => x.BidCurrency,
                                                   y => y.FromCurrency,
                                                   (x, y) => new { Deal = x, CurrencyRate = y })
                                           .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                           .Where(x => x.Deal.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                           .GroupBy(x => x.Deal.Id)
                                           .Select(x => new
                                           {
                                               count = x.Count(),
                                               deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                   x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : x.Deal.PerPeriodValue) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                           }).ToList();

            var wonDealsSqlQuery = Query(CRMDbContext.Deals)
                                           .Join(Query(CRMDbContext.CurrencyRate),
                                                 x => x.BidCurrency,
                                                 y => y.FromCurrency,
                                                 (x, y) => new { Deal = x, CurrencyRate = y })
                                           .Join(Query(CRMDbContext.DealMilestones),
                                                 x => x.Deal.DealMilestoneId,
                                                 y => y.Id,
                                                 (x, y) => new { Deal = x.Deal, CurrencyRate = x.CurrencyRate, DealMilestone = y })
                                          .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                                          .Where(x => x.Deal.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                                          .Where(x => x.DealMilestone.Status == DealMilestoneStatus.ClosedAndWon)
                                          .GroupBy(x => x.Deal.Id)
                                          .Select(x => new
                                          {
                                              count = x.Count(),
                                              deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                                  x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : x.Deal.PerPeriodValue) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                                          }).ToList();

            var lostDealsSqlQuery = Query(CRMDbContext.Deals)
                               .Join(Query(CRMDbContext.CurrencyRate),
                                     x => x.BidCurrency,
                                     y => y.FromCurrency,
                                     (x, y) => new { Deal = x, CurrencyRate = y })
                               .Join(Query(CRMDbContext.DealMilestones),
                                     x => x.Deal.DealMilestoneId,
                                     y => y.Id,
                                     (x, y) => new { Deal = x.Deal, CurrencyRate = x.CurrencyRate, DealMilestone = y })
                              .Where(x => managers != null && managers.Any() ? managers.Contains(x.Deal.ResponsibleId) : true)
                              .Where(x => x.Deal.ActualCloseDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.Deal.ActualCloseDate <= _tenantUtil.DateTimeToUtc(toDate))
                              .Where(x => x.DealMilestone.Status == DealMilestoneStatus.ClosedAndLost)
                              .GroupBy(x => x.Deal.Id)
                              .Select(x => new
                              {
                                  count = x.Count(),
                                  deals_value = x.Sum(x => x.Deal.BidType == 0 ? x.Deal.BidValue * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate) :
                                                                      x.Deal.BidValue * (x.Deal.PerPeriodValue == 0 ? 1.0m : x.Deal.PerPeriodValue) * (x.Deal.BidCurrency == defaultCurrency ? 1.0m : x.CurrencyRate.Rate))
                              }).ToList();


            var overdueDealsSqlQuery = Query(CRMDbContext.Deals)
                               .Join(Query(CRMDbContext.CurrencyRate),
                                     x => x.BidCurrency,
                                     y => y.FromCurrency,
                                     (x, y) => new { Deal = x, CurrencyRate = y })
                               .Join(Query(CRMDbContext.DealMilestones),
                                     x => x.Deal.DealMilestoneId,
                                     y => y.Id,
                                     (x, y) => new { Deal = x.Deal, CurrencyRate = x.CurrencyRate, DealMilestone = y })
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

            var invoicesSqlQuery = Query(CRMDbContext.Invoices)
                                  .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                  .Where(x => x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                  .GroupBy(x => x.Status)
                                  .Select(x => new {
                                     sent = x.Sum(x => x.Status != InvoiceStatus.Draft ? 1 :0),
                                     paid = x.Sum(x => x.Status == InvoiceStatus.Paid ? 1 : 0),
                                     rejected = x.Sum(x => x.Status == InvoiceStatus.Rejected ? 1 : 0),
                                     overdue = x.Sum(x => (x.Status == InvoiceStatus.Sent && x.DueDate < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) ||
                                                          (x.Status ==  InvoiceStatus.Paid && x.LastModifedOn > x.DueDate)      
                                     ? 1 : 0)
                                  }).ToList();

            var contactsSqlQuery = Query(CRMDbContext.Contacts)
                                  .Join(Query(CRMDbContext.ListItem).DefaultIfEmpty(),
                                      x => x.ContactTypeId,
                                      y => y.Id,
                                      (x, y) => new { Contact = x, ListItem = y })
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





            var tasksSqlQuery = Query(CRMDbContext.Tasks)
                                .Join(Query(CRMDbContext.ListItem).DefaultIfEmpty(), 
                                      x => x.CategoryId,
                                      y => y.Id,
                                      (x,y) => new { Task = x, ListItem = y })
                              .Where(x => x.ListItem.ListType == ListType.TaskCategory)
                              .Where(x => managers != null && managers.Any() ? managers.Contains(x.Task.ResponsibleId) : true)
                              .Where(x => x.Task.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                              .GroupBy(x => new { x.ListItem.Title })
                              .Select(x => new
                              {

                                  title = x.Key.Title,
                                  sum1 = x.Sum(x => x.Task.IsClosed && x.Task.LastModifedOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.Task.LastModifedOn <= _tenantUtil.DateTimeToUtc(toDate) ? 1 : 0),
                                  sum2 = x.Sum(x => (!x.Task.IsClosed && x.Task.Deadline < _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())) || (x.Task.IsClosed && x.Task.LastModifedOn > x.Task.Deadline) ? 1 :0),
                                  count = x.Count()
                              })
                              .OrderBy(x => x.title)
                              .ToList();
//                .OrderBy("i.sort_order, i.title", true);




            var voipSqlQuery = Query(CRMDbContext.VoipCalls)
                                .Where(x => String.IsNullOrEmpty(x.ParentCallId))
                                .Where(x => managers != null && managers.Any() ? managers.Contains(x.AnsweredBy) : true)
                                .Where(x => x.DialDate >= _tenantUtil.DateTimeToUtc(fromDate) && x.DialDate <= _tenantUtil.DateTimeToUtc(toDate))
                                .GroupBy(x => x.Status)
                                .Select(x => new {
                                    status = x.Key,
                                    calls_count = x.Count(),
                                    duration = x.Sum(x => x.DialDuration)
                                })
                                .ToList();

            object res;
                              
            using (var tx = CRMDbContext.Database.BeginTransaction())
            {
                res = new
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

                tx.Commit();
            }

            return res;
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

            var dealsSqlQuery = Query(CRMDbContext.Deals)
                                        .Join(CRMDbContext.DealMilestones,
                                              x => x.DealMilestoneId,
                                              y => y.Id,
                                              (x, y) => new { x, y })
                                        .Where(x => managers != null && managers.Any() ? managers.Contains(x.x.ResponsibleId) : true)
                                        .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                        .Where(x => x.y.Status == DealMilestoneStatus.Open)
                                        .Any();


            var contactsSqlQuery = Query(CRMDbContext.Contacts)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.CreateBy) : true)
                                      .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();

            var tasksSqlQuery = Query(CRMDbContext.Tasks)
                                      .Where(x => managers != null && managers.Any() ? managers.Contains(x.ResponsibleId) : true)
                                      .Where(x => timePeriod == ReportTimePeriod.DuringAllTime ? true : x.CreateOn >= _tenantUtil.DateTimeToUtc(fromDate) && x.CreateOn <= _tenantUtil.DateTimeToUtc(toDate))
                                      .Any();

            var invoicesSqlQuery = Query(CRMDbContext.Invoices)
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

            throw new NotImplementedException();

            //var openDealsSqlQuery = Query("crm_deal d")
            //    .Select("count(d.id) as count",
            //            string.Format(@"sum(case d.bid_type
            //                            when 0 then
            //                                d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
            //                            else
            //                                d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
            //                            end) as deals_value", defaultCurrency))
            //    .LeftOuterJoin("crm_currency_rate r",
            //                   Exp.EqColumns("r.tenant_id", "d.tenant_id") &
            //                   Exp.EqColumns("r.from_currency", "d.bid_currency"))
            //    .LeftOuterJoin("crm_deal_milestone m",
            //                   Exp.EqColumns("m.tenant_id", "d.tenant_id") &
            //                   Exp.EqColumns("m.id", "d.deal_milestone_id"))
            //    .Where(managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty)
            //    .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("d.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .Where("m.status", (int)DealMilestoneStatus.Open);

            //var overdueDealsSqlQuery = Query("crm_deal d")
            //    .Select("count(d.id) as count",
            //            string.Format(@"sum(case d.bid_type
            //                            when 0 then
            //                                d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
            //                            else
            //                                d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
            //                            end) as deals_value", defaultCurrency))
            //    .LeftOuterJoin("crm_currency_rate r",
            //                   Exp.EqColumns("r.tenant_id", "d.tenant_id") &
            //                   Exp.EqColumns("r.from_currency", "d.bid_currency"))
            //    .LeftOuterJoin("crm_deal_milestone m",
            //                   Exp.EqColumns("m.tenant_id", "d.tenant_id") &
            //                   Exp.EqColumns("m.id", "d.deal_milestone_id"))
            //    .Where(managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty)
            //    .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("d.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .Where("m.status", (int)DealMilestoneStatus.Open)
            //    .Where(Exp.Lt("d.expected_close_date", _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())));

            //var nearDealsSqlQuery = Query("crm_deal d")
            //    .Select("count(d.id) as count",
            //            string.Format(@"sum(case d.bid_type
            //                            when 0 then
            //                                d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
            //                            else
            //                                d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
            //                            end) as deals_value", defaultCurrency))
            //    .LeftOuterJoin("crm_currency_rate r",
            //                   Exp.EqColumns("r.tenant_id", "d.tenant_id") &
            //                   Exp.EqColumns("r.from_currency", "d.bid_currency"))
            //    .LeftOuterJoin("crm_deal_milestone m",
            //                   Exp.EqColumns("m.tenant_id", "d.tenant_id") &
            //                   Exp.EqColumns("m.id", "d.deal_milestone_id"))
            //    .Where(managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty)
            //    .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("d.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .Where("m.status", (int)DealMilestoneStatus.Open)
            //    .Where(Exp.Between("d.expected_close_date", _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()), _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow().AddDays(30))));

            //var dealsByStageSqlQuery = Query("crm_deal_milestone m")
            //    .Select("m.title",
            //            "count(d.id) as deals_count",
            //            string.Format(@"sum(case d.bid_type
            //            when 0 then
            //                d.bid_value * (if(d.bid_currency = '{0}', 1, r.rate))
            //            else
            //                d.bid_value * (if(d.per_period_value = 0, 1, d.per_period_value)) * (if(d.bid_currency = '{0}', 1, r.rate))
            //            end) as deals_value", defaultCurrency))
            //    .LeftOuterJoin("crm_deal d", Exp.EqColumns("d.tenant_id", "m.tenant_id") &
            //        Exp.EqColumns("d.deal_milestone_id", "m.id") &
            //        (managers != null && managers.Any() ? Exp.In("d.responsible_id", managers) : Exp.Empty) &
            //        (timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("d.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate))))
            //    .LeftOuterJoin("crm_currency_rate r", Exp.EqColumns("r.tenant_id", "m.tenant_id") & Exp.EqColumns("r.from_currency", "d.bid_currency"))
            //    .Where("m.status", (int)DealMilestoneStatus.Open)
            //    .GroupBy("m.id")
            //    .OrderBy("m.sort_order, m.title", true);

            //var contactsByTypeSqlQuery = Query("crm_contact c")
            //    .Select("i.title",
            //            "count(c.id) as count")
            //    .LeftOuterJoin("crm_list_item i", Exp.EqColumns("i.tenant_id", "c.tenant_id") &
            //                                      Exp.EqColumns("i.id", "c.contact_type_id") &
            //                                      Exp.Eq("i.list_type", (int)ListType.ContactType))
            //    .Where(managers != null && managers.Any() ? Exp.In("c.create_by", managers) : Exp.Empty)
            //    .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("c.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .GroupBy("i.id")
            //    .OrderBy("i.sort_order, i.title", true);

            //var contactsByStageSqlQuery = Query("crm_contact c")
            //    .Select("i.title",
            //            "count(c.id) as count")
            //    .LeftOuterJoin("crm_list_item i", Exp.EqColumns("i.tenant_id", "c.tenant_id") &
            //                                      Exp.EqColumns("i.id", "c.status_id") &
            //                                      Exp.Eq("i.list_type", (int)ListType.ContactStatus))
            //    .Where(managers != null && managers.Any() ? Exp.In("c.create_by", managers) : Exp.Empty)
            //    .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("c.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .GroupBy("i.id")
            //    .OrderBy("i.sort_order, i.title", true);

            //var tasksSqlQuery = Query("crm_task t")
            //    .Select("i.title")
            //    .Select(Exp.Sum(Exp.If(Exp.Eq("t.is_closed", 0), 1, 0)))
            //    .Select(Exp.Sum(Exp.If(Exp.Eq("t.is_closed", 0) & Exp.Lt("t.deadline", _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())), 1, 0)))
            //    .LeftOuterJoin("crm_list_item i", Exp.EqColumns("i.tenant_id", "t.tenant_id") & Exp.EqColumns("i.id", "t.category_id") & Exp.Eq("i.list_type", (int)ListType.TaskCategory))
            //    .Where(managers != null && managers.Any() ? Exp.In("t.responsible_id", managers) : Exp.Empty)
            //    .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("t.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)))
            //    .GroupBy("i.id")
            //    .OrderBy("i.sort_order, i.title", true);

            //var sent = Exp.Sum(Exp.If(Exp.Eq("i.status", (int)InvoiceStatus.Sent), 1, 0));
            //var overdue = Exp.Sum(Exp.If(Exp.Eq("i.status", (int)InvoiceStatus.Sent) & Exp.Lt("i.due_date", _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow())), 1, 0));

            //var invoicesSqlQuery = Query("crm_invoice i")
            //    .Select(sent)
            //    .Select(overdue)
            //    .Where(managers != null && managers.Any() ? Exp.In("i.create_by", managers) : Exp.Empty)
            //    .Where(timePeriod == ReportTimePeriod.DuringAllTime ? Exp.Empty : Exp.Between("i.create_on", _tenantUtil.DateTimeToUtc(fromDate), _tenantUtil.DateTimeToUtc(toDate)));

            //object res;

            //using (var tx = Db.BeginTransaction())
            //{
            //    res = new
            //    {
            //        DealsInfo = new
            //        {
            //            Open = Db.ExecuteList(openDealsSqlQuery),
            //            Overdue = Db.ExecuteList(overdueDealsSqlQuery),
            //            Near = Db.ExecuteList(nearDealsSqlQuery),
            //            ByStage = Db.ExecuteList(dealsByStageSqlQuery)
            //        },
            //        ContactsInfo = new
            //        {
            //            ByType = Db.ExecuteList(contactsByTypeSqlQuery),
            //            ByStage = Db.ExecuteList(contactsByStageSqlQuery)
            //        },
            //        TasksInfo = Db.ExecuteList(tasksSqlQuery),
            //        InvoicesInfo = Db.ExecuteList(invoicesSqlQuery),
            //    };

            //    tx.Commit();
            //}

            //return res;
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