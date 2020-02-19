

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

namespace ASC.Calendar.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        public Tenant Tenant { get { return ApiContext.Tenant; } }
        public ApiContext ApiContext { get; }

        public UserManager UserManager { get; }
        public CalendarEngine CalendarEngine { get; }
        public ILog Log { get; }

        public CalendarController(
           
            ApiContext apiContext,
             
            UserManager userManager,

            IOptionsMonitor<ILog> option,

            CalendarEngine calendarEngine)
        {
            Log = option.Get("ASC.Api");
            ApiContext = apiContext;
            UserManager = userManager;
            CalendarEngine = calendarEngine;
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new CalendarProduct();
            product.Init();
            return new Module(product, true);
        }

        [Read("{calendarId}")]
        public CalendarModel GetCalendarById(int calendarId)
        {
            var cal = CalendarEngine.GetCalendarById(calendarId);

            return cal;
        }
    }

    public static class PeopleControllerExtention
    {
        public static IServiceCollection AddCalendarController(this IServiceCollection services)
        {
            return services
                .AddApiContextService()
                .AddSecurityContextService()
                .AddCalendarDbContextService()
                .AddCalendarEngineService();
        }
    }
}