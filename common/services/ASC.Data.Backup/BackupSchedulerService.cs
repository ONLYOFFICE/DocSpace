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

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Data.Backup.Storage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Service
{
    [Scope]
    internal class BackupSchedulerServiceHelper
    {
        private ILog Log { get; }
        private PaymentManager PaymentManager { get; }
        private BackupWorker BackupWorker { get; }
        private BackupRepository BackupRepository { get; }
        private Schedule Schedule { get; }
        private TenantManager TenantManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }

        public BackupSchedulerServiceHelper(
            IOptionsMonitor<ILog> options,
            PaymentManager paymentManager,
            BackupWorker backupWorker,
            BackupRepository backupRepository,
            Schedule schedule,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings)
        {
            PaymentManager = paymentManager;
            BackupWorker = backupWorker;
            BackupRepository = backupRepository;
            Schedule = schedule;
            TenantManager = tenantManager;
            CoreBaseSettings = coreBaseSettings;
            Log = options.CurrentValue;
        }

        public void ScheduleBackupTasks(BackupSchedulerService backupSchedulerService)
        {
            Log.DebugFormat("started to schedule backups");
            var backupsToSchedule = BackupRepository.GetBackupSchedules().Where(schedule => Schedule.IsToBeProcessed(schedule)).ToList();
            Log.DebugFormat("{0} backups are to schedule", backupsToSchedule.Count);
            foreach (var schedule in backupsToSchedule)
            {
                if (!backupSchedulerService.IsStarted)
                {
                    return;
                }
                try
                {
                    if (CoreBaseSettings.Standalone || TenantManager.GetTenantQuota(schedule.TenantId).AutoBackup)
                    {
                        var tariff = PaymentManager.GetTariff(schedule.TenantId);
                        if (tariff.State < TariffState.Delay)
                        {
                            schedule.LastBackupTime = DateTime.UtcNow;
                            BackupRepository.SaveBackupSchedule(schedule);
                            Log.DebugFormat("Start scheduled backup: {0}, {1}, {2}, {3}", schedule.TenantId, schedule.BackupMail, schedule.StorageType, schedule.StorageBasePath);
                            BackupWorker.StartScheduledBackup(schedule);
                        }
                        else
                        {
                            Log.DebugFormat("Skip portal {0} not paid", schedule.TenantId);
                        }
                    }
                    else
                    {
                        Log.DebugFormat("Skip portal {0} haven't access", schedule.TenantId);
                    }
                }
                catch (Exception error)
                {
                    Log.Error("error while scheduling backups: {0}", error);
                }
            }
        }
    }

    [Singletone(Additional = typeof(BackupSchedulerServiceExtension))]
    public class BackupSchedulerService
    {
        private readonly object schedulerLock = new object();
        private Timer SchedulerTimer { get; set; }
        internal bool IsStarted { get; set; }
        public TimeSpan Period { get; set; }
        private ILog Log { get; }
        private IServiceProvider ServiceProvider { get; }

        public BackupSchedulerService(
            IOptionsMonitor<ILog> options,
            IServiceProvider serviceProvider)
        {
            Log = options.CurrentValue;
            Period = TimeSpan.FromMinutes(15);
            ServiceProvider = serviceProvider;
        }

        public void Start()
        {
            if (!IsStarted && Period > TimeSpan.Zero)
            {
                Log.Info("staring backup scheduler service...");
                SchedulerTimer = new Timer(_ => ScheduleBackupTasks(), null, TimeSpan.Zero, Period);
                Log.Info("backup scheduler service service started");
                IsStarted = true;
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                Log.Info("stoping backup scheduler service...");
                if (SchedulerTimer != null)
                {
                    SchedulerTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    SchedulerTimer.Dispose();
                    SchedulerTimer = null;
                }
                Log.Info("backup scheduler service stoped");
                IsStarted = false;
            }
        }

        private void ScheduleBackupTasks()
        {
            if (Monitor.TryEnter(schedulerLock))
            {
                try
                {
                    using var scope = ServiceProvider.CreateScope();
                    var backupSchedulerServiceHelper = scope.ServiceProvider.GetService<BackupSchedulerServiceHelper>();
                    backupSchedulerServiceHelper.ScheduleBackupTasks(this);
                }
                catch (Exception error)
                {
                    Log.Error("error while scheduling backups: {0}", error);
                }
                finally
                {
                    Monitor.Exit(schedulerLock);
                }
            }
        }
    }

    public class BackupSchedulerServiceExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<BackupSchedulerServiceHelper>();
        }
    }
}
