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

namespace ASC.Web.Core.WhiteLabel;

[Scope]
public class TenantLogoManager
{
    private string CacheKey
    {
        get { return "letterlogodata" + _tenantManager.GetCurrentTenant().Id; }
    }

    public bool WhiteLabelEnabled { get; private set; }

    private readonly IDistributedCache _distributedCache;

    public TenantLogoManager(
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        SettingsManager settingsManager,
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        TenantManager tenantManager,
        AuthContext authContext,
        IConfiguration configuration,
        IDistributedCache distributedCache)
    {
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _settingsManager = settingsManager;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _configuration = configuration;
        var hideSettings = (_configuration["web:hide-settings"] ?? "").Split(new[] { ',', ';', ' ' });
        WhiteLabelEnabled = !hideSettings.Contains("WhiteLabel", StringComparer.CurrentCultureIgnoreCase);
        _distributedCache = distributedCache;
    }

    public async Task<string> GetFaviconAsync(bool timeParam, bool dark)
    {
        string faviconPath;
        var tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();
        if (WhiteLabelEnabled)
        {
            faviconPath = await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Favicon, dark);
            if (timeParam)
            {
                var now = DateTime.Now;
                faviconPath = string.Format("{0}?t={1}", faviconPath, now.Ticks);
            }
        }
        else
        {
            faviconPath = await _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPathAsync(WhiteLabelLogoTypeEnum.Favicon, dark);
        }

        return faviconPath;
    }

    public async Task<string> GetTopLogoAsync(bool dark)//LogoLightSmall
    {
        var tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.LightSmall, dark);
        }
        return await _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPathAsync(WhiteLabelLogoTypeEnum.LightSmall, dark);
    }

    public async Task<string> GetLogoDarkAsync(bool dark)
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();
            return await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Notification, dark);
        }

        /*** simple scheme ***/
        return await _tenantInfoSettingsHelper.GetAbsoluteCompanyLogoPathAsync(await _settingsManager.LoadAsync<TenantInfoSettings>());
        /***/
    }

    public async Task<string> GetLogoDocsEditorAsync(bool dark)
    {
        var tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditor, dark);
        }
        return await _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPathAsync(WhiteLabelLogoTypeEnum.DocsEditor, dark);
    }

    public async Task<string> GetLogoDocsEditorEmbedAsync(bool dark)
    {
        var tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        if (WhiteLabelEnabled)
        {
            return await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditorEmbed, dark);
        }
        return await _tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPathAsync(WhiteLabelLogoTypeEnum.DocsEditorEmbed, dark);
    }


    public async Task<string> GetLogoTextAsync()
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

            return await tenantWhiteLabelSettings.GetLogoTextAsync(_settingsManager) ?? TenantWhiteLabelSettings.DefaultLogoText;
        }
        return TenantWhiteLabelSettings.DefaultLogoText;
    }

    public bool IsRetina(HttpRequest request)
    {
        if (request != null)
        {
            var cookie = request.Cookies["is_retina"];
            if (cookie != null && !string.IsNullOrEmpty(cookie))
            {
                if (bool.TryParse(cookie, out var result))
                {
                    return result;
                }
            }
        }
        return !_authContext.IsAuthenticated;
    }

    public async Task<bool> GetWhiteLabelPaidAsync()
    {
        return (await _tenantManager.GetTenantQuotaAsync(await _tenantManager.GetCurrentTenantIdAsync())).WhiteLabel;
    }

    private readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    private readonly SettingsManager _settingsManager;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Get logo stream or null in case of default logo
    /// </summary>
    public async Task<Stream> GetWhitelabelMailLogoAsync()
    {
        if (WhiteLabelEnabled)
        {
            var tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();
            return await _tenantWhiteLabelSettingsHelper.GetWhitelabelLogoData(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Notification);
        }

        /*** simple scheme ***/
        return await _tenantInfoSettingsHelper.GetStorageLogoData(await _settingsManager.LoadAsync<TenantInfoSettings>());
        /***/
    }

    public async Task<NotifyMessageAttachment> GetMailLogoAsAttacmentAsync()
    {
        var logoData = await GetMailLogoDataFromCacheAsync();

        if (logoData == null)
        {
            var logoStream = await GetWhitelabelMailLogoAsync();
            logoData = await ReadStreamToByteArrayAsync(logoStream) ?? await GetDefaultMailLogoAsync();

            if (logoData != null)
            {
                await InsertMailLogoDataToCacheAsync(logoData);
            }
        }

        if (logoData != null)
        {
            var attachment = new NotifyMessageAttachment
            {
                FileName = "logo.png",
                Content = logoData,
                ContentId = MimeUtils.GenerateMessageId()
            };

            return attachment;
        }

        return null;
    }

    public async Task RemoveMailLogoDataFromCacheAsync()
    {
        await _distributedCache.RemoveAsync(CacheKey);
    }


    private async Task<byte[]> GetMailLogoDataFromCacheAsync()
    {
        return await _distributedCache.GetAsync(CacheKey);
    }

    private async Task InsertMailLogoDataToCacheAsync(byte[] data)
    {
        await _distributedCache.SetAsync(CacheKey, data, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.UtcNow.Add(TimeSpan.FromDays(1))
        });
    }

    private static async Task<byte[]> ReadStreamToByteArrayAsync(Stream inputStream)
    {
        if (inputStream == null)
        {
            return null;
        }

        await using (inputStream)
        {
            using var memoryStream = new MemoryStream();
            await inputStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }

    private static async Task<byte[]> GetDefaultMailLogoAsync()
    {
        var myAssembly = Assembly.GetExecutingAssembly();
        await using var stream = myAssembly.GetManifestResourceStream("ASC.Web.Core.PublicResources.logo.png");
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
