/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Common.WhiteLabel;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;

using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;

using TMResourceData;

using UnknownImageFormatException = SixLabors.ImageSharp.UnknownImageFormatException;

namespace ASC.Web.Core.WhiteLabel
{
    [Serializable]
    public class TenantWhiteLabelSettings : ISettings
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

        internal string LogoDocsEditorEmbedExt;

        [JsonPropertyName("DefaultLogoDocsEditorEmbed")]
        internal bool IsDefaultLogoDocsEditorEmbed { get; set; }

        public string LogoText { get; set; }

        public string GetLogoText(SettingsManager settingsManager)
        {
            if (!string.IsNullOrEmpty(LogoText) && LogoText != DefaultLogoText)
                return LogoText;

            var partnerSettings = settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
            return string.IsNullOrEmpty(partnerSettings.LogoText) ? DefaultLogoText : partnerSettings.LogoText;
        }

        public void SetLogoText(string val)
        {
            LogoText = val;
        }

        #endregion

        #region Logo available sizes

        public static readonly Size logoLightSmallSize = new Size(284, 46);
        public static readonly Size logoDarkSize = new Size(432, 70);
        public static readonly Size logoFaviconSize = new Size(32, 32);
        public static readonly Size logoDocsEditorSize = new Size(172, 40);
        public static readonly Size logoDocsEditorEmbedSize = new Size(172, 40);

        #endregion

        #region ISettings Members

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TenantWhiteLabelSettings
            {
                LogoLightSmallExt = null,
                LogoDarkExt = null,
                LogoFaviconExt = null,
                LogoDocsEditorExt = null,
                LogoDocsEditorEmbedExt = null,

                IsDefaultLogoLightSmall = true,
                IsDefaultLogoDark = true,
                IsDefaultLogoFavicon = true,
                IsDefaultLogoDocsEditor = true,
                IsDefaultLogoDocsEditorEmbed = true,

                LogoText = null
            };
        }
        #endregion

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
            }
        }

        #endregion
    }

    [Scope]
    public class TenantWhiteLabelSettingsHelper
    {
        private const string moduleName = "whitelabel";

        private WebImageSupplier WebImageSupplier { get; }
        private UserPhotoManager UserPhotoManager { get; }
        private StorageFactory StorageFactory { get; }
        private WhiteLabelHelper WhiteLabelHelper { get; }
        private TenantManager TenantManager { get; }
        private SettingsManager SettingsManager { get; }
        public IServiceProvider ServiceProvider { get; }
        private ILog Log { get; set; }

        public TenantWhiteLabelSettingsHelper(
            WebImageSupplier webImageSupplier,
            UserPhotoManager userPhotoManager,
            StorageFactory storageFactory,
            WhiteLabelHelper whiteLabelHelper,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> option)
        {
            WebImageSupplier = webImageSupplier;
            UserPhotoManager = userPhotoManager;
            StorageFactory = storageFactory;
            WhiteLabelHelper = whiteLabelHelper;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            ServiceProvider = serviceProvider;
            Log = option.CurrentValue;
        }

        #region Restore default

        public bool IsDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings)
        {
            var defaultSettings = tenantWhiteLabelSettings.GetDefault(ServiceProvider) as TenantWhiteLabelSettings;

            if (defaultSettings == null) return false;

            return tenantWhiteLabelSettings.LogoLightSmallExt == defaultSettings.LogoLightSmallExt &&
                    tenantWhiteLabelSettings.LogoDarkExt == defaultSettings.LogoDarkExt &&
                    tenantWhiteLabelSettings.LogoFaviconExt == defaultSettings.LogoFaviconExt &&
                    tenantWhiteLabelSettings.LogoDocsEditorExt == defaultSettings.LogoDocsEditorExt &&
                    tenantWhiteLabelSettings.LogoDocsEditorEmbedExt == defaultSettings.LogoDocsEditorEmbedExt &&

                    tenantWhiteLabelSettings.IsDefaultLogoLightSmall == defaultSettings.IsDefaultLogoLightSmall &&
                    tenantWhiteLabelSettings.IsDefaultLogoDark == defaultSettings.IsDefaultLogoDark &&
                    tenantWhiteLabelSettings.IsDefaultLogoFavicon == defaultSettings.IsDefaultLogoFavicon &&
                    tenantWhiteLabelSettings.IsDefaultLogoDocsEditor == defaultSettings.IsDefaultLogoDocsEditor &&
                    tenantWhiteLabelSettings.IsDefaultLogoDocsEditorEmbed == defaultSettings.IsDefaultLogoDocsEditorEmbed &&

                    tenantWhiteLabelSettings.LogoText == defaultSettings.LogoText;
        }

        public void RestoreDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings, TenantLogoManager tenantLogoManager, int tenantId, IDataStore storage = null)
        {
            tenantWhiteLabelSettings.LogoLightSmallExt = null;
            tenantWhiteLabelSettings.LogoDarkExt = null;
            tenantWhiteLabelSettings.LogoFaviconExt = null;
            tenantWhiteLabelSettings.LogoDocsEditorExt = null;
            tenantWhiteLabelSettings.LogoDocsEditorEmbedExt = null;

            tenantWhiteLabelSettings.IsDefaultLogoLightSmall = true;
            tenantWhiteLabelSettings.IsDefaultLogoDark = true;
            tenantWhiteLabelSettings.IsDefaultLogoFavicon = true;
            tenantWhiteLabelSettings.IsDefaultLogoDocsEditor = true;
            tenantWhiteLabelSettings.IsDefaultLogoDocsEditorEmbed = true;

            tenantWhiteLabelSettings.SetLogoText(null);

            var store = storage ?? StorageFactory.GetStorage(tenantId.ToString(), moduleName);

            try
            {
                store.DeleteFilesAsync("", "*", false).Wait();
            }
            catch (Exception e)
            {
                Log.Error(e);
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
                    var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().TenantId.ToString(), moduleName);
                    DeleteLogoFromStore(tenantWhiteLabelSettings, store, type);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        #endregion

        #region Set logo

        public void SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string logoFileExt, byte[] data, IDataStore storage = null)
        {
            var store = storage ?? StorageFactory.GetStorage(TenantManager.GetCurrentTenant().TenantId.ToString(), moduleName);

            #region delete from storage if already exists

            var isAlreadyHaveBeenChanged = !tenantWhiteLabelSettings.GetIsDefault(type);

            if (isAlreadyHaveBeenChanged)
            {
                try
                {
                    DeleteLogoFromStore(tenantWhiteLabelSettings, store, type);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            #endregion

            using (var memory = new MemoryStream(data))
            using (var image = Image.Load(memory))
            {
                var logoFileName = BuildLogoFileName(type, logoFileExt, false);

                memory.Seek(0, SeekOrigin.Begin);
                store.SaveAsync(logoFileName, memory).Wait();
            }

            tenantWhiteLabelSettings.SetExt(type, logoFileExt);
            tenantWhiteLabelSettings.SetIsDefault(type, false);

            var generalSize = GetSize(type, true);
            var generalFileName = BuildLogoFileName(type, logoFileExt, true);
            ResizeLogo(generalFileName, data, -1, generalSize, store);
        }

        public void SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, Dictionary<int, string> logo, IDataStore storage = null)
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
                        data = UserPhotoManager.GetTempPhotoData(fileName);
                        try
                        {
                            UserPhotoManager.RemoveTempPhoto(fileName);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                    else
                    {
                        var xB64 = currentLogoPath.Substring(xStart.Length); // Get the Base64 string
                        data = System.Convert.FromBase64String(xB64); // Convert the Base64 string to binary data
                    }

                    if (data != null)
                    {
                        SetLogo(tenantWhiteLabelSettings, currentLogoType, fileExt, data, storage);
                    }
                }
            }
        }

        public void SetLogoFromStream(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string fileExt, Stream fileStream, IDataStore storage = null)
        {
            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                data = memoryStream.ToArray();
            }

            if (data != null)
            {
                SetLogo(tenantWhiteLabelSettings, type, fileExt, data, storage);
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
            var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().TenantId.ToString(), moduleName);
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
                return partnerLogoPath;

            return type switch
            {
                WhiteLabelLogoTypeEnum.LightSmall => general ? WebImageSupplier.GetAbsoluteWebPath("logo/light_small_general.svg") : WebImageSupplier.GetAbsoluteWebPath("logo/light_small.svg"),
                WhiteLabelLogoTypeEnum.Dark => general ? WebImageSupplier.GetAbsoluteWebPath("logo/dark_general.png") : WebImageSupplier.GetAbsoluteWebPath("logo/dark.png"),
                WhiteLabelLogoTypeEnum.DocsEditor => general ? WebImageSupplier.GetAbsoluteWebPath("logo/editor_logo_general.png") : WebImageSupplier.GetAbsoluteWebPath("logo/editor_logo.png"),
                WhiteLabelLogoTypeEnum.DocsEditorEmbed => general ? WebImageSupplier.GetAbsoluteWebPath("logo/editor_logo_embed_general.png") : WebImageSupplier.GetAbsoluteWebPath("logo/editor_logo_embed.png"),
                WhiteLabelLogoTypeEnum.Favicon => general ? WebImageSupplier.GetAbsoluteWebPath("logo/favicon_general.ico") : WebImageSupplier.GetAbsoluteWebPath("logo/favicon.ico"),
                _ => "",
            };
        }

        private string GetPartnerStorageLogoPath(WhiteLabelLogoTypeEnum type, bool general)
        {
            var partnerSettings = SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();

            if (partnerSettings.GetIsDefault(type)) return null;

            var partnerStorage = StorageFactory.GetStorage(string.Empty, "static_partnerdata");

            if (partnerStorage == null) return null;

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
                return GetPartnerStorageLogoData(type, general);

            return GetStorageLogoData(tenantWhiteLabelSettings, type, general);
        }

        private Stream GetStorageLogoData(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, bool general)
        {
            var storage = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().TenantId.ToString(CultureInfo.InvariantCulture), moduleName);

            if (storage == null) return null;

            var fileName = BuildLogoFileName(type, tenantWhiteLabelSettings.GetExt(type), general);

            return storage.IsFileAsync(fileName).Result ? storage.GetReadStreamAsync(fileName).Result : null;
        }

        private Stream GetPartnerStorageLogoData(WhiteLabelLogoTypeEnum type, bool general)
        {
            var partnerSettings = SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();

            if (partnerSettings.GetIsDefault(type)) return null;

            var partnerStorage = StorageFactory.GetStorage(string.Empty, "static_partnerdata");

            if (partnerStorage == null) return null;

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
                       general ? TenantWhiteLabelSettings.logoLightSmallSize.Width / 2 : TenantWhiteLabelSettings.logoLightSmallSize.Width,
                       general ? TenantWhiteLabelSettings.logoLightSmallSize.Height / 2 : TenantWhiteLabelSettings.logoLightSmallSize.Height),
                WhiteLabelLogoTypeEnum.Dark => new Size(
                        general ? TenantWhiteLabelSettings.logoDarkSize.Width / 2 : TenantWhiteLabelSettings.logoDarkSize.Width,
                        general ? TenantWhiteLabelSettings.logoDarkSize.Height / 2 : TenantWhiteLabelSettings.logoDarkSize.Height),
                WhiteLabelLogoTypeEnum.Favicon => new Size(
                        general ? TenantWhiteLabelSettings.logoFaviconSize.Width / 2 : TenantWhiteLabelSettings.logoFaviconSize.Width,
                        general ? TenantWhiteLabelSettings.logoFaviconSize.Height / 2 : TenantWhiteLabelSettings.logoFaviconSize.Height),
                WhiteLabelLogoTypeEnum.DocsEditor => new Size(
                        general ? TenantWhiteLabelSettings.logoDocsEditorSize.Width / 2 : TenantWhiteLabelSettings.logoDocsEditorSize.Width,
                        general ? TenantWhiteLabelSettings.logoDocsEditorSize.Height / 2 : TenantWhiteLabelSettings.logoDocsEditorSize.Height),
                WhiteLabelLogoTypeEnum.DocsEditorEmbed => new Size(
                        general ? TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Width / 2 : TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Width,
                        general ? TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Height / 2 : TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Height),
                _ => new Size(0, 0),
            };
        }

        private static void ResizeLogo(string fileName, byte[] data, long maxFileSize, Size size, IDataStore store)
        {
            //Resize synchronously
            if (data == null || data.Length <= 0) throw new UnknownImageFormatException("data null");
            if (maxFileSize != -1 && data.Length > maxFileSize) throw new ImageWeightLimitException();

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

        private static readonly List<int> AppliedTenants = new List<int>();

        public void Apply(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId)
        {
            if (AppliedTenants.Contains(tenantId)) return;

            SetNewLogoText(tenantWhiteLabelSettings, tenantId);

            if (!AppliedTenants.Contains(tenantId)) AppliedTenants.Add(tenantId);
        }

        public void Save(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId, TenantLogoManager tenantLogoManager, bool restore = false)
        {
            SettingsManager.SaveForTenant(tenantWhiteLabelSettings, tenantId);

            if (tenantId == Tenant.DEFAULT_TENANT)
            {
                AppliedTenants.Clear();
            }
            else
            {
                SetNewLogoText(tenantWhiteLabelSettings, tenantId, restore);
                tenantLogoManager.RemoveMailLogoDataFromCache();
            }
        }

        private void SetNewLogoText(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId, bool restore = false)
        {
            WhiteLabelHelper.DefaultLogoText = TenantWhiteLabelSettings.DefaultLogoText;
            var partnerSettings = SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();

            if (restore && string.IsNullOrEmpty(partnerSettings.GetLogoText(SettingsManager)))
            {
                WhiteLabelHelper.RestoreOldText(tenantId);
            }
            else
            {
                WhiteLabelHelper.SetNewText(tenantId, tenantWhiteLabelSettings.GetLogoText(SettingsManager));
            }
        }

        #endregion

        #region Delete from Store

        private void DeleteLogoFromStore(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type)
        {
            DeleteLogoFromStoreByGeneral(tenantWhiteLabelSettings, store, type, false);
            DeleteLogoFromStoreByGeneral(tenantWhiteLabelSettings, store, type, true);
        }

        private void DeleteLogoFromStoreByGeneral(TenantWhiteLabelSettings tenantWhiteLabelSettings, IDataStore store, WhiteLabelLogoTypeEnum type, bool general)
        {
            var fileExt = tenantWhiteLabelSettings.GetExt(type);
            var logo = BuildLogoFileName(type, fileExt, general);
            if (store.IsFileAsync(logo).Result)
            {
                store.DeleteAsync(logo).Wait();
            }
        }

        #endregion
    }
}