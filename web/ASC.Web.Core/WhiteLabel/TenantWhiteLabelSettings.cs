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

    public async Task<string> GetLogoTextAsync(SettingsManager settingsManager)
    {
        if (!string.IsNullOrEmpty(LogoText) && LogoText != DefaultLogoText)
        {
            return LogoText;
        }

        var partnerSettings = await settingsManager.LoadForDefaultTenantAsync<TenantWhiteLabelSettings>();
        return string.IsNullOrEmpty(partnerSettings.LogoText) ? DefaultLogoText : partnerSettings.LogoText;
    }

    public void SetLogoText(string val)
    {
        LogoText = val;
    }

    #endregion

    #region Logo available sizes

    public static readonly Size LogoLightSmallSize = new Size(422, 48);
    public static readonly Size LogoLoginPageSize = new Size(772, 88);
    public static readonly Size LogoFaviconSize = new Size(32, 32);
    public static readonly Size LogoDocsEditorSize = new Size(172, 40);
    public static readonly Size LogoDocsEditorEmbedSize = new Size(172, 40);
    public static readonly Size LogoLeftMenuSize = new Size(56, 56);
    public static readonly Size LogoAboutPageSize = new Size(442, 48);
    public static readonly Size LogoNotificationSize = new Size(386, 44);
    public static Size GetSize(WhiteLabelLogoTypeEnum type)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => LogoLightSmallSize,
            WhiteLabelLogoTypeEnum.LoginPage => LogoLoginPageSize,
            WhiteLabelLogoTypeEnum.Favicon => LogoFaviconSize,
            WhiteLabelLogoTypeEnum.DocsEditor => LogoDocsEditorSize,
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => LogoDocsEditorEmbedSize,
            WhiteLabelLogoTypeEnum.LeftMenu => LogoLeftMenuSize,
            WhiteLabelLogoTypeEnum.AboutPage => LogoAboutPageSize,
            _ => new Size(),
        };
    }

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
            WhiteLabelLogoTypeEnum.Notification => IsDefaultLogoDark,
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
            WhiteLabelLogoTypeEnum.Notification => "png",
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

        var store = storage ?? await _storageFactory.GetStorageAsync(tenantId, ModuleName);

        try
        {
            await store.DeleteFilesAsync("", "*", false);
        }
        catch (Exception e)
        {
            _log.ErrorRestoreDefault(e);
        }

        await SaveAsync(tenantWhiteLabelSettings, tenantId, tenantLogoManager, true);
    }

    public async Task RestoreDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type)
    {
        if (!tenantWhiteLabelSettings.GetIsDefault(type))
        {
            try
            {
                tenantWhiteLabelSettings.SetIsDefault(type, true);
                var store = await _storageFactory.GetStorageAsync(await _tenantManager.GetCurrentTenantIdAsync(), ModuleName);
                await DeleteLogoFromStore(tenantWhiteLabelSettings, store, type, false);
                await DeleteLogoFromStore(tenantWhiteLabelSettings, store, type, true);
            }
            catch (Exception e)
            {
                _log.ErrorRestoreDefault(e);
            }
        }
    }

    #endregion

    #region Set logo

    public async Task SetLogoAsync(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string logoFileExt, byte[] data, bool dark, IDataStore storage = null)
    {
        var store = storage ?? await _storageFactory.GetStorageAsync(await _tenantManager.GetCurrentTenantIdAsync(), ModuleName);

        #region delete from storage if already exists

        var isAlreadyHaveBeenChanged = !tenantWhiteLabelSettings.GetIsDefault(type);

        if (isAlreadyHaveBeenChanged)
        {
            try
            {
                await DeleteLogoFromStore(tenantWhiteLabelSettings, store, type, dark);
            }
            catch (Exception e)
            {
                _log.ErrorSetLogo(e);
            }
        }
        #endregion

        using (var memory = new MemoryStream(data))
        {
            var logoFileName = BuildLogoFileName(type, logoFileExt, dark);

            memory.Seek(0, SeekOrigin.Begin);
            await store.SaveAsync(logoFileName, memory);
        }
    }

    public async Task SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, Dictionary<int, KeyValuePair<string, string>> logo, IDataStore storage = null)
    {
        foreach (var currentLogo in logo)
        {
            var currentLogoType = (WhiteLabelLogoTypeEnum)currentLogo.Key;

            byte[] darkData;
            string extDark;

            var (lightData, extLight) = await GetLogoData(currentLogo.Value.Key);

            if (currentLogo.Value.Key == currentLogo.Value.Value)
            {
                darkData = lightData;
                extDark = extLight;
            }
            else
            {
                (darkData, extDark) = await GetLogoData(currentLogo.Value.Value);
            }

            if (lightData == null && darkData == null)
            {
                return;
            }

            if (lightData != null)
            {
                await SetLogoAsync(tenantWhiteLabelSettings, currentLogoType, extLight, lightData, false, storage);

                if (currentLogoType == WhiteLabelLogoTypeEnum.LoginPage)
                {
                    var (notificationData, extNotification) = GetNotificationLogoData(lightData, extLight, tenantWhiteLabelSettings);

                    if (notificationData != null)
                    {
                        await SetLogoAsync(tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Notification, extNotification, notificationData, false, storage);
                    }
                }
            }

            if (darkData != null && CanBeDark(currentLogoType))
            {
                await SetLogoAsync(tenantWhiteLabelSettings, currentLogoType, extDark, darkData, true, storage);
            }

            tenantWhiteLabelSettings.SetExt(currentLogoType, extLight);
            tenantWhiteLabelSettings.SetIsDefault(currentLogoType, false);
        }
    }

    private async Task<(byte[], string)> GetLogoData(string logo)
    {
        var supportedFormats = new[]
        {
            new {
                    mime = "image/jpeg",
                    ext = "jpg"
                },
            new {
                    mime = "image/png",
                    ext = "png"
                },
            new {
                    mime = "image/svg+xml",
                    ext = "svg"
                }
        };

        string ext = null;

        if (!string.IsNullOrEmpty(logo))
        {
            byte[] data;
            var format = supportedFormats.FirstOrDefault(r => logo.StartsWith($"data:{r.mime};base64,"));
            if (format == null)
            {
                var fileName = Path.GetFileName(logo);
                ext = fileName.Split('.').Last();
                data = await _userPhotoManager.GetTempPhotoData(fileName);
                try
                {
                    await _userPhotoManager.RemoveTempPhotoAsync(fileName);
                }
                catch (Exception ex)
                {
                    _log.ErrorSetLogo(ex);
                }
            }
            else
            {
                ext = format.ext;
                var xB64 = logo.Substring($"data:{format.mime};base64,".Length); // Get the Base64 string
                data = Convert.FromBase64String(xB64); // Convert the Base64 string to binary data
            }

            return (data, ext);
        }
        else
        {
            return (null, ext);
        }
    }

    private (byte[], string) GetNotificationLogoData(byte[] logoData, string extLogo, TenantWhiteLabelSettings tenantWhiteLabelSettings)
    {
        var extNotification = tenantWhiteLabelSettings.GetExt(WhiteLabelLogoTypeEnum.Notification);

        switch (extLogo)
        {
            case "png":
                return (logoData, extNotification);
            case "svg":
                return (GetLogoDataFromSvg(), extNotification);
            case "jpg":
            case "jpeg":
                return (GetLogoDataFromJpg(), extNotification);
            default:
                return (null, extNotification);
        }

        byte[] GetLogoDataFromSvg()
        {
            var size = GetSize(WhiteLabelLogoTypeEnum.Notification);
            var skSize = new SKSize(size.Width, size.Height);

            var svg = new SkiaSharp.Extended.Svg.SKSvg(skSize);

            using (var stream = new MemoryStream(logoData))
            {
                svg.Load(stream);
            }

            using (var bitMap = new SKBitmap((int)svg.CanvasSize.Width, (int)svg.CanvasSize.Height))
            using (var canvas = new SKCanvas(bitMap))
            {
                canvas.DrawPicture(svg.Picture);

                using (var image = SKImage.FromBitmap(bitMap))
                using (var pngData = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return pngData.ToArray();
                }
            }
        }

        byte[] GetLogoDataFromJpg()
        {
            using (var image = SKImage.FromEncodedData(logoData))
            using (var pngData = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return pngData.ToArray();
            }
        }
    }

    public async Task SetLogoFromStream(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string fileExt, Stream fileStream, Stream fileDarkStream, IDataStore storage = null)
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
            && CanBeDark(type))
        {
            throw new InvalidOperationException("current logos and downloaded logo have different extention");
        }

        if (lightData != null)
        {
            await SetLogoAsync(tenantWhiteLabelSettings, type, fileExt, lightData, false, storage);
        }
        if (darkData != null && CanBeDark(type))
        {
            await SetLogoAsync(tenantWhiteLabelSettings, type, fileExt, darkData, true, storage);
        }

        tenantWhiteLabelSettings.SetExt(type, fileExt);
        tenantWhiteLabelSettings.SetIsDefault(type, false);
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

    public async Task<string> GetAbsoluteLogoPathAsync(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool dark = false)
    {
        if (tenantWhiteLabelSettings.GetIsDefault(type))
        {
            return await GetAbsoluteDefaultLogoPathAsync(type, dark);
        }

        return await GetAbsoluteStorageLogoPath(tenantWhiteLabelSettings, type, dark);
    }

    private async Task<string> GetAbsoluteStorageLogoPath(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool dark)
    {
        var store = await _storageFactory.GetStorageAsync(await _tenantManager.GetCurrentTenantIdAsync(), ModuleName);
        var fileName = BuildLogoFileName(type, tenantWhiteLabelSettings.GetExt(type), dark);

        if (await store.IsFileAsync(fileName))
        {
            return (await store.GetUriAsync(fileName)).ToString();
        }
        return await GetAbsoluteDefaultLogoPathAsync(type, dark);
    }

    public async Task<string> GetAbsoluteDefaultLogoPathAsync(WhiteLabelLogoTypeEnum type, bool dark)
    {
        var partnerLogoPath = await GetPartnerStorageLogoPathAsync(type, dark);
        if (!string.IsNullOrEmpty(partnerLogoPath))
        {
            return partnerLogoPath;
        }

        var ext = type switch
        {
            WhiteLabelLogoTypeEnum.Favicon => "ico",
            WhiteLabelLogoTypeEnum.Notification => "png",
            _ => "svg"
        };

        var path = type switch
        {
            WhiteLabelLogoTypeEnum.Notification => "notifications/",
            _ => "logo/"
        };

        return _webImageSupplier.GetAbsoluteWebPath(path + BuildLogoFileName(type, ext, dark));
    }

    private async Task<string> GetPartnerStorageLogoPathAsync(WhiteLabelLogoTypeEnum type, bool dark)
    {
        var partnerSettings = await _settingsManager.LoadForDefaultTenantAsync<TenantWhiteLabelSettings>();

        if (partnerSettings.GetIsDefault(type))
        {
            return null;
        }

        var partnerStorage = await _storageFactory.GetStorageAsync(-1, "static_partnerdata");

        if (partnerStorage == null)
        {
            return null;
        }

        var logoPath = BuildLogoFileName(type, partnerSettings.GetExt(type), dark);
 
        return (await partnerStorage.IsFileAsync(logoPath)) ? (await partnerStorage.GetUriAsync(logoPath)).ToString() : null;
    }

    #endregion

    #region Get Whitelabel Logo Stream

    /// <summary>
    /// Get logo stream or null in case of default whitelabel
    /// </summary>
    public async Task<Stream> GetWhitelabelLogoData(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool dark = false)
    {
        if (tenantWhiteLabelSettings.GetIsDefault(type))
        {
            return await GetPartnerStorageLogoData(type, dark);
        }

        return await GetStorageLogoData(tenantWhiteLabelSettings, type, dark);
    }

    private async Task<Stream> GetStorageLogoData(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool dark)
    {
        var storage = await _storageFactory.GetStorageAsync(await _tenantManager.GetCurrentTenantIdAsync(), ModuleName);

        if (storage == null)
        {
            return null;
        }

        var fileName = BuildLogoFileName(type, tenantWhiteLabelSettings.GetExt(type), dark);

        return await storage.IsFileAsync(fileName) ? await storage.GetReadStreamAsync(fileName) : null;
    }

    private async Task<Stream> GetPartnerStorageLogoData(WhiteLabelLogoTypeEnum type, bool dark)
    {
        var partnerSettings = await _settingsManager.LoadForDefaultTenantAsync<TenantWhiteLabelSettings>();

        if (partnerSettings.GetIsDefault(type))
        {
            return null;
        }

        var partnerStorage = await _storageFactory.GetStorageAsync(-1, "static_partnerdata");

        if (partnerStorage == null)
        {
            return null;
        }

        var fileName = BuildLogoFileName(type, partnerSettings.GetExt(type), dark);

        return await partnerStorage.IsFileAsync(fileName) ? await partnerStorage.GetReadStreamAsync(fileName) : null;
    }

    #endregion

    public static string BuildLogoFileName(WhiteLabelLogoTypeEnum type, string fileExt, bool dark)
    {
        if (CanBeDark(type))
        {
            return $"{(dark ? "dark_" : "")}{type.ToString().ToLowerInvariant()}.{fileExt}";
        }

        return $"{type.ToString().ToLowerInvariant()}.{fileExt}";
    }

    public static Size GetSize(WhiteLabelLogoTypeEnum type)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.LightSmall => TenantWhiteLabelSettings.LogoLightSmallSize,
            WhiteLabelLogoTypeEnum.LoginPage => TenantWhiteLabelSettings.LogoLoginPageSize,
            WhiteLabelLogoTypeEnum.Favicon => TenantWhiteLabelSettings.LogoFaviconSize,
            WhiteLabelLogoTypeEnum.DocsEditor => TenantWhiteLabelSettings.LogoDocsEditorSize,
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => TenantWhiteLabelSettings.LogoDocsEditorEmbedSize,
            WhiteLabelLogoTypeEnum.LeftMenu => TenantWhiteLabelSettings.LogoLeftMenuSize,
            WhiteLabelLogoTypeEnum.AboutPage => TenantWhiteLabelSettings.LogoAboutPageSize,
            WhiteLabelLogoTypeEnum.Notification => TenantWhiteLabelSettings.LogoNotificationSize,
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
            using var img = Image.Load(stream);

            if (size != img.Size)
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

    public async Task ApplyAsync(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId)
    {
        if (_appliedTenants.Contains(tenantId))
        {
            return;
        }

        await SetNewLogoTextAsync(tenantWhiteLabelSettings, tenantId);

        if (!_appliedTenants.Contains(tenantId))
        {
            _appliedTenants.Add(tenantId);
        }
    }

    public async Task SaveAsync(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId, TenantLogoManager tenantLogoManager, bool restore = false)
    {
        await _settingsManager.SaveAsync(tenantWhiteLabelSettings, tenantId);

        if (tenantId == Tenant.DefaultTenant)
        {
            _appliedTenants.Clear();
        }
        else
        {
            await SetNewLogoTextAsync(tenantWhiteLabelSettings, tenantId, restore);
            await tenantLogoManager.RemoveMailLogoDataFromCacheAsync();
        }
    }

    private async Task SetNewLogoTextAsync(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId, bool restore = false)
    {
        _whiteLabelHelper.DefaultLogoText = TenantWhiteLabelSettings.DefaultLogoText;
        var partnerSettings = await _settingsManager.LoadForDefaultTenantAsync<TenantWhiteLabelSettings>();

        if (restore && string.IsNullOrEmpty(await partnerSettings.GetLogoTextAsync(_settingsManager)))
        {
            _whiteLabelHelper.RestoreOldText(tenantId);
        }
        else
        {
            _whiteLabelHelper.SetNewText(tenantId, await tenantWhiteLabelSettings.GetLogoTextAsync(_settingsManager));
        }
    }

    #endregion

    #region Delete from Store

    private async Task DeleteLogoFromStore(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type, bool dark)
    {
        await DeleteLogoFromStoreByGeneral(tenantWhiteLabelSettings, store, type, dark);
    }

    private async Task DeleteLogoFromStoreByGeneral(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type, bool dark)
    {
        var fileExt = tenantWhiteLabelSettings.GetExt(type);
        var logo = BuildLogoFileName(type, fileExt, dark);
        if (await store.IsFileAsync(logo))
        {
            await store.DeleteAsync(logo);
        }
    }

    #endregion

    private static bool CanBeDark(WhiteLabelLogoTypeEnum type)
    {
        return type switch
        {
            WhiteLabelLogoTypeEnum.Favicon => false,
            WhiteLabelLogoTypeEnum.DocsEditor => false,
            WhiteLabelLogoTypeEnum.DocsEditorEmbed => false,
            _ => true,
        };
    }
}
