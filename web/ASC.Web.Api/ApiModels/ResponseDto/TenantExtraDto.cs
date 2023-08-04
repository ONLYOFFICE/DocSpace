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

namespace ASC.Web.Api.ApiModels.ResponseDto;

/// <summary>
/// </summary>
public class TenantExtraDto
{
    /// <summary>Specifies if the extra tenant license is customizable or not</summary>
    /// <type>System.Boolean, System</type>
    public bool CustomMode { get; set; }

    /// <summary>Specifies if the extra tenant license is Opensource or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Opensource { get; set; }

    /// <summary>Specifies if the extra tenant license is Enterprise or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Enterprise { get; set; }

    /// <summary>License tariff</summary>
    /// <type>ASC.Core.Billing.Tariff, ASC.Core.Common</type>
    public Tariff Tariff { get; set; }

    /// <summary>License quota</summary>
    /// <type>ASC.Web.Api.ApiModels.ResponseDto.QuotaDto, ASC.Web.Api</type>
    public QuotaDto Quota { get; set; }

    /// <summary>Specifies if the license is paid or not</summary>
    /// <type>System.Boolean, System</type>
    public bool NotPaid { get; set; }

    /// <summary>The time when the license was accepted</summary>
    /// <type>System.String, System</type>
    public string LicenseAccept { get; set; }

    /// <summary>Specifies if the tariff page is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool EnableTariffPage { get; set; }

    /// <summary>Document server user quotas</summary>
    /// <type>System.Collections.Generic.Dictionary{System.String, System.DateTime}, System.Collections.Generic</type>
    public Dictionary<string, DateTime> DocServerUserQuota { get; set; }

    /// <summary>Document server license</summary>
    /// <type>ASC.Core.Billing.License, ASC.Core.Common</type>
    public License DocServerLicense { get; set; }
}
