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
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ASC.Data.Backup.Service
{
    internal class BackupWorker
    {
        private ILog Log { get; set; }
        private ProgressQueue<IProgressItem> ProgressQueue { get; set; }
        internal string TempFolder { get; set; }
        private string CurrentRegion { get; set; }
        private Dictionary<string, string> ConfigPaths { get; set; }
        private int Limit { get; set; }
        private string UpgradesPath { get; set; }
        public BackupRepository BackupRepository { get; }
        public FactoryProgressItem factoryProgressItem { get; set; }

        public BackupWorker(
            IOptionsMonitor<ILog> options,
            ProgressQueue<IProgressItem> progressQueue,
            BackupRepository backupRepository,
            FactoryProgressItem factoryProgressItem)
        {
            Log = options.CurrentValue;
            BackupRepository = backupRepository;
            ProgressQueue = progressQueue;
            this.factoryProgressItem = factoryProgressItem;
        }
        public void Start(BackupSettings settings)
        {
            TempFolder = PathHelper.ToRootedPath(settings.TempFolder);
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
                ProgressQueue.Terminate();
                ProgressQueue = null;
            }
        }

        public BackupProgress StartBackup(StartBackupRequest request)
        {
            lock (ProgressQueue.SynchRoot)
            {
                var item = ProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = factoryProgressItem.CreateBackupProgressItem(request, false,TempFolder, Limit, CurrentRegion, ConfigPaths);
                    ProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public void StartScheduledBackup(BackupSchedule schedule)
        {
            lock (ProgressQueue.SynchRoot)
            {
                var item = ProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == schedule.TenantId);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = factoryProgressItem.CreateBackupProgressItem(schedule, false, TempFolder, Limit, CurrentRegion, ConfigPaths);                
                    ProgressQueue.Add(item);
                }
            }
        }

        public BackupProgress GetBackupProgress(int tenantId)
        {
            lock (ProgressQueue.SynchRoot)
            {
                return ToBackupProgress(ProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
            }
        }

        public void ResetBackupError(int tenantId)
        {
            lock (ProgressQueue.SynchRoot)
            {
                var progress = ProgressQueue.GetItems().OfType<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Error = null;
                }
            }
        }

        public void ResetRestoreError(int tenantId)
        {
            lock (ProgressQueue.SynchRoot)
            {
                var progress = ProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (progress != null)
                {
                    progress.Error = null;
                }
            }
        }

        public BackupProgress StartRestore(StartRestoreRequest request)
        {
            lock (ProgressQueue.SynchRoot)
            {
                var item = ProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = factoryProgressItem.CreateRestoreProgressItem(request, TempFolder, UpgradesPath, CurrentRegion);
                    ProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public BackupProgress GetRestoreProgress(int tenantId)
        {
            lock (ProgressQueue.SynchRoot)
            {
                return ToBackupProgress(ProgressQueue.GetItems().OfType<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
            }
        }

        public BackupProgress StartTransfer(int tenantId, string targetRegion, bool transferMail, bool notify)
        {
            lock (ProgressQueue.SynchRoot)
            {
                var item = ProgressQueue.GetItems().OfType<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
                if (item != null && item.IsCompleted)
                {
                    ProgressQueue.Remove(item);
                    item = null;
                }
                if (item == null)
                {
                    item = factoryProgressItem.CreateTransferProgressItem(targetRegion, transferMail, tenantId, TempFolder, Limit, notify, CurrentRegion, ConfigPaths);
                    ProgressQueue.Add(item);
                }
                return ToBackupProgress(item);
            }
        }

        public BackupProgress GetTransferProgress(int tenantId)
        {
            lock (ProgressQueue.SynchRoot)
            {
                return ToBackupProgress(ProgressQueue.GetItems().OfType<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
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
        public string TempFolder { get; set; }
        private string CurrentRegion { get; set; }
        private Dictionary<string, string> ConfigPaths { get; set; }
        private int Limit { get; set; }
        private TenantManager TenantManager { get; set; }
        private BackupStorageFactory BackupStorageFactory { get; set; }
        private NotifyHelper NotifyHelper { get; set; }
        private BackupsContext BackupRecordContext{ get; set; }
        private BackupRepository BackupRepository{ get; set; }
        private CoreBaseSettings CoreBaseSettings{ get; set; }
        private IOptionsMonitor<ILog> Options{ get; set; }
        private StorageFactory StorageFactory{ get; set; }
        private StorageFactoryConfig StorageFactoryConfig{ get; set; }
        private ModuleProvider ModuleProvider{ get; set; }
        private ILog Log { get; set; }
        public BackupProgressItem(IOptionsMonitor<ILog> options, StorageFactory storageFactory, StorageFactoryConfig storageFactoryConfig, ModuleProvider moduleProvider, BackupStorageFactory backupStorageFactory, NotifyHelper notifyHelper, TenantManager tenantManager, CoreBaseSettings coreBaseSettings, BackupsContext backupRecordContext, BackupRepository backupRepository)
        {
            Id = Guid.NewGuid();

            TenantManager = tenantManager;
            BackupStorageFactory = backupStorageFactory;
            NotifyHelper = notifyHelper;
            BackupRecordContext = backupRecordContext;
            BackupRepository = backupRepository;
            Options = options;
            StorageFactory = storageFactory;
            StorageFactoryConfig = storageFactoryConfig;
            ModuleProvider = moduleProvider;
            CoreBaseSettings = coreBaseSettings;
            Log = options.CurrentValue;

        }
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
        }
        public void Init(StartBackupRequest request, bool isScheduled, string tempFolder, int limit, string currentRegion, Dictionary<string, string> configPaths)
        {
            UserId = request.UserId;
            TenantId = request.TenantId;
            StorageType = request.StorageType;
            StorageBasePath = request.StorageBasePath;
            BackupMail = request.BackupMail;
            StorageParams = request.StorageParams;
            IsScheduled = isScheduled;
            TempFolder = tempFolder;
            Limit = limit;
            CurrentRegion = currentRegion;
            ConfigPaths = configPaths;
        }
        public void RunJob()
        {
            if (ThreadPriority.BelowNormal < Thread.CurrentThread.Priority)
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            }

            var backupName = string.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.{2}", TenantManager.GetTenant(TenantId).TenantAlias, DateTime.UtcNow, ArchiveFormat);
            var tempFile = Path.Combine(TempFolder, backupName);
            var storagePath = tempFile;
            try
            {
                var backupTask = new BackupPortalTask(Options, TenantId, ConfigPaths[CurrentRegion], tempFile, Limit, CoreBaseSettings, StorageFactory, StorageFactoryConfig, ModuleProvider, BackupRecordContext);
                if (!BackupMail)
                {
                    backupTask.IgnoreModule(ModuleName.Mail);
                }
                backupTask.IgnoreTable("tenants_tariff");
                backupTask.ProgressChanged += (sender, args) => Percentage = 0.9 * args.Progress;
                backupTask.RunJob();

                var backupStorage = BackupStorageFactory.GetBackupStorage(StorageType, TenantId, StorageParams);
                if (backupStorage != null)
                {
                    storagePath = backupStorage.Upload(StorageBasePath, tempFile, UserId);
                    Link = backupStorage.GetPublicLink(storagePath);
                }

                var repo = BackupRepository;
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
                    NotifyHelper.SendAboutBackupCompleted(TenantId, UserId, Link);
                }

                IsCompleted = true;
            }
            catch (Exception error)
            {
                Log.ErrorFormat("RunJob - Params: {0}, Error = {1}", new { Id = Id, Tenant = TenantId, File = tempFile, BasePath = StorageBasePath, }, error);
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
                    Log.Error("can't delete file: {0}", error);
                }
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
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
        public string TempFolder { get; set; }
        private string CurrentRegion { get; set; }
        private string UpgradesPath { get; set; }
        private Dictionary<string, string> ConfigPaths { get; set; }

        private TenantManager TenantManager { get; set; }
        private NotifyHelper NotifyHelper { get; set; }
        private BackupStorageFactory BackupStorageFactory { get; set; }
        private CoreBaseSettings CoreBaseSettings { get; set; }
        private IOptionsMonitor<ILog> Options { get; set; }
        private StorageFactory StorageFactory { get; set; }
        private StorageFactoryConfig StorageFactoryConfig { get; set; }
        private ModuleProvider ModuleProvider { get; set; }
        private LicenseReader LicenseReader { get; set; }
        private AscCacheNotify AscCacheNotify { get; set; }
        private ILog Log { get; set; }

        public RestoreProgressItem(
            BackupStorageFactory backupStorageFactory,
            IOptionsMonitor<ILog> options, 
            StorageFactory storageFactory, 
            StorageFactoryConfig storageFactoryConfig, 
            ModuleProvider moduleProvider, 
            CoreBaseSettings coreBaseSettings,
            LicenseReader licenseReader, 
            AscCacheNotify ascCacheNotify, 
            NotifyHelper notifyHelper,
            TenantManager tenantManager)
        { 
            BackupStorageFactory = backupStorageFactory;
            Options = options;
            StorageFactory = storageFactory;
            StorageFactoryConfig = storageFactoryConfig;
            ModuleProvider = moduleProvider;
            CoreBaseSettings = coreBaseSettings;
            LicenseReader = licenseReader;
            AscCacheNotify = ascCacheNotify;
            Log = options.CurrentValue;
            NotifyHelper = notifyHelper;
            TenantManager = tenantManager;
        }
        public void Init(StartRestoreRequest request, string tempFolder, string upgradesPath, string currentRegion)
        {
            Id = Guid.NewGuid();
            TenantId = request.TenantId;
            Notify = request.NotifyAfterCompletion;
            StoragePath = request.FilePathOrId;
            StorageType = request.StorageType;
            TempFolder = tempFolder;
            UpgradesPath = upgradesPath;
            CurrentRegion = currentRegion;
        }
        public void RunJob()
        {
            Tenant tenant = null;
            var tempFile = PathHelper.GetTempFileName(TempFolder);
            try
            {
                NotifyHelper.SendAboutRestoreStarted(TenantId, Notify);

                var storage = BackupStorageFactory.GetBackupStorage(StorageType, TenantId, StorageParams);
                storage.Download(StoragePath, tempFile);

                Percentage = 10;

                tenant = TenantManager.GetTenant(TenantId);
                tenant.SetStatus(TenantStatus.Restoring);
                TenantManager.SaveTenant(tenant);

                var columnMapper = new ColumnMapper();
                columnMapper.SetMapping("tenants_tenants", "alias", tenant.TenantAlias, ((Guid)Id).ToString("N"));
                columnMapper.Commit();

                var restoreTask = new RestorePortalTask(Options, TenantId, ConfigPaths[CurrentRegion], tempFile, StorageFactory, StorageFactoryConfig, CoreBaseSettings, LicenseReader, AscCacheNotify, ModuleProvider, columnMapper, UpgradesPath);
                restoreTask.ProgressChanged += (sender, args) => Percentage = (10d + 0.65 * args.Progress);
                restoreTask.RunJob();

                Tenant restoredTenant = null;

                if (restoreTask.Dump)
                {
                    if (Notify)
                    {
                        AscCacheNotify.OnClearCache();
                        var tenants = TenantManager.GetTenants();
                        foreach (var t in tenants)
                        {
                            NotifyHelper.SendAboutRestoreCompleted(t.TenantId, Notify);
                        }
                    }
                }
                else
                {
                    TenantManager.RemoveTenant(tenant.TenantId);

                    restoredTenant = TenantManager.GetTenant(columnMapper.GetTenantMapping());
                    restoredTenant.SetStatus(TenantStatus.Active);
                    restoredTenant.TenantAlias = tenant.TenantAlias;
                    restoredTenant.PaymentId = string.Empty;
                    if (string.IsNullOrEmpty(restoredTenant.MappedDomain) && !string.IsNullOrEmpty(tenant.MappedDomain))
                    {
                        restoredTenant.MappedDomain = tenant.MappedDomain;
                    }
                    TenantManager.SaveTenant(restoredTenant);

                    // sleep until tenants cache expires
                    Thread.Sleep(TimeSpan.FromMinutes(2));

                    NotifyHelper.SendAboutRestoreCompleted(restoredTenant.TenantId, Notify);
                }

                Percentage = 75;

                File.Delete(tempFile);

                Percentage = 100;
            }
            catch (Exception error)
            {
                Log.Error(error);
                Error = error;

                if (tenant != null)
                {
                    tenant.SetStatus(TenantStatus.Active);
                    TenantManager.SaveTenant(tenant);
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
        public string TempFolder { get; set; }
        public Dictionary<string, string> ConfigPaths { get; set; }
        public IOptionsMonitor<ILog> Options { get; set; }
        public TenantManager TenantManager { get; set; }
        public NotifyHelper NotifyHelper { get; set; }
        public string CurrentRegion { get; set; }
        public int Limit { get; set; }
        public StorageFactory StorageFactory { get; set; }
        public StorageFactoryConfig StorageFactoryConfig { get; set; }
        public CoreBaseSettings CoreBaseSettings { get; set; }
        public ModuleProvider ModuleProvider { get; set; }
        public BackupsContext BackupRecordContext { get; set; }
        public ILog Log { get; set; }


        public TransferProgressItem(  
            IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            NotifyHelper notifyHelper,
            StorageFactory storageFactory,
            StorageFactoryConfig storageFactoryConfig,
            CoreBaseSettings coreBaseSettings,
            ModuleProvider moduleProvider,
            BackupsContext backupRecordContext
            )
        {
            Options = options;
            TenantManager = tenantManager;
            NotifyHelper = notifyHelper;
            StorageFactory = storageFactory;
            StorageFactoryConfig = storageFactoryConfig;
            CoreBaseSettings = coreBaseSettings;
            ModuleProvider = moduleProvider;
            BackupRecordContext = backupRecordContext;
            Log = options.CurrentValue;
        }
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
            Id = Guid.NewGuid();
            TenantId = tenantId;
            TargetRegion = targetRegion;
            TransferMail = transferMail;
            Notify = notify;
            TempFolder = tempFolder;
            ConfigPaths = configPaths;
            CurrentRegion = currentRegion;
            Limit = limit;

        }
        public void RunJob()
        {
            var tempFile = PathHelper.GetTempFileName(TempFolder);
            var alias = TenantManager.GetTenant(TenantId).TenantAlias;
            try
            {
                NotifyHelper.SendAboutTransferStart(TenantId, TargetRegion, Notify);

                var transferProgressItem = new TransferPortalTask(Options, TenantId, ConfigPaths[CurrentRegion], ConfigPaths[TargetRegion], Limit, StorageFactory, StorageFactoryConfig, CoreBaseSettings, TenantManager, ModuleProvider, BackupRecordContext) { BackupDirectory = TempFolder };
                transferProgressItem.ProgressChanged += (sender, args) => Percentage = args.Progress;
                if (!TransferMail)
                {
                    transferProgressItem.IgnoreModule(ModuleName.Mail);
                }
                transferProgressItem.RunJob();

                Link = GetLink(alias, false);
                NotifyHelper.SendAboutTransferComplete(TenantId, TargetRegion, Link, !Notify);
            }
            catch (Exception error)
            {
                Log.Error(error);
                Error = error;

                Link = GetLink(alias, true);
                NotifyHelper.SendAboutTransferError(TenantId, TargetRegion, Link, !Notify);
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
            return "http://" + alias + "." + ConfigurationProvider.Open(ConfigPaths[isErrorLink ? CurrentRegion : TargetRegion]).AppSettings.Settings["core.base-domain"].Value;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class FactoryProgressItem
    {
        public IServiceProvider ServiceProvider { get; set; }

        public FactoryProgressItem( 
            IServiceProvider serviceProvider
            )
        {
            ServiceProvider = serviceProvider;
        }

        public BackupProgressItem CreateBackupProgressItem(
            StartBackupRequest request,
            bool isScheduled,
            string tempFolder,
            int limit,
            string currentRegion,
            Dictionary<string, string> configPaths
            )
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
            string currentRegion
            )
        {
            var item = ServiceProvider.GetService<RestoreProgressItem>();
            item.Init(request, tempFolder, upgradesPath, currentRegion);
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
    public static class BackupWorkerExtension
    {
        public static DIHelper AddBackupWorkerService(this DIHelper services)
        {
            services.TryAddScoped<BackupWorker>();
            services.TryAddScoped<FactoryProgressItem>();
            services.TryAddSingleton<ProgressQueue<IProgressItem>>();
            return services
                .AddTenantManagerService()
                .AddCoreBaseSettingsService()
                .AddStorageFactoryService()
                .AddStorageFactoryConfigService()
                .AddLicenseReaderService()
                .AddNotifyHelperService();
        }
    }
}
