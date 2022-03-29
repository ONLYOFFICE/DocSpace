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
using System.Security.Cryptography;
using System.Threading;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Backup.Contracts;
using ASC.Data.Backup.Core;
using ASC.Data.Backup.EF.Model;
using ASC.Data.Backup.Storage;
using ASC.Data.Backup.Tasks;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Backup.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ASC.Data.Backup.Services
{
    [Singletone(Additional = typeof(BackupWorkerExtension))]
    public class BackupWorker
    {
        private ILog Log { get; set; }
        private DistributedTaskQueue ProgressQueue { get; set; }
        internal string TempFolder { get; set; }
        private string CurrentRegion { get; set; }
        private Dictionary<string, string> ConfigPaths { get; set; }
        private int Limit { get; set; }
        private string UpgradesPath { get; set; }
        private ICacheNotify<BackupProgress> CacheBackupProgress { get; }
        private FactoryProgressItem FactoryProgressItem { get; set; }
        private TempPath TempPath { get; }

        private readonly object SynchRoot = new object();

        public BackupWorker(
            IOptionsMonitor<ILog> options,
            ICacheNotify<BackupProgress> cacheBackupProgress,
            DistributedTaskQueueOptionsManager progressQueue,
            FactoryProgressItem factoryProgressItem,
            TempPath tempPath)
        {
            Log = options.CurrentValue;
            ProgressQueue = progressQueue.Get<BaseBackupProgressItem>();
            CacheBackupProgress = cacheBackupProgress;
            FactoryProgressItem = factoryProgressItem;
            TempPath = tempPath;
        }

        public void Start(BackupSettings settings)
        {
            TempFolder = TempPath.GetTempPath();
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }

            Limit = settings.Limit;
            UpgradesPath = settings.UpgradesPath;
            CurrentRegion = settings.WebConfigs.CurrentRegion;
            ConfigPaths = settings.WebConfigs.Elements.ToDictionary(el => el.Region, el => PathHelper.ToRootedConfigPath(el.Path));
            ConfigPaths[CurrentRegion] = PathHelper.ToRootedConfigPath(settings.WebConfigs.CurrentPath);

            var invalidConfigPath = ConfigPaths.Values.FirstOrDefault(path => !File.Exists(path));
            if (invalidConfigPath != null)
            {
                Log.WarnFormat("Configuration file {0} not found", invalidConfigPath);
            }
        }

        public void Stop()
        {
            if (ProgressQueue != null)
            {
                var tasks = ProgressQueue.GetTasks();
                foreach (var t in tasks)
                {
                    ProgressQueue.CancelTask(t.Id);
                }

                ProgressQueue = null;
            }
        }

        public BackupProgress StartBackup(StartBackupRequest request)
        {
            lock (SynchRoot)
            {
                var item = ProgressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId && t.BackupProgressItemEnum == BackupProgressItemEnum.Backup);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.RemoveTask(item.Id);
                    item = null;
                }
                if (item == null)
                {
                    item = FactoryProgressItem.CreateBackupProgressItem(request, false, TempFolder, Limit, CurrentRegion, ConfigPaths);
                    ProgressQueue.QueueTask(item);
                }

                item.PublishChanges();

                return ToBackupProgress(item);
            }
        }

        public void StartScheduledBackup(BackupSchedule schedule)
        {
            lock (SynchRoot)
            {
                var item = ProgressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == schedule.TenantId && t.BackupProgressItemEnum == BackupProgressItemEnum.Backup);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.RemoveTask(item.Id);
                    item = null;
                }
                if (item == null)
                {
                    item = FactoryProgressItem.CreateBackupProgressItem(schedule, false, TempFolder, Limit, CurrentRegion, ConfigPaths);
                    ProgressQueue.QueueTask(item);
                }
            }
        }

        public BackupProgress GetBackupProgress(int tenantId)
        {
            lock (SynchRoot)
            {
                return ToBackupProgress(ProgressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId && t.BackupProgressItemEnum == BackupProgressItemEnum.Backup));
            }
        }

        public BackupProgress GetTransferProgress(int tenantId)
        {
            lock (SynchRoot)
            {
                return ToBackupProgress(ProgressQueue.GetTasks<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId && t.BackupProgressItemEnum == BackupProgressItemEnum.Transfer));
            }
        }

        public BackupProgress GetRestoreProgress(int tenantId)
        {
            lock (SynchRoot)
            {
                return ToBackupProgress(ProgressQueue.GetTasks<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId && t.BackupProgressItemEnum == BackupProgressItemEnum.Restore));
            }
        }

        public void ResetBackupError(int tenantId)
        {
            lock (SynchRoot)
            {
                var progress = ProgressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Exception = null;
                }
            }
        }

        public void ResetRestoreError(int tenantId)
        {
            lock (SynchRoot)
            {
                var progress = ProgressQueue.GetTasks<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Exception = null;
                }
            }
        }

        public BackupProgress StartRestore(StartRestoreRequest request)
        {
            lock (SynchRoot)
            {
                var item = ProgressQueue.GetTasks<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.RemoveTask(item.Id);
                    item = null;
                }
                if (item == null)
                {
                    item = FactoryProgressItem.CreateRestoreProgressItem(request, TempFolder, UpgradesPath, CurrentRegion, ConfigPaths);
                    ProgressQueue.QueueTask(item);
                }
                return ToBackupProgress(item);
            }
        }

        public BackupProgress StartTransfer(int tenantId, string targetRegion, bool transferMail, bool notify)
        {
            lock (SynchRoot)
            {
                var item = ProgressQueue.GetTasks<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.RemoveTask(item.Id);
                    item = null;
                }

                if (item == null)
                {
                    item = FactoryProgressItem.CreateTransferProgressItem(targetRegion, transferMail, tenantId, TempFolder, Limit, notify, CurrentRegion, ConfigPaths);
                    ProgressQueue.QueueTask(item);
                }

                return ToBackupProgress(item);
            }
        }


        private BackupProgress ToBackupProgress(BaseBackupProgressItem progressItem)
        {
            if (progressItem == null)
            {
                return null;
            }
            var progress = new BackupProgress
            {
                IsCompleted = progressItem.IsCompleted,
                Progress = (int)progressItem.Percentage,
                Error = progressItem.Exception != null ? progressItem.Exception.Message : "",
                TenantId = progressItem.TenantId,
                BackupProgressEnum = progressItem.BackupProgressItemEnum.Convert()
            };

            if ((progressItem.BackupProgressItemEnum == BackupProgressItemEnum.Backup || progressItem.BackupProgressItemEnum == BackupProgressItemEnum.Transfer) && progressItem.Link != null)
            {
                progress.Link = progressItem.Link;
            }

            return progress;
        }

        internal static string GetBackupHash(string path)
        {
            using (var sha256 = SHA256.Create())
            using (var fileStream = File.OpenRead(path))
            {
                fileStream.Position = 0;
                var hash = sha256.ComputeHash(fileStream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }

    public enum BackupProgressItemEnum
    {
        Backup,
        Restore,
        Transfer
    }

    public static class BackupProgressItemEnumConverter
    {
        public static BackupProgressEnum Convert(this BackupProgressItemEnum backupProgressItemEnum)
        {
            return backupProgressItemEnum switch
            {
                BackupProgressItemEnum.Backup => BackupProgressEnum.Backup,
                BackupProgressItemEnum.Restore => BackupProgressEnum.Restore,
                BackupProgressItemEnum.Transfer => BackupProgressEnum.Transfer,
                _ => BackupProgressEnum.Backup
            };
        }
    }

    public abstract class BaseBackupProgressItem : DistributedTaskProgress
    {
        private int? _tenantId;
        public int TenantId
        {
            get
            {
                return _tenantId ?? GetProperty<int>(nameof(_tenantId));
            }
            set
            {
                _tenantId = value;
                SetProperty(nameof(_tenantId), value);
            }
        }

        private string _link;
        public string Link
        {
            get
            {
                return _link ?? GetProperty<string>(nameof(_link));
            }
            set
            {
                _link = value;
                SetProperty(nameof(_link), value);
            }
        }

        private BackupProgressItemEnum? backupProgressItemEnum;
        public BackupProgressItemEnum BackupProgressItemEnum
        {
            get
            {
                return backupProgressItemEnum ?? GetProperty<BackupProgressItemEnum>(nameof(backupProgressItemEnum));
            }
            protected set
            {
                backupProgressItemEnum = value;
                SetProperty(nameof(backupProgressItemEnum), value);
            }
        }

        public abstract object Clone();

        protected ILog Log { get; set; }

        protected IServiceProvider ServiceProvider { get; set; }

        protected BaseBackupProgressItem(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider)
        {
            Log = options.CurrentValue;
            ServiceProvider = serviceProvider;
    }
    }

    [Transient]
    public class BackupProgressItem : BaseBackupProgressItem
    {
        private const string ArchiveFormat = "tar.gz";

        public BackupProgressItem(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        private bool IsScheduled { get; set; }
        private Guid UserId { get; set; }
        private BackupStorageType StorageType { get; set; }
        private string StorageBasePath { get; set; }
        public bool BackupMail { get; set; }
        public Dictionary<string, string> StorageParams { get; set; }
        public string TempFolder { get; set; }
        private string CurrentRegion { get; set; }
        private Dictionary<string, string> ConfigPaths { get; set; }
        private int Limit { get; set; }

        public void Init(BackupSchedule schedule, bool isScheduled, string tempFolder, int limit, string currentRegion, Dictionary<string, string> configPaths)
        {
            UserId = Guid.Empty;
            TenantId = schedule.TenantId;
            StorageType = schedule.StorageType;
            StorageBasePath = schedule.StorageBasePath;
            BackupMail = schedule.BackupMail;
            StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(schedule.StorageParams);
            IsScheduled = isScheduled;
            TempFolder = tempFolder;
            Limit = limit;
            CurrentRegion = currentRegion;
            ConfigPaths = configPaths;
            BackupProgressItemEnum = BackupProgressItemEnum.Backup;
        }

        public void Init(StartBackupRequest request, bool isScheduled, string tempFolder, int limit, string currentRegion, Dictionary<string, string> configPaths)
        {
            UserId = request.UserId;
            TenantId = request.TenantId;
            StorageType = request.StorageType;
            StorageBasePath = request.StorageBasePath;
            BackupMail = request.BackupMail;
            StorageParams = request.StorageParams.ToDictionary(r => r.Key, r => r.Value);
            IsScheduled = isScheduled;
            TempFolder = tempFolder;
            Limit = limit;
            CurrentRegion = currentRegion;
            ConfigPaths = configPaths;
        }

        protected override void DoJob()
        {
            if (ThreadPriority.BelowNormal < Thread.CurrentThread.Priority)
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            }

            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<BackupWorkerScope>();
            var (tenantManager, backupStorageFactory, notifyHelper, backupRepository, backupWorker, backupPortalTask, _, _, coreBaseSettings) = scopeClass;

            var dateTime = coreBaseSettings.Standalone ? DateTime.Now : DateTime.UtcNow;
            var backupName = string.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.{2}", tenantManager.GetTenant(TenantId).TenantAlias, dateTime, ArchiveFormat);

            var tempFile = CrossPlatform.PathCombine(TempFolder, backupName);
            var storagePath = tempFile;
            try
            {
                var backupTask = backupPortalTask;

                backupTask.Init(TenantId, ConfigPaths[CurrentRegion], tempFile, Limit);
                if (!BackupMail)
                {
                    backupTask.IgnoreModule(ModuleName.Mail);
                }

                backupTask.ProgressChanged += (sender, args) =>
                {
                    Percentage = 0.9 * args.Progress;
                    PublishChanges();
                };

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
                        Id = Guid.Parse(Id),
                        TenantId = TenantId,
                        IsScheduled = IsScheduled,
                        Name = Path.GetFileName(tempFile),
                        StorageType = StorageType,
                        StorageBasePath = StorageBasePath,
                        StoragePath = storagePath,
                        CreatedOn = DateTime.UtcNow,
                        ExpiresOn = StorageType == BackupStorageType.DataStore ? DateTime.UtcNow.AddDays(1) : DateTime.MinValue,
                        StorageParams = JsonConvert.SerializeObject(StorageParams),
                        Hash = BackupWorker.GetBackupHash(tempFile)
                    });

                Percentage = 100;

                if (UserId != Guid.Empty && !IsScheduled)
                {
                    notifyHelper.SendAboutBackupCompleted(TenantId, UserId);
                }

                IsCompleted = true;
                PublishChanges();
            }
            catch (Exception error)
            {
                Log.ErrorFormat("RunJob - Params: {0}, Error = {1}", new { Id, Tenant = TenantId, File = tempFile, BasePath = StorageBasePath, }, error);
                Exception = error;
                IsCompleted = true;
            }
            finally
            {
                try
                {
                    PublishChanges();
                }
                catch (Exception error)
                {
                    Log.Error("publish", error);
                }

                try
                {
                    if (!(storagePath == tempFile && StorageType == BackupStorageType.Local))
                    {
                        File.Delete(tempFile);
                    }
                }
                catch (Exception error)
                {
                    Log.Error("can't delete file: {0}", error);
                }
            }
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Transient]
    public class RestoreProgressItem : BaseBackupProgressItem
    {
        public RestoreProgressItem(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        public BackupStorageType StorageType { get; set; }
        public string StoragePath { get; set; }
        public bool Notify { get; set; }
        public Dictionary<string, string> StorageParams { get; set; }
        public string TempFolder { get; set; }
        private string CurrentRegion { get; set; }
        private string UpgradesPath { get; set; }
        private Dictionary<string, string> ConfigPaths { get; set; }

        public void Init(StartRestoreRequest request, string tempFolder, string upgradesPath, string currentRegion, Dictionary<string, string> configPaths)
        {
            TenantId = request.TenantId;
            Notify = request.NotifyAfterCompletion;
            StoragePath = request.FilePathOrId;
            StorageType = request.StorageType;
            TempFolder = tempFolder;
            UpgradesPath = upgradesPath;
            CurrentRegion = currentRegion;
            ConfigPaths = configPaths;
            BackupProgressItemEnum = BackupProgressItemEnum.Restore;
            StorageParams = request.StorageParams;
        }

        protected override void DoJob()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<BackupWorkerScope>();
            var (tenantManager, backupStorageFactory, notifyHelper, backupRepository, backupWorker, _, restorePortalTask, _, coreBaseSettings) = scopeClass;
            Tenant tenant = null;
            var tempFile = PathHelper.GetTempFileName(TempFolder);
            try
            {
                tenant = tenantManager.GetTenant(TenantId);
                tenantManager.SetCurrentTenant(tenant);
                notifyHelper.SendAboutRestoreStarted(tenant, Notify);
                tenant.SetStatus(TenantStatus.Restoring);
                tenantManager.SaveTenant(tenant);

                var storage = backupStorageFactory.GetBackupStorage(StorageType, TenantId, StorageParams);
                storage.Download(StoragePath, tempFile);

                if (!coreBaseSettings.Standalone)
                {
                    var backupHash = BackupWorker.GetBackupHash(tempFile);
                    var record = backupRepository.GetBackupRecord(backupHash, TenantId);
                    if (record == null)
                    {
                        throw new Exception(BackupResource.BackupNotFound);
                    }
                }

                Percentage = 10;

                var columnMapper = new ColumnMapper();
                columnMapper.SetMapping("tenants_tenants", "alias", tenant.TenantAlias, Guid.Parse(Id).ToString("N"));
                columnMapper.Commit();

                var restoreTask = restorePortalTask;
                restoreTask.Init(ConfigPaths[CurrentRegion], tempFile, TenantId, columnMapper, UpgradesPath);
                restoreTask.ProgressChanged += (sender, args) =>
                {
                    Percentage = Percentage = 10d + 0.65 * args.Progress;
                    PublishChanges();
                };
                restoreTask.RunJob();

                Tenant restoredTenant = null;

                if (restoreTask.Dump)
                {
                    AscCacheNotify.OnClearCache();

                    if (Notify)
                    {
                        var tenants = tenantManager.GetTenants();
                        foreach (var t in tenants)
                        {
                            notifyHelper.SendAboutRestoreCompleted(t, Notify);
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
                    tenantManager.SetCurrentTenant(restoredTenant);
                    // sleep until tenants cache expires
                    Thread.Sleep(TimeSpan.FromMinutes(2));

                    notifyHelper.SendAboutRestoreCompleted(restoredTenant, Notify);
                }

                Percentage = 75;

                PublishChanges();

                File.Delete(tempFile);

                Percentage = 100;
                PublishChanges();
            }
            catch (Exception error)
            {
                Log.Error(error);
                Exception = error;

                if (tenant != null)
                {
                    tenant.SetStatus(TenantStatus.Active);
                    tenantManager.SaveTenant(tenant);
                }
            }
            finally
            {
                IsCompleted = true;
                try
                {
                    PublishChanges();
                }
                catch (Exception error)
                {
                    Log.Error("publish", error);
                }

                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }


    }

    [Transient]
    public class TransferProgressItem : BaseBackupProgressItem
    {
        public TransferProgressItem(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        public string TargetRegion { get; set; }
        public bool TransferMail { get; set; }
        public bool Notify { get; set; }
        public string TempFolder { get; set; }
        public Dictionary<string, string> ConfigPaths { get; set; }
        public string CurrentRegion { get; set; }
        public int Limit { get; set; }

        public void Init(
            string targetRegion,
            bool transferMail,
            int tenantId,
            string tempFolder,
            int limit,
            bool notify,
            string currentRegion,
            Dictionary<string, string> configPaths)
        {
            TenantId = tenantId;
            TargetRegion = targetRegion;
            TransferMail = transferMail;
            Notify = notify;
            TempFolder = tempFolder;
            ConfigPaths = configPaths;
            CurrentRegion = currentRegion;
            Limit = limit;
            BackupProgressItemEnum = BackupProgressItemEnum.Restore;
        }

        protected override void DoJob()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<BackupWorkerScope>();
            var (tenantManager, _, notifyHelper, _, backupWorker, _, _, transferPortalTask, _) = scopeClass;
            var tempFile = PathHelper.GetTempFileName(TempFolder);
            var tenant = tenantManager.GetTenant(TenantId);
            var alias = tenant.TenantAlias;

            try
            {
                notifyHelper.SendAboutTransferStart(tenant, TargetRegion, Notify);
                var transferProgressItem = transferPortalTask;
                transferProgressItem.Init(TenantId, ConfigPaths[CurrentRegion], ConfigPaths[TargetRegion], Limit, TempFolder);
                transferProgressItem.ProgressChanged += (sender, args) =>
                {
                    Percentage = args.Progress;
                    PublishChanges();
                };
                if (!TransferMail)
                {
                    transferProgressItem.IgnoreModule(ModuleName.Mail);
                }
                transferProgressItem.RunJob();

                Link = GetLink(alias, false);
                notifyHelper.SendAboutTransferComplete(tenant, TargetRegion, Link, !Notify, transferProgressItem.ToTenantId);
                PublishChanges();
            }
            catch (Exception error)
            {
                Log.Error(error);
                Exception = error;

                Link = GetLink(alias, true);
                notifyHelper.SendAboutTransferError(tenant, TargetRegion, Link, !Notify);
            }
            finally
            {
                IsCompleted = true;
                try
                {
                    PublishChanges();
                }
                catch (Exception error)
                {
                    Log.Error("publish", error);
                }

                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        private string GetLink(string alias, bool isErrorLink)
        {
            return "https://" + alias + "." + ConfigurationProvider.Open(ConfigPaths[isErrorLink ? CurrentRegion : TargetRegion]).AppSettings.Settings["core:base-domain"].Value;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Singletone(Additional = typeof(FactoryProgressItemExtension))]
    public class FactoryProgressItem
    {
        public IServiceProvider ServiceProvider { get; }

        public FactoryProgressItem(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public BackupProgressItem CreateBackupProgressItem(
            StartBackupRequest request,
            bool isScheduled,
            string tempFolder,
            int limit,
            string currentRegion,
            Dictionary<string, string> configPaths)
        {
            var item = ServiceProvider.GetService<BackupProgressItem>();
            item.Init(request, isScheduled, tempFolder, limit, currentRegion, configPaths);
            return item;
        }

        public BackupProgressItem CreateBackupProgressItem(
            BackupSchedule schedule,
            bool isScheduled,
            string tempFolder,
            int limit,
            string currentRegion,
            Dictionary<string, string> configPaths
            )
        {
            var item = ServiceProvider.GetService<BackupProgressItem>();
            item.Init(schedule, isScheduled, tempFolder, limit, currentRegion, configPaths);
            return item;
        }

        public RestoreProgressItem CreateRestoreProgressItem(
            StartRestoreRequest request,
            string tempFolder,
            string upgradesPath,
            string currentRegion,
            Dictionary<string, string> configPaths
            )
        {
            var item = ServiceProvider.GetService<RestoreProgressItem>();
            item.Init(request, tempFolder, upgradesPath, currentRegion, configPaths);
            return item;
        }

        public TransferProgressItem CreateTransferProgressItem(
            string targetRegion,
            bool transferMail,
            int tenantId,
            string tempFolder,
            int limit,
            bool notify,
            string currentRegion,
            Dictionary<string, string> configPaths
            )
        {
            var item = ServiceProvider.GetService<TransferProgressItem>();
            item.Init(targetRegion, transferMail, tenantId, tempFolder, limit, notify, currentRegion, configPaths);
            return item;
        }
    }

    [Scope]
    internal class BackupWorkerScope
    {
        private TenantManager TenantManager { get; }
        private BackupStorageFactory BackupStorageFactory { get; }
        private NotifyHelper NotifyHelper { get; }
        private BackupRepository BackupRepository { get; }
        private BackupWorker BackupWorker { get; }
        private BackupPortalTask BackupPortalTask { get; }
        private RestorePortalTask RestorePortalTask { get; }
        private TransferPortalTask TransferPortalTask { get; }
        private CoreBaseSettings CoreBaseSettings { get; }

        public BackupWorkerScope(TenantManager tenantManager,
            BackupStorageFactory backupStorageFactory,
            NotifyHelper notifyHelper,
            BackupRepository backupRepository,
            BackupWorker backupWorker,
            BackupPortalTask backupPortalTask,
            RestorePortalTask restorePortalTask,
            TransferPortalTask transferPortalTask,
            CoreBaseSettings coreBaseSettings)
        {
            TenantManager = tenantManager;
            BackupStorageFactory = backupStorageFactory;
            NotifyHelper = notifyHelper;
            BackupRepository = backupRepository;
            BackupWorker = backupWorker;
            BackupPortalTask = backupPortalTask;
            RestorePortalTask = restorePortalTask;
            TransferPortalTask = transferPortalTask;
            CoreBaseSettings = coreBaseSettings;
        }

        public void Deconstruct(out TenantManager tenantManager,
            out BackupStorageFactory backupStorageFactory,
            out NotifyHelper notifyHelper,
            out BackupRepository backupRepository,
            out BackupWorker backupWorker,
            out BackupPortalTask backupPortalTask,
            out RestorePortalTask restorePortalTask,
            out TransferPortalTask transferPortalTask,
            out CoreBaseSettings coreBaseSettings)
        {
            tenantManager = TenantManager;
            backupStorageFactory = BackupStorageFactory;
            notifyHelper = NotifyHelper;
            backupRepository = BackupRepository;
            backupWorker = BackupWorker;
            backupPortalTask = BackupPortalTask;
            restorePortalTask = RestorePortalTask;
            transferPortalTask = TransferPortalTask;
            coreBaseSettings = CoreBaseSettings;
        }
    }

    public static class BackupWorkerExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<BackupWorkerScope>();
            services.AddDistributedTaskQueueService<BackupProgressItem>(5);
        }
    }

    public static class FactoryProgressItemExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<BackupProgressItem>();
            services.TryAdd<RestoreProgressItem>();
            services.TryAdd<TransferProgressItem>();
        }
    }
}
