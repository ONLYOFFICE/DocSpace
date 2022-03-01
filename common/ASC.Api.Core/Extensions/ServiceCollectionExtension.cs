using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core.Common.Hosting.Interfaces;
using ASC.Core.Common.Hosting;
using ASC.EventBus;
using ASC.EventBus.Abstractions;
using ASC.EventBus.RabbitMQ;

using Autofac;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using ASC.Common;
using ASC.EventBus.MemoryCache;

namespace ASC.Api.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
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

                    var subscriptionClientName = "backup";

                    if (!string.IsNullOrEmpty(cfg["core:eventBus:subscriptionClientName"]))
                    {
                        subscriptionClientName = cfg["core:eventBus:subscriptionClientName"];
                    }

                    var retryCount = 5;

                    if (!string.IsNullOrEmpty(cfg["core:eventBus:connectRetryCount"]))
                    {
                        retryCount = int.Parse(cfg["core:eventBus:connectRetryCount"]);
                    }

                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                });
            }
            else
            {
                services.AddSingleton<IEventBus, EventBusMemoryCache>(sp =>
                {
                    var cfg = sp.GetRequiredService<IConfiguration>();

                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<IOptionsMonitor<ILog>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                   
                    return new EventBusMemoryCache(logger, iLifetimeScope, eventBusSubcriptionsManager);
                });
            }

        }

        public static void AddActivePassiveHostedService<T>(this IServiceCollection services) where T : class, IHostedService
        {
            var diHelper = new DIHelper(services);

            diHelper.TryAdd<IRegisterInstanceDao<T>, RegisterInstanceDao<T>>();
            diHelper.TryAdd<IRegisterInstanceManager<T>, RegisterInstanceManager<T>>();

            services.AddHostedService<RegisterInstanceWorkerService<T>>();

            diHelper.TryAdd<T>();
            services.AddHostedService<T>();

        }
    }
}
