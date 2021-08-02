/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;

#endregion

namespace ASC.Projects.Core.Domain.Reports
{
    public class ReportFilter
    {
        public ReportTimeInterval TimeInterval { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<int> ProjectIds { get; set; }
        public int TagId { get; set; }
        public string ProjectTag { get; set; }
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public List<ProjectStatus> ProjectStatuses { get; set; }
        public List<MilestoneStatus> MilestoneStatuses { get; set; }
        public MessageStatus? MessageStatus { get; set; }
        public List<TaskStatus> TaskStatuses { get; set; }
        public List<PaymentStatus> PaymentStatuses { get; set; }
        public int ViewType { get; set; }
        public bool NoResponsible { get; set; }

        public bool HasUserId
        {
            get { return UserId != default(Guid) || DepartmentId != default(Guid); }
        }

        public bool HasProjectIds
        {
            get { return 0 < ProjectIds.Count; }
        }

        public bool HasProjectStatuses
        {
            get { return 0 < ProjectStatuses.Count; }
        }

        public bool HasMilestoneStatuses
        {
            get { return 0 < MilestoneStatuses.Count; }
        }

        public virtual bool HasTaskStatuses
        {
            get { return 0 < TaskStatuses.Count && Substatus.HasValue; }
        }

        public int? Substatus { get; set; }

        public ReportFilter()
        {
            ToDate = DateTime.MaxValue;
            ProjectIds = new List<int>();
            ProjectStatuses = new List<ProjectStatus>();
            MilestoneStatuses = new List<MilestoneStatus>();
            TaskStatuses = new List<TaskStatus>();
            PaymentStatuses = new List<PaymentStatus>();
        }

        public void SetProjectIds(IEnumerable<int> ids)
        {
            ProjectIds.Clear();
            if (ids != null && ids.Any()) ProjectIds.AddRange(ids);
            else ProjectIds.Add(-1);
        }
    }

    [Scope]
    public class FilterHelper
    {
        public UserManager UserManager { get; set; }
        public TenantUtil TenantUtil { get; set; }
        public TenantManager TenantManager { get; set; }

        public FilterHelper(UserManager userManager, TenantUtil  tenantUtil, TenantManager tenantManager)
        {
            UserManager = userManager;
            TenantUtil = tenantUtil;
            TenantManager = tenantManager;
        }

        public List<string> GetUserIds(ReportFilter reportFilter)
        {
            var result = new List<string>();
            if (reportFilter.UserId != Guid.Empty)
            {
                result.Add(reportFilter.UserId.ToString());
            }
            else if (reportFilter.DepartmentId != Guid.Empty)
            {
                result.AddRange(UserManager.GetUsersByGroup(reportFilter.DepartmentId).Select(u => u.ID.ToString()));
            }
            return result;
        }

        public DateTime GetFromDate(ReportFilter reportFilter, bool toUtc)
        {
            var date = DateTime.MinValue;
            switch (reportFilter.TimeInterval)
            {
                case ReportTimeInterval.Absolute:
                    date = reportFilter.FromDate;
                    break;
                case ReportTimeInterval.Relative:
                    if (reportFilter.FromDate != DateTime.MinValue && reportFilter.FromDate != DateTime.MaxValue)
                    {
                        date = TenantUtil.DateTimeNow();
                    }
                    break;
                //Hack for Russian Standard Time
                case ReportTimeInterval.CurrYear:
                    date = GetDate(reportFilter, true);
                    date = date.AddHours(1);
                    break;
                default:
                    date = GetDate(reportFilter, true);
                    break;
            }
            if (date != DateTime.MinValue && date != DateTime.MaxValue && reportFilter.TimeInterval != ReportTimeInterval.CurrYear)
            {
                date = date.Date;
                if (toUtc) date = TenantUtil.DateTimeToUtc(date);
            }
            return date;
        }

        private DateTime GetDate(ReportFilter reportFilter, bool start)
        {
            var date = TenantUtil.DateTimeNow();

            if (reportFilter.TimeInterval == ReportTimeInterval.Today)
            {
                return date;
            }
            if (reportFilter.TimeInterval == ReportTimeInterval.Yesterday)
            {
                return date.AddDays(-1);
            }
            if (reportFilter.TimeInterval == ReportTimeInterval.Tomorrow)
            {
                return date.AddDays(1);
            }
            if (reportFilter.TimeInterval == ReportTimeInterval.CurrWeek || reportFilter.TimeInterval == ReportTimeInterval.NextWeek || reportFilter.TimeInterval == ReportTimeInterval.PrevWeek)
            {
                var diff = TenantManager.GetCurrentTenant().GetCulture().DateTimeFormat.FirstDayOfWeek - date.DayOfWeek;
                if (0 < diff) diff -= 7;
                date = date.AddDays(diff);
                if (reportFilter.TimeInterval == ReportTimeInterval.NextWeek) date = date.AddDays(7);
                if (reportFilter.TimeInterval == ReportTimeInterval.PrevWeek) date = date.AddDays(-7);
                if (!start) date = date.AddDays(7).AddDays(-1);
                return date;
            }
            if (reportFilter.TimeInterval == ReportTimeInterval.CurrMonth || reportFilter.TimeInterval == ReportTimeInterval.NextMonth || reportFilter.TimeInterval == ReportTimeInterval.PrevMonth)
            {
                date = new DateTime(date.Year, date.Month, 1);
                if (reportFilter.TimeInterval == ReportTimeInterval.NextMonth) date = date.AddMonths(1);
                if (reportFilter.TimeInterval == ReportTimeInterval.PrevMonth) date = date.AddMonths(-1);
                if (!start) date = date.AddMonths(1).AddDays(-1);
                return date;
            }
            if (reportFilter.TimeInterval == ReportTimeInterval.CurrYear || reportFilter.TimeInterval == ReportTimeInterval.NextYear || reportFilter.TimeInterval == ReportTimeInterval.PrevYear)
            {
                date = new DateTime(date.Year, 1, 1);
                if (reportFilter.TimeInterval == ReportTimeInterval.NextYear) date = date.AddYears(1);
                if (reportFilter.TimeInterval == ReportTimeInterval.PrevYear) date = date.AddYears(-1);
                if (!start) date = date.AddYears(1).AddDays(-1);
                return date;
            }
            throw new ArgumentOutOfRangeException(string.Format("TimeInterval"));
        }

        public DateTime GetToDate(ReportFilter reportFilter, bool toUtc)
        {
            var date = DateTime.MaxValue;
            switch (reportFilter.TimeInterval)
            {
                case ReportTimeInterval.Absolute:
                    date = reportFilter.ToDate;
                    break;
                case ReportTimeInterval.Relative:
                    if (reportFilter.FromDate != DateTime.MinValue && reportFilter.FromDate != DateTime.MaxValue && reportFilter.ToDate != DateTime.MinValue && reportFilter.ToDate != DateTime.MaxValue)
                    {
                        date = TenantUtil.DateTimeNow().Add(reportFilter.ToDate - reportFilter.FromDate);
                    }
                    break;
                default:
                    date = GetDate(reportFilter, false);
                    break;
            }
            if (date != DateTime.MinValue && date != DateTime.MaxValue)
            {
                date = date.Date.AddTicks(TimeSpan.TicksPerDay - 1);
                if (toUtc) date = TenantUtil.DateTimeToUtc(date);
            }
            return date;
        }
        public DateTime GetFromDate(ReportFilter reportFilter)
        {
            return GetFromDate(reportFilter, false);
        }

        public DateTime GetToDate(ReportFilter reportFilter)
        {
            return GetToDate(reportFilter, false);
        }
    }
}
