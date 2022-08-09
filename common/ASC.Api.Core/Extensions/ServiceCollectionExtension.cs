﻿// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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

                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

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
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var serializer = new EventBus.Serializers.ProtobufSerializer();

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

    public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
                                    where T : class, IStartupTask
    {
        services.AddTransient<IStartupTask, T>();

        return services;
    }
}
