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

using System.Text.Json;

using ASC.Core.Data;

using ICSharpCode.SharpZipLib.Zip;

namespace ASC.Web.Core;

[Scope]
public class WebPluginManager
{
    private const string StorageModuleName = "webplugins";
    private const string ConfigFileName = "config.json";
    private const string PluginFileName = "plugin.js";
    private const string AssetsFolderName = "assets";

    private readonly DbWebPluginService _webPluginService;
    private readonly IConfiguration _configuration;
    private readonly StorageFactory _storageFactory;
    private readonly AuthContext _authContext;

    private readonly bool _webPluginsEnabled;
    private readonly long _webPluginMaxSize;
    private readonly string _webPluginExtension;
    private readonly List<string> _webPluginAllowedActions;

    public WebPluginManager(
        DbWebPluginService webPluginService,
        IConfiguration configuration,
        StorageFactory storageFactory,
        AuthContext authContext)
    {
        _webPluginService = webPluginService;
        _configuration = configuration;
        _storageFactory = storageFactory;
        _authContext = authContext;

        _ = bool.TryParse(_configuration["plugins:enabled"], out _webPluginsEnabled);
        _ = long.TryParse(_configuration["plugins:max-size"], out _webPluginMaxSize);

        _webPluginExtension = _configuration["plugins:extension"];
        _webPluginAllowedActions = _configuration.GetSection("plugins:allow").Get<List<string>>() ?? new List<string>();
    }

    private void DemandWebPlugins(string action = null)
    {
        if (!_webPluginsEnabled)
        {
            throw new SecurityException("Plugins disabled");
        }

        if (!string.IsNullOrWhiteSpace(action) && !_webPluginAllowedActions.Contains(action))
        {
            throw new SecurityException("Forbidden action");
        }
    }

    public async Task<string> GetPluginUrlTemplate(int tenantId)
    {
        var storage = await _storageFactory.GetStorageAsync(tenantId, StorageModuleName);

        var uri = await storage.GetUriAsync(Path.Combine("{0}", PluginFileName));

        return uri?.ToString() ?? string.Empty;
    }

    public async Task<DbWebPlugin> AddWebPluginFromFile(int tenantId, IFormFile file)
    {
        DemandWebPlugins("upload");

        if (Path.GetExtension(file.FileName)?.ToLowerInvariant() != _webPluginExtension)
        {
            throw new ArgumentException("Wrong file extension");
        }

        if (file.Length > _webPluginMaxSize)
        {
            throw new ArgumentException("File size exceeds limit");
        }

        var storage = await _storageFactory.GetStorageAsync(tenantId, StorageModuleName);

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

                if (webPlugin == null || string.IsNullOrEmpty(webPlugin.Name))
                {
                    throw new ArgumentException("Wrong plugin archive");
                }

                //TODO: think about special characters
                webPlugin.Name = webPlugin.Name.Replace(" ", string.Empty).ToLowerInvariant();

                var exist = await storage.IsDirectoryAsync(string.Empty, webPlugin.Name);
                if (exist)
                {
                    throw new ArgumentException("Plugin already exist");
                }

                webPlugin.TenantId = tenantId;
                webPlugin.CreateBy = _authContext.CurrentAccount.ID;
                webPlugin.CreateOn = DateTime.UtcNow;
                webPlugin.Enabled = true;
                webPlugin.System = false;

                webPlugin = await _webPluginService.SaveAsync(webPlugin);
            }

            using (var stream = zipFile.GetInputStream(pluginFile))
            {
                uri = await storage.SaveAsync(Path.Combine(webPlugin.Name, PluginFileName), stream);
            }

            foreach (ZipEntry zipEntry in zipFile)
            {
                if (zipEntry.IsFile && zipEntry.Name.StartsWith(AssetsFolderName))
                {
                    using (var stream = zipFile.GetInputStream(zipEntry))
                    {
                        uri = await storage.SaveAsync(Path.Combine(webPlugin.Name, zipEntry.Name), stream);
                    }
                }
            }
        }

        return webPlugin;
    }

    public async Task<IEnumerable<DbWebPlugin>> GetWebPluginsAsync(int tenantId, bool? enabled = null)
    {
        DemandWebPlugins();

        var plugins = await _webPluginService.GetAsync(tenantId, enabled);

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

        if (plugin.System)
        {
            throw new SecurityException("System plugin");
        }

        await _webPluginService.UpdateAsync(tenantId, plugin.Id, enabled);
    }

    public async Task DeleteWebPluginAsync(int tenantId, int id)
    {
        DemandWebPlugins("delete");

        var plugin = await _webPluginService.GetByIdAsync(tenantId, id) ?? throw new ItemNotFoundException("Plugin not found");

        if (plugin.System)
        {
            throw new SecurityException("System plugin");
        }

        await _webPluginService.DeleteAsync(tenantId, plugin.Id);

        var storage = await _storageFactory.GetStorageAsync(tenantId, StorageModuleName);

        await storage.DeleteAsync(string.Empty, plugin.Name);
    }
}
