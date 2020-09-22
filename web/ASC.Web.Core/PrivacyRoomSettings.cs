/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
            return SetupInfo.IsVisibleSettings(ManagementType.PrivacyRoom.ToString())
                && tenantManager.GetTenantQuota(tenantManager.GetCurrentTenant().TenantId).PrivacyRoom;
        }
    }
}