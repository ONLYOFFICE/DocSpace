using ASC.Data.Backup.BackgroundTasks;
using ASC.Data.Backup.Core.IntegrationEvents.Events;

using Microsoft.Extensions.Hosting.WindowsServices;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseWindowsService();
builder.Host.UseSystemd();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost.ConfigureKestrel((hostingContext, serverOptions) =>
{
    var kestrelConfig = hostingContext.Configuration.GetSection("Kestrel");

    if (!kestrelConfig.Exists()) return;

    var unixSocket = kestrelConfig.GetValue<string>("ListenUnixSocket");

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        if (!string.IsNullOrWhiteSpace(unixSocket))
        {
            unixSocket = string.Format(unixSocket, hostingContext.HostingEnvironment.ApplicationName.Replace("ASC.", "").Replace(".", ""));

            serverOptions.ListenUnixSocket(unixSocket);
        }
    }
});

builder.Host.ConfigureAppConfiguration((hostContext, config) =>
{
    var buided = config.Build();

    var path = buided["pathToConf"];

    if (!Path.IsPathRooted(path))
    {
        path = Path.GetFullPath(CrossPlatform.PathCombine(hostContext.HostingEnvironment.ContentRootPath, path));
    }

    config.SetBasePath(path);

    var env = hostContext.Configuration.GetValue("ENVIRONMENT", "Production");
    config
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env}.json", true)
        .AddJsonFile("storage.json")
        .AddJsonFile("notify.json")
        .AddJsonFile($"notify.{env}.json", true)
        .AddJsonFile("backup.json")
        .AddJsonFile("kafka.json")
        .AddJsonFile($"kafka.{env}.json", true)
        .AddJsonFile("redis.json")
        .AddJsonFile($"redis.{env}.json", true)
        .AddJsonFile("rabbitmq.json")
        .AddJsonFile($"rabbitmq.{env}.json", true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddInMemoryCollection(new Dictionary<string, string>
            {
                    {"pathToConf", path }
            }
        );
});

builder.Host.ConfigureNLogLogging();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    startup.ConfigureContainer(containerBuilder);
});

var app = builder.Build();

startup.Configure(app, app.Environment);

var eventBus = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IEventBus>();

eventBus.Subscribe<BackupRequestIntegrationEvent, BackupRequesteIntegrationEventHandler>();

app.Run();


public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}