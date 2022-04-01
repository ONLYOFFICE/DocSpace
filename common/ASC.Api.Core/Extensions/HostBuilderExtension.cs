namespace ASC.Api.Core.Extensions;

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

            services.AddDistributedCache(hostContext.Configuration);
            services.AddEventBus(hostContext.Configuration);
            services.AddDistributedTaskQueue();
            services.AddCacheNotify(hostContext.Configuration);

            var diHelper = new DIHelper(services);

            configureDelegate?.Invoke(hostContext, services, diHelper);
        });

        return hostBuilder;
    }
}