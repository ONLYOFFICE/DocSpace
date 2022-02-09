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

namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class PersonalSettings : ISettings
    {

        [JsonPropertyName("IsNewUser")]
        public bool IsNewUserSetting { get; set; }

        [JsonPropertyName("IsNotActivated")]
        public bool IsNotActivatedSetting { get; set; }

        public Guid ID
        {
            get { return new Guid("{B3427865-8E32-4E66-B6F3-91C61922239F}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new PersonalSettings
            {
                IsNewUserSetting = false,
                IsNotActivatedSetting = false,
            };
        }
    }

    [Scope]
    public class PersonalSettingsHelper
    {
        public PersonalSettingsHelper(SettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public bool IsNewUser
        {
            get { return SettingsManager.LoadForCurrentUser<PersonalSettings>().IsNewUserSetting; }
            set
            {
                var settings = SettingsManager.LoadForCurrentUser<PersonalSettings>();
                settings.IsNewUserSetting = value;
                SettingsManager.SaveForCurrentUser(settings);
            }
        }

        public bool IsNotActivated
        {
            get { return SettingsManager.LoadForCurrentUser<PersonalSettings>().IsNotActivatedSetting; }
            set
            {
                var settings = SettingsManager.LoadForCurrentUser<PersonalSettings>();
                settings.IsNotActivatedSetting = value;
                SettingsManager.SaveForCurrentUser(settings);
            }
        }

        private SettingsManager SettingsManager { get; }
    }
}