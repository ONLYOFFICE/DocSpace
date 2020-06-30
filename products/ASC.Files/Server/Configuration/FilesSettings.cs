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
using System.Runtime.Serialization;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Files.Core;

namespace ASC.Web.Files.Classes
{
    [Serializable]
    public class FilesSettings : ISettings
    {
        public bool EnableThirdpartySetting { get; set; }

        public bool FastDeleteSetting { get; set; }

        public bool StoreOriginalFilesSetting { get; set; }

        public bool UpdateIfExistSetting { get; set; }

        public bool ConvertNotifySetting { get; set; }

        public SortedByType DefaultSortedBySetting { get; set; }

        public bool DefaultSortedAscSetting { get; set; }

        public bool HideConfirmConvertSaveSetting { get; set; }

        public bool HideConfirmConvertOpenSetting { get; set; }

        public bool ForcesaveSetting { get; set; }

        public bool StoreForcesaveSetting { get; set; }

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
                StoreForcesaveSetting = false
            };
        }

        public Guid ID
        {
            get { return new Guid("{03B382BD-3C20-4f03-8AB9-5A33F016316E}"); }
        }
    }

    public class FilesSettingsHelper
    {
        public SettingsManager SettingsManager { get; }
        public CoreBaseSettings CoreBaseSettings { get; }

        public FilesSettingsHelper(SettingsManager settingsManager, CoreBaseSettings coreBaseSettings)
        {
            SettingsManager = settingsManager;
            CoreBaseSettings = coreBaseSettings;
        }

        public bool ConfirmDelete
        {
            set
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.FastDeleteSetting = !value;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get { return !SettingsManager.LoadForCurrentUser<FilesSettings>().FastDeleteSetting; }
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
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.StoreOriginalFilesSetting = value;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get { return SettingsManager.LoadForCurrentUser<FilesSettings>().StoreOriginalFilesSetting; }
        }

        public bool UpdateIfExist
        {
            set
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.UpdateIfExistSetting = value;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get { return SettingsManager.LoadForCurrentUser<FilesSettings>().UpdateIfExistSetting; }
        }

        public bool ConvertNotify
        {
            set
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.ConvertNotifySetting = value;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get { return SettingsManager.LoadForCurrentUser<FilesSettings>().ConvertNotifySetting; }
        }

        public bool HideConfirmConvertSave
        {
            set
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.HideConfirmConvertSaveSetting = value;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get { return SettingsManager.LoadForCurrentUser<FilesSettings>().HideConfirmConvertSaveSetting; }
        }

        public bool HideConfirmConvertOpen
        {
            set
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.HideConfirmConvertOpenSetting = value;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get { return SettingsManager.LoadForCurrentUser<FilesSettings>().HideConfirmConvertOpenSetting; }
        }

        public OrderBy DefaultOrder
        {
            set
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.DefaultSortedBySetting = value.SortedBy;
                setting.DefaultSortedAscSetting = value.IsAsc;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                return new OrderBy(setting.DefaultSortedBySetting, setting.DefaultSortedAscSetting);
            }
        }

        public bool Forcesave
        {
            set
            {
                var setting = SettingsManager.LoadForCurrentUser<FilesSettings>();
                setting.ForcesaveSetting = value;
                SettingsManager.SaveForCurrentUser(setting);
            }
            get { return SettingsManager.LoadForCurrentUser<FilesSettings>().ForcesaveSetting; }
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
    }
    public static class FilesSettingsHelperExtention
    {
        public static DIHelper AddFilesSettingsHelperService(this DIHelper services)
        {
            services.TryAddScoped<FilesSettingsHelper>();
            return services
                .AddSettingsManagerService()
                .AddCoreBaseSettingsService();
        }
    }
}