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
using ASC.Data.Backup.Storage;
using ASC.Files.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Service
{
    [Scope]
    internal class BackupCleanerHelperService
    {
        private readonly ILog log;

        private BackupRepository BackupRepository { get; }
        private BackupStorageFactory BackupStorageFactory { get; }

        public BackupCleanerHelperService(
            IOptionsMonitor<ILog> options,
            BackupRepository backupRepository,
            BackupStorageFactory backupStorageFactory)
        {
            log = options.CurrentValue;
            BackupRepository = backupRepository;
            BackupStorageFactory = backupStorageFactory;
        }


        internal void DeleteExpiredBackups(BackupCleanerService backupCleanerService)
        {
            log.Debug("started to clean expired backups");

            var backupsToRemove = BackupRepository.GetExpiredBackupRecords();
            log.DebugFormat("found {0} backups which are expired", backupsToRemove.Count);

            if (!backupCleanerService.IsStarted) return;
            foreach (var scheduledBackups in BackupRepository.GetScheduledBackupRecords().GroupBy(r => r.TenantId))
            {
                if (!backupCleanerService.IsStarted) return;
                var schedule = BackupRepository.GetBackupSchedule(scheduledBackups.Key);
                if (schedule != null)
                {
                    var scheduledBackupsToRemove = scheduledBackups.OrderByDescending(r => r.CreatedOn).Skip(schedule.BackupsStored).ToList();
                    if (scheduledBackupsToRemove.Any())
                    {
                        log.DebugFormat("only last {0} scheduled backup records are to keep for tenant {1} so {2} records must be removed", schedule.BackupsStored, schedule.TenantId, scheduledBackupsToRemove.Count);
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
                if (!backupCleanerService.IsStarted) return;
                try
                {
                    var backupStorage = BackupStorageFactory.GetBackupStorage(backupRecord);
                    if (backupStorage == null) continue;

                    backupStorage.Delete(backupRecord.StoragePath);

                    BackupRepository.DeleteBackupRecord(backupRecord.Id);
                }
                catch (ProviderInfoArgumentException error)
                {
                    log.Warn("can't remove backup record " + backupRecord.Id, error);
                    if (DateTime.UtcNow > backupRecord.CreatedOn.AddMonths(6))
                    {
                        BackupRepository.DeleteBackupRecord(backupRecord.Id);
                    }
                }
                catch (Exception error)
                {
                    log.Warn("can't remove backup record: " + backupRecord.Id, error);
                }
            }
        }
    }

    [Singletone(Additional = typeof(BackupCleanerServiceExtension))]
    public class BackupCleanerService
    {
        private readonly object cleanerLock = new object();
        private Timer CleanTimer { get; set; }
        internal bool IsStarted { get; set; }
        private ILog Log { get; set; }
        public TimeSpan Period { get; set; }
        private IServiceProvider ServiceProvider { get; set; }

        public BackupCleanerService(
            IOptionsMonitor<ILog> options,
            IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Log = options.CurrentValue;
            Period = TimeSpan.FromMinutes(15);
        }


        public void Start()
        {
            if (!IsStarted && Period > TimeSpan.Zero)
            {
                Log.Info("starting backup cleaner service...");
                CleanTimer = new Timer(_ => DeleteExpiredBackups(), null, TimeSpan.Zero, Period);
                Log.Info("backup cleaner service started");
                IsStarted = true;
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                Log.Info("stopping backup cleaner service...");
                if (CleanTimer != null)
                {
                    CleanTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    CleanTimer.Dispose();
                    CleanTimer = null;
                }
                Log.Info("backup cleaner service stopped");
                IsStarted = false;
            }
        }

        private void DeleteExpiredBackups()
        {
            if (Monitor.TryEnter(cleanerLock))
            {
                try
                {

                    using var scope = ServiceProvider.CreateScope();
                    var backupCleanerHelperService = scope.ServiceProvider.GetService<BackupCleanerHelperService>();
                    backupCleanerHelperService.DeleteExpiredBackups(this);
                }
                catch (Exception error)
                {
                    Log.Error("error while cleaning expired backup records: {0}", error);
                }
                finally
                {
                    Monitor.Exit(cleanerLock);
                }
            }
        }
    }

    public class BackupCleanerServiceExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<BackupCleanerHelperService>();
        }
    }
}
