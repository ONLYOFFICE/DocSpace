using ASC.Api.Core.Core;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Api.Core
{
    public class BaseWorkerStartup
    {
        public IConfiguration Configuration { get; }

        public BaseWorkerStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddCustomHealthCheck(Configuration);
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}