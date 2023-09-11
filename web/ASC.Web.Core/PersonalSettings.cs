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

namespace ASC.Web.Studio.Core;

public class PersonalSettings : ISettings<PersonalSettings>
{

    [JsonPropertyName("IsNewUser")]
    public bool IsNewUserSetting { get; set; }

    [JsonPropertyName("IsNotActivated")]
    public bool IsNotActivatedSetting { get; set; }

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{B3427865-8E32-4E66-B6F3-91C61922239F}"); }
    }

    public PersonalSettings GetDefault()
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
    private readonly SettingsManager _settingsManager;

    public PersonalSettingsHelper(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public bool IsNewUser
    {
        get { return _settingsManager.LoadForCurrentUser<PersonalSettings>().IsNewUserSetting; }
        set
        {
            var settings = _settingsManager.LoadForCurrentUser<PersonalSettings>();
            settings.IsNewUserSetting = value;
            _settingsManager.SaveForCurrentUser(settings);
        }
    }

    public bool IsNotActivated
    {
        get { return _settingsManager.LoadForCurrentUser<PersonalSettings>().IsNotActivatedSetting; }
        set
        {
            var settings = _settingsManager.LoadForCurrentUser<PersonalSettings>();
            settings.IsNotActivatedSetting = value;
            _settingsManager.SaveForCurrentUser(settings);
        }
    }
}
