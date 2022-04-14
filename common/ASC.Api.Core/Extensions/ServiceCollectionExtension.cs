namespace ASC.Api.Core.Extensions;
public static class ServiceCollectionExtension
{
    public static void AddCacheNotify(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection("Redis").Get<RedisConfiguration>();
        var kafkaConfiguration = configuration.GetSection("kafka").Get<KafkaSettings>();
        var rabbitMQConfiguration = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

        if (redisConfiguration != null)
        {
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);

            services.AddSingleton(typeof(ICacheNotify<>), typeof(RedisCacheNotify<>));
        }
        else if (rabbitMQConfiguration != null)
        {
            services.AddSingleton(typeof(ICacheNotify<>), typeof(RabbitMQCache<>));
        }
        else if (kafkaConfiguration != null)
        {
            services.AddSingleton(typeof(ICacheNotify<>), typeof(KafkaCacheNotify<>));
        }
        else
        {
            services.AddSingleton(typeof(ICacheNotify<>), typeof(MemoryCacheNotify<>));
        }
    }

    public static void AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection("Redis").Get<RedisConfiguration>();

        if (redisConfiguration != null)
        {
            services.AddStackExchangeRedisCache(config =>
            {
                config.ConfigurationOptions = redisConfiguration.ConfigurationOptions;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }
    }

    public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        var rabbitMQConfiguration = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

        if (rabbitMQConfiguration != null)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                var settings = cfg.GetSection("RabbitMQ").Get<RabbitMQSettings>();

                var logger = sp.GetRequiredService<IOptionsMonitor<ILog>>();

                var factory = new ConnectionFactory()
                {
                    HostName = settings.HostName,
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(settings.UserName))
                {
                    factory.UserName = settings.UserName;
                }

                if (!string.IsNullOrEmpty(settings.Password))
                {
                    factory.Password = settings.Password;
                }

                var retryCount = 5;

                if (!string.IsNullOrEmpty(cfg["core:eventBus:connectRetryCount"]))
                {
                    retryCount = int.Parse(cfg["core:eventBus:connectRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();

                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<IOptionsMonitor<ILog>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var serializer = new ASC.EventBus.Serializers.ProtobufSerializer();

                var subscriptionClientName = "asc_event_bus_default_queue";

                if (!string.IsNullOrEmpty(cfg["core:eventBus:subscriptionClientName"]))
                {
                    subscriptionClientName = cfg["core:eventBus:subscriptionClientName"];
                }

                var retryCount = 5;

                if (!string.IsNullOrEmpty(cfg["core:eventBus:connectRetryCount"]))
                {
                    retryCount = int.Parse(cfg["core:eventBus:connectRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, serializer, subscriptionClientName, retryCount);
            });
        }
        else
        {
            throw new NotImplementedException("EventBus: Provider not found.");
        }
    }

    /// <remarks>
    /// Add a IHostedService for given type. 
    /// Only one copy of this instance type will active in multi process architecture.
    /// </remarks>
    public static void AddActivePassiveHostedService<T>(this IServiceCollection services) where T : class, IHostedService
    {
        var diHelper = new DIHelper(services);

        diHelper.TryAdd<IRegisterInstanceDao<T>, RegisterInstanceDao<T>>();
        diHelper.TryAdd<IRegisterInstanceManager<T>, RegisterInstanceManager<T>>();

        services.AddHostedService<RegisterInstanceWorkerService<T>>();

        diHelper.TryAdd<T>();
        services.AddHostedService<T>();

    }

    public static void AddDistributedTaskQueue(this IServiceCollection services)
    {
        services.AddTransient<DistributedTaskQueue>();

        services.AddSingleton<IDistributedTaskQueueFactory, DefaultDistributedTaskQueueFactory>();
    }
}
