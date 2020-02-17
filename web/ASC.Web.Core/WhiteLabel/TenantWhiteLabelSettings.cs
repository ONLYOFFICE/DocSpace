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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Common.WhiteLabel;
using ASC.Data.Storage;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;

using Microsoft.Extensions.Options;

using TMResourceData;

namespace ASC.Web.Core.WhiteLabel
{
    [Serializable]
    [DataContract]
    public class TenantWhiteLabelSettings : ISettings
    {
        public const string DefaultLogoText = BaseWhiteLabelSettings.DefaultLogoText;

        #region Logos information: extension, isDefault, text for img auto generating

        [DataMember(Name = "LogoLightSmallExt")]
        internal string _logoLightSmallExt;

        [DataMember(Name = "DefaultLogoLightSmall")]
        internal bool _isDefaultLogoLightSmall { get; set; }

        [DataMember(Name = "LogoDarkExt")]
        internal string _logoDarkExt;

        [DataMember(Name = "DefaultLogoDark")]
        internal bool _isDefaultLogoDark { get; set; }

        [DataMember(Name = "LogoFaviconExt")]
        internal string _logoFaviconExt;

        [DataMember(Name = "DefaultLogoFavicon")]
        internal bool _isDefaultLogoFavicon { get; set; }

        [DataMember(Name = "LogoDocsEditorExt")]
        internal string _logoDocsEditorExt;

        [DataMember(Name = "DefaultLogoDocsEditor")]
        internal bool _isDefaultLogoDocsEditor { get; set; }


        [DataMember(Name = "LogoText")]
        private string _logoText { get; set; }

        public string GetLogoText(SettingsManager settingsManager)
        {
            if (!string.IsNullOrEmpty(_logoText) && _logoText != DefaultLogoText)
                return _logoText;

            var partnerSettings = settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
            return string.IsNullOrEmpty(partnerSettings._logoText) ? DefaultLogoText : partnerSettings._logoText;
        }

        public void SetLogoText(string val)
        {
            _logoText = val;
        }

        #endregion

        #region Logo available sizes

        public static readonly Size logoLightSmallSize = new Size(284, 46);
        public static readonly Size logoDarkSize = new Size(432, 70);
        public static readonly Size logoFaviconSize = new Size(32, 32);
        public static readonly Size logoDocsEditorSize = new Size(172, 40);

        #endregion

        #region ISettings Members

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TenantWhiteLabelSettings
            {
                _logoLightSmallExt = null,
                _logoDarkExt = null,
                _logoFaviconExt = null,
                _logoDocsEditorExt = null,

                _isDefaultLogoLightSmall = true,
                _isDefaultLogoDark = true,
                _isDefaultLogoFavicon = true,
                _isDefaultLogoDocsEditor = true,

                _logoText = null
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
                WhiteLabelLogoTypeEnum.LightSmall => _isDefaultLogoLightSmall,
                WhiteLabelLogoTypeEnum.Dark => _isDefaultLogoDark,
                WhiteLabelLogoTypeEnum.Favicon => _isDefaultLogoFavicon,
                WhiteLabelLogoTypeEnum.DocsEditor => _isDefaultLogoDocsEditor,
                _ => true,
            };
        }

        internal void SetIsDefault(WhiteLabelLogoTypeEnum type, bool value)
        {
            switch (type)
            {
                case WhiteLabelLogoTypeEnum.LightSmall:
                    _isDefaultLogoLightSmall = value;
                    break;
                case WhiteLabelLogoTypeEnum.Dark:
                    _isDefaultLogoDark = value;
                    break;
                case WhiteLabelLogoTypeEnum.Favicon:
                    _isDefaultLogoFavicon = value;
                    break;
                case WhiteLabelLogoTypeEnum.DocsEditor:
                    _isDefaultLogoDocsEditor = value;
                    break;
            }
        }

        internal string GetExt(WhiteLabelLogoTypeEnum type)
        {
            return type switch
            {
                WhiteLabelLogoTypeEnum.LightSmall => _logoLightSmallExt,
                WhiteLabelLogoTypeEnum.Dark => _logoDarkExt,
                WhiteLabelLogoTypeEnum.Favicon => _logoFaviconExt,
                WhiteLabelLogoTypeEnum.DocsEditor => _logoDocsEditorExt,
                _ => "",
            };
        }

        internal void SetExt(WhiteLabelLogoTypeEnum type, string fileExt)
        {
            switch (type)
            {
                case WhiteLabelLogoTypeEnum.LightSmall:
                    _logoLightSmallExt = fileExt;
                    break;
                case WhiteLabelLogoTypeEnum.Dark:
                    _logoDarkExt = fileExt;
                    break;
                case WhiteLabelLogoTypeEnum.Favicon:
                    _logoFaviconExt = fileExt;
                    break;
                case WhiteLabelLogoTypeEnum.DocsEditor:
                    _logoDocsEditorExt = fileExt;
                    break;
            }
        }

        #endregion
    }

    public class TenantWhiteLabelSettingsHelper
    {
        private const string moduleName = "whitelabel";

        public WebImageSupplier WebImageSupplier { get; }
        public UserPhotoManager UserPhotoManager { get; }
        public StorageFactory StorageFactory { get; }
        public WhiteLabelHelper WhiteLabelHelper { get; }
        public TenantManager TenantManager { get; }
        public SettingsManager SettingsManager { get; }
        public CoreBaseSettings CoreBaseSettings { get; }
        public IOptionsMonitor<ILog> Option { get; }

        public ILog Log { get; set; }

        public TenantWhiteLabelSettingsHelper(
            WebImageSupplier webImageSupplier,
            UserPhotoManager userPhotoManager,
            StorageFactory storageFactory,
            WhiteLabelHelper whiteLabelHelper,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            CoreBaseSettings coreBaseSettings,
            IOptionsMonitor<ILog> option)
        {
            WebImageSupplier = webImageSupplier;
            UserPhotoManager = userPhotoManager;
            StorageFactory = storageFactory;
            WhiteLabelHelper = whiteLabelHelper;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            CoreBaseSettings = coreBaseSettings;
            Option = option;
            Log = option.CurrentValue;
        }

        #region Restore default

        public void RestoreDefault(TenantWhiteLabelSettings tenantWhiteLabelSettings, TenantLogoManager tenantLogoManager)
        {
            tenantWhiteLabelSettings._logoLightSmallExt = null;
            tenantWhiteLabelSettings._logoDarkExt = null;
            tenantWhiteLabelSettings._logoFaviconExt = null;
            tenantWhiteLabelSettings._logoDocsEditorExt = null;

            tenantWhiteLabelSettings._isDefaultLogoLightSmall = true;
            tenantWhiteLabelSettings._isDefaultLogoDark = true;
            tenantWhiteLabelSettings._isDefaultLogoFavicon = true;
            tenantWhiteLabelSettings._isDefaultLogoDocsEditor = true;

            tenantWhiteLabelSettings.SetLogoText(null);

            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            var store = StorageFactory.GetStorage(tenantId.ToString(), moduleName);
            try
            {
                store.DeleteFiles("", "*", false);
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

        public void SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string logoFileExt, byte[] data)
        {
            var store = StorageFactory.GetStorage(TenantManager.GetCurrentTenant().TenantId.ToString(), moduleName);

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
            using (var image = Image.FromStream(memory))
            {
                var logoSize = image.Size;
                var logoFileName = BuildLogoFileName(type, logoFileExt, false);

                memory.Seek(0, SeekOrigin.Begin);
                store.Save(logoFileName, memory);
            }

            tenantWhiteLabelSettings.SetExt(type, logoFileExt);
            tenantWhiteLabelSettings.SetIsDefault(type, false);

            var generalSize = GetSize(type, true);
            var generalFileName = BuildLogoFileName(type, logoFileExt, true);
            ResizeLogo(type, generalFileName, data, -1, generalSize, store);
        }

        public void SetLogo(TenantWhiteLabelSettings tenantWhiteLabelSettings, Dictionary<int, string> logo)
        {
            var xStart = @"data:image/png;base64,";

            foreach (var currentLogo in logo)
            {
                var currentLogoType = (WhiteLabelLogoTypeEnum)(currentLogo.Key);
                var currentLogoPath = currentLogo.Value;

                if (!string.IsNullOrEmpty(currentLogoPath))
                {
                    var fileExt = "png";
                    byte[] data = null;

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
                        SetLogo(tenantWhiteLabelSettings, currentLogoType, fileExt, data);
                    }
                }
            }
        }

        public void SetLogoFromStream(TenantWhiteLabelSettings tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum type, string fileExt, Stream fileStream)
        {
            byte[] data = null;
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                data = memoryStream.ToArray();
            }

            if (data != null)
            {
                SetLogo(tenantWhiteLabelSettings, type, fileExt, data);
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

            if (store.IsFile(fileName))
            {
                return store.GetUri(fileName).ToString();
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
                WhiteLabelLogoTypeEnum.LightSmall => general ? WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/light_small_general.svg") : WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/light_small.svg"),
                WhiteLabelLogoTypeEnum.Dark => general ? WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/dark_general.png") : WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/dark.png"),
                WhiteLabelLogoTypeEnum.DocsEditor => general ? WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/editor_logo_general.png") : WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/editor_logo.png"),
                WhiteLabelLogoTypeEnum.Favicon => general ? WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/favicon_general.ico") : WebImageSupplier.GetAbsoluteWebPath("onlyoffice_logo/favicon.ico"),
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

            return partnerStorage.IsFile(logoPath) ? partnerStorage.GetUri(logoPath).ToString() : null;
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

            return storage.IsFile(fileName) ? storage.GetReadStream(fileName) : null;
        }

        private Stream GetPartnerStorageLogoData(WhiteLabelLogoTypeEnum type, bool general)
        {
            var partnerSettings = SettingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();

            if (partnerSettings.GetIsDefault(type)) return null;

            var partnerStorage = StorageFactory.GetStorage(string.Empty, "static_partnerdata");

            if (partnerStorage == null) return null;

            var fileName = BuildLogoFileName(type, partnerSettings.GetExt(type), general);

            return partnerStorage.IsFile(fileName) ? partnerStorage.GetReadStream(fileName) : null;
        }

        #endregion

        public static string BuildLogoFileName(WhiteLabelLogoTypeEnum type, string fileExt, bool general)
        {
            return string.Format("logo_{0}{2}.{1}", type.ToString().ToLowerInvariant(), fileExt, general ? "_general" : "");
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
                _ => new Size(0, 0),
            };
        }

        private static void ResizeLogo(WhiteLabelLogoTypeEnum type, string fileName, byte[] data, long maxFileSize, Size size, IDataStore store)
        {
            //Resize synchronously
            if (data == null || data.Length <= 0) throw new UnknownImageFormatException();
            if (maxFileSize != -1 && data.Length > maxFileSize) throw new ImageWeightLimitException();

            try
            {
                using var stream = new MemoryStream(data);
                using var img = Image.FromStream(stream);
                var imgFormat = img.RawFormat;
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
                store.Save(fileName, stream2);
            }
            catch (ArgumentException error)
            {
                throw new UnknownImageFormatException(error);
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
            SetNewLogoText(tenantWhiteLabelSettings, tenantId, restore);

            tenantLogoManager.RemoveMailLogoDataFromCache();
        }

        private void SetNewLogoText(TenantWhiteLabelSettings tenantWhiteLabelSettings, int tenantId, bool restore = false)
        {
            WhiteLabelHelper.DefaultLogoText = TenantWhiteLabelSettings.DefaultLogoText;
            if (restore && !CoreBaseSettings.CustomMode)
            {
                WhiteLabelHelper.RestoreOldText(tenantId);
            }
            else if (!string.IsNullOrEmpty(tenantWhiteLabelSettings.GetLogoText(SettingsManager)))
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
            if (store.IsFile(logo))
            {
                store.Delete(logo);
            }
        }

        #endregion
    }

    public static class TenantWhiteLabelSettingsExtension
    {
        public static DIHelper AddTenantWhiteLabelSettingsService(this DIHelper services)
        {
            services.TryAddScoped<TenantWhiteLabelSettingsHelper>();
            return services
                .AddUserPhotoManagerService()
                .AddWebImageSupplierService()
                .AddStorageFactoryService()
                .AddWhiteLabelHelperService()
                .AddSettingsManagerService()
                .AddCoreBaseSettingsService();
        }
    }
}