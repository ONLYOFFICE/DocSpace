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
public sealed class BackupSchedulerService : BackgroundService
{
    private readonly TimeSpan _backupSchedulerPeriod;
    private readonly ILog _logger;

    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBus _eventBus;

    public BackupSchedulerService(
        ILog<BackupSchedulerService> logger,
        IServiceScopeFactory scopeFactory,
        ConfigurationExtension configuration,
        CoreBaseSettings coreBaseSettings,
        IEventBus eventBus)
    {
        _logger = logger;
        _coreBaseSettings = coreBaseSettings;
        _backupSchedulerPeriod = configuration.GetSetting<BackupSettings>("backup").Scheduler.Period;
        _scopeFactory = scopeFactory;
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Debug("BackupSchedulerService is starting.");

        stoppingToken.Register(() => _logger.Debug("#1 BackupSchedulerService background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            using var serviceScope = _scopeFactory.CreateScope();

            var registerInstanceService = serviceScope.ServiceProvider.GetService<IRegisterInstanceManager<BackupSchedulerService>>();

            if (!await registerInstanceService.IsActive(RegisterInstanceWorkerService<BackupSchedulerService>.InstanceId))
            {
                _logger.Debug($"BackupSchedulerService background task with instance id {RegisterInstanceWorkerService<BackupSchedulerService>.InstanceId} is't active.");

                await Task.Delay(1000, stoppingToken);

                continue;
            }

            _logger.Debug("BackupSchedulerService background task is doing background work.");

            ExecuteBackupScheduler(stoppingToken);

            await Task.Delay(_backupSchedulerPeriod, stoppingToken);
        }

        _logger.Debug("BackupSchedulerService background task is stopping.");
    }

    private void ExecuteBackupScheduler(CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();

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

                        _eventBus.Publish(new BackupRequestIntegrationEvent(
                                                 tenantId: schedule.TenantId,
                                                 storageBasePath: schedule.StorageBasePath,
                                                 storageParams: JsonConvert.DeserializeObject<Dictionary<string,string>>(schedule.StorageParams),
                                                 storageType: schedule.StorageType,
                                                 backupMail: schedule.BackupMail,
                                                 createBy: ASC.Core.Configuration.Constants.CoreSystem.ID,
                                                 isScheduled: true,
                                                 backupsStored: schedule.BackupsStored                                                
                                          ));
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
