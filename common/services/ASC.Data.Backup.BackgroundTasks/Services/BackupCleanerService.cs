// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.Data.Backup.Services;

[Singletone]
internal sealed class BackupCleanerService : BackgroundService
{
    private readonly ILogger<BackupCleanerService> _logger;
    private readonly TimeSpan _backupCleanerPeriod;
    private readonly IServiceScopeFactory _scopeFactory;

    public BackupCleanerService(
        ConfigurationExtension configuration,
        ILogger<BackupCleanerService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _backupCleanerPeriod = configuration.GetSetting<BackupSettings>("backup").Cleaner.Period;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.DebugBackupCleanerServiceStarting();

        stoppingToken.Register(() => _logger.DebugBackupCleanerServiceStopping());

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.DebugBackupCleanerServiceDoingWork();

            await using var serviceScope = _scopeFactory.CreateAsyncScope();

            var registerInstanceService = serviceScope.ServiceProvider.GetService<IRegisterInstanceManager<BackupCleanerService>>();

            if (!await registerInstanceService.IsActive(RegisterInstanceWorkerService<BackupCleanerService>.InstanceId))
            {
                _logger.DebugBackupCleanerServiceIsNotActive(RegisterInstanceWorkerService<BackupCleanerService>.InstanceId);

                await Task.Delay(1000, stoppingToken);

                continue;
            }

            await ExecuteBackupCleanerAsync(stoppingToken);

            await Task.Delay(_backupCleanerPeriod, stoppingToken);
        }

        _logger.DebugBackupCleanerServiceStopping();
    }

    private async Task ExecuteBackupCleanerAsync(CancellationToken stoppingToken)
    {
        await using var serviceScope = _scopeFactory.CreateAsyncScope();

        var backupRepository = serviceScope.ServiceProvider.GetRequiredService<BackupRepository>();
        var backupStorageFactory = serviceScope.ServiceProvider.GetRequiredService<BackupStorageFactory>();

        _logger.DebugStartedClean();

        var backupsToRemove = await backupRepository.GetExpiredBackupRecordsAsync();

        _logger.DebugFoundBackups(backupsToRemove.Count);

        foreach (var scheduledBackups in (await backupRepository.GetScheduledBackupRecordsAsync()).GroupBy(r => r.TenantId))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            var schedule = await backupRepository.GetBackupScheduleAsync(scheduledBackups.Key);

            if (schedule != null)
            {
                var scheduledBackupsToRemove = scheduledBackups.OrderByDescending(r => r.CreatedOn).Skip(schedule.BackupsStored).ToList();
                if (scheduledBackupsToRemove.Any())
                {
                    _logger.DebugOnlyLast(schedule.BackupsStored, schedule.TenantId, scheduledBackupsToRemove.Count);
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
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                var backupStorage = await backupStorageFactory.GetBackupStorageAsync(backupRecord);
                if (backupStorage == null)
                {
                    continue;
                }

                await backupStorage.DeleteAsync(backupRecord.StoragePath);

                await backupRepository.DeleteBackupRecordAsync(backupRecord.Id);
            }
            catch (ProviderInfoArgumentException error)
            {
                _logger.WarningCanNotRemoveBackup(backupRecord.Id, error);

                if (DateTime.UtcNow > backupRecord.CreatedOn.AddMonths(6))
                {
                    await backupRepository.DeleteBackupRecordAsync(backupRecord.Id);
                }
            }
            catch (Exception error)
            {
                _logger.WarningCanNotRemoveBackup(backupRecord.Id, error);
            }
        }

    }
}
