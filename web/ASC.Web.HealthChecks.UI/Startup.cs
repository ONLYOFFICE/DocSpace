namespace ASC.Web.HealthChecks.UI;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseHealthChecksUI(config =>
        {
            config.UIPath = "/hc-ui";

        });

        app.UseEndpoints(endpoints =>
        {

            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });

    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks()                
                .AddCheck("self", () => HealthCheckResult.Healthy());

        services.AddHealthChecksUI()
                .AddInMemoryStorage();
    }
}
