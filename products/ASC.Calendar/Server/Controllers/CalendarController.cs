

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
using Ical.Net.DataTypes;
using System.Threading;
using System.Text;
using ASC.Common.Web;
using System.Net.Mime;
using ASC.Common.Caching;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ASC.Calendar.Controllers
{
    public interface IApiContentResponce
    {
        Stream ContentStream { get; }
        ContentType ContentType { get; }
        Encoding ContentEncoding { get; }
        ContentDisposition ContentDisposition { get; }
    }
    public class iCalApiContentResponse : IApiContentResponce
    {
        private Stream _stream;
        private string _fileName;

        public iCalApiContentResponse(Stream stream, string fileName)
        {
            _stream = stream;
            _fileName = fileName;
        }

        #region IApiContentResponce Members

        public Encoding ContentEncoding
        {
            get { return Encoding.UTF8; }
        }

        public Stream ContentStream
        {
            get { return _stream; }
        }

        public System.Net.Mime.ContentType ContentType
        {
            get { return new System.Net.Mime.ContentType("text/calendar; charset=UTF-8"); }
        }

        public System.Net.Mime.ContentDisposition ContentDisposition
        {
            get { return new System.Net.Mime.ContentDisposition { Inline = true, FileName = _fileName }; }
        }

        #endregion
    }

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
        public TodoWrapperHelper TodoWrapperHelper { get; }
        public Signature Signature { get; }
        public SecurityContext SecurityContext { get; }
        public ExportDataCache ExportDataCache { get; }
        public SubscriptionWrapperHelper SubscriptionWrapperHelper { get; }


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
            PublicItemCollectionHelper publicItemCollectionHelper,
            TodoWrapperHelper todoWrapperHelper,
            Signature signature,
            SecurityContext securityContext,
            ExportDataCache exportDataCache,
            SubscriptionWrapperHelper subscriptionWrapperHelper)
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
            TodoWrapperHelper = todoWrapperHelper;
            Signature = signature;
            SecurityContext = securityContext;
            ExportDataCache = exportDataCache;
            SubscriptionWrapperHelper = subscriptionWrapperHelper;

            CalendarManager.Instance.RegistryCalendar(new SharedEventsCalendar(AuthContext, TimeZoneConverter, TenantManager, DataProvider));
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

        public enum EventRemoveType
        {
            Single = 0,
            AllFollowing = 1,
            AllSeries = 2
        }
        [Read("info")]
        public Module GetModule()
        {
            var product = new CalendarProduct();
            product.Init();
            return new Module(product, true);
        }
        [Read("subscriptions")]
        public List<SubscriptionWrapper> LoadSubscriptions()
        {
            var result = new List<SubscriptionWrapper>();

            var calendars = DataProvider.LoadSubscriptionsForUser(SecurityContext.CurrentAccount.ID);
            result.AddRange(calendars.FindAll(c => !c.OwnerId.Equals(SecurityContext.CurrentAccount.ID)).ConvertAll(c => SubscriptionWrapperHelper.Get(c)));

            var iCalStreams = DataProvider.LoadiCalStreamsForUser(SecurityContext.CurrentAccount.ID);
            result.AddRange(iCalStreams.ConvertAll(c => SubscriptionWrapperHelper.Get(c)));


            var extCalendars = CalendarManager.Instance.GetCalendarsForUser(SecurityContext.CurrentAccount.ID, UserManager);
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
        [Update("subscriptions/manage")]
        public void ManageSubscriptions(IEnumerable<SubscriptionState> states)
        {
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
        [Read("eventdays/{startDate}/{endDate}")]
        public List<ApiDateTime> GetEventDays(ApiDateTime startDate, ApiDateTime endDate)
        {
            var result = new List<CalendarWrapper>();
            int newCalendarsCount;
            //internal
            var calendars = DataProvider.LoadCalendarsForUser(SecurityContext.CurrentAccount.ID, out newCalendarsCount);

            result.AddRange(calendars.ConvertAll(c => CalendarWrapperHelper.Get(c)));

            
            //external
            var extCalendars = CalendarManager.Instance.GetCalendarsForUser(SecurityContext.CurrentAccount.ID, UserManager);
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
        [Read("calendars/{startDate}/{endDate}")]
        public List<CalendarWrapper> LoadCalendars(ApiDateTime startDate, ApiDateTime endDate)
        {
            var result = LoadInternalCalendars();

            //external
           
                var extCalendars = CalendarManager.Instance.GetCalendarsForUser(SecurityContext.CurrentAccount.ID, UserManager);
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
                        c.Todos = c.UserCalendar.GetTodoWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate);
                        c.Events.RemoveAll(e => sharedEvents.Events.Exists(sEv => string.Equals(sEv.Id, e.Id, StringComparison.InvariantCultureIgnoreCase)));
                    });
                else
                    result.ForEach(c =>
                    {
                        c.Events = c.UserCalendar.GetEventWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate, EventWrapperHelper);
                        c.Todos = c.UserCalendar.GetTodoWrappers(SecurityContext.CurrentAccount.ID, startDate, endDate);
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
        
        [Read("{calendarId}/icalurl")]
        public object GetCalendariCalUrl(string calendarId)
        {
            var sig = Signature.Create(AuthContext.CurrentAccount.ID);

            // var path = UrlPath.ResolveUrl(() => new CalendarApi().GetCalendariCalStream(calendarId, sig));

            var path = "api/2.0/calendar/" + calendarId + "/ical/" + sig;
            
            var result = new Uri(HttpContext.Request.GetUrlRewriter(), VirtualPathUtility.ToAbsolute("~/" + path)).ToString();

            return new{result};
        }
        [Read("{calendarId}/ical/{signature}", false)] //NOTE: this method doesn't requires auth!!!
        public iCalApiContentResponse GetCalendariCalStream(string calendarId, string signature)
        {
            try
            {
                //do not use compression
               
                var acceptEncoding = HttpContext.Request.Headers["Accept-Encoding"];
                if (acceptEncoding.Count > 0)
                {
                    /*var encodings = acceptEncoding.Split(',');
                    if (encodings.Contains("gzip"))
                    {
                        encodings = (from x in encodings where x != "gzip" select x).ToArray();

                        Type t = HttpContext.Request.Headers.GetType();
                        PropertyInfo propertyInfo = t.GetProperty("IsReadOnly", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        propertyInfo.SetValue(HttpContext.Request.Headers, false, null);

                        HttpContext.Request.Headers.Set("Accept-Encoding", string.Join(",", encodings));

                    }*/
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }


            iCalApiContentResponse resp = null;
            var userId = Signature.Read<Guid>(signature);
            if (UserManager.GetUsers(userId).ID != Constants.LostUser.ID)
            {
                var currentUserId = Guid.Empty;
                if (AuthContext.IsAuthenticated)
                {
                    currentUserId = AuthContext.CurrentAccount.ID;
                    AuthContext.Logout();
                }
                try
                {
                    SecurityContext.AuthenticateMe(userId);
                    var icalFormat = GetCalendariCalString(calendarId);
                    if (icalFormat != null)
                        resp = new iCalApiContentResponse(new MemoryStream(Encoding.UTF8.GetBytes(icalFormat)), calendarId + ".ics");
                }
                finally
                {
                    AuthContext.Logout();
                    if (currentUserId != Guid.Empty)
                    {
                        SecurityContext.AuthenticateMe(currentUserId);
                    }
                }
            }
            return resp;
        }

        [Create]
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
            if (!string.IsNullOrEmpty(calendar.ICalUrl))
            {
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(calendar.ICalUrl);
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

        [Update("{calendarId}")]
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
                    var req = (HttpWebRequest)WebRequest.Create(iCalUrl);
                    using (var resp = req.GetResponse())
                    using (var stream = resp.GetResponseStream())
                    {
                        var ms = new MemoryStream();
                        stream.StreamCopyTo(ms);
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
                    Log.Info(String.Format("Error import events to calendar by ical url: {0}", ex.Message));
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
                    string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, UserManager.GetUserByEmail(_email).ID);

                    var cal = DataProvider.UpdateCalendar(calId, name, description,
                                        sharingOptionsList.Select(o => o as SharingOptions.PublicItem).ToList(),
                                        userOptions);

                    var oldSharingList = new List<SharingParam>();
                    var owner = UserManager.GetUsers(cal.OwnerId);
                    var ownerAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, cal.OwnerId);

                    //TODO Caldav
                    /*
                    if (AuthContext.CurrentAccount.ID != cal.OwnerId)
                    {
                        UpdateCalDavCalendar(name, description, backgroundColor, oldCal.calDavGuid, myUri, owner.Email, ownerAccountPaswd);
                    }
                    else
                    {
                        UpdateCalDavCalendar(name, description, backgroundColor, oldCal.calDavGuid, myUri, _email, currentAccountPaswd);
                    }*/
                    var pic = PublicItemCollectionHelper.GetForCalendar(oldCal);
                    if (pic.Items.Count > 1)
                    {
                        oldSharingList.AddRange(from publicItem in pic.Items
                                                where publicItem.ItemId != owner.ID.ToString()
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

                        //TODO Caldav
                        /* var caldavTask = new Task(() => UpdateSharedCalDavCalendar(name, description, backgroundColor, oldCal.calDavGuid, myUri, sharingOptionsList, events, calendarId, cal.calDavGuid, tenant.TenantId, DateTime.Now, targetCalendar.TimeZones[0], cal.TimeZone));
                         caldavTask.Start();*/
                    }

                    oldSharingList.RemoveAll(c => sharingOptionsList.Contains(sharingOptionsList.Find((x) => x.Id == c.Id)));

                    if (oldSharingList.Count > 0)
                    {
                        var currentTenantId = TenantManager.GetCurrentTenant().TenantId;
                        var caldavHost = myUri.Host;

                        //TODO Caldav
                        /*var replaceSharingEventThread = new Thread(() =>
                        {
                            TenantManager.SetCurrentTenant(currentTenantId);
                            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin@ascsystem" + ":" + ASC.Core.Configuration.Constants.CoreSystem.ID));

                            foreach (var sharingOption in oldSharingList)
                            {
                                if (!sharingOption.IsGroup)
                                {
                                    var user = UserManager.GetUsers(sharingOption.itemId);
                                    var currentUserName = user.Email.ToLower() + "@" + caldavHost;
                                    var userEmail = user.Email;
                                    string userPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, UserManager.GetUserByEmail(userEmail).ID);
                                    DataProvider.RemoveCaldavCalendar(currentUserName, userEmail, userPaswd, encoded, cal.calDavGuid, myUri, user.ID != cal.OwnerId);
                                }
                                else
                                {
                                    var users = CoreContext.UserManager.GetUsersByGroup(sharingOption.itemId);
                                    foreach (var user in users)
                                    {
                                        var currentUserName = user.Email.ToLower() + "@" + caldavHost;
                                        var userEmail = user.Email;
                                        string userPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, UserManager.GetUserByEmail(userEmail).ID);
                                        DataProvider.RemoveCaldavCalendar(currentUserName, userEmail, userPaswd, encoded, cal.calDavGuid, myUri, user.ID != cal.OwnerId);
                                    }
                                }
                            }
                        });
                        replaceSharingEventThread.Start();*/
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

        [Update("{calendarId}/view")]
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

        [Delete("{calendarId}")]
        public void RemoveCalendar(int calendarId)
        {
            var cal = DataProvider.GetCalendarById(calendarId);
            var events = cal.LoadEvents(AuthContext.CurrentAccount.ID, DateTime.MinValue, DateTime.MaxValue);

            var pic = PublicItemCollectionHelper.GetForCalendar(cal);
            //check permissions
            CheckPermissions(cal, CalendarAccessRights.FullAccessAction);
            //clear old rights
            AuthorizationManager.RemoveAllAces(cal);

            DataProvider.RemoveCalendar(calendarId, HttpContext.Request.GetUrlRewriter());

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
            var currentTenantId = TenantManager.GetCurrentTenant().TenantId;
            var myUri = HttpContext.Request.GetUrlRewriter();
            var caldavHost = myUri.Host;

            //TODO Caldav
            /* var replaceSharingEventThread = new Thread(() =>
             {
                 TenantManager.SetCurrentTenant(currentTenantId);
                 var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin@ascsystem" + ":" + ASC.Core.Configuration.Constants.CoreSystem.ID));

                 foreach (var sharingOption in sharingList)
                 {
                     if (!sharingOption.IsGroup)
                     {
                         var user = UserManager.GetUsers(sharingOption.itemId);
                         var currentUserName = user.Email.ToLower() + "@" + caldavHost;
                         var _email = user.Email;
                         string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, UserManager.GetUserByEmail(_email).ID);
                         DataProvider.RemoveCaldavCalendar(currentUserName, _email, currentAccountPaswd, encoded, cal.calDavGuid, myUri, user.ID != cal.OwnerId);
                     }
                     else
                     {
                         var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                         foreach (var user in users)
                         {
                             var currentUserName = user.Email.ToLower() + "@" + caldavHost;
                             var _email = user.Email;
                             string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, UserManager.GetUserByEmail(_email).ID);
                             DataProvider.RemoveCaldavCalendar(currentUserName, _email, currentAccountPaswd, encoded, cal.calDavGuid, myUri, user.ID != cal.OwnerId);
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
                                 var currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, user.ID);

                                 deleteEvent(fullAccess ? split[0] + "_write" : split[0], SharedEventsCalendar.CalendarId, user.Email, currentAccountPaswd, myUri, user.ID != evt.OwnerId);
                             }
                             else
                             {
                                 var users = UserManager.GetUsersByGroup(sharingOption.itemId);
                                 foreach (var user in users)
                                 {
                                     var eventUid = user.ID == evt.OwnerId
                                                        ? split[0]
                                                        : fullAccess ? split[0] + "_write" : split[0];
                                     var currentAccountPaswd = Authentication.GetUserPasswordHash(user.ID);

                                     deleteEvent(eventUid, SharedEventsCalendar.CalendarId, user.Email, currentAccountPaswd, myUri, true);
                                 }
                             }
                         }
                     }
                 }

             });
             replaceSharingEventThread.Start();*/

        }

        [Delete("events/{eventId}/unsubscribe")]
        public void UnsubscribeEvent(int eventId)
        {
            var evt = DataProvider.GetEventById(eventId);

            if (evt != null)
            {
                string[] split = evt.Uid.Split(new Char[] { '@' });
                var myUri = HttpContext.Request.GetUrlRewriter();
                var email = UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email;
                var currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, SecurityContext.CurrentAccount.ID);
                var fullAccess = CheckPermissions(evt, CalendarAccessRights.FullAccessAction, true);

                deleteEvent(fullAccess ? split[0] + "_write" : split[0], SharedEventsCalendar.CalendarId, email, currentAccountPaswd, myUri, SecurityContext.CurrentAccount.ID != evt.OwnerId);

                DataProvider.UnsubscribeFromEvent(eventId, SecurityContext.CurrentAccount.ID);
            }
        }
        //TODO Caldav
        private void deleteEvent(string uid, string calendarId, string email, string currentAccountPaswd, Uri myUri, bool isShared = false)
        {
            
            try
            {
                 /*var сaldavGuid = "";
                if (calendarId != BirthdayReminderCalendar.CalendarId &&
                    calendarId != SharedEventsCalendar.CalendarId &&
                    calendarId != "crm_calendar" &&
                    !calendarId.Contains("Project_"))
                {
                    var dataCaldavGuid = db.ExecuteList(new SqlQuery("calendar_calendars")
                        .Select("caldav_guid")
                        .Where("id", calendarId))
                        .Select(r => r[0])
                        .ToArray();

                    сaldavGuid = dataCaldavGuid[0].ToString();
                }
                else
                {
                    сaldavGuid = calendarId;
                }

                if (сaldavGuid != "")
                {
                    var calDavServerUrl = myUri.Scheme + "://" + myUri.Host + "/caldav";

                    Logger.Info("RADICALE REWRITE URL: " + myUri);

                    var currentUserName = email + "@" + myUri.Host;

                    var encoded = isShared ? Convert.ToBase64String(Encoding.UTF8.GetBytes("admin@ascsystem" + ":" + ASC.Core.Configuration.Constants.CoreSystem.ID)) : Convert.ToBase64String(Encoding.UTF8.GetBytes(email.ToLower() + ":" + currentAccountPaswd));

                    var requestUrl = calDavServerUrl + "/" + HttpUtility.UrlEncode(currentUserName) + "/" + (isShared ? сaldavGuid + "-shared" : сaldavGuid) + "/" + uid + ".ics";

                    try
                    {
                        var webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                        webRequest.Method = "DELETE";
                        webRequest.Headers.Add("Authorization", "Basic " + encoded);
                        using (var webResponse = webRequest.GetResponse())
                        using (var reader = new StreamReader(webResponse.GetResponseStream())) { }
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                        {
                            var resp = (HttpWebResponse)ex.Response;
                            if (resp.StatusCode == HttpStatusCode.NotFound || resp.StatusCode == HttpStatusCode.Conflict)
                                Logger.Debug("ERROR: " + ex.Message);
                            else
                                Logger.Error("ERROR: " + ex.Message);
                        }
                        else
                        {
                            Logger.Error("ERROR: " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("ERROR: " + ex.Message);
                    }
                }*/
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: " + ex.Message);
            }

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
        [Read("events/{eventId}/historybyid")]
        public EventHistoryWrapper GetEventHistoryById(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentException("eventId");
            }

            var evt = DataProvider.GetEventById(eventId);

            return GetEventHistoryWrapper(evt);
        }
        [Create("event")]
        public List<EventWrapper> CreateEvent(EventModel eventModel)
        {
            var calendar = LoadInternalCalendars().First(x => (!x.IsSubscription && x.IsTodo != 1));
            int calendarId;

            if (int.TryParse(calendar.Id, out calendarId))
            {
                return AddEvent(calendarId, eventModel);
            }

            throw new Exception(string.Format("Can't parse {0} to int", calendar.Id));
        }
        [Create("{calendarId}/event")]
        public List<EventWrapper> AddEvent(int calendarId, EventModel eventModel)
        {
            eventModel.CalendarId = calendarId.ToString();
            return AddEvent(eventModel);
        }

        [Update("{calendarId}/{eventId}")]
        public List<EventWrapper> Update(string calendarId, int eventId, EventModel eventModel)
        {
            eventModel.CalendarId = calendarId;
            eventModel.EventId = eventId;

            return UpdateEvent(eventModel);
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

        [Update("icsevent.json")]
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

        [Delete("events/{eventId}")]
        public void RemoveEvent(int eventId)
        {

            RemoveEvent(eventId, new EventDeleteModel { EventId = eventId, Date = null, Type = EventRemoveType.AllSeries });
        }
        [Delete("events/{eventId}/custom")]
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

            var currentTenantId = TenantManager.GetCurrentTenant().TenantId;
            var calendarId = evt.CalendarId;
            var myUri = HttpContext.Request != null ? HttpContext.Request.GetUrlRewriter() : eventDeleteModel.Uri ?? new Uri("http://localhost");
            var currentUserId = AuthContext.CurrentAccount.ID;

            //TODO Caldav
            /* var removeEventThread = new Thread(() =>
             {
                 CoreContext.TenantManager.SetCurrentTenant(currentTenantId);
                 //calendar sharing list
                 foreach (var sharingOption in sharingList)
                 {
                     var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                     if (!sharingOption.IsGroup)
                     {
                         var user = CoreContext.UserManager.GetUsers(sharingOption.itemId);
                         var currentAccountPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);

                         deleteEvent(fullAccess ? split[0] + "_write" : split[0], calendarId, user.Email, currentAccountPaswd, myUri, user.ID != cal.OwnerId);
                     }
                     else
                     {
                         var users = CoreContext.UserManager.GetUsersByGroup(sharingOption.itemId);
                         foreach (var user in users)
                         {
                             var eventUid = user.ID == evt.OwnerId
                                                ? split[0]
                                                : fullAccess ? split[0] + "_write" : split[0];
                             var currentAccountPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);
                             deleteEvent(eventUid, calendarId, user.Email, currentAccountPaswd, myUri, true);
                         }
                     }
                 }
                 //event sharing list
                 foreach (var sharingOption in so)
                 {
                     var fullAccess = sharingOption.actionId == AccessOption.FullAccessOption.Id;

                     if (!sharingOption.IsGroup)
                     {
                         var user = CoreContext.UserManager.GetUsers(sharingOption.itemId);
                         var currentAccountPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);

                         deleteEvent(fullAccess ? split[0] + "_write" : split[0], SharedEventsCalendar.CalendarId, user.Email, currentAccountPaswd, myUri, user.ID != evt.OwnerId);
                     }
                     else
                     {
                         var users = CoreContext.UserManager.GetUsersByGroup(sharingOption.itemId);
                         foreach (var user in users)
                         {
                             var eventUid = user.ID == evt.OwnerId
                                                ? split[0]
                                                : fullAccess ? split[0] + "_write" : split[0];
                             var currentAccountPaswd = CoreContext.Authentication.GetUserPasswordHash(user.ID);

                             deleteEvent(eventUid, SharedEventsCalendar.CalendarId, user.Email, currentAccountPaswd, myUri, true);
                         }
                     }
                 }
                 if (currentUserId == evt.OwnerId)
                 {
                     var owner = CoreContext.UserManager.GetUsers(evt.OwnerId);
                     var ownerPaswd = CoreContext.Authentication.GetUserPasswordHash(evt.OwnerId);
                     deleteEvent(split[0], evt.CalendarId, owner.Email, ownerPaswd, myUri, false);
                 }
                 if (calendarId != BirthdayReminderCalendar.CalendarId &&
                        calendarId != SharedEventsCalendar.CalendarId &&
                        calendarId != "crm_calendar" &&
                        !calendarId.Contains("Project_"))
                 {
                     if (currentUserId == cal.OwnerId)
                     {
                         var owner = CoreContext.UserManager.GetUsers(currentUserId);
                         var ownerPaswd = CoreContext.Authentication.GetUserPasswordHash(currentUserId);

                         deleteEvent(split[0], evt.CalendarId, owner.Email, ownerPaswd, myUri, false);
                     }
                 }
             }); */


            if (evt.OwnerId.Equals(AuthContext.CurrentAccount.ID) || CheckPermissions(evt, CalendarAccessRights.FullAccessAction, true) || CheckPermissions(cal, CalendarAccessRights.FullAccessAction, true))
            {
                if (eventDeleteModel.Type == EventRemoveType.AllSeries || evt.RecurrenceRule.Freq == Frequency.Never)
                {
                    DataProvider.RemoveEvent(eventId);
                    //TODO Caldav
                    /* if (!fromCaldavServer)
                     {
                         var ownerId = SecurityContext.CurrentAccount.ID != cal.OwnerId ? cal.OwnerId : SecurityContext.CurrentAccount.ID;
                         var email = CoreContext.UserManager.GetUsers(ownerId).Email;
                         string currentAccountPaswd = CoreContext.Authentication.GetUserPasswordHash(ownerId);
                         deleteEvent(split[0], evt.CalendarId, email, currentAccountPaswd, myUri);
                         removeEventThread.Start();
                     }*/
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
                        isDateTime = !evt.AllDayLong
                    });
                }
                else if (eventDeleteModel.Type == EventRemoveType.AllFollowing)
                {
                    var lastEventDate = evt.AllDayLong ? utcDate.Date : utcDate;
                    var dates = evt.RecurrenceRule
                        .GetDates(evt.UtcStartDate, evt.UtcStartDate, evt.UtcStartDate.AddMonths(_monthCount), int.MaxValue, false)
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
                        string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

                        var calendarObj = DataProvider.GetCalendarById(Convert.ToInt32(cal.Id));
                        var calendarObjViewSettings = calendarObj != null && calendarObj.ViewSettings != null ? calendarObj.ViewSettings.FirstOrDefault() : null;
                        var targetCalendar = DDayICalParser.ConvertCalendar(calendarObj != null ? calendarObj.GetUserCalendar(calendarObjViewSettings) : null);

                        targetCalendar.Events.Clear();

                        var convertedEvent = DDayICalParser.ConvertEvent(evt as BaseEvent);
                        convertedEvent.ExceptionDates.Clear();

                        foreach (var exDate in evt.RecurrenceRule.ExDates)
                        {
                            var periodList = new PeriodList { new CalDateTime(exDate.Date) };

                            if (exDate.isDateTime)
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

                        //TODo Caldav
                        /* var updateCaldavThread = new Thread(() => updateCaldavEvent(ics, split[0], true, calDavGuid, myUri, currentUserEmail, currentAccountPaswd, DateTime.Now, targetCalendar.TimeZones[0], cal.TimeZone, true));
                         updateCaldavThread.Start();*/
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
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
                                        exDate.isDateTime ?
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


        public class CreateTodoModel
        {
            public string ics { get; set; }
            public string todoUid { get; set; }
            public string todoId { get; set; }
            public string calendarId { get; set; }
            public bool fromCalDavServer { get; set; }

        }
        [Create("icstodo")]
        public List<TodoWrapper> AddTodo(CreateTodoModel createTodoModel)
        {
            var ics = createTodoModel.ics;
            var todoUid = createTodoModel.todoUid;

            var old_ics = ics;

            var todoCalendars = DataProvider.LoadTodoCalendarsForUser(AuthContext.CurrentAccount.ID);
            var userTimeZone = TenantManager.GetCurrentTenant().TimeZone;

            var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager);
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
                    TimeZone = userTimeZone.ToString(),
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
                    throw new Exception(string.Format("Can't parse {0} to int", defaultCalendar.Id));
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

            todoUid = todoUid == null ? null : string.Format("{0}@onlyoffice.com", todoUid);


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
                string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

                //TODO Caldav
                /*var updateCaldavThread = new Thread(() => updateCaldavEvent(old_ics, split[0], true, calDavGuid, myUri, currentUserEmail, currentAccountPaswd));
                updateCaldavThread.Start();*/
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return result;
        }

        [Update("icstodo")]
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
                    string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

                    //TODO Caldav
                    /* var updateCaldavThread = new Thread(() => updateCaldavEvent(old_ics, split[0], true, calDavGuid, myUri, currentUserEmail, currentAccountPaswd));
                      updateCaldavThread.Start();*/
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
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

        [Delete("todos/{todoId}")]
        public void RemoveTodo(int todoId, CreateTodoModel createTodoModel)
        {
            var fromCaldavServer = createTodoModel.fromCalDavServer;
            var todo = DataProvider.GetTodoById(todoId);

            var uid = todo.Uid;
            string[] split = uid.Split(new Char[] { '@' });

            if (!fromCaldavServer)
            {
                var email = UserManager.GetUsers(AuthContext.CurrentAccount.ID).Email;
                string currentAccountPaswd = Authentication.GetUserPasswordHash(TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);
                var myUri = HttpContext.Request.GetUrlRewriter();

                //TODO Caldav
                // deleteEvent(split[0], todo.CalendarId, email, currentAccountPaswd, myUri);
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

        [Create("outsideevent")]
        public void AddEventOutside(string calendarGuid, string eventGuid, string ics)
        {

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

        [Read("{calendarId}/sharing")]
        public PublicItemCollection GetCalendarSharingOptions(int calendarId)
        {
            var cal = DataProvider.GetCalendarById(calendarId);
            if (cal == null)
                throw new Exception(Resources.CalendarApiResource.ErrorItemNotFound);

            return PublicItemCollectionHelper.GetForCalendar(cal);
        }
        [Read("sharing")]
        public PublicItemCollection GetDefaultSharingOptions()
        {
            return PublicItemCollectionHelper.GetDefault();
        }

        [Create("calendarUrl")]
        public CalendarWrapper CreateCalendarStream(СalendarUrlModel сalendarUrl)
        {
            var iCalUrl = сalendarUrl.ICalUrl;
            var name = сalendarUrl.Name;
            var textColor = сalendarUrl.TextColor;
            var backgroundColor = сalendarUrl.BackgroundColor;

            var icalendar = new iCalendar(AuthContext, TimeZoneConverter, TenantManager);
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

        private string GetCalendariCalString(string calendarId, bool ignoreCache = false)
        {
            Log.Debug("GetCalendariCalString calendarId = " + calendarId);

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
                    icalendar = CalendarManager.Instance.GetCalendarForUser(AuthContext.CurrentAccount.ID, calendarId, UserManager);
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

                            if (exDate.isDateTime)
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

                Log.Debug(elapsedTime);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
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
                .AddSignatureService()
                .AddDisplayUserSettingsService()
                .AddCalendarDbContextService()
                .AddCalendarDataProviderService()
                .AddCalendarWrapper()
                .AddCalendarNotifyClient()
                .AddDDayICalParser()
                .AddEventHistoryWrapper()
                .AddEventWrapper()
                .AddTodoWrapper()
                .AddExportDataCache()
                .AddSubscriptionWrapperHelper();
        }
    }
}