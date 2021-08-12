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
using System.Globalization;

using ASC.Common;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Web.Core;
using ASC.Web.Core.Calendars;

namespace ASC.CRM.Api
{
    [Scope]
    public sealed class CrmCalendar : BaseCalendar
    {
        [AllDayLongUTC]
        private class Event : BaseEvent
        {
        }

        private WebItemSecurity _webItemSecurity;
        private DaoFactory _daoFactory;
        private TenantManager _tenantManager;
        private TenantUtil _tenantUtil;

        public CrmCalendar(WebItemSecurity webItemSecurity,
                           DaoFactory daoFactory,
                           AuthContext authContext,
                           TimeZoneConverter timeZoneConverter,
                           TenantManager tenantManager,
                           TenantUtil tenantUtil
                           ) : base(authContext, timeZoneConverter)
        {
            _tenantUtil = tenantUtil;
            _tenantManager = tenantManager;
            _webItemSecurity = webItemSecurity;
            _daoFactory = daoFactory;

            Context.HtmlBackgroundColor = "";
            Context.HtmlTextColor = "";
            Context.CanChangeAlertType = false;
            Context.CanChangeTimeZone = false;
            Context.GetGroupMethod = () => CRMCommonResource.ProductName;
            Id = "crm_calendar";
            EventAlertType = EventAlertType.Never;
            Name = CRMCommonResource.ProductName;
            Description = "";
            SharingOptions = new SharingOptions();
            //            SharingOptions.PublicItems.Add(new SharingOptions.PublicItem { Id = userId, IsGroup = false });
        }
        public override List<ITodo> LoadTodos(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            return new List<ITodo>();
        }
        public override List<IEvent> LoadEvents(Guid userId, DateTime startDate, DateTime endDate)
        {
            var events = new List<IEvent>();

            if (
                !_webItemSecurity.IsAvailableForMe(WebItemManager.CRMProductID))
            {
                return events;
            }

            var tasks = _daoFactory.GetTaskDao().GetTasks(String.Empty, userId, 0, false, DateTime.MinValue,
                DateTime.MinValue, EntityType.Any, 0, 0, 0, null);

            foreach (var t in tasks)
            {
                if (t.DeadLine == DateTime.MinValue) continue;

                var allDayEvent = t.DeadLine.Hour == 0 && t.DeadLine.Minute == 0;
                var utcDate = allDayEvent ? t.DeadLine.Date : _tenantUtil.DateTimeToUtc(t.DeadLine);

                var e = new Event
                {
                    AlertType = EventAlertType.Never,
                    AllDayLong = allDayEvent,
                    CalendarId = Id,
                    UtcStartDate = utcDate,
                    UtcEndDate = utcDate,
                    Id = "crm_task_" + t.ID.ToString(CultureInfo.InvariantCulture),
                    Name = CRMCommonResource.ProductName + ": " + t.Title,
                    Description = t.Description
                };

                if (IsVisibleEvent(startDate, endDate, e.UtcStartDate, e.UtcEndDate))
                    events.Add(e);
            }

            return events;

        }

        public override TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(_tenantManager.GetCurrentTenant().TimeZone); }
        }

        private bool IsVisibleEvent(DateTime startDate, DateTime endDate, DateTime eventStartDate, DateTime eventEndDate)
        {
            return (startDate <= eventStartDate && eventStartDate <= endDate) ||
                   (startDate <= eventEndDate && eventEndDate <= endDate) ||
                   (eventStartDate < startDate && eventEndDate > endDate);
        }
    }
}