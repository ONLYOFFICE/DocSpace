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



using ASC.Common.Services;
using ASC.Common.Services.Interfaces;
using ASC.Core.Common.Services.Interfaces;

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
    private readonly IInstanceWorkerInfo<BackupSchedulerService> _instanceWorker;

    public BackupSchedulerService(
        IOptionsMonitor<ILog> options,
        IServiceScopeFactory scopeFactory,
        IInstanceWorkerInfo<BackupSchedulerService> instanceWorkerInfo,
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
        _instanceWorker = instanceWorkerInfo;
        _eventBus = eventBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Info("starting backup scheduler service...");

        while (!stoppingToken.IsCancellationRequested)
        {
            ExecuteBackupScheduler(stoppingToken);

            await Task.Delay(_backupSchedulerPeriod, stoppingToken);
        }

        _logger.Info("backup scheduler service started");
    }

    private async void ExecuteBackupScheduler(CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();

        var registerInstanceService = serviceScope.ServiceProvider.GetService<IRegisterInstanceService<BackupSchedulerService>>();

        if (!await registerInstanceService.IsActive(_instanceWorker.GetInstanceId())) return;

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
