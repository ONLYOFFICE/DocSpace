var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);
var startup = new Startup(builder.Configuration, builder.Environment);

builder.Host.UseSystemd();
builder.Host.UseWindowsService();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost.ConfigureDefaultKestrel();

builder.Host.ConfigureDefaultAppConfiguration(args);

startup.ConfigureServices(builder.Services);

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    startup.ConfigureContainer(containerBuilder);
});

builder.Host.ConfigureNLogLogging();

var app = builder.Build();

startup.Configure(app, app.Environment);

await app.RunAsync();