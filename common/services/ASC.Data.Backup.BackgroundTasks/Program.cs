using ASC.EventBus.Events;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.ConfigureDefault(args, (hostContext, config, env, path) =>
{
    config.AddJsonFile("notify.json")
          .AddJsonFile($"notify.{env.EnvironmentName}.json", true)
          .AddJsonFile("backup.json");
});

builder.WebHost.ConfigureDefaultKestrel();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    startup.ConfigureContainer(containerBuilder);
});

var app = builder.Build();

startup.Configure(app, app.Environment);

var eventBus = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IEventBus>();

eventBus.Subscribe<BackupRequestIntegrationEvent, BackupRequestIntegrationEventHandler>();
eventBus.Subscribe<BackupRestoreRequestIntegrationEvent, BackupRestoreRequestIntegrationEventHandler>();
eventBus.Subscribe<IntegrationEvent, BackupDeleteScheldureRequestIntegrationEventHandler>();

app.Run();


public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}