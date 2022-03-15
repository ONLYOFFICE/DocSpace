namespace ASC.Data.Backup.Services;

[Singletone]
internal sealed class BackupWorkerService : IHostedService
{
    private readonly BackupWorker _backupWorker;
    private readonly ConfigurationExtension _configuration;
    private readonly NotifyConfiguration _notifyConfiguration;

    public BackupWorkerService(
        BackupWorker backupWorker,
        ConfigurationExtension configuration,
        NotifyConfiguration notifyConfiguration)
    {
        _backupWorker = backupWorker;
        _configuration = configuration;
        _notifyConfiguration = notifyConfiguration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _notifyConfiguration.Configure();

        var settings = _configuration.GetSetting<BackupSettings>("backup");

        _backupWorker.Start(settings);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _backupWorker.Stop();

        return Task.CompletedTask;
    }
}
