/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage.DiscStorage;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ASC.Common;
using Microsoft.Extensions.Configuration;
using ASC.Common.Caching;
using System.Threading.Tasks;

namespace ASC.Data.Storage.Encryption
{
    public class EncryptionOperation : ProgressBase
    {
        private const string ConfigPath = "";
        private bool HasErrors = false;
        private const string ProgressFileName = "EncryptionProgress.tmp";
        private IServiceProvider ServiceProvider { get; }
        private EncryptionSettings EncryptionSettings { get; set; }
        private bool IsEncryption { get; set; }
        private bool UseProgressFile { get; set; }
        private IEnumerable<string> Modules { get; set; }
        private IEnumerable<Tenant> Tenants { get; set; }
        private string ServerRootPath { get; set; }

        public EncryptionOperation(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Init(EncryptionSettingsProto encryptionSettingsProto)
        {
            EncryptionSettings = new EncryptionSettings(encryptionSettingsProto);
            IsEncryption = EncryptionSettings.Status == EncryprtionStatus.EncryptionStarted;
            ServerRootPath = encryptionSettingsProto.ServerRootPath;
        }

        protected override void DoJob()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<EncryptionOperationScope>();
            var (log, encryptionSettingsHelper, tenantManager, notifyHelper, coreBaseSettings, storageFactoryConfig, storageFactory, progressEncryption, configuration) = scopeClass;
            notifyHelper.Init(ServerRootPath);
            Tenants = tenantManager.GetTenants(false);
            Modules = storageFactoryConfig.GetModuleList(ConfigPath, true);
            UseProgressFile = Convert.ToBoolean(configuration["storage:encryption:progressfile"] ?? "true");

            Percentage = 10;
            GetProgress(progressEncryption);
            try
            {
                if (!coreBaseSettings.Standalone)
                {
                  //  throw new NotSupportedException();
                }

                if (EncryptionSettings.Status == EncryprtionStatus.Encrypted || EncryptionSettings.Status == EncryprtionStatus.Decrypted)
                {
                    log.Debug("Storage already " + EncryptionSettings.Status);
                    return;
                }
                Percentage = 30;
                GetProgress(progressEncryption);
                foreach (var tenant in Tenants)
                {
                    Queue<DiscDataStore> queue = new Queue<DiscDataStore>();
                    foreach (var module in Modules)
                    {
                        queue.Enqueue((DiscDataStore)storageFactory.GetStorage(ConfigPath, tenant.TenantId.ToString(), module));
                    }
                    Parallel.ForEach(Modules, (module) =>
                    {
                        EncryptStore(tenant, module, queue.Dequeue(), storageFactoryConfig, log);
                    });
                }
                Percentage = 70;
                GetProgress(progressEncryption);
                if (!HasErrors)
                {
                    DeleteProgressFiles(storageFactory);
                    SaveNewSettings(encryptionSettingsHelper, log);
                }
                Percentage = 90;
                GetProgress(progressEncryption);
                ActivateTenants(tenantManager, log, notifyHelper);
            }
            catch (Exception e)
            {
                Error = e;
                log.Error(e);
            }
        }


        private void EncryptStore(Tenant tenant, string module, DiscDataStore store, StorageFactoryConfig storageFactoryConfig, ILog log)
        {
           // var store = (DiscDataStore)storageFactory.GetStorage(ConfigPath, tenant.TenantId.ToString(), module);

            var domains = storageFactoryConfig.GetDomainList(ConfigPath, module).ToList();

            domains.Add(string.Empty);

            var progress = ReadProgress(store);

            foreach (var domain in domains)
            {
                var logParent = string.Format("Tenant: {0}, Module: {1}, Domain: {2}", tenant.TenantAlias, module, domain);

                var files = GetFiles(domains, progress, store, domain);

                EncryptFiles(store, domain, files, logParent, log);
            }

            StepDone();

            log.DebugFormat("Percentage: {0}", Percentage);
        }

        private List<string> ReadProgress(DiscDataStore store)
        {
            var encryptedFiles = new List<string>();

            if (!UseProgressFile)
            {
                return encryptedFiles;
            }

            if (store.IsFile(string.Empty, ProgressFileName))
            {
                using (var stream = store.GetReadStream(string.Empty, ProgressFileName))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            encryptedFiles.Add(line);
                        }
                    }
                }
            }
            else
            {
                store.GetWriteStream(string.Empty, ProgressFileName).Close();
            }

            return encryptedFiles;
        }

        public void GetProgress(ICacheNotify<ProgressEncryption> progress)
        {
            var progressEncryption = new ProgressEncryption()
            {
                Proggress = Percentage
            };
            progress.Publish(progressEncryption, CacheNotifyAction.Insert);
        }

        private IEnumerable<string> GetFiles(List<string> domains, List<string> progress, DiscDataStore targetStore, string targetDomain)
        {
            IEnumerable<string> files = targetStore.ListFilesRelative(targetDomain, "\\", "*.*", true);

            if (progress.Any())
            {
                files = files.Where(path => !progress.Contains(path));
            }

            if (!string.IsNullOrEmpty(targetDomain))
            {
                return files;
            }

            var notEmptyDomains = domains.Where(domain => !string.IsNullOrEmpty(domain));

            if (notEmptyDomains.Any())
            {
                files = files.Where(path => notEmptyDomains.All(domain => !path.Contains(domain + Path.DirectorySeparatorChar)));
            }

            files = files.Where(path => !path.EndsWith(ProgressFileName));

            return files;
        }

        private void EncryptFiles(DiscDataStore store, string domain, IEnumerable<string> files, string logParent, ILog log)
        {
            foreach (var file in files)
            {
                var logItem = string.Format("{0}, File: {1}", logParent, file);

                log.Debug(logItem);

                try
                {
                    if (IsEncryption)
                    {
                        store.Encrypt(domain, file);
                    }
                    else
                    {
                        store.Decrypt(domain, file);
                    }

                    WriteProgress(store, file, UseProgressFile);
                }
                catch (Exception e)
                {
                    HasErrors = true;
                    log.Error(logItem + " " + e.Message, e);

                    // ERROR_DISK_FULL: There is not enough space on the disk.
                    // if (e is IOException && e.HResult == unchecked((int)0x80070070)) break;
                }
            }
        }

        private void WriteProgress(DiscDataStore store, string file, bool useProgressFile)
        {
            if (!useProgressFile)
            {
                return;
            }

            using (var stream = store.GetWriteStream(string.Empty, ProgressFileName, FileMode.Append))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(file);
                }
            }
        }

        private void DeleteProgressFiles(StorageFactory storageFactory)
        {
            foreach (var tenant in Tenants)
            {
                foreach (var module in Modules)
                {
                    var store = (DiscDataStore)storageFactory.GetStorage(ConfigPath, tenant.TenantId.ToString(), module);

                    if (store.IsFile(string.Empty, ProgressFileName))
                    {
                        store.Delete(string.Empty, ProgressFileName);
                    }
                }
            }
        }

        private void SaveNewSettings(EncryptionSettingsHelper encryptionSettingsHelper, ILog log)
        {
            if (IsEncryption)
            {
                EncryptionSettings.Status = EncryprtionStatus.Encrypted;
            }
            else
            {
                EncryptionSettings.Status = EncryprtionStatus.Decrypted;
                EncryptionSettings.Password = string.Empty;
            }

            encryptionSettingsHelper.Save(EncryptionSettings);

            log.Debug("Save new EncryptionSettings");
        }

        private void ActivateTenants(TenantManager tenantManager, ILog log, NotifyHelper notifyHelper)
        {
            foreach (var tenant in Tenants)
            {
                if (tenant.Status == TenantStatus.Encryption)
                {
                    tenantManager.SetCurrentTenant(tenant);

                    tenant.SetStatus(TenantStatus.Active);
                    tenantManager.SaveTenant(tenant);
                    log.DebugFormat("Tenant {0} SetStatus Active", tenant.TenantAlias);

                    if (!HasErrors)
                    {
                       if (EncryptionSettings.NotifyUsers)
                        {
                            if (IsEncryption)
                            {
                                notifyHelper.SendStorageEncryptionSuccess(tenant.TenantId);
                            }
                            else
                            {
                                notifyHelper.SendStorageDecryptionSuccess(tenant.TenantId);
                            }
                            log.DebugFormat("Tenant {0} SendStorageEncryptionSuccess", tenant.TenantAlias);
                        }
                    }
                    else
                    {
                        if (IsEncryption)
                        {
                            notifyHelper.SendStorageEncryptionError(tenant.TenantId);
                        }
                        else
                        {
                            notifyHelper.SendStorageDecryptionError(tenant.TenantId);
                        }
                        log.DebugFormat("Tenant {0} SendStorageEncryptionError", tenant.TenantAlias);
                    }
                }
            }
        }
    }

    public class EncryptionOperationScope 
    {
        private ILog Log { get; set; }
        private EncryptionSettingsHelper EncryptionSettingsHelper { get; set; }
        private TenantManager TenantManager { get; set; }
        private NotifyHelper NotifyHelper { get; set; }
        private CoreBaseSettings CoreBaseSettings { get; set; }
        private StorageFactoryConfig StorageFactoryConfig { get; set; }
        private StorageFactory StorageFactory { get; set; }
        private ICacheNotify<ProgressEncryption> ProgressEncryption { get; }
        private IConfiguration Configuration { get; }

        public EncryptionOperationScope(IOptionsMonitor<ILog> options,
           StorageFactoryConfig storageFactoryConfig,
           StorageFactory storageFactory,
           TenantManager tenantManager,
           CoreBaseSettings coreBaseSettings,
           NotifyHelper notifyHelper,
           EncryptionSettingsHelper encryptionSettingsHelper,
           IConfiguration configuration,
           ICacheNotify<ProgressEncryption> progressEncryption)
        {
            Log = options.CurrentValue;
            StorageFactoryConfig = storageFactoryConfig;
            StorageFactory = storageFactory;
            TenantManager = tenantManager;
            CoreBaseSettings = coreBaseSettings;
            NotifyHelper = notifyHelper;
            EncryptionSettingsHelper = encryptionSettingsHelper;
            ProgressEncryption = progressEncryption;
            Configuration = configuration;
        }

        public void Deconstruct( out ILog log, out EncryptionSettingsHelper encryptionSettingsHelper, out TenantManager tenantManager, out NotifyHelper notifyHelper, out CoreBaseSettings coreBaseSettings, out StorageFactoryConfig storageFactoryConfig, out StorageFactory storageFactory, out ICacheNotify<ProgressEncryption> progressEncryption, out IConfiguration configuration)
        {
            log = Log;
            encryptionSettingsHelper = EncryptionSettingsHelper;
            tenantManager = TenantManager;
            notifyHelper = NotifyHelper;
            coreBaseSettings = CoreBaseSettings;
            storageFactoryConfig = StorageFactoryConfig;
            storageFactory = StorageFactory;
            progressEncryption = ProgressEncryption;
            configuration = Configuration;
    }
    }

    public static class EncryptionOperationExtension
    {
        public static DIHelper AddEncryptionOperationService(this DIHelper services)
        {
            services.TryAddTransient<EncryptionOperation>();
            services.TryAddTransient<EncryptionOperationScope>();
            return services
                .AddStorageFactoryConfigService()
                .AddStorageFactoryService()
                .AddNotifyHelperService()
                .AddEncryptionSettingsHelperService();
        }
    }
}
