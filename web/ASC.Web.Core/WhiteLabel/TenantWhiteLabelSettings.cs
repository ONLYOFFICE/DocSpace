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

using UnknownImageFormatException = SixLabors.ImageSharp.UnknownImageFormatException;

namespace ASC.Web.Core.WhiteLabel;

[Serializable]
public class TenantWhiteLabelSettings : ISettings<TenantWhiteLabelSettings>
{
    public const string DefaultLogoText = BaseWhiteLabelSettings.DefaultLogoText;

    #region Logos information: extension, isDefault, text for img auto generating

    internal string LogoLightSmallExt { get; set; }

    [JsonPropertyName("DefaultLogoLightSmall")]
    internal bool IsDefaultLogoLightSmall { get; set; }

    internal string LogoDarkExt { get; set; }

    [JsonPropertyName("DefaultLogoDark")]
    internal bool IsDefaultLogoDark { get; set; }

    internal string LogoFaviconExt { get; set; }

    [JsonPropertyName("DefaultLogoFavicon")]
    internal bool IsDefaultLogoFavicon { get; set; }

    internal string LogoDocsEditorExt { get; set; }

    [JsonPropertyName("DefaultLogoDocsEditor")]
    internal bool IsDefaultLogoDocsEditor { get; set; }

    internal string LogoDocsEditorEmbedExt { get; set; }

    [JsonPropertyName("DefaultLogoDocsEditorEmbed")]
    internal bool IsDefaultLogoDocsEditorEmbed { get; set; }

    internal string LogoLeftMenuExt { get; set; }

    [JsonPropertyName("DefaultLogoLeftMenu")]
    internal bool IsDefaultLogoLeftMenu { get; set; }

    internal string LogoAboutPageExt { get; set; }

    [JsonPropertyName("DefaultLogoAboutPage")]
    internal bool IsDefaultLogoAboutPage { get; set; }

    public string LogoText { get; set; }

    public string GetLogoText(SettingsManager settingsManager)
    {
        if (!string.IsNullOrEmpty(LogoText) && LogoText != DefaultLogoText)
        {
            return LogoText;
        }

        var partnerSettings = settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
        return string.IsNullOrEmpty(partnerSettings.LogoText) ? DefaultLogoText : partnerSettings.LogoText;
    }

    public void SetLogoText(string val)
    {
        LogoText = val;
    }

    #endregion

    #region Logo available sizes

    public static readonly Size LogoLightSmallSize = new Size(422, 48);
    public static readonly Size LogoDarkSize = new Size(810, 92);
    public static readonly Size LogoFaviconSize = new Size(32, 32);
    public static readonly Size LogoDocsEditorSize = new Size(172, 40);
    public static readonly Size LogoDocsEditorEmbedSize = new Size(172, 40);
    public static readonly Size LogoLeftMenuSize = new Size(56, 56);
    public static readonly Size LogoAboutPageSize = new Size(810, 92);

    #endregion

    #region ISettings Members

    public TenantWhiteLabelSettings GetDefault()
    {
        return new TenantWhiteLabelSettings
        {
            LogoLightSmallExt = null,
            LogoDarkExt = null,
            LogoFaviconExt = null,
            LogoDocsEditorExt = null,
            LogoDocsEditorEmbedExt = null,
            LogoLeftMenuExt = null,
            LogoAboutPageExt = null,

            IsDefaultLogoLightSmall = true,
            IsDefaultLogoDark = true,
            IsDefaultLogoFavicon = true,
            IsDefaultLogoDocsEditor = true,
            IsDefaultLogoDocsEditorEmbed = true,
            IsDefaultLogoLeftMenu = true,
            IsDefaultLogoAboutPage = true,

            LogoText = null
        };
    }
    #endregion

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{05d35540-c80b-4b17-9277-abd9e543bf93}"); }
    }

    #region Get/Set IsDefault and Extension

    internal bool GetIsDefault(WhiteLabelLogoTypeEnum type)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => IsDefaultLogoLightSmall,
            WhiteLabelLogoTypeEnum.Dark => IsDefaultLogoDark,
            WhiteLabelLogoTypeEnum.Favicon => IsDefaultLogoFavicon,
            WhiteLabelLogoTypeEnum.DocsEditor => IsDefaultLogoDocsEditor,
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => IsDefaultLogoDocsEditorEmbed,
            WhiteLabelLogoTypeEnum.LeftMenu => IsDefaultLogoLeftMenu,
            WhiteLabelLogoTypeEnum.AboutPage => IsDefaultLogoAboutPage,
            _ => true,
        };
    }

    internal void SetIsDefault(WhiteLabelLogoTypeEnum type, bool value)
    {
        switch (type)
        {
            case WhiteLabelLogoTypeEnum.LightSmall:
                IsDefaultLogoLightSmall = value;
                break;
            case WhiteLabelLogoTypeEnum.Dark:
                IsDefaultLogoDark = value;
                break;
            case WhiteLabelLogoTypeEnum.Favicon:
                IsDefaultLogoFavicon = value;
                break;
            case WhiteLabelLogoTypeEnum.DocsEditor:
                IsDefaultLogoDocsEditor = value;
                break;
            case WhiteLabelLogoTypeEnum.DocsEditorEmbed:
                IsDefaultLogoDocsEditorEmbed = value;
                break;
            case WhiteLabelLogoTypeEnum.LeftMenu:
                IsDefaultLogoLeftMenu = value;
                break;
            case WhiteLabelLogoTypeEnum.AboutPage:
                IsDefaultLogoAboutPage = value;
                break;
        }
    }

    internal string GetExt(WhiteLabelLogoTypeEnum type)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => LogoLightSmallExt,
            WhiteLabelLogoTypeEnum.Dark => LogoDarkExt,
            WhiteLabelLogoTypeEnum.Favicon => LogoFaviconExt,
            WhiteLabelLogoTypeEnum.DocsEditor => LogoDocsEditorExt,
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => LogoDocsEditorEmbedExt,
            WhiteLabelLogoTypeEnum.LeftMenu => LogoLeftMenuExt,
            WhiteLabelLogoTypeEnum.AboutPage => LogoAboutPageExt,
            _ => "",
        };
    }

    internal void SetExt(WhiteLabelLogoTypeEnum type, string fileExt)
    {
        switch (type)
        {
            case WhiteLabelLogoTypeEnum.LightSmall:
                LogoLightSmallExt = fileExt;
                break;
            case WhiteLabelLogoTypeEnum.Dark:
                LogoDarkExt = fileExt;
                break;
            case WhiteLabelLogoTypeEnum.Favicon:
                LogoFaviconExt = fileExt;
                break;
            case WhiteLabelLogoTypeEnum.DocsEditor:
                LogoDocsEditorExt = fileExt;
                break;
            case WhiteLabelLogoTypeEnum.DocsEditorEmbed:
                LogoDocsEditorEmbedExt = fileExt;
                break;
            case WhiteLabelLogoTypeEnum.LeftMenu:
                LogoLeftMenuExt = fileExt;
                break;
            case WhiteLabelLogoTypeEnum.AboutPage:
                LogoAboutPageExt = fileExt;
                break;
        }
    }

    #endregion
}

[Scope]
public class TenantWhiteLabelSettingsHelper
{
    private const string ModuleName = "whitelabel";

    private readonly WebImageSupplier _webImageSupplier;
    private readonly UserPhotoManager _userPhotoManager;
    private readonly StorageFactory _storageFactory;
    private readonly WhiteLabelHelper _whiteLabelHelper;
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly ILogger<TenantWhiteLabelSettingsHelper> _log;

    public TenantWhiteLabelSettingsHelper(
        WebImageSupplier webImageSupplier,
        UserPhotoManager userPhotoManager,
        StorageFactory storageFactory,
        WhiteLabelHelper whiteLabelHelper,
        TenantManager tenantManager,
        SettingsManager settingsManager,
        ILogger<TenantWhiteLabelSettingsHelper> logger)
    {
        _webImageSupplier = webImageSupplier;
        _userPhotoManager = userPhotoManager;
        _storageFactory = storageFactory;
        _whiteLabelHelper = whiteLabelHelper;
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _log = logger;
    }

    #region Restore default

    public bool IsDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings)
    {
        var defaultSettings = _settingsManager.GetDefault<TenantWhiteLabelSettings>();

        return tenantWhiteLabelSettings.LogoLightSmallExt == defaultSettings.LogoLightSmallExt &&
                tenantWhiteLabelSettings.LogoDarkExt == defaultSettings.LogoDarkExt &&
                tenantWhiteLabelSettings.LogoFaviconExt == defaultSettings.LogoFaviconExt &&
                tenantWhiteLabelSettings.LogoDocsEditorExt == defaultSettings.LogoDocsEditorExt &&
                tenantWhiteLabelSettings.LogoDocsEditorEmbedExt == defaultSettings.LogoDocsEditorEmbedExt &&
                tenantWhiteLabelSettings.LogoLeftMenuExt == defaultSettings.LogoLeftMenuExt &&
                tenantWhiteLabelSettings.LogoAboutPageExt == defaultSettings.LogoAboutPageExt &&

                tenantWhiteLabelSettings.IsDefaultLogoLightSmall == defaultSettings.IsDefaultLogoLightSmall &&
                tenantWhiteLabelSettings.IsDefaultLogoDark == defaultSettings.IsDefaultLogoDark &&
                tenantWhiteLabelSettings.IsDefaultLogoFavicon == defaultSettings.IsDefaultLogoFavicon &&
                tenantWhiteLabelSettings.IsDefaultLogoDocsEditor == defaultSettings.IsDefaultLogoDocsEditor &&
                tenantWhiteLabelSettings.IsDefaultLogoDocsEditorEmbed == defaultSettings.IsDefaultLogoDocsEditorEmbed &&
                tenantWhiteLabelSettings.IsDefaultLogoLeftMenu == defaultSettings.IsDefaultLogoLeftMenu &&
                tenantWhiteLabelSettings.IsDefaultLogoAboutPage == defaultSettings.IsDefaultLogoAboutPage &&

                tenantWhiteLabelSettings.LogoText == defaultSettings.LogoText;
    }

    public async Task RestoreDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings, TenantLogoManager tenantLogoManager, int tenantId, IDataStore storage = null)
    {
        tenantWhiteLabelSettings.LogoLightSmallExt = null;
        tenantWhiteLabelSettings.LogoDarkExt = null;
        tenantWhiteLabelSettings.LogoFaviconExt = null;
        tenantWhiteLabelSettings.LogoDocsEditorExt = null;
        tenantWhiteLabelSettings.LogoDocsEditorEmbedExt = null;
        tenantWhiteLabelSettings.LogoLeftMenuExt = null;
        tenantWhiteLabelSettings.LogoAboutPageExt = null;

        tenantWhiteLabelSettings.IsDefaultLogoLightSmall = true;
        tenantWhiteLabelSettings.IsDefaultLogoDark = true;
        tenantWhiteLabelSettings.IsDefaultLogoFavicon = true;
        tenantWhiteLabelSettings.IsDefaultLogoDocsEditor = true;
        tenantWhiteLabelSettings.IsDefaultLogoDocsEditorEmbed = true;
        tenantWhiteLabelSettings.IsDefaultLogoLeftMenu = true;
        tenantWhiteLabelSettings.IsDefaultLogoAboutPage = true;

        tenantWhiteLabelSettings.SetLogoText(null);

        var store = storage ?? _storageFactory.GetStorage(tenantId, ModuleName);

        try
        {
           await store.DeleteFilesAsync("", "*", false);
        }
        catch (Exception e)
        {
            _log.ErrorRestoreDefault(e);
        }

        Save(tenantWhiteLabelSettings, tenantId, tenantLogoManager, true);
    }

    public async Task RestoreDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type)
    {
        if (!tenantWhiteLabelSettings.GetIsDefault(type))
        {
            try
            {
                tenantWhiteLabelSettings.SetIsDefault(type, true);
                var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);
                await DeleteLogoFromStore(tenantWhiteLabelSettings, store, type);
            }
            catch (Exception e)
            {
                _log.ErrorRestoreDefault(e);
            }
        }
    }

    #endregion

    #region Set logo

    public async Task SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string logoFileExt, byte[] data, IDataStore storage = null)
    {
        var store = storage ?? _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);

        #region delete from storage if already exists

        var isAlreadyHaveBeenChanged = !tenantWhiteLabelSettings.GetIsDefault(type);

        if (isAlreadyHaveBeenChanged)
        {
            try
            {
                await DeleteLogoFromStore(tenantWhiteLabelSettings, store, type);
            }
            catch (Exception e)
            {
                _log.ErrorSetLogo(e);
            }
        }
        #endregion

        using (var memory = new MemoryStream(data))
        using (var image = Image.Load(memory))
        {
            var logoFileName = BuildLogoFileName(type, logoFileExt, false);

            memory.Seek(0, SeekOrigin.Begin);
            await store.SaveAsync(logoFileName, memory);
        }

        tenantWhiteLabelSettings.SetExt(type, logoFileExt);
        tenantWhiteLabelSettings.SetIsDefault(type, false);

        var generalSize = GetSize(type, true);
        var generalFileName = BuildLogoFileName(type, logoFileExt, true);
        await ResizeLogo(generalFileName, data, -1, generalSize, store);
    }

    public async Task SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, Dictionary<int, string> logo, IDataStore storage = null)
    {
        var xStart = @"data:image/png;base64,";

        foreach (var currentLogo in logo)
        {
            var currentLogoType = (WhiteLabelLogoTypeEnum)currentLogo.Key;
            var currentLogoPath = currentLogo.Value;

            if (!string.IsNullOrEmpty(currentLogoPath))
            {
                var fileExt = "png";
                byte[] data;
                if (!currentLogoPath.StartsWith(xStart))
                {
                    var fileName = Path.GetFileName(currentLogoPath);
                    fileExt = fileName.Split('.').Last();
                    data = _userPhotoManager.GetTempPhotoData(fileName);
                    try
                    {
                        await _userPhotoManager.RemoveTempPhoto(fileName);
                    }
                    catch (Exception ex)
                    {
                        _log.ErrorSetLogo(ex);
                    }
                }
                else
                {
                    var xB64 = currentLogoPath.Substring(xStart.Length); // Get the Base64 string
                    data = Convert.FromBase64String(xB64); // Convert the Base64 string to binary data
                }

                if (data != null)
                {
                    await SetLogo(tenantWhiteLabelSettings, currentLogoType, fileExt, data, storage);
                }
            }
        }
    }

    public async Task SetLogoFromStream(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string fileExt, Stream fileStream, IDataStore storage = null)
    {
        byte[] data;
        using (var memoryStream = new MemoryStream())
        {
            fileStream.CopyTo(memoryStream);
            data = memoryStream.ToArray();
        }

        if (data != null)
        {
            await SetLogo(tenantWhiteLabelSettings, type, fileExt, data, storage);
        }
    }

    #endregion

    #region Get logo path

    public string GetAbsoluteLogoPath(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general = true)
    {
        if (tenantWhiteLabelSettings.GetIsDefault(type))
        {
            return GetAbsoluteDefaultLogoPath(type, general);
        }

        return GetAbsoluteStorageLogoPath(tenantWhiteLabelSettings, type, general);
    }

    private string GetAbsoluteStorageLogoPath(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general)
    {
        var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);
        var fileName = BuildLogoFileName(type, tenantWhiteLabelSettings.GetExt(type), general);

        if (store.IsFileAsync(fileName).Result)
        {
            return store.GetUriAsync(fileName).Result.ToString();
        }
        return GetAbsoluteDefaultLogoPath(type, general);
    }

    public string GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum type, bool general)
    {
        var partnerLogoPath = GetPartnerStorageLogoPath(type, general);
        if (!string.IsNullOrEmpty(partnerLogoPath))
        {
            return partnerLogoPath;
        }

        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => _webImageSupplier.GetAbsoluteWebPath("logo/light_small_doc_space.svg"),
            WhiteLabelLogoTypeEnum.Dark => _webImageSupplier.GetAbsoluteWebPath("logo/dark_doc_space.svg"),
            WhiteLabelLogoTypeEnum.DocsEditor => general ? _webImageSupplier.GetAbsoluteWebPath("logo/editor_logo_general.png") : _webImageSupplier.GetAbsoluteWebPath("logo/editor_logo.png"),
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => general ? _webImageSupplier.GetAbsoluteWebPath("logo/editor_logo_embed_general.png") : _webImageSupplier.GetAbsoluteWebPath("logo/editor_logo_embed.png"),
            WhiteLabelLogoTypeEnum.Favicon => general ? _webImageSupplier.GetAbsoluteWebPath("logo/favicon_general.ico") : _webImageSupplier.GetAbsoluteWebPath("logo/favicon.ico"),
            WhiteLabelLogoTypeEnum.LeftMenu => _webImageSupplier.GetAbsoluteWebPath("logo/left_menu_general.svg"),
            WhiteLabelLogoTypeEnum.AboutPage => _webImageSupplier.GetAbsoluteWebPath("logo/about_doc_space.svg"),
            _ => "",
        };
    }

    private string GetPartnerStorageLogoPath(WhiteLabelLogoTypeEnum type, bool general)
    {
        var partnerSettings = _settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();

        if (partnerSettings.GetIsDefault(type))
        {
            return null;
        }

        var partnerStorage = _storageFactory.GetStorage(null, "static_partnerdata");

        if (partnerStorage == null)
        {
            return null;
        }

        var logoPath = BuildLogoFileName(type, partnerSettings.GetExt(type), general);

        return partnerStorage.IsFileAsync(logoPath).Result ? partnerStorage.GetUriAsync(logoPath).Result.ToString() : null;
    }

    #endregion

    #region Get Whitelabel Logo Stream

    /// <summary>
    /// Get logo stream or null in case of default whitelabel
    /// </summary>
    public Stream GetWhitelabelLogoData(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general)
    {
        if (tenantWhiteLabelSettings.GetIsDefault(type))
        {
            return GetPartnerStorageLogoData(type, general);
        }

        return GetStorageLogoData(tenantWhiteLabelSettings, type, general);
    }

    private Stream GetStorageLogoData(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general)
    {
        var storage = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);

        if (storage == null)
        {
            return null;
        }

        var fileName = BuildLogoFileName(type, tenantWhiteLabelSettings.GetExt(type), general);

        return storage.IsFileAsync(fileName).Result ? storage.GetReadStreamAsync(fileName).Result : null;
    }

    private Stream GetPartnerStorageLogoData(WhiteLabelLogoTypeEnum type, bool general)
    {
        var partnerSettings = _settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();

        if (partnerSettings.GetIsDefault(type))
        {
            return null;
        }

        var partnerStorage = _storageFactory.GetStorage(null, "static_partnerdata");

        if (partnerStorage == null)
        {
            return null;
        }

        var fileName = BuildLogoFileName(type, partnerSettings.GetExt(type), general);

        return partnerStorage.IsFileAsync(fileName).Result ? partnerStorage.GetReadStreamAsync(fileName).Result : null;
    }

    #endregion

    public static string BuildLogoFileName(WhiteLabelLogoTypeEnum type, string fileExt, bool general)
    {
        return $"logo_{type.ToString().ToLowerInvariant()}{(general ? "_general" : "")}.{fileExt}";
    }

    public static Size GetSize(WhiteLabelLogoTypeEnum type, bool general)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => new Size(
                   general ? TenantWhiteLabelSettings.LogoLightSmallSize.Width / 2 : TenantWhiteLabelSettings.LogoLightSmallSize.Width,
                   general ? TenantWhiteLabelSettings.LogoLightSmallSize.Height / 2 : TenantWhiteLabelSettings.LogoLightSmallSize.Height),
            WhiteLabelLogoTypeEnum.Dark => new Size(
                    general ? TenantWhiteLabelSettings.LogoDarkSize.Width / 2 : TenantWhiteLabelSettings.LogoDarkSize.Width,
                    general ? TenantWhiteLabelSettings.LogoDarkSize.Height / 2 : TenantWhiteLabelSettings.LogoDarkSize.Height),
            WhiteLabelLogoTypeEnum.Favicon => new Size(
                    general ? TenantWhiteLabelSettings.LogoFaviconSize.Width / 2 : TenantWhiteLabelSettings.LogoFaviconSize.Width,
                    general ? TenantWhiteLabelSettings.LogoFaviconSize.Height / 2 : TenantWhiteLabelSettings.LogoFaviconSize.Height),
            WhiteLabelLogoTypeEnum.DocsEditor => new Size(
                    general ? TenantWhiteLabelSettings.LogoDocsEditorSize.Width / 2 : TenantWhiteLabelSettings.LogoDocsEditorSize.Width,
                    general ? TenantWhiteLabelSettings.LogoDocsEditorSize.Height / 2 : TenantWhiteLabelSettings.LogoDocsEditorSize.Height),
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => new Size(
                    general ? TenantWhiteLabelSettings.LogoDocsEditorEmbedSize.Width / 2 : TenantWhiteLabelSettings.LogoDocsEditorEmbedSize.Width,
                    general ? TenantWhiteLabelSettings.LogoDocsEditorEmbedSize.Height / 2 : TenantWhiteLabelSettings.LogoDocsEditorEmbedSize.Height),
            WhiteLabelLogoTypeEnum.LeftMenu => new Size(
                    general ? TenantWhiteLabelSettings.LogoLeftMenuSize.Width / 2 : TenantWhiteLabelSettings.LogoLeftMenuSize.Width,
                    general ? TenantWhiteLabelSettings.LogoLeftMenuSize.Height / 2 : TenantWhiteLabelSettings.LogoLeftMenuSize.Height),
            WhiteLabelLogoTypeEnum.AboutPage => new Size(
                    general ? TenantWhiteLabelSettings.LogoAboutPageSize.Width / 2 : TenantWhiteLabelSettings.LogoAboutPageSize.Width,
                    general ? TenantWhiteLabelSettings.LogoAboutPageSize.Height / 2 : TenantWhiteLabelSettings.LogoAboutPageSize.Height),
            _ => new Size(0, 0),
        };
    }

    private static async Task ResizeLogo(string fileName, byte[] data, long maxFileSize, Size size, IDataStore store)
    {
        //Resize synchronously
        if (data == null || data.Length <= 0)
        {
            throw new UnknownImageFormatException("data null");
        }

        if (maxFileSize != -1 && data.Length > maxFileSize)
        {
            throw new ImageWeightLimitException();
        }

        try
        {
            using var stream = new MemoryStream(data);
            using var img = Image.Load(stream, out var format);

            if (size != img.Size())
            {
                using var img2 = CommonPhotoManager.DoThumbnail(img, size, false, true, false);
                data = CommonPhotoManager.SaveToBytes(img2);
            }
            else
            {
                data = CommonPhotoManager.SaveToBytes(img);
            }

            //fileExt = CommonPhotoManager.GetImgFormatName(imgFormat);

            using var stream2 = new MemoryStream(data);
            await store.SaveAsync(fileName, stream2);
        }
        catch (ArgumentException error)
        {
            throw new UnknownImageFormatException(error.Message);
        }
    }

    #region Save for Resource replacement

    private static readonly List<int> _appliedTenants = new List<int>();

    public void Apply(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId)
    {
        if (_appliedTenants.Contains(tenantId))
        {
            return;
        }

        SetNewLogoText(tenantWhiteLabelSettings, tenantId);

        if (!_appliedTenants.Contains(tenantId))
        {
            _appliedTenants.Add(tenantId);
        }
    }

    public void Save(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId, TenantLogoManager tenantLogoManager, bool restore = false)
    {
        _settingsManager.SaveForTenant(tenantWhiteLabelSettings, tenantId);

        if (tenantId == Tenant.DefaultTenant)
        {
            _appliedTenants.Clear();
        }
        else
        {
            SetNewLogoText(tenantWhiteLabelSettings, tenantId, restore);
            tenantLogoManager.RemoveMailLogoDataFromCache();
        }
    }

    private void SetNewLogoText(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId, bool restore = false)
    {
        _whiteLabelHelper.DefaultLogoText = TenantWhiteLabelSettings.DefaultLogoText;
        var partnerSettings = _settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();

        if (restore && string.IsNullOrEmpty(partnerSettings.GetLogoText(_settingsManager)))
        {
            _whiteLabelHelper.RestoreOldText(tenantId);
        }
        else
        {
            _whiteLabelHelper.SetNewText(tenantId, tenantWhiteLabelSettings.GetLogoText(_settingsManager));
        }
    }

    #endregion

    #region Delete from Store

    private async Task DeleteLogoFromStore(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type)
    {
        await DeleteLogoFromStoreByGeneral(tenantWhiteLabelSettings, store, type, false);
        await DeleteLogoFromStoreByGeneral(tenantWhiteLabelSettings, store, type, true);
    }

    private async Task DeleteLogoFromStoreByGeneral(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type, bool general)
    {
        var fileExt = tenantWhiteLabelSettings.GetExt(type);
        var logo = BuildLogoFileName(type, fileExt, general);
        if (await store.IsFileAsync(logo))
        {
            await store.DeleteAsync(logo);
        }
    }

    #endregion
}
