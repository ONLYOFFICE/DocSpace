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

using Tenant = ASC.Core.Tenants.Tenant;

namespace ASC.Data.Storage.Encryption;

[Transient(Additional = typeof(EncryptionOperationExtension))]
public class EncryptionOperation : DistributedTaskProgress
{
    private const string ConfigPath = "";
    private const string ProgressFileName = "EncryptionProgress.tmp";

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private bool _hasErrors = false;
    private EncryptionSettings _encryptionSettings;
    private bool _isEncryption;
    private bool _useProgressFile;
    private IEnumerable<string> _modules;
    private IEnumerable<Tenant> _tenants;
    private string _serverRootPath;

    public EncryptionOperation(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void Init(EncryptionSettingsProto encryptionSettingsProto, string id)
    {
        Id = id;
        _encryptionSettings = new EncryptionSettings(encryptionSettingsProto);
        _isEncryption = _encryptionSettings.Status == EncryprtionStatus.EncryptionStarted;
        _serverRootPath = encryptionSettingsProto.ServerRootPath;
    }

    protected override void DoJob()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<EncryptionOperationScope>();
        var (log, encryptionSettingsHelper, tenantManager, notifyHelper, coreBaseSettings, storageFactoryConfig, storageFactory, configuration) = scopeClass;
        notifyHelper.Init(_serverRootPath);
        _tenants = tenantManager.GetTenants(false);
        _modules = storageFactoryConfig.GetModuleList(ConfigPath, true);
        _useProgressFile = Convert.ToBoolean(configuration["storage:encryption:progressfile"] ?? "true");

        Percentage = 10;
        PublishChanges();

        try
        {
            if (!coreBaseSettings.Standalone)
            {
                throw new NotSupportedException();
            }

            if (_encryptionSettings.Status == EncryprtionStatus.Encrypted || _encryptionSettings.Status == EncryprtionStatus.Decrypted)
            {
                log.Debug("Storage already " + _encryptionSettings.Status);

                return;
            }

            Percentage = 30;
            PublishChanges();

            foreach (var tenant in _tenants)
            {
                var dictionary = new Dictionary<string, DiscDataStore>();

                foreach (var module in _modules)
                {
                    dictionary.Add(module, (DiscDataStore)storageFactory.GetStorage(ConfigPath, tenant.Id.ToString(), module));
                }

                Parallel.ForEach(dictionary, (elem) =>
                {
                        EncryptStoreAsync(tenant, elem.Key, elem.Value, storageFactoryConfig, log).Wait();
                });
            }

            Percentage = 70;
            PublishChanges();

            if (!_hasErrors)
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
                var logParent = $"Tenant: {tenant.Alias}, Module: {module}, Domain: {domain}";

                var files = await GetFilesAsync(domains, progress, store, domain);

            EncryptFiles(store, domain, files, logParent, log);
        }

        StepDone();

        log.DebugFormat("Percentage: {0}", Percentage);
    }

        private Task<List<string>> ReadProgressAsync(DiscDataStore store)
    {
        if (!_useProgressFile)
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
                if (_isEncryption)
                {
                    store.Encrypt(domain, file);
                }
                else
                {
                    store.Decrypt(domain, file);
                }

                WriteProgress(store, file, _useProgressFile);
            }
            catch (Exception e)
            {
                _hasErrors = true;
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
        foreach (var tenant in _tenants)
        {
            foreach (var module in _modules)
            {
                var store = (DiscDataStore)storageFactory.GetStorage(ConfigPath, tenant.Id.ToString(), module);

                    if (await store.IsFileAsync(string.Empty, ProgressFileName))
                {
                        await store.DeleteAsync(string.Empty, ProgressFileName);
                }
            }
        }
    }

    private void SaveNewSettings(EncryptionSettingsHelper encryptionSettingsHelper, ILog log)
    {
        if (_isEncryption)
        {
            _encryptionSettings.Status = EncryprtionStatus.Encrypted;
        }
        else
        {
            _encryptionSettings.Status = EncryprtionStatus.Decrypted;
            _encryptionSettings.Password = string.Empty;
        }

        encryptionSettingsHelper.Save(_encryptionSettings);

        log.Debug("Save new EncryptionSettings");
    }

    private void ActivateTenants(TenantManager tenantManager, ILog log, NotifyHelper notifyHelper)
    {
        foreach (var tenant in _tenants)
        {
            if (tenant.Status == TenantStatus.Encryption)
            {
                tenantManager.SetCurrentTenant(tenant);

                tenant.SetStatus(TenantStatus.Active);
                tenantManager.SaveTenant(tenant);
                log.DebugFormat("Tenant {0} SetStatus Active", tenant.Alias);

                if (!_hasErrors)
                {
                    if (_encryptionSettings.NotifyUsers)
                    {
                        if (_isEncryption)
                        {
                            notifyHelper.SendStorageEncryptionSuccess(tenant.Id);
                        }
                        else
                        {
                            notifyHelper.SendStorageDecryptionSuccess(tenant.Id);
                        }
                        log.DebugFormat("Tenant {0} SendStorageEncryptionSuccess", tenant.Alias);
                    }
                }
                else
                {
                    if (_isEncryption)
                    {
                        notifyHelper.SendStorageEncryptionError(tenant.Id);
                    }
                    else
                    {
                        notifyHelper.SendStorageDecryptionError(tenant.Id);
                    }

                    log.DebugFormat("Tenant {0} SendStorageEncryptionError", tenant.Alias);
                }
            }
        }
    }
}

[Scope]
public class EncryptionOperationScope
{
    private readonly ILog _logger;
    private readonly EncryptionSettingsHelper _encryptionSettingsHelper;
    private readonly TenantManager _tenantManager;
    private readonly NotifyHelper _notifyHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly StorageFactoryConfig _storageFactoryConfig;
    private readonly StorageFactory _storageFactory;
    private readonly IConfiguration _configuration;

    public EncryptionOperationScope(IOptionsMonitor<ILog> options,
       StorageFactoryConfig storageFactoryConfig,
       StorageFactory storageFactory,
       TenantManager tenantManager,
       CoreBaseSettings coreBaseSettings,
       NotifyHelper notifyHelper,
       EncryptionSettingsHelper encryptionSettingsHelper,
       IConfiguration configuration)
    {
        _logger = options.CurrentValue;
        _storageFactoryConfig = storageFactoryConfig;
        _storageFactory = storageFactory;
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _notifyHelper = notifyHelper;
        _encryptionSettingsHelper = encryptionSettingsHelper;
        _configuration = configuration;
    }

    public void Deconstruct(out ILog log, out EncryptionSettingsHelper encryptionSettingsHelper, out TenantManager tenantManager, out NotifyHelper notifyHelper, out CoreBaseSettings coreBaseSettings, out StorageFactoryConfig storageFactoryConfig, out StorageFactory storageFactory, out IConfiguration configuration)
    {
        log = _logger;
        encryptionSettingsHelper = _encryptionSettingsHelper;
        tenantManager = _tenantManager;
        notifyHelper = _notifyHelper;
        coreBaseSettings = _coreBaseSettings;
        storageFactoryConfig = _storageFactoryConfig;
        storageFactory = _storageFactory;
        configuration = _configuration;
    }
}

public static class EncryptionOperationExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EncryptionOperationScope>();
    }
}
