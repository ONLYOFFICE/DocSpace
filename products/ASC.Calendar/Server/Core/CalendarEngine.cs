using ASC.Calendar.Core.Dao;
using ASC.Calendar.Core;
using ASC.Calendar.Models;
using ASC.Core.Common.EF;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Api.Core;
using ASC.Core;
using ASC.Common.Logging;
using Microsoft.Extensions.Options;
using ASC.Calendar.Core.Dao.Models;
using System.Linq.Expressions;

namespace ASC.Calendar.Core
{
    public class CalendarEngine
    {
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

        public SecurityContext SecurityContext { get; }

        public ILog Log { get; }


        private Expression<Func<CalendarCalendars, CalendarModel>> FromDbCalendarCalendarsToCalendarModel { get; set; }

        public CalendarEngine(DbContextManager<CalendarDbContext> calendarDbContext, 
            ApiContext apiContext,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> option)
        {
            CalendarDb = calendarDbContext.Get("calendar");
            ApiContext = apiContext;
            SecurityContext = securityContext;
            Log = option.Get("ASC.CalendarEngine");
            FromDbCalendarCalendarsToCalendarModel = c => new CalendarModel
            {
                Id = c.Id,
                Name = c.Name
              
            };
        }
        public CalendarModel GetCalendarById(int id)
        {
            var t = CalendarDb.CalendarCalendars.Where(cal => cal.Id == id).FirstOrDefault();

            var cal = CalendarDb.CalendarCalendars.Select(FromDbCalendarCalendarsToCalendarModel).Where(cal => cal.Id == id).FirstOrDefault();
            

            /* int calId;
            if (int.TryParse(calendarId, out calId))
            {
                var cal = _dataProvider.GetCalendarById(calId);
                return (cal != null ? new CalendarWrapper(cal) : null);
            }

            //external                
            var extCalendar = CalendarManager.Instance.GetCalendarForUser(SecurityContext.CurrentAccount.ID, calendarId);
            if (extCalendar != null)
            {
                var viewSettings = _dataProvider.GetUserViewSettings(SecurityContext.CurrentAccount.ID, new List<string> { calendarId });
                return new CalendarWrapper(extCalendar, viewSettings.FirstOrDefault());
            }
            */
            return cal;
        }
    }

    public static class CalendarEngineExtention
    {
        public static IServiceCollection AddCalendarEngineService(this IServiceCollection services)
        {

            services.AddScoped<CalendarEngine>();

            return services;
        }
    }
}
