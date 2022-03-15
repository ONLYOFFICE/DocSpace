namespace ASC.Data.Backup;

public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        : base(configuration, hostEnvironment)
    {

    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        DIHelper.AddDistributedTaskQueueService<BaseBackupProgressItem>(1);

        DIHelper.TryAdd<BackupProgressItem>();
        DIHelper.TryAdd<RestoreProgressItem>();
        DIHelper.TryAdd<TransferProgressItem>();

        DIHelper.TryAdd<Schedule>();

        DIHelper.TryAdd<BackupCleanerService>();
        DIHelper.TryAdd<BackupSchedulerService>();
        DIHelper.TryAdd<BackupListenerService>();
        DIHelper.TryAdd<BackupWorkerService>();

        NotifyConfigurationExtension.Register(DIHelper);

        services.AddHostedService<BackupCleanerService>();
        services.AddHostedService<BackupSchedulerService>();
        services.AddHostedService<BackupListenerService>();
        services.AddHostedService<BackupWorkerService>();
    }
}
