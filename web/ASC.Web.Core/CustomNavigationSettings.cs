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

public class CustomNavigationSettings : ISettings<CustomNavigationSettings>
{
    public List<CustomNavigationItem> Items { get; set; }

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{32E02E4C-925D-4391-BAA4-3B5D223A2104}"); }
    }

    public CustomNavigationSettings GetDefault()
    {
        return new CustomNavigationSettings { Items = new List<CustomNavigationItem>() };
    }
}

/// <summary>
/// </summary>
public class CustomNavigationItem
{
    /// <summary>ID</summary>
    /// <type>System.Guid, System</type>
    /// <exammple>"00000000-0000-0000-0000-000000000000"</exammple>
    public Guid Id { get; set; }

    /// <summary>Label</summary>
    /// <type>System.String, System</type>
    /// <exammple>"Label"</exammple>
    public string Label { get; set; }

    /// <summary>URL</summary>
    /// <type>System.String, System</type>
    /// <exammple>"Url"</exammple>
    public string Url { get; set; }

    /// <summary>Big image</summary>
    /// <type>System.String, System</type>
    /// <exammple>"data:image\\/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGQAAABkAgMAAAANjH3HAAAADFBMVEUAAADJycnJycnJycmiuNtHAAAAA3RSTlMAf4C\\/aSLHAAAAyElEQVR4Xu3NsQ3CMBSE4YubFB4ilHQegdGSjWACvEpGoEyBYiL05AdnXUGHolx10lf82MmOpfLeo5UoJUhBlpKkRCnhUy7b9XCWkqQMUkYlXVHSf8kTvkHKqKQrSnopg5SRxTMklLmS1MwaSWpmCSQ1MyOzWGZCYrEMEFksA4QqlAFuJJYBcCKxjM3FMySeIfEMC2dMOONCGZZgmdr1ly3TSrJMK9EyJBaaGrHQikYstAiJZRYSyiQEdyg5S8Evckih\\/YPscsdej0H6dc0TYw4AAAAASUVORK5CYII="</exammple>
    public string BigImg { get; set; }

    /// <summary>Small image</summary>
    /// <type>System.String, System</type>
    /// <exammple>"data:image\\/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8\\/9hAAAAUUlEQVR4AWMY\\/KC5o\\/cAEP9HxxgKcSpCGELYADyu2E6mAQjNxBlAWPNxkHdwGkBIM3KYYDUAr2ZCAE+oH8eujrAXDsA0k2EAAtDXAGLx4MpsADUgvkRKUlqfAAAAAElFTkSuQmCC"</exammple>
    public string SmallImg { get; set; }

    /// <summary>Show in menu or not</summary>
    /// <type>System.Boolean, System</type>
    /// <exammple>true</exammple>
    public bool ShowInMenu { get; set; }

    /// <summary>Show on home page or not</summary>
    /// <type>System.Boolean, System</type>
    /// <exammple>true</exammple>
    public bool ShowOnHomePage { get; set; }

    private static string GetDefaultBigImg()
    {
        return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGQAAABkAgMAAAANjH3HAAAADFBMVEUAAADJycnJycnJycmiuNtHAAAAA3RSTlMAf4C/aSLHAAAAyElEQVR4Xu3NsQ3CMBSE4YubFB4ilHQegdGSjWACvEpGoEyBYiL05AdnXUGHolx10lf82MmOpfLeo5UoJUhBlpKkRCnhUy7b9XCWkqQMUkYlXVHSf8kTvkHKqKQrSnopg5SRxTMklLmS1MwaSWpmCSQ1MyOzWGZCYrEMEFksA4QqlAFuJJYBcCKxjM3FMySeIfEMC2dMOONCGZZgmdr1ly3TSrJMK9EyJBaaGrHQikYstAiJZRYSyiQEdyg5S8Evckih/YPscsdej0H6dc0TYw4AAAAASUVORK5CYII=";
    }

    private static string GetDefaultSmallImg()
    {
        return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAUUlEQVR4AWMY/KC5o/cAEP9HxxgKcSpCGELYADyu2E6mAQjNxBlAWPNxkHdwGkBIM3KYYDUAr2ZCAE+oH8eujrAXDsA0k2EAAtDXAGLx4MpsADUgvkRKUlqfAAAAAElFTkSuQmCC";
    }

    public static CustomNavigationItem GetSample()
    {
        return new CustomNavigationItem
        {
            Id = Guid.Empty,
            ShowInMenu = true,
            ShowOnHomePage = true,
            BigImg = GetDefaultBigImg(),
            SmallImg = GetDefaultSmallImg()
        };
    }
}
