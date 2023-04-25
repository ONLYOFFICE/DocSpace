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

namespace ASC.Web.Api.ApiModel.RequestsDto;

/// <summary>
/// </summary>
public class WhiteLabelRequestsDto
{
    /// <summary>Logo text</summary>
    /// <type>System.String, System</type>
    public string LogoText { get; set; }

    /// <summary>Tenant IDs with their logos (light or dark)</summary>
    /// <type>System.Collections.Generic.IEnumerable{ASC.Api.Collections.ItemKeyValuePair{System.String, ASC.Web.Api.ApiModel.RequestsDto.LogoRequestsDto}}, System.Collections.Generic</type>
    public IEnumerable<ItemKeyValuePair<string, LogoRequestsDto>> Logo { get; set; }
}

/// <summary>
/// </summary>
public class LogoRequestsDto
{
    /// <summary>Light theme logo</summary>
    /// <type>System.String, System</type>
    public string Light { get; set; }

    /// <summary>Dark theme logo</summary>
    /// <type>System.String, System</type>
    public string Dark { get; set; }
}

/// <summary>
/// </summary>
public class WhiteLabelQueryRequestsDto
{
    /// <summary>Specifies if the logo is for a dark theme or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? IsDark { get; set; }
}
