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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ASC.Api.Core;
using ASC.Calendar.Core.Dao;
using ASC.Calendar.Core.Dao.Models;
using ASC.Calendar.ExternalCalendars;
using ASC.Calendar.iCalParser;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.EF;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core.Calendars;
using ASC.Web.Core.WhiteLabel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Calendar.BusinessObjects
{
    public class DataProvider
    {
        /* private IDbManager db;
         private const string DBId = "calendar";
         private const string _calendarTable = "calendar_calendars cal";
         private const string _calendarItemTable = "calendar_calendar_item cal_itm";
         private const string _calendarUserTable = "calendar_calendar_user cal_usr";
         private const string _eventTable = "calendar_events evt";
         private const string _todoTable = "calendar_todos td";
         private const string _eventItemTable = "calendar_event_item evt_itm";*/

        public CalendarDbContext CalendarDb { get; }
        public int Tenant
        {
            get
            {
                return ApiContext.Tenant.TenantId;
            }
        }

        public ApiContext ApiContext { get; }
        public string UserId
        {
            get
            {
                return SecurityContext.CurrentAccount.ID.ToString();
            }
        }
        private AuthManager Authentication { get; }
        public SecurityContext SecurityContext { get; }
        public AuthContext AuthContext { get; }
        public TimeZoneConverter TimeZoneConverter { get; }
        public EventHistoryHelper EventHistoryHelper { get; }
        private TenantManager TenantManager { get; }
        protected UserManager UserManager { get; }
        protected DDayICalParser DDayICalParser { get; }

        public ILog Log { get; }

        public DataProvider(DbContextManager<CalendarDbContext> calendarDbContext,
            AuthManager authentication,
            ApiContext apiContext,
            SecurityContext securityContext,
            AuthContext authContext,
            TimeZoneConverter timeZoneConverter,
            TenantManager tenantManager,
            EventHistoryHelper eventHistoryHelper,
            UserManager userManager,
            DDayICalParser dDayICalParser,
            IOptionsMonitor<ILog> option)
        {
            Authentication = authentication;
            CalendarDb = calendarDbContext.Get("calendar");
            ApiContext = apiContext;
            SecurityContext = securityContext;
            AuthContext = authContext;
            TimeZoneConverter = timeZoneConverter;
            TenantManager = tenantManager;
            EventHistoryHelper = eventHistoryHelper;
            UserManager = userManager;
            DDayICalParser = dDayICalParser;
            Log = option.Get("ASC.CalendarDataProvider");

        }

        public List<UserViewSettings> GetUserViewSettings(Guid userId, List<string> calendarIds)
        {
            var data = CalendarDb.CalendarCalendarUser
                .Where(ccu =>
                        (calendarIds.Contains(ccu.CalendarId.ToString()) || calendarIds.Contains(ccu.ExtCalendarId)) &&
                        ccu.UserId == userId)
                .ToList();

            var options = new List<UserViewSettings>();
            foreach (var r in data)
            {
                options.Add(new UserViewSettings()
                {
                    CalendarId =
                            Convert.ToInt32(r.CalendarId) == 0
                                ? Convert.ToString(r.ExtCalendarId)
                                : Convert.ToString(r.CalendarId),
                    UserId = r.UserId,
                    IsHideEvents = Convert.ToBoolean(r.HideEvents),
                    IsAccepted = Convert.ToBoolean(r.IsAccepted),
                    TextColor = r.TextColor,
                    BackgroundColor = r.BackgroundColor,
                    EventAlertType = (EventAlertType)r.AlertType,
                    Name = r.Name,
                    TimeZone = TimeZoneConverter.GetTimeZone(r.TimeZone)
                });
            }

            return options;
        }

        public List<Calendar> LoadTodoCalendarsForUser(Guid userId)
        {
            var groups = UserManager.GetUserGroups(userId).Select(g => g.ID).ToList();
            groups.AddRange(
                UserManager.GetUserGroups(userId, Constants.SysGroupCategoryId).Select(g => g.ID));
            var currentTenantId = TenantManager.GetCurrentTenant().TenantId;

            var calIds = CalendarDb.CalendarCalendars.Where(p => p.OwnerId == userId.ToString() && p.IsTodo == 1 && p.Tenant == currentTenantId).Select(s => s.Id).ToArray();

            var cals = GetCalendarsByIds(calIds);

            return cals;
        }

        public void RemoveTodo(int todoId)
        {
            var tenant = TenantManager.GetCurrentTenant().TenantId;
            using var tx = CalendarDb.Database.BeginTransaction();
            var calendarTodo = CalendarDb.CalendarTodos.Where(r => r.Id == todoId && r.Tenant == tenant).SingleOrDefault();

            if (calendarTodo != null)
            {
                CalendarDb.CalendarTodos.Remove(calendarTodo);
            }
            CalendarDb.SaveChanges();
            tx.Commit();
        }
        public List<Calendar> LoadCalendarsForUser(Guid userId, out int newCalendarsCount)
        {
            var groups = UserManager.GetUserGroups(userId).Select(g => g.ID).ToList();
            groups.AddRange(UserManager.GetUserGroups(userId, Constants.SysGroupCategoryId).Select(g => g.ID));

            var currentTenantId = TenantManager.GetCurrentTenant().TenantId;

            var calItemId = from calItem in CalendarDb.CalendarCalendarItem
                            join cal in CalendarDb.CalendarCalendars on calItem.CalendarId equals cal.Id
                            where
                                cal.Tenant == TenantManager.GetCurrentTenant().TenantId &&
                                (calItem.ItemId == userId ||
                                (groups.Contains(calItem.ItemId) && calItem.IsGroup == 1))
                            select calItem.CalendarId;
            var calId = from cal in CalendarDb.CalendarCalendars
                        where
                            cal.OwnerId == userId.ToString() &&
                            cal.Tenant == TenantManager.GetCurrentTenant().TenantId
                        select cal.Id;

            var calIds = calId.Union(calItemId);

            var cals = GetCalendarsByIds(calIds.ToArray());

            //filter by is_accepted field
            newCalendarsCount =
                cals.RemoveAll(
                    c =>
                    (!c.OwnerId.Equals(userId) &&
                        !c.ViewSettings.Exists(v => v.UserId.Equals(userId) && v.IsAccepted))
                    || (c.IsiCalStream() && c.ViewSettings.Exists(v => v.UserId.Equals(userId) && !v.IsAccepted)));
            return cals;

        }

        public List<Calendar> LoadiCalStreamsForUser(Guid userId)
        {
            var calIds = CalendarDb.CalendarCalendars.Where(p => 
                    p.Tenant == TenantManager.GetCurrentTenant().TenantId && 
                    p.OwnerId == userId.ToString() && 
                    p.IcalUrl != null)
                .Select(s => s.Id).ToArray();
            var calendars = GetCalendarsByIds(calIds.ToArray());
            return calendars;
        }

        public List<Calendar> LoadSubscriptionsForUser(Guid userId)
        {
            var groups = UserManager.GetUserGroups(userId).Select(g => g.ID).ToList();
            groups.AddRange(UserManager.GetUserGroups(userId, Constants.SysGroupCategoryId).Select(g => g.ID));

            var calIds = from calItem in CalendarDb.CalendarCalendarItem
                         join calendar in CalendarDb.CalendarCalendars on calItem.CalendarId equals calendar.Id
                         where
                              calendar.Tenant == TenantManager.GetCurrentTenant().TenantId &&
                              (calItem.ItemId == userId || (groups.Contains(calItem.ItemId) && calItem.IsGroup == 1))
                         select calItem.CalendarId;

            var calendars = GetCalendarsByIds(calIds.ToArray());
            return calendars;
        }

        public TimeZoneInfo GetTimeZoneForSharedEventsCalendar(Guid userId)
        {
            var data = CalendarDb.CalendarCalendarUser.Where(p => p.ExtCalendarId == SharedEventsCalendar.CalendarId && p.UserId == userId).Select(s => s.TimeZone).ToList();
            if (data.Count > 0)
                return data.Select(r => TimeZoneConverter.GetTimeZone(Convert.ToString(r[0]))).First();

            return TimeZoneInfo.FindSystemTimeZoneById(TenantManager.GetCurrentTenant().TimeZone);
        }

        public TimeZoneInfo GetTimeZoneForCalendar(Guid userId, int caledarId)
        {

            var data = from cc in CalendarDb.CalendarCalendars
                       join calUsr in CalendarDb.CalendarCalendarUser
                       on cc.Id equals calUsr.CalendarId
                       into UserCalendar
                       from uc in UserCalendar.DefaultIfEmpty()
                       where cc.Id == caledarId
                       select new
                       {
                           calTimeZone = cc.TimeZone,
                           calUsrTimeZone = uc.TimeZone
                       };

            return data.FirstOrDefault().calUsrTimeZone == null ? TimeZoneConverter.GetTimeZone(data.FirstOrDefault().calTimeZone) : TimeZoneConverter.GetTimeZone(data.FirstOrDefault().calUsrTimeZone);

        }

         public List<object[]> GetCalendarIdByCaldavGuid(string caldavGuid)
         {
            var data = CalendarDb.CalendarCalendars
                .Where(p => p.CaldavGuid == caldavGuid)
                .Select(s => new object[]{ 
                    s.Id,
                    s.OwnerId,
                    s.Tenant
                }).ToList();

             return data;
         }
         public Event GetEventIdByUid(string uid, int calendarId)
         {
            var eventId = CalendarDb.CalendarEvents
                .Where(p => 
                    uid.Contains(p.Uid) &&
                    p.CalendarId == calendarId
                 )
                .Select(s => s.Id).FirstOrDefault();

             return eventId == 0 ? null : GetEventById(eventId);
         }
         /*
         public Event GetEventIdOnlyByUid(string uid)
         {
             var sql = new SqlQuery("calendar_events")
                 .Select("id")
                 .Where(Exp.Like("uid", uid));

             var eventId = db.ExecuteScalar<int>(sql);

             return eventId == 0 ? null : GetEventById(eventId);
         }*/
        public List<Calendar> GetCalendarsByIds(int[] calIds)
        {
            var data = from cc in CalendarDb.CalendarCalendars
                       join calUsr in CalendarDb.CalendarCalendarUser
                       on cc.Id equals calUsr.CalendarId
                       into UserCalendar
                       from uc in UserCalendar.DefaultIfEmpty()
                       where calIds.Contains(cc.Id)
                       select new
                       {
                           calId = cc.Id,
                           calName = cc.Name,
                           calDescription = cc.Description,
                           calTenant = cc.Tenant,
                           calTextColor = cc.TextColor,
                           calBackground = cc.BackgroundColor,
                           calOwner = cc.OwnerId,
                           calAlertType = cc.AlertType,
                           calTimeZone = cc.TimeZone,
                           iCalUrl = cc.IcalUrl,
                           calDavGuid = cc.CaldavGuid,
                           isTodo = cc.IsTodo,

                           usrId = uc.UserId,
                           usrHideEvents = uc.HideEvents,
                           usrIsAccepted = uc.IsAccepted,
                           usrTextColor = uc.TextColor,
                           usrBackground = uc.BackgroundColor,
                           usrAlertType = uc.AlertType,
                           usrCalName = uc.Name,
                           usrTimeZone = uc.TimeZone
                       };

            var sharingData = CalendarDb.CalendarCalendarItem.Where(p => calIds.Contains(p.CalendarId)).ToList();

            var calendars = new List<Calendar>();
            foreach (var r in data)
            {
                var calendar =
                    calendars.Find(
                        c =>
                        string.Equals(c.Id, r.calId.ToString(), StringComparison.InvariantCultureIgnoreCase));
                if (calendar == null)
                {
                    var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager);
                    calendar = new Calendar(AuthContext, TimeZoneConverter, icalendar, this)
                    {
                        Id = r.calId.ToString(),
                        Name = r.calName,
                        Description = r.calDescription,
                        TenantId = r.calTenant,
                        OwnerId = Guid.Parse(r.calOwner),
                        EventAlertType = (EventAlertType)r.calAlertType,
                        TimeZone = TimeZoneConverter.GetTimeZone(r.calTimeZone),
                        iCalUrl = r.iCalUrl,
                        calDavGuid = r.calDavGuid,
                        IsTodo = Convert.ToInt32(r.isTodo)
                    };
                    calendar.Context.HtmlTextColor = r.calTextColor;
                    calendar.Context.HtmlBackgroundColor = r.calBackground;
                    if (!String.IsNullOrEmpty(calendar.iCalUrl))
                    {
                        calendar.Context.CanChangeTimeZone = false;
                        calendar.Context.CanChangeAlertType = false;
                    }

                    calendars.Add(calendar);

                    foreach (var row in sharingData)
                    {
                        var _calId = row.CalendarId.ToString();
                        if (String.Equals(_calId, calendar.Id, StringComparison.InvariantCultureIgnoreCase))
                        {
                            calendar.SharingOptions.PublicItems.Add(new SharingOptions.PublicItem
                            {
                                Id = row.ItemId,
                                IsGroup = Convert.ToBoolean(row.IsGroup)
                            });
                        }
                    }
                }
                if (r.usrId != null)
                {
                    var uvs = new UserViewSettings
                    {
                        CalendarId = calendar.Id,
                        UserId = r.usrId,
                        IsHideEvents = Convert.ToBoolean(r.usrHideEvents),
                        IsAccepted = Convert.ToBoolean(r.usrIsAccepted),
                        TextColor = r.usrTextColor,
                        BackgroundColor = r.usrBackground,
                        EventAlertType = (EventAlertType)r.usrAlertType,
                        Name = r.usrCalName,
                        TimeZone = TimeZoneConverter.GetTimeZone(r.usrTimeZone)
                    };

                    calendar.ViewSettings.Add(uvs);
                }
            }
            return calendars;
        }
        public Calendar GetCalendarById(int calendarId)
        {
            var calendars = GetCalendarsByIds(new int[] { calendarId });
            if (calendars.Count > 0)
                return calendars[0];

            return null;
        }
        public Calendar CreateCalendar(Guid ownerId, string name, string description, string textColor, string backgroundColor, TimeZoneInfo timeZone, EventAlertType eventAlertType, string iCalUrl, List<SharingOptions.PublicItem> publicItems, List<UserViewSettings> viewSettings, Guid calDavGuid, int isTodo = 0)
        {
            using var tx = CalendarDb.Database.BeginTransaction();

            var calendar = new CalendarCalendars
            {
                OwnerId = ownerId.ToString(),
                Name = name,
                Description = description,
                Tenant = TenantManager.GetCurrentTenant().TenantId,
                TextColor = textColor,
                BackgroundColor = backgroundColor,
                AlertType = (int)eventAlertType,
                TimeZone = timeZone.Id,
                IcalUrl = iCalUrl,
                CaldavGuid = calDavGuid.ToString(),
                IsTodo = isTodo
            };

            calendar = CalendarDb.CalendarCalendars.Add(calendar).Entity;

            if (publicItems != null)
            {
                foreach (var item in publicItems)
                {
                    var calendarItem = new CalendarCalendarItem
                    {
                        CalendarId = calendar.Id,
                        ItemId = item.Id,
                        IsGroup = Convert.ToInt32(item.IsGroup)
                    };
                    CalendarDb.CalendarCalendarItem.Add(calendarItem);
                }
            }
            if (viewSettings != null)
            {
                foreach (var view in viewSettings)
                {
                    var calendarUser = new CalendarCalendarUser
                    {
                        CalendarId = calendar.Id,
                        UserId = view.UserId,
                        HideEvents = Convert.ToInt32(view.IsHideEvents),
                        IsAccepted = Convert.ToInt32(view.IsAccepted),
                        TextColor = view.TextColor,
                        BackgroundColor = view.BackgroundColor,
                        AlertType = (int)view.EventAlertType,
                        Name = view.Name ?? "",
                        TimeZone = view.TimeZone != null ? view.TimeZone.Id : null
                    };
                    CalendarDb.CalendarCalendarUser.Add(calendarUser);
                }
            }
            CalendarDb.SaveChanges();

            tx.Commit();

            return GetCalendarById(calendar.Id);
        }

        public Calendar UpdateCalendarGuid(int calendarId, Guid calDavGuid)
        {

            using var tx = CalendarDb.Database.BeginTransaction();

            var originalData = CalendarDb.CalendarCalendars.SingleOrDefault(i => i.Id == calendarId);
            if (originalData != null)
            {
                originalData.CaldavGuid = calDavGuid.ToString();

                CalendarDb.AddOrUpdate(r => r.CalendarCalendars, originalData);
            }

            CalendarDb.SaveChanges();
            tx.Commit();

            return GetCalendarById(calendarId);
        }
        public string GetCalDavGuid(string id)
        {
            var dataCaldavGuid = CalendarDb.CalendarCalendars.Where(p => p.Id.ToString() == id).Select(s => s.CaldavGuid).FirstOrDefault();

            return dataCaldavGuid;
           
        }
        public Calendar UpdateCalendar(int calendarId, string name, string description, List<SharingOptions.PublicItem> publicItems, List<UserViewSettings> viewSettings)
        {

            using var tx = CalendarDb.Database.BeginTransaction();

            var originalData = CalendarDb.CalendarCalendars.SingleOrDefault(i => i.Id == calendarId);
            if (originalData != null)
            {
                originalData.Name = name;
                originalData.Description = description;

                CalendarDb.AddOrUpdate(r => r.CalendarCalendars, originalData);
            }

            //sharing
            var existsItems = CalendarDb.CalendarCalendarItem
                .Where(p => p.CalendarId == calendarId)
                .Select(s => new SharingOptions.PublicItem
                {
                    Id = s.ItemId,
                    IsGroup = Convert.ToBoolean(s.IsGroup)
                }
                        ).ToList();

            foreach (var existCalendar in existsItems)
            {
                var cci = CalendarDb.CalendarCalendarItem
                    .Where(r =>
                        r.CalendarId == calendarId &&
                        r.ItemId == existCalendar.Id &&
                        Convert.ToBoolean(r.IsGroup) == existCalendar.IsGroup
                    ).SingleOrDefault();

                if (cci != null)
                {
                    CalendarDb.CalendarCalendarItem.Remove(cci);
                }
            }

            foreach (var item in publicItems)
            {
                var newEventItem = new CalendarCalendarItem
                {
                    CalendarId = calendarId,
                    ItemId = item.Id,
                    IsGroup = Convert.ToInt32(item.IsGroup)
                };
                CalendarDb.CalendarCalendarItem.Add(newEventItem);
            }

            //view

            var existsUsers = CalendarDb.CalendarCalendarUser
               .Where(p => p.CalendarId == calendarId)
               .Select(s => new
               {
                   ExtCalendarId = (string)s.ExtCalendarId,
                   Id = s.UserId.ToString()
               }
               ).ToList();

            foreach (var user in existsUsers)
            {
                var ccu = CalendarDb.CalendarCalendarUser
                    .Where(r =>
                        r.CalendarId == calendarId &&
                        r.ExtCalendarId == user.ExtCalendarId &&
                        r.UserId.ToString() == user.Id
                    ).SingleOrDefault();

                if (ccu != null)
                {
                    CalendarDb.CalendarCalendarUser.Remove(ccu);
                }
            }
            foreach (var view in viewSettings)
            {
                var calUser = new CalendarCalendarUser
                {
                    CalendarId = calendarId,
                    UserId = view.UserId,
                    HideEvents = Convert.ToInt32(view.IsHideEvents),
                    IsAccepted = Convert.ToInt32(view.IsAccepted),
                    TextColor = view.TextColor,
                    BackgroundColor = view.BackgroundColor,
                    AlertType = (int)view.EventAlertType,
                    Name = view.Name ?? "",
                    TimeZone = view.TimeZone != null ? view.TimeZone.Id : null
                };

                CalendarDb.CalendarCalendarUser.Add(calUser);
            }

            //update notifications

            var eventsData = CalendarDb.CalendarEvents
              .Where(p => p.CalendarId == calendarId && p.Tenant == TenantManager.GetCurrentTenant().TenantId)
              .Select(s => new
              {
                  eId = s.Id,
                  eStartDate = s.StartDate,
                  eAlertType = s.AlertType,
                  eRRule = s.Rrule,
                  eIsAllDay = s.AllDayLong
              }
              ).ToList();

            CalendarDb.SaveChanges();
            tx.Commit();

            foreach (var r in eventsData)
            {
                UpdateEventNotifications(r.eId, calendarId,
                                            r.eStartDate,
                                            (EventAlertType)r.eAlertType,
                                            RecurrenceRule.Parse(r.eRRule), null, publicItems,
                                            Convert.ToBoolean(r.eIsAllDay));
            }



            return GetCalendarById(calendarId);
        }

        public void UpdateCalendarUserView(List<UserViewSettings> viewSettings)
        {
            using var tx = CalendarDb.Database.BeginTransaction();

            foreach (var s in viewSettings)
                UpdateCalendarUserView(s);

            CalendarDb.SaveChanges();
            tx.Commit();

        }
        public void UpdateCalendarUserView(UserViewSettings viewSettings)
        {
            using var tx = CalendarDb.Database.BeginTransaction();

            int calendarId;
            if (int.TryParse(viewSettings.CalendarId, out calendarId))
            {
                var calendarUser = new CalendarCalendarUser
                {
                    CalendarId = calendarId,
                    UserId = viewSettings.UserId,
                    HideEvents = Convert.ToInt32(viewSettings.IsHideEvents),
                    TextColor = viewSettings.TextColor,
                    BackgroundColor = viewSettings.BackgroundColor,
                    IsAccepted = Convert.ToInt32(viewSettings.IsAccepted),
                    AlertType = (int)viewSettings.EventAlertType,
                    Name = viewSettings.Name ?? "",
                    TimeZone = viewSettings.TimeZone != null ? viewSettings.TimeZone.Id : null
                };
                CalendarDb.CalendarCalendarUser.Add(calendarUser);

                //update notifications
                var eventsData = CalendarDb.CalendarEvents
                      .Where(p => p.CalendarId == calendarId && p.Tenant == TenantManager.GetCurrentTenant().TenantId)
                      .Select(s => new
                      {
                          eId = s.Id,
                          eStartDate = s.StartDate,
                          eAlertType = s.AlertType,
                          eRRule = s.Rrule,
                          eCalId = s.CalendarId,
                          eIsAllDay = s.AllDayLong
                      }
                      ).ToList();

                CalendarDb.SaveChanges();
                tx.Commit();

                foreach (var r in eventsData)
                {
                    UpdateEventNotifications(r.eId, calendarId,
                                                r.eStartDate,
                                                (EventAlertType)r.eAlertType,
                                                RecurrenceRule.Parse(r.eRRule), null, null,
                                                Convert.ToBoolean(r.eIsAllDay));
                }
            }
            else
            {
                var calendarUser = new CalendarCalendarUser
                {
                    ExtCalendarId = viewSettings.CalendarId,
                    UserId = viewSettings.UserId,
                    HideEvents = Convert.ToInt32(viewSettings.IsHideEvents),
                    TextColor = viewSettings.TextColor,
                    BackgroundColor = viewSettings.BackgroundColor,
                    IsAccepted = Convert.ToInt32(viewSettings.IsAccepted),
                    AlertType = (int)viewSettings.EventAlertType,
                    Name = viewSettings.Name ?? "",
                    TimeZone = viewSettings.TimeZone != null ? viewSettings.TimeZone.Id : null
                };
                CalendarDb.CalendarCalendarUser.Add(calendarUser);

                CalendarDb.SaveChanges();
                tx.Commit();

                if (String.Equals(viewSettings.CalendarId, SharedEventsCalendar.CalendarId,
                                    StringComparison.InvariantCultureIgnoreCase))
                {
                    //update notifications
                    var groups = UserManager.GetUserGroups(viewSettings.UserId).Select(g => g.ID).ToList();

                    groups.AddRange(
                            UserManager.GetUserGroups(viewSettings.UserId, Constants.SysGroupCategoryId)
                                       .Select(g => g.ID)
                        );


                    var eventsData = from events in CalendarDb.CalendarEvents
                                     join eventItem in CalendarDb.CalendarEventItem on events.Id equals eventItem.EventId
                                     where
                                          events.Tenant == TenantManager.GetCurrentTenant().TenantId &&
                                          ((eventItem.IsGroup == 0 && eventItem.ItemId == viewSettings.UserId) ||
                                           (eventItem.IsGroup == 1 && groups.Contains(eventItem.ItemId)))
                                     select new
                                     {
                                         eId = events.Id,
                                         eStartDate = events.StartDate,
                                         eAlertType = events.AlertType,
                                         eRRule = events.Rrule,
                                         eCalId = events.CalendarId,
                                         eIsAllDay = events.AllDayLong
                                     };

                    foreach (var r in eventsData)
                    {
                        UpdateEventNotifications(r.eId, r.eCalId,
                                                    r.eStartDate,
                                                    (EventAlertType)r.eAlertType,
                                                    RecurrenceRule.Parse(r.eRRule), null, null,
                                                    Convert.ToBoolean(r.eIsAllDay));
                    }
                }
            }
        }

        public void RemoveCalendar(int calendarId, Uri myUri)
        {
            using var tx = CalendarDb.Database.BeginTransaction();

            try
            {
                var dataCaldavGuid = CalendarDb.CalendarCalendars.Where(p => p.Id == calendarId).Select(s => s.CaldavGuid).ToArray();
                var caldavGuid = dataCaldavGuid[0] != null ? Guid.Parse(dataCaldavGuid[0].ToString()) : Guid.Empty;
                if (caldavGuid != Guid.Empty)
                {
                    var caldavHost = myUri.Host;

                    Log.Info("RADICALE REWRITE URL: " + myUri);

                    var currentUserName = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email.ToLower() + "@" + caldavHost;
                    var _email = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email;
                    string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, UserManager.GetUserByEmail(_email).ID);
                    var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email.ToLower() + ":" + currentAccountPaswd));

                    //TODO Caldav
                    /*var caldavTask = new Task(() => RemoveCaldavCalendar(currentUserName, _email, currentAccountPaswd, encoded, caldavGuid.ToString(), myUri));
                    caldavTask.Start();*/

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            var cc = CalendarDb.CalendarCalendars.Where(r => r.Id == calendarId).SingleOrDefault();
            if (cc != null) CalendarDb.CalendarCalendars.Remove(cc);

            var ccu = CalendarDb.CalendarCalendarUser.Where(r => r.CalendarId == calendarId).SingleOrDefault();
            if (ccu != null) CalendarDb.CalendarCalendarUser.Remove(ccu);

            var cci = CalendarDb.CalendarCalendarItem.Where(r => r.CalendarId == calendarId).SingleOrDefault();
            if (cci != null) CalendarDb.CalendarCalendarItem.Remove(cci);

            var tenant = TenantManager.GetCurrentTenant().TenantId;

            var data = CalendarDb.CalendarEvents.Where(p => p.CalendarId == calendarId && p.Tenant == tenant).Select(s => s.Id).ToArray();

            var ce = CalendarDb.CalendarEvents.Where(r => r.CalendarId == calendarId && r.Tenant == tenant).SingleOrDefault();
            if (ce != null) CalendarDb.CalendarEvents.Remove(ce);

            var cei = CalendarDb.CalendarEventItem.Where(r => data.Contains(r.EventId)).ToList();
            foreach (var eventItem in cei) { CalendarDb.CalendarEventItem.Remove(eventItem); }

            var ceu = CalendarDb.CalendarEventUser.Where(r => data.Contains(r.EventId)).ToList();
            foreach (var eventUser in ceu) { CalendarDb.CalendarEventUser.Remove(eventUser); }

            var cn = CalendarDb.CalendarNotifications.Where(r => data.Contains(r.EventId)).ToList();
            foreach (var calNotifications in cn) { CalendarDb.CalendarNotifications.Remove(calNotifications); }

            var ceh = CalendarDb.CalendarEventHistory.Where(r => data.Contains(r.EventId) && r.Tenant == tenant).ToList();
            foreach (var eventHistory in ceh) { CalendarDb.CalendarEventHistory.Remove(eventHistory); }

            CalendarDb.SaveChanges();
            tx.Commit();
        }
        /*
        public void RemoveCaldavCalendar(string currentUserName, string email, string currentAccountPaswd, string encoded, string calDavGuid, Uri myUri, bool isShared = false)
        {
            var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";
            var calDavUrl = calDavServerUrl.Insert(calDavServerUrl.IndexOf("://") + 3, HttpUtility.UrlEncode(currentUserName) + ":" + HttpUtility.UrlEncode(currentAccountPaswd) + "@");
            var requestUrl = calDavUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + (isShared ? calDavGuid + "-shared" : calDavGuid);

            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                webRequest.Method = "DELETE";
                webRequest.ContentType = "text/xml; charset=utf-8";
                webRequest.Headers.Add("Authorization", "Basic " + encoded);
                using (var webResponse = webRequest.GetResponse())
                using (var reader = new StreamReader(webResponse.GetResponseStream())){}
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ASC.Calendar").Error(ex);
            }
        }
        public void RemoveExternalCalendarData(string calendarId)
        {
            using (var tr = db.BeginTransaction())
            {
                db.ExecuteNonQuery(new SqlDelete("calendar_calendar_user").Where("ext_calendar_id",calendarId));
                tr.Commit();
            }
        }

    */
        public Todo GetTodoByUid(string todoUid)
        {
            var data = from todos in CalendarDb.CalendarTodos
                       join calendar in CalendarDb.CalendarCalendars on todos.CalendarId equals calendar.Id
                       where
                          todos.Tenant == TenantManager.GetCurrentTenant().TenantId &&
                          todos.Uid.Contains(todoUid) &&
                          todos.OwnerId == AuthContext.CurrentAccount.ID &&
                          calendar.OwnerId == AuthContext.CurrentAccount.ID.ToString() &&
                          calendar.IcalUrl == null
                       select todos.Id;

            var todoId = data.FirstOrDefault();

            return todoId == 0 ? null : GetTodoById(todoId);
        }
        public Todo GetTodoIdByUid(string uid, int calendarId)
        {
            var todoId = CalendarDb.CalendarTodos.Where(p => p.Uid.Contains(uid) && p.CalendarId == calendarId).Select(s => s.Id).FirstOrDefault();

            return todoId == 0 ? null : GetTodoById(todoId);
        }

        public Todo UpdateTodo(string id, int calendarId, Guid ownerId, string name, string description, DateTime utcStartDate, string uid, DateTime completed)
        {
            var todoUid = GetEventUid(uid);
            using var tx = CalendarDb.Database.BeginTransaction();

            var updateTodo = new CalendarTodos
            {
                Id = Convert.ToInt32(id),
                Name = name,
                Description = description,
                CalendarId = calendarId,
                OwnerId = ownerId,
                StartDate = utcStartDate,
                Uid = todoUid,
                Completed = completed
            };
            CalendarDb.AddOrUpdate(r => r.CalendarTodos, updateTodo);

            CalendarDb.SaveChanges();
            tx.Commit();

            return GetTodoById(int.Parse(id));

        }

        public Todo CreateTodo(int calendarId,
                                 Guid ownerId,
                                 string name,
                                 string description,
                                 DateTime utcStartDate,
                                 string uid,
                                 DateTime completed)
        {
            var todoUid = GetEventUid(uid);

            using var tx = CalendarDb.Database.BeginTransaction();

            var newTodo = new CalendarTodos
            {
                Tenant = TenantManager.GetCurrentTenant().TenantId,
                Name = name,
                Description = description,
                CalendarId = calendarId,
                OwnerId = ownerId,
                StartDate = utcStartDate,
                Uid = todoUid,
                Completed = completed
            };

            newTodo = CalendarDb.CalendarTodos.Add(newTodo).Entity;

            CalendarDb.SaveChanges();
            tx.Commit();

            return GetTodoById(newTodo.Id);
        }
        public Todo GetTodoById(int todoId)
        {
            var todos = GetTodosByIds(new int[] { todoId }, AuthContext.CurrentAccount.ID);
            if (todos.Count > 0)
                return todos[0];

            return null;
        }

        public List<Todo> GetTodosByIds(int[] todoIds, Guid userId, int tenantId = -1)
        {

            var todoList = new List<Todo>();
            if (todoIds.Length > 0)
            {

                if (tenantId != -1)
                {
                    var data = CalendarDb.CalendarTodos
                                 .Where(p => todoIds.Contains(p.Id) && p.Tenant == tenantId)
                                 .Select(s => new
                                 {
                                     Id = s.Id.ToString(),
                                     Name = s.Name,
                                     Description = s.Description,
                                     TenantId = s.Tenant,
                                     CalendarId = s.CalendarId.ToString(),
                                     UtcStartDate = s.StartDate,
                                     Completed = s.Completed,
                                     OwnerId = s.OwnerId,
                                     Uid = s.Uid
                                 })
                                 .ToList();
                    var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager);
                    todoList = data.ConvertAll(r => new Todo(AuthContext, TimeZoneConverter, icalendar, this)
                    {
                        Id = r.Id.ToString(),
                        Name = r.Name,
                        Description = r.Description,
                        TenantId = r.TenantId,
                        CalendarId = r.CalendarId.ToString(),
                        UtcStartDate = r.UtcStartDate ?? DateTime.MinValue,
                        Completed = r.Completed ?? DateTime.MinValue,
                        OwnerId = r.OwnerId,
                        Uid = r.Uid

                    });
                }
                else
                {
                    var data = CalendarDb.CalendarTodos
                                 .Where(p => todoIds.Contains(p.Id))
                                 .Select(s => new
                                 {
                                     Id = s.Id.ToString(),
                                     Name = s.Name,
                                     Description = s.Description,
                                     TenantId = s.Tenant,
                                     CalendarId = s.CalendarId.ToString(),
                                     UtcStartDate = s.StartDate,
                                     Completed = s.Completed,
                                     OwnerId = s.OwnerId,
                                     Uid = s.Uid
                                 })
                                 .ToList();
                    var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager);
                    todoList = data.ConvertAll(r => new Todo(AuthContext, TimeZoneConverter, icalendar, this)
                    {
                        Id = r.Id.ToString(),
                        Name = r.Name,
                        Description = r.Description,
                        TenantId = r.TenantId,
                        CalendarId = r.CalendarId.ToString(),
                        UtcStartDate = r.UtcStartDate ?? DateTime.MinValue,
                        Completed = r.Completed ?? DateTime.MinValue,
                        OwnerId = r.OwnerId,
                        Uid = r.Uid
                    });
                }
            }


            //parsing           
            var todos = new List<Todo>();

            foreach (var r in todoList)
            {
                var td =
                    todos.Find(
                        e => String.Equals(e.Id, r.Id, StringComparison.InvariantCultureIgnoreCase));
                if (td == null)
                {
                    todos.Add(r);
                }
            }
            return todos;
        }

        public List<Todo> LoadTodos(int calendarId, Guid userId, int tenantId, DateTime utcStartDate, DateTime utcEndDate)
        {
            var tdIds = CalendarDb.CalendarTodos.Where(p => p.CalendarId == calendarId && p.Tenant == tenantId).Select(s => s.Id).ToArray();

            return GetTodosByIds(tdIds, userId, tenantId);
        }

        internal List<Event> LoadSharedEvents(Guid userId, int tenantId, DateTime utcStartDate, DateTime utcEndDate)
        {
            var groups = UserManager.GetUserGroups(userId).Select(g => g.ID).ToList();
            groups.AddRange(UserManager.GetUserGroups(userId, Constants.SysGroupCategoryId).Select(g => g.ID));

            var evIds = from events in CalendarDb.CalendarEvents
                       join eventItem in CalendarDb.CalendarEventItem on events.Id equals eventItem.EventId
                       where
                            events.Tenant == tenantId &&
                            (
                                eventItem.ItemId == userId || (groups.Contains(eventItem.ItemId) && eventItem.IsGroup == 1) &&
                                events.Tenant == tenantId &&
                                ((events.StartDate >= utcStartDate && events.StartDate <= utcEndDate && events.Rrule == "") || events.Rrule != "") &&
                                events.OwnerId != userId &&
                                !(from calEventUser in CalendarDb.CalendarEventUser
                                    where calEventUser.EventId == events.Id && calEventUser.UserId == userId && calEventUser.IsUnsubscribe == 1 
                                    select calEventUser.EventId).Any()
                            )
                       select events.Id;

            return GetEventsByIds(evIds.ToArray(), userId, tenantId);
        }

        public List<Event> LoadEvents(int calendarId, Guid userId, int tenantId, DateTime utcStartDate, DateTime utcEndDate)
        {

            var evIds = CalendarDb.CalendarEvents
                .Where(p =>
                    p.CalendarId == calendarId &&
                    p.Tenant == tenantId &&
                    (
                        p.Rrule != "" ||
                        (
                            p.Rrule == "" &&
                            (
                               (p.StartDate >= utcStartDate && p.EndDate <= utcEndDate) ||
                               (p.StartDate <= utcStartDate && p.EndDate >= utcEndDate) ||
                               (p.StartDate <= utcEndDate && p.EndDate >= utcStartDate)

                            )
                        )
                    )
                )
                .Select(s => s.Id).ToList();

            return GetEventsByIds(evIds.ToArray(), userId, tenantId);
        }
        /*
        public Event GetEventById(int eventId)
        {
            var events = GetEventsByIds(new object[] { eventId }, AuthContext.CurrentAccount.ID);
            if (events.Count > 0)
                return events[0];

            return null;
        }

        public List<Event> GetEventsByIds(object[] evtIds, Guid userId, int tenantId = -1)
        {
            var cc = new ColumnCollection();
            var eId = cc.RegistryColumn("evt.id");
            var eName = cc.RegistryColumn("evt.name");
            var eDescription = cc.RegistryColumn("evt.description");
            var eTenant = cc.RegistryColumn("evt.tenant");
            var eCalId = cc.RegistryColumn("evt.calendar_id");
            var eStartDate = cc.RegistryColumn("evt.start_date");
            var eEndDate = cc.RegistryColumn("evt.end_date");
            var eUpdateDate = cc.RegistryColumn("evt.update_date");
            var eIsAllDay = cc.RegistryColumn("evt.all_day_long");
            var eRRule = cc.RegistryColumn("evt.rrule");
            var eOwner = cc.RegistryColumn("evt.owner_id");
            var usrAlertType = cc.RegistryColumn("evt_usr.alert_type");
            var eAlertType = cc.RegistryColumn("evt.alert_type");
            var eUid = cc.RegistryColumn("evt.uid");
            var eStatus = cc.RegistryColumn("evt.status");

            var data = new List<Object[]>();

            if (evtIds.Length > 0)
            {
                if (tenantId != -1)
                {
                    data = db.ExecuteList(new SqlQuery(_eventTable)
                                              .LeftOuterJoin("calendar_event_user evt_usr",
                                                             Exp.EqColumns(eId.Name, "evt_usr.event_id") &
                                                             Exp.Eq("evt_usr.user_id", userId))
                                              .Select(cc.SelectQuery)
                                              .Where(Exp.In(eId.Name, evtIds))
                                              .Where("tenant", tenantId));
                }
                else
                {
                    data = db.ExecuteList(new SqlQuery(_eventTable)
                                                .LeftOuterJoin("calendar_event_user evt_usr",
                                                               Exp.EqColumns(eId.Name, "evt_usr.event_id") &
                                                               Exp.Eq("evt_usr.user_id", userId))
                                                .Select(cc.SelectQuery)
                                                .Where(Exp.In(eId.Name, evtIds)));
                    
                }
                
            }

            var cc1 = new ColumnCollection();
            var evId = cc1.RegistryColumn("evt_itm.event_id");
            var itemId = cc1.RegistryColumn("evt_itm.item_id");
            var itemIsGroup = cc1.RegistryColumn("evt_itm.is_group");

            var sharingData = new List<Object[]>();

            if (evtIds.Length > 0)
            {
                sharingData = db.ExecuteList(new SqlQuery(_eventItemTable).Select(cc1.SelectQuery)
                                                                                   .Where(Exp.In(evId.Name, evtIds)));
            }

            //parsing           
            var events = new List<Event>();

            foreach (var r in data)
            {
                var ev =
                    events.Find(
                        e => String.Equals(e.Id, eId.Parse<string>(r), StringComparison.InvariantCultureIgnoreCase));
                if (ev == null)
                {
                    ev = new Event
                        {
                            Id = eId.Parse<string>(r),
                            Name = eName.Parse<string>(r),
                            Description = eDescription.Parse<string>(r),
                            TenantId = eTenant.Parse<int>(r),
                            CalendarId = eCalId.Parse<string>(r),
                            UtcStartDate = eStartDate.Parse<DateTime>(r),
                            UtcEndDate = eEndDate.Parse<DateTime>(r),
                            UtcUpdateDate = eUpdateDate.Parse<DateTime>(r),
                            AllDayLong = eIsAllDay.Parse<bool>(r),
                            OwnerId = eOwner.Parse<Guid>(r),
                            AlertType =
                                (usrAlertType.IsNull(r))
                                    ? (EventAlertType) eAlertType.Parse<int>(r)
                                    : (EventAlertType) usrAlertType.Parse<int>(r),
                            RecurrenceRule = eRRule.Parse<RecurrenceRule>(r),
                            Uid = eUid.Parse<string>(r),
                            Status = (EventStatus) eStatus.Parse<int>(r)
                        };
                    events.Add(ev);
                }

                foreach (var row in sharingData)
                {
                    if (String.Equals(evId.Parse<string>(row), ev.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ev.SharingOptions.PublicItems.Add(new SharingOptions.PublicItem
                            {
                                Id = itemId.Parse<Guid>(row),
                                IsGroup = itemIsGroup.Parse<bool>(row)
                            });
                    }
                }
            }
            return events;
        }*/
        public Event GetEventByUid(string eventUid)
        {
            var data = from events in CalendarDb.CalendarEvents
                       join calendars in CalendarDb.CalendarCalendars on events.CalendarId equals calendars.Id
                       where
                        events.Uid == eventUid &&
                        events.OwnerId == AuthContext.CurrentAccount.ID &&
                        calendars.OwnerId == AuthContext.CurrentAccount.ID.ToString() &&
                        calendars.IcalUrl == null
                       select events.Id;

            var eventId = data.FirstOrDefault();
            return eventId == 0 ? null : GetEventById(eventId);
        }
        public Event GetEventById(int eventId)
        {
            var events = GetEventsByIds(new int[] { eventId }, SecurityContext.CurrentAccount.ID);
            if (events.Count > 0)
                return events[0];

            return null;
        }
        //TODO duplicate code
        public List<Event> GetEventsByIds(int[] evtIds, Guid userId, int tenantId = -1)
        {
            var sharingData = CalendarDb.CalendarEventItem.Where(p => evtIds.Contains(p.EventId)).ToList();

            var events = new List<Event>();

            if (evtIds.Length > 0)
            {
                if (tenantId != -1)
                {
                    var data = from calEvt in CalendarDb.CalendarEvents
                               join evtUsr in CalendarDb.CalendarEventUser
                               on calEvt.Id equals evtUsr.EventId
                               into UserEvent
                               from ue in UserEvent.DefaultIfEmpty()
                               where
                                    evtIds.Contains(calEvt.Id) &&
                                    calEvt.Tenant == tenantId
                               select new
                               {
                                   Id = calEvt.Id.ToString(),
                                   Name = calEvt.Name,
                                   Description = calEvt.Description,
                                   TenantId = calEvt.Tenant,
                                   CalendarId = calEvt.CalendarId.ToString(),
                                   UtcStartDate = calEvt.StartDate,
                                   UtcEndDate = calEvt.EndDate,
                                   UtcUpdateDate = (DateTime)calEvt.UpdateDate,
                                   AllDayLong = Convert.ToBoolean(calEvt.AllDayLong),
                                   OwnerId = calEvt.OwnerId,
                                   AlertType = (EventAlertType)ue.AlertType,
                                   RecurrenceRule = calEvt.Rrule,
                                   Uid = calEvt.Uid,
                                   Status = (EventStatus)calEvt.Status
                               };
                    foreach (var r in data)
                    {
                        var ev =
                            events.Find(
                                e => String.Equals(e.Id, r.Id, StringComparison.InvariantCultureIgnoreCase));
                        if (ev == null)
                        {
                            var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager);
                            ev = new Event(AuthContext, TimeZoneConverter, icalendar, this)
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Description = r.Description,
                                TenantId = r.TenantId,
                                CalendarId = r.CalendarId,
                                UtcStartDate = r.UtcStartDate,
                                UtcEndDate = r.UtcEndDate,
                                UtcUpdateDate = r.UtcUpdateDate,
                                AllDayLong = r.AllDayLong,
                                OwnerId = r.OwnerId,
                                AlertType = r.AlertType,
                                RecurrenceRule = RecurrenceRule.Parse(r.RecurrenceRule),
                                Uid = r.Uid,
                                Status = r.Status
                            };
                            events.Add(ev);
                        }
                        foreach (var row in sharingData)
                        {
                            if (String.Equals(r.Id, ev.Id, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ev.SharingOptions.PublicItems.Add(new SharingOptions.PublicItem
                                {
                                    Id = row.ItemId,
                                    IsGroup = Convert.ToBoolean(row.IsGroup)
                                });
                            }
                        }
                    }
                }
                else
                {
                    var data = from calEvt in CalendarDb.CalendarEvents
                               join evtUsr in CalendarDb.CalendarEventUser
                               on calEvt.Id equals evtUsr.EventId
                               into UserEvent
                               from ue in UserEvent.DefaultIfEmpty()
                               where
                                    evtIds.Contains(calEvt.Id)
                               select new
                               {
                                   Id = calEvt.Id.ToString(),
                                   Name = calEvt.Name,
                                   Description = calEvt.Description,
                                   TenantId = calEvt.Tenant,
                                   CalendarId = calEvt.CalendarId.ToString(),
                                   UtcStartDate = calEvt.StartDate,
                                   UtcEndDate = calEvt.EndDate,
                                   UtcUpdateDate = (DateTime)calEvt.UpdateDate,
                                   AllDayLong = Convert.ToBoolean(calEvt.AllDayLong),
                                   OwnerId = calEvt.OwnerId,
                                   AlertType = (EventAlertType)ue.AlertType,
                                   RecurrenceRule = calEvt.Rrule,
                                   Uid = calEvt.Uid,
                                   Status = (EventStatus)calEvt.Status
                               };
                    foreach (var r in data)
                    {
                        var ev =
                            events.Find(
                                e => String.Equals(e.Id, r.Id, StringComparison.InvariantCultureIgnoreCase));
                        if (ev == null)
                        {
                            var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager);
                            ev = new Event(AuthContext, TimeZoneConverter, icalendar, this)
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Description = r.Description,
                                TenantId = r.TenantId,
                                CalendarId = r.CalendarId,
                                UtcStartDate = r.UtcStartDate,
                                UtcEndDate = r.UtcEndDate,
                                UtcUpdateDate = r.UtcUpdateDate,
                                AllDayLong = r.AllDayLong,
                                OwnerId = r.OwnerId,
                                AlertType = r.AlertType,
                                RecurrenceRule = RecurrenceRule.Parse(r.RecurrenceRule),
                                Uid = r.Uid,
                                Status = r.Status
                            };
                            events.Add(ev);
                        }
                        foreach (var row in sharingData)
                        {
                            if (String.Equals(r.Id, ev.Id, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ev.SharingOptions.PublicItems.Add(new SharingOptions.PublicItem
                                {
                                    Id = row.ItemId,
                                    IsGroup = Convert.ToBoolean(row.IsGroup)
                                });
                            }
                        }
                    }
                }
            }

            return events;
        }
        /*public Event GetEventOnlyByUid(string eventUid)
        {
            var sql = new SqlQuery("calendar_events e")
                .Select("e.id")
                .Where("e.tenant", TenantManager.GetCurrentTenant().TenantId)
                .Where("e.uid", eventUid);

            var eventId = db.ExecuteScalar<int>(sql);

            return eventId == 0 ? null : GetEventById(eventId);
        }*/

        public void SetEventUid(int eventId, string uid)
        {
            using var tx = CalendarDb.Database.BeginTransaction();

            var newEvent = new CalendarEvents
            {
                Id = eventId,
                Uid = uid
            };
            CalendarDb.AddOrUpdate(r => r.CalendarEvents, newEvent);

            CalendarDb.SaveChanges();
            tx.Commit();
        }

        public void UnsubscribeFromEvent(int eventID, Guid userId)
        {
            using var tx = CalendarDb.Database.BeginTransaction();

            var cei = CalendarDb.CalendarEventItem.Where(r => r.EventId == eventID && r.ItemId == userId && r.IsGroup == 0).SingleOrDefault();

            if (cei != null)
            {
                CalendarDb.CalendarEventItem.Remove(cei);
            }
            else
            {
                var newEventUser = new CalendarEventUser
                {
                    EventId = eventID,
                    UserId = userId,
                    IsUnsubscribe = 1
                };
                CalendarDb.CalendarEventUser.Add(newEventUser);

            }

            var cn = CalendarDb.CalendarNotifications.Where(r => r.EventId == eventID && r.UserId == userId).SingleOrDefault();

            if (cn != null) CalendarDb.CalendarNotifications.Remove(cn);

            CalendarDb.SaveChanges();
            tx.Commit();

        }

        public void RemoveEvent(int eventId)
        {
            using var tx = CalendarDb.Database.BeginTransaction();
            var tenant = TenantManager.GetCurrentTenant().TenantId;

            var ce = CalendarDb.CalendarEvents.Where(r => r.Id == eventId && r.Tenant == tenant).SingleOrDefault();
            if (ce != null) CalendarDb.CalendarEvents.Remove(ce);

            var cei = CalendarDb.CalendarEventItem.Where(r => r.EventId == eventId).SingleOrDefault();
            if (cei != null) CalendarDb.CalendarEventItem.Remove(cei);

            var ceu = CalendarDb.CalendarEventUser.Where(r => r.EventId == eventId).SingleOrDefault();
            if (ceu != null) CalendarDb.CalendarEventUser.Remove(ceu);

            var cn = CalendarDb.CalendarNotifications.Where(r => r.EventId == eventId).SingleOrDefault();
            if (cn != null) CalendarDb.CalendarNotifications.Remove(cn);

            var ceh = CalendarDb.CalendarEventHistory.Where(r => r.EventId == eventId && r.Tenant == tenant).SingleOrDefault();
            if (ceh != null) CalendarDb.CalendarEventHistory.Remove(ceh);

            CalendarDb.SaveChanges();
            tx.Commit();

        }

        public Event CreateEvent(int calendarId,
                                 Guid ownerId,
                                 string name,
                                 string description,
                                 DateTime utcStartDate,
                                 DateTime utcEndDate,
                                 RecurrenceRule rrule,
                                 EventAlertType alertType,
                                 bool isAllDayLong,
                                 List<SharingOptions.PublicItem> publicItems,
                                 string uid,
                                 EventStatus status,
                                 DateTime createDate)
        {

            using var tx = CalendarDb.Database.BeginTransaction();

            var newEvent = new CalendarEvents
            {
                Id = 0,
                Tenant = TenantManager.GetCurrentTenant().TenantId,
                Name = name,
                Description = description,
                CalendarId = calendarId,
                StartDate = utcStartDate,
                EndDate = utcEndDate,
                UpdateDate = createDate,
                AllDayLong = Convert.ToInt32(isAllDayLong),
                OwnerId = ownerId,
                AlertType = (int)alertType,
                Rrule = rrule.ToString(),
                Uid = GetEventUid(uid),
                Status = (int)status

            };
            newEvent = CalendarDb.CalendarEvents.Add(newEvent).Entity;

            foreach (var item in publicItems)
            {
                var calEventItem = new CalendarEventItem
                {
                    EventId = newEvent.Id,
                    ItemId = item.Id,
                    IsGroup = Convert.ToInt32(item.IsGroup)
                };

                CalendarDb.CalendarEventItem.Add(calEventItem);

            }

            CalendarDb.SaveChanges();
            tx.Commit();

            //update notifications
            UpdateEventNotifications(newEvent.Id, calendarId, utcStartDate, alertType, rrule, publicItems, null, isAllDayLong);

            return GetEventById(newEvent.Id);
        }

        public Event UpdateEvent(int eventId,
            string eventUid,
            int calendarId,
            Guid ownerId,
            string name,
            string description,
            DateTime utcStartDate,
            DateTime utcEndDate,
            RecurrenceRule rrule,
            EventAlertType alertType,
            bool isAllDayLong,
            List<SharingOptions.PublicItem> publicItems,
            EventStatus status,
            DateTime createDate
            )
        {
            using var tx = CalendarDb.Database.BeginTransaction();

            var newEvent = new CalendarEvents
            {
                Id = eventId,
                Uid = eventUid,
                Name = name,
                Description = description,
                CalendarId = calendarId,
                OwnerId = ownerId,
                StartDate = utcStartDate,
                EndDate = utcEndDate,
                UpdateDate = createDate,
                AllDayLong = Convert.ToInt32(isAllDayLong),
                Rrule = rrule.ToString(),
                Status = (int)status
            };
            if (ownerId.Equals(AuthContext.CurrentAccount.ID))
            {
                newEvent.AlertType = (int)alertType;
            }
            else
            {
                var newCalEvtUser = new CalendarEventUser
                {
                    EventId = eventId,
                    UserId = AuthContext.CurrentAccount.ID,
                    AlertType = (int)alertType
                };
                CalendarDb.CalendarEventUser.Add(newCalEvtUser);
            }

            CalendarDb.AddOrUpdate(r => r.CalendarEvents, newEvent);

            var userIds = CalendarDb.CalendarEventUser.Where(p => p.EventId == eventId).Select(t => t.UserId).ToList();

            foreach (var usrId in userIds)
            {
                if (!publicItems.Exists(i => (i.IsGroup && UserManager.IsUserInGroup(usrId, i.Id))
                                                || (!i.IsGroup && i.Id.Equals(usrId))))
                {
                    var eu = CalendarDb.CalendarEventUser
                        .Where(r => r.EventId == eventId && r.UserId == usrId)
                        .SingleOrDefault();
                    if (eu != null)
                        CalendarDb.CalendarEventUser.Remove(eu);
                }
            }
            var cei = CalendarDb.CalendarEventItem
                .Where(r => r.EventId == eventId)
                .SingleOrDefault();
            if (cei != null)
                CalendarDb.CalendarEventItem.Remove(cei);

            foreach (var item in publicItems)
            {
                var calEvtItem = new CalendarEventItem
                {
                    EventId = eventId,
                    ItemId = item.Id,
                    IsGroup = Convert.ToInt32(item.IsGroup)
                };

                CalendarDb.CalendarEventItem.Add(calEvtItem);
            }



            CalendarDb.SaveChanges();
            tx.Commit();
            /*using (var tr = db.BeginTransaction())
            {
                var query = new SqlUpdate("calendar_events")
                    .Set("name", name)
                    .Set("description", description)
                    .Set("calendar_id", calendarId)
                    .Set("owner_id", ownerId)
                    .Set("start_date", utcStartDate.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Set("end_date", utcEndDate.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Set("update_date", createDate.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Set("all_day_long", isAllDayLong)
                    .Set("rrule", rrule.ToString())
                    .Set("status", (int) status)
                    .Where(Exp.Eq("id", eventId));

                if (ownerId.Equals(AuthContext.CurrentAccount.ID))
                    query = query.Set("alert_type", (int) alertType);
                else
                    db.ExecuteNonQuery(new SqlInsert("calendar_event_user", true)
                                                    .InColumnValue("event_id", eventId)
                                                    .InColumnValue("user_id", AuthContext.CurrentAccount.ID)
                                                    .InColumnValue("alert_type", alertType));


                db.ExecuteNonQuery(query);

                var userIds = db.ExecuteList(new SqlQuery("calendar_event_user")
                                                        .Select("user_id")
                                                        .Where("event_id", eventId))
                                        .Select(r => new Guid(Convert.ToString(r[0])));

                foreach (var usrId in userIds)
                {
                    if (!publicItems.Exists(i => (i.IsGroup && UserManager.IsUserInGroup(usrId, i.Id))
                                                    || (!i.IsGroup && i.Id.Equals(usrId))))
                    {
                        db.ExecuteNonQuery(new SqlDelete("calendar_event_user")
                                                        .Where(Exp.Eq("user_id", usrId) & Exp.Eq("event_id", eventId)));
                    }
                }

                db.ExecuteNonQuery(new SqlDelete("calendar_event_item").Where("event_id", eventId));
                foreach (var item in publicItems)
                {
                    db.ExecuteNonQuery(new SqlInsert("calendar_event_item")
                                                    .InColumnValue("event_id", eventId)
                                                    .InColumnValue("item_id", item.Id)
                                                    .InColumnValue("is_group", item.IsGroup));


                }

                //update notifications
                var baseAlertType =
                    db.ExecuteList(new SqlQuery("calendar_events").Select("alert_type").Where("id", eventId))
                                .Select(r => (EventAlertType) Convert.ToInt32(r[0])).First();
                UpdateEventNotifications(eventId, calendarId, utcStartDate, baseAlertType, rrule, publicItems, null, isAllDayLong);


                tr.Commit();
            }*/

            return GetEventById(eventId);
        }

        public EventHistory GetEventHistory(string eventUid)
        {
            var data = from history in CalendarDb.CalendarEventHistory
                       join events in CalendarDb.CalendarEvents on history.EventUid equals events.Uid
                       join calendars in CalendarDb.CalendarCalendars on history.CalendarId equals calendars.Id
                       where
                        history.EventUid == eventUid &&
                        events.OwnerId == AuthContext.CurrentAccount.ID &&
                        calendars.OwnerId == AuthContext.CurrentAccount.ID.ToString() &&
                        calendars.IcalUrl == null
                       select new EventHistory
                       {
                           CalendarId = history.CalendarId,
                           EventUid = history.EventUid,
                           EventId = history.EventId,
                           Ics = history.Ics
                       };

            return data.Count() > 0 ? data.FirstOrDefault() : null;
        }
        private EventHistory ToEventHistory(object[] row)
        {
            return EventHistoryHelper.Get(Convert.ToInt32(row[0]),
                                    Convert.ToString(row[1]),
                                    Convert.ToInt32(row[2]),
                                    Convert.ToString(row[3]));
        }
        public List<EventHistory> GetEventsHistory(int[] eventIds)
        {
            var data = from eventHistory in CalendarDb.CalendarEventHistory
                       where
                        eventHistory.Tenant == TenantManager.GetCurrentTenant().TenantId &&
                        eventIds.Contains(eventHistory.EventId)
                       select new
                       {
                           eventHistory.CalendarId,
                           eventHistory.EventUid,
                           eventHistory.EventId,
                           eventHistory.Ics
                       };


            var items = new List<EventHistory>();
            foreach (var r in data)
            {
                var eventHistory = EventHistoryHelper.Get(r.CalendarId, r.EventUid, r.EventId, r.Ics);
                items.Add(eventHistory);
            }
            return items;
        }
        public EventHistory GetEventHistory(int eventId)
        {
            var items = GetEventsHistory(new[] { eventId });
            return items.Count > 0 ? items[0] : null;
        }
        public EventHistory AddEventHistory(int calendarId, string eventUid, int eventId, string ics)
        {
            var icsCalendars = DDayICalParser.DeserializeCalendar(ics);
            var icsCalendar = icsCalendars == null ? null : icsCalendars.FirstOrDefault();
            var icsEvents = icsCalendar == null ? null : icsCalendar.Events;
            var icsEvent = icsEvents == null ? null : icsEvents.FirstOrDefault();

            if (icsEvent == null) return null;

            EventHistory history;
            using var tx = CalendarDb.Database.BeginTransaction();

            history = GetEventHistory(eventId);

            if (history == null)
            {
                history = EventHistoryHelper.Get(calendarId, eventUid, eventId, ics);

                var newHistory = new CalendarEventHistory
                {
                    Tenant = TenantManager.GetCurrentTenant().TenantId,
                    CalendarId = calendarId,
                    EventUid = eventUid,
                    EventId = eventId,
                    Ics = history.Ics
                };
                CalendarDb.CalendarEventHistory.Add(newHistory);
            }
            else
            {
                var exist = history.History
                                        .Where(x => x.Method == icsCalendar.Method)
                                        .Select(x => x.Events.FirstOrDefault())
                                        .Any(x => x.Uid == icsEvent.Uid &&
                                                    x.Sequence == icsEvent.Sequence &&
                                                    DDayICalParser.ToUtc(x.DtStamp) == DDayICalParser.ToUtc(icsEvent.DtStamp));
                if (exist) return history;

                history.Ics = history.Ics + Environment.NewLine + ics;
                var newHistory = new CalendarEventHistory
                {
                    Tenant = TenantManager.GetCurrentTenant().TenantId,
                    CalendarId = calendarId,
                    EventUid = eventUid,
                    EventId = eventId,
                    Ics = history.Ics
                };
                CalendarDb.AddOrUpdate(r => r.CalendarEventHistory, newHistory);

            }
            CalendarDb.SaveChanges();
            tx.Commit();

            return history;
        }

        public void RemoveEventHistory(int calendarId, string eventUid)
        {
            using var tx = CalendarDb.Database.BeginTransaction();
            var eh = CalendarDb.CalendarEventHistory
                .Where(r =>
                        r.Tenant == TenantManager.GetCurrentTenant().TenantId &&
                        r.CalendarId == calendarId &&
                        r.EventUid == eventUid
                      )
                .SingleOrDefault();

            if (eh != null)
            {
                CalendarDb.CalendarEventHistory.Remove(eh);
            }

            CalendarDb.SaveChanges();
            tx.Commit();
        }

        public void RemoveEventHistory(int eventId)
        {
            using var tx = CalendarDb.Database.BeginTransaction();
            var eh = CalendarDb.CalendarEventHistory
                .Where(r =>
                        r.Tenant == TenantManager.GetCurrentTenant().TenantId &&
                        r.EventId == eventId
                      )
                .SingleOrDefault();

            if (eh != null)
            {
                CalendarDb.CalendarEventHistory.Remove(eh);
            }

            CalendarDb.SaveChanges();
            tx.Commit();
        }
        /*
        private static EventHistory ToEventHistory(object[] row)
        {
            return new EventHistory(Convert.ToInt32(row[0]),
                                    Convert.ToString(row[1]),
                                    Convert.ToInt32(row[2]),
                                    Convert.ToString(row[3]));
        }*/
        public static string GetEventUid(string uid, string id = null)
        {
            if (!string.IsNullOrEmpty(uid))
                return uid;

            return string.Format("{0}@onlyoffice.com", string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id);

        }

        // #region Event Notifications

        internal static int GetBeforeMinutes(EventAlertType eventAlertType)
        {
            switch (eventAlertType)
            {
                case EventAlertType.Day:
                    return -24 * 60;
                case EventAlertType.FifteenMinutes:
                    return -15;
                case EventAlertType.FiveMinutes:
                    return -5;
                case EventAlertType.HalfHour:
                    return -30;
                case EventAlertType.Hour:
                    return -60;
                case EventAlertType.TwoHours:
                    return -120;
            }

            return 0;
        }
        private DateTime GetNextAlertDate(DateTime utcStartDate, RecurrenceRule rrule, EventAlertType eventAlertType, TimeZoneInfo timeZone, bool isAllDayLong)
        {
            if (eventAlertType == EventAlertType.Never)
                return DateTime.MinValue;

            var offset = timeZone.GetOffset();
            var localFromDate = DateTime.UtcNow.Add(offset);
            var localStartDate = isAllDayLong ? utcStartDate : utcStartDate.Add(offset);
            var dates = rrule.GetDates(localStartDate, localFromDate, 3);
            for (var i = 0; i < dates.Count; i++)
            {
                dates[i] = dates[i].Subtract(offset);
            }

            foreach (var d in dates)
            {
                var dd = d.AddMinutes(GetBeforeMinutes(eventAlertType));
                if (dd > DateTime.UtcNow)
                    return dd;
            }

            return DateTime.MinValue;
        }
        private class UserAlertType
        {
            public Guid UserId { get; set; }
            public EventAlertType AlertType { get; set; }
            public TimeZoneInfo TimeZone { get; set; }


            public UserAlertType(Guid userId, EventAlertType alertType, TimeZoneInfo timeZone)
            {
                UserId = userId;
                AlertType = alertType;
                TimeZone = timeZone;
            }
        }
        private void UpdateEventNotifications(int eventId, int calendarId, DateTime eventUtcStartDate, EventAlertType baseEventAlertType, RecurrenceRule rrule,
            IEnumerable<SharingOptions.PublicItem> eventPublicItems,
            IEnumerable<SharingOptions.PublicItem> calendarPublicItems,
            bool isAllDayLong)
        {
            var eventUsersData =
                from calEventUser in CalendarDb.CalendarEventUser
                where
                calEventUser.EventId == eventId
                select new
                {
                    userIdCol = calEventUser.UserId,
                    alertTypeCol = calEventUser.AlertType,
                    isUnsubscribeCol = calEventUser.IsUnsubscribe
                };
            var calendarData =
                from cal in CalendarDb.CalendarCalendars
                where
                cal.Id == calendarId
                select new
                {
                    alertType = cal.AlertType,
                    ownerId = cal.OwnerId,
                    timeZone = cal.TimeZone
                };

            var calendarAlertType = (EventAlertType)calendarData.FirstOrDefault().alertType;
            var calendarOwner = Convert.ToString(calendarData.FirstOrDefault().ownerId);
            var calendarTimeZone = TimeZoneConverter.GetTimeZone(calendarData.FirstOrDefault().timeZone);

            var eventUsers = new List<UserAlertType>();

            #region shared event's data

            if (eventPublicItems == null)
            {
                var epItems =
                    from cei in CalendarDb.CalendarEventItem
                    where
                    cei.EventId == eventId
                    select new SharingOptions.PublicItem
                    {
                        Id = cei.ItemId,
                        IsGroup = Convert.ToBoolean(cei.IsGroup)
                    };
                eventPublicItems = new List<SharingOptions.PublicItem>(epItems);
            }

            foreach (var item in eventPublicItems)
            {
                if (item.IsGroup)
                    eventUsers.AddRange(UserManager.GetUsersByGroup(item.Id).Select(u => new UserAlertType(u.ID, baseEventAlertType, calendarTimeZone)));
                else
                    eventUsers.Add(new UserAlertType(item.Id, baseEventAlertType, calendarTimeZone));
            }

            //remove calendar owner
            eventUsers.RemoveAll(u => u.UserId.Equals(calendarOwner));

            //remove unsubscribed and exec personal alert_type
            if (eventUsers.Count > 0)
            {
                foreach (var r in eventUsersData)
                {
                    if (Convert.ToBoolean(r.isUnsubscribeCol))
                        eventUsers.RemoveAll(u => u.UserId.Equals(r.userIdCol));
                    else
                        eventUsers.ForEach(u =>
                        {
                            if (u.UserId.Equals(r.userIdCol))
                                u.AlertType = (EventAlertType)r.alertTypeCol;
                        });

                }
            }

            //remove and exec sharing calendar options
            if (eventUsers.Count > 0)
            {
                var userIds = eventUsers.Select(u => u.UserId).ToArray();
                var extCalendarAlertTypes =
                     from calUser in CalendarDb.CalendarCalendarUser
                     where
                     calUser.CalendarId.ToString() == SharedEventsCalendar.CalendarId &&
                     userIds.Contains(calUser.UserId)
                     select new
                     {
                         userId = calUser.UserId,
                         alertType = calUser.AlertType,
                         isAccepted = calUser.IsAccepted,
                         timeZone = calUser.TimeZone
                     };
                foreach (var r in extCalendarAlertTypes)
                {
                    if (!Convert.ToBoolean(r.isAccepted))
                    {
                        //remove unsubscribed from shared events calendar
                        eventUsers.RemoveAll(u => u.UserId.Equals(r.userId));
                        continue;
                    }
                    eventUsers.ForEach(u =>
                    {
                        if (u.UserId.Equals(r.userId))
                            u.TimeZone = r.timeZone == null ? calendarTimeZone : TimeZoneConverter.GetTimeZone(Convert.ToString(r.isAccepted));

                        if (u.AlertType == EventAlertType.Default && u.UserId.Equals(r.userId))
                            u.AlertType = (EventAlertType)Convert.ToInt32(r.alertType);
                    });
                }

                eventUsers.ForEach(u =>
                {
                    if (u.AlertType == EventAlertType.Default)
                        u.AlertType = EventAlertType.Hour;
                });

            }
            #endregion

            #region calendar's data

            if (calendarPublicItems == null)
            {
                var cpItems =
                    from cpi in CalendarDb.CalendarCalendarItem
                    where
                        cpi.CalendarId == calendarId
                    select new SharingOptions.PublicItem
                    {
                        Id = cpi.ItemId,
                        IsGroup = Convert.ToBoolean(cpi.IsGroup)
                    };

                calendarPublicItems = new List<SharingOptions.PublicItem>(cpItems);
            }

            //calendar users
            var calendarUsers = new List<UserAlertType>();
            foreach (var item in eventPublicItems)
            {
                if (item.IsGroup)
                    calendarUsers.AddRange(UserManager.GetUsersByGroup(item.Id).Select(u => new UserAlertType(u.ID, baseEventAlertType, calendarTimeZone)));
                else
                    calendarUsers.Add(new UserAlertType(item.Id, baseEventAlertType, calendarTimeZone));
            }

            calendarUsers.Add(new UserAlertType(Guid.Parse(calendarOwner), baseEventAlertType, calendarTimeZone));

            //remove event's users
            calendarUsers.RemoveAll(u => eventUsers.Exists(eu => eu.UserId.Equals(u.UserId)));

            //calendar options            
            if (calendarUsers.Count > 0)
            {
                //set personal alert_type
                foreach (var r in eventUsersData)
                {

                    eventUsers.ForEach(u =>
                   {
                       if (u.UserId.Equals(r.userIdCol))
                           u.AlertType = (EventAlertType)(Convert.ToInt32(r.alertTypeCol));
                   });

                }
                var userIds = calendarUsers.Select(u => u.UserId).ToArray();
                var calendarAlertTypes =
                     from calUser in CalendarDb.CalendarCalendarUser
                     where
                     calUser.CalendarId == calendarId &&
                     userIds.Contains(calUser.UserId)
                     select new
                     {
                         userId = calUser.UserId,
                         alertType = calUser.AlertType,
                         isAccepted = calUser.IsAccepted,
                         timeZone = calUser.TimeZone
                     };

                foreach (var r in calendarAlertTypes)
                {
                    if (!Convert.ToBoolean(r.isAccepted))
                    {
                        //remove unsubscribed
                        calendarUsers.RemoveAll(u => u.UserId.Equals(r.userId));
                        continue;
                    }
                    calendarUsers.ForEach(u =>
                    {
                        if (u.UserId.Equals(r.userId))
                            u.TimeZone = r.timeZone == null ? calendarTimeZone : TimeZoneConverter.GetTimeZone(r.timeZone);

                        if (u.AlertType == EventAlertType.Default && u.UserId.Equals(r.userId))
                            u.AlertType = (EventAlertType)Convert.ToInt32(r.alertType);
                    });
                }

                calendarUsers.ForEach(u =>
                {
                    if (u.AlertType == EventAlertType.Default)
                        u.AlertType = calendarAlertType;
                });
            }

            #endregion

            //clear notifications
            using var tx = CalendarDb.Database.BeginTransaction();
            var en = CalendarDb.CalendarNotifications
                 .Where(r => r.EventId == eventId)
                 .SingleOrDefault();
            if (en != null)
            {
                CalendarDb.CalendarNotifications.Remove(en);
            }

            eventUsers.AddRange(calendarUsers);

            foreach (var u in eventUsers)
            {
                //todo: recount
                var alertDate = GetNextAlertDate(eventUtcStartDate, rrule, u.AlertType, u.TimeZone, isAllDayLong);
                if (!alertDate.Equals(DateTime.MinValue))
                {
                    var calNotification = new CalendarNotifications
                    {
                        UserId = u.UserId,
                        EventId = eventId,
                        NotifyDate = alertDate,
                        Tenant = TenantManager.GetCurrentTenant().TenantId,
                        AlertType = (int)u.AlertType,
                        TimeZone = u.TimeZone.Id,
                        Rrule = rrule.ToString()
                    };
                    CalendarDb.CalendarNotifications.Add(calNotification);
                }
            }
            CalendarDb.SaveChanges();
            tx.Commit();
            /*
            public List<EventNotificationData> ExtractAndRecountNotifications(DateTime utcDate)
            {
                List<EventNotificationData> data;
                using (var tr = db.BeginTransaction())
                {
                    var cc = new ColumnCollection();
                    var userIdCol = cc.RegistryColumn("user_id");
                    var tenantCol = cc.RegistryColumn("tenant");
                    var eventIdCol = cc.RegistryColumn("event_id");
                    var notifyDateCol = cc.RegistryColumn("notify_date");
                    var rruleCol = cc.RegistryColumn("rrule");
                    var alertTypeCol = cc.RegistryColumn("alert_type");
                    var timeZoneCol = cc.RegistryColumn("time_zone");

                    data = new List<EventNotificationData>(db.ExecuteList(new SqlQuery("calendar_notifications").Select(cc.SelectQuery)
                                            .Where(Exp.Le(notifyDateCol.Name, utcDate)))
                                            .Select(r => new EventNotificationData
                                            {
                                                UserId = userIdCol.Parse<Guid>(r),
                                                TenantId = tenantCol.Parse<int>(r),
                                                EventId = eventIdCol.Parse<int>(r),
                                                NotifyUtcDate = notifyDateCol.Parse<DateTime>(r),
                                                RRule = rruleCol.Parse<RecurrenceRule>(r),
                                                AlertType = (EventAlertType)alertTypeCol.Parse<int>(r),
                                                TimeZone = timeZoneCol.Parse<TimeZoneInfo>(r)
                                            }));


                    var events = GetEventsByIds(data.Select(d => (object)d.EventId).Distinct().ToArray(), Guid.Empty);
                    data.ForEach(d => d.Event = events.Find(e => String.Equals(e.Id, d.EventId.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase)));

                    foreach (var d in data)   
                    {
                        if (d.RRule.Freq == Frequency.Never)
                            db.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where(Exp.Eq("user_id", d.UserId) & Exp.Eq("event_id", d.EventId)));
                        else
                        {
                            var alertDate = GetNextAlertDate(d.Event.UtcStartDate, d.RRule, d.AlertType, d.TimeZone, d.Event.AllDayLong);
                            if (!alertDate.Equals(DateTime.MinValue))
                            {
                                db.ExecuteNonQuery(new SqlInsert("calendar_notifications", true).InColumnValue("user_id", d.UserId)
                                                                                                    .InColumnValue("event_id", d.EventId)
                                                                                                    .InColumnValue("rrule", d.RRule.ToString())
                                                                                                    .InColumnValue("alert_type", (int)d.AlertType)
                                                                                                    .InColumnValue("tenant", d.TenantId)
                                                                                                    .InColumnValue("notify_date", alertDate)
                                                                                                    .InColumnValue("time_zone", d.TimeZone.Id));
                            }
                            else
                                db.ExecuteNonQuery(new SqlDelete("calendar_notifications").Where(Exp.Eq("user_id", d.UserId) & Exp.Eq("event_id", d.EventId)));
                        }
                    }

                    tr.Commit();
                }

                return data;
            }

            #endregion

            public void Dispose()
            {
                if (HttpContext.Current == null && db != null)
                {
                    db.Dispose();
                }
            }*/
        }
    }
    public static class CalendarDataProviderExtention
    {
        public static DIHelper AddCalendarDataProviderService(this DIHelper services)
        {

            services.TryAddScoped<DataProvider>();

            return services
                .AddEventHistoryHelper();
        }
    }
}
