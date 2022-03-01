/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Data.Storage.Configuration;
using ASC.Protos.Migration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Storage
{
    [Singletone]
    public class StorageUploader
    {
        protected readonly DistributedTaskQueue Queue;

        private static readonly object Locker;

        private IServiceProvider ServiceProvider { get; }
        private TempStream TempStream { get; }
        private ICacheNotify<MigrationProgress> CacheMigrationNotify { get; }

        static StorageUploader()
        {
            Locker = new object();
        }

        public StorageUploader(IServiceProvider serviceProvider, TempStream tempStream, ICacheNotify<MigrationProgress> cacheMigrationNotify, DistributedTaskQueueOptionsManager options)
        {
            ServiceProvider = serviceProvider;
            TempStream = tempStream;
            CacheMigrationNotify = cacheMigrationNotify;
            Queue = options.Get(nameof(StorageUploader));
        }

        public void Start(int tenantId, StorageSettings newStorageSettings, StorageFactoryConfig storageFactoryConfig)
        {
            lock (Locker)
            {
                var id = GetCacheKey(tenantId);
                var migrateOperation = Queue.GetTask<MigrateOperation>(id);
                if (migrateOperation != null) return;

                migrateOperation = new MigrateOperation(ServiceProvider, CacheMigrationNotify, id, tenantId, newStorageSettings, storageFactoryConfig, TempStream);
                Queue.QueueTask(migrateOperation);
            }
        }

        public MigrateOperation GetProgress(int tenantId)
                {
                    lock (Locker)
                    {
                return Queue.GetTask<MigrateOperation>(GetCacheKey(tenantId));
                    }
        }

        public void Stop()
        {
            foreach (var task in Queue.GetTasks<MigrateOperation>().Where(r => r.Status == DistributedTaskStatus.Running))
            {
                Queue.CancelTask(task.Id);
            }
        }

        private static string GetCacheKey(int tenantId)
        {
            return typeof(MigrateOperation).FullName + tenantId;
        }
    }

    [Transient]
    public class MigrateOperation : DistributedTaskProgress
    {
        private readonly ILog Log;
        private static readonly string ConfigPath;
        private readonly IEnumerable<string> Modules;
        private readonly StorageSettings settings;
        private readonly int tenantId;

        static MigrateOperation()
        {
            ConfigPath = "";
        }

        public MigrateOperation(
            IServiceProvider serviceProvider,
            ICacheNotify<MigrationProgress> cacheMigrationNotify,
            string id,
            int tenantId,
            StorageSettings settings,
            StorageFactoryConfig storageFactoryConfig,
            TempStream tempStream)
        {
            Id = id;
            Status = DistributedTaskStatus.Created;

            ServiceProvider = serviceProvider;
            CacheMigrationNotify = cacheMigrationNotify;
            this.tenantId = tenantId;
            this.settings = settings;
            StorageFactoryConfig = storageFactoryConfig;
            TempStream = tempStream;
            Modules = storageFactoryConfig.GetModuleList(ConfigPath, true);
            StepCount = Modules.Count();
            Log = serviceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
        }

        private IServiceProvider ServiceProvider { get; }
        private StorageFactoryConfig StorageFactoryConfig { get; }
        private TempStream TempStream { get; }
        private ICacheNotify<MigrationProgress> CacheMigrationNotify { get; }

        protected override void DoJob()
        {
            try
            {
                Log.DebugFormat("Tenant: {0}", tenantId);
                Status = DistributedTaskStatus.Running;

                using var scope = ServiceProvider.CreateScope();
                var tempPath = scope.ServiceProvider.GetService<TempPath>();
                var scopeClass = scope.ServiceProvider.GetService<MigrateOperationScope>();
                var (tenantManager, securityContext, storageFactory, options, storageSettingsHelper, settingsManager) = scopeClass;
                var tenant = tenantManager.GetTenant(tenantId);
                tenantManager.SetCurrentTenant(tenant);

                securityContext.AuthenticateMeWithoutCookie(tenant.OwnerId);

                foreach (var module in Modules)
                {
                    var oldStore = storageFactory.GetStorage(ConfigPath, tenantId.ToString(), module);
                    var store = storageFactory.GetStorageFromConsumer(ConfigPath, tenantId.ToString(), module, storageSettingsHelper.DataStoreConsumer(settings));
                    var domains = StorageFactoryConfig.GetDomainList(ConfigPath, module).ToList();

                    var crossModuleTransferUtility = new CrossModuleTransferUtility(options, TempStream, tempPath, oldStore, store);

                    string[] files;
                    foreach (var domain in domains)
                    {
                        //Status = module + domain;
                        Log.DebugFormat("Domain: {0}", domain);
                        files = oldStore.ListFilesRelativeAsync(domain, "\\", "*.*", true).ToArrayAsync().Result;

                        foreach (var file in files)
                        {
                            Log.DebugFormat("File: {0}", file);
                            crossModuleTransferUtility.CopyFileAsync(domain, file, domain, file).Wait();
                        }
                    }

                    Log.Debug("Domain:");

                    files = oldStore.ListFilesRelativeAsync(string.Empty, "\\", "*.*", true).ToArrayAsync().Result
                        .Where(path => domains.All(domain => !path.Contains(domain + "/")))
                        .ToArray();

                    foreach (var file in files)
                    {
                        Log.DebugFormat("File: {0}", file);
                        crossModuleTransferUtility.CopyFileAsync("", file, "", file).Wait();
                    }

                    StepDone();

                    MigrationPublish();
                }

                settingsManager.Save(settings);
                tenant.SetStatus(TenantStatus.Active);
                tenantManager.SaveTenant(tenant);

                Status = DistributedTaskStatus.Completed;
            }
            catch (Exception e)
            {
                Status = DistributedTaskStatus.Failted;
                Exception = e;
                Log.Error(e);
            }

            MigrationPublish();
        }       

        public object Clone()
        {
            return MemberwiseClone();
        }

        private void MigrationPublish()
        {
            CacheMigrationNotify.Publish(new MigrationProgress
            {
                TenantId = tenantId,
                Progress = Percentage,
                Error = Exception.ToString(),
                IsCompleted = IsCompleted
            },
            CacheNotifyAction.Insert);
        }
    }

    public class MigrateOperationScope
    {
        private TenantManager TenantManager { get; }
        private SecurityContext SecurityContext { get; }
        private StorageFactory StorageFactory { get; }
        private IOptionsMonitor<ILog> Options { get; }
        private StorageSettingsHelper StorageSettingsHelper { get; }
        private SettingsManager SettingsManager { get; }

        public MigrateOperationScope(TenantManager tenantManager,
            SecurityContext securityContext,
            StorageFactory storageFactory,
            IOptionsMonitor<ILog> options,
            StorageSettingsHelper storageSettingsHelper,
            SettingsManager settingsManager)
        {
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            StorageFactory = storageFactory;
            Options = options;
            StorageSettingsHelper = storageSettingsHelper;
            SettingsManager = settingsManager;
        }

        public void Deconstruct(out TenantManager tenantManager,
            out SecurityContext securityContext,
            out StorageFactory storageFactory,
            out IOptionsMonitor<ILog> options,
            out StorageSettingsHelper storageSettingsHelper,
            out SettingsManager settingsManager)
        {
            tenantManager = TenantManager;
            securityContext = SecurityContext;
            storageFactory = StorageFactory;
            options = Options;
            storageSettingsHelper = StorageSettingsHelper;
            settingsManager = SettingsManager;
        }
    }
}
