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

namespace ASC.Web.Core;

[Singletone]
public class WebPluginSettings
{
    public WebPluginSettings(ConfigurationExtension configuration)
    {
        configuration.GetSetting("plugins", this);
    }

    private bool _enabled;
    private long _maxSize;
    private string _extension;
    private string[] _allow;
    private string[] _assetExtensions;
    private string _systemUrl;

    public bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }
    public long MaxSize
    {
        get => _maxSize > 0 ? _maxSize : 5L * 1024L * 1024L;
        set => _maxSize = value;
    }

    public string Extension
    {
        get => _extension ?? ".zip";
        set => _extension = value;
    }

    public string[] Allow
    {
        get => _allow ?? Array.Empty<string>();
        set => _allow = value;
    }

    public string[] AssetExtensions
    {
        get => _assetExtensions ?? Array.Empty<string>();
        set => _assetExtensions = value;
    }
}

public class SystemWebPluginSettings : ISettings<SystemWebPluginSettings>
{
    public List<string> DisabledPlugins { get; set; }

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{33039FD8-CF74-46B5-9AF2-2B3D4B651F31}"); }
    }

    public SystemWebPluginSettings GetDefault()
    {
        return new SystemWebPluginSettings();
    }
}