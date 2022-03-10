var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseSystemd();
builder.Host.UseWindowsService();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureDefaultAppConfiguration(args);

builder.WebHost.ConfigureDefaultKestrel((hostingContext, serverOptions) =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
    serverOptions.Limits.MaxRequestBufferSize = 100 * 1024 * 1024;
    serverOptions.Limits.MinRequestBodyDataRate = null;
    serverOptions.Limits.MinResponseDataRate = null;
});

builder.Host.ConfigureNLogLogging();

var startup = new ASC.Files.Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    startup.ConfigureContainer(containerBuilder);
});

var app = builder.Build();

startup.Configure(app, app.Environment);

await app.RunAsync();