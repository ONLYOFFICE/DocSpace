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
internal sealed class BackupCleanerService : BackgroundService
{
    private readonly ILog _logger;
    private readonly TimeSpan _backupCleanerPeriod;
    private readonly IServiceScopeFactory _scopeFactory;

    public BackupCleanerService(
        ConfigurationExtension configuration,
        IOptionsMonitor<ILog> options,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = options.CurrentValue;
        _backupCleanerPeriod = configuration.GetSetting<BackupSettings>("backup").Cleaner.Period;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Debug("BackupCleanerService is starting.");

        stoppingToken.Register(() => _logger.Debug("#1 BackupCleanerService background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Debug("BackupCleanerService background task is doing background work.");

            await ExecuteBackupCleaner(stoppingToken);

            await Task.Delay(_backupCleanerPeriod, stoppingToken);
        }

        _logger.Debug("BackupCleanerService background task is stopping.");
    }

    private async Task ExecuteBackupCleaner(CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();

        var registerInstanceService = serviceScope.ServiceProvider.GetService<IRegisterInstanceManager<BackupSchedulerService>>();

        if (!await registerInstanceService.IsActive(RegisterInstanceWorkerService<BackupCleanerService>.InstanceId)) return;

        var backupRepository = serviceScope.ServiceProvider.GetRequiredService<BackupRepository>();
        var backupStorageFactory = serviceScope.ServiceProvider.GetRequiredService<BackupStorageFactory>();

        _logger.Debug("started to clean expired backups");

        var backupsToRemove = backupRepository.GetExpiredBackupRecords();

        _logger.DebugFormat("found {0} backups which are expired", backupsToRemove.Count);

        foreach (var scheduledBackups in backupRepository.GetScheduledBackupRecords().GroupBy(r => r.TenantId))
        {
            if (stoppingToken.IsCancellationRequested) return;

            var schedule = backupRepository.GetBackupSchedule(scheduledBackups.Key);

            if (schedule != null)
            {
                var scheduledBackupsToRemove = scheduledBackups.OrderByDescending(r => r.CreatedOn).Skip(schedule.BackupsStored).ToList();
                if (scheduledBackupsToRemove.Any())
                {
                    _logger.DebugFormat("only last {0} scheduled backup records are to keep for tenant {1} so {2} records must be removed", schedule.BackupsStored, schedule.TenantId, scheduledBackupsToRemove.Count);
                    backupsToRemove.AddRange(scheduledBackupsToRemove);
                }
            }
            else
            {
                backupsToRemove.AddRange(scheduledBackups);
            }
        }

        foreach (var backupRecord in backupsToRemove)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                var backupStorage = backupStorageFactory.GetBackupStorage(backupRecord);
                if (backupStorage == null) continue;

                backupStorage.Delete(backupRecord.StoragePath);

                backupRepository.DeleteBackupRecord(backupRecord.Id);
            }
            catch (ProviderInfoArgumentException error)
            {
                _logger.Warn("can't remove backup record " + backupRecord.Id, error);

                if (DateTime.UtcNow > backupRecord.CreatedOn.AddMonths(6))
                {
                    backupRepository.DeleteBackupRecord(backupRecord.Id);
                }
            }
            catch (Exception error)
            {
                _logger.Warn("can't remove backup record: " + backupRecord.Id, error);
            }
        }

    }
}
