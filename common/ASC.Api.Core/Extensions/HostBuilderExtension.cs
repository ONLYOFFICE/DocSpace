﻿namespace ASC.Api.Core.Extensions;

public static class HostBuilderExtension
{
    public static IHostBuilder ConfigureDefault(this IHostBuilder hostBuilder, string[] args,
        Action<HostBuilderContext, IConfigurationBuilder, IHostEnvironment, string> configureApp = null,
        Action<HostBuilderContext, IServiceCollection, DIHelper> configureServices = null)
    {
        hostBuilder.UseSystemd();
        hostBuilder.UseWindowsService();
        hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        hostBuilder.ConfigureNLogLogging();
        hostBuilder.ConfigureDefaultAppConfiguration(args, configureApp);
        hostBuilder.ConfigureDefaultServices(configureServices);

        return hostBuilder;
    }

    public static IHostBuilder ConfigureDefaultAppConfiguration(this IHostBuilder hostBuilder, string[] args, Action<HostBuilderContext, IConfigurationBuilder, IHostEnvironment, string> configureDelegate = null)
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
            config.AddJsonFile("appsettings.json")
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                  .AddJsonFile("storage.json")
                  .AddJsonFile("kafka.json")
                  .AddJsonFile($"kafka.{env.EnvironmentName}.json", true)
                  .AddJsonFile("rabbitmq.json")
                  .AddJsonFile($"rabbitmq.{env.EnvironmentName}.json", true)
                  .AddJsonFile("redis.json")
                  .AddJsonFile($"redis.{env.EnvironmentName}.json", true);

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

    public static IHostBuilder ConfigureDefaultServices(this IHostBuilder hostBuilder, Action<HostBuilderContext, IServiceCollection, DIHelper> configureDelegate)
    {
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddMemoryCache();

            var diHelper = new DIHelper(services);

            var redisConfiguration = hostContext.Configuration.GetSection("Redis").Get<RedisConfiguration>();
            var kafkaConfiguration = hostContext.Configuration.GetSection("kafka").Get<KafkaSettings>();
            var rabbitMQConfiguration = hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

            if (kafkaConfiguration != null)
            {
                diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCacheNotify<>));
            }
            else if (rabbitMQConfiguration != null)
            {
                diHelper.TryAdd(typeof(ICacheNotify<>), typeof(RabbitMQCache<>));
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

            configureDelegate?.Invoke(hostContext, services, diHelper);
        });

        return hostBuilder;
    }
}