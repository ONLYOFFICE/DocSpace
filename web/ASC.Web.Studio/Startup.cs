namespace ASC.Web.Studio;
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();

        base.ConfigureServices(services);

        services.AddMemoryCache();
        DIHelper.TryAdd<Login>();
        DIHelper.TryAdd<PathUtils>();
        DIHelper.TryAdd<StorageHandlerScope>();
        DIHelper.TryAdd<GoogleLoginProvider>();
        DIHelper.TryAdd<FacebookLoginProvider>();
        DIHelper.TryAdd<LinkedInLoginProvider>();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseRouting();

        app.UseCors(builder =>
            builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

        app.UseAuthentication();

        app.UseEndpoints(endpoints =>
        {
            endpoints.InitializeHttpHandlers();
        });

        app.MapWhen(
            context => context.Request.Path.ToString().EndsWith("login.ashx"),
            appBranch =>
            {
                appBranch.UseLoginHandler();
            });
    }
}
