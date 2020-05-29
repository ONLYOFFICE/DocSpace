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

using Newtonsoft.Json;

namespace ASC.Data.Backup.Service
{
    internal class BackupWorker
    {
        private ILog Log { get; set; }
        private ProgressQueue<BackupProgressItem> BackupProgressQueue { get; set; }
        private ProgressQueue<ScheduledProgressItem> ScheduledProgressQueue { get; set; }
        private ProgressQueue<RestoreProgressItem> RestoreProgressQueue { get; set; }
        private ProgressQueue<TransferProgressItem> TransferProgressQueue { get; set; }
        internal string TempFolder { get; set; }
        private string CurrentRegion { get; set; }
        private Dictionary<string, string> ConfigPaths { get; set; }
        private int Limit { get; set; }
        private string UpgradesPath { get; set; }
        private TenantManager TenantManager { get; set; }
        private BackupStorageFactory BackupStorageFactory { get; set; }
        private NotifyHelper NotifyHelper { get; set; }
        private CoreBaseSettings CoreBaseSettings { get; set; }
        private IOptionsMonitor<ILog> Options { get; set; }
        private StorageFactory StorageFactory { get; set; }
        private StorageFactoryConfig StorageFactoryConfig { get; set; }
        private LicenseReader LicenseReader { get; set; }
        private AscCacheNotify AscCacheNotify { get; set; }
        private ModuleProvider ModuleProvider { get; set; }
        private BackupsContext BackupRecordContext { get; set; }
        public BackupRepository BackupRepository { get; }

        public BackupWorker(
            IOptionsMonitor<ILog> options,
            ProgressQueue<BackupProgressItem> backupProgressQueue,
            ProgressQueue<ScheduledProgressItem> scheduledProgressQueue,
            ProgressQueue<RestoreProgressItem> restoreProgressQueue,
            ProgressQueue<TransferProgressItem> transferProgressQueue,
            BackupStorageFactory backupStorageFactory,
            NotifyHelper notifyHelper,
            TenantManager tenantManager,
            CoreBaseSettings coreBaseSettings,
            StorageFactory storageFactory,
            StorageFactoryConfig storageFactoryConfig,
            LicenseReader licenseReader,
            AscCacheNotify ascCacheNotify,
            ModuleProvider moduleProvider,
            BackupsContext backupRecordContext,
            BackupRepository backupRepository)
        {
            Log = options.CurrentValue;

            TenantManager = tenantManager;
            BackupStorageFactory = backupStorageFactory;
            NotifyHelper = notifyHelper;
            CoreBaseSettings = coreBaseSettings;
            StorageFactory = storageFactory;
            StorageFactoryConfig = storageFactoryConfig;
            LicenseReader = licenseReader;
            AscCacheNotify = ascCacheNotify;
            ModuleProvider = moduleProvider;
            BackupRecordContext = backupRecordContext;
            BackupRepository = backupRepository;
            BackupProgressQueue = backupProgressQueue;
            ScheduledProgressQueue = scheduledProgressQueue;
            RestoreProgressQueue = restoreProgressQueue;
            this.TransferProgressQueue = transferProgressQueue;
        }
        public void Start(BackupConfigurationSection config)
        {
            TempFolder = PathHelper.ToRootedPath(config.TempFolder);
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }

            Limit = config.Limit;
            UpgradesPath = config.UpgradesPath;
            CurrentRegion = config.WebConfigs.CurrentRegion;
            ConfigPaths = config.WebConfigs.Cast<WebConfigElement>().ToDictionary(el => el.Region, el => PathHelper.ToRootedConfigPath(el.Path));
            ConfigPaths[CurrentRegion] = PathHelper.ToRootedConfigPath(config.WebConfigs.CurrentPath);

            var invalidConfigPath = ConfigPaths.Values.FirstOrDefault(path => !File.Exists(path));
            if (invalidConfigPath != null)
            {
                Log.WarnFormat("Configuration file {0} not found", invalidConfigPath);
            }
        }

        public void Stop()
        {
            if (BackupProgressQueue != null)
            {
                BackupProgressQueue.Terminate();
                BackupProgressQueue = null;
            }
            if (ScheduledProgressQueue != null)
            {
                ScheduledProgressQueue.Terminate();
                ScheduledProgressQueue = null;
            }
            if (RestoreProgressQueue != null)
            {
                RestoreProgressQueue.Terminate();
                RestoreProgressQueue = null;
            }
            if (TransferProgressQueue != null)
            {
                TransferProgressQueue.Terminate();
                TransferProgressQueue = null;
            }
        }

        public BackupProgress StartBackup(StartBackupRequest request)
        {
            lock (BackupProgressQueue.SynchRoot)
            {
                var item = BackupProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
                if (item != null && item.IsCompleted)
                {
                    BackupProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new BackupProgressItem(false, request.TenantId, request.UserId, request.StorageType, request.StorageBasePath, BackupStorageFactory, this, NotifyHelper, TenantManager, CoreBaseSettings, BackupRecordContext, BackupRepository) { BackupMail = request.BackupMail, StorageParams = request.StorageParams };
                    BackupProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public void StartScheduledBackup(BackupSchedule schedule)
        {
            lock (ScheduledProgressQueue.SynchRoot)
            {
                var item = ScheduledProgressQueue.GetItems().OfType<ScheduledProgressItem>().FirstOrDefault(t => t.TenantId == schedule.TenantId);
                if (item != null && item.IsCompleted)
                {
                    ScheduledProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new ScheduledProgressItem(schedule.TenantId, Guid.Empty, schedule.StorageType, schedule.StorageBasePath, BackupStorageFactory, this, NotifyHelper, TenantManager, CoreBaseSettings, BackupRecordContext, BackupRepository)
                    {
                        BackupMail = schedule.BackupMail,
                        StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(schedule.StorageParams)
                    };
                    ScheduledProgressQueue.Add(item);
                }
            }
        }

        public BackupProgress GetBackupProgress(int tenantId)
        {
            lock (BackupProgressQueue.SynchRoot)
            {
                return ToBackupProgress(BackupProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
            }
        }

        public void ResetBackupError(int tenantId)
        {
            lock (BackupProgressQueue.SynchRoot)
            {
                var progress = BackupProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Error = null;
                }
            }
        }

        public void ResetRestoreError(int tenantId)
        {
            lock (RestoreProgressQueue.SynchRoot)
            {
                var progress = RestoreProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Error = null;
                }
            }
        }

        public BackupProgress StartRestore(StartRestoreRequest request)
        {
            lock (RestoreProgressQueue.SynchRoot)
            {
                var item = RestoreProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
                if (item != null && item.IsCompleted)
                {
                    RestoreProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new RestoreProgressItem(request.TenantId, request.StorageType, request.FilePathOrId, request.NotifyAfterCompletion, BackupStorageFactory, this) { StorageParams = request.StorageParams };
                    RestoreProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public BackupProgress GetRestoreProgress(int tenantId)
        {
            lock (RestoreProgressQueue.SynchRoot)
            {
                return ToBackupProgress(RestoreProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
            }
        }

        public BackupProgress StartTransfer(int tenantId, string targetRegion, bool transferMail, bool notify)
        {
            lock (TransferProgressQueue.SynchRoot)
            {
                var item = TransferProgressQueue.GetItems().OfType<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (item != null && item.IsCompleted)
                {
                    TransferProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = new TransferProgressItem(tenantId, targetRegion, transferMail, notify, this);
                    TransferProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public BackupProgress GetTransferProgress(int tenantId)
        {
            lock (TransferProgressQueue.SynchRoot)
            {
                return ToBackupProgress(TransferProgressQueue.GetItems().OfType<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
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

        internal class BackupProgressItem : IProgressItem
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
            private readonly BackupRepository backupRepository;
            private CoreBaseSettings coreBaseSettings;
            public BackupProgressItem(bool isScheduled, int tenantId, Guid userId, BackupStorageType storageType, string storageBasePath, BackupStorageFactory backupStorageFactory, BackupWorker backupWorker, NotifyHelper notifyHelper, TenantManager tenantManager, CoreBaseSettings coreBaseSettings, BackupsContext backupRecordContext, BackupRepository backupRepository)
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
                this.backupRepository = backupRepository;
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
                    var backupTask = new BackupPortalTask(backupWorker.Options, TenantId, backupWorker.ConfigPaths[backupWorker.CurrentRegion], tempFile, backupWorker.Limit, coreBaseSettings, backupWorker.StorageFactory, backupWorker.StorageFactoryConfig, backupWorker.ModuleProvider, backupRecordContext);
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

                    var repo = backupRepository;
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
                            StorageParams = JsonConvert.SerializeObject(StorageParams)
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
        internal class ScheduledProgressItem : BackupProgressItem
        {
            public ScheduledProgressItem(int tenantId, Guid userId, BackupStorageType storageType, string storageBasePath, BackupStorageFactory backupStorageFactory, BackupWorker backupWorker, NotifyHelper notifyHelper, TenantManager tenantManager, CoreBaseSettings coreBaseSettings, BackupsContext backupRecordContext, BackupRepository backupRepository)
                : base(true, tenantId, userId, storageType, storageBasePath, backupStorageFactory, backupWorker, notifyHelper, tenantManager, coreBaseSettings, backupRecordContext, backupRepository)
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

                    var restoreTask = new RestorePortalTask(backupWorker.Options, TenantId, backupWorker.ConfigPaths[backupWorker.CurrentRegion], tempFile, backupWorker.StorageFactory, backupWorker.StorageFactoryConfig, backupWorker.CoreBaseSettings, backupWorker.LicenseReader, backupWorker.AscCacheNotify, backupWorker.ModuleProvider, columnMapper, backupWorker.UpgradesPath);
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
                var alias = backupWorker.TenantManager.GetTenant(TenantId).TenantAlias;
                try
                {
                    backupWorker.NotifyHelper.SendAboutTransferStart(TenantId, TargetRegion, Notify);

                    var transferProgressItem = new TransferPortalTask(backupWorker.Options, TenantId, backupWorker.ConfigPaths[backupWorker.CurrentRegion], backupWorker.ConfigPaths[TargetRegion], backupWorker.Limit, backupWorker.StorageFactory, backupWorker.StorageFactoryConfig, backupWorker.CoreBaseSettings, backupWorker.TenantManager, backupWorker.ModuleProvider, backupWorker.BackupRecordContext) { BackupDirectory = backupWorker.TempFolder };
                    transferProgressItem.ProgressChanged += (sender, args) => Percentage = args.Progress;
                    if (!TransferMail)
                    {
                        transferProgressItem.IgnoreModule(ModuleName.Mail);
                    }
                    transferProgressItem.RunJob();

                    Link = GetLink(alias, false);
                    backupWorker.NotifyHelper.SendAboutTransferComplete(TenantId, TargetRegion, Link, !Notify);
                }
                catch (Exception error)
                {
                    backupWorker.Log.Error(error);
                    Error = error;

                    Link = GetLink(alias, true);
                    backupWorker.NotifyHelper.SendAboutTransferError(TenantId, TargetRegion, Link, !Notify);
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
                return "http://" + alias + "." + ConfigurationProvider.Open(backupWorker.ConfigPaths[isErrorLink ? backupWorker.CurrentRegion : TargetRegion]).AppSettings.Settings["core.base-domain"].Value;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }
    }
    public static class BackupWorkerExtension
    {
        public static DIHelper AddBackupWorkerService(this DIHelper services)
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
