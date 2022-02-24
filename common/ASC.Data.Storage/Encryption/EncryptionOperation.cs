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
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Encryption;
using ASC.Core.Tenants;
using ASC.Data.Storage.DiscStorage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Storage.Encryption
{
    [Transient(Additional = typeof(EncryptionOperationExtension))]
    public class EncryptionOperation : DistributedTaskProgress
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

        public void Init(EncryptionSettingsProto encryptionSettingsProto, string id)
        {
            Id = id;
            EncryptionSettings = new EncryptionSettings(encryptionSettingsProto);
            IsEncryption = EncryptionSettings.Status == EncryprtionStatus.EncryptionStarted;
            ServerRootPath = encryptionSettingsProto.ServerRootPath;
        }

        protected override void DoJob()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<EncryptionOperationScope>();
            var (log, encryptionSettingsHelper, tenantManager, notifyHelper, coreBaseSettings, storageFactoryConfig, storageFactory, configuration) = scopeClass;
            notifyHelper.Init(ServerRootPath);
            Tenants = tenantManager.GetTenants(false);
            Modules = storageFactoryConfig.GetModuleList(ConfigPath, true);
            UseProgressFile = Convert.ToBoolean(configuration["storage:encryption:progressfile"] ?? "true");

            Percentage = 10;
            PublishChanges();

            try
            {
                if (!coreBaseSettings.Standalone)
                {
                    throw new NotSupportedException();
                }

                if (EncryptionSettings.Status == EncryprtionStatus.Encrypted || EncryptionSettings.Status == EncryprtionStatus.Decrypted)
                {
                    log.Debug("Storage already " + EncryptionSettings.Status);
                    return;
                }

                Percentage = 30;
                PublishChanges();

                foreach (var tenant in Tenants)
                {
                    var dictionary = new Dictionary<string, DiscDataStore>();
                    foreach (var module in Modules)
                    {
                        dictionary.Add(module, (DiscDataStore)storageFactory.GetStorage(ConfigPath, tenant.TenantId.ToString(), module));
                    }
                    Parallel.ForEach(dictionary, (elem) =>
                    {
                        EncryptStoreAsync(tenant, elem.Key, elem.Value, storageFactoryConfig, log).Wait();
                    });
                }

                Percentage = 70;
                PublishChanges();

                if (!HasErrors)
                {
                    DeleteProgressFilesAsync(storageFactory).Wait();
                    SaveNewSettings(encryptionSettingsHelper, log);
                }

                Percentage = 90;
                PublishChanges();

                ActivateTenants(tenantManager, log, notifyHelper);

                Percentage = 100;
                PublishChanges();
            }
            catch (Exception e)
            {
                Exception = e;
                log.Error(e);
            }
        }

        private async Task EncryptStoreAsync(Tenant tenant, string module, DiscDataStore store, StorageFactoryConfig storageFactoryConfig, ILog log)
        {
            var domains = storageFactoryConfig.GetDomainList(ConfigPath, module).ToList();

            domains.Add(string.Empty);

            var progress = await ReadProgressAsync(store);

            foreach (var domain in domains)
            {
                var logParent = $"Tenant: {tenant.TenantAlias}, Module: {module}, Domain: {domain}";

                var files = await GetFilesAsync(domains, progress, store, domain);

                EncryptFiles(store, domain, files, logParent, log);
            }

            StepDone();

            log.DebugFormat("Percentage: {0}", Percentage);
        }
      
        private Task<List<string>> ReadProgressAsync(DiscDataStore store)
        {
            if (!UseProgressFile)
            {
                return Task.FromResult(new List<string>());
            }

            return InternalReadProgressAsync(store);
        }

        private async Task<List<string>> InternalReadProgressAsync(DiscDataStore store)
        {
            var encryptedFiles = new List<string>();

            if (await store.IsFileAsync(string.Empty, ProgressFileName))
            {
                using var stream = await store.GetReadStreamAsync(string.Empty, ProgressFileName);
                using var reader = new StreamReader(stream);
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    encryptedFiles.Add(line);
                }
            }
            else
            {
                store.GetWriteStream(string.Empty, ProgressFileName).Close();
            }

            return encryptedFiles;
        }

        private async Task<IEnumerable<string>> GetFilesAsync(List<string> domains, List<string> progress, DiscDataStore targetStore, string targetDomain)
        {
            IEnumerable<string> files = await targetStore.ListFilesRelativeAsync(targetDomain, "\\", "*.*", true).ToListAsync();

            if (progress.Count > 0)
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
                var logItem = $"{logParent}, File: {file}";

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

            using var stream = store.GetWriteStream(string.Empty, ProgressFileName, FileMode.Append);
            using var writer = new StreamWriter(stream);
            writer.WriteLine(file);
        }

        private async Task DeleteProgressFilesAsync(StorageFactory storageFactory)
        {
            foreach (var tenant in Tenants)
            {
                foreach (var module in Modules)
                {
                    var store = (DiscDataStore)storageFactory.GetStorage(ConfigPath, tenant.TenantId.ToString(), module);

                    if (await store.IsFileAsync(string.Empty, ProgressFileName))
                    {
                        await store.DeleteAsync(string.Empty, ProgressFileName);
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

    [Scope]
    public class EncryptionOperationScope
    {
        private ILog Log { get; set; }
        private EncryptionSettingsHelper EncryptionSettingsHelper { get; set; }
        private TenantManager TenantManager { get; set; }
        private NotifyHelper NotifyHelper { get; set; }
        private CoreBaseSettings CoreBaseSettings { get; set; }
        private StorageFactoryConfig StorageFactoryConfig { get; set; }
        private StorageFactory StorageFactory { get; set; }
        private IConfiguration Configuration { get; }

        public EncryptionOperationScope(IOptionsMonitor<ILog> options,
           StorageFactoryConfig storageFactoryConfig,
           StorageFactory storageFactory,
           TenantManager tenantManager,
           CoreBaseSettings coreBaseSettings,
           NotifyHelper notifyHelper,
           EncryptionSettingsHelper encryptionSettingsHelper,
           IConfiguration configuration)
        {
            Log = options.CurrentValue;
            StorageFactoryConfig = storageFactoryConfig;
            StorageFactory = storageFactory;
            TenantManager = tenantManager;
            CoreBaseSettings = coreBaseSettings;
            NotifyHelper = notifyHelper;
            EncryptionSettingsHelper = encryptionSettingsHelper;
            Configuration = configuration;
        }

        public void Deconstruct(out ILog log, out EncryptionSettingsHelper encryptionSettingsHelper, out TenantManager tenantManager, out NotifyHelper notifyHelper, out CoreBaseSettings coreBaseSettings, out StorageFactoryConfig storageFactoryConfig, out StorageFactory storageFactory, out IConfiguration configuration)
        {
            log = Log;
            encryptionSettingsHelper = EncryptionSettingsHelper;
            tenantManager = TenantManager;
            notifyHelper = NotifyHelper;
            coreBaseSettings = CoreBaseSettings;
            storageFactoryConfig = StorageFactoryConfig;
            storageFactory = StorageFactory;
            configuration = Configuration;
        }
    }

    public static class EncryptionOperationExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<EncryptionOperationScope>();
        }
    }
}
