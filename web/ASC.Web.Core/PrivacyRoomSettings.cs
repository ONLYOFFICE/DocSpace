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

using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Core
{
    public class PrivacyRoomSettings : ISettings
    {
        [JsonPropertyName("enbaled")]
        public bool EnabledSetting { get; set; }

        public Guid ID
        {
            get { return new Guid("{FCF002BC-EC4B-4DAB-A6CE-BDE0ABDA44D3}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new PrivacyRoomSettings
            {
                EnabledSetting = true
            };
        }

        public static bool GetEnabled(SettingsManager settingsManager)
        {
            return settingsManager.Load<PrivacyRoomSettings>().EnabledSetting;
        }

        public static void SetEnabled(TenantManager tenantManager, SettingsManager settingsManager, bool value)
        {
            if (!IsAvailable(tenantManager)) return;

            var settings = settingsManager.Load<PrivacyRoomSettings>();
            settings.EnabledSetting = value;
            settingsManager.Save(settings);
        }

        public static bool IsAvailable(TenantManager tenantManager)
        {
            return SetupInfo.IsVisibleSettings(nameof(ManagementType.PrivacyRoom))
                && tenantManager.GetTenantQuota(tenantManager.GetCurrentTenant().TenantId).PrivacyRoom;
        }
    }
}