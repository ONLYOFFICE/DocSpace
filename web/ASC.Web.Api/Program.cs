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
    config
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true)
    .AddJsonFile("storage.json")
    .AddJsonFile("kafka.json")
    .AddJsonFile($"kafka.{hostContext.HostingEnvironment.EnvironmentName}.json", true)
    .AddJsonFile("redis.json")
    .AddJsonFile($"redis.{hostContext.HostingEnvironment.EnvironmentName}.json", true)
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

app.Run();
