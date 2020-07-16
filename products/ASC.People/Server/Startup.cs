
using System;

using ASC.Api.Core;
using ASC.Common;
using ASC.Data.Reassigns;
using ASC.Employee.Core.Controllers;
using ASC.Web.Core.Users;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.People
{
    public class Startup : BaseStartup
    {
        public override string[] LogParams { get => new string[] { "ASC.Api", "ASC.Web" }; }
        public override bool ConfirmAddScheme { get => true; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var diHelper = new DIHelper(services);

            diHelper.AddProgressQueue<RemoveProgressItem>(1, (int)TimeSpan.FromMinutes(5).TotalMilliseconds, true, false, 0);
            diHelper.AddProgressQueue<ReassignProgressItem>(1, (int)TimeSpan.FromMinutes(5).TotalMilliseconds, true, false, 0);
            diHelper.AddWorkerQueue<ResizeWorkerItem>(2, (int)TimeSpan.FromMinutes(30).TotalMilliseconds, true, 1);

            diHelper
                .AddPeopleController()
                .AddGroupController();

            base.ConfigureServices(services);
        }
    }
}