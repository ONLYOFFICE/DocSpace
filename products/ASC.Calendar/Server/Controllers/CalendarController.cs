

using System.Collections.Generic;

using ASC.Api.Core;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Reassigns;
using ASC.FederatedLogin;
using ASC.MessagingSystem;
using ASC.Calendar.Models;
using ASC.Security.Cryptography;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SecurityContext = ASC.Core.SecurityContext;
using ASC.Calendar.Core;
using ASC.Calendar.Core.Dao;
using ASC.Calendar.BusinessObjects;
using ASC.Web.Core.Calendars;
using ASC.Calendar.ExternalCalendars;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using System.Web;
using ASC.Calendar.Notification;
using ASC.Common;
using System.Net;
using System.IO;
using ASC.Calendar.iCalParser;
using ASC.Common.Security;
using System.Globalization;

namespace ASC.Calendar.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private const int _monthCount = 3;
        public Tenant Tenant { get { return ApiContext.Tenant; } }
        public ApiContext ApiContext { get; }
        public AuthContext AuthContext { get; }
        public UserManager UserManager { get; }
        public DataProvider DataProvider { get; }
        public ILog Log { get; }
        private TenantManager TenantManager { get; }
        public TimeZoneConverter TimeZoneConverter { get; }
        public CalendarWrapperHelper CalendarWrapperHelper { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private AuthorizationManager AuthorizationManager { get; }
        private AuthManager Authentication { get; }
        private CalendarNotifyClient CalendarNotifyClient { get; }
        public DDayICalParser DDayICalParser { get; }
        public HttpContext HttpContext { get; set; }
        public PermissionContext PermissionContext { get; }
        public EventHistoryWrapperHelper EventHistoryWrapperHelper { get; }
        public EventWrapperHelper EventWrapperHelper { get; }
        public EventHistoryHelper EventHistoryHelper { get; }
        public PublicItemCollectionHelper PublicItemCollectionHelper { get; }


        public CalendarController(

            ApiContext apiContext,
            AuthContext authContext,
            AuthorizationManager authorizationManager,
            UserManager userManager,
            TenantManager tenantManager,
            TimeZoneConverter timeZoneConverter,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            IOptionsMonitor<ILog> option,
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
            PublicItemCollectionHelper publicItemCollectionHelper)
        {
            AuthContext = authContext;
            Authentication = authentication;
            AuthorizationManager = authorizationManager;
            TenantManager = tenantManager;
            Log = option.Get("ASC.Api");
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


            CalendarManager.Instance.RegistryCalendar(new SharedEventsCalendar(AuthContext, TimeZoneConverter, TenantManager));
            var birthdayReminderCalendar = new BirthdayReminderCalendar(AuthContext, TimeZoneConverter, UserManager, DisplayUserSettingsHelper);
            if (UserManager.IsUserInGroup(AuthContext.CurrentAccount.ID, Constants.GroupVisitor.ID))
            {
                CalendarManager.Instance.UnRegistryCalendar(birthdayReminderCalendar.Id);
            }
            else
            {
                CalendarManager.Instance.RegistryCalendar(birthdayReminderCalendar);
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

        [Read("info")]
        public Module GetModule()
        {
            var product = new CalendarProduct();
            product.Init();
            return new Module(product, true);
        }

        [Read("{calendarId}")]
        public CalendarWrapper GetCalendarById(string calendarId)
        {
            int calId;
            if (int.TryParse(calendarId, out calId))
            {
                var calendars = DataProvider.GetCalendarById(calId);

                return (calendars != null ? CalendarWrapperHelper.Get(calendars) : null);
            }

            var extCalendar = CalendarManager.Instance.GetCalendarForUser(AuthContext.CurrentAccount.ID, calendarId, UserManager);
            if (extCalendar != null)
            {
                var viewSettings = DataProvider.GetUserViewSettings(AuthContext.CurrentAccount.ID, new List<string> { calendarId });
                return CalendarWrapperHelper.Get(extCalendar, viewSettings.FirstOrDefault());
            }
            return null;
        }

        [Create]
        public CalendarWrapper CreateCalendar(CalendarModel calendar)
        {
            var sharingOptionsList = calendar.sharingOptions ?? new List<SharingParam>();
            var timeZoneInfo = TimeZoneConverter.GetTimeZone(calendar.TimeZone);

            calendar.Name = (calendar.Name ?? "").Trim();
            if (String.IsNullOrEmpty(calendar.Name))
                throw new Exception(Resources.CalendarApiResource.ErrorEmptyName);

            calendar.Description = (calendar.Description ?? "").Trim();
            calendar.TextColor = (calendar.TextColor ?? "").Trim();
            calendar.BackgroundColor = (calendar.BackgroundColor ?? "").Trim();

            Guid calDavGuid = Guid.NewGuid();

            var myUri = HttpContext.Request.GetUrlRewriter();

            var _email = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email;
            var currentUserName = _email.ToLower() + "@" + myUri.Host;

            string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, UserManager.GetUserByEmail(_email).ID);

            //TODO caldav

            /*var caldavTask = new Task(() => CreateCalDavCalendar(name, description, backgroundColor, calDavGuid.ToString(), myUri, currentUserName, _email, currentAccountPaswd));
            caldavTask.Start();*/

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
            if (!string.IsNullOrEmpty(calendar.iCalUrl))
            {
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(calendar.iCalUrl);
                    using (var resp = req.GetResponse())
                    using (var stream = resp.GetResponseStream())
                    {
                        var ms = new MemoryStream();
                        stream.StreamCopyTo(ms);
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
                    Log.Info(String.Format("Error import events to new calendar by ical url: {0}", ex.Message));
                }

            }

            return CalendarWrapperHelper.Get(cal);
        }

        [Read("events/{eventUid}/historybyuid")]
        public EventHistoryWrapper GetEventHistoryByUid(string eventUid)
        {
            if (string.IsNullOrEmpty(eventUid))
            {
                throw new ArgumentException("eventUid");
            }

            var evt = DataProvider.GetEventByUid(eventUid);

            return GetEventHistoryWrapper(evt);
        }

        [Create("icsevent")]
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
                    throw new Exception(string.Format("Can't parse {0} to int", defaultCalendar.Id));
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

            eventUid = eventUid == null ? null : string.Format("{0}@onlyoffice.com", eventUid);

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

            //TODO caldav
            /*try
            {
                var uid = evt.Uid;
                string[] split = uid.Split(new Char[] { '@' });

                var calDavGuid = calendarObj != null ? calendarObj.calDavGuid : "";
                var myUri = HttpContext.Current.Request.GetUrlRewriter();
                var userId = SecurityContext.CurrentAccount.ID;
                var currentUserEmail = CoreContext.UserManager.GetUsers(userId).Email.ToLower();
                string currentAccountPaswd = CoreContext.Authentication.GetUserPasswordHash(userId);

                var currentEventUid = split[0];

                var pic = PublicItemCollection.GetForCalendar(calendarObj);
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
                var currentTenantId = TenantProvider.CurrentTenantID;

                if (!calendarObj.OwnerId.Equals(SecurityContext.CurrentAccount.ID) && CheckPermissions(calendarObj, CalendarAccessRights.FullAccessAction, true))
                {
                    currentEventUid = currentEventUid + "_write";
                }
                /*var updateCaldavThread = new Thread(() =>
                {

                    updateCaldavEvent(old_ics, currentEventUid, true, calDavGuid, myUri,
                                      currentUserEmail, currentAccountPaswd, DateTime.Now,
                                      targetCalendar.TimeZones[0], calendarObj.TimeZone, false, userId != calendarObj.OwnerId);

                    CoreContext.TenantManager.SetCurrentTenant(currentTenantId);
                    //calendar sharing list
                    foreach (var sharingOption in sharingList)
                    {
                        var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                        if (!sharingOption.IsGroup)
                        {
                            var user = CoreContext.UserManager.GetUsers(sharingOption.itemId);
                            var userPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);


                            var sharedEventUid = user.ID == calendarObj.OwnerId
                                                     ? split[0]
                                                     : fullAccess ? split[0] + "_write" : split[0];

                            updateCaldavEvent(old_ics, sharedEventUid, true, calDavGuid, myUri,
                                              user.Email, userPaswd, DateTime.Now, targetCalendar.TimeZones[0],
                                              calendarObj.TimeZone, false, user.ID != calendarObj.OwnerId);

                        }
                        else
                        {
                            var users = CoreContext.UserManager.GetUsersByGroup(sharingOption.itemId);

                            foreach (var user in users)
                            {
                                var sharedEventUid = user.ID == calendarObj.OwnerId
                                                     ? split[0]
                                                     : fullAccess ? split[0] + "_write" : split[0];
                                var userPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);

                                updateCaldavEvent(old_ics, sharedEventUid, true, calDavGuid, myUri,
                                              user.Email, userPaswd, DateTime.Now, targetCalendar.TimeZones[0],
                                              calendarObj.TimeZone, false, user.ID != calendarObj.OwnerId);

                            }
                        }
                    }
                    //event sharing list
                    foreach (var sharingOption in sharingOptions)
                    {
                        var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                        if (!sharingOption.IsGroup)
                        {
                            var user = CoreContext.UserManager.GetUsers(sharingOption.itemId);
                            var userPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);


                            var sharedEventUid = user.ID == calendarObj.OwnerId
                                                     ? split[0]
                                                     : fullAccess ? split[0] + "_write" : split[0];

                            updateCaldavEvent(old_ics, sharedEventUid, true, SharedEventsCalendar.CalendarId, myUri,
                                              user.Email, userPaswd, DateTime.Now, targetCalendar.TimeZones[0],
                                              calendarObj.TimeZone, false, user.ID != calendarObj.OwnerId);

                        }
                        else
                        {
                            var users = CoreContext.UserManager.GetUsersByGroup(sharingOption.itemId);

                            foreach (var user in users)
                            {
                                var sharedEventUid = user.ID == calendarObj.OwnerId
                                                     ? split[0]
                                                     : fullAccess ? split[0] + "_write" : split[0];
                                var userPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);

                                updateCaldavEvent(old_ics, sharedEventUid, true, SharedEventsCalendar.CalendarId, myUri,
                                              user.Email, userPaswd, DateTime.Now, targetCalendar.TimeZones[0],
                                              calendarObj.TimeZone, false, user.ID != calendarObj.OwnerId);

                            }
                        }
                    }

                });
                updateCaldavThread.Start(); 
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }*/


            DataProvider.AddEventHistory(calId, evt.Uid, int.Parse(evt.Id), ics);

            return result;
        }

        [Update("icsevent")]
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
                    string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

                    var isFullAccess = PermissionContext.PermissionResolver.Check(AuthContext.CurrentAccount, (ISecurityObject)evt, null,
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

                    var currentTenantId = TenantManager.GetCurrentTenant().TenantId;
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
                    //TODO caldav
                    /*
                    var sharingEventThread = new Thread(() =>
                    {
                        CoreContext.TenantManager.SetCurrentTenant(currentTenantId);
                        //event sharing ptions
                        foreach (var sharingOption in sharingOptions)
                        {
                            if (!sharingOption.IsGroup)
                            {
                                var user = CoreContext.UserManager.GetUsers(sharingOption.itemId);
                                ReplaceSharingEvent(user, sharingOption.actionId, uidData[0], myUri, old_ics,
                                                    calendarId, createDate, targetCalendar.TimeZones[0],
                                                    calendarObj.TimeZone);
                            }
                            else
                            {
                                var users = CoreContext.UserManager.GetUsersByGroup(sharingOption.itemId);
                                foreach (var user in users)
                                {
                                    ReplaceSharingEvent(user, sharingOption.actionId, uidData[0], myUri, old_ics,
                                                    calendarId, createDate, targetCalendar.TimeZones[0],
                                                    calendarObj.TimeZone);
                                }
                            }
                        }
                        //calendar sharing ptions
                        foreach (var sharingOption in calendarCharingList)
                        {
                            if (!sharingOption.IsGroup)
                            {
                                var user = CoreContext.UserManager.GetUsers(sharingOption.itemId);
                                ReplaceSharingEvent(user, sharingOption.actionId, uidData[0], myUri, old_ics,
                                                    calendarId, createDate, targetCalendar.TimeZones[0],
                                                    calendarObj.TimeZone, cal.calDavGuid);
                            }
                            else
                            {
                                var users = CoreContext.UserManager.GetUsersByGroup(sharingOption.itemId);
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
                            updateCaldavEvent(old_ics, eventUid, true, calDavGuid, myUri, currentUserEmail, currentAccountPaswd, createDate, targetCalendar.TimeZones[0], calendarObj.TimeZone, false, isShared);
                        }
                        else
                        {
                            var owner = CoreContext.UserManager.GetUsers(evt.OwnerId);
                            var ownerPaswd = CoreContext.Authentication.GetUserPasswordHash(evt.OwnerId);
                            updateCaldavEvent(old_ics, uidData[0], true, calendarObj.calDavGuid, myUri, owner.Email, ownerPaswd, createDate, targetCalendar.TimeZones[0], calendarObj.TimeZone, false, false);
                        }

                    });
                    sharingEventThread.Start();
                    */

                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
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

        private List<CalendarWrapper> LoadInternalCalendars()
        {
            var result = new List<CalendarWrapper>();
            int newCalendarsCount;
            //internal
            var calendars = DataProvider.LoadCalendarsForUser(AuthContext.CurrentAccount.ID, out newCalendarsCount);

            var userTimeZone = TimeZoneConverter.GetTimeZone(TenantManager.GetCurrentTenant().TimeZone);

            result.AddRange(calendars.ConvertAll(c => CalendarWrapperHelper.Get(c)));
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
                //TODO
                //canNotify = CheckIsOrganizer(history);
            }

            return EventHistoryWrapperHelper.Get(history, canEdit, canNotify, cal, fullHistory);
        }
        private bool CheckIsOrganizer(EventHistory history)
        {
            //TODO
            /*  var canNotify = false;

              var apiServer = new ApiServer();
              var apiResponse = apiServer.GetApiResponse(String.Format("{0}mail/accounts.json", SetupInfo.WebApiBaseUrl), "GET");
              var obj = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(apiResponse)));

              if (obj["response"] != null)
              {
                  var accounts = (from account in JArray.Parse(obj["response"].ToString())
                                  let email = account.Value<String>("email")
                                  let enabled = account.Value<Boolean>("enabled")
                                  let isGroup = account.Value<Boolean>("isGroup")
                                  where enabled && !isGroup
                                  select email).ToList();

                  if (accounts.Any())
                  {
                      var mergedHistory = history.GetMerged();
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

              return canNotify;*/
            return false;
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
                            string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

                            //TODO caldav
                            /*var updateCaldavThread = new Thread(() => updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail, currentAccountPaswd, DateTime.Now, tmpCalendar.TimeZones[0], existCalendar.TimeZone));
                             updateCaldavThread.Start();*/
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }

                        /*var result = CreateEvent(calendarId,
                                                 eventObj.Summary,
                                                 eventObj.Description,
                                                 utcStartDate,
                                                 utcEndDate,
                                                 RecurrenceRule.Parse(rrule),
                                                 EventAlertType.Default,
                                                 eventObj.IsAllDay,
                                                 null,
                                                 eventObj.Uid,
                                                 calendar.Method == Ical.Net.CalendarMethods.Cancel ? EventStatus.Cancelled : DDayICalParser.ConvertEventStatus(eventObj.Status), eventObj.Created != null ? eventObj.Created.Value : DateTime.Now);*/

                        // var eventId = result != null && result.Any() ? Int32.Parse(result.First().Id) : 0;

                        //if (eventId > 0)
                        // {
                        //DataProvider.AddEventHistory(calendarId, eventObj.Uid, eventId, ics);
                        //  counter++;
                        // }
                    }
                    else
                    {
                        /* if (eventHistory.Contains(tmpCalendar)) continue;

                         eventHistory = _dataProvider.AddEventHistory(eventHistory.CalendarId, eventHistory.EventUid,
                                                                      eventHistory.EventId, ics);

                         var mergedCalendar = eventHistory.GetMerged();

                         if (mergedCalendar == null || mergedCalendar.Events == null || !mergedCalendar.Events.Any()) continue;

                         var mergedEvent = mergedCalendar.Events.First();

                         rrule = GetRRuleString(mergedEvent);

                         var utcStartDate = mergedEvent.IsAllDay ? mergedEvent.Start.Value : DDayICalParser.ToUtc(mergedEvent.Start);
                         var utcEndDate = mergedEvent.IsAllDay ? mergedEvent.End.Value : DDayICalParser.ToUtc(mergedEvent.End);

                         var existCalendar = _dataProvider.GetCalendarById(calendarId);
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
                         var permissions = PublicItemCollection.GetForEvent(targetEvent);
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
                             var myUri = HttpContext.Current.Request.GetUrlRewriter();
                             var currentUserEmail = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email.ToLower();
                             string currentAccountPaswd = CoreContext.Authentication.GetUserPasswordHash(SecurityContext.CurrentAccount.ID);

                            //TODO caldav
                            // var updateCaldavThread = new Thread(() => updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail, currentAccountPaswd, DateTime.Now, tmpCalendar.TimeZones[0], existCalendar.TimeZone));
                            // updateCaldavThread.Start();
                         }
                         catch (Exception e)
                         {
                             Log.Error(e.Message);
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

                         counter++;*/
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
                        if (date.ToLowerInvariant().IndexOf('t') >= 0)
                        {
                            //is utc time
                            if (date.ToLowerInvariant().IndexOf('z') >= 0)
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
                        else if (date.ToLowerInvariant().IndexOf("/p") >= 0)
                        {
                            try
                            {
                                rrule += date.Split('/')[0];
                            }
                            catch (Exception ex)
                            {
                                Log.Error(String.Format("Error: {0}, Date string: {1}", ex, date));
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
    }

    public static class CalendarControllerExtention
    {
        public static DIHelper AddCalendarController(this DIHelper services)
        {
            return services
                .AddApiContextService()
                .AddSecurityContextService()
                .AddPermissionContextService()
                .AddCommonLinkUtilityService()
                .AddDisplayUserSettingsService()
                .AddCalendarDbContextService()
                .AddCalendarDataProviderService()
                .AddCalendarWrapper()
                .AddCalendarNotifyClient()
                .AddDDayICalParser()
                .AddEventHistoryWrapper()
                .AddEventWrapper();
        }
    }
}