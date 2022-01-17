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


using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Data.Backup.Storage;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Service
{
    [Scope]
    public class BackupSchedulerService : IHostedService, IDisposable
    {
        private readonly TimeSpan _period;
        private Timer _timer;
        private readonly ILog _logger;
        private readonly PaymentManager _paymentManager;
        private readonly BackupWorker _backupWorker;
        private readonly BackupRepository _backupRepository;
        private readonly Schedule _schedule;
        private readonly TenantManager _tenantManager;
        private readonly CoreBaseSettings _coreBaseSettings;

        public BackupSchedulerService(
            IOptionsMonitor<ILog> options,
            PaymentManager paymentManager,
            BackupWorker backupWorker,
            BackupRepository backupRepository,
            Schedule schedule,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            ConfigurationExtension configuration)
        {
            _paymentManager = paymentManager;
            _backupWorker = backupWorker;
            _backupRepository = backupRepository;
            _schedule = schedule;
            _tenantManager = tenantManager;
            _coreBaseSettings = coreBaseSettings;
            _logger = options.CurrentValue;
            _period = configuration.GetSetting<BackupSettings>("backup").Scheduler.Period;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("starting backup scheduler service...");

            _timer = new Timer(ScheduleBackupTasks, null, TimeSpan.Zero, _period);

            _logger.Info("backup scheduler service started");

            return Task.CompletedTask;
        }

        public void ScheduleBackupTasks(object state)
        {
            _logger.DebugFormat("started to schedule backups");

            var backupsToSchedule = _backupRepository.GetBackupSchedules().Where(schedule => _schedule.IsToBeProcessed(schedule)).ToList();

            _logger.DebugFormat("{0} backups are to schedule", backupsToSchedule.Count);

            foreach (var schedule in backupsToSchedule)
            {
                try
                {
                    if (_coreBaseSettings.Standalone || _tenantManager.GetTenantQuota(schedule.TenantId).AutoBackup)
                    {
                        var tariff = _paymentManager.GetTariff(schedule.TenantId);
                        if (tariff.State < TariffState.Delay)
                        {
                            schedule.LastBackupTime = DateTime.UtcNow;
                            
                            _backupRepository.SaveBackupSchedule(schedule);
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("stopping backup cleaner service...");

            _timer?.Change(Timeout.Infinite, 0);

            _logger.Info("backup cleaner service stopped");

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
