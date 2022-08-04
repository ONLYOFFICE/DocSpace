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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ASC.Api.Core;
using ASC.Calendar.BusinessObjects;
using ASC.Calendar.Configuration;
using ASC.Calendar.Core;
using ASC.Calendar.ExternalCalendars;
using ASC.Calendar.iCalParser;
using ASC.Calendar.Models;
using ASC.Calendar.Notification;
using ASC.Common;
using ASC.Common.Security;
using ASC.Common.Utils;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Calendars;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Calendar.Controllers
{
    [DefaultRoute]
    [ApiController]
    [Scope]
    public class CalendarController : ControllerBase
    {
        private const int _monthCount = 3;
        private static readonly List<String> updatedEvents = new List<string>();
        private ProductEntryPoint ProductEntryPoint { get; }

        private Tenant Tenant { get { return ApiContext.Tenant; } }
        private ApiContext ApiContext { get; }
        private AuthContext AuthContext { get; }
        private UserManager UserManager { get; }
        private DataProvider DataProvider { get; }
        private ILogger<CalendarController> Log { get; }
        private TenantManager TenantManager { get; }
        private TimeZoneConverter TimeZoneConverter { get; }
        private CalendarWrapperHelper CalendarWrapperHelper { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private AuthorizationManager AuthorizationManager { get; }
        private AuthManager Authentication { get; }
        private CalendarNotifyClient CalendarNotifyClient { get; }
        private DDayICalParser DDayICalParser { get; }
        private new HttpContext HttpContext { get; set; }
        private PermissionContext PermissionContext { get; }
        private EventHistoryWrapperHelper EventHistoryWrapperHelper { get; }
        private EventWrapperHelper EventWrapperHelper { get; }
        private EventHistoryHelper EventHistoryHelper { get; }
        private PublicItemCollectionHelper PublicItemCollectionHelper { get; }
        private TodoWrapperHelper TodoWrapperHelper { get; }
        private Signature Signature { get; }
        private SecurityContext SecurityContext { get; }
        private ExportDataCache ExportDataCache { get; }
        private SubscriptionWrapperHelper SubscriptionWrapperHelper { get; }
        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }
        private SetupInfo SetupInfo { get; }
        private InstanceCrypto InstanceCrypto { get; }
        private CalendarManager CalendarManager { get; }
        private IHttpClientFactory ClientFactory { get; }

        public CalendarController(

            ApiContext apiContext,
            AuthContext authContext,
            AuthorizationManager authorizationManager,
            UserManager userManager,
            TenantManager tenantManager,
            TimeZoneConverter timeZoneConverter,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            ILogger<CalendarController> option,
            DDayICalParser dDayICalParser,
            DataProvider dataProvider,
            IHttpContextAccessor httpContextAccessor,
            CalendarWrapperHelper calendarWrapperHelper,
            AuthManager authentication,
            CalendarNotifyClient calendarNotifyClient,
            PermissionContext permissionContext,
            EventHistoryWrapperHelper eventHistoryWrapperHelper,
            EventWrapperHelper eventWrapperHelper,
            EventHistoryHelper eventHistoryHelper,
            PublicItemCollectionHelper publicItemCollectionHelper,
            TodoWrapperHelper todoWrapperHelper,
            Signature signature,
            SecurityContext securityContext,
            ExportDataCache exportDataCache,
            SubscriptionWrapperHelper subscriptionWrapperHelper,
            EmailValidationKeyProvider emailValidationKeyProvider,
            SetupInfo setupInfo,
            InstanceCrypto instanceCrypto,
            CalendarManager calendarManager,
            ProductEntryPoint productEntryPoint,
            IHttpClientFactory clientFactory)
        {
            AuthContext = authContext;
            Authentication = authentication;
            AuthorizationManager = authorizationManager;
            TenantManager = tenantManager;
            Log = option;
            TimeZoneConverter = timeZoneConverter;
            ApiContext = apiContext;
            UserManager = userManager;
            DataProvider = dataProvider;
            CalendarWrapperHelper = calendarWrapperHelper;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            CalendarNotifyClient = calendarNotifyClient;
            DDayICalParser = dDayICalParser;
            PermissionContext = permissionContext;
            EventHistoryWrapperHelper = eventHistoryWrapperHelper;
            EventWrapperHelper = eventWrapperHelper;
            EventHistoryHelper = eventHistoryHelper;
            PublicItemCollectionHelper = publicItemCollectionHelper;
            TodoWrapperHelper = todoWrapperHelper;
            Signature = signature;
            SecurityContext = securityContext;
            ExportDataCache = exportDataCache;
            SubscriptionWrapperHelper = subscriptionWrapperHelper;
            EmailValidationKeyProvider = emailValidationKeyProvider;
            SetupInfo = setupInfo;
            InstanceCrypto = instanceCrypto;
            CalendarManager = calendarManager;
            ProductEntryPoint = productEntryPoint;
            ClientFactory = clientFactory;

            CalendarManager.RegistryCalendar(new SharedEventsCalendar(AuthContext, TimeZoneConverter, TenantManager, DataProvider));
            var birthdayReminderCalendar = new BirthdayReminderCalendar(AuthContext, TimeZoneConverter, UserManager, DisplayUserSettingsHelper);
            if (UserManager.IsUserInGroup(AuthContext.CurrentAccount.ID, Constants.GroupVisitor.ID))
            {
                CalendarManager.UnRegistryCalendar(birthdayReminderCalendar.Id);
            }
            else
            {
                CalendarManager.RegistryCalendar(birthdayReminderCalendar);
            }
            HttpContext = httpContextAccessor?.HttpContext;
        }
        public class SharingParam : SharingOptions.PublicItem
        {
            public string actionId { get; set; }
            public Guid itemId
            {
                get { return Id; }
                set { Id = value; }
            }
            public bool isGroup
            {
                get { return IsGroup; }
                set { IsGroup = value; }
            }
        }

        public enum EventRemoveType
        {
            Single = 0,
            AllFollowing = 1,
            AllSeries = 2
        }

        [HttpGet("info")]
        public Module GetModule()
        {
            ProductEntryPoint.Init();
            return new Module(ProductEntryPoint);
        }


        /// <summary>
        /// Returns the list of all subscriptions available to the user
        /// </summary>
        /// <short>
        /// Subscription list
        /// </short>
        /// <returns>List of subscriptions</returns>
        [HttpGet("subscriptions")]
        public List<SubscriptionWrapper> LoadSubscriptions()
        {
            var result = new List<SubscriptionWrapper>();

            var calendars = DataProvider.LoadSubscriptionsForUser(SecurityContext.CurrentAccount.ID);
            result.AddRange(calendars.FindAll(c => !c.OwnerId.Equals(SecurityContext.CurrentAccount.ID)).ConvertAll(c => SubscriptionWrapperHelper.Get(c)));

            var iCalStreams = DataProvider.LoadiCalStreamsForUser(SecurityContext.CurrentAccount.ID);
            result.AddRange(iCalStreams.ConvertAll(c => SubscriptionWrapperHelper.Get(c)));


            var extCalendars = CalendarManager.GetCalendarsForUser(SecurityContext.CurrentAccount.ID, UserManager);
            var viewSettings = DataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, extCalendars.ConvertAll(c => c.Id));

            result.AddRange(extCalendars.ConvertAll(c =>
                                    SubscriptionWrapperHelper.Get(c, viewSettings.Find(o => o.CalendarId.Equals(c.Id, StringComparison.InvariantCultureIgnoreCase)))));


            //TODO For personal
            /*else
            {
                var iCalStreams = DataProvider.LoadiCalStreamsForUser(SecurityContext.CurrentAccount.ID);
                result.AddRange(iCalStreams.ConvertAll(c => new SubscriptionWrapper(c)));
            }*/

            return result;
        }
        public class SubscriptionState
        {
            public string id { get; set; }
            public bool isAccepted { get; set; }
        }

        public class SubscriptionModel
        {
            public IEnumerable<SubscriptionState> States { get; set; }
        }

        /// <summary>
        /// Updates the subscription state either subscribing or unsubscribing the user to/from it
        /// </summary>
        /// <short>
        /// Update subscription
        /// </short>
        /// <param name="states">Updated subscription states</param>
        /// <visible>false</visible>
        [HttpPut("subscriptions/manage")]
        public void ManageSubscriptions(SubscriptionModel subscriptionModel)
        {
            var states = subscriptionModel.States;
            var viewSettings = DataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, states.Select(s => s.id).ToList());

            var settingsCollection = new List<UserViewSettings>();
            foreach (var s in states)
            {
                var settings = viewSettings.Find(vs => vs.CalendarId.Equals(s.id, StringComparison.InvariantCultureIgnoreCase));
                if (settings == null)
                {
                    settings = new UserViewSettings
                    {
                        CalendarId = s.id,
                        UserId = SecurityContext.CurrentAccount.ID
                    };
                }
                settings.IsAccepted = s.isAccepted;
                settingsCollection.Add(settings);

            }
            DataProvider.UpdateCalendarUserView(settingsCollection);
        }

        /// <summary>
        /// Returns the list of all dates which contain the events from the displayed calendars
        /// </summary>
        /// <short>
        /// Calendar events
        /// </short>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Date list</returns>
        /// <visible>false</visible>
        [HttpGet("eventdays/{startDate}/{endDate}")]
        public List<ApiDateTime> GetEventDays(ApiDateTime startDate, ApiDateTime endDate)
        {
            var result = new List<CalendarWrapper>();
            int newCalendarsCount;
            //internal
            var calendars = DataProvider.LoadCalendarsForUser(SecurityContext.CurrentAccount.ID, out newCalendarsCount);

            result.AddRange(calendars.ConvertAll(c => CalendarWrapperHelper.Get(c)));


            //external
            var extCalendars = CalendarManager.GetCalendarsForUser(SecurityContext.CurrentAccount.ID, UserManager);
            var viewSettings = DataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, extCalendars.ConvertAll(c => c.Id));

            var extCalendarsWrappers = extCalendars.ConvertAll(c =>
                                      CalendarWrapperHelper.Get(c, viewSettings.Find(o => o.CalendarId.Equals(c.Id, StringComparison.InvariantCultureIgnoreCase)))
                                    )
                                    .FindAll(c => c.IsAcceptedSubscription);


            extCalendarsWrappers.ForEach(c => c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, EventWrapperHelper));
            var sharedEvents = extCalendarsWrappers.Find(c => String.Equals(c.Id, SharedEventsCalendar.CalendarId, StringComparison.InvariantCultureIgnoreCase));


            if (sharedEvents != null)
                result.ForEach(c =>
                {
                    c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, EventWrapperHelper);
                    c.Events.RemoveAll(e => sharedEvents.Events.Exists(sEv => string.Equals(sEv.Id, e.Id, StringComparison.InvariantCultureIgnoreCase)));
                });
            else
                result.ForEach(c => c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, EventWrapperHelper));

            result.AddRange(extCalendarsWrappers);

            //TODO for personal
            /*
                //remove all subscription except ical streams
                result.RemoveAll(c => c.IsSubscription && !c.IsiCalStream);

                result.ForEach(c => c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate));
            */

            var days = new List<ApiDateTime>();
            foreach (var cal in result)
            {
                if (cal.IsHidden)
                    continue;

                foreach (var e in cal.Events)
                {
                    var d = (e.Start.UtcTime + e.Start.TimeZoneOffset).Date;
                    var dend = (e.End.UtcTime + e.End.TimeZoneOffset).Date;
                    while (d <= dend)
                    {
                        if (!days.Exists(day => day == d))
                            days.Add(new ApiDateTime(d, TimeZoneInfo.Utc.GetOffset()));

                        d = d.AddDays(1);
                    }

                }
            }

            return days;
        }

        /// <summary>
        /// Returns the list of calendars and subscriptions with the events for the current user for the selected period
        /// </summary>
        /// <short>
        /// Calendars and subscriptions
        /// </short>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>List of calendars and subscriptions with events</returns>
        [HttpGet("calendars/{startDate}/{endDate}")]
        public List<CalendarWrapper> LoadCalendars(ApiDateTime startDate, ApiDateTime endDate)
        {
            var result = LoadInternalCalendars();

            //external

            var extCalendars = CalendarManager.GetCalendarsForUser(SecurityContext.CurrentAccount.ID, UserManager);
            var viewSettings = DataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, extCalendars.ConvertAll(c => c.Id));

            var extCalendarsWrappers = extCalendars.ConvertAll(c =>
                                      CalendarWrapperHelper.Get(c, viewSettings.Find(o => o.CalendarId.Equals(c.Id, StringComparison.InvariantCultureIgnoreCase)))
                                    )
                                    .FindAll(c => c.IsAcceptedSubscription);


            extCalendarsWrappers.ForEach(c =>
                {
                    c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, EventWrapperHelper);
                    c.Todos = c.UserCalendar.GetTodoWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, TodoWrapperHelper);
                });

            var sharedEvents = extCalendarsWrappers.Find(c => String.Equals(c.Id, SharedEventsCalendar.CalendarId, StringComparison.InvariantCultureIgnoreCase));
            if (sharedEvents != null)
                result.ForEach(c =>
                {
                    c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, EventWrapperHelper);
                    c.Todos = c.UserCalendar.GetTodoWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, TodoWrapperHelper);
                    c.Events.RemoveAll(e => sharedEvents.Events.Exists(sEv => string.Equals(sEv.Id, e.Id, StringComparison.InvariantCultureIgnoreCase)));
                });
            else
                result.ForEach(c =>
                {
                    c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, EventWrapperHelper);
                    c.Todos = c.UserCalendar.GetTodoWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, TodoWrapperHelper);
                });

            result.AddRange(extCalendarsWrappers);

            //TODO For personal
            /*
                //remove all subscription except ical streams
                result.RemoveAll(c => c.IsSubscription && !c.IsiCalStream);
                result.ForEach(c => c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate));
            */

            return result;
        }

        /// <summary>
        /// Returns the detailed information about the calendar with the ID specified in the request
        /// </summary>
        /// <short>
        /// Calendar by ID
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <returns>Calendar</returns>
        [HttpGet("{calendarId}")]
        public CalendarWrapper GetCalendarById(string calendarId)
        {
            int calId;
            if (int.TryParse(calendarId, out calId))
            {
                var calendars = DataProvider.GetCalendarById(calId);

                return (calendars != null ? CalendarWrapperHelper.Get(calendars) : null);
            }

            var extCalendar = CalendarManager.GetCalendarForUser(AuthContext.CurrentAccount.ID, calendarId, UserManager);
            if (extCalendar != null)
            {
                var viewSettings = DataProvider.GetUserViewSettings(AuthContext.CurrentAccount.ID, new List<string> { calendarId });
                return CalendarWrapperHelper.Get(extCalendar, viewSettings.FirstOrDefault());
            }
            return null;
        }

        /// <summary>
        /// Returns the link for the iCal associated with the calendar with the ID specified in the request
        /// </summary>
        /// <short>
        /// Get iCal link
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <returns>iCal link</returns>
        [HttpGet("{calendarId}/icalurl")]
        public object GetCalendariCalUrl(string calendarId)
        {
            var sig = Signature.Create(AuthContext.CurrentAccount.ID, calendarId);

            // var path = UrlPath.ResolveUrl(() => new CalendarApi().GetCalendariCalStream(calendarId, sig));

            var path = "api/2.0/calendar/" + calendarId + "/ical/" + sig;

            var result = new Uri(HttpContext.Request.GetUrlRewriter(), VirtualPathUtility.ToAbsolute("~/" + path)).ToString();

            return new { result };
        }

        /// <summary>
        /// Returns the link for the CalDav associated with the calendar with the ID specified in the request
        /// </summary>
        /// <short>
        /// Get CalDav link
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <returns>CalDav link</returns>
        [HttpGet("{calendarId}/caldavurl")]
        public string GetCalendarCalDavUrl(string calendarId)
        {
            var myUri = HttpContext.Request.GetUrlRewriter();

            var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + ":" + myUri.Port + "/caldav";
            var caldavHost = myUri.Host;

            var userId = SecurityContext.CurrentAccount.ID;
            var user = UserManager.GetUsers(userId);
            if (!DataProvider.CheckUserEmail(user)) return "";
            var userName = UserManager.GetUsers(userId).Email.ToLower();

            var curCaldavUserName = userName + "@" + caldavHost;

            if (calendarId == "todo_calendar")
            {
                var todoCalendars = DataProvider.LoadTodoCalendarsForUser(SecurityContext.CurrentAccount.ID);
                var userTimeZone = TenantManager.GetCurrentTenant().TimeZone;

                var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager, ClientFactory);
                var newCalendar = new BusinessObjects.Calendar(AuthContext, TimeZoneConverter, icalendar, DataProvider);
                var todoCal = CalendarWrapperHelper.Get(newCalendar);

                if (todoCalendars.Count == 0)
                {
                    var todoCalModel = new CalendarModel
                    {
                        Name = "Todo_calendar",
                        Description = "",
                        TextColor = BusinessObjects.Calendar.DefaultTextColor,
                        BackgroundColor = BusinessObjects.Calendar.DefaultTodoBackgroundColor,
                        TimeZone = userTimeZone,
                        AlertType = EventAlertType.FifteenMinutes,
                        SharingOptions = null,
                        ICalUrl = null,
                        IsTodo = 1
                    };
                    todoCal = CreateCalendar(todoCalModel);

                    if (todoCal != null)
                    {
                        try
                        {
                            var dataCaldavGuid = DataProvider.GetCalDavGuid(todoCal.Id);

                            var caldavGuid = dataCaldavGuid != null
                                        ? Guid.Parse(dataCaldavGuid)
                                        : Guid.Empty;

                            var sharedCalUrl = new Uri(new Uri(calDavServerUrl), "/caldav/" + curCaldavUserName + "/" + caldavGuid).ToString();
                            var calendar = GetUserCaldavCalendar(sharedCalUrl, userName);
                            if (calendar.Length != 0)
                            {
                                if (calendar == "NotFound")
                                {
                                    return CreateCalDavCalendar(
                                                            "Todo_calendar",
                                                            "",
                                                            BusinessObjects.Calendar.DefaultTodoBackgroundColor,
                                                            caldavGuid.ToString(),
                                                            myUri,
                                                            curCaldavUserName,
                                                            userName
                                                        );
                                }
                                return sharedCalUrl;
                            }
                            else
                            {
                                return "";
                            }

                        }
                        catch (Exception exception)
                        {
                            Log.LogError("ERROR: " + exception.Message);
                            return "";
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    var sharedCalUrl = new Uri(new Uri(calDavServerUrl), "/caldav/" + curCaldavUserName + "/" + todoCalendars[0].calDavGuid).ToString();
                    var calendar = GetUserCaldavCalendar(sharedCalUrl, userName);
                    if (calendar.Length != 0)
                    {
                        if (calendar == "NotFound")
                        {
                            return CreateCalDavCalendar(
                                                    "Todo_calendar",
                                                    "",
                                                    BusinessObjects.Calendar.DefaultTodoBackgroundColor,
                                                    todoCalendars[0].calDavGuid,
                                                    myUri,
                                                    curCaldavUserName,
                                                    userName
                                                );
                        }
                        return sharedCalUrl;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (calendarId == BirthdayReminderCalendar.CalendarId ||
                calendarId == SharedEventsCalendar.CalendarId ||
                calendarId == "crm_calendar" ||
                calendarId.Contains("Project_"))
            {

                if (SecurityContext.IsAuthenticated)
                {
                    var sharedCalendar = GetCalendarById(calendarId);

                    var currentCaldavUserName = userName + "@" + caldavHost;
                    var sharedCalUrl = new Uri(new Uri(calDavServerUrl), "/caldav/" + currentCaldavUserName + "/" + calendarId + "-shared").ToString();

                    var calendar = GetUserCaldavCalendar(sharedCalUrl, userName);

                    if (calendar.Length != 0)
                    {
                        if (calendar == "NotFound")
                        {
                            sharedCalUrl = CreateCalDavCalendar(
                                sharedCalendar.UserCalendar.Name,
                                sharedCalendar.UserCalendar.Description,
                                sharedCalendar.TextColor,
                                calendarId,
                                myUri,
                                currentCaldavUserName,
                                userName,
                                true
                                );
                        }
                        if (sharedCalUrl.Length != 0)
                        {
                            var calendarIcs = GetCalendariCalString(calendarId, true);

                            var tenant = TenantManager.GetCurrentTenant();
                            var caldavTask = new Task(() => CreateCaldavSharedEvents(calendarId, calendarIcs, myUri, userName, sharedCalendar.UserCalendar, SecurityContext.CurrentAccount, tenant.Id));
                            caldavTask.Start();

                            return sharedCalUrl;
                        }
                    }
                }
                return "";
            }

            var cal = DataProvider.GetCalendarById(Convert.ToInt32(calendarId));
            var ownerId = cal.OwnerId;

            TenantManager.SetCurrentTenant(cal.TenantId);

            var isShared = ownerId != SecurityContext.CurrentAccount.ID;
            var calDavGuid = cal.calDavGuid;
            if (calDavGuid.Length == 0 || calDavGuid == Guid.Empty.ToString())
            {
                calDavGuid = Guid.NewGuid().ToString();
                DataProvider.UpdateCalendarGuid(Convert.ToInt32(cal.Id), Guid.Parse(calDavGuid));
            }

            var calUrl = new Uri(new Uri(calDavServerUrl), "/caldav/" + curCaldavUserName + "/" + calDavGuid + (isShared ? "-shared" : "")).ToString();

            Log.LogInformation("RADICALE REWRITE URL: " + myUri);

            var caldavCalendar = GetUserCaldavCalendar(calUrl, userName);

            if (caldavCalendar.Length != 0)
            {
                if (caldavCalendar == "NotFound")
                {
                    return SyncCaldavCalendar(calendarId, cal.Name, cal.Description, cal.Context.HtmlBackgroundColor, Guid.Parse(calDavGuid), myUri, curCaldavUserName, userName, isShared, cal.SharingOptions);
                }
                return calUrl;
            }
            return "";
        }

        /// <summary>
        /// Run caldav event update function
        /// </summary>
        /// <short>
        /// Update CalDav Event
        /// </short>
        /// <param name="change">changes of event</param>
        /// <param name="key"></param>
        /// <visible>false</visible>
        [AllowAnonymous]
        [HttpGet("change_to_storage")] //NOTE: this method doesn't requires auth!!!
        public void ChangeOfCalendarStorage(string change, string key)
        {
            var authInterval = TimeSpan.FromHours(1);
            var checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(change + ConfirmType.Auth, key, authInterval);
            if (checkKeyResult != EmailValidationKeyProvider.ValidationResult.Ok) throw new SecurityException("Access Denied.");

            var urlRewriter = HttpContext.Request.GetUrlRewriter();
            var caldavUser = change.Split('/')[0];
            var portalName = caldavUser.Split('@')[2];

            if (change != null && portalName != null)
            {
                var calDavUrl = new Uri(urlRewriter.Scheme + "://" + portalName);
                var caldavTask = new Task(() => UpdateCalDavEvent(change, calDavUrl));
                caldavTask.Start();
            }
        }


        /// <summary>
        /// Deletes the project calendar with the ID specified in the request
        /// </summary>
        /// <short>
        /// Delete project calendar
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <param name="team">Project team</param>
        [HttpDelete("caldavprojcal")]
        public void DeleteCaldavCalendar(CaldavCalendarModel caldavCalendarModel)
        {
            var calendarId = caldavCalendarModel.CalendarId;
            var team = caldavCalendarModel.Team;

            try
            {
                var myUri = HttpContext.Request.GetUrlRewriter();
                var caldavHost = myUri.Host;

                var currentTenantId = TenantManager.GetCurrentTenant().Id;
                var deleteCaldavCalendar = new Thread(() =>
                {
                    TenantManager.SetCurrentTenant(currentTenantId);
                    foreach (var teamMember in team)
                    {
                        var currentUserEmail = UserManager.GetUsers(Guid.Parse(teamMember)).Email;
                        var currentUserName = currentUserEmail + "@" + caldavHost;
                        DataProvider.RemoveCaldavCalendar(
                            currentUserName,
                            currentUserEmail,
                            calendarId,
                            myUri,
                            true);
                    }
                });
                deleteCaldavCalendar.Start();
            }
            catch (Exception ex)
            {
                Log.LogError($"Delete project caldav calendar: {ex.Message}");
            }

        }

        /// <summary>
        /// Deletes the whole caldav event from the calendar
        /// </summary>
        /// <short>
        /// Delete event
        /// </short>
        /// <param name="calendarId">ID of the calendar where the event belongs</param>
        /// <param name="uid">Event uid</param>
        /// <param name="responsibles">Task responsibles</param>
        [HttpDelete("caldavevent")]
        public void DeleteCaldavEvent(CaldavEventModel caldavEventModel)
        {
            var calendarId = caldavEventModel.CalendarId;
            var uid = caldavEventModel.Uid;
            var responsibles = caldavEventModel.Responsibles;

            try
            {
                var currentUserId = SecurityContext.CurrentAccount.ID;
                var myUri = HttpContext.Request.GetUrlRewriter();
                if (responsibles == null || responsibles.Count == 0)
                {
                    var currentUserEmail = UserManager.GetUsers(currentUserId).Email;
                    deleteEvent(uid, calendarId, currentUserEmail, myUri, true);
                }
                else if (responsibles.Count > 0)
                {
                    var currentTenantId = TenantManager.GetCurrentTenant().Id;
                    var deleteCaldavEvent = new Thread(() =>
                    {
                        TenantManager.SetCurrentTenant(currentTenantId);
                        foreach (var responsibleSid in responsibles)
                        {
                            var currentUser = UserManager.GetUsers(Guid.Parse(responsibleSid));
                            if (DataProvider.CheckUserEmail(currentUser))
                            {
                                deleteEvent(
                                    uid,
                                    calendarId,
                                    currentUser.Email,
                                    myUri,
                                    true
                                );
                            }
                        }

                    });
                    deleteCaldavEvent.Start();
                }
            }
            catch (Exception ex)
            {
                Log.LogError($"Delete CRM caldav event: {ex.Message}");
            }

        }

        /// <summary>
        /// Create/update the existing caldav event in the selected calendar
        /// </summary>
        /// <short>
        /// Update caldav event
        /// </short>
        /// <param name="calendarId">ID of the calendar where the event belongs</param>
        /// <param name="uid">Event uid</param>
        /// <param name="alert">Event notification type</param>
        /// <param name="responsibles">Task responsibles</param>
        [HttpPut("caldavevent")]
        public void UpdateCaldavEvent(CaldavEventModel caldavEventModel)
        {
            var calendarId = caldavEventModel.CalendarId;
            var uid = caldavEventModel.Uid;
            var alert = caldavEventModel.Alert;
            var responsibles = caldavEventModel.Responsibles;

            try
            {
                if (responsibles.Count > 0)
                {
                    var myUri = HttpContext.Request.GetUrlRewriter();
                    var currentTenantId = TenantManager.GetCurrentTenant().Id;
                    var sharedCalendar = GetCalendarById(calendarId);

                    var currentUserId = Guid.Empty;
                    if (SecurityContext.IsAuthenticated)
                    {
                        currentUserId = SecurityContext.CurrentAccount.ID;
                        SecurityContext.Logout();
                    }

                    try
                    {
                        foreach (var responsibleSid in responsibles)
                        {
                            SecurityContext.AuthenticateMeWithoutCookie(Guid.Parse(responsibleSid));

                            var calendarIcs = GetCalendariCalString(calendarId, true);
                            var parseCalendar = DDayICalParser.DeserializeCalendar(calendarIcs);
                            var calendar = parseCalendar.FirstOrDefault();

                            if (calendar != null)
                            {
                                var events = calendar.Events;
                                var ddayCalendar = new Ical.Net.Calendar();
                                foreach (var evt in events)
                                {
                                    var eventUid = evt.Uid;
                                    string[] split = eventUid.Split(new Char[] { '@' });
                                    if (uid == split[0])
                                    {
                                        if (sharedCalendar == null)
                                        {
                                            sharedCalendar = GetCalendarById(calendarId);
                                        }
                                        if (sharedCalendar != null)
                                            ddayCalendar = getEventIcs(alert, sharedCalendar, evt, calendarId);
                                    }
                                }
                                var serializeIcs = DDayICalParser.SerializeCalendar(ddayCalendar);
                                var updateEvent = new Thread(() =>
                                {
                                    TenantManager.SetCurrentTenant(currentTenantId);
                                    var user = UserManager.GetUsers(Guid.Parse(responsibleSid));
                                    if (DataProvider.CheckUserEmail(user))
                                    {
                                        updateCaldavEvent(
                                            serializeIcs,
                                            uid,
                                            true,
                                            calendarId,
                                            myUri,
                                            user.Email,
                                            DateTime.Now,
                                            ddayCalendar.TimeZones[0],
                                            sharedCalendar.UserCalendar.TimeZone, false, true
                                        );
                                    }
                                });
                                updateEvent.Start();
                                SecurityContext.Logout();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.LogError($"Error: {ex.Message}");
                    }
                    finally
                    {
                        SecurityContext.Logout();
                        if (currentUserId != Guid.Empty)
                        {
                            SecurityContext.AuthenticateMeWithoutCookie(currentUserId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError($"Create/update CRM caldav event: {ex.Message}");
            }

        }


        private Ical.Net.Calendar getEventIcs(int alert, CalendarWrapper calendar, CalendarEvent evt, string calendarId)
        {
            var ddayCalendar = DDayICalParser.ConvertCalendar(calendar.UserCalendar);
            ddayCalendar.Events.Clear();
            evt.Created = null;
            if (calendarId.Contains("Project_"))
            {
                evt.End = new CalDateTime(evt.End.AddDays(1));
            }
            evt.Status = nameof(EventStatus.Confirmed);
            if (alert > 0)
            {
                evt.Alarms.Add(
                    new Alarm()
                    {
                        Action = "DISPLAY",
                        Description = "Reminder",
                        Trigger = new Trigger(TimeSpan.FromMinutes((-1) * alert))
                    }
                );
            }
            ddayCalendar.Events.Add(evt);
            return ddayCalendar;
        }
        /// <summary>
        /// Run caldav event delete function
        /// </summary>
        /// <short>
        /// Delete CalDav Event
        /// </short>
        /// <param name="eventInfo">event info</param>
        /// <param name="key"></param>
        /// <visible>false</visible>
        [AllowAnonymous]
        [HttpGet("caldav_delete_event")] //NOTE: this method doesn't requires auth!!!
        public void CaldavDeleteEvent(string eventInfo, string key)
        {
            var authInterval = TimeSpan.FromHours(1);
            var checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(eventInfo + ConfirmType.Auth, key, authInterval);
            if (checkKeyResult != EmailValidationKeyProvider.ValidationResult.Ok) throw new SecurityException("Access Denied.");

            if (eventInfo != null)
            {
                var myUri = HttpContext.Request.GetUrlRewriter();
                var calEvent = eventInfo.Split('/')[2].Replace("_write", "");
                var eventGuid = calEvent.Split('.')[0];

                var updateEventGuid = updatedEvents.Find((x) => x == eventGuid);
                if (updateEventGuid == null)
                {
                    Task.Run(() => DeleteCalDavEvent(eventInfo, myUri));
                }
                else
                {
                    updatedEvents.Remove(updateEventGuid);
                }
            }
        }
        private string SyncCaldavCalendar(string calendarId,
                                            string name,
                                            string description,
                                            string backgroundColor,
                                            Guid calDavGuid,
                                            Uri myUri,
                                            string curCaldavUserName,
                                            string email,
                                            bool isShared = false,
                                            SharingOptions sharingOptions = null)
        {
            var calendarUrl = CreateCalDavCalendar(name, description, backgroundColor, calDavGuid.ToString(), myUri, curCaldavUserName, email, isShared);

            BaseCalendar icalendar;
            int calId;

            var viewSettings = DataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, new List<string> { calendarId });

            if (int.TryParse(calendarId, out calId))
            {
                icalendar = DataProvider.GetCalendarById(calId);
                if (icalendar != null)
                {
                    icalendar = icalendar.GetUserCalendar(viewSettings.FirstOrDefault());
                }
            }
            else
            {
                //external
                icalendar = CalendarManager.GetCalendarForUser(SecurityContext.CurrentAccount.ID, calendarId, UserManager);
                if (icalendar != null)
                {
                    icalendar = icalendar.GetUserCalendar(viewSettings.FirstOrDefault());
                }
            }

            if (icalendar == null) return "";

            var calendarIcs = GetCalendariCalString(icalendar.Id, true);

            var tenant = TenantManager.GetCurrentTenant();
            var caldavTask = isShared
                ? new Task(() => CreateCaldavSharedEvents(calDavGuid.ToString(), calendarIcs, myUri, email, icalendar, SecurityContext.CurrentAccount, tenant.Id))
                : new Task(() => CreateCaldavEvents(calDavGuid.ToString(), myUri, email, icalendar, calendarIcs, tenant.Id));
            caldavTask.Start();

            return calendarUrl;
        }
        private string HexFromRGB(int r, int g, int b)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }
        private string GetUserCaldavCalendar(string calUrl, string encoded)
        {
            var authorization = DataProvider.GetUserAuthorization(encoded);

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(calUrl);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml")
            {
                CharSet = Encoding.UTF8.WebName
            };
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));

            try
            {
                var httpClient = ClientFactory.CreateClient();
                using var response = httpClient.Send(request);
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string ics = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(ics))
                    {
                        return ics;
                    }
                    return "";

                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    return "NotFound";
                }
                Log.LogInformation("ERROR. Get calendar CalDav url: " + exception.Message);
                return "";
            }
        }
        private string CreateCalDavCalendar(string name, string description, string backgroundColor, string calDavGuid, Uri myUri, string currentUserName, string email, bool isSharedCalendar = false)
        {
            name = (name ?? "").Trim();
            if (String.IsNullOrEmpty(name))
                throw new Exception(Resources.CalendarApiResource.ErrorEmptyName);

            description = (description ?? "").Trim();
            backgroundColor = (backgroundColor ?? "").Trim();

            var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";

            Log.LogInformation("RADICALE REWRITE URL: " + myUri);

            string[] numbers = Regex.Split(backgroundColor, @"\D+");
            var color = numbers.Length > 4 ? HexFromRGB(int.Parse(numbers[1]), int.Parse(numbers[2]), int.Parse(numbers[3])) : "#000000";

            var data = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                      "<mkcol xmlns=\"DAV:\" xmlns:C=\"urn:ietf:params:xml:ns:caldav\" xmlns:CR=\"urn:ietf:params:xml:ns:carddav\" xmlns:I=\"http://apple.com/ns/ical/\" xmlns:INF=\"http://inf-it.com/ns/ab/\">" +
                      "<set><prop>" +
                      "<resourcetype><collection /><C:calendar /></resourcetype>" +
                      "<C:supported-calendar-component-set><C:comp name=\"VEVENT\" /><C:comp name=\"VJOURNAL\" /><C:comp name=\"VTODO\" />" +
                      "</C:supported-calendar-component-set><displayname>" + name + "</displayname>" +
                      "<I:calendar-color>" + color + "</I:calendar-color>" +
                      "<C:calendar-description>" + description + "</C:calendar-description></prop></set></mkcol>";

            var requestUrl = calDavServerUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + calDavGuid + (isSharedCalendar ? "-shared" : "");

            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(requestUrl);
                request.Method = new HttpMethod("MKCOL");

                var authorization = isSharedCalendar ? DataProvider.GetSystemAuthorization() : DataProvider.GetUserAuthorization(email);
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));

                request.Content = new StringContent(data, Encoding.UTF8, "text/xml");

                var httpClient = ClientFactory.CreateClient();
                using var response = httpClient.Send(request);

                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    reader.ReadToEnd();
                    return calDavServerUrl + "/" + currentUserName + "/" + calDavGuid + (isSharedCalendar ? "-shared" : "");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "CreateCalDav");
                return "";
            }
        }


        /// <summary>
        /// Returns the feed for the iCal associated with the calendar by its ID and signagure specified in the request
        /// </summary>
        /// <short>Get iCal feed</short>
        /// <param name="calendarId">Calendar ID</param>
        /// <param name="signature">Signature</param>
        /// <remarks>To get the feed you need to use the method returning the iCal feed link (it will generate the necessary signature)</remarks>
        /// <returns>Calendar iCal feed</returns>
        [AllowAnonymous]
        [HttpGet("{calendarId}/ical/{signature}")] //NOTE: this method doesn't requires auth!!!
        public IActionResult GetCalendariCalStream(string calendarId, string signature)
        {
            try
            {
                //do not use compression
                var acceptEncoding = HttpContext.Request.Headers["Accept-Encoding"];
                if (acceptEncoding.Count > 0)
                {
                    var encodings = acceptEncoding.FirstOrDefault().Split(',');
                    if (encodings.Contains("gzip"))
                    {
                        encodings = (from x in encodings where x != "gzip" select x).ToArray();

                        Type t = HttpContext.Request.Headers.GetType();

                        System.Reflection.PropertyInfo propertyInfo = t.GetProperty("IsReadOnly", System.Reflection.BindingFlags.IgnoreCase |
                                                                                               System.Reflection.BindingFlags.Instance |
                                                                                               System.Reflection.BindingFlags.NonPublic |
                                                                                               System.Reflection.BindingFlags.FlattenHierarchy);
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(HttpContext.Request.Headers, false, null);
                        }

                        HttpContext.Request.Headers.Remove("Accept-Encoding");
                        HttpContext.Request.Headers.Add("Accept-Encoding", string.Join(",", encodings));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "GetCalendar");
            }


            FileStreamResult resp = null;
            var userId = Signature.Read<Guid>(signature, calendarId);
            if (UserManager.GetUsers(userId).Id != Constants.LostUser.Id)
            {
                var currentUserId = Guid.Empty;
                if (AuthContext.IsAuthenticated)
                {
                    currentUserId = AuthContext.CurrentAccount.ID;
                    AuthContext.Logout();
                }
                try
                {
                    SecurityContext.AuthenticateMeWithoutCookie(userId);
                    var icalFormat = GetCalendariCalString(calendarId);
                    if (icalFormat != null)
                    {
                        resp = new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes(icalFormat)), "text/calendar");
                        resp.FileDownloadName = calendarId + ".ics";
                    }
                }
                finally
                {
                    AuthContext.Logout();
                    if (currentUserId != Guid.Empty)
                    {
                        SecurityContext.AuthenticateMeWithoutCookie(currentUserId);
                    }
                }
            }
            return resp;
        }


        /// <summary>
        /// Creates the new calendar with the parameters (name, description, color, etc.) specified in the request
        /// </summary>
        /// <short>
        /// Create calendar
        /// </short>
        /// <param name="name">Calendar name</param>
        /// <param name="description">Calendar description</param>
        /// <param name="textColor">Event text color</param>
        /// <param name="backgroundColor">Event background color</param>
        /// <param name="timeZone">Calendar time zone</param>
        /// <param name="alertType">Event alert type, in case alert type is set by default</param>
        /// <param name="sharingOptions">Calendar sharing options with other users</param>
        /// <param name="iCalUrl">iCal url</param>
        /// <param name="isTodo">Calendar for todo list</param>
        /// <returns>Created calendar</returns>
        [HttpPost]
        public CalendarWrapper CreateCalendar(CalendarModel calendar)
        {
            var sharingOptionsList = calendar.SharingOptions ?? new List<SharingParam>();
            var timeZoneInfo = TimeZoneConverter.GetTimeZone(calendar.TimeZone);

            calendar.Name = (calendar.Name ?? "").Trim();
            if (String.IsNullOrEmpty(calendar.Name))
                throw new Exception(Resources.CalendarApiResource.ErrorEmptyName);

            calendar.Description = (calendar.Description ?? "").Trim();
            calendar.TextColor = (calendar.TextColor ?? "").Trim();
            calendar.BackgroundColor = (calendar.BackgroundColor ?? "").Trim();

            Guid calDavGuid = Guid.NewGuid();

            var myUri = HttpContext.Request.GetUrlRewriter();

            var currentUser = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            var currentUserEmail = DataProvider.CheckUserEmail(currentUser) ? UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email : null;
            var currentUserName = currentUserEmail != null ? currentUserEmail.ToLower() + "@" + myUri.Host : null;
            var tenant = TenantManager.GetCurrentTenant();

            var caldavTask = new Task(() =>
            {
                TenantManager.SetCurrentTenant(tenant);
                CreateCalDavCalendar(calendar.Name, calendar.Description, calendar.BackgroundColor, calDavGuid.ToString(), myUri, currentUserName, currentUserEmail);

            });
            caldavTask.Start();

            var cal = DataProvider.CreateCalendar(
                        AuthContext.CurrentAccount.ID,
                        calendar.Name,
                        calendar.Description,
                        calendar.TextColor,
                        calendar.BackgroundColor,
                        timeZoneInfo,
                        calendar.AlertType,
                        null,
                        sharingOptionsList.Select(o => o as SharingOptions.PublicItem).ToList(),
                        new List<UserViewSettings>(),
                        calDavGuid,
                        calendar.IsTodo);

            if (cal == null) throw new Exception("calendar is null");

            foreach (var opt in sharingOptionsList)
                if (String.Equals(opt.actionId, AccessOption.FullAccessOption.Id, StringComparison.InvariantCultureIgnoreCase))
                    AuthorizationManager.AddAce(new AzRecord(opt.Id, CalendarAccessRights.FullAccessAction.ID, Common.Security.Authorizing.AceType.Allow, cal));

            //notify
            CalendarNotifyClient.NotifyAboutSharingCalendar(cal);

            //iCalUrl
            if (!string.IsNullOrEmpty(calendar.ICalUrl))
            {
                try
                {
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(calendar.ICalUrl);
                    var httpClient = ClientFactory.CreateClient();
                    using var response = httpClient.Send(request);

                    using (var stream = response.Content.ReadAsStream())
                    {
                        var ms = new MemoryStream();
                        stream.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        using (var tempReader = new StreamReader(ms))
                        {

                            var cals = DDayICalParser.DeserializeCalendar(tempReader);
                            ImportEvents(Convert.ToInt32(cal.Id), cals);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.LogInformation($"Error import events to new calendar by ical url: {ex.Message}");
                }

            }

            return CalendarWrapperHelper.Get(cal);
        }

        /// <summary>
        /// Updates the selected calendar with the parameters (name, description, color, etc.) specified in the request for the current user and access rights for other users
        /// </summary>
        /// <short>
        /// Update calendar
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <param name="name">Calendar new name</param>
        /// <param name="description">Calendar new description</param>
        /// <param name="textColor">Event text color</param>
        /// <param name="backgroundColor">Event background color</param>
        /// <param name="timeZone">Calendar time zone</param>
        /// <param name="alertType">Event alert type, in case alert type is set by default</param>
        /// <param name="hideEvents">Display type: show or hide events in calendar</param>
        /// <param name="sharingOptions">Calendar sharing options with other users</param>
        /// <param name="iCalUrl">iCal url</param>
        /// <returns>Updated calendar</returns>
        [HttpPut("{calendarId}")]
        public CalendarWrapper UpdateCalendar(string calendarId, CalendarModel calendar)
        {
            var name = calendar.Name;
            var description = calendar.Description;
            var textColor = calendar.TextColor;
            var backgroundColor = calendar.BackgroundColor;
            var timeZone = calendar.TimeZone;
            var alertType = calendar.AlertType;
            var hideEvents = calendar.HideEvents;
            var sharingOptions = calendar.SharingOptions;
            var iCalUrl = calendar.ICalUrl ?? "";

            TimeZoneInfo timeZoneInfo = TimeZoneConverter.GetTimeZone(timeZone);
            int calId;
            if (!string.IsNullOrEmpty(iCalUrl))
            {
                try
                {
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(iCalUrl);

                    var httpClient = ClientFactory.CreateClient();
                    using var response = httpClient.Send(request);
                    using (var stream = response.Content.ReadAsStream())
                    {
                        var ms = new MemoryStream();
                        stream.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        using (var tempReader = new StreamReader(ms))
                        {

                            var cals = DDayICalParser.DeserializeCalendar(tempReader);
                            ImportEvents(Convert.ToInt32(calendarId), cals);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.LogInformation($"Error import events to calendar by ical url: {ex.Message}");
                }

            }


            if (int.TryParse(calendarId, out calId))
            {
                var oldCal = DataProvider.GetCalendarById(calId);

                if (CheckPermissions(oldCal, CalendarAccessRights.FullAccessAction, true))
                {
                    //update calendar and share options
                    var sharingOptionsList = sharingOptions ?? new List<SharingParam>();

                    name = (name ?? "").Trim();
                    if (String.IsNullOrEmpty(name))
                        throw new Exception(Resources.CalendarApiResource.ErrorEmptyName);

                    description = (description ?? "").Trim();
                    textColor = (textColor ?? "").Trim();
                    backgroundColor = (backgroundColor ?? "").Trim();


                    //view
                    var userOptions = oldCal.ViewSettings;
                    var usrOpt = userOptions.Find(o => o.UserId.Equals(AuthContext.CurrentAccount.ID));
                    if (usrOpt == null)
                    {
                        userOptions.Add(new UserViewSettings
                        {
                            Name = name,
                            TextColor = textColor,
                            BackgroundColor = backgroundColor,
                            EventAlertType = alertType,
                            IsAccepted = true,
                            UserId = AuthContext.CurrentAccount.ID,
                            TimeZone = timeZoneInfo
                        });
                    }
                    else
                    {
                        usrOpt.Name = name;
                        usrOpt.TextColor = textColor;
                        usrOpt.BackgroundColor = backgroundColor;
                        usrOpt.EventAlertType = alertType;
                        usrOpt.TimeZone = timeZoneInfo;
                    }

                    userOptions.RemoveAll(o => !o.UserId.Equals(oldCal.OwnerId) && !sharingOptionsList.Exists(opt => (!opt.IsGroup && o.UserId.Equals(opt.Id))
                                                                               || opt.IsGroup && UserManager.IsUserInGroup(o.UserId, opt.Id)));

                    //check owner
                    if (!oldCal.OwnerId.Equals(AuthContext.CurrentAccount.ID))
                    {
                        name = oldCal.Name;
                        description = oldCal.Description;
                    }

                    var myUri = HttpContext.Request.GetUrlRewriter();
                    var _email = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email;

                    var cal = DataProvider.UpdateCalendar(calId, name, description,
                                        sharingOptionsList.Select(o => o as SharingOptions.PublicItem).ToList(),
                                        userOptions);

                    var oldSharingList = new List<SharingParam>();
                    var owner = UserManager.GetUsers(cal.OwnerId);

                    if (AuthContext.CurrentAccount.ID != cal.OwnerId)
                    {
                        if (DataProvider.CheckUserEmail(owner))
                        {
                            UpdateCalDavCalendar(name, description, backgroundColor, oldCal.calDavGuid, myUri, owner.Email);
                        }
                    }
                    else
                    {
                        UpdateCalDavCalendar(name, description, backgroundColor, oldCal.calDavGuid, myUri, _email);
                    }
                    var pic = PublicItemCollectionHelper.GetForCalendar(oldCal);
                    if (pic.Items.Count > 1)
                    {
                        oldSharingList.AddRange(from publicItem in pic.Items
                                                where publicItem.ItemId != owner.Id.ToString()
                                                select new SharingParam
                                                {
                                                    Id = Guid.Parse(publicItem.ItemId),
                                                    isGroup = publicItem.IsGroup,
                                                    actionId = publicItem.SharingOption.Id
                                                });
                    }
                    if (sharingOptionsList.Count > 0)
                    {
                        var tenant = TenantManager.GetCurrentTenant();
                        var events = cal.LoadEvents(AuthContext.CurrentAccount.ID, DateTime.MinValue, DateTime.MaxValue);
                        var calendarObjViewSettings = cal != null && cal.ViewSettings != null ? cal.ViewSettings.FirstOrDefault() : null;
                        var targetCalendar = DDayICalParser.ConvertCalendar(cal != null ? cal.GetUserCalendar(calendarObjViewSettings) : null);

                        var caldavTask = new Task(() => UpdateSharedCalDavCalendar(name, description, backgroundColor, oldCal.calDavGuid, myUri, sharingOptionsList, events, calendarId, cal.calDavGuid, tenant.Id, DateTime.Now, targetCalendar.TimeZones[0], cal.TimeZone));
                        caldavTask.Start();
                    }

                    oldSharingList.RemoveAll(c => sharingOptionsList.Contains(sharingOptionsList.Find((x) => x.Id == c.Id)));

                    if (oldSharingList.Count > 0)
                    {
                        var currentTenantId = TenantManager.GetCurrentTenant().Id;
                        var caldavHost = myUri.Host;

                        var replaceSharingEventThread = new Thread(() =>
                        {
                            TenantManager.SetCurrentTenant(currentTenantId);

                            foreach (var sharingOption in oldSharingList)
                            {
                                if (!sharingOption.IsGroup)
                                {
                                    var user = UserManager.GetUsers(sharingOption.itemId);

                                    if (DataProvider.CheckUserEmail(user))
                                    {
                                        var currentUserName = user.Email.ToLower() + "@" + caldavHost;
                                        var userEmail = user.Email;
                                        DataProvider.RemoveCaldavCalendar(currentUserName, userEmail, cal.calDavGuid, myUri, user.Id != cal.OwnerId);
                                    }
                                }
                                else
                                {
                                    var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                                    foreach (var user in users)
                                    {
                                        var userEmail = user.Email;
                                        var currentUserName = userEmail.ToLower() + "@" + caldavHost;
                                        DataProvider.RemoveCaldavCalendar(currentUserName, userEmail, cal.calDavGuid, myUri, user.Id != cal.OwnerId);
                                    }
                                }
                            }
                        });
                        replaceSharingEventThread.Start();
                    }

                    if (cal != null)
                    {
                        //clear old rights
                        AuthorizationManager.RemoveAllAces(cal);

                        foreach (var opt in sharingOptionsList)
                            if (String.Equals(opt.actionId, AccessOption.FullAccessOption.Id, StringComparison.InvariantCultureIgnoreCase))
                                AuthorizationManager.AddAce(new AzRecord(opt.Id, CalendarAccessRights.FullAccessAction.ID, Common.Security.Authorizing.AceType.Allow, cal));

                        //notify
                        CalendarNotifyClient.NotifyAboutSharingCalendar(cal, oldCal);
                        return CalendarWrapperHelper.Get(cal);
                    }
                    return null;
                }
            }

            //update view
            return UpdateCalendarView(calendarId, new CalendarModel
            {
                Name = name,
                TextColor = textColor,
                BackgroundColor = backgroundColor,
                TimeZone = timeZone,
                AlertType = alertType,
                HideEvents = hideEvents
            });

        }

        /// <summary>
        /// Change the calendar display parameters specified in the request for the current user
        /// </summary>
        /// <short>
        /// Update calendar user view
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <param name="name">Calendar name</param>
        /// <param name="textColor">Event text color</param>
        /// <param name="backgroundColor">Event background color</param>
        /// <param name="timeZone">Calendar time zone</param>
        /// <param name="alertType">Event alert type, in case alert type is set by default</param>
        /// <param name="hideEvents">Display type: show or hide events in calendar</param>
        /// <returns>Updated calendar</returns>
        [HttpPut("{calendarId}/view")]
        public CalendarWrapper UpdateCalendarView(string calendarId, CalendarModel calendar)
        {

            var name = calendar.Name;
            var textColor = calendar.TextColor;
            var backgroundColor = calendar.BackgroundColor;
            var timeZone = calendar.TimeZone;
            var alertType = calendar.AlertType;
            var hideEvents = calendar.HideEvents;

            TimeZoneInfo timeZoneInfo = TimeZoneConverter.GetTimeZone(timeZone);
            name = (name ?? "").Trim();
            if (String.IsNullOrEmpty(name))
                throw new Exception(Resources.CalendarApiResource.ErrorEmptyName);

            var settings = new UserViewSettings
            {
                BackgroundColor = backgroundColor,
                CalendarId = calendarId,
                IsHideEvents = hideEvents,
                TextColor = textColor,
                EventAlertType = alertType,
                IsAccepted = true,
                UserId = AuthContext.CurrentAccount.ID,
                Name = name,
                TimeZone = timeZoneInfo
            };

            DataProvider.UpdateCalendarUserView(settings);
            return GetCalendarById(calendarId);
        }

        /// <summary>
        /// Deletes the calendar with the ID specified in the request
        /// </summary>
        /// <short>
        /// Delete calendar
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        [HttpDelete("{calendarId}")]
        public void RemoveCalendar(int calendarId)
        {
            var cal = DataProvider.GetCalendarById(calendarId);
            var events = cal.LoadEvents(AuthContext.CurrentAccount.ID, DateTime.MinValue, DateTime.MaxValue);

            var pic = PublicItemCollectionHelper.GetForCalendar(cal);
            //check permissions
            CheckPermissions(cal, CalendarAccessRights.FullAccessAction);
            //clear old rights

            AuthorizationManager.RemoveAllAces(cal);
            var caldavGuid = DataProvider.RemoveCalendar(calendarId);

            var myUri = HttpContext.Request.GetUrlRewriter();
            var currentTenantId = TenantManager.GetCurrentTenant().Id;
            var caldavHost = myUri.Host;
            if (caldavGuid != Guid.Empty)
            {
                Log.LogInformation("RADICALE REWRITE URL: " + myUri);

                var currentUser = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                var currentUserEmail = DataProvider.CheckUserEmail(currentUser) ? UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email : null;
                var currentUserName = currentUserEmail != null ? currentUserEmail.ToLower() + "@" + caldavHost : null;

                if (currentUserEmail != null)
                {
                    var caldavTask = new Task(() =>
                    {
                        TenantManager.SetCurrentTenant(currentTenantId);
                        DataProvider.RemoveCaldavCalendar(currentUserName, currentUserEmail, caldavGuid.ToString(), myUri);
                    });
                    caldavTask.Start();
                }
            }

            var sharingList = new List<SharingParam>();
            if (pic.Items.Count > 1)
            {
                sharingList.AddRange(from publicItem in pic.Items
                                     where publicItem.ItemId != AuthContext.CurrentAccount.ID.ToString()
                                     select new SharingParam
                                     {
                                         Id = Guid.Parse(publicItem.ItemId),
                                         isGroup = publicItem.IsGroup,
                                         actionId = publicItem.SharingOption.Id
                                     });
            }

            var replaceSharingEventThread = new Thread(() =>
            {
                TenantManager.SetCurrentTenant(currentTenantId);

                foreach (var sharingOption in sharingList)
                {
                    if (!sharingOption.IsGroup)
                    {
                        var user = UserManager.GetUsers(sharingOption.itemId);
                        if (DataProvider.CheckUserEmail(user))
                        {
                            DataProvider.RemoveCaldavCalendar(user.Email + "@" + caldavHost, user.Email, cal.calDavGuid, myUri, user.Id != cal.OwnerId);
                        }
                    }
                    else
                    {
                        var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                        foreach (var user in users)
                        {
                            if (DataProvider.CheckUserEmail(user))
                            {
                                DataProvider.RemoveCaldavCalendar(user.Email + "@" + caldavHost, user.Email, cal.calDavGuid, myUri, user.Id != cal.OwnerId);
                            }
                        }
                    }
                }
                foreach (var evt in events)
                {
                    if (evt.SharingOptions.PublicItems.Count > 0)
                    {
                        var permissions = PublicItemCollectionHelper.GetForEvent(evt);
                        var so = permissions.Items
                            .Where(x => x.SharingOption.Id != AccessOption.OwnerOption.Id)
                            .Select(x => new SharingParam
                            {
                                Id = x.Id,
                                actionId = x.SharingOption.Id,
                                isGroup = x.IsGroup
                            }).ToList();
                        var uid = evt.Uid;
                        string[] split = uid.Split(new Char[] { '@' });
                        foreach (var sharingOption in so)
                        {
                            var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                            if (!sharingOption.IsGroup)
                            {
                                var user = UserManager.GetUsers(sharingOption.itemId);
                                if (DataProvider.CheckUserEmail(user))
                                {
                                    deleteEvent(fullAccess ? split[0] + "_write" : split[0], SharedEventsCalendar.CalendarId, user.Email, myUri, user.Id != evt.OwnerId);
                                }
                            }
                            else
                            {
                                var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                                foreach (var user in users)
                                {
                                    if (DataProvider.CheckUserEmail(user))
                                    {
                                        var eventUid = user.Id == evt.OwnerId
                                                       ? split[0]
                                                       : fullAccess ? split[0] + "_write" : split[0];

                                        deleteEvent(eventUid, SharedEventsCalendar.CalendarId, user.Email, myUri, true);
                                    }
                                }
                            }
                        }
                    }
                }

            });
            replaceSharingEventThread.Start();

        }

        /// <summary>
        /// Unsubscribes the current user from the event with the ID specified in the request
        /// </summary>
        /// <short>
        /// Unsubscribe from event
        /// </short>
        /// <param name="eventId">Event ID</param>
        [HttpDelete("events/{eventId}/unsubscribe")]
        public void UnsubscribeEvent(int eventId)
        {
            var evt = DataProvider.GetEventById(eventId);

            if (evt != null)
            {
                string[] split = evt.Uid.Split(new Char[] { '@' });
                var myUri = HttpContext.Request.GetUrlRewriter();
                var email = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email;
                var fullAccess = CheckPermissions(evt, CalendarAccessRights.FullAccessAction, true);

                deleteEvent(fullAccess ? split[0] + "_write" : split[0], SharedEventsCalendar.CalendarId, email, myUri, SecurityContext.CurrentAccount.ID != evt.OwnerId);

                DataProvider.UnsubscribeFromEvent(eventId, SecurityContext.CurrentAccount.ID);
            }
        }

        private void deleteEvent(string uid, string calendarId, string email, Uri myUri, bool isShared = false)
        {

            try
            {
                var сaldavGuid = "";
                if (calendarId != BirthdayReminderCalendar.CalendarId &&
                    calendarId != SharedEventsCalendar.CalendarId &&
                    calendarId != "crm_calendar" &&
                    !calendarId.Contains("Project_"))
                {
                    сaldavGuid = DataProvider.GetCalDavGuid(calendarId);
                }
                else
                {
                    сaldavGuid = calendarId;
                }

                if (сaldavGuid.Length != 0)
                {
                    var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";

                    Log.LogInformation("RADICALE REWRITE URL: " + myUri);

                    var currentUserName = email + "@" + myUri.Host;

                    var requestUrl = calDavServerUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + (isShared ? сaldavGuid + "-shared" : сaldavGuid) + "/" + uid + ".ics";

                    try
                    {
                        var request = new HttpRequestMessage();
                        request.RequestUri = new Uri(requestUrl);
                        request.Method = HttpMethod.Delete;

                        var authorization = isShared ? DataProvider.GetSystemAuthorization() : DataProvider.GetUserAuthorization(email);
                        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));

                        var httpClient = ClientFactory.CreateClient();
                        httpClient.Send(request);
                    }
                    catch (HttpRequestException ex)
                    {
                        if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Conflict)
                            Log.LogDebug("ERROR: " + ex.Message);
                        else
                            Log.LogError(ex, "ERROR");
                    }
                    catch (Exception ex)
                    {
                        Log.LogError(ex, "ERROR: ");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "ERROR: ");
            }

        }

        /// <summary>
        /// Returns the event in ics format from history
        /// </summary>
        /// <short>
        /// Returns the event in ics format from history
        /// </short>
        /// <param name="eventUid">Event UID</param>
        /// <returns>Event History</returns>
        [HttpGet("events/{eventUid}/historybyuid")]
        public EventHistoryWrapper GetEventHistoryByUid(string eventUid)
        {
            if (string.IsNullOrEmpty(eventUid))
            {
                throw new ArgumentException("eventUid");
            }

            var evt = DataProvider.GetEventByUid(eventUid);

            return GetEventHistoryWrapper(evt);
        }

        /// <summary>
        /// Returns the event in ics format from history
        /// </summary>
        /// <short>
        /// Returns the event in ics format from history
        /// </short>
        /// <param name="eventId">Event ID</param>
        /// <returns>Event History</returns>
        [HttpGet("events/{eventId}/historybyid")]
        public EventHistoryWrapper GetEventHistoryById(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentException("eventId");
            }

            var evt = DataProvider.GetEventById(eventId);

            return GetEventHistoryWrapper(evt);
        }

        /// <summary>
        /// Creates the new event in the default calendar with the parameters specified in the request
        /// </summary>
        /// <short>
        /// Create new event
        /// </short>
        /// <param name="name">Event name</param>
        /// <param name="description">Event description</param>
        /// <param name="startDate">Event start date</param>
        /// <param name="endDate">Event end date</param>
        /// <param name="repeatType">Event recurrence type (RRULE string in iCal format)</param>
        /// <param name="alertType">Event notification type</param>
        /// <param name="isAllDayLong">Event duration type: all day long or not</param>
        /// <param name="sharingOptions">Event sharing access parameters</param>
        /// <returns>Event list</returns>
        [HttpPost("event")]
        public List<EventWrapper> CreateEvent(EventModel eventModel)
        {
            var calendar = LoadInternalCalendars().First(x => (!x.IsSubscription && x.IsTodo != 1));
            int calendarId;

            if (int.TryParse(calendar.Id, out calendarId))
            {
                return AddEvent(calendarId, eventModel);
            }

            throw new Exception($"Can't parse {calendar.Id} to int");
        }

        /// <summary>
        /// Creates the new event in the selected calendar with the parameters specified in the request
        /// </summary>
        /// <short>
        /// Create new event
        /// </short>
        /// <param name="calendarId">ID of the calendar where the event is created</param>
        /// <param name="name">Event name</param>
        /// <param name="description">Event description</param>
        /// <param name="startDate">Event start date</param>
        /// <param name="endDate">Event end date</param>
        /// <param name="repeatType">Event recurrence type (RRULE string in iCal format)</param>
        /// <param name="alertType">Event notification type</param>
        /// <param name="isAllDayLong">Event duration type: all day long or not</param>
        /// <param name="sharingOptions">Event sharing access parameters</param>
        /// <returns>Event list</returns>
        [HttpPost("{calendarId}/event")]
        public List<EventWrapper> AddEvent(int calendarId, EventModel eventModel)
        {
            eventModel.CalendarId = calendarId.ToString();
            return AddEvent(eventModel);
        }

        /// <summary>
        /// Updates the existing event in the selected calendar with the parameters specified in the request
        /// </summary>
        /// <short>
        /// Update event
        /// </short>
        /// <param name="calendarId">ID of the calendar where the event belongs</param>
        /// <param name="eventId">Event ID</param>
        /// <param name="name">Event new name</param>
        /// <param name="description">Event new description</param>
        /// <param name="startDate">Event start date</param>
        /// <param name="endDate">Event end date</param>
        /// <param name="repeatType">Event recurrence type (RRULE string in iCal format)</param>
        /// <param name="alertType">Event notification type</param>
        /// <param name="isAllDayLong">Event duration type: all day long or not</param>
        /// <param name="sharingOptions">Event sharing access parameters</param>
        /// <param name="status">Event status</param>
        /// <returns>Updated event list</returns>
        [HttpPut("{calendarId}/{eventId}")]
        public List<EventWrapper> Update(string calendarId, int eventId, EventModel eventModel)
        {
            eventModel.CalendarId = calendarId;
            eventModel.EventId = eventId;

            return UpdateEvent(eventModel);
        }

        /// <summary>
        /// Creates the new event in the selected calendar with the parameters specified in the request
        /// </summary>
        /// <short>
        /// Create new event
        /// </short>
        /// <param name="calendarId">ID of the calendar where the event is created</param>
        /// <param name="ics">Event in iCal format</param>
        /// <param name="alertType">Event notification type</param>
        /// <param name="sharingOptions">Event sharing access parameters</param>
        /// <param name="eventUid">Event uid</param>
        /// <returns>Event</returns>
        [HttpPost("icsevent")]
        public List<EventWrapper> AddEvent(EventModel eventData)
        {
            var calendarId = eventData.CalendarId;
            var eventUid = eventData.EventUid;
            var ics = eventData.Ics;

            var old_ics = eventData.Ics;
            int calId;

            if (!int.TryParse(calendarId, out calId))
            {
                calId = int.Parse(eventData.CalendarId);
            }

            if (calId <= 0)
            {
                var defaultCalendar = LoadInternalCalendars().First(x => (!x.IsSubscription && x.IsTodo != 1));
                if (!int.TryParse(defaultCalendar.Id, out calId))
                    throw new Exception($"Can't parse {defaultCalendar.Id} to int");
            }

            var calendars = DDayICalParser.DeserializeCalendar(ics);

            if (calendars == null) return null;

            var calendar = calendars.FirstOrDefault();

            if (calendar == null || calendar.Events == null) return null;

            var eventObj = calendar.Events.FirstOrDefault();

            if (eventObj == null) return null;

            var calendarObj = DataProvider.GetCalendarById(calId);
            var calendarObjViewSettings = calendarObj != null && calendarObj.ViewSettings != null ? calendarObj.ViewSettings.FirstOrDefault() : null;
            var targetCalendar = DDayICalParser.ConvertCalendar(calendarObj?.GetUserCalendar(calendarObjViewSettings));

            if (targetCalendar == null) return null;

            var rrule = GetRRuleString(eventObj);

            var utcStartDate = eventObj.IsAllDay ? eventObj.Start.Value : DDayICalParser.ToUtc(eventObj.Start);
            var utcEndDate = eventObj.IsAllDay ? eventObj.End.Value : DDayICalParser.ToUtc(eventObj.End);

            if (eventObj.IsAllDay && utcStartDate.Date < utcEndDate.Date)
                utcEndDate = utcEndDate.AddDays(-1);

            eventUid = eventUid == null ? null : $"{eventUid}@onlyoffice.com";

            var result = CreateEvent(calId,
                                     eventObj.Summary,
                                     eventObj.Description,
                                     utcStartDate,
                                     utcEndDate,
                                     RecurrenceRule.Parse(rrule),
                                     eventData.AlertType,
                                     eventObj.IsAllDay,
                                     eventData.SharingOptions,
                                     DataProvider.GetEventUid(eventUid),
                                     EventStatus.Confirmed,
                                     eventObj.Created != null ? eventObj.Created.Value : DateTime.Now);

            if (result == null || !result.Any()) return null;

            var evt = result.First();

            eventObj.Uid = evt.Uid;
            eventObj.Sequence = 0;
            eventObj.Status = Ical.Net.EventStatus.Confirmed;

            targetCalendar.Method = Ical.Net.CalendarMethods.Request;
            targetCalendar.Events.Clear();
            targetCalendar.Events.Add(eventObj);

            ics = DDayICalParser.SerializeCalendar(targetCalendar);

            try
            {
                var uid = evt.Uid;
                string[] split = uid.Split(new Char[] { '@' });

                var calDavGuid = calendarObj != null ? calendarObj.calDavGuid : "";
                var myUri = HttpContext.Request.GetUrlRewriter();
                var userId = SecurityContext.CurrentAccount.ID;
                var currentUserEmail = UserManager.GetUsers(userId).Email.ToLower();

                var currentEventUid = split[0];

                var pic = PublicItemCollectionHelper.GetForCalendar(calendarObj);
                var sharingList = new List<SharingParam>();
                if (pic.Items.Count > 1)
                {
                    sharingList.AddRange(from publicItem in pic.Items
                                         where publicItem.ItemId != SecurityContext.CurrentAccount.ID.ToString()
                                         select new SharingParam
                                         {
                                             Id = Guid.Parse(publicItem.ItemId),
                                             isGroup = publicItem.IsGroup,
                                             actionId = publicItem.SharingOption.Id
                                         });
                }
                var currentTenantId = TenantManager.GetCurrentTenant().Id;

                if (!calendarObj.OwnerId.Equals(SecurityContext.CurrentAccount.ID) && CheckPermissions(calendarObj, CalendarAccessRights.FullAccessAction, true))
                {
                    currentEventUid = currentEventUid + "_write";
                }
                var updateCaldavThread = new Thread(() =>
                {

                    updateCaldavEvent(old_ics, currentEventUid, true, calDavGuid, myUri,
                                      currentUserEmail, DateTime.Now,
                                      targetCalendar.TimeZones[0], calendarObj.TimeZone, false, userId != calendarObj.OwnerId);

                    TenantManager.SetCurrentTenant(currentTenantId);
                    //calendar sharing list
                    foreach (var sharingOption in sharingList)
                    {
                        var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                        if (!sharingOption.IsGroup)
                        {
                            var user = UserManager.GetUsers(sharingOption.itemId);
                            if (DataProvider.CheckUserEmail(user))
                            {
                                var sharedEventUid = user.Id == calendarObj.OwnerId
                                                     ? split[0]
                                                     : fullAccess ? split[0] + "_write" : split[0];

                                updateCaldavEvent(old_ics, sharedEventUid, true, calDavGuid, myUri,
                                                  user.Email, DateTime.Now, targetCalendar.TimeZones[0],
                                                  calendarObj.TimeZone, false, user.Id != calendarObj.OwnerId);
                            }

                        }
                        else
                        {
                            var users = UserManager.GetUsersByGroup(sharingOption.itemId);

                            foreach (var user in users)
                            {
                                if (DataProvider.CheckUserEmail(user))
                                {
                                    var sharedEventUid = user.Id == calendarObj.OwnerId
                                                     ? split[0]
                                                     : fullAccess ? split[0] + "_write" : split[0];
                                    updateCaldavEvent(old_ics, sharedEventUid, true, calDavGuid, myUri,
                                                  user.Email, DateTime.Now, targetCalendar.TimeZones[0],
                                                  calendarObj.TimeZone, false, user.Id != calendarObj.OwnerId);
                                }
                            }
                        }
                    }
                    //event sharing list
                    if (eventData.SharingOptions != null)
                    {
                        foreach (var sharingOption in eventData.SharingOptions)
                        {
                            var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                            if (!sharingOption.IsGroup)
                            {
                                var user = UserManager.GetUsers(sharingOption.itemId);
                                if (DataProvider.CheckUserEmail(user))
                                {
                                    var sharedEventUid = user.Id == calendarObj.OwnerId
                                                         ? split[0]
                                                         : fullAccess ? split[0] + "_write" : split[0];

                                    updateCaldavEvent(old_ics, sharedEventUid, true, SharedEventsCalendar.CalendarId, myUri,
                                                      user.Email, DateTime.Now, targetCalendar.TimeZones[0],
                                                      calendarObj.TimeZone, false, user.Id != calendarObj.OwnerId);
                                }
                            }
                            else
                            {
                                var users = UserManager.GetUsersByGroup(sharingOption.itemId);

                                foreach (var user in users)
                                {
                                    if (DataProvider.CheckUserEmail(user))
                                    {
                                        var sharedEventUid = user.Id == calendarObj.OwnerId
                                                         ? split[0]
                                                         : fullAccess ? split[0] + "_write" : split[0];

                                        updateCaldavEvent(old_ics, sharedEventUid, true, SharedEventsCalendar.CalendarId, myUri,
                                                      user.Email, DateTime.Now, targetCalendar.TimeZones[0],
                                                      calendarObj.TimeZone, false, user.Id != calendarObj.OwnerId);
                                    }
                                }
                            }
                        }
                    }

                });
                updateCaldavThread.Start();
            }
            catch (Exception e)
            {
                Log.LogError(e, "addevent");
            }


            DataProvider.AddEventHistory(calId, evt.Uid, int.Parse(evt.Id), ics);

            return result;
        }

        /// <summary>
        /// Updates the existing event in the selected calendar with the parameters specified in the request
        /// </summary>
        /// <short>
        /// Update event
        /// </short>
        /// <param name="eventId">Event ID</param>
        /// <param name="calendarId">ID of the calendar where the event belongs</param>
        /// <param name="ics">Event in iCal format</param>
        /// <param name="alertType">Event notification type</param>
        /// <param name="sharingOptions">Event sharing access parameters</param>
        /// <param name="fromCalDavServer">bool flag says that request from caldav server</param>
        /// <param name="ownerId">Event owner id</param>
        /// <returns>Updated event</returns>
        [HttpPut("icsevent.json")]
        public List<EventWrapper> UpdateEvent(EventModel eventData)
        {

            var fromCalDavServer = eventData.FromCalDavServer;
            var ownerId = eventData.OwnerId ?? "";

            var ics = eventData.Ics;
            var old_ics = eventData.Ics;

            var calendarId = eventData.CalendarId;

            var evt = DataProvider.GetEventById(eventData.EventId);

            if (evt == null)
                throw new Exception(Resources.CalendarApiResource.ErrorItemNotFound);

            var cal = DataProvider.GetCalendarById(Int32.Parse(evt.CalendarId));
            if (!fromCalDavServer)
            {
                if (!evt.OwnerId.Equals(AuthContext.CurrentAccount.ID) &&
                    !CheckPermissions(evt, CalendarAccessRights.FullAccessAction, true) &&
                    !CheckPermissions(cal, CalendarAccessRights.FullAccessAction, true))
                    throw new System.Security.SecurityException(Resources.CalendarApiResource.ErrorAccessDenied);
            }
            int calId;

            if (!int.TryParse(calendarId, out calId))
            {
                calId = int.Parse(evt.CalendarId);
            }

            EventHistory evtHistory = null;

            if (string.IsNullOrEmpty(evt.Uid))
            {
                evt.Uid = DataProvider.GetEventUid(evt.Uid);
                DataProvider.SetEventUid(eventData.EventId, evt.Uid);
            }
            else
            {
                evtHistory = DataProvider.GetEventHistory(eventData.EventId);
            }

            var sequence = 0;
            if (evtHistory != null)
            {
                var maxSequence = evtHistory.History.Select(x => x.Events.First()).Max(x => x.Sequence);
                if (!fromCalDavServer)
                {
                    if (evt.OwnerId == AuthContext.CurrentAccount.ID && !CheckIsOrganizer(evtHistory))
                        sequence = maxSequence;
                    else
                        sequence = maxSequence + 1;
                }
            }

            var calendars = DDayICalParser.DeserializeCalendar(ics);

            if (calendars == null) return null;

            var calendar = calendars.FirstOrDefault();

            if (calendar == null || calendar.Events == null) return null;

            var eventObj = calendar.Events.FirstOrDefault();

            if (eventObj == null) return null;

            var calendarObj = DataProvider.GetCalendarById(calId);
            var calendarObjViewSettings = calendarObj != null && calendarObj.ViewSettings != null ? calendarObj.ViewSettings.FirstOrDefault() : null;
            var targetCalendar = DDayICalParser.ConvertCalendar(calendarObj != null ? calendarObj.GetUserCalendar(calendarObjViewSettings) : null);

            if (targetCalendar == null) return null;

            eventObj.Uid = evt.Uid;
            eventObj.Sequence = sequence;
            //eventObj.ExceptionDates.Clear();

            targetCalendar.Method = Ical.Net.CalendarMethods.Request;
            targetCalendar.Events.Clear();
            targetCalendar.Events.Add(eventObj);

            ics = (evtHistory != null ? (evtHistory.Ics + Environment.NewLine) : string.Empty) + DDayICalParser.SerializeCalendar(targetCalendar);

            DataProvider.RemoveEventHistory(eventData.EventId);

            evtHistory = DataProvider.AddEventHistory(calId, evt.Uid, eventData.EventId, ics);

            var mergedCalendar = EventHistoryHelper.GetMerged(evtHistory);

            if (mergedCalendar == null || mergedCalendar.Events == null || !mergedCalendar.Events.Any()) return null;

            var mergedEvent = mergedCalendar.Events.First();

            var rrule = GetRRuleString(mergedEvent);

            var utcStartDate = eventObj.IsAllDay ? eventObj.Start.Value : DDayICalParser.ToUtc(eventObj.Start);
            var utcEndDate = eventObj.IsAllDay ? eventObj.End.Value : DDayICalParser.ToUtc(eventObj.End);


            var createDate = mergedEvent.Created != null ? mergedEvent.Created.Value : DateTime.Now;
            if (eventObj.IsAllDay && utcStartDate.Date < utcEndDate.Date)
                utcEndDate = utcEndDate.AddDays(-1);

            if (!fromCalDavServer)
            {
                try
                {
                    var uid = evt.Uid;
                    string[] uidData = uid.Split(new Char[] { '@' });

                    var calDavGuid = calendarObj != null ? calendarObj.calDavGuid : "";
                    var myUri = HttpContext.Request.GetUrlRewriter();
                    var currentUserEmail = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email.ToLower();

                    var isFullAccess = PermissionContext.PermissionResolver.Check(AuthContext.CurrentAccount, evt, null,
                                                                              CalendarAccessRights.FullAccessAction);
                    var isShared = false;
                    if (calendarId == BirthdayReminderCalendar.CalendarId ||
                        calendarId == SharedEventsCalendar.CalendarId ||
                        calendarId == "crm_calendar")
                    {
                        calDavGuid = calendarId;
                        isShared = true;
                    }
                    else
                    {
                        isShared = calendarObj != null && calendarObj.OwnerId != AuthContext.CurrentAccount.ID;
                    }

                    var eventUid = isShared && isFullAccess ? uidData[0] + "_write" : uidData[0];

                    var currentTenantId = TenantManager.GetCurrentTenant().Id;
                    var pic = PublicItemCollectionHelper.GetForCalendar(cal);
                    var calendarCharingList = new List<SharingParam>();
                    if (pic.Items.Count > 1)
                    {
                        calendarCharingList.AddRange(from publicItem in pic.Items
                                                     where publicItem.ItemId != calendarObj.OwnerId.ToString()
                                                     select new SharingParam
                                                     {
                                                         Id = Guid.Parse(publicItem.ItemId),
                                                         isGroup = publicItem.IsGroup,
                                                         actionId = publicItem.SharingOption.Id
                                                     });
                    }

                    var sharingEventThread = new Thread(() =>
                    {
                        TenantManager.SetCurrentTenant(currentTenantId);
                        //event sharing options
                        if (eventData.SharingOptions != null)
                        {
                            foreach (var sharingOption in eventData.SharingOptions)
                            {
                                if (!sharingOption.IsGroup)
                                {
                                    var user = UserManager.GetUsers(sharingOption.itemId);
                                    ReplaceSharingEvent(user, sharingOption.actionId, uidData[0], myUri, old_ics,
                                                        calendarId, createDate, targetCalendar.TimeZones[0],
                                                        calendarObj.TimeZone);
                                }
                                else
                                {
                                    var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                                    foreach (var user in users)
                                    {
                                        ReplaceSharingEvent(user, sharingOption.actionId, uidData[0], myUri, old_ics,
                                                        calendarId, createDate, targetCalendar.TimeZones[0],
                                                        calendarObj.TimeZone);
                                    }
                                }
                            }
                        }
                        //calendar sharing options
                        foreach (var sharingOption in calendarCharingList)
                        {
                            if (!sharingOption.IsGroup)
                            {
                                var user = UserManager.GetUsers(sharingOption.itemId);
                                ReplaceSharingEvent(user, sharingOption.actionId, uidData[0], myUri, old_ics,
                                                    calendarId, createDate, targetCalendar.TimeZones[0],
                                                    calendarObj.TimeZone, cal.calDavGuid);
                            }
                            else
                            {
                                var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                                foreach (var user in users)
                                {
                                    ReplaceSharingEvent(user, sharingOption.actionId, uidData[0], myUri, old_ics,
                                                    calendarId, createDate, targetCalendar.TimeZones[0],
                                                    calendarObj.TimeZone, cal.calDavGuid);
                                }
                            }
                        }
                        if (!isShared)
                        {
                            updateCaldavEvent(old_ics, eventUid, true, calDavGuid, myUri, currentUserEmail, createDate, targetCalendar.TimeZones[0], calendarObj.TimeZone, false, isShared);
                        }
                        else
                        {
                            var owner = UserManager.GetUsers(evt.OwnerId);
                            updateCaldavEvent(old_ics, uidData[0], true, calendarObj.calDavGuid, myUri, owner.Email, createDate, targetCalendar.TimeZones[0], calendarObj.TimeZone, false, false);
                        }

                    });
                    sharingEventThread.Start();

                }
                catch (Exception e)
                {
                    Log.LogError(e, "update");
                }
            }

            return UpdateEvent(calendarId,
                               eventData.EventId,
                               eventObj.Summary,
                               eventObj.Description,
                               new ApiDateTime(utcStartDate, TimeZoneInfo.Utc.GetOffset()),
                               new ApiDateTime(utcEndDate, TimeZoneInfo.Utc.GetOffset()),
                               rrule,
                               eventData.AlertType,
                               eventObj.IsAllDay,
                               eventData.SharingOptions,
                               DDayICalParser.ConvertEventStatus(mergedEvent.Status), createDate,
                               fromCalDavServer, ownerId);
        }

        /// <summary>
        /// Deletes the whole event from the calendar (all events in the series)
        /// </summary>
        /// <short>
        /// Delete event series
        /// </short>
        /// <param name="eventId">Event ID</param>
        [HttpDelete("events/{eventId}")]
        public void RemoveEvent(int eventId)
        {
            RemoveEvent(eventId, new EventDeleteModel { EventId = eventId, Date = null, Type = EventRemoveType.AllSeries });
        }

        /// <summary>
        /// Deletes one event from the series of recurrent events
        /// </summary>
        /// <short>
        /// Delete event
        /// </short>
        /// <param name="eventId">Event ID</param>
        /// <param name="date">Date to be deleted from the recurrent event</param>
        /// <param name="type">Recurrent event deletion type</param>
        /// <param name="fromCaldavServer">Bool flag says that request from caldav server</param>
        /// <param name="uri">Current uri</param>
        /// <returns>Updated event series collection</returns>
        [HttpDelete("events/{eventId}/custom")]
        public List<EventWrapper> RemoveEvent(int eventId, EventDeleteModel eventDeleteModel)
        {
            var events = new List<EventWrapper>();
            var evt = DataProvider.GetEventById(eventId);

            if (evt == null)
                throw new Exception(Resources.CalendarApiResource.ErrorItemNotFound);

            var cal = DataProvider.GetCalendarById(Convert.ToInt32(evt.CalendarId));
            var pic = PublicItemCollectionHelper.GetForCalendar(cal);

            var uid = evt.Uid;
            string[] split = uid.Split(new Char[] { '@' });

            var sharingList = new List<SharingParam>();
            if (pic.Items.Count > 1)
            {
                sharingList.AddRange(from publicItem in pic.Items
                                     where publicItem.ItemId != cal.OwnerId.ToString()
                                     select new SharingParam
                                     {
                                         Id = Guid.Parse(publicItem.ItemId),
                                         isGroup = publicItem.IsGroup,
                                         actionId = publicItem.SharingOption.Id
                                     });
            }
            var permissions = PublicItemCollectionHelper.GetForEvent(evt);
            var so = permissions.Items
                .Where(x => x.SharingOption.Id != AccessOption.OwnerOption.Id)
                .Select(x => new SharingParam
                {
                    Id = x.Id,
                    actionId = x.SharingOption.Id,
                    isGroup = x.IsGroup
                }).ToList();

            var currentTenantId = TenantManager.GetCurrentTenant().Id;
            var calendarId = evt.CalendarId;
            var myUri = HttpContext.Request != null ? HttpContext.Request.GetUrlRewriter() : eventDeleteModel.Uri ?? new Uri("http://localhost");
            var currentUserId = AuthContext.CurrentAccount.ID;

            var removeEventThread = new Thread(() =>
             {
                 TenantManager.SetCurrentTenant(currentTenantId);
                 //calendar sharing list
                 foreach (var sharingOption in sharingList)
                 {
                     var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                     if (!sharingOption.IsGroup)
                     {
                         var user = UserManager.GetUsers(sharingOption.itemId);
                         if (DataProvider.CheckUserEmail(user))
                         {
                             deleteEvent(fullAccess ? split[0] + "_write" : split[0], calendarId, user.Email, myUri, user.Id != cal.OwnerId);
                         }
                     }
                     else
                     {
                         var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                         foreach (var user in users)
                         {
                             if (DataProvider.CheckUserEmail(user))
                             {
                                 var eventUid = user.Id == evt.OwnerId
                                                ? split[0]
                                                : fullAccess ? split[0] + "_write" : split[0];
                                 deleteEvent(eventUid, calendarId, user.Email, myUri, true);
                             }
                         }
                     }
                 }
                 //event sharing list
                 foreach (var sharingOption in so)
                 {
                     var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                     if (!sharingOption.IsGroup)
                     {
                         var user = UserManager.GetUsers(sharingOption.itemId);

                         deleteEvent(fullAccess ? split[0] + "_write" : split[0], SharedEventsCalendar.CalendarId, user.Email, myUri, user.Id != evt.OwnerId);
                     }
                     else
                     {
                         var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                         foreach (var user in users)
                         {
                             var eventUid = user.Id == evt.OwnerId
                                                ? split[0]
                                                : fullAccess ? split[0] + "_write" : split[0];

                             deleteEvent(eventUid, SharedEventsCalendar.CalendarId, user.Email, myUri, true);
                         }
                     }
                 }
                 if (currentUserId == evt.OwnerId)
                 {
                     var owner = UserManager.GetUsers(evt.OwnerId);

                     deleteEvent(split[0], evt.CalendarId, owner.Email, myUri);
                 }
                 if (calendarId != BirthdayReminderCalendar.CalendarId &&
                        calendarId != SharedEventsCalendar.CalendarId &&
                        calendarId != "crm_calendar" &&
                        !calendarId.Contains("Project_"))
                 {
                     if (currentUserId == cal.OwnerId)
                     {
                         var owner = UserManager.GetUsers(currentUserId);

                         deleteEvent(split[0], evt.CalendarId, owner.Email, myUri);
                     }
                 }
             });


            if (evt.OwnerId.Equals(AuthContext.CurrentAccount.ID) || CheckPermissions(evt, CalendarAccessRights.FullAccessAction, true) || CheckPermissions(cal, CalendarAccessRights.FullAccessAction, true))
            {
                if (eventDeleteModel.Type == EventRemoveType.AllSeries || evt.RecurrenceRule.Freq == Frequency.Never)
                {
                    DataProvider.RemoveEvent(eventId);

                    if (!eventDeleteModel.FromCaldavServer)
                    {
                        var ownerId = SecurityContext.CurrentAccount.ID != cal.OwnerId ? cal.OwnerId : SecurityContext.CurrentAccount.ID;
                        var email = UserManager.GetUsers(ownerId).Email;

                        deleteEvent(split[0], evt.CalendarId, email, myUri);
                        removeEventThread.Start();
                    }
                    return events;
                }

                var utcDate = evt.AllDayLong
                                  ? eventDeleteModel.Date.UtcTime.Date
                                  : TimeZoneInfo.ConvertTime(new DateTime(eventDeleteModel.Date.UtcTime.Ticks),
                                                             cal.ViewSettings.Any() && cal.ViewSettings.First().TimeZone != null
                                                                 ? cal.ViewSettings.First().TimeZone
                                                                 : cal.TimeZone,
                                                             TimeZoneInfo.Utc);

                if (eventDeleteModel.Type == EventRemoveType.Single)
                {
                    evt.RecurrenceRule.ExDates.Add(new RecurrenceRule.ExDate
                    {
                        Date = evt.AllDayLong ? utcDate.Date : utcDate,
                        IsDateTime = !evt.AllDayLong
                    });
                }
                else if (eventDeleteModel.Type == EventRemoveType.AllFollowing)
                {
                    var lastEventDate = evt.AllDayLong ? utcDate.Date : utcDate;
                    var dates = evt.RecurrenceRule
                        .GetDates(evt.UtcStartDate, evt.TimeZone, evt.AllDayLong, evt.UtcStartDate, evt.UtcStartDate.AddMonths(_monthCount), int.MaxValue, false)
                        .Where(x => x < lastEventDate)
                        .ToList();

                    var untilDate = dates.Any() ? dates.Last() : evt.UtcStartDate.AddDays(-1);

                    evt.RecurrenceRule.Until = evt.AllDayLong ? untilDate.Date : untilDate;
                }

                evt = DataProvider.UpdateEvent(int.Parse(evt.Id), evt.Uid, int.Parse(evt.CalendarId), evt.OwnerId, evt.Name, evt.Description,
                                              evt.UtcStartDate, evt.UtcEndDate, evt.RecurrenceRule, evt.AlertType, evt.AllDayLong,
                                              evt.SharingOptions.PublicItems, evt.Status, DateTime.Now);
                if (!eventDeleteModel.FromCaldavServer)
                {
                    try
                    {
                        var calDavGuid = cal != null ? cal.calDavGuid : "";
                        var currentUserEmail = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email.ToLower();

                        var calendarObj = DataProvider.GetCalendarById(Convert.ToInt32(cal.Id));
                        var calendarObjViewSettings = calendarObj != null && calendarObj.ViewSettings != null ? calendarObj.ViewSettings.FirstOrDefault() : null;
                        var targetCalendar = DDayICalParser.ConvertCalendar(calendarObj != null ? calendarObj.GetUserCalendar(calendarObjViewSettings) : null);

                        targetCalendar.Events.Clear();

                        var convertedEvent = DDayICalParser.ConvertEvent(evt);
                        convertedEvent.ExceptionDates.Clear();

                        foreach (var exDate in evt.RecurrenceRule.ExDates)
                        {
                            var periodList = new PeriodList { new CalDateTime(exDate.Date) };

                            if (exDate.IsDateTime)
                            {
                                periodList.Parameters.Add("TZID", targetCalendar.TimeZones[0].TzId);
                            }
                            else
                            {
                                periodList.Parameters.Add("VALUE", "DATE");
                            }
                            convertedEvent.ExceptionDates.Add(periodList);
                        }
                        targetCalendar.Events.Add(convertedEvent);
                        var ics = DDayICalParser.SerializeCalendar(targetCalendar);

                        var updateCaldavThread = new Thread(() => updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail, DateTime.Now, targetCalendar.TimeZones[0], cal.TimeZone, true));
                        updateCaldavThread.Start();
                    }
                    catch (Exception e)
                    {
                        Log.LogError(e.Message);
                    }
                }

                if (eventDeleteModel.Type != EventRemoveType.AllSeries)
                {
                    var history = DataProvider.GetEventHistory(eventId);
                    if (history != null)
                    {
                        var mergedCalendar = EventHistoryHelper.GetMerged(history);
                        if (mergedCalendar != null && mergedCalendar.Events != null && mergedCalendar.Events.Any())
                        {
                            if (evt.OwnerId != AuthContext.CurrentAccount.ID || CheckIsOrganizer(history))
                            {
                                mergedCalendar.Events[0].Sequence++;
                            }

                            mergedCalendar.Events[0].RecurrenceRules.Clear();

                            mergedCalendar.Events[0].RecurrenceRules.Add(DDayICalParser.DeserializeRecurrencePattern(evt.RecurrenceRule.ToString(true)));

                            mergedCalendar.Events[0].ExceptionDates.Clear();

                            foreach (var exDate in evt.RecurrenceRule.ExDates)
                            {
                                mergedCalendar.Events[0].ExceptionDates.Add(new Ical.Net.DataTypes.PeriodList
                                    {
                                        exDate.IsDateTime ?
                                            new Ical.Net.DataTypes.CalDateTime(exDate.Date.Year, exDate.Date.Month, exDate.Date.Day, exDate.Date.Hour, exDate.Date.Minute, exDate.Date.Second) :
                                            new Ical.Net.DataTypes.CalDateTime(exDate.Date.Year, exDate.Date.Month, exDate.Date.Day)
                                    });
                            }

                            DataProvider.AddEventHistory(int.Parse(evt.CalendarId), evt.Uid, int.Parse(evt.Id), DDayICalParser.SerializeCalendar(mergedCalendar));
                        }
                    }
                }

                //define timeZone
                TimeZoneInfo timeZone;
                if (!CheckPermissions(cal, CalendarAccessRights.FullAccessAction, true))
                {
                    timeZone = DataProvider.GetTimeZoneForSharedEventsCalendar(AuthContext.CurrentAccount.ID);
                    evt.CalendarId = SharedEventsCalendar.CalendarId;
                }
                else
                    timeZone = DataProvider.GetTimeZoneForCalendar(AuthContext.CurrentAccount.ID, int.Parse(evt.CalendarId));

                var eventWrapper = EventWrapperHelper.Get(evt, AuthContext.CurrentAccount.ID, timeZone);
                events = EventWrapperHelper.GetList(evt.UtcStartDate, evt.UtcStartDate.AddMonths(_monthCount), eventWrapper.UserId, evt);
            }
            else
                DataProvider.UnsubscribeFromEvent(eventId, AuthContext.CurrentAccount.ID);

            return events;
        }

        /// <summary>
        /// Creates the new task in the selected calendar with the parameters specified in the request
        /// </summary>
        /// <short>
        /// Create new task
        /// </short>
        /// <param name="ics">Task in iCal format</param>
        /// <param name="todoUid">Task uid</param>
        /// <returns>Todo</returns>
        [HttpPost("icstodo")]
        public List<TodoWrapper> AddTodo(CreateTodoModel createTodoModel)
        {
            var ics = createTodoModel.ics;
            var todoUid = createTodoModel.todoUid;

            var old_ics = ics;

            var todoCalendars = DataProvider.LoadTodoCalendarsForUser(AuthContext.CurrentAccount.ID);
            var userTimeZone = TenantManager.GetCurrentTenant().TimeZone;

            var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager, ClientFactory);
            var newCalendar = new BusinessObjects.Calendar(AuthContext, TimeZoneConverter, icalendar, DataProvider);
            var todoCal = CalendarWrapperHelper.Get(newCalendar);

            if (todoCalendars.Count == 0)
            {
                todoCal = CreateCalendar(new CalendarModel
                {
                    Name = "Todo_calendar",
                    Description = "",
                    TextColor = BusinessObjects.Calendar.DefaultTextColor,
                    BackgroundColor = BusinessObjects.Calendar.DefaultTodoBackgroundColor,
                    TimeZone = userTimeZone,
                    AlertType = EventAlertType.FifteenMinutes,
                    SharingOptions = null,
                    ICalUrl = null,
                    IsTodo = 1,
                });
            }

            var calendarId = Convert.ToInt32(todoCalendars.Count == 0 ? todoCal.Id : todoCalendars.FirstOrDefault().Id);

            if (calendarId <= 0)
            {
                var defaultCalendar = LoadInternalCalendars().First(x => (!x.IsSubscription && x.IsTodo != 1));
                if (!int.TryParse(defaultCalendar.Id, out calendarId))
                    throw new Exception($"Can't parse {defaultCalendar.Id} to int");
            }
            var calendars = DDayICalParser.DeserializeCalendar(ics);

            if (calendars == null) return null;

            var calendar = calendars.FirstOrDefault();

            if (calendar == null || calendar.Todos == null) return null;

            var todoObj = calendar.Todos.FirstOrDefault();

            if (todoObj == null) return null;

            var calendarObj = todoCalendars.Count == 0 ? DataProvider.GetCalendarById(Convert.ToInt32(todoCal.Id)) : todoCalendars.FirstOrDefault();
            var calendarObjViewSettings = calendarObj != null && calendarObj.ViewSettings != null ? calendarObj.ViewSettings.FirstOrDefault() : null;

            var targetCalendar = DDayICalParser.ConvertCalendar(calendarObj?.GetUserCalendar(calendarObjViewSettings));

            if (targetCalendar == null) return null;

            var utcStartDate = todoObj.Start != null ? DDayICalParser.ToUtc(todoObj.Start) : DateTime.MinValue;

            todoUid = todoUid == null ? null : $"{todoUid}@onlyoffice.com";


            var result = CreateTodo(calendarId,
                                    todoObj.Summary,
                                    todoObj.Description,
                                    utcStartDate,
                                    DataProvider.GetEventUid(todoUid),
                                    DateTime.MinValue);

            if (result == null || !result.Any()) return null;

            var todo = result.First();

            todoObj.Uid = todo.Uid;

            targetCalendar.Method = Ical.Net.CalendarMethods.Request;
            targetCalendar.Todos.Clear();
            targetCalendar.Todos.Add(todoObj);

            try
            {
                var uid = todo.Uid;
                string[] split = uid.Split(new Char[] { '@' });

                var calDavGuid = calendarObj != null ? calendarObj.calDavGuid : "";
                var myUri = HttpContext.Request.GetUrlRewriter();
                var currentUserEmail = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email.ToLower();

                var updateCaldavThread = new Thread(() => updateCaldavEvent(old_ics, split[0], true, calDavGuid, myUri, currentUserEmail));
                updateCaldavThread.Start();
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Updates the existing task with the parameters specified in the request
        /// </summary>
        /// <short>
        /// Update task
        /// </short>
        /// <param name="todoId">Task ID</param>
        /// <param name="calendarId">ID of the calendar where the task belongs</param>
        /// <param name="ics">Task in iCal format</param>
        /// <param name="fromCalDavServer">bool flag says that request from caldav server</param>
        /// <returns>Updated task</returns>
        [HttpPut("icstodo")]
        public List<TodoWrapper> UpdateTodo(CreateTodoModel createTodoModel)
        {
            var ics = createTodoModel.ics;
            var todoId = createTodoModel.todoId;
            var calendarId = createTodoModel.calendarId;
            var fromCalDavServer = createTodoModel.fromCalDavServer;

            var todo = DataProvider.GetTodoById(Convert.ToInt32(todoId));
            if (todo == null)
                throw new Exception(Resources.CalendarApiResource.ErrorItemNotFound);
            var old_ics = ics;

            var cal = DataProvider.GetCalendarById(Int32.Parse(todo.CalendarId));
            if (!fromCalDavServer)
            {
                if (!todo.OwnerId.Equals(AuthContext.CurrentAccount.ID) &&
                    !CheckPermissions(todo, CalendarAccessRights.FullAccessAction, true) &&
                    !CheckPermissions(cal, CalendarAccessRights.FullAccessAction, true))
                    throw new System.Security.SecurityException(Resources.CalendarApiResource.ErrorAccessDenied);
            }
            int calId;

            if (!int.TryParse(calendarId, out calId))
            {
                calId = int.Parse(todo.CalendarId);
            }

            var calendars = DDayICalParser.DeserializeCalendar(ics);

            if (calendars == null) return null;

            var calendar = calendars.FirstOrDefault();

            if (calendar == null || calendar.Events == null) return null;

            var todoObj = calendar.Todos.FirstOrDefault();

            if (todoObj == null) return null;

            var calendarObj = DataProvider.GetCalendarById(calId);
            var calendarObjViewSettings = calendarObj != null && calendarObj.ViewSettings != null ? calendarObj.ViewSettings.FirstOrDefault() : null;
            var targetCalendar = DDayICalParser.ConvertCalendar(calendarObj?.GetUserCalendar(calendarObjViewSettings));


            if (targetCalendar == null) return null;


            todoObj.Uid = todo.Uid;

            if (!fromCalDavServer)
            {
                try
                {
                    var uid = todo.Uid;
                    string[] split = uid.Split(new Char[] { '@' });

                    var calDavGuid = calendarObj != null ? calendarObj.calDavGuid : "";
                    var myUri = HttpContext.Request.GetUrlRewriter();
                    var currentUserEmail = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email.ToLower();

                    var updateCaldavThread = new Thread(() => updateCaldavEvent(old_ics, split[0], true, calDavGuid, myUri, currentUserEmail));
                    updateCaldavThread.Start();
                }
                catch (Exception e)
                {
                    Log.LogError(e.Message);
                }

            }

            var completed = todoObj.Completed == null ? DateTime.MinValue : DDayICalParser.ToUtc(todoObj.Completed);
            var utcStartDate = todoObj.DtStart != null ? DDayICalParser.ToUtc(todoObj.DtStart) : DateTime.MinValue;

            var result = UpdateTodo(
                                   int.Parse(calendarId),
                                   todoObj.Summary,
                                   todoObj.Description,
                                   utcStartDate,
                                   todoObj.Uid,
                                   completed);

            return result;

        }

        /// <summary>
        /// Deletes task
        /// </summary>
        /// <short>
        /// Delete task
        /// </short>
        /// <param name="todoId">Task ID</param>
        /// <param name="fromCaldavServer">Bool flag says that request from caldav server</param>
        [HttpDelete("todos/{todoId}")]
        public void RemoveTodo(int todoId, CreateTodoModel createTodoModel)
        {
            var fromCaldavServer = createTodoModel.fromCalDavServer;
            var todo = DataProvider.GetTodoById(todoId);

            var uid = todo.Uid;
            string[] split = uid.Split(new Char[] { '@' });

            if (!fromCaldavServer)
            {
                var email = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email;
                var myUri = HttpContext.Request.GetUrlRewriter();

                deleteEvent(split[0], todo.CalendarId, email, myUri);
            }

            DataProvider.RemoveTodo(todoId);

        }
        private List<TodoWrapper> CreateTodo(int calendarId, string name, string description, DateTime utcStartDate, string uid, DateTime completed)
        {
            name = (name ?? "").Trim();
            description = (description ?? "").Trim();

            if (!string.IsNullOrEmpty(uid))
            {
                var existTodo = DataProvider.GetTodoByUid(uid);

                if (existTodo != null)
                {
                    return null;
                }
            }

            CheckPermissions(DataProvider.GetCalendarById(calendarId), CalendarAccessRights.FullAccessAction);

            var todo = DataProvider.CreateTodo(calendarId,
                                                AuthContext.CurrentAccount.ID,
                                                name,
                                                description,
                                                utcStartDate,
                                                uid,
                                                completed);

            if (todo != null)
            {

                var todoResult = TodoWrapperHelper.Get(todo, AuthContext.CurrentAccount.ID,
                                        DataProvider.GetTimeZoneForCalendar(AuthContext.CurrentAccount.ID, calendarId))
                                        .GetList();
                return todoResult;
            }
            return null;
        }
        private List<TodoWrapper> UpdateTodo(int calendarId, string name, string description, DateTime utcStartDate, string uid, DateTime completed)
        {
            name = (name ?? "").Trim();
            description = (description ?? "").Trim();

            if (!string.IsNullOrEmpty(uid))
            {
                var existTodo = DataProvider.GetTodoByUid(uid);
                CheckPermissions(DataProvider.GetCalendarById(calendarId), CalendarAccessRights.FullAccessAction);

                var todo = DataProvider.UpdateTodo(existTodo.Id, calendarId, AuthContext.CurrentAccount.ID, name, description, utcStartDate, uid, completed);

                if (todo != null)
                {

                    var todoResult = TodoWrapperHelper.Get(todo, AuthContext.CurrentAccount.ID,
                                            DataProvider.GetTimeZoneForCalendar(AuthContext.CurrentAccount.ID, calendarId))
                                            .GetList();
                    return todoResult;
                }
            }
            return null;
        }


        [HttpPost("outsideevent")]
        public void AddEventOutside(OutsideEventModel outsidEventModel)
        {
            var calendarGuid = outsidEventModel.CalendarGuid;
            var eventGuid = outsidEventModel.EventGuid;
            var ics = outsidEventModel.Ics;

            if (calendarGuid.IndexOf("-shared") > 0)
            {
                var caldavGuid = calendarGuid.Replace("-shared", "");

                var calendarTmp = DataProvider.GetCalendarIdByCaldavGuid(caldavGuid);
                var calendarId = Convert.ToInt32(calendarTmp[0][0]);

                var eventData = DataProvider.GetEventIdByUid(eventGuid.Split('.')[0], calendarId);

                if (eventData == null)
                {
                    var eventModel = new EventModel
                    {
                        Ics = ics,
                        AlertType = EventAlertType.Never,
                        SharingOptions = new List<SharingParam>()
                    };
                    AddEvent(calendarId, eventModel);
                }
                else
                {
                    if (eventData.OwnerId == SecurityContext.CurrentAccount.ID)
                    {
                        var cal = DataProvider.GetCalendarById(calendarId);
                        var sharingOptions = eventData.SharingOptions;
                        var eventCharingList = new List<SharingParam>();
                        if (sharingOptions.PublicItems.Count > 1)
                        {
                            eventCharingList.AddRange(from publicItem in sharingOptions.PublicItems
                                                      where publicItem.Id.ToString() != AccessOption.OwnerOption.Id
                                                      select new SharingParam
                                                      {
                                                          Id = publicItem.Id,
                                                          isGroup = publicItem.IsGroup
                                                      });
                        }
                        var eventModel = new EventModel
                        {
                            EventId = Convert.ToInt32(eventData.Id),
                            CalendarId = calendarId.ToString(),
                            Ics = ics,
                            AlertType = EventAlertType.Never,
                            SharingOptions = eventCharingList

                        };
                        UpdateEvent(eventModel);
                    }

                }
            }
        }

        /// <summary>
        /// Returns the sharing access parameters to the calendar with the ID specified in the request
        /// </summary>
        /// <short>
        /// Get access parameters
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <returns>Sharing access parameters</returns>
        [HttpGet("{calendarId}/sharing")]
        public PublicItemCollection GetCalendarSharingOptions(int calendarId)
        {
            var cal = DataProvider.GetCalendarById(calendarId);
            if (cal == null)
                throw new Exception(Resources.CalendarApiResource.ErrorItemNotFound);

            return PublicItemCollectionHelper.GetForCalendar(cal);
        }

        /// <summary>
        /// Returns the default values for the sharing access parameters
        /// </summary>
        /// <short>
        /// Get default access
        /// </short>
        /// <returns>Default sharing access parameters</returns>
        [HttpGet("sharing")]
        public PublicItemCollection GetDefaultSharingOptions()
        {
            return PublicItemCollectionHelper.GetDefault();
        }
        public class UploadModel
        {
            public IEnumerable<IFormFile> Files { get; set; }
        }

        /// <summary>
        /// Imports the events from the iCal files
        /// </summary>
        /// <short>
        /// Import iCal
        /// </short>
        /// <param name="files">iCal formatted files with the events to be imported</param>
        /// <returns>Returns the number of imported events</returns>
        [HttpPost("import")]
        public int ImportEvents(UploadModel uploadModel)
        {
            var calendar = LoadInternalCalendars().First(x => (!x.IsSubscription && x.IsTodo != 1));
            int calendarId;

            if (int.TryParse(calendar.Id, out calendarId))
                return ImportEvents(calendarId, uploadModel);

            throw new Exception($"Can't parse {calendar.Id} to int");
        }

        /// <summary>
        /// Imports the events from the iCal files to the existing calendar
        /// </summary>
        /// <short>
        /// Import iCal
        /// </short>
        /// <param name="calendarId">ID for the calendar which serves as the future storage base for the imported events</param>
        /// <param name="files">iCal formatted files with the events to be imported</param>
        /// <returns>Returns the number of imported events</returns>
        [HttpPost("{calendarId}/import")]
        public int ImportEvents(int calendarId, UploadModel uploadModel)
        {
            var counter = 0;
            var files = uploadModel.Files;

            if (files != null)
            {
                foreach (var file in files)
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        var cals = DDayICalParser.DeserializeCalendar(reader);

                        counter = ImportEvents(calendarId, cals);
                    }
                }
            }

            return counter;
        }

        public class ImportModel
        {
            public int CalendarId { get; set; }
            public string ICalString { get; set; }
        }

        /// <summary>
        /// Imports the events from the iCal files
        /// </summary>
        /// <short>
        /// Import iCal
        /// </short>
        /// <param name="calendarId">Calendar ID</param>
        /// <param name="iCalString">iCal formatted string</param>
        /// <returns>Returns the number of imported events</returns>
        [HttpPost("importIcs")]
        public int ImportEvents(ImportModel importModel)
        {
            var calendarId = importModel.CalendarId;
            var iCalString = importModel.ICalString;

            if (calendarId > 0)
            {
                var cals = DDayICalParser.DeserializeCalendar(iCalString);
                return ImportEvents(calendarId, cals);
            }

            var calendar = LoadInternalCalendars().First(x => (!x.IsSubscription && x.IsTodo != 1));

            if (int.TryParse(calendar.Id, out calendarId))
            {
                importModel.CalendarId = calendarId;
                return ImportEvents(importModel);
            }


            throw new Exception($"Can't parse {calendar.Id} to int");
        }

        /// <summary>
        /// Creates a calendar by the link to the external iCal feed
        /// </summary>
        /// <short>
        /// Create calendar
        /// </short>
        /// <param name="iCalUrl">Link to the external iCal feed</param>
        /// <param name="name">Calendar name</param>
        /// <param name="textColor">Event text name</param>
        /// <param name="backgroundColor">Event background name</param>
        /// <returns>Created calendar</returns>
        [HttpPost("calendarUrl")]
        public CalendarWrapper CreateCalendarStream(СalendarUrlModel сalendarUrl)
        {
            var iCalUrl = сalendarUrl.ICalUrl;
            var name = сalendarUrl.Name;
            var textColor = сalendarUrl.TextColor;
            var backgroundColor = сalendarUrl.BackgroundColor;

            var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager, ClientFactory);
            var cal = icalendar.GetFromUrl(iCalUrl);
            if (cal.isEmptyName)
                cal.Name = iCalUrl;

            if (String.IsNullOrEmpty(name))
                name = cal.Name;

            textColor = (textColor ?? "").Trim();
            backgroundColor = (backgroundColor ?? "").Trim();

            var calendar = DataProvider.CreateCalendar(
                        AuthContext.CurrentAccount.ID, name, cal.Description ?? "", textColor, backgroundColor,
                        cal.TimeZone, cal.EventAlertType, iCalUrl, null, new List<UserViewSettings>(), Guid.Empty);

            if (calendar != null)
            {
                var calendarWrapperr = UpdateCalendarView(calendar.Id, new CalendarModel
                {
                    Name = calendar.Name,
                    TextColor = textColor,
                    BackgroundColor = backgroundColor,
                    TimeZone = calendar.TimeZone.Id,
                    AlertType = cal.EventAlertType,
                    HideEvents = false
                });

                return calendarWrapperr;
            }

            return null;
        }

        private List<CalendarWrapper> LoadInternalCalendars()
        {
            var result = new List<CalendarWrapper>();
            int newCalendarsCount;
            //internal
            var calendars = DataProvider.LoadCalendarsForUser(AuthContext.CurrentAccount.ID, out newCalendarsCount);

            var userTimeZone = TimeZoneConverter.GetTimeZone(TenantManager.GetCurrentTenant().TimeZone);

            result.AddRange(calendars.ConvertAll(c => CalendarWrapperHelper.Get(c)));

            foreach (var calendarWrapper in result.ToList())
            {
                if (calendarWrapper.Owner.Id != SecurityContext.CurrentAccount.ID)
                {

                    var ownerViewSettings = DataProvider.GetUserViewSettings(calendarWrapper.Owner.Id, new List<string>() { calendarWrapper.Id });

                    var userViewSettings = DataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, new List<string>() { calendarWrapper.Id });

                    userViewSettings.FirstOrDefault().TimeZone = ownerViewSettings.FirstOrDefault().TimeZone;

                    var newCal = CalendarWrapperHelper.Get(calendarWrapper.UserCalendar, userViewSettings.FirstOrDefault());

                    result.Remove(calendarWrapper);
                    result.Add(newCal);
                }
            }

            if (!result.Exists(c => !c.IsSubscription))
            {
                //create first calendar
                var firstCal = DataProvider.CreateCalendar(AuthContext.CurrentAccount.ID,
                        Resources.CalendarApiResource.DefaultCalendarName, "", BusinessObjects.Calendar.DefaultTextColor, BusinessObjects.Calendar.DefaultBackgroundColor, userTimeZone, EventAlertType.FifteenMinutes, null, new List<SharingOptions.PublicItem>(), new List<UserViewSettings>(), Guid.Empty);

                result.Add(CalendarWrapperHelper.Get(firstCal));
            }

            return result;
        }
        private EventHistoryWrapper GetEventHistoryWrapper(Event evt, bool fullHistory = false)
        {
            if (evt == null) return null;

            int calId;
            BusinessObjects.Calendar cal = null;

            if (int.TryParse(evt.CalendarId, out calId))
                cal = DataProvider.GetCalendarById(calId);

            if (cal == null) return null;

            int evtId;
            EventHistory history = null;

            if (int.TryParse(evt.Id, out evtId))
                history = DataProvider.GetEventHistory(evtId);

            if (history == null) return null;

            return ToEventHistoryWrapper(evt, cal, history, fullHistory);
        }
        private EventHistoryWrapper ToEventHistoryWrapper(Event evt, BusinessObjects.Calendar cal, EventHistory history, bool fullHistory = false)
        {
            var canNotify = false;
            bool canEdit;

            var calIsShared = cal.SharingOptions.SharedForAll || cal.SharingOptions.PublicItems.Count > 0;
            if (calIsShared)
            {
                canEdit = canNotify = CheckPermissions(cal, CalendarAccessRights.FullAccessAction, true);
                return EventHistoryWrapperHelper.Get(history, canEdit, canNotify, cal, fullHistory);
            }

            var evtIsShared = evt.SharingOptions.SharedForAll || evt.SharingOptions.PublicItems.Count > 0;
            if (evtIsShared)
            {
                canEdit = canNotify = CheckPermissions(evt, CalendarAccessRights.FullAccessAction, true);
                return EventHistoryWrapperHelper.Get(history, canEdit, canNotify, cal, fullHistory);
            }

            canEdit = CheckPermissions(evt, CalendarAccessRights.FullAccessAction, true);
            if (canEdit)
            {
                canNotify = CheckIsOrganizer(history);
            }

            return EventHistoryWrapperHelper.Get(history, canEdit, canNotify, cal, fullHistory);
        }
        private bool CheckIsOrganizer(EventHistory history)
        {

            var canNotify = false;
            //TODO

            //var apiServer = new ApiServer();
            //var apiResponse = apiServer.GetApiResponse(String.Format("{0}mail/accounts.json", SetupInfo.WebApiBaseUrl), "GET");
            //var obj = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(apiResponse)));

            //var obj = JObject.Parse("");

            var obj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(@"{}");

            if (obj.TryGetValue("response", out var response))
            {
                var accounts = (from account in response.EnumerateArray()
                                let email = account.GetProperty("email").GetString()
                                let enabled = account.GetProperty("enabled").GetBoolean()
                                let isGroup = account.GetProperty("isGroup").GetBoolean()
                                where enabled && !isGroup
                                select email).ToList();

                if (accounts.Any())
                {
                    var mergedHistory = EventHistoryHelper.GetMerged(history);
                    if (mergedHistory != null && mergedHistory.Events != null)
                    {
                        var eventObj = mergedHistory.Events.FirstOrDefault();
                        if (eventObj != null && eventObj.Organizer != null)
                        {
                            var organizerEmail = eventObj.Organizer.Value.ToString()
                                                         .ToLowerInvariant()
                                                         .Replace("mailto:", "");

                            canNotify = accounts.Contains(organizerEmail);
                        }
                    }
                }
            }

            return canNotify;
        }
        private int ImportEvents(int calendarId, IEnumerable<Ical.Net.Calendar> cals)
        {
            var counter = 0;

            CheckPermissions(DataProvider.GetCalendarById(calendarId), CalendarAccessRights.FullAccessAction);

            if (cals == null) return counter;

            var calendars = cals.Where(x => string.IsNullOrEmpty(x.Method) ||
                                            x.Method == Ical.Net.CalendarMethods.Publish ||
                                            x.Method == Ical.Net.CalendarMethods.Request ||
                                            x.Method == Ical.Net.CalendarMethods.Reply ||
                                            x.Method == Ical.Net.CalendarMethods.Cancel).ToList();

            foreach (var calendar in calendars)
            {
                if (calendar.Events == null) continue;

                if (string.IsNullOrEmpty(calendar.Method))
                    calendar.Method = Ical.Net.CalendarMethods.Publish;

                foreach (var eventObj in calendar.Events)
                {
                    if (eventObj == null) continue;

                    var tmpCalendar = calendar.Copy<Ical.Net.Calendar>();
                    tmpCalendar.Events.Clear();
                    tmpCalendar.Events.Add(eventObj);

                    string rrule;
                    var ics = DDayICalParser.SerializeCalendar(tmpCalendar);

                    var eventHistory = DataProvider.GetEventHistory(eventObj.Uid);

                    if (eventHistory == null)
                    {
                        rrule = GetRRuleString(eventObj);

                        var utcStartDate = eventObj.IsAllDay ? eventObj.Start.Value : DDayICalParser.ToUtc(eventObj.Start);
                        var utcEndDate = eventObj.IsAllDay ? eventObj.End.Value : DDayICalParser.ToUtc(eventObj.End);

                        var existCalendar = DataProvider.GetCalendarById(calendarId);
                        if (!eventObj.IsAllDay && eventObj.Created != null && !eventObj.Start.IsUtc)
                        {
                            var offset = existCalendar.TimeZone.GetUtcOffset(eventObj.Created.Value);

                            var _utcStartDate = eventObj.Start.Subtract(offset).Value;
                            var _utcEndDate = eventObj.End.Subtract(offset).Value;

                            utcStartDate = _utcStartDate;
                            utcEndDate = _utcEndDate;
                        }
                        else if (!eventObj.IsAllDay && eventObj.Created != null)
                        {
                            var createOffset = existCalendar.TimeZone.GetUtcOffset(eventObj.Created.Value);
                            var startOffset = existCalendar.TimeZone.GetUtcOffset(eventObj.Start.Value);
                            var endOffset = existCalendar.TimeZone.GetUtcOffset(eventObj.End.Value);

                            if (createOffset != startOffset)
                            {
                                var _utcStartDate = eventObj.Start.Subtract(createOffset).Add(startOffset).Value;
                                utcStartDate = _utcStartDate;
                            }
                            if (createOffset != endOffset)
                            {
                                var _utcEndDate = eventObj.End.Subtract(createOffset).Add(endOffset).Value;
                                utcEndDate = _utcEndDate;
                            }
                        }

                        if (eventObj.IsAllDay && utcStartDate.Date < utcEndDate.Date)
                            utcEndDate = utcEndDate.AddDays(-1);

                        try
                        {
                            var uid = eventObj.Uid;
                            string[] split = uid.Split(new Char[] { '@' });

                            var calDavGuid = existCalendar != null ? existCalendar.calDavGuid : "";
                            var myUri = HttpContext.Request.GetUrlRewriter();
                            var currentUserEmail = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email.ToLower();

                            //TODO caldav
                            var updateCaldavThread = new Thread(() => updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail, DateTime.Now, tmpCalendar.TimeZones[0], existCalendar.TimeZone));
                            updateCaldavThread.Start();
                        }
                        catch (Exception e)
                        {
                            Log.LogError(e.Message);
                        }

                        var result = CreateEvent(calendarId,
                                                 eventObj.Summary,
                                                 eventObj.Description,
                                                 utcStartDate,
                                                 utcEndDate,
                                                 RecurrenceRule.Parse(rrule),
                                                 EventAlertType.Default,
                                                 eventObj.IsAllDay,
                                                 null,
                                                 eventObj.Uid,
                                                 calendar.Method == Ical.Net.CalendarMethods.Cancel ? EventStatus.Cancelled : DDayICalParser.ConvertEventStatus(eventObj.Status), eventObj.Created != null ? eventObj.Created.Value : DateTime.Now);

                        var eventId = result != null && result.Any() ? Int32.Parse(result.First().Id) : 0;

                        if (eventId > 0)
                        {
                            DataProvider.AddEventHistory(calendarId, eventObj.Uid, eventId, ics);
                            counter++;
                        }
                    }
                    else
                    {
                        if (EventHistoryHelper.Contains(tmpCalendar, eventHistory)) continue;

                        eventHistory = DataProvider.AddEventHistory(eventHistory.CalendarId, eventHistory.EventUid,
                                                                     eventHistory.EventId, ics);

                        var mergedCalendar = EventHistoryHelper.GetMerged(eventHistory);

                        if (mergedCalendar == null || mergedCalendar.Events == null || !mergedCalendar.Events.Any()) continue;

                        var mergedEvent = mergedCalendar.Events.First();

                        rrule = GetRRuleString(mergedEvent);

                        var utcStartDate = mergedEvent.IsAllDay ? mergedEvent.Start.Value : DDayICalParser.ToUtc(mergedEvent.Start);
                        var utcEndDate = mergedEvent.IsAllDay ? mergedEvent.End.Value : DDayICalParser.ToUtc(mergedEvent.End);

                        var existCalendar = DataProvider.GetCalendarById(calendarId);
                        if (!eventObj.IsAllDay && eventObj.Created != null && !eventObj.Start.IsUtc)
                        {
                            var offset = existCalendar.TimeZone.GetUtcOffset(eventObj.Created.Value);

                            var _utcStartDate = eventObj.Start.Subtract(offset).Value;
                            var _utcEndDate = eventObj.End.Subtract(offset).Value;

                            utcStartDate = _utcStartDate;
                            utcEndDate = _utcEndDate;
                        }

                        if (mergedEvent.IsAllDay && utcStartDate.Date < utcEndDate.Date)
                            utcEndDate = utcEndDate.AddDays(-1);

                        var targetEvent = DataProvider.GetEventById(eventHistory.EventId);
                        var permissions = PublicItemCollectionHelper.GetForEvent(targetEvent);
                        var sharingOptions = permissions.Items
                            .Where(x => x.SharingOption.Id != AccessOption.OwnerOption.Id)
                            .Select(x => new SharingParam
                            {
                                Id = x.Id,
                                actionId = x.SharingOption.Id,
                                isGroup = x.IsGroup
                            }).ToList();

                        try
                        {
                            var uid = eventObj.Uid;
                            string[] split = uid.Split(new Char[] { '@' });

                            var calDavGuid = existCalendar != null ? existCalendar.calDavGuid : "";
                            var myUri = HttpContext.Request.GetUrlRewriter(); ;
                            var currentUserEmail = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email.ToLower();

                            var updateCaldavThread = new Thread(() => updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail, DateTime.Now, tmpCalendar.TimeZones[0], existCalendar.TimeZone));
                            updateCaldavThread.Start();
                        }
                        catch (Exception e)
                        {
                            Log.LogError(e.Message);
                        }

                        //updateEvent(ics, split[0], calendarId.ToString(), true, DateTime.Now, tmpCalendar.TimeZones[0], existCalendar.TimeZone);

                        CreateEvent(eventHistory.CalendarId,
                                    mergedEvent.Summary,
                                    mergedEvent.Description,
                                    utcStartDate,
                                    utcEndDate,
                                    RecurrenceRule.Parse(rrule),
                                    EventAlertType.Default,
                                    mergedEvent.IsAllDay,
                                    sharingOptions,
                                    mergedEvent.Uid,
                                    DDayICalParser.ConvertEventStatus(mergedEvent.Status), eventObj.Created != null ? eventObj.Created.Value : DateTime.Now);

                        counter++;
                    }
                }
            }

            return counter;

        }
        private void CheckPermissions(ISecurityObject securityObj, Common.Security.Authorizing.Action action)
        {
            CheckPermissions(securityObj, action, false);
        }
        private bool CheckPermissions(ISecurityObject securityObj, Common.Security.Authorizing.Action action, bool silent)
        {
            if (securityObj == null)
                throw new Exception(Resources.CalendarApiResource.ErrorItemNotFound);

            if (silent)
                return PermissionContext.CheckPermissions(securityObj, action);

            PermissionContext.DemandPermissions(securityObj, action);

            return true;
        }
        private string GetRRuleString(Ical.Net.CalendarComponents.CalendarEvent evt)
        {
            var rrule = string.Empty;

            if (evt.RecurrenceRules != null && evt.RecurrenceRules.Any())
            {
                var recurrenceRules = evt.RecurrenceRules.ToList();

                rrule = DDayICalParser.SerializeRecurrencePattern(recurrenceRules.First());

                if (evt.ExceptionDates != null && evt.ExceptionDates.Any())
                {
                    rrule += ";exdates=";

                    var exceptionDates = evt.ExceptionDates.ToList();

                    foreach (var periodList in exceptionDates)
                    {
                        var date = periodList.ToString();

                        //has time
                        if (date.IndexOf('t', StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            //is utc time
                            if (date.IndexOf('z', StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                rrule += date;
                            }
                            else
                            {
                                //convert to utc time
                                DateTime dt;
                                if (DateTime.TryParseExact(date.ToUpper(), "yyyyMMdd'T'HHmmssK", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dt))
                                {
                                    var tzid = periodList.TzId ?? evt.Start.TzId;
                                    if (!String.IsNullOrEmpty(tzid))
                                    {
                                        dt = TimeZoneInfo.ConvertTime(dt, TimeZoneConverter.GetTimeZone(tzid), TimeZoneInfo.Utc);
                                    }
                                    rrule += dt.ToString("yyyyMMdd'T'HHmmssK");
                                }
                                else
                                {
                                    rrule += date;
                                }
                            }
                        }
                        //for yyyyMMdd/P1D date. Bug in the ical.net
                        else if (date.IndexOf("/p", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            try
                            {
                                rrule += date.Split('/')[0];
                            }
                            catch (Exception ex)
                            {
                                Log.LogError(String.Format("Error: {0}, Date string: {1}", ex, date));
                                rrule += date;
                            }
                        }
                        else
                        {
                            rrule += date;
                        }

                        rrule += ",";
                    }

                    rrule = rrule.TrimEnd(',');
                }
            }

            return rrule;
        }
        private List<EventWrapper> CreateEvent(int calendarId, string name, string description, DateTime utcStartDate, DateTime utcEndDate, RecurrenceRule rrule, EventAlertType alertType, bool isAllDayLong, List<SharingParam> sharingOptions, string uid, EventStatus status, DateTime createDate)
        {
            var sharingOptionsList = sharingOptions ?? new List<SharingParam>();

            name = (name ?? "").Trim();
            description = (description ?? "").Trim();

            if (!string.IsNullOrEmpty(uid))
            {
                var existEvent = DataProvider.GetEventByUid(uid);

                if (existEvent != null)
                {
                    return UpdateEvent(existEvent.CalendarId,
                                       int.Parse(existEvent.Id),
                                       name,
                                       description,
                                       new ApiDateTime(utcStartDate, TimeZoneInfo.Utc.GetOffset()),
                                       new ApiDateTime(utcEndDate, TimeZoneInfo.Utc.GetOffset()),
                                       rrule.ToString(),
                                       alertType,
                                       isAllDayLong,
                                       sharingOptions,
                                       status,
                                       createDate);
                }
            }

            CheckPermissions(DataProvider.GetCalendarById(calendarId), CalendarAccessRights.FullAccessAction);

            var evt = DataProvider.CreateEvent(calendarId,
                                                AuthContext.CurrentAccount.ID,
                                                name,
                                                description,
                                                utcStartDate,
                                                utcEndDate,
                                                rrule,
                                                alertType,
                                                isAllDayLong,
                                                sharingOptionsList.Select(o => o as SharingOptions.PublicItem).ToList(),
                                                uid,
                                                status,
                                                createDate);

            if (evt != null)
            {
                foreach (var opt in sharingOptionsList)
                    if (String.Equals(opt.actionId, AccessOption.FullAccessOption.Id, StringComparison.InvariantCultureIgnoreCase))
                        AuthorizationManager.AddAce(new AzRecord(opt.Id, CalendarAccessRights.FullAccessAction.ID, Common.Security.Authorizing.AceType.Allow, evt));

                //notify
                CalendarNotifyClient.NotifyAboutSharingEvent(evt);

                var eventWrapper = EventWrapperHelper.Get(evt, AuthContext.CurrentAccount.ID,
                                       DataProvider.GetTimeZoneForCalendar(AuthContext.CurrentAccount.ID, calendarId));
                return EventWrapperHelper.GetList(utcStartDate, utcStartDate.AddMonths(_monthCount), eventWrapper.UserId, evt);
            }
            return null;
        }
        private List<EventWrapper> UpdateEvent(string calendarId, int eventId, string name, string description, ApiDateTime startDate, ApiDateTime endDate, string repeatType, EventAlertType alertType, bool isAllDayLong, List<SharingParam> sharingOptions, EventStatus status, DateTime createDate, bool fromCalDavServer = false, string ownerId = "")
        {
            var sharingOptionsList = sharingOptions ?? new List<SharingParam>();

            var oldEvent = DataProvider.GetEventById(eventId);
            var ownerGuid = fromCalDavServer ? Guid.Parse(ownerId) : Guid.Empty; //get userGuid in the case of a request from the server
            if (oldEvent == null)
                throw new Exception(Resources.CalendarApiResource.ErrorItemNotFound);

            var cal = DataProvider.GetCalendarById(Int32.Parse(oldEvent.CalendarId));

            if (!fromCalDavServer)
            {
                if (!oldEvent.OwnerId.Equals(AuthContext.CurrentAccount.ID) &&
                    !CheckPermissions(oldEvent, CalendarAccessRights.FullAccessAction, true) &&
                    !CheckPermissions(cal, CalendarAccessRights.FullAccessAction, true))
                    throw new System.Security.SecurityException(Resources.CalendarApiResource.ErrorAccessDenied);

            }
            name = (name ?? "").Trim();
            description = (description ?? "").Trim();

            TimeZoneInfo timeZone;

            var calId = int.Parse(oldEvent.CalendarId);

            if (!int.TryParse(calendarId, out calId))
            {
                calId = int.Parse(oldEvent.CalendarId);
                timeZone = fromCalDavServer ? DataProvider.GetTimeZoneForSharedEventsCalendar(ownerGuid) : DataProvider.GetTimeZoneForSharedEventsCalendar(AuthContext.CurrentAccount.ID);
            }
            else
                timeZone = fromCalDavServer ? DataProvider.GetTimeZoneForCalendar(ownerGuid, calId) : DataProvider.GetTimeZoneForCalendar(AuthContext.CurrentAccount.ID, calId);

            var rrule = RecurrenceRule.Parse(repeatType);
            var evt = DataProvider.UpdateEvent(eventId, oldEvent.Uid, calId,
                                                oldEvent.OwnerId, name, description, startDate.UtcTime, endDate.UtcTime, rrule, alertType, isAllDayLong,
                                                sharingOptionsList.Select(o => o as SharingOptions.PublicItem).ToList(), status, createDate);

            if (evt != null)
            {
                //clear old rights
                AuthorizationManager.RemoveAllAces(evt);

                foreach (var opt in sharingOptionsList)
                    if (String.Equals(opt.actionId, AccessOption.FullAccessOption.Id, StringComparison.InvariantCultureIgnoreCase))
                        AuthorizationManager.AddAce(new AzRecord(opt.Id, CalendarAccessRights.FullAccessAction.ID, Common.Security.Authorizing.AceType.Allow, evt));

                //notify
                CalendarNotifyClient.NotifyAboutSharingEvent(evt, oldEvent);

                evt.CalendarId = calendarId;

                var eventWrapper = fromCalDavServer ?
                    EventWrapperHelper.Get(evt, ownerGuid, timeZone) :
                    EventWrapperHelper.Get(evt, AuthContext.CurrentAccount.ID, timeZone);
                return EventWrapperHelper.GetList(startDate.UtcTime, startDate.UtcTime.AddMonths(_monthCount), eventWrapper.UserId, evt);

            }
            return null;
        }
        private string GetCalendariCalString(string calendarId, bool ignoreCache = false)
        {
            Log.LogDebug("GetCalendariCalString calendarId = " + calendarId);

            try
            {
                var result = ExportDataCache.Get(calendarId);

                if (!string.IsNullOrEmpty(result) && !ignoreCache)
                    return result;

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                BaseCalendar icalendar;
                int calId;

                var viewSettings = DataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, new List<string> { calendarId });

                if (int.TryParse(calendarId, out calId))
                {
                    icalendar = DataProvider.GetCalendarById(calId);
                    if (icalendar != null)
                    {
                        icalendar = icalendar.GetUserCalendar(viewSettings.FirstOrDefault());
                    }
                }
                else
                {
                    //external                
                    icalendar = CalendarManager.GetCalendarForUser(AuthContext.CurrentAccount.ID, calendarId, UserManager);
                    if (icalendar != null)
                    {
                        icalendar = icalendar.GetUserCalendar(viewSettings.FirstOrDefault());
                    }
                }

                if (icalendar == null) return null;

                var ddayCalendar = DDayICalParser.ConvertCalendar(icalendar);
                ddayCalendar.Events.Clear();

                var events = icalendar.LoadEvents(SecurityContext.CurrentAccount.ID, DateTime.MinValue, DateTime.MaxValue);
                var eventIds = new List<int>();

                foreach (var e in events)
                {
                    int evtId;

                    if (int.TryParse(e.Id, out evtId))
                        eventIds.Add(evtId);
                }

                var eventsHystory = DataProvider.GetEventsHistory(eventIds.ToArray());

                foreach (var e in events)
                {
                    int evtId;
                    EventHistory evtHistory = null;

                    if (int.TryParse(e.Id, out evtId))
                        evtHistory = eventsHystory.FirstOrDefault(x => x.EventId == evtId);

                    var offset = icalendar.TimeZone.GetUtcOffset(e.UtcUpdateDate);

                    if (evtHistory != null)
                    {
                        var mergedCalendar = EventHistoryHelper.GetMerged(evtHistory);
                        if (mergedCalendar == null || mergedCalendar.Events == null || !mergedCalendar.Events.Any())
                            continue;

                        var mergedEvent = mergedCalendar.Events.First();

                        mergedEvent.ExceptionDates.Clear();

                        foreach (var exDate in e.RecurrenceRule.ExDates)
                        {
                            var periodList = new PeriodList { new CalDateTime(exDate.Date) };

                            if (exDate.IsDateTime)
                            {
                                periodList.Parameters.Add("TZID", ddayCalendar.TimeZones[0].TzId);
                            }
                            else
                            {
                                periodList.Parameters.Add("VALUE", "DATE");
                            }
                            mergedEvent.ExceptionDates.Add(periodList);
                        }

                        if (!mergedEvent.IsAllDay && mergedEvent.DtStart.IsUtc)
                        {
                            var _DtStart = mergedEvent.DtStart.Add(offset).Value;
                            var _DtEnd = mergedEvent.DtEnd.Add(offset).Value;

                            mergedEvent.DtStart = new CalDateTime(_DtStart.Year, _DtStart.Month, _DtStart.Day, _DtStart.Hour, _DtStart.Minute, _DtStart.Second, ddayCalendar.TimeZones[0].TzId);
                            mergedEvent.DtEnd = new CalDateTime(_DtEnd.Year, _DtEnd.Month, _DtEnd.Day, _DtEnd.Hour, _DtEnd.Minute, _DtEnd.Second, ddayCalendar.TimeZones[0].TzId);

                        }
                        var alarm = mergedEvent.Alarms.FirstOrDefault();
                        if (alarm != null)
                        {
                            if (alarm.Trigger == null)
                            {
                                mergedEvent.Alarms.Clear();
                            }
                        }
                        else
                        {
                            mergedEvent.Alarms.Clear();
                        }
                        ddayCalendar.Events.Add(mergedEvent);
                    }
                    else
                    {
                        var convertedEvent = DDayICalParser.ConvertEvent(e as BaseEvent);
                        if (string.IsNullOrEmpty(convertedEvent.Uid))
                            convertedEvent.Uid = DataProvider.GetEventUid(e.Uid, e.Id);

                        if (!convertedEvent.IsAllDay)
                        {
                            var _DtStart = convertedEvent.DtStart.Add(offset).Value;
                            var _DtEnd = convertedEvent.DtEnd.Add(offset).Value;

                            convertedEvent.DtStart = new CalDateTime(_DtStart.Year, _DtStart.Month, _DtStart.Day, _DtStart.Hour, _DtStart.Minute, _DtStart.Second, ddayCalendar.TimeZones[0].TzId);
                            convertedEvent.DtEnd = new CalDateTime(_DtEnd.Year, _DtEnd.Month, _DtEnd.Day, _DtEnd.Hour, _DtEnd.Minute, _DtEnd.Second, ddayCalendar.TimeZones[0].TzId);
                        }
                        var alarm = convertedEvent.Alarms.FirstOrDefault();
                        if (alarm != null)
                        {
                            if (alarm.Trigger == null)
                            {
                                convertedEvent.Alarms.Clear();
                            }
                        }
                        else
                        {
                            convertedEvent.Alarms.Clear();
                        }

                        ddayCalendar.Events.Add(convertedEvent);
                    }
                }
                ddayCalendar.TimeZones[0].Children.Clear();
                result = DDayICalParser.SerializeCalendar(ddayCalendar);

                //for yyyyMMdd/P1D date. Bug in the ical.net
                result = Regex.Replace(result, @"(\w*EXDATE;VALUE=DATE:\d{8})(/\w*)", "$1");

                ExportDataCache.Insert(calendarId, result);

                stopWatch.Stop();
                var timeSpan = stopWatch.Elapsed;
                var elapsedTime = String.Format("GetCalendariCalString elapsedTime = {0:00}:{1:00}:{2:00}.{3:00}",
                                                timeSpan.Hours,
                                                timeSpan.Minutes,
                                                timeSpan.Seconds,
                                                timeSpan.Milliseconds / 10);

                Log.LogDebug(elapsedTime);

                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "GetCalendarUri");
                return null;
            }
        }

        private void CreateCaldavSharedEvents(
            string calendarId,
            string calendarIcs,
            Uri myUri,
            string currentUserEmail,
            BaseCalendar icalendar,
            Common.Security.Authentication.IAccount currentUser,
            int tenantId
            )
        {
            var parseCalendar = DDayICalParser.DeserializeCalendar(calendarIcs);
            var calendar = parseCalendar.FirstOrDefault();
            TenantManager.SetCurrentTenant(tenantId);
            try
            {
                if (calendar != null)
                {
                    var events = new List<CalendarEvent>();
                    var isFullAccess = false;
                    if (calendarId != BirthdayReminderCalendar.CalendarId && calendarId != "crm_calendar" && !calendarId.Contains("Project_"))
                    {
                        foreach (var e in icalendar.LoadEvents(currentUser.ID, DateTime.MinValue, DateTime.MaxValue))
                        {
                            events.Add(DDayICalParser.ConvertEvent(e as BaseEvent));
                        }
                    }
                    else
                    {
                        events.AddRange(calendar.Events);
                    }
                    foreach (var e in events)
                    {
                        Event evt = null;
                        evt = DataProvider.GetEventOnlyByUid(e.Uid);


                        isFullAccess = calendarId != BirthdayReminderCalendar.CalendarId && calendarId != "crm_calendar" ?
                                            evt != null ? PermissionContext.PermissionResolver.Check(currentUser, evt, null, CalendarAccessRights.FullAccessAction) : isFullAccess
                                            : isFullAccess;
                        var uid = e.Uid;
                        string[] split = uid.Split(new Char[] { '@' });
                        e.Uid = split[0];

                        if (calendarId == BirthdayReminderCalendar.CalendarId)
                        {
                            e.Created = null;
                            e.End = new CalDateTime(e.Start.AddDays(1));
                            var evtUid = split[0].Split(new Char[] { '_' });
                            e.Uid = evtUid[1];
                        }
                        else if (calendarId.Contains("Project_"))
                        {
                            e.Created = null;
                            e.End = new CalDateTime(e.End.AddDays(1));
                        }
                        else if (calendarId == "crm_calendar" || calendarId.Contains("Project_"))
                        {
                            e.Created = null;
                            e.Status = nameof(EventStatus.Confirmed);
                        }

                        calendar.Events.Clear();
                        calendar.Events.Add(e);
                        var ics = DDayICalParser.SerializeCalendar(calendar);

                        var eventUid = isFullAccess ? e.Uid + "_write" : e.Uid;
                        updateCaldavEvent(ics, eventUid, true, calendarId,
                                              myUri, currentUserEmail, DateTime.Now,
                                              calendar.TimeZones[0], icalendar.TimeZone, false, true);

                    }
                }
            }
            catch (Exception exception)
            {
                Log.LogError("ERROR. Create shared caldav events: " + exception.Message);
            }
        }
        private void updateCaldavEvent(
                            string ics,
                            string uid,
                            bool sendToRadicale,
                            string guid,
                            Uri myUri,
                            string userEmail,
                            DateTime updateDate = default(DateTime),
                            VTimeZone calendarVTimeZone = null,
                            TimeZoneInfo calendarTimeZone = null,
                            bool isDelete = false,
                            bool isShared = false
            )
        {
            if (sendToRadicale)
            {
                try
                {
                    if (guid != null && guid.Length != 0)
                    {

                        var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";
                        var caldavHost = myUri.Host;

                        Log.LogInformation("RADICALE REWRITE URL: " + myUri);

                        var currentUserName = userEmail.ToLower() + "@" + caldavHost;

                        int indexOfChar = ics.IndexOf("BEGIN:VTIMEZONE");
                        int indexOfCharEND = ics.IndexOf("END:VTIMEZONE");

                        if (indexOfChar != -1)
                        {
                            ics = ics.Remove(indexOfChar, indexOfCharEND + 14 - indexOfChar);
                            if (ics.IndexOf("BEGIN:VTIMEZONE") > -1)
                                updateCaldavEvent(ics, uid, true, guid, myUri, userEmail);
                        }

                        var requestUrl = calDavServerUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + guid + (isShared ? "-shared" : "") +
                                            "/" + HttpUtility.UrlEncode(uid) + ".ics";
                        if (calendarTimeZone != null && calendarVTimeZone != null)
                        {
                            var icsCalendars = DDayICalParser.DeserializeCalendar(ics);
                            var icsCalendar = icsCalendars == null ? null : icsCalendars.FirstOrDefault();
                            var icsEvents = icsCalendar == null ? null : icsCalendar.Events;
                            var icsEvent = icsEvents == null ? null : icsEvents.FirstOrDefault();
                            if (icsEvent != null && !icsEvent.IsAllDay)
                            {
                                var offset = updateDate != DateTime.MinValue ? calendarTimeZone.GetUtcOffset(updateDate) : calendarTimeZone.GetUtcOffset(icsEvent.DtStart.Value);

                                if (icsEvent.DtStart.TzId != calendarVTimeZone.TzId)
                                {
                                    var _DtStart = icsEvent.DtStart.Add(offset).Value;
                                    icsEvent.DtStart = new CalDateTime(_DtStart.Year, _DtStart.Month, _DtStart.Day, _DtStart.Hour, _DtStart.Minute, _DtStart.Second, calendarVTimeZone.TzId);

                                }
                                if (icsEvent.DtEnd.TzId != calendarVTimeZone.TzId)
                                {
                                    var _DtEnd = icsEvent.DtEnd.Add(offset).Value;
                                    icsEvent.DtEnd = new CalDateTime(_DtEnd.Year, _DtEnd.Month, _DtEnd.Day, _DtEnd.Hour, _DtEnd.Minute, _DtEnd.Second, calendarVTimeZone.TzId);
                                }

                                foreach (var periodList in icsEvent.ExceptionDates)
                                {
                                    periodList.Parameters.Add("TZID", calendarVTimeZone.TzId);
                                }

                            }
                            if (icsEvent != null)
                            {
                                icsEvent.Created = null;
                                if (!isDelete)
                                {
                                    icsEvent.ExceptionDates.Clear();
                                }
                                icsEvent.Uid = uid;
                            }

                            ics = DDayICalParser.SerializeCalendar(icsCalendar);
                        }


                        try
                        {
                            var request = new HttpRequestMessage();
                            request.RequestUri = new Uri(requestUrl);
                            request.Method = HttpMethod.Put;

                            var authorization = isShared ? DataProvider.GetSystemAuthorization() : DataProvider.GetUserAuthorization(userEmail);
                            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));

                            request.Content = new StringContent(ics, Encoding.UTF8, "text/calendar");

                            var httpClient = ClientFactory.CreateClient();
                            httpClient.Send(request);
                        }
                        catch (HttpRequestException ex)
                        {
                            if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Conflict)
                                Log.LogDebug("ERROR: " + ex.Message);
                            else
                                Log.LogError("ERROR: " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Log.LogError("ERROR: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("ERROR: " + ex.Message);
                }
            }
        }
        private void UpdateCalDavEvent(string change, Uri calDavUrl)
        {
            try
            {

                var serverCalDavUrl = new Uri(calDavUrl.Scheme + "://" + calDavUrl.Host + "/caldav");

                var eventURl = serverCalDavUrl + "/" + change;

                var changeData = change.Split('/');

                var caldavGuid = changeData[1];
                var eventGuid = changeData[2].Split('.')[0];

                var sharedPostfixIndex = caldavGuid.IndexOf("-shared");

                var calendarId = 0;
                var ownerId = new Guid();

                if (sharedPostfixIndex != -1)
                {
                    int ind = caldavGuid.Length;
                    caldavGuid = caldavGuid.Remove(sharedPostfixIndex, ind - sharedPostfixIndex);

                    var fullAccessPostfixIndex = eventGuid.IndexOf("_write");
                    if (fullAccessPostfixIndex != -1)
                    {
                        eventGuid = eventGuid.Remove(fullAccessPostfixIndex, eventGuid.Length - fullAccessPostfixIndex);
                    }
                }

                if (caldavGuid == BirthdayReminderCalendar.CalendarId ||
                    caldavGuid == SharedEventsCalendar.CalendarId ||
                    caldavGuid == "crm_calendar")
                {
                    var userName = changeData[0];
                    var userData = userName.Split('@');
                    var tenantName = userData[2];
                    try
                    {
                        var tenant = TenantManager.GetTenant(tenantName);
                        if (tenant != null)
                        {
                            var email = string.Join("@", userData[0], userData[1]);
                            TenantManager.SetCurrentTenant(tenant);
                            var user = UserManager.GetUserByEmail(email);

                            var extCalendar = CalendarManager.GetCalendarForUser(user.Id, caldavGuid, UserManager);
                            var events = extCalendar.LoadEvents(user.Id, DateTime.MinValue, DateTime.MaxValue);

                            string currentEventId =
                                (from e in events where e.Uid.Split('@')[0] == eventGuid select e.Id).FirstOrDefault();

                            if (currentEventId != null)
                            {
                                var evt = DataProvider.GetEventById(Convert.ToInt32(currentEventId));
                                calendarId = Convert.ToInt32(evt.CalendarId);
                                ownerId = Guid.Parse(evt.OwnerId.ToString());
                                SecurityContext.AuthenticateMeWithoutCookie(ownerId);
                            }
                            else
                            {
                                Log.LogError("ERROR: error update calDav event. get current event id");
                                return;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.LogError(exception, "UpdateCalDavEvent");
                        return;
                    }
                }
                else
                {
                    var calendar = DataProvider.GetCalendarIdByCaldavGuid(caldavGuid);

                    calendarId = Convert.ToInt32(calendar[0][0]);
                    ownerId = Guid.Parse(calendar[0][1].ToString());
                    TenantManager.SetCurrentTenant(Convert.ToInt32(calendar[0][2]));
                }

                var currentUserId = Guid.Empty;
                if (SecurityContext.IsAuthenticated)
                {
                    currentUserId = SecurityContext.CurrentAccount.ID;
                    SecurityContext.Logout();
                }
                try
                {
                    SecurityContext.AuthenticateMeWithoutCookie(ownerId);
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(eventURl);
                    request.Method = HttpMethod.Get;

                    var _email = UserManager.GetUsers(ownerId).Email;
                    var authorization = sharedPostfixIndex != -1 ? DataProvider.GetSystemAuthorization() : DataProvider.GetUserAuthorization(_email);
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/calendar")
                    {
                        CharSet = Encoding.UTF8.WebName
                    };

                    Log.LogInformation($"UpdateCalDavEvent eventURl: {eventURl}");

                    string ics = "";

                    var httpClient = ClientFactory.CreateClient();

                    using (var response = httpClient.Send(request))
                    using (var reader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        ics = reader.ReadToEnd();
                    }
                    Log.LogInformation($"UpdateCalDavEvent: {ics}");
                    var existEvent = DataProvider.GetEventIdByUid(eventGuid + "%", calendarId);
                    var existCalendar = DataProvider.GetCalendarById(calendarId);

                    var calendars = DDayICalParser.DeserializeCalendar(ics);
                    var _calendar = calendars == null ? null : calendars.FirstOrDefault();
                    var eventObj = _calendar == null || _calendar.Events == null ? null : _calendar.Events.FirstOrDefault();
                    if (eventObj != null && existCalendar.IsTodo == 0)
                    {
                        var name = eventObj.Summary;
                        var description = eventObj.Description ?? " ";

                        var alarm = eventObj.Alarms == null ? null : eventObj.Alarms.FirstOrDefault();
                        var alertType = EventAlertType.Default;
                        if (alarm != null)
                        {
                            if (alarm.Trigger.Duration != null)
                            {
                                var alarmMinutes = alarm.Trigger.Duration.Value.Minutes;
                                var alarmHours = alarm.Trigger.Duration.Value.Hours;
                                var alarmDays = alarm.Trigger.Duration.Value.Days;
                                switch (alarmMinutes)
                                {
                                    case -5:
                                        alertType = EventAlertType.FiveMinutes;
                                        break;
                                    case -15:
                                        alertType = EventAlertType.FifteenMinutes;
                                        break;
                                    case -30:
                                        alertType = EventAlertType.HalfHour;
                                        break;
                                }
                                switch (alarmHours)
                                {
                                    case -1:
                                        alertType = EventAlertType.Hour;
                                        break;
                                    case -2:
                                        alertType = EventAlertType.TwoHours;
                                        break;
                                }
                                if (alarmDays == -1)
                                    alertType = EventAlertType.Day;
                            }
                        }

                        var utcStartDate = eventObj.IsAllDay ? eventObj.Start.Value : DDayICalParser.ToUtc(eventObj.Start);
                        var utcEndDate = eventObj.IsAllDay ? eventObj.End.Value : DDayICalParser.ToUtc(eventObj.End);

                        if (existEvent != null && existCalendar != null && !eventObj.IsAllDay)
                        {
                            var offset = existCalendar.TimeZone.GetUtcOffset(existEvent.UtcUpdateDate);
                            if (!eventObj.End.IsUtc && !eventObj.Start.IsUtc)
                            {
                                utcStartDate = eventObj.Start.Subtract(offset).Value;
                                utcEndDate = eventObj.End.Subtract(offset).Value;
                            }
                            else
                            {
                                var createOffset = existCalendar.TimeZone.GetUtcOffset(eventObj.Created.Value);
                                var startOffset = existCalendar.TimeZone.GetUtcOffset(eventObj.Start.Value);
                                var endOffset = existCalendar.TimeZone.GetUtcOffset(eventObj.End.Value);

                                if (createOffset != startOffset)
                                {
                                    var _utcStartDate = eventObj.Start.Subtract(createOffset).Add(startOffset).Value;
                                    utcStartDate = _utcStartDate;
                                }
                                if (createOffset != endOffset)
                                {
                                    var _utcEndDate = eventObj.End.Subtract(createOffset).Add(endOffset).Value;
                                    utcEndDate = _utcEndDate;
                                }
                            }
                        }


                        bool isAllDayLong = eventObj.IsAllDay;

                        var rrule = RecurrenceRule.Parse(GetRRuleString(eventObj));
                        var status = DDayICalParser.ConvertEventStatus(eventObj.Status);

                        if (existEvent != null)
                        {
                            var eventId = int.Parse(existEvent.Id);

                            var cal = new Ical.Net.Calendar();

                            var permissions = PublicItemCollectionHelper.GetForEvent(existEvent);
                            var sharingOptions = permissions.Items
                                .Where(x => x.SharingOption.Id != AccessOption.OwnerOption.Id)
                                .Select(x => new SharingParam
                                {
                                    Id = x.Id,
                                    actionId = x.SharingOption.Id,
                                    isGroup = x.IsGroup
                                }).ToList();
                            eventObj.Start = new CalDateTime(DateTime.SpecifyKind(utcStartDate, DateTimeKind.Utc), TimeZoneInfo.Utc.Id);
                            eventObj.End = new CalDateTime(DateTime.SpecifyKind(utcEndDate, DateTimeKind.Utc), TimeZoneInfo.Utc.Id);
                            eventObj.Created = new CalDateTime(DateTime.SpecifyKind(eventObj.Created != null ? eventObj.Created.Value : DateTime.Now, DateTimeKind.Utc), TimeZoneInfo.Utc.Id);


                            cal.Events.Add(eventObj);
                            var eventModel = new EventModel
                            {
                                EventId = eventId,
                                CalendarId = calendarId.ToString(),
                                Ics = DDayICalParser.SerializeCalendar(cal),
                                AlertType = alertType,
                                SharingOptions = sharingOptions,
                                FromCalDavServer = true,
                                OwnerId = ownerId.ToString()
                            };
                            UpdateEvent(eventModel);
                        }
                        else
                        {
                            var eventModel = new EventModel
                            {
                                Ics = ics,
                                AlertType = alertType,
                                SharingOptions = null,
                                EventUid = null
                            };
                            AddEvent(calendarId, eventModel);
                        }
                    }
                    var todoObj = _calendar == null || _calendar.Todos == null ? null : _calendar.Todos.FirstOrDefault();
                    if (todoObj != null && existCalendar.IsTodo == 1)
                    {
                        var todoName = todoObj.Summary;
                        var todoDescription = todoObj.Description ?? " ";
                        var todoUtcStartDate = todoObj.Start != null ? DDayICalParser.ToUtc(todoObj.Start) : DateTime.MinValue;
                        var todoCompleted = todoObj.Completed != null ? DDayICalParser.ToUtc(todoObj.Completed) : DateTime.MinValue;

                        var existTodo = DataProvider.GetTodoIdByUid(eventGuid + "%", calendarId);

                        if (existTodo != null)
                        {
                            var todoId = int.Parse(existTodo.Id);


                            UpdateTodo(
                                calendarId,
                                todoName,
                                todoDescription,
                                todoUtcStartDate,
                                existTodo.Uid,
                                todoCompleted);
                        }
                        else
                        {
                            CreateTodo(calendarId,
                                        todoName,
                                        todoDescription,
                                        todoUtcStartDate,
                                        eventGuid,
                                        todoCompleted);
                        }
                    }
                }
                finally
                {
                    SecurityContext.Logout();
                    if (currentUserId != Guid.Empty)
                    {
                        SecurityContext.AuthenticateMeWithoutCookie(currentUserId);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Conflict)
                    Log.LogDebug("ERROR: " + ex.Message);
                else
                    Log.LogError("ERROR: " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "UpdateCalDavEvent");
            }
        }
        private void DeleteCalDavEvent(string eventInfo, Uri myUri)
        {
            Thread.Sleep(1000);
            var caldavGuid = eventInfo.Split('/')[1].Replace("-shared", "");
            var calEvent = eventInfo.Split('/')[2].Replace("_write", ""); ;
            var eventGuid = calEvent.Split('.')[0];

            if (caldavGuid != SharedEventsCalendar.CalendarId)
            {
                var calendar = DataProvider.GetCalendarIdByCaldavGuid(caldavGuid);

                var calendarId = Convert.ToInt32(calendar[0][0]);
                var ownerId = Guid.Parse(calendar[0][1].ToString());


                TenantManager.SetCurrentTenant(Convert.ToInt32(calendar[0][2]));
                SecurityContext.AuthenticateMeWithoutCookie(ownerId);

                var existEvent = DataProvider.GetEventIdByUid(eventGuid + "%", calendarId);
                if (existEvent != null)
                {
                    var eventDeleteModel = new EventDeleteModel
                    {
                        Date = null,
                        Type = EventRemoveType.AllSeries,
                        Uri = myUri,
                        FromCaldavServer = true
                    };
                    RemoveEvent(Convert.ToInt32(existEvent.Id), eventDeleteModel);
                }
                else
                {
                    var existTodo = DataProvider.GetTodoByUid(eventGuid + "%");
                    if (existTodo != null)
                    {
                        var todoModel = new CreateTodoModel
                        {
                            fromCalDavServer = true
                        };
                        RemoveTodo(Convert.ToInt32(existTodo.Id), todoModel);
                    }
                }
            }
            else
            {
                var existEvent = DataProvider.GetEventIdOnlyByUid(eventGuid + "%");
                if (existEvent != null)
                {
                    TenantManager.SetCurrentTenant(existEvent.TenantId);
                    SecurityContext.AuthenticateMeWithoutCookie(existEvent.OwnerId);
                    var eventDeleteModel = new EventDeleteModel
                    {
                        Date = null,
                        Type = EventRemoveType.AllSeries,
                        Uri = myUri,
                        FromCaldavServer = true
                    };
                    RemoveEvent(Convert.ToInt32(existEvent.Id), eventDeleteModel);
                }
            }
        }
        private void CreateCaldavEvents(
            string calDavGuid,
            Uri myUri,
            string currentUserEmail,
            BaseCalendar icalendar,
            string calendarIcs,
            int tenantId
            )
        {
            var parseCalendar = DDayICalParser.DeserializeCalendar(calendarIcs);
            var calendar = parseCalendar.FirstOrDefault();
            TenantManager.SetCurrentTenant(tenantId);

            var calendarId = icalendar.Id;
            var ddayCalendar = new Ical.Net.Calendar();
            try
            {
                if (calendar != null)
                {
                    var events = calendar.Events;
                    foreach (var evt in events)
                    {
                        var uid = evt.Uid;
                        string[] split = uid.Split(new Char[] { '@' });
                        ddayCalendar = DDayICalParser.ConvertCalendar(icalendar);
                        ddayCalendar.Events.Clear();
                        ddayCalendar.Events.Add(evt);

                        var ics = DDayICalParser.SerializeCalendar(ddayCalendar);
                        updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail,
                                          DateTime.Now, ddayCalendar.TimeZones[0],
                                          icalendar.TimeZone);
                    }

                    var todos = icalendar.GetTodoWrappers(SecurityContext.CurrentAccount.ID, new ApiDateTime(DateTime.MinValue, icalendar.TimeZone.GetOffset()), new ApiDateTime(DateTime.MaxValue, icalendar.TimeZone.GetOffset()), TodoWrapperHelper);
                    foreach (var td in todos)
                    {
                        ddayCalendar = DDayICalParser.ConvertCalendar(icalendar);
                        ddayCalendar.Todos.Clear();

                        var todo = new Ical.Net.CalendarComponents.Todo
                        {
                            Summary = td.Name,
                            Description = td.Description,
                            Start = td.Start != DateTime.MinValue ? new CalDateTime(td.Start) : null,
                            Completed = td.Completed != DateTime.MinValue ? new CalDateTime(td.Completed) : null,
                        };

                        ddayCalendar.Todos.Add(todo);

                        var ics = DDayICalParser.SerializeCalendar(ddayCalendar);
                        var uid = td.Uid;
                        string[] split = uid.Split(new Char[] { '@' });
                        updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail, DateTime.Now, ddayCalendar.TimeZones[0], icalendar.TimeZone);
                    }
                }

            }
            catch (Exception exception)
            {
                Log.LogError("ERROR. Create caldav events: " + exception.Message);
            }

        }
        private string UpdateCalDavCalendar(string name, string description, string backgroundColor, string calDavGuid, Uri myUri, string email, bool isSharedCalendar = false)
        {

            var currentUserName = email.ToLower() + "@" + myUri.Host;
            var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";

            var requestUrl = calDavServerUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + calDavGuid + (isSharedCalendar ? "-shared" : "");


            name = (name ?? "").Trim();
            if (String.IsNullOrEmpty(name))
                throw new Exception(Resources.CalendarApiResource.ErrorEmptyName);

            description = (description ?? "").Trim();
            backgroundColor = (backgroundColor ?? "").Trim();

            Log.LogInformation("RADICALE REWRITE URL: " + myUri);

            string[] numbers = Regex.Split(backgroundColor, @"\D+");
            var color = numbers.Length > 4 ? HexFromRGB(int.Parse(numbers[1]), int.Parse(numbers[2]), int.Parse(numbers[3])) : "#000000";


            var data = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                        "<propertyupdate xmlns=\"DAV:\" xmlns:C=\"urn:ietf:params:xml:ns:caldav\" xmlns:CR=\"urn:ietf:params:xml:ns:carddav\" xmlns:I=\"http://apple.com/ns/ical/\" xmlns:INF=\"http://inf-it.com/ns/ab/\">" +
                        "<set><prop>" +
                        "<C:supported-calendar-component-set><C:comp name=\"VEVENT\" /><C:comp name=\"VJOURNAL\" /><C:comp name=\"VTODO\" />" +
                        "</C:supported-calendar-component-set><displayname>" + name + "</displayname>" +
                        "<I:calendar-color>" + color + "</I:calendar-color>" +
                        "<C:calendar-description>" + description + "</C:calendar-description></prop></set><remove><prop>" +
                        "<INF:addressbook-color /><CR:addressbook-description /></prop></remove></propertyupdate>";

            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(requestUrl);
                request.Method = new HttpMethod("PROPPATCH");

                var authorization = isSharedCalendar ? DataProvider.GetSystemAuthorization() : DataProvider.GetUserAuthorization(email);
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));
                request.Content = new StringContent(data, Encoding.UTF8, "text/calendar");

                var httpClient = ClientFactory.CreateClient();
                var response = httpClient.Send(request);

                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    return requestUrl;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "UpdateCalDavCalendar");
                return "";
            }
        }
        private void UpdateSharedCalDavCalendar(string name, string description, string backgroundColor, string calDavGuid, Uri myUri, List<SharingParam> sharingOptionsList, List<IEvent> events, string calendarId, string calendarGuid, int tenantId, DateTime updateDate = default(DateTime),
                            VTimeZone calendarVTimeZone = null,
                            TimeZoneInfo calendarTimeZone = null)
        {
            try
            {
                TenantManager.SetCurrentTenant(tenantId);

                var calendarIcs = GetCalendariCalString(calendarId);
                var parseCalendar = DDayICalParser.DeserializeCalendar(calendarIcs);
                var calendar = parseCalendar.FirstOrDefault();


                foreach (var sharingParam in sharingOptionsList)
                {
                    var fullAccess = sharingParam.actionId == AccessOption.FullAccessOption.Id ||
                                                 sharingParam.actionId == AccessOption.OwnerOption.Id;
                    if (sharingParam.isGroup)
                    {
                        var users = UserManager.GetUsersByGroup(sharingParam.itemId);

                        foreach (var userGroup in users)
                        {
                            UpdateCalDavCalendar(name, description, backgroundColor, calDavGuid, myUri, userGroup.Email.ToLower(), true);

                            foreach (var e in events)
                            {

                                var evt = DDayICalParser.ConvertEvent(e as BaseEvent);

                                var uid = evt.Uid;
                                string[] split = uid.Split(new Char[] { '@' });
                                evt.Uid = split[0];

                                calendar.Events.Clear();
                                calendar.Events.Add(evt);

                                var ics = DDayICalParser.SerializeCalendar(calendar);

                                UpdateSharedEvent(userGroup, evt.Uid, fullAccess, myUri, ics, calendarGuid, updateDate, calendarVTimeZone, calendarTimeZone);
                            }
                        }
                    }
                    else
                    {
                        var user = UserManager.GetUsers(sharingParam.itemId);
                        UpdateCalDavCalendar(name, description, backgroundColor, calDavGuid, myUri, user.Email.ToLower(), true);

                        foreach (var sharedEvent in events)
                        {
                            var evt = DDayICalParser.ConvertEvent(sharedEvent as BaseEvent);

                            var uid = evt.Uid;
                            string[] split = uid.Split(new Char[] { '@' });
                            evt.Uid = split[0];

                            calendar.Events.Clear();
                            calendar.Events.Add(evt);

                            var ics = DDayICalParser.SerializeCalendar(calendar);

                            UpdateSharedEvent(user, evt.Uid, fullAccess, myUri, ics, calendarGuid, updateDate, calendarVTimeZone, calendarTimeZone);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("ERROR: " + ex.Message);
            }

        }
        private void ReplaceSharingEvent(
                            ASC.Core.Users.UserInfo user,
                            string actionId,
                            string guid,
                            Uri myUri,
                            string oldIcs,
                            string calendarId,
                            DateTime updateDate = default(DateTime),
                            VTimeZone calendarVTimeZone = null,
                            TimeZoneInfo calendarTimeZone = null,
                            string calGuid = null)
        {
            if (calGuid.Length != 0 && myUri != null && user != null)
            {
                string eventUid = guid,
                    oldEventUid = guid;
                if (actionId == AccessOption.FullAccessOption.Id)
                {
                    eventUid = guid + "_write";
                    oldEventUid = guid;
                }
                else if (actionId != AccessOption.OwnerOption.Id)
                {
                    oldEventUid = guid + "_write";
                    eventUid = guid;
                }

                var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";
                var caldavHost = myUri.Host;

                Log.LogInformation("RADICALE REWRITE URL: " + myUri);

                var encoded =
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes("admin@ascsystem" + ":" +
                                                ASC.Core.Configuration.Constants.CoreSystem.ID));

                var currentUserName = user.Email.ToLower() + "@" + caldavHost;

                var requestDeleteUrl = calDavServerUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" +
                    (calGuid ?? SharedEventsCalendar.CalendarId) + (actionId != AccessOption.OwnerOption.Id ? "-shared" : "") + "/" + oldEventUid +
                                        ".ics";

                try
                {
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(requestDeleteUrl);
                    request.Method = HttpMethod.Delete;
                    request.Headers.Add("Authorization", "Basic " + encoded);

                    var httpClient = ClientFactory.CreateClient();
                    httpClient.Send(request);
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Conflict)
                        Log.LogDebug("ERROR: " + ex.Message);
                    else
                        Log.LogError("ERROR: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Log.LogError("ERROR: " + ex.Message);
                }
                finally
                {
                    updateCaldavEvent(oldIcs, eventUid, true,
                                        (calGuid ?? SharedEventsCalendar.CalendarId), myUri,
                                        user.Email, updateDate, calendarVTimeZone, calendarTimeZone, false, actionId != AccessOption.OwnerOption.Id);
                }
            }

        }
        private void UpdateSharedEvent(UserInfo userSharingInfo, string guid, bool fullAccess,
                            Uri myUri,
                            string oldIcs,
                            string calendarId,
                            DateTime updateDate = default(DateTime),
                            VTimeZone calendarVTimeZone = null,
                            TimeZoneInfo calendarTimeZone = null)
        {
            string eventUid = guid,
                   oldEventUid = guid;

            if (fullAccess)
            {
                eventUid = guid + "_write";
                oldEventUid = guid;
            }
            else
            {
                oldEventUid = guid + "_write";
                eventUid = guid;
            }

            var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";
            var caldavHost = myUri.Host;

            Log.LogInformation("RADICALE REWRITE URL: " + myUri);

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin@ascsystem" + ":" + ASC.Core.Configuration.Constants.CoreSystem.ID));

            var currentUserName = userSharingInfo.Email.ToLower() + "@" + caldavHost;

            var requestDeleteUrl = calDavServerUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + calendarId + "-shared" + "/" + oldEventUid + ".ics";

            updatedEvents.Add(guid);

            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(requestDeleteUrl);
                request.Method = HttpMethod.Delete;
                request.Headers.Add("Authorization", "Basic " + encoded);

                var httpClient = ClientFactory.CreateClient();
                httpClient.Send(request);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Conflict)
                    Log.LogDebug("ERROR: " + ex.Message);
                else
                    Log.LogError("ERROR: " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "UpdateSharedEvent");
            }
            finally
            {
                updateCaldavEvent(oldIcs, eventUid, true, calendarId, myUri,
                                  userSharingInfo.Email, updateDate, calendarVTimeZone, calendarTimeZone, false, true);
            }
        }
    }
}