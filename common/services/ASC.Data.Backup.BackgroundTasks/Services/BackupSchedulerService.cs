/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

using ASC.Core.Common.Hosting;

namespace ASC.Data.Backup.Services;

[Singletone]
public sealed class BackupSchedulerService : BackgroundService
{
    private readonly TimeSpan _backupSchedulerPeriod;
    private readonly ILog _logger;

    private readonly BackupWorker _backupWorker;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBus _eventBus;

    public BackupSchedulerService(
        IOptionsMonitor<ILog> options,
        IServiceScopeFactory scopeFactory,
        ConfigurationExtension configuration,
        CoreBaseSettings coreBaseSettings,
        BackupWorker backupWorker,
        IEventBus eventBus)

    {
        _logger = options.CurrentValue;
        _coreBaseSettings = coreBaseSettings;
        _backupWorker = backupWorker;
        _backupSchedulerPeriod = configuration.GetSetting<BackupSettings>("backup").Scheduler.Period;
        _scopeFactory = scopeFactory;
        _eventBus = eventBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Debug("BackupSchedulerService is starting.");

        stoppingToken.Register(() => _logger.Debug("#1 BackupSchedulerService background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Debug("BackupSchedulerService background task is doing background work.");

            await ExecuteBackupScheduler(stoppingToken);

            await Task.Delay(_backupSchedulerPeriod, stoppingToken);
        }

        _logger.Debug("BackupSchedulerService background task is stopping.");
    }

    private async Task ExecuteBackupScheduler(CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();

        var registerInstanceService = serviceScope.ServiceProvider.GetService<IRegisterInstanceManager<BackupSchedulerService>>();

        if (!await registerInstanceService.IsActive(RegisterInstanceWorkerService<BackupSchedulerService>.InstanceId)) return;

        var paymentManager = serviceScope.ServiceProvider.GetRequiredService<PaymentManager>();
        var backupRepository = serviceScope.ServiceProvider.GetRequiredService<BackupRepository>(); ;
        var backupSchedule = serviceScope.ServiceProvider.GetRequiredService<Schedule>();
        var tenantManager = serviceScope.ServiceProvider.GetRequiredService<TenantManager>();
                
        _logger.DebugFormat("started to schedule backups");

        var backupsToSchedule = backupRepository.GetBackupSchedules().Where(schedule => backupSchedule.IsToBeProcessed(schedule)).ToList();

        _logger.DebugFormat("{0} backups are to schedule", backupsToSchedule.Count);

        foreach (var schedule in backupsToSchedule)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                if (_coreBaseSettings.Standalone || tenantManager.GetTenantQuota(schedule.TenantId).AutoBackup)
                {
                    var tariff = paymentManager.GetTariff(schedule.TenantId);

                    if (tariff.State < TariffState.Delay)
                    {
                        schedule.LastBackupTime = DateTime.UtcNow;

                        backupRepository.SaveBackupSchedule(schedule);

                        _logger.DebugFormat("Start scheduled backup: {0}, {1}, {2}, {3}", schedule.TenantId, schedule.BackupMail, schedule.StorageType, schedule.StorageBasePath);

                        _backupWorker.StartScheduledBackup(schedule);
                    }
                    else
                    {
                        _logger.DebugFormat("Skip portal {0} not paid", schedule.TenantId);
                    }
                }
                else
                {
                    _logger.DebugFormat("Skip portal {0} haven't access", schedule.TenantId);
                }
            }
            catch (Exception error)
            {
                _logger.Error("error while scheduling backups: {0}", error);
            }
        }
    }
}
