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

namespace ASC.Web.Api.ApiModel.ResponseDto;

/// <summary>
/// </summary>
public class UsageSpaceStatItemDto
{
    /// <summary>Name</summary>
    /// <type>System.String, System</type>
    public string Name { get; set; }

    /// <summary>Icon</summary>
    /// <type>System.String, System</type>
    public string Icon { get; set; }

    /// <summary>Specifies if the module space is disabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Disabled { get; set; }

    /// <summary>Size</summary>
    /// <type>System.String, System</type>
    public string Size { get; set; }

    /// <summary>URL</summary>
    /// <type>System.String, System</type>
    public string Url { get; set; }

    public static UsageSpaceStatItemDto GetSample()
    {
        return new UsageSpaceStatItemDto
        {
            Name = "Item name",
            Icon = "Item icon path",
            Disabled = false,
            Size = "0 Byte",
            Url = "Item url"
        };
    }
}

/// <summary>
/// </summary>
public class ChartPointDto
{
    /// <summary>Display date</summary>
    /// <type>System.String, System</type>
    public string DisplayDate { get; set; }

    /// <summary>Date</summary>
    /// <type>System.DateTime, System</type>
    public DateTime Date { get; set; }

    /// <summary>Hosts</summary>
    /// <type>System.Int32, System</type>
    public int Hosts { get; set; }

    /// <summary>Hits</summary>
    /// <type>System.Int32, System</type>
    public int Hits { get; set; }

    public static ChartPointDto GetSample()
    {
        return new ChartPointDto
        {
            DisplayDate = DateTime.Now.ToShortDateString(),
            Date = DateTime.Now,
            Hosts = 0,
            Hits = 0
        };
    }
}