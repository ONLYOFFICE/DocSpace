var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseWindowsService();
builder.Host.UseSystemd();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost.ConfigureDefaultKestrel();

builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
 {
     builder.Register(context.Configuration, false, false);
 });

builder.Host.ConfigureDefaultAppConfiguration(args, (hostContext, config, env, path) =>
{
    config.AddJsonFile($"appsettings.services.json", true);
});

builder.Host.ConfigureDefaultServices((hostContext, services, diHelper) =>
{
    services.AddHttpClient();

    diHelper.TryAdd<DbWorker>();

    services.AddHostedService<BuildQueueService>();
    diHelper.TryAdd<BuildQueueService>();

    services.AddHostedService<WorkerService>();
    diHelper.TryAdd<WorkerService>();
});

builder.Host.ConfigureNLogLogging();

var startup = new BaseWorkerStartup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

app.Run();