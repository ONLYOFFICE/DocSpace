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
using System.Text.Json.Serialization;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Files.Core;
using ASC.Web.Studio.Core;

namespace ASC.Web.Files.Classes
{
    [Serializable]
    public class FilesSettings : ISettings
    {
        [JsonPropertyName("EnableThirdpartySettings")]
        public bool EnableThirdpartySetting { get; set; }

        [JsonPropertyName("FastDelete")]
        public bool FastDeleteSetting { get; set; }

        [JsonPropertyName("StoreOriginalFiles")]
        public bool StoreOriginalFilesSetting { get; set; }

        [JsonPropertyName("UpdateIfExist")]
        public bool UpdateIfExistSetting { get; set; }

        [JsonPropertyName("ConvertNotify")]
        public bool ConvertNotifySetting { get; set; }

        [JsonPropertyName("DefaultSortedBy")]
        public SortedByType DefaultSortedBySetting { get; set; }

        [JsonPropertyName("DefaultSortedAsc")]
        public bool DefaultSortedAscSetting { get; set; }

        [JsonPropertyName("HideConfirmConvertSave")]
        public bool HideConfirmConvertSaveSetting { get; set; }

        [JsonPropertyName("HideConfirmConvertOpen")]
        public bool HideConfirmConvertOpenSetting { get; set; }

        [JsonPropertyName("Forcesave")]
        public bool ForcesaveSetting { get; set; }

        [JsonPropertyName("StoreForcesave")]
        public bool StoreForcesaveSetting { get; set; }

        [JsonPropertyName("HideRecent")]
        public bool HideRecentSetting { get; set; }

        [JsonPropertyName("HideFavorites")]
        public bool HideFavoritesSetting { get; set; }

        [JsonPropertyName("HideTemplates")]
        public bool HideTemplatesSetting { get; set; }

        [JsonPropertyName("DownloadZip")]
        public bool DownloadTarGzSetting { get; set; }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new FilesSettings
            {
                FastDeleteSetting = false,
                EnableThirdpartySetting = true,
                StoreOriginalFilesSetting = true,
                UpdateIfExistSetting = false,
                ConvertNotifySetting = true,
                DefaultSortedBySetting = SortedByType.DateAndTime,
                DefaultSortedAscSetting = false,
                HideConfirmConvertSaveSetting = false,
                HideConfirmConvertOpenSetting = false,
                ForcesaveSetting = false,
                StoreForcesaveSetting = false,
                HideRecentSetting = false,
                HideFavoritesSetting = false,
                HideTemplatesSetting = false,
                DownloadTarGzSetting = false
            };
        }

        public Guid ID
        {
            get { return new Guid("{03B382BD-3C20-4f03-8AB9-5A33F016316E}"); }
        }
    }

    [Scope]
    public class FilesSettingsHelper
    {
        private SettingsManager SettingsManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private SetupInfo SetupInfo { get; }

        public FilesSettingsHelper(SettingsManager settingsManager, CoreBaseSettings coreBaseSettings, SetupInfo setupInfo)
        {
            SettingsManager = settingsManager;
            CoreBaseSettings = coreBaseSettings;
            SetupInfo = setupInfo;
        }

        public bool ConfirmDelete
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.FastDeleteSetting = !value;
                SaveForCurrentUser(setting);
            }
            get { return !LoadForCurrentUser().FastDeleteSetting; }
        }

        public bool EnableThirdParty
        {
            set
            {
                var setting = SettingsManager.Load<FilesSettings>();
                setting.EnableThirdpartySetting = value;
                SettingsManager.Save(setting);
            }
            get { return SettingsManager.Load<FilesSettings>().EnableThirdpartySetting; }
        }

        public bool StoreOriginalFiles
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.StoreOriginalFilesSetting = value;
                SaveForCurrentUser(setting);
            }
            get { return LoadForCurrentUser().StoreOriginalFilesSetting; }
        }

        public bool UpdateIfExist
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.UpdateIfExistSetting = value;
                SaveForCurrentUser(setting);
            }
            get { return LoadForCurrentUser().UpdateIfExistSetting; }
        }

        public bool ConvertNotify
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.ConvertNotifySetting = value;
                SaveForCurrentUser(setting);
            }
            get { return LoadForCurrentUser().ConvertNotifySetting; }
        }

        public bool HideConfirmConvertSave
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.HideConfirmConvertSaveSetting = value;
                SaveForCurrentUser(setting);
            }
            get { return LoadForCurrentUser().HideConfirmConvertSaveSetting; }
        }

        public bool HideConfirmConvertOpen
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.HideConfirmConvertOpenSetting = value;
                SaveForCurrentUser(setting);
            }
            get { return LoadForCurrentUser().HideConfirmConvertOpenSetting; }
        }

        public OrderBy DefaultOrder
        {
            set
            {
                var setting = LoadForCurrentUser();
                if (setting.DefaultSortedBySetting != value.SortedBy || setting.DefaultSortedAscSetting != value.IsAsc)
                {
                    setting.DefaultSortedBySetting = value.SortedBy;
                    setting.DefaultSortedAscSetting = value.IsAsc;
                    SaveForCurrentUser(setting);
                }
            }
            get
            {
                var setting = LoadForCurrentUser();
                return new OrderBy(setting.DefaultSortedBySetting, setting.DefaultSortedAscSetting);
            }
        }

        public bool Forcesave
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.ForcesaveSetting = value;
                SaveForCurrentUser(setting);
            }
            get { return LoadForCurrentUser().ForcesaveSetting; }
        }

        public bool StoreForcesave
        {
            set
            {
                if (CoreBaseSettings.Personal) throw new NotSupportedException();
                var setting = SettingsManager.Load<FilesSettings>();
                setting.StoreForcesaveSetting = value;
                SettingsManager.Save(setting);
            }
            get { return !CoreBaseSettings.Personal && SettingsManager.Load<FilesSettings>().StoreForcesaveSetting; }
        }

        public bool RecentSection
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.HideRecentSetting = !value;
                SaveForCurrentUser(setting);
            }
            get { return !LoadForCurrentUser().HideRecentSetting; }
        }

        public bool FavoritesSection
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.HideFavoritesSetting = !value;
                SaveForCurrentUser(setting);
            }
            get { return !LoadForCurrentUser().HideFavoritesSetting; }
        }

        public bool TemplatesSection
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.HideTemplatesSetting = !value;
                SaveForCurrentUser(setting);
            }
            get { return !LoadForCurrentUser().HideTemplatesSetting; }
        }

        public bool DownloadTarGz
        {
            set
            {
                var setting = LoadForCurrentUser();
                setting.DownloadTarGzSetting = value;
                SaveForCurrentUser(setting);
            }
            get => LoadForCurrentUser().DownloadTarGzSetting;
        }

        public long ChunkUploadSize
        {
            get => SetupInfo.ChunkUploadSize;
        }

        private FilesSettings LoadForCurrentUser()
        {
            return SettingsManager.LoadForCurrentUser<FilesSettings>();
        }

        private void SaveForCurrentUser(FilesSettings settings)
        {
            SettingsManager.SaveForCurrentUser(settings);
        }
    }
}