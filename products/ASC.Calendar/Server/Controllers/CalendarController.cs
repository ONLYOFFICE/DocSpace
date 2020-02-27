

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

namespace ASC.Calendar.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class CalendarController : ControllerBase
    {

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
        public HttpContext HttpContext { get; set; }

        public CalendarController(
           
            ApiContext apiContext,
            AuthContext authContext,
            AuthorizationManager authorizationManager,
            UserManager userManager,
            TenantManager tenantManager,
            TimeZoneConverter timeZoneConverter,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            IOptionsMonitor<ILog> option,

            DataProvider dataProvider,
            IHttpContextAccessor httpContextAccessor,
            CalendarWrapperHelper calendarWrapperHelper,
            AuthManager authentication)
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
            //string name, string description, string textColor, string backgroundColor, string timeZone, EventAlertType alertType, List<SharingParam> sharingOptions, string iCalUrl, int isTodo = 0

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
            //CalendarNotifyClient.NotifyAboutSharingCalendar(cal);

            /*//iCalUrl
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
                            ImportEvents(Convert.ToInt32(cal.Id), cals);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Info(String.Format("Error import events to new calendar by ical url: {0}", ex.Message));
                }

            }*/

            return CalendarWrapperHelper.Get(cal);
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
                .AddCalendarWrapper();                
        }
    }
}