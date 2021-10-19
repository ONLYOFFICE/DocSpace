using ASC.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Resource.Manager
{
    public class Startup
    {
        IConfiguration Configuration { get; }

        public Startup(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var diHelper = new DIHelper(services);
            services.AddLogging();
            diHelper.Configure(services);
            services.AddSingleton(Configuration);
            diHelper.TryAdd<ProgramScope>();
            //LogNLogExtension.ConfigureLog(diHelper);
        }
    }
}
