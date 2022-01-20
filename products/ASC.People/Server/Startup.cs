
using ASC.Api.Core;
using ASC.Common;
using ASC.Employee.Core.Controllers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace ASC.People
{
public class Startup : BaseStartup
    {
        public override bool ConfirmAddScheme { get => true; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            DIHelper.TryAdd<PeopleController>();
            DIHelper.TryAdd<GroupController>();

            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis").Get<RedisConfiguration>());
        }
    }
}