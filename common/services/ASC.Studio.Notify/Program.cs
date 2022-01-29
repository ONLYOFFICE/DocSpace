namespace ASC.Studio.Notify
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .UseWindowsService()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<BaseWorkerStartup>())
                .ConfigureAppConfiguration((hostContext, config) =>
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
                        .AddJsonFile("appsettings.json")
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
                            }
                        );
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMemoryCache();
                    var diHelper = new DIHelper(services);

                    var redisConfiguration = hostContext.Configuration.GetSection("Redis").Get<RedisConfiguration>();

                    if (redisConfiguration != null)
                    {
                        diHelper.TryAdd(typeof(ICacheNotify<>), typeof(RedisCache<>));

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

                })
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.Register(context.Configuration);
                })
            .ConfigureNLogLogging();
    }
}
