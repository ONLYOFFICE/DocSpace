var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseSystemd();
builder.Host.UseWindowsService();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var buildedConfig = config.Build();

    var path = buildedConfig["pathToConf"];
    if (!Path.IsPathRooted(path))
    {
        path = Path.GetFullPath(CrossPlatform.PathCombine(hostingContext.HostingEnvironment.ContentRootPath, path));
    }

    config.SetBasePath(path);

    config.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
    .AddJsonFile("storage.json")
    .AddJsonFile("kafka.json")
    .AddJsonFile($"kafka.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
    .AddJsonFile("redis.json")
    .AddJsonFile($"redis.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        {"pathToConf", path}
    });
});

builder.WebHost.ConfigureKestrel((hostingContext, serverOptions) =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
    serverOptions.Limits.MaxRequestBufferSize = 100 * 1024 * 1024;
    serverOptions.Limits.MinRequestBodyDataRate = null;
    serverOptions.Limits.MinResponseDataRate = null;

    var kestrelConfig = hostingContext.Configuration.GetSection("Kestrel");
    if (!kestrelConfig.Exists())
    {
        return;
    }

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