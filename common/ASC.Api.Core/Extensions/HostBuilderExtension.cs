namespace ASC.Api.Core.Extensions;

public static class HostBuilderExtension
{
    public static IHostBuilder ConfigureBaseAppConfiguration(this IHostBuilder hostBuilder, string[] args, Action<HostBuilderContext, IConfigurationBuilder, IHostEnvironment, string> configureDelegate = null)
    {
        hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
        {
            var buildedConfig = config.Build();

            var path = buildedConfig["pathToConf"];

            if (!Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(CrossPlatform.PathCombine(hostContext.HostingEnvironment.ContentRootPath, path));
            }

            var env = hostContext.HostingEnvironment;

            config.SetBasePath(path);

            config.AddJsonFile("appsettings.json");

            configureDelegate?.Invoke(hostContext, config, env, path);

            config.AddEnvironmentVariables()
                  .AddCommandLine(args)
                  .AddInMemoryCollection(new Dictionary<string, string>
                  {
                      {"pathToConf", path }
                  });
        });

        return hostBuilder;
    }

    public static IHostBuilder ConfigureDefaultAppConfiguration(this IHostBuilder hostBuilder, string[] args, Action<HostBuilderContext, IConfigurationBuilder, IHostEnvironment, string> configureDelegate = null)
    {
        hostBuilder.ConfigureBaseAppConfiguration(args, (hostContext, config, env, path) =>
        {
            config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                  .AddJsonFile("storage.json")
                  .AddJsonFile("kafka.json")
                  .AddJsonFile($"kafka.{env.EnvironmentName}.json", true)
                  .AddJsonFile("redis.json")
                  .AddJsonFile($"redis.{env.EnvironmentName}.json", true);

            configureDelegate?.Invoke(hostContext, config, env, path);
        });

        return hostBuilder;
    }

    public static IHostBuilder ConfigureDefaultServices(this IHostBuilder hostBuilder, Action<HostBuilderContext, IServiceCollection, DIHelper> configureDelegate)
    {
        hostBuilder.ConfigureServices((hostContext, services) =>
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

            configureDelegate?.Invoke(hostContext, services, diHelper);
        });

        return hostBuilder;
    }
}