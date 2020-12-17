
using ASC.Api.Core;
using ASC.Common;
using ASC.Common.DependencyInjection;
using ASC.Employee.Core.Controllers;

using Autofac;

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
            base.ConfigureServices(services);

            DIHelper.TryAdd<PeopleController>();
            DIHelper.TryAdd<GroupController>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(Configuration, HostEnvironment.ContentRootPath);
        }
    }
}