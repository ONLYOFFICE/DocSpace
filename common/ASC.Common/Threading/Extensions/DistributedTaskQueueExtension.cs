namespace ASC.Common.Threading;
public static class DistributedTaskQueueExtention
{
    public static DIHelper AddDistributedTaskQueueService<T>(this DIHelper services, int maxThreadsCount) where T : DistributedTask
    {
        services.TryAdd<DistributedTaskQueue>();

        var type = typeof(T);

        if (!type.IsAbstract)
        {
            services.TryAdd<T>();
        }

        services.Configure<DistributedTaskQueueFactoryOptions>(type.FullName, r =>
        {
            r.MaxThreadsCount = maxThreadsCount;
        });

        services.TryAddSingleton<IDistributedTaskQueueFactory, DefaultDistributedTaskQueueFactory>();

        return services;
    }
}
