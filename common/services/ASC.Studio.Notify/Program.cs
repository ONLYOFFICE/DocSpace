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

builder.Host.ConfigureDefaultAppConfiguration(args, (hostContext, config, env, path) =>
{
    config.AddJsonFile($"appsettings.services.json", true)
          .AddJsonFile("notify.json")
          .AddJsonFile($"notify.{env.EnvironmentName}.json", true);
});

builder.Host.ConfigureServices((hostContext, services) =>
{
    services.AddMemoryCache();
    services.AddHttpClient();

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
    services.AddHostedService<ServiceLauncher>();
    diHelper.TryAdd<ServiceLauncher>();
    NotifyConfigurationExtension.Register(diHelper);
    diHelper.TryAdd<EmailSenderSink>();
    services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
});

builder.Host.ConfigureNLogLogging();

var startup = new BaseWorkerStartup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

app.Run();
