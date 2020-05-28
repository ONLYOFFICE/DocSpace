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
using System.Collections.Generic;
using System.Extensions;
using System.IO;
using System.Linq;
using System.Threading;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Contracts;
using ASC.Core.Tenants;
using ASC.Data.Backup.EF.Context;
using ASC.Data.Backup.EF.Model;
using ASC.Data.Backup.Storage;
using ASC.Data.Backup.Tasks;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Backup.Utils;
using ASC.Data.Storage;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Service
{
    public class BackupWorker
    {
        private  readonly ILog Log;
        private ProgressQueue<BackupProgressItem> backupProgressQueue;
        private ProgressQueue<ScheduledProgressItem> scheduledProgressQueue;
        private ProgressQueue<RestoreProgressItem> restoreProgressQueue;
        private ProgressQueue<TransferProgressItem> transferProgressQueue;
        internal  string TempFolder;
        private  string currentRegion;
        private  Dictionary<string, string> configPaths;
        private  int limit;
        private  string upgradesPath;
        private TenantManager tenantManager;
        private BackupStorageFactory backupStorageFactory;
        private NotifyHelper notifyHelper;
        private CoreBaseSettings coreBaseSettings;
        private IOptionsMonitor<ILog> options;
        private StorageFactory storageFactory;
        private StorageFactoryConfig storageFactoryConfig;
        private LicenseReader licenseReader;
        private AscCacheNotify ascCacheNotify;//here
        private ModuleProvider moduleProvider; 
        private BackupsContext backupRecordContext;
        public BackupWorker(IOptionsMonitor<ILog> options, ProgressQueue<BackupProgressItem> backupProgressQueue, ProgressQueue<ScheduledProgressItem> scheduledProgressQueue, ProgressQueue<RestoreProgressItem> restoreProgressQueue, ProgressQueue<TransferProgressItem> transferProgressQueue, BackupStorageFactory backupStorageFactory, NotifyHelper notifyHelper, TenantManager tenantManager, CoreBaseSettings coreBaseSettings, StorageFactory storageFactory, StorageFactoryConfig storageFactoryConfig, LicenseReader licenseReader, AscCacheNotify ascCacheNotify, ModuleProvider moduleProvider, BackupsContext backupRecordContext)
        {
            Log = options.CurrentValue;
            
            this.tenantManager = tenantManager;
            this.backupStorageFactory = backupStorageFactory;
            this.notifyHelper = notifyHelper;
            this.coreBaseSettings = coreBaseSettings;
            this.storageFactory = storageFactory;
            this.storageFactoryConfig = storageFactoryConfig;
            this.licenseReader = licenseReader;
            this.ascCacheNotify = ascCacheNotify;
            this.moduleProvider = moduleProvider;
            this.backupRecordContext = backupRecordContext;
            this.backupProgressQueue = backupProgressQueue;
            this.scheduledProgressQueue = scheduledProgressQueue;
            this.restoreProgressQueue = restoreProgressQueue;
            this.transferProgressQueue = transferProgressQueue;
        }
        public  void Start(BackupConfigurationSection config)
        {
            TempFolder = PathHelper.ToRootedPath(config.TempFolder);
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }

            limit = config.Limit;
            upgradesPath = config.UpgradesPath;
            currentRegion = config.WebConfigs.CurrentRegion;
            configPaths = config.WebConfigs.Cast<WebConfigElement>().ToDictionary(el => el.Region, el => PathHelper.ToRootedConfigPath(el.Path));
            configPaths[currentRegion] = PathHelper.ToRootedConfigPath(config.WebConfigs.CurrentPath);
           
            var invalidConfigPath = configPaths.Values.FirstOrDefault(path => !File.Exists(path));
            if (invalidConfigPath != null)
            {
                Log.WarnFormat("Configuration file {0} not found", invalidConfigPath);
            }
        }

        public  void Stop()
        {
            if (backupProgressQueue != null)
            {
                backupProgressQueue.Terminate();
                backupProgressQueue = null;
            }
            if (scheduledProgressQueue != null)
            {
                scheduledProgressQueue.Terminate();
                scheduledProgressQueue = null;
            }
            if (restoreProgressQueue != null)
            {
                restoreProgressQueue.Terminate();
                restoreProgressQueue = null;
            }
            if (transferProgressQueue != null)
            {
                transferProgressQueue.Terminate();
                transferProgressQueue = null;
            }
        }

        public  BackupProgress StartBackup(StartBackupRequest request)
        {
            lock (backupProgressQueue.SynchRoot)
            {
                var item = backupProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
                if (item != null && item.IsCompleted)
                {
                    backupProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new BackupProgressItem(false, request.TenantId, request.UserId, request.StorageType, request.StorageBasePath, backupStorageFactory, this, notifyHelper, tenantManager, coreBaseSettings, backupRecordContext) { BackupMail = request.BackupMail, StorageParams = request.StorageParams };
                    backupProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public  void StartScheduledBackup(Schedule schedule)
        {
            lock (scheduledProgressQueue.SynchRoot)
            {
                var item = scheduledProgressQueue.GetItems().OfType<ScheduledProgressItem>().FirstOrDefault(t => t.TenantId == schedule.TenantId);
                if (item != null && item.IsCompleted)
                {
                    scheduledProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new ScheduledProgressItem(schedule.TenantId, Guid.Empty, schedule.StorageType, schedule.StorageBasePath, backupStorageFactory, this, notifyHelper, tenantManager, coreBaseSettings, backupRecordContext) { BackupMail = schedule.BackupMail, StorageParams = schedule.StorageParams };
                    scheduledProgressQueue.Add(item);
                }
            }
        }

        public  BackupProgress GetBackupProgress(int tenantId)
        {
            lock (backupProgressQueue.SynchRoot)
            {
                return ToBackupProgress(backupProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
            }
        }

        public  void ResetBackupError(int tenantId)
        {
            lock (backupProgressQueue.SynchRoot)
            {
                var progress = backupProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Error = null;
                }
            }
        }

        public  void ResetRestoreError(int tenantId)
        {
            lock (restoreProgressQueue.SynchRoot)
            {
                var progress = restoreProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Error = null;
                }
            }
        }

        public  BackupProgress StartRestore(StartRestoreRequest request)
        {
            lock (restoreProgressQueue.SynchRoot)
            {
                var item = restoreProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
                if (item != null && item.IsCompleted)
                {
                    restoreProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new RestoreProgressItem(request.TenantId, request.StorageType, request.FilePathOrId, request.NotifyAfterCompletion, backupStorageFactory, this) { StorageParams = request.StorageParams };
                    restoreProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public  BackupProgress GetRestoreProgress(int tenantId)
        {
            lock (restoreProgressQueue.SynchRoot)
            {
                return ToBackupProgress(restoreProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
            }
        }

        public  BackupProgress StartTransfer(int tenantId, string targetRegion, bool transferMail, bool notify)
        {
            lock (transferProgressQueue.SynchRoot)
            {
                var item = transferProgressQueue.GetItems().OfType<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (item != null && item.IsCompleted)
                {
                    transferProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new TransferProgressItem(tenantId, targetRegion, transferMail, notify, this);
                    transferProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public  BackupProgress GetTransferProgress(int tenantId)
        {
            lock (transferProgressQueue.SynchRoot)
            {
                return ToBackupProgress(transferProgressQueue.GetItems().OfType<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
            }
        }

        private BackupProgress ToBackupProgress(IProgressItem progressItem)
        {
            if (progressItem == null)
            {
                return null;
            }

            var progress = new BackupProgress
            {
                IsCompleted = progressItem.IsCompleted,
                Progress = (int)progressItem.Percentage,
                Error = progressItem.Error != null ? ((Exception)progressItem.Error).Message : null
            };

            var backupProgressItem = progressItem as BackupProgressItem;
            if (backupProgressItem != null)
            {
                progress.Link = backupProgressItem.Link;
            }
            else
            {
                var transferProgressItem = progressItem as TransferProgressItem;
                if (transferProgressItem != null)
                {
                    progress.Link = transferProgressItem.Link;
                }
            }

            return progress;
        }

        public class BackupProgressItem : IProgressItem
        {
            private const string ArchiveFormat = "tar.gz";
            private bool IsScheduled { get; set; }
            public int TenantId { get; private set; }
            private Guid UserId { get; set; }
            private BackupStorageType StorageType { get; set; }
            private string StorageBasePath { get; set; }
            public bool BackupMail { get; set; }

            public Dictionary<string, string> StorageParams { get; set; }

            public string Link { get; private set; }

            public object Id { get; set; }
            public object Status { get; set; }
            public object Error { get; set; }
            public double Percentage { get; set; }
            public bool IsCompleted { get; set; }

            private BackupWorker backupWorker;
            private TenantManager tenantManager;
            private BackupStorageFactory backupStorageFactory;
            private NotifyHelper notifyHelper;
            private BackupsContext backupRecordContext;
            private CoreBaseSettings coreBaseSettings;
            public BackupProgressItem(bool isScheduled, int tenantId, Guid userId, BackupStorageType storageType, string storageBasePath, BackupStorageFactory backupStorageFactory, BackupWorker backupWorker, NotifyHelper notifyHelper, TenantManager tenantManager, CoreBaseSettings coreBaseSettings, BackupsContext backupRecordContext)
            {
                Id = Guid.NewGuid();
                IsScheduled = isScheduled;
                TenantId = tenantId;
                UserId = userId;
                StorageType = storageType;
                StorageBasePath = storageBasePath;
                this.backupWorker = backupWorker;
                this.tenantManager = tenantManager;
                this.backupStorageFactory = backupStorageFactory;
                this.notifyHelper = notifyHelper;
                this.coreBaseSettings = coreBaseSettings;
                this.backupRecordContext = backupRecordContext;

            }

            public void RunJob()
            {
                if (ThreadPriority.BelowNormal < Thread.CurrentThread.Priority)
                {
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                }

                var backupName = string.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.{2}", tenantManager.GetTenant(TenantId).TenantAlias, DateTime.UtcNow, ArchiveFormat);
                var tempFile = Path.Combine(backupWorker.TempFolder, backupName);
                var storagePath = tempFile;
                try
                {
                    var backupTask = new BackupPortalTask(backupWorker.options, TenantId, backupWorker.configPaths[backupWorker.currentRegion], tempFile, backupWorker.limit, coreBaseSettings, backupWorker.storageFactory, backupWorker.storageFactoryConfig, backupWorker.moduleProvider, backupRecordContext);
                    if (!BackupMail)
                    {
                        backupTask.IgnoreModule(ModuleName.Mail);
                    }
                    backupTask.IgnoreTable("tenants_tariff");
                    backupTask.ProgressChanged += (sender, args) => Percentage = 0.9 * args.Progress;
                    backupTask.RunJob();

                    var backupStorage = backupStorageFactory.GetBackupStorage(StorageType, TenantId, StorageParams);
                    if (backupStorage != null)
                    {
                        storagePath = backupStorage.Upload(StorageBasePath, tempFile, UserId);
                        Link = backupStorage.GetPublicLink(storagePath);
                    }

                    var repo = backupStorageFactory.GetBackupRepository();
                    repo.SaveBackupRecord(
                        new BackupRecord
                        {
                            Id = (Guid)Id,
                            TenantId = TenantId,
                            IsScheduled = IsScheduled,
                            Name = Path.GetFileName(tempFile),
                            StorageType = StorageType,
                            StorageBasePath = StorageBasePath,
                            StoragePath = storagePath,
                            CreatedOn = DateTime.UtcNow,
                            ExpiresOn = StorageType == BackupStorageType.DataStore ? DateTime.UtcNow.AddDays(1) : DateTime.MinValue,
                            StorageParams = StorageParams
                        });

                    Percentage = 100;

                    if (UserId != Guid.Empty && !IsScheduled)
                    {
                        notifyHelper.SendAboutBackupCompleted(TenantId, UserId, Link);
                    }

                    IsCompleted = true;
                }
                catch (Exception error)
                {
                    backupWorker.Log.ErrorFormat("RunJob - Params: {0}, Error = {1}", new { Id = Id, Tenant = TenantId, File = tempFile, BasePath = StorageBasePath, }, error);
                    Error = error;
                    IsCompleted = true;
                }
                finally
                {
                    try
                    {
                        if (!(storagePath == tempFile && StorageType == BackupStorageType.Local))
                        {
                            File.Delete(tempFile);
                        }
                    }
                    catch (Exception error)
                    {
                        backupWorker.Log.Error("can't delete file: {0}", error);
                    }
                }
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }
        public class ScheduledProgressItem : BackupProgressItem
        {
            public ScheduledProgressItem(int tenantId, Guid userId, BackupStorageType storageType, string storageBasePath, BackupStorageFactory backupStorageFactory, BackupWorker backupWorker, NotifyHelper notifyHelper, TenantManager tenantManager, CoreBaseSettings coreBaseSettings, BackupsContext backupRecordContext) 
                : base(true, tenantId, userId, storageType, storageBasePath, backupStorageFactory, backupWorker, notifyHelper, tenantManager, coreBaseSettings, backupRecordContext)
            {
            }
        }
        public class RestoreProgressItem : IProgressItem
        {
            public int TenantId { get; private set; }
            public BackupStorageType StorageType { get; set; }
            public string StoragePath { get; set; }
            public bool Notify { get; set; }

            public Dictionary<string, string> StorageParams { get; set; }

            public object Id { get; set; }
            public object Status { get; set; }
            public object Error { get; set; }
            public double Percentage { get; set; }
            public bool IsCompleted { get; set; }
            private BackupWorker backupWorker;
            private TenantManager tenantManager;

            private NotifyHelper notifyHelper;
            private BackupStorageFactory backupStorageFactory;

            public RestoreProgressItem(int tenantId, BackupStorageType storageType, string storagePath, bool notify, BackupStorageFactory backupStorageFactory, BackupWorker backupWorker)
            {
                Id = Guid.NewGuid();
                TenantId = tenantId;
                StorageType = storageType;
                StoragePath = storagePath;
                Notify = notify;
                this.backupWorker = backupWorker;
                this.backupStorageFactory = backupStorageFactory;

            }

            public void RunJob()
            {
                Tenant tenant = null;
                var tempFile = PathHelper.GetTempFileName(backupWorker.TempFolder);
                try
                {
                    notifyHelper.SendAboutRestoreStarted(TenantId, Notify);

                    var storage = backupStorageFactory.GetBackupStorage(StorageType, TenantId, StorageParams);
                    storage.Download(StoragePath, tempFile);

                    Percentage = 10;

                    tenant = tenantManager.GetTenant(TenantId);
                    tenant.SetStatus(TenantStatus.Restoring);
                    tenantManager.SaveTenant(tenant);

                    var columnMapper = new ColumnMapper();
                    columnMapper.SetMapping("tenants_tenants", "alias", tenant.TenantAlias, ((Guid)Id).ToString("N"));
                    columnMapper.Commit();

                    var restoreTask = new RestorePortalTask(backupWorker.options, TenantId, backupWorker.configPaths[backupWorker.currentRegion], tempFile, backupWorker.storageFactory, backupWorker.storageFactoryConfig, backupWorker.coreBaseSettings, backupWorker.licenseReader, backupWorker.ascCacheNotify, backupWorker.moduleProvider, columnMapper, backupWorker.upgradesPath);
                    restoreTask.ProgressChanged += (sender, args) => Percentage = (10d + 0.65 * args.Progress);
                    restoreTask.RunJob();

                    Tenant restoredTenant = null;

                    if (restoreTask.Dump)
                    {
                        if (Notify)
                        {
                            AscCacheNotify.OnClearCache();
                            var tenants = tenantManager.GetTenants();
                            foreach (var t in tenants)
                            {
                                notifyHelper.SendAboutRestoreCompleted(t.TenantId, Notify);
                            }
                        }
                    }
                    else
                    {
                        tenantManager.RemoveTenant(tenant.TenantId);

                        restoredTenant = tenantManager.GetTenant(columnMapper.GetTenantMapping());
                        restoredTenant.SetStatus(TenantStatus.Active);
                        restoredTenant.TenantAlias = tenant.TenantAlias;
                        restoredTenant.PaymentId = string.Empty;
                        if (string.IsNullOrEmpty(restoredTenant.MappedDomain) && !string.IsNullOrEmpty(tenant.MappedDomain))
                        {
                            restoredTenant.MappedDomain = tenant.MappedDomain;
                        }
                        tenantManager.SaveTenant(restoredTenant);

                        // sleep until tenants cache expires
                        Thread.Sleep(TimeSpan.FromMinutes(2));

                        notifyHelper.SendAboutRestoreCompleted(restoredTenant.TenantId, Notify);
                    }

                    Percentage = 75;

                    File.Delete(tempFile);

                    Percentage = 100;
                }
                catch (Exception error)
                {
                    backupWorker.Log.Error(error);
                    Error = error;

                    if (tenant != null)
                    {
                        tenant.SetStatus(TenantStatus.Active);
                        tenantManager.SaveTenant(tenant);
                    }
                }
                finally
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                    IsCompleted = true;
                }
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public class TransferProgressItem : IProgressItem
        {
            public int TenantId { get; private set; }
            public string TargetRegion { get; set; }
            public bool TransferMail { get; set; }
            public bool Notify { get; set; }

            public string Link { get; set; }

            public object Id { get; set; }
            public object Status { get; set; }
            public object Error { get; set; }
            public double Percentage { get; set; }
            public bool IsCompleted { get; set; }
            private BackupWorker backupWorker;

            public TransferProgressItem(int tenantId, string targetRegion, bool transferMail, bool notify, BackupWorker backupWorker)
            {
                   Id = Guid.NewGuid();
                TenantId = tenantId;
                TargetRegion = targetRegion;
                TransferMail = transferMail;
                Notify = notify;
                this.backupWorker = backupWorker;

            }

            public void RunJob()
            {
                var tempFile = PathHelper.GetTempFileName(backupWorker.TempFolder);
                var alias = backupWorker.tenantManager.GetTenant(TenantId).TenantAlias;
                try
                {
                    backupWorker.notifyHelper.SendAboutTransferStart(TenantId, TargetRegion, Notify);

                    var transferProgressItem = new TransferPortalTask(backupWorker.options, TenantId, backupWorker.configPaths[backupWorker.currentRegion], backupWorker.configPaths[TargetRegion], backupWorker.limit, backupWorker.storageFactory, backupWorker.storageFactoryConfig, backupWorker.coreBaseSettings, backupWorker.tenantManager, backupWorker.moduleProvider, backupWorker.backupRecordContext) { BackupDirectory = backupWorker.TempFolder };
                    transferProgressItem.ProgressChanged += (sender, args) => Percentage = args.Progress;
                    if (!TransferMail)
                    {
                        transferProgressItem.IgnoreModule(ModuleName.Mail);
                    }
                    transferProgressItem.RunJob();

                    Link = GetLink(alias, false);
                    backupWorker.notifyHelper.SendAboutTransferComplete(TenantId, TargetRegion, Link, !Notify);
                }
                catch (Exception error)
                {
                    backupWorker.Log.Error(error);
                    Error = error;

                    Link = GetLink(alias, true);
                    backupWorker.notifyHelper.SendAboutTransferError(TenantId, TargetRegion, Link, !Notify);
                }
                finally
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                    IsCompleted = true;
                }
            }

            private string GetLink(string alias, bool isErrorLink)
            {
                return "http://" + alias + "." + ConfigurationProvider.Open(backupWorker.configPaths[isErrorLink ? backupWorker.currentRegion : TargetRegion]).AppSettings.Settings["core.base-domain"].Value;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }
    }
    public static class BackupWorkerExtension
    {
        public static DIHelper BackupWorkerService(this DIHelper services)
        {
            services.TryAddScoped<BackupWorker>();
            return services
                .AddTenantManagerService()
                .AddCoreBaseSettingsService()
                .AddStorageFactoryService()
                .AddStorageFactoryConfigService()
                .AddLicenseReaderService();
        }
    }
}
