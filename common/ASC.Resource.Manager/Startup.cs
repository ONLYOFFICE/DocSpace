using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Resource.Manager
{
    public class Startup
    {
        IConfiguration Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var diHelper = new DIHelper(services);
            services.AddLogging();
            diHelper.TryAddScoped<ResourceData>();
            diHelper.TryAddScoped<ProgramScope>();

            diHelper.AddDbContextManagerService<ResourceDbContext>();
            diHelper.AddLoggerService();
            diHelper.AddNLogManager();
            diHelper.TryAddSingleton(Configuration);
        }
    }
}
