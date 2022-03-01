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
        .AddJsonFile("elastic.json", true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            {"pathToConf", path }
        });
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

    if (!bool.TryParse(hostContext.Configuration["disable_elastic"], out var disableElastic))
    {
        disableElastic = false;
    }

    if (!disableElastic)
    {
        services.AddHostedService<ElasticSearchIndexService>();
        diHelper.TryAdd<FactoryIndexer>();
        diHelper.TryAdd<ElasticSearchService>();
        //diHelper.TryAdd<FileConverter>();
        diHelper.TryAdd<FactoryIndexerFile>();
        diHelper.TryAdd<FactoryIndexerFolder>();
    }

    services.AddHostedService<FeedAggregatorService>();
    diHelper.TryAdd<FeedAggregatorService>();

    services.AddHostedService<FeedCleanerService>();
    diHelper.TryAdd<FeedCleanerService>();

    diHelper.TryAdd<FileDataQueue>();

    services.AddHostedService<ThumbnailService>();
    diHelper.TryAdd<ThumbnailService>();

    services.AddHostedService<ThumbnailBuilderService>();
    diHelper.TryAdd<ThumbnailBuilderService>();

    diHelper.TryAdd<AuthManager>();
    diHelper.TryAdd<BaseCommonLinkUtility>();
    diHelper.TryAdd<FeedAggregateDataProvider>();
    diHelper.TryAdd<SecurityContext>();
    diHelper.TryAdd<TenantManager>();
    diHelper.TryAdd<UserManager>();
});

builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
{
    builder.Register(context.Configuration, true, false, "search.json", "feed.json");
});

builder.Host.ConfigureNLogLogging();

var startup = new BaseWorkerStartup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

await app.RunAsync();