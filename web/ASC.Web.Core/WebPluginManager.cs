// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Core;

[Singletone]
public class WebPluginCache
{
    private readonly ICache _сache;
    private readonly ICacheNotify<WebPluginCacheItem> _notify;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);

    public WebPluginCache(ICacheNotify<WebPluginCacheItem> notify, ICache cache)
    {
        _сache = cache;
        _notify = notify;

        _notify.Subscribe((i) => _сache.Remove(i.Key), CacheNotifyAction.Remove);
    }

    public List<DbWebPlugin> Get(string key)
    {
        return _сache.Get<List<DbWebPlugin>>(key);
    }

    public void Insert(string key, object value)
    {
        _notify.Publish(new WebPluginCacheItem { Key = key }, CacheNotifyAction.Remove);

        _сache.Insert(key, value, _cacheExpiration);
    }

    public void Remove(string key)
    {
        _notify.Publish(new WebPluginCacheItem { Key = key }, CacheNotifyAction.Remove);

        _сache.Remove(key);
    }
}

[Scope]
public class WebPluginManager
{
    private const string StorageSystemModuleName = "systemwebplugins";
    private const string StorageModuleName = "webplugins";
    private const string ConfigFileName = "config.json";
    private const string PluginFileName = "plugin.js";
    private const string AssetsFolderName = "assets";

    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly SettingsManager _settingsManager;
    private readonly DbWebPluginService _webPluginService;
    private readonly WebPluginSettings _webPluginSettings;
    private readonly WebPluginCache _webPluginCache;
    private readonly StorageFactory _storageFactory;
    private readonly AuthContext _authContext;
    private readonly ILogger<WebPluginManager> _log;

    public WebPluginManager(
        CoreBaseSettings coreBaseSettings,
        SettingsManager settingsManager,
        DbWebPluginService webPluginService,
        WebPluginSettings webPluginSettings,
        WebPluginCache webPluginCache,
        StorageFactory storageFactory,
        AuthContext authContext,
        ILogger<WebPluginManager> log)
    {
        _coreBaseSettings = coreBaseSettings;
        _settingsManager = settingsManager;
        _webPluginService = webPluginService;
        _webPluginSettings = webPluginSettings;
        _webPluginCache = webPluginCache;
        _storageFactory = storageFactory;
        _authContext = authContext;
        _log = log;
    }

    private void DemandWebPlugins(string action = null)
    {
        if (!_webPluginSettings.Enabled)
        {
            throw new SecurityException("Plugins disabled");
        }

        if (!string.IsNullOrWhiteSpace(action) && _webPluginSettings.Allow.Any() && !_webPluginSettings.Allow.Contains(action))
        {
            throw new SecurityException("Forbidden action");
        }
    }

    private async Task<IDataStore> GetPluginStorageAsync(int tenantId)
    {
        var module = tenantId == Tenant.DefaultTenant ? StorageSystemModuleName : StorageModuleName;

        var storage = await _storageFactory.GetStorageAsync(tenantId, module);

        return storage;
    }

    private static string GetCacheKey(int tenantId)
    {
        return $"{StorageModuleName}:{tenantId}";
    }

    public async Task<string> GetPluginUrlTemplateAsync(int tenantId)
    {
        var storage = await GetPluginStorageAsync(tenantId);

        var uri = await storage.GetUriAsync(Path.Combine("{0}", PluginFileName));

        return uri?.ToString() ?? string.Empty;
    }

    public async Task<DbWebPlugin> AddWebPluginFromFileAsync(int tenantId, IFormFile file)
    {
        DemandWebPlugins("upload");

        var system = tenantId == Tenant.DefaultTenant;

        if (system && !_coreBaseSettings.Standalone)
        {
            throw new SecurityException("System plugin");
        }

        var webPlugin = await SaveWebPluginToStorageAsync(tenantId, file);

        webPlugin.TenantId = tenantId;
        webPlugin.Enabled = true;
        webPlugin.System = system;

        if (!system)
        {
            var existingPlugin = await _webPluginService.GetByNameAsync(tenantId, webPlugin.Name);

            if (existingPlugin != null)
            {
                webPlugin.Id = existingPlugin.Id;
            }

            webPlugin.CreateBy = _authContext.CurrentAccount.ID;
            webPlugin.CreateOn = DateTime.UtcNow;

            webPlugin = await _webPluginService.SaveAsync(webPlugin);
        }

        var key = GetCacheKey(tenantId);

        _webPluginCache.Remove(key);

        return webPlugin;
    }

    private async Task<DbWebPlugin> SaveWebPluginToStorageAsync(int tenantId, IFormFile file)
    {
        if (Path.GetExtension(file.FileName)?.ToLowerInvariant() != _webPluginSettings.Extension)
        {
            throw new ArgumentException("Wrong file extension");
        }

        if (file.Length > _webPluginSettings.MaxSize)
        {
            throw new ArgumentException("File size exceeds limit");
        }

        var storage = await GetPluginStorageAsync(tenantId);

        DbWebPlugin webPlugin = null;
        Uri uri = null;

        using (var zipFile = new ZipFile(file.OpenReadStream()))
        {
            var configFile = zipFile.GetEntry(ConfigFileName);
            var pluginFile = zipFile.GetEntry(PluginFileName);

            if (configFile == null || pluginFile == null)
            {
                throw new ArgumentException("Wrong plugin archive");
            }

            using (var stream = zipFile.GetInputStream(configFile))
            using (var reader = new StreamReader(stream))
            {
                var configContent = reader.ReadToEnd();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                webPlugin = System.Text.Json.JsonSerializer.Deserialize<DbWebPlugin>(configContent, options);

                if (webPlugin == null)
                {
                    throw new ArgumentException("Wrong plugin archive");
                }

                var nameRegex = new Regex(@"^[a-z0-9_.-]+$");

                if (string.IsNullOrEmpty(webPlugin.Name) || !nameRegex.IsMatch(webPlugin.Name) || webPlugin.Name.StartsWith('.'))
                {
                    throw new ArgumentException("Wrong plugin name");
                }

                if (await storage.IsDirectoryAsync(webPlugin.Name))
                {
                    await storage.DeleteDirectoryAsync(webPlugin.Name);
                }

                uri = await storage.SaveAsync(Path.Combine(webPlugin.Name, ConfigFileName), stream);
            }

            using (var stream = zipFile.GetInputStream(pluginFile))
            {
                uri = await storage.SaveAsync(Path.Combine(webPlugin.Name, PluginFileName), stream);
            }

            foreach (ZipEntry zipEntry in zipFile)
            {
                if (zipEntry.IsFile && zipEntry.Name.StartsWith(AssetsFolderName))
                {
                    var ext = Path.GetExtension(zipEntry.Name);

                    if (_webPluginSettings.AssetExtensions.Any() && !_webPluginSettings.AssetExtensions.Contains(ext))
                    {
                        continue;
                    }

                    using (var stream = zipFile.GetInputStream(zipEntry))
                    {
                        uri = await storage.SaveAsync(Path.Combine(webPlugin.Name, zipEntry.Name), stream);
                    }
                }
            }
        }

        return webPlugin;
    }

    public async Task<List<DbWebPlugin>> GetWebPluginsAsync(int tenantId)
    {
        DemandWebPlugins();

        var key = GetCacheKey(tenantId);

        var plugins = _webPluginCache.Get(key);

        if (plugins == null)
        {
            plugins = await _webPluginService.GetAsync(tenantId);

            _webPluginCache.Insert(key, plugins);
        }

        return plugins;
    }

    public async Task<DbWebPlugin> GetWebPluginByIdAsync(int tenantId, int id)
    {
        DemandWebPlugins();

        var plugin = await _webPluginService.GetByIdAsync(tenantId, id) ?? throw new ItemNotFoundException("Plugin not found");

        return plugin;
    }

    public async Task UpdateWebPluginAsync(int tenantId, int id, bool enabled)
    {
        DemandWebPlugins();

        var plugin = await _webPluginService.GetByIdAsync(tenantId, id) ?? throw new ItemNotFoundException("Plugin not found");

        await _webPluginService.UpdateAsync(tenantId, plugin.Id, enabled);

        var key = GetCacheKey(tenantId);

        _webPluginCache.Remove(key);
    }

    public async Task DeleteWebPluginAsync(int tenantId, int id)
    {
        DemandWebPlugins("delete");

        var plugin = await _webPluginService.GetByIdAsync(tenantId, id) ?? throw new ItemNotFoundException("Plugin not found");

        await _webPluginService.DeleteAsync(tenantId, plugin.Id);

        var storage = await GetPluginStorageAsync(tenantId);

        await storage.DeleteDirectoryAsync(plugin.Name);

        var key = GetCacheKey(tenantId);

        _webPluginCache.Remove(key);
    }



    public async Task<List<DbWebPlugin>> GetSystemWebPluginsAsync()
    {
        var key = GetCacheKey(Tenant.DefaultTenant);

        var systemPlugins = _webPluginCache.Get(key);

        if (systemPlugins == null)
        {
            systemPlugins = await GetSystemWebPluginsFromStorageAsync();

            _webPluginCache.Insert(key, systemPlugins);
        }

        return systemPlugins;
    }

    private async Task<List<DbWebPlugin>> GetSystemWebPluginsFromStorageAsync()
    {
        var systemPlugins = new List<DbWebPlugin>();

        var systemWebPluginSettings = await _settingsManager.LoadForDefaultTenantAsync<SystemWebPluginSettings>();

        var disabledPlugins = systemWebPluginSettings?.DisabledPlugins ?? new List<string>();

        var storage = await GetPluginStorageAsync(Tenant.DefaultTenant);

        var configFiles = await storage.ListFilesRelativeAsync(string.Empty, string.Empty, ConfigFileName, true).ToArrayAsync();

        foreach (var path in configFiles)
        {
            try
            {
                using var readStream = await storage.GetReadStreamAsync(path);

                using var reader = new StreamReader(readStream);

                var configContent = reader.ReadToEnd();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var webPlugin = System.Text.Json.JsonSerializer.Deserialize<DbWebPlugin>(configContent, options);

                webPlugin.TenantId = Tenant.DefaultTenant;
                webPlugin.System = true;
                webPlugin.Enabled = !disabledPlugins.Contains(webPlugin.Name);

                systemPlugins.Add(webPlugin);
            }
            catch (Exception e)
            {
                _log.ErrorWithException(e);
            }
        }

        return systemPlugins;
    }

    public async Task<DbWebPlugin> GetSystemWebPluginAsync(string name)
    {
        var systemWebPluginSettings = await _settingsManager.LoadForDefaultTenantAsync<SystemWebPluginSettings>();

        var disabledPlugins = systemWebPluginSettings?.DisabledPlugins ?? new List<string>();

        var storage = await GetPluginStorageAsync(Tenant.DefaultTenant);

        var path = Path.Combine(name, ConfigFileName);

        if (!await storage.IsFileAsync(path))
        {
            throw new ItemNotFoundException("Plugin not found");
        }

        using var readStream = await storage.GetReadStreamAsync(path);

        using var reader = new StreamReader(readStream);

        var configContent = reader.ReadToEnd();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var webPlugin = System.Text.Json.JsonSerializer.Deserialize<DbWebPlugin>(configContent, options);

        webPlugin.TenantId = Tenant.DefaultTenant;
        webPlugin.System = true;
        webPlugin.Enabled = !disabledPlugins.Contains(webPlugin.Name);

        return webPlugin;
    }

    public async Task UpdateSystemWebPluginAsync(string name, bool enabled)
    {
        DemandWebPlugins();

        var systemWebPluginSettings = await _settingsManager.LoadForDefaultTenantAsync<SystemWebPluginSettings>();

        var disabledPlugins = systemWebPluginSettings?.DisabledPlugins ?? new List<string>();

        if (enabled)
        {
            disabledPlugins.Remove(name);
        }
        else
        {
            disabledPlugins.Add(name);
        }

        systemWebPluginSettings.DisabledPlugins = disabledPlugins.Any() ? disabledPlugins : null;

        await _settingsManager.SaveForDefaultTenantAsync(systemWebPluginSettings);

        var key = GetCacheKey(Tenant.DefaultTenant);

        _webPluginCache.Remove(key);
    }

    public async Task DeleteSystemWebPluginAsync(string name)
    {
        DemandWebPlugins("delete");

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException("System plugin");
        }

        var storage = await GetPluginStorageAsync(Tenant.DefaultTenant);

        if (!await storage.IsDirectoryAsync(name))
        {
            throw new ItemNotFoundException("Plugin not found");
        }

        await UpdateSystemWebPluginAsync(name, true);

        await storage.DeleteDirectoryAsync(name);
    }
}
