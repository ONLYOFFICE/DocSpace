var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseSystemd();
builder.Host.UseWindowsService();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureAppConfiguration((hostContext, config) =>
{
    var configRoot = config.Build();
    var path = configRoot["pathToConf"];

                    if (!Path.IsPathRooted(path))
                        path = Path.GetFullPath(CrossPlatform.PathCombine(hostContext.HostingEnvironment.ContentRootPath, path));

                    config.SetBasePath(path);


    config.AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                      .AddInMemoryCollection(new Dictionary<string, string> { { "pathToConf", path } });
});

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
                    }
                    else
                    {
                        diHelper.TryAdd(typeof(ICacheNotify<>), typeof(MemoryCacheNotify<>));
                    }

    services.AddHostedService<ClearEventsService>();
    diHelper.TryAdd<ClearEventsService>();
    diHelper.TryAdd<DbContextManager<MessagesContext>>();
});

builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
    builder.Register(context.Configuration, false, false));

builder.Host.ConfigureNLogLogging();

var app = builder.Build();

await app.RunAsync();