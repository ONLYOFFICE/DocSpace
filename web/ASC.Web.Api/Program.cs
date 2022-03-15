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

builder.Host.ConfigureDefaultAppConfiguration(args);

builder.Host.ConfigureNLogLogging();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    startup.ConfigureContainer(containerBuilder);
});

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
