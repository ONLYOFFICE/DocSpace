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