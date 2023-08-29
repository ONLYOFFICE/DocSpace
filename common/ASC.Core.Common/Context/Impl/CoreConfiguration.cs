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

namespace ASC.Core;

[Singletone]
public class CoreBaseSettings
{
    private bool? _standalone;
    private string _basedomain;
    private bool? _personal;
    private bool? _customMode;
    private bool? _disableDocSpace;
    private string _serverRoot;

    private IConfiguration Configuration { get; }

    public CoreBaseSettings(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public string Basedomain
    {
        get
        {
            if (_basedomain == null)
            {
                _basedomain = Configuration["core:base-domain"] ?? string.Empty;
            }

            return _basedomain;
        }
    }

    public string ServerRoot
    {
        get
        {
            if (_serverRoot == null)
            {
                _serverRoot = Configuration["core:server-root"] ?? string.Empty;
            }

            return _serverRoot;
        }
    }

    public bool Standalone => _standalone ?? (bool)(_standalone = Configuration["core:base-domain"] == "localhost");

    public bool Personal =>
            //TODO:if (CustomMode && HttpContext.Current != null && HttpContext.Current.Request.SailfishApp()) return true;
            _personal ?? (bool)(_personal = string.Equals(Configuration["core:personal"], "true", StringComparison.OrdinalIgnoreCase));

    public bool CustomMode => _customMode ?? (bool)(_customMode = string.Equals(Configuration["core:custom-mode"], "true", StringComparison.OrdinalIgnoreCase));

    public bool DisableDocSpace => _disableDocSpace ?? (bool)(_disableDocSpace = string.Equals(Configuration["core:disableDocspace"], "true", StringComparison.OrdinalIgnoreCase));
}

/// <summary>
/// </summary>
[Scope]
public class CoreSettings : IDisposable
{
    /// <summary>Base domain</summary>
    /// <type>System.String, System</type>
    public string BaseDomain
    {
        get
        {
            string result;
            if (CoreBaseSettings.Standalone || string.IsNullOrEmpty(CoreBaseSettings.Basedomain))
            {
                result = GetSetting("BaseDomain") ?? CoreBaseSettings.Basedomain;
            }
            else
            {
                result = CoreBaseSettings.Basedomain;
            }
            return result;
        }
        set
        {
            if (CoreBaseSettings.Standalone || string.IsNullOrEmpty(CoreBaseSettings.Basedomain))
            {
                SaveSetting("BaseDomain", value);
            }
        }
    }

    internal ITenantService TenantService { get; set; }
    internal CoreBaseSettings CoreBaseSettings { get; set; }
    internal IConfiguration Configuration { get; set; }
    internal SemaphoreSlim Semaphore { get; set; }

    public CoreSettings() { }

    public CoreSettings(
        ITenantService tenantService,
        CoreBaseSettings coreBaseSettings,
        IConfiguration configuration)
    {
        TenantService = tenantService;
        CoreBaseSettings = coreBaseSettings;
        Configuration = configuration;
        Semaphore = new SemaphoreSlim(1);
    }

    public string GetBaseDomain(string hostedRegion)
    {
        var baseHost = BaseDomain;

        if (string.IsNullOrEmpty(hostedRegion) || string.IsNullOrEmpty(baseHost) || baseHost.IndexOf('.') < 0)
        {
            return baseHost;
        }
        var subdomain = baseHost.Remove(baseHost.IndexOf('.') + 1);

        return hostedRegion.StartsWith(subdomain) ? hostedRegion : (subdomain + hostedRegion.TrimStart('.'));
    }

    public async Task SaveSettingAsync(string key, string value, int tenant = Tenant.DefaultTenant)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(key);

        byte[] bytes = null;
        if (value != null)
        {
            bytes = Crypto.GetV(Encoding.UTF8.GetBytes(value), 2, true);
        }

        await TenantService.SetTenantSettingsAsync(tenant, key, bytes);
    }

    public void SaveSetting(string key, string value, int tenant = Tenant.DefaultTenant)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(key);

        byte[] bytes = null;
        if (value != null)
        {
            bytes = Crypto.GetV(Encoding.UTF8.GetBytes(value), 2, true);
        }

        TenantService.SetTenantSettings(tenant, key, bytes);
    }

    public async Task<string> GetSettingAsync(string key, int tenant = Tenant.DefaultTenant)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(key);

        var bytes = await TenantService.GetTenantSettingsAsync(tenant, key);

        var result = bytes != null ? Encoding.UTF8.GetString(Crypto.GetV(bytes, 2, false)) : null;

        return result;
    }

    public string GetSetting(string key, int tenant = Tenant.DefaultTenant)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(key);

        var bytes = TenantService.GetTenantSettings(tenant, key);

        var result = bytes != null ? Encoding.UTF8.GetString(Crypto.GetV(bytes, 2, false)) : null;

        return result;
    }

    public async Task<string> GetKeyAsync(int tenant)
    {
        if (CoreBaseSettings.Standalone)
        {
            var key = await GetSettingAsync("PortalId");
            if (string.IsNullOrEmpty(key))
            {
                try
                {
                    await Semaphore.WaitAsync();
                    // thread safe
                    key = await GetSettingAsync("PortalId");
                    if (string.IsNullOrEmpty(key))
                    {
                        key = Guid.NewGuid().ToString();
                        await SaveSettingAsync("PortalId", key);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Semaphore.Release();
                }
            }

            return key;
        }
        else
        {
            var t = await TenantService.GetTenantAsync(tenant);
            if (t != null && !string.IsNullOrWhiteSpace(t.PaymentId))
            {
                return t.PaymentId;
            }

            return Configuration["core:payment:region"] + tenant;
        }
    }

    public string GetKey(int tenant)
    {
        if (CoreBaseSettings.Standalone)
        {
            var key = GetSetting("PortalId");
            if (string.IsNullOrEmpty(key))
            {
                try
                {
                    Semaphore.Wait();
                    // thread safe
                    key = GetSetting("PortalId");
                    if (string.IsNullOrEmpty(key))
                    {
                        key = Guid.NewGuid().ToString();
                        SaveSetting("PortalId", key);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Semaphore.Release();
                }
            }

            return key;
        }
        else
        {
            var t = TenantService.GetTenant(tenant);
            if (t != null && !string.IsNullOrWhiteSpace(t.PaymentId))
            {
                return t.PaymentId;
            }

            return Configuration["core:payment:region"] + tenant;
        }
    }

    public async Task<string> GetAffiliateIdAsync(int tenant)
    {
        var t = await TenantService.GetTenantAsync(tenant);
        if (t != null && !string.IsNullOrWhiteSpace(t.AffiliateId))
        {
            return t.AffiliateId;
        }

        return null;
    }

    public async Task<string> GetCampaignAsync(int tenant)
    {
        var t = await TenantService.GetTenantAsync(tenant);
        if (t != null && !string.IsNullOrWhiteSpace(t.Campaign))
        {
            return t.Campaign;
        }

        return null;
    }

    public void Dispose()
    {
        Semaphore.Dispose();
    }
}

[Scope]
public class CoreConfiguration
{
    private long? _personalMaxSpace;

    public CoreConfiguration(CoreSettings coreSettings, TenantManager tenantManager, IConfiguration configuration)
    {
        _coreSettings = coreSettings;
        _tenantManager = tenantManager;
        _configuration = configuration;
    }

    public async Task<long> PersonalMaxSpaceAsync(SettingsManager settingsManager)
    {
        var quotaSettings = await settingsManager.LoadForCurrentUserAsync<PersonalQuotaSettings>();

        if (quotaSettings.MaxSpace != long.MaxValue)
        {
            return quotaSettings.MaxSpace;
        }

        if (_personalMaxSpace.HasValue)
        {
            return _personalMaxSpace.Value;
        }

        if (!long.TryParse(_configuration["core:personal.maxspace"], out var value))
        {
            value = long.MaxValue;
        }

        _personalMaxSpace = value;

        return _personalMaxSpace.Value;
    }

    public async Task<SmtpSettings> GetDefaultSmtpSettingsAsync()
    {
        var isDefaultSettings = false;
        var tenant = await _tenantManager.GetCurrentTenantAsync(false);

        if (tenant != null)
        {

            var settingsValue = await GetSettingAsync("SmtpSettings", tenant.Id);
            if (string.IsNullOrEmpty(settingsValue))
            {
                isDefaultSettings = true;
                settingsValue = await GetSettingAsync("SmtpSettings");
            }
            var settings = SmtpSettings.Deserialize(settingsValue);
            settings.IsDefaultSettings = isDefaultSettings;

            return settings;
        }
        else
        {
            var settingsValue = await GetSettingAsync("SmtpSettings");

            var settings = SmtpSettings.Deserialize(settingsValue);
            settings.IsDefaultSettings = true;

            return settings;
        }
    }

    public async Task SetSmtpSettingsAsync(SmtpSettings value)
    {
        await SaveSettingAsync("SmtpSettings", value?.Serialize(), await _tenantManager.GetCurrentTenantIdAsync());
    }

    private readonly CoreSettings _coreSettings;
    private readonly TenantManager _tenantManager;
    private readonly IConfiguration _configuration;

    #region Methods Get/Save Setting

    public async Task SaveSettingAsync(string key, string value, int tenant = Tenant.DefaultTenant)
    {
        await _coreSettings.SaveSettingAsync(key, value, tenant);
    }

    public async Task<string> GetSettingAsync(string key, int tenant = Tenant.DefaultTenant)
    {
        return await _coreSettings.GetSettingAsync(key, tenant);
    }

    public string GetSetting(string key, int tenant = Tenant.DefaultTenant)
    {
        return _coreSettings.GetSetting(key, tenant);
    }

    #endregion

    #region Methods Get/Set Section

    public async Task<T> GetSectionAsync<T>() where T : class
    {
        return await GetSectionAsync<T>(typeof(T).Name);
    }

    public async Task<T> GetSectionAsync<T>(int tenantId) where T : class
    {
        return await GetSectionAsync<T>(tenantId, typeof(T).Name);
    }

    public async Task<T> GetSectionAsync<T>(string sectionName) where T : class
    {
        return await GetSectionAsync<T>(await _tenantManager.GetCurrentTenantIdAsync(), sectionName);
    }

    public async Task<T> GetSectionAsync<T>(int tenantId, string sectionName) where T : class
    {
        var serializedSection = await GetSettingAsync(sectionName, tenantId);
        if (serializedSection == null && tenantId != Tenant.DefaultTenant)
        {
            serializedSection = await GetSettingAsync(sectionName, Tenant.DefaultTenant);
        }

        return serializedSection != null ? JsonConvert.DeserializeObject<T>(serializedSection) : null;
    }

    public async Task SaveSectionAsync<T>(string sectionName, T section) where T : class
    {
        await SaveSectionAsync(await _tenantManager.GetCurrentTenantIdAsync(), sectionName, section);
    }

    public async Task SaveSectionAsync<T>(T section) where T : class
    {
        await SaveSectionAsync(typeof(T).Name, section);
    }

    public async Task SaveSectionAsync<T>(int tenantId, T section) where T : class
    {
        await SaveSectionAsync(tenantId, typeof(T).Name, section);
    }

    public async Task SaveSectionAsync<T>(int tenantId, string sectionName, T section) where T : class
    {
        var serializedSection = section != null ? JsonConvert.SerializeObject(section) : null;
        await SaveSettingAsync(sectionName, serializedSection, tenantId);
    }

    #endregion
}
