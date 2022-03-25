namespace ASC.Common.Threading;
public static class DistributedTaskQueueExtention
{
    public static DIHelper AddDistributedTaskQueue(this DIHelper services, string name, Action<DistributedTaskQueueFactoryOptions> options)
    {
        services.TryAdd<DistributedTaskQueue>();

        services.Configure<DistributedTaskQueueFactoryOptions>(name, options);

        services.TryAddSingleton<IDistributedTaskQueueFactory, DefaultDistributedTaskQueueFactory>();

        return services;
        
    }
    public static DIHelper AddDistributedTaskQueue<T>(this DIHelper services, int maxThreadsCount) where T : DistributedTask
    {
        var type = typeof(T);

        if (!type.IsAbstract)
        {
            services.TryAdd<T>();
        }

        services.AddDistributedTaskQueue(type.FullName, x =>
        {
            x.MaxThreadsCount = maxThreadsCount;
        });

        return services;
    }
}
