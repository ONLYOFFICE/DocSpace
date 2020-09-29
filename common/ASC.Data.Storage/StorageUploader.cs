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
using System.Linq;

using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Data.Storage.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Storage
{
    public class StorageUploader
    {
        protected readonly DistributedTaskQueue Queue;

        private static readonly object Locker;

        public IServiceProvider ServiceProvider { get; }

        static StorageUploader()
        {
            Locker = new object();
        }

        public StorageUploader(IServiceProvider serviceProvider, DistributedTaskQueueOptionsManager options)
        {
            ServiceProvider = serviceProvider;
            Queue = options.Get(nameof(StorageUploader));
        }

        public void Start(int tenantId, StorageSettings newStorageSettings, StorageFactoryConfig storageFactoryConfig)
        {
            lock (Locker)
            {
                var id = GetCacheKey(tenantId);
                var migrateOperation = Queue.GetTask<MigrateOperation>(id);
                if (migrateOperation != null) return;

                migrateOperation = new MigrateOperation(ServiceProvider, id, tenantId, newStorageSettings, storageFactoryConfig);
                Queue.QueueTask((a, b) => migrateOperation.RunJob(), migrateOperation);
            }
        }

        public MigrateOperation GetProgress(int tenantId)
        {
            lock (Locker)
            {
                return Queue.GetTask<MigrateOperation>(GetCacheKey(tenantId));
            }
        }

        public void Stop(int tenantId)
        {
            Queue.CancelTask(GetCacheKey(tenantId));
        }

        private static string GetCacheKey(int tenantId)
        {
            return typeof(MigrateOperation).FullName + tenantId;
        }
    }

    public class MigrateOperation : DistributedTask, IProgressItem
    {
        private readonly ILog Log;
        private static readonly string ConfigPath;
        private readonly IEnumerable<string> Modules;
        private readonly StorageSettings settings;
        private readonly int tenantId;
        private readonly int StepCount;

        static MigrateOperation()
        {
            ConfigPath = "";
        }

        public MigrateOperation(IServiceProvider serviceProvider, string id, int tenantId, StorageSettings settings, StorageFactoryConfig storageFactoryConfig)
        {
            Id = id;
            Status = DistributedTaskStatus.Created;

            ServiceProvider = serviceProvider;
            this.tenantId = tenantId;
            this.settings = settings;
            StorageFactoryConfig = storageFactoryConfig;
            Modules = storageFactoryConfig.GetModuleList(ConfigPath, true);
            StepCount = Modules.Count();
            Log = serviceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
        }

        public IServiceProvider ServiceProvider { get; }
        public StorageFactoryConfig StorageFactoryConfig { get; }

        public object Error { get; set; }

        public double Percentage { get; set; }

        public bool IsCompleted { get; set; }

        public void RunJob()
        {
            try
            {
                Log.DebugFormat("Tenant: {0}", tenantId);
                Status = DistributedTaskStatus.Running;

                using var scope = ServiceProvider.CreateScope();
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                var tenant = tenantManager.GetTenant(tenantId);
                tenantManager.SetCurrentTenant(tenant);

                var securityContext = scope.ServiceProvider.GetService<SecurityContext>();
                var storageFactory = scope.ServiceProvider.GetService<StorageFactory>();
                var options = scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>();
                var storageSettingsHelper = scope.ServiceProvider.GetService<StorageSettingsHelper>();
                var settingsManager = scope.ServiceProvider.GetService<SettingsManager>();

                securityContext.AuthenticateMe(tenant.OwnerId);

                foreach (var module in Modules)
                {
                    var oldStore = storageFactory.GetStorage(ConfigPath, tenantId.ToString(), module);
                    var store = storageFactory.GetStorageFromConsumer(ConfigPath, tenantId.ToString(), module, storageSettingsHelper.DataStoreConsumer(settings));
                    var domains = StorageFactoryConfig.GetDomainList(ConfigPath, module).ToList();

                    var crossModuleTransferUtility = new CrossModuleTransferUtility(options, oldStore, store);

                    string[] files;
                    foreach (var domain in domains)
                    {
                        //Status = module + domain;
                        Log.DebugFormat("Domain: {0}", domain);
                        files = oldStore.ListFilesRelative(domain, "\\", "*.*", true);

                        foreach (var file in files)
                        {
                            Log.DebugFormat("File: {0}", file);
                            crossModuleTransferUtility.CopyFile(domain, file, domain, file);
                        }
                    }

                    Log.Debug("Domain:");

                    files = oldStore.ListFilesRelative(string.Empty, "\\", "*.*", true)
                        .Where(path => domains.All(domain => !path.Contains(domain + "/")))
                        .ToArray();

                    foreach (var file in files)
                    {
                        Log.DebugFormat("File: {0}", file);
                        crossModuleTransferUtility.CopyFile("", file, "", file);
                    }

                    StepDone();
                }

                settingsManager.Save(settings);
                tenant.SetStatus(TenantStatus.Active);
                tenantManager.SaveTenant(tenant);
                Status = DistributedTaskStatus.Completed;
            }
            catch (Exception e)
            {
                Status = DistributedTaskStatus.Failted;
                Error = e;
                Log.Error(e);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        protected void StepDone()
        {
            if (StepCount > 0)
            {
                Percentage += 100.0 / StepCount;
            }
        }
    }
}
