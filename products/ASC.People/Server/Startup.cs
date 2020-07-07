
using System;

using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Middleware;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Common.Threading.Workers;
using ASC.Data.Reassigns;
using ASC.Employee.Core.Controllers;
using ASC.Web.Core.Users;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.People
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment): base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            var diHelper = new DIHelper(services);

            diHelper
                .AddConfirmAuthHandler()
                .AddCookieAuthHandler()
                .AddCultureMiddleware()
                .AddIpSecurityFilter()
                .AddPaymentFilter()
                .AddProductSecurityFilter()
                .AddTenantStatusFilter();

            diHelper.AddProgressQueue<RemoveProgressItem>(1, (int)TimeSpan.FromMinutes(5).TotalMilliseconds, true, false, 0);
            diHelper.AddProgressQueue<ReassignProgressItem>(1, (int)TimeSpan.FromMinutes(5).TotalMilliseconds, true, false, 0);
            diHelper.AddWorkerQueue<ResizeWorkerItem>(2, (int)TimeSpan.FromMinutes(30).TotalMilliseconds, true, 1);
            LogParams = new string[] { "ASC.Api", "ASC.Web" };
            diHelper
                .AddPeopleController()
                .AddGroupController();

            addcontrollers = true;
            confirmAddScheme= true;
            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }
    }
}