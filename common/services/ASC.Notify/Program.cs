var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);
var startup = new BaseWorkerStartup(builder.Configuration);

builder.Host.UseSystemd();
builder.Host.UseWindowsService();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureAppConfiguration((hostContext, config) =>
{
    var buildedConfig = config.Build();
    var path = buildedConfig["pathToConf"];
    if (!Path.IsPathRooted(path))
    {
        path = Path.GetFullPath(CrossPlatform.PathCombine(hostContext.HostingEnvironment.ContentRootPath, path));
    }

    config.SetBasePath(path);

    var env = hostContext.Configuration.GetValue("ENVIRONMENT", "Production");

    config.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env}.json", true)
    .AddJsonFile($"appsettings.services.json", true)
    .AddJsonFile("storage.json")
    .AddJsonFile("notify.json")
    .AddJsonFile($"notify.{env}.json", true)
    .AddJsonFile("kafka.json")
    .AddJsonFile($"kafka.{env}.json", true)
    .AddJsonFile("redis.json")
    .AddJsonFile($"redis.{env}.json", true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        {"pathToConf", path }
    });
});

startup.ConfigureServices(builder.Services);
builder.Host.ConfigureServices((hostContext, services) =>
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

    diHelper.RegisterProducts(hostContext.Configuration, hostContext.HostingEnvironment.ContentRootPath);

    services.Configure<NotifyServiceCfg>(hostContext.Configuration.GetSection("notify"));

    diHelper.TryAdd<NotifyService>();
    diHelper.TryAdd<NotifySenderService>();
    diHelper.TryAdd<NotifyCleanerService>();
    diHelper.TryAdd<TenantManager>();
    diHelper.TryAdd<TenantWhiteLabelSettingsHelper>();
    diHelper.TryAdd<SettingsManager>();
    diHelper.TryAdd<JabberSender>();
    diHelper.TryAdd<SmtpSender>();
    diHelper.TryAdd<AWSSender>(); // fix private

    services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

    services.AddHostedService<NotifyService>();
    services.AddHostedService<NotifySenderService>();
    services.AddHostedService<NotifyCleanerService>();
});

builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
{
    builder.Register(context.Configuration);
});

builder.Host.ConfigureNLogLogging();

var app = builder.Build();
startup.Configure(app);

await app.RunAsync();