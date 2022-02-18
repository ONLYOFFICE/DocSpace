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

builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
 {
     builder.Register(context.Configuration, false, false);
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
        .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"pathToConf", path }
            }
        )
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env}.json", true)
        .AddJsonFile($"appsettings.services.json", true)
        .AddJsonFile("storage.json")
        .AddJsonFile("kafka.json")
        .AddJsonFile($"kafka.{env}.json", true)
        .AddJsonFile("redis.json")
        .AddJsonFile($"redis.{env}.json", true)
        .AddEnvironmentVariables()
        .AddCommandLine(args);
});

builder.WebHost.ConfigureServices((hostContext, services) =>
{
    services.AddMemoryCache();

    var diHelper = new DIHelper(services);

    var redisConfiguration = hostContext.Configuration.GetSection("Redis").Get<RedisConfiguration>();
    var kafkaConfiguration = hostContext.Configuration.GetSection("kafka").Get<KafkaSettings>();

    if (kafkaConfiguration != null)
    {
        diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCacheNotify<>));
    }
    else if (redisConfiguration != null)
    {
        diHelper.TryAdd(typeof(ICacheNotify<>), typeof(RedisCacheNotify<>));

        services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);
    }
    else
    {
        diHelper.TryAdd(typeof(ICacheNotify<>), typeof(MemoryCacheNotify<>));
    }

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