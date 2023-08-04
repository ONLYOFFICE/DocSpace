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
public class QuotaDto
{
    /// <summary>ID</summary>
    /// <type>System.Int32, System</type>
    public int Id { get; set; }

    /// <summary>Title</summary>
    /// <type>System.String, System</type>
    public string Title { get; set; }

    /// <summary>Price</summary>
    /// <type>ASC.Web.Api.ApiModels.ResponseDto.PriceDto, ASC.Web.Api</type>
    public PriceDto Price { get; set; }

    /// <summary>Specifies if the quota is nonprofit or not</summary>
    /// <type>System.Boolean, System</type>
    public bool NonProfit { get; set; }

    /// <summary>Specifies if the quota is free or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Free { get; set; }

    /// <summary>Specifies if the quota is trial or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Trial { get; set; }

    /// <summary>List of quota features</summary>
    /// <type>System.Collections.Generic.IEnumerable{ASC.Web.Api.ApiModels.ResponseDto.TenantQuotaFeatureDto}, ASC.Web.Api</type>
    public IEnumerable<TenantQuotaFeatureDto> Features { get; set; }
}

/// <summary>
/// </summary>
public class TenantQuotaFeatureDto : IEquatable<TenantQuotaFeatureDto>
{
    /// <summary>ID</summary>
    /// <type>System.String, System</type>
    public string Id { get; set; }

    /// <summary>Title</summary>
    /// <type>System.String, System</type>
    public string Title { get; set; }

    /// <summary>Image URL</summary>
    /// <type>System.String, System</type>
    public string Image { get; set; }

    /// <summary>Value</summary>
    /// <type>System.Object, System</type>
    public object Value { get; set; }

    /// <summary>Type</summary>
    /// <type>System.String, System</type>
    public string Type { get; set; }

    /// <summary>Used feature parameters</summary>
    /// <type>ASC.Web.Api.ApiModels.ResponseDto.FeatureUsedDto, ASC.Web.Api</type>
    public FeatureUsedDto Used { get; set; }

    /// <summary>Price title</summary>
    /// <type>System.String, System</type>
    public string PriceTitle { get; set; }

    public bool Equals(TenantQuotaFeatureDto other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id;
    }

    public override bool Equals(object obj) => Equals(obj as TenantQuotaFeatureDto);
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// </summary>
public class PriceDto
{
    /// <summary>Value</summary>
    /// <type>System.Nullable{System.Decimal}, System</type>
    public decimal? Value { get; set; }

    /// <summary>Currency symbol</summary>
    /// <type>System.String, System</type>
    public string CurrencySymbol { get; set; }
}

/// <summary>
/// </summary>
public class FeatureUsedDto
{
    /// <summary>Value</summary>
    /// <type>System.Object, System</type>
    public object Value { get; set; }

    /// <summary>Title</summary>
    /// <type>System.String, System</type>
    public string Title { get; set; }
}