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

    public string LogoLightSmallExt { get; set; }

    [JsonPropertyName("DefaultLogoLightSmall")]
    public bool IsDefaultLogoLightSmall { get; set; }

    public string LogoDarkExt { get; set; }

    [JsonPropertyName("DefaultLogoDark")]
    public bool IsDefaultLogoDark { get; set; }

    public string LogoFaviconExt { get; set; }

    [JsonPropertyName("DefaultLogoFavicon")]
    public bool IsDefaultLogoFavicon { get; set; }

    public string LogoDocsEditorExt { get; set; }

    [JsonPropertyName("DefaultLogoDocsEditor")]
    public bool IsDefaultLogoDocsEditor { get; set; }

    public string LogoDocsEditorEmbedExt { get; set; }

    [JsonPropertyName("DefaultLogoDocsEditorEmbed")]
    public bool IsDefaultLogoDocsEditorEmbed { get; set; }

    public string LogoLeftMenuExt { get; set; }

    [JsonPropertyName("DefaultLogoLeftMenu")]
    public bool IsDefaultLogoLeftMenu { get; set; }

    public string LogoAboutPageExt { get; set; }

    [JsonPropertyName("DefaultLogoAboutPage")]
    public bool IsDefaultLogoAboutPage { get; set; }

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
            WhiteLabelLogoTypeEnum.LoginPage => IsDefaultLogoDark,
            WhiteLabelLogoTypeEnum.Favicon => IsDefaultLogoFavicon,
            WhiteLabelLogoTypeEnum.DocsEditor => IsDefaultLogoDocsEditor,
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => IsDefaultLogoDocsEditorEmbed,
            WhiteLabelLogoTypeEnum.LeftMenu => IsDefaultLogoLeftMenu,
            WhiteLabelLogoTypeEnum.AboutPage => IsDefaultLogoAboutPage,
            _ => true,
        };
    }

    internal bool CanBeDark(WhiteLabelLogoTypeEnum type)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.Favicon => false,
            WhiteLabelLogoTypeEnum.DocsEditor => false,
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => false,
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
            case WhiteLabelLogoTypeEnum.LoginPage:
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
            WhiteLabelLogoTypeEnum.LoginPage => LogoDarkExt,
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
            case WhiteLabelLogoTypeEnum.LoginPage:
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

    public void RestoreDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings, TenantLogoManager tenantLogoManager, int tenantId, IDataStore storage = null)
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
            store.DeleteFilesAsync("", "*", false).Wait();
        }
        catch (Exception e)
        {
            _log.ErrorRestoreDefault(e);
        }

        Save(tenantWhiteLabelSettings, tenantId, tenantLogoManager, true);
    }

    public void RestoreDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type)
    {
        if (!tenantWhiteLabelSettings.GetIsDefault(type))
        {
            try
            {
                tenantWhiteLabelSettings.SetIsDefault(type, true);
                var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);
                DeleteLogoFromStore(tenantWhiteLabelSettings, store, type, false);
                DeleteLogoFromStore(tenantWhiteLabelSettings, store, type, true);
            }
            catch (Exception e)
            {
                _log.ErrorRestoreDefault(e);
            }
        }
    }

    #endregion

    #region Set logo

    public void SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string logoFileExt, byte[] data, bool dark, IDataStore storage = null)
    {
        var store = storage ?? _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);

        #region delete from storage if already exists

        var isAlreadyHaveBeenChanged = !tenantWhiteLabelSettings.GetIsDefault(type);

        if (isAlreadyHaveBeenChanged)
        {
            try
            {
                DeleteLogoFromStore(tenantWhiteLabelSettings, store, type, dark);
            }
            catch (Exception e)
            {
                _log.ErrorSetLogo(e);
            }
        }
        #endregion

        using (var memory = new MemoryStream(data))
        {
            var logoFileName = BuildLogoFileName(type, logoFileExt, false, dark, tenantWhiteLabelSettings);

            memory.Seek(0, SeekOrigin.Begin);
            store.SaveAsync(logoFileName, memory).Wait();
        }

        var generalSize = GetSize(type, true);
        var generalFileName = BuildLogoFileName(type, logoFileExt, true, dark, tenantWhiteLabelSettings);
        if (logoFileExt != "svg")
        {
            ResizeLogo(generalFileName, data, -1, generalSize, store);
        }
    }

    public void SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, Dictionary<int, KeyValuePair<string, string>> logo, IDataStore storage = null)
    {
        foreach (var currentLogo in logo)
        {
            var currentLogoType = (WhiteLabelLogoTypeEnum)currentLogo.Key;

            byte[] lightData;
            byte[] darkData;

            string extLight;
            string extDark;

            lightData = GetLogoData(currentLogo.Value.Key, out extLight);
            if (currentLogo.Value.Key == currentLogo.Value.Value)
            {
                darkData = lightData;
                extDark = extLight;
            }
            else
            {
                darkData = GetLogoData(currentLogo.Value.Value, out extDark);
            }

            if(lightData == null && darkData == null)
            {
                return;
            }

            if (tenantWhiteLabelSettings.GetIsDefault(currentLogoType)) {
                if (lightData == null)
                {
                    lightData = darkData;
                    extLight = extDark;
                }
                if (darkData == null)
                {
                    darkData = lightData;
                    extDark = extLight;
                }
            }
            if (extLight != null && extDark != null && extLight != extDark)
            {
                throw new InvalidOperationException("logo light and logo dark have different extention");
            }

            if ((extLight == null || extDark == null) 
                && tenantWhiteLabelSettings.GetExt(currentLogoType) != extLight
                && tenantWhiteLabelSettings.GetExt(currentLogoType) != extDark
                && tenantWhiteLabelSettings.CanBeDark(currentLogoType))
            {
                throw new InvalidOperationException("current logos and downloaded logo have different extention");
            }

            tenantWhiteLabelSettings.SetExt(currentLogoType, extLight);
            tenantWhiteLabelSettings.SetIsDefault(currentLogoType, false);

            if (lightData!= null)
            {
                SetLogo(tenantWhiteLabelSettings, currentLogoType, extLight, lightData, false, storage);
            }
            if(darkData != null && tenantWhiteLabelSettings.CanBeDark(currentLogoType))
            {
                SetLogo(tenantWhiteLabelSettings, currentLogoType, extDark, darkData, true, storage);
            }

        }
    }

    private byte[] GetLogoData(string logo, out string ext)
    {
        var xStart = @"data:image/png;base64,";
        ext = null;
        if (!string.IsNullOrEmpty(logo))
        {
            byte[] data;
            if (!logo.StartsWith(xStart))
            {
                var fileName = Path.GetFileName(logo);
                ext = fileName.Split('.').Last();
                data = _userPhotoManager.GetTempPhotoData(fileName);
                try
                {
                    _userPhotoManager.RemoveTempPhoto(fileName);
                }
                catch (Exception ex)
                {
                    _log.ErrorSetLogo(ex);
                }
            }
            else
            {
                ext = "png";
                var xB64 = logo.Substring(xStart.Length); // Get the Base64 string
                data = Convert.FromBase64String(xB64); // Convert the Base64 string to binary data
            }

            return data;
        }
        else
        {
            return null;
        }
    }

    public void SetLogoFromStream(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string fileExt, Stream fileStream, Stream fileDarkStream, IDataStore storage = null)
    {
        var lightData = GetData(fileStream);
        var darkData = GetData(fileDarkStream);

        if (tenantWhiteLabelSettings.GetIsDefault(type))
        {
            if (lightData == null)
            {
                lightData = darkData;
            }
            if (darkData == null)
            {
                darkData = lightData;
            }

        }

        if ((lightData == null || darkData == null) 
            && tenantWhiteLabelSettings.GetExt(type) != fileExt
            && tenantWhiteLabelSettings.CanBeDark(type))
        {
            throw new InvalidOperationException("current logos and downloaded logo have different extention");
        }

        tenantWhiteLabelSettings.SetExt(type, fileExt);
        tenantWhiteLabelSettings.SetIsDefault(type, false);
        if (lightData != null)
        {
            SetLogo(tenantWhiteLabelSettings, type, fileExt, lightData, false, storage);
        }
        if (darkData != null && tenantWhiteLabelSettings.CanBeDark(type))
        {
            SetLogo(tenantWhiteLabelSettings, type, fileExt, darkData, true, storage);
        }
    }

    private byte[] GetData(Stream stream)
    {
        if (stream != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        return null;
    }

    #endregion

    #region Get logo path

    public string GetAbsoluteLogoPath(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general = true, bool dark = false)
    {
        if (tenantWhiteLabelSettings.GetIsDefault(type))
        {
            return GetAbsoluteDefaultLogoPath(type, general, dark);
        }

        return GetAbsoluteStorageLogoPath(tenantWhiteLabelSettings, type, general, dark);
    }

    private string GetAbsoluteStorageLogoPath(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general, bool dark)
    {
        var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);
        var fileName = BuildLogoFileName(type, tenantWhiteLabelSettings.GetExt(type), general, dark, tenantWhiteLabelSettings);

        if (store.IsFileAsync(fileName).Result)
        {
            return store.GetUriAsync(fileName).Result.ToString();
        }
        return GetAbsoluteDefaultLogoPath(type, general, dark);
    }

    public string GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum type, bool general, bool dark)
    {
        var partnerLogoPath = GetPartnerStorageLogoPath(type, general, dark);
        if (!string.IsNullOrEmpty(partnerLogoPath))
        {
            return partnerLogoPath;
        }

        var generalStr = general ? "_general" : "";
        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => _webImageSupplier.GetAbsoluteWebPath("logo/light_small_doc_space.svg"),
            WhiteLabelLogoTypeEnum.LoginPage => _webImageSupplier.GetAbsoluteWebPath("logo/login_page_doc_space.svg"),
            WhiteLabelLogoTypeEnum.DocsEditor => _webImageSupplier.GetAbsoluteWebPath($"logo/editor_logo{generalStr}.png"),
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => _webImageSupplier.GetAbsoluteWebPath($"logo/editor_logo_embed{generalStr}.png"),
            WhiteLabelLogoTypeEnum.Favicon => _webImageSupplier.GetAbsoluteWebPath($"logo/favicon.ico"),
            WhiteLabelLogoTypeEnum.LeftMenu => _webImageSupplier.GetAbsoluteWebPath($"logo/left_menu.svg"),
            WhiteLabelLogoTypeEnum.AboutPage => _webImageSupplier.GetAbsoluteWebPath("logo/about_doc_space.svg"),
            _ => "",
        };
    }

    private string GetPartnerStorageLogoPath(WhiteLabelLogoTypeEnum type, bool general, bool dark)
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

        var logoPath = BuildLogoFileName(type, partnerSettings.GetExt(type), general, dark, partnerSettings);

        return partnerStorage.IsFileAsync(logoPath).Result ? partnerStorage.GetUriAsync(logoPath).Result.ToString() : null;
    }

    #endregion

    #region Get Whitelabel Logo Stream

    /// <summary>
    /// Get logo stream or null in case of default whitelabel
    /// </summary>
    public Stream GetWhitelabelLogoData(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general, bool dark = false)
    {
        if (tenantWhiteLabelSettings.GetIsDefault(type))
        {
            return GetPartnerStorageLogoData(type, general, dark);
        }

        return GetStorageLogoData(tenantWhiteLabelSettings, type, general, dark);
    }

    private Stream GetStorageLogoData(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general, bool dark)
    {
        var storage = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id, ModuleName);

        if (storage == null)
        {
            return null;
        }

        var fileName = BuildLogoFileName(type, tenantWhiteLabelSettings.GetExt(type), general, dark, tenantWhiteLabelSettings);

        return storage.IsFileAsync(fileName).Result ? storage.GetReadStreamAsync(fileName).Result : null;
    }

    private Stream GetPartnerStorageLogoData(WhiteLabelLogoTypeEnum type, bool general, bool dark)
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

        var fileName = BuildLogoFileName(type, partnerSettings.GetExt(type), general, dark, partnerSettings);

        return partnerStorage.IsFileAsync(fileName).Result ? partnerStorage.GetReadStreamAsync(fileName).Result : null;
    }

    #endregion

    public static string BuildLogoFileName(WhiteLabelLogoTypeEnum type, string fileExt, bool general, bool dark, TenantWhiteLabelSettings tenantWhiteLabelSettings)
    {
        general = fileExt != "svg" && general;
        var arr = new WhiteLabelLogoTypeEnum[] { WhiteLabelLogoTypeEnum.Favicon, WhiteLabelLogoTypeEnum.DocsEditor, WhiteLabelLogoTypeEnum.DocsEditorEmbed };
        dark = !arr.Contains(type) && dark;
        return $"logo_{type.ToString().ToLowerInvariant()}{(general ? "_general" : "")}{(dark ? "_dark" : "")}.{fileExt}";
    }

    public static Size GetSize(WhiteLabelLogoTypeEnum type, bool general)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => new Size(
                   general ? TenantWhiteLabelSettings.LogoLightSmallSize.Width / 2 : TenantWhiteLabelSettings.LogoLightSmallSize.Width,
                   general ? TenantWhiteLabelSettings.LogoLightSmallSize.Height / 2 : TenantWhiteLabelSettings.LogoLightSmallSize.Height),
            WhiteLabelLogoTypeEnum.LoginPage => new Size(
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

    private static void ResizeLogo(string fileName, byte[] data, long maxFileSize, Size size, IDataStore store)
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
            store.SaveAsync(fileName, stream2).Wait();
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

    private void DeleteLogoFromStore(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type, bool dark)
    {

        DeleteLogoFromStoreByGeneral(tenantWhiteLabelSettings, store, type, false, dark);
        DeleteLogoFromStoreByGeneral(tenantWhiteLabelSettings, store, type, true, dark);
    }

    private void DeleteLogoFromStoreByGeneral(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type, bool general, bool dark)
    {
        var fileExt = tenantWhiteLabelSettings.GetExt(type);
        var logo = BuildLogoFileName(type, fileExt, general, dark, tenantWhiteLabelSettings);
        if (store.IsFileAsync(logo).Result)
        {
            store.DeleteAsync(logo).Wait();
        }
    }

    #endregion
}
