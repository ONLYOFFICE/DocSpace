

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
        public CalendarController(
           
            ApiContext apiContext,
            AuthContext authContext,
            UserManager userManager,
            TenantManager tenantManager,
            TimeZoneConverter timeZoneConverter,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            IOptionsMonitor<ILog> option,

            DataProvider dataProvider,

            CalendarWrapperHelper calendarWrapperHelper)
        {
            AuthContext = authContext;
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
    }

    public static class CalendarControllerExtention
    {
        public static IServiceCollection AddCalendarController(this IServiceCollection services)
        {
            return services
                .AddApiContextService()
                .AddSecurityContextService()
                .AddCalendarDbContextService()
                .AddCalendarDataProviderService()
                .AddCalendarWrapper();
        }
    }
}