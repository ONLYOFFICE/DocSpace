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
public class TenantDto : IMapFrom<Tenant>
{
    /// <summary>Affiliate ID</summary>
    /// <type>System.String, System</type>
    public string AffiliateId { get; set; }

    /// <summary>Tenant alias</summary>
    /// <type>System.String, System</type>
    public string TenantAlias { get; set; }

    /// <summary>Specifies if the calls are available for this tenant or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Calls { get; set; }

    /// <summary>Campaign</summary>
    /// <type>System.String, System</type>
    public string Campaign { get; set; }

    /// <summary>Creation date and time</summary>
    /// <type>System.DateTime, System</type>
    public DateTime CreationDateTime { get; internal set; }

    /// <summary>Hosted region</summary>
    /// <type>System.String, System</type>
    public string HostedRegion { get; set; }

    /// <summary>Tenant ID</summary>
    /// <type>System.Int32, System</type>
    public int TenantId { get; internal set; }

    /// <summary>Tenant industry</summary>
    /// <type>ASC.Core.Tenants.TenantIndustry, ASC.Core.Common</type>
    public TenantIndustry Industry { get; set; }

    /// <summary>Language</summary>
    /// <type>System.String, System</type>
    public string Language { get; set; }

    /// <summary>Last modified date</summary>
    /// <type>System.DateTime, System</type>
    public DateTime LastModified { get; set; }

    /// <summary>Mapped domain</summary>
    /// <type>System.String, System</type>
    public string MappedDomain { get; set; }

    /// <summary>Name</summary>
    /// <type>System.String, System</type>
    public string Name { get; set; }

    /// <summary>Owner ID</summary>
    /// <type>System.Guid, System</type>
    public Guid OwnerId { get; set; }

    /// <summary>Payment ID</summary>
    /// <type>System.String, System</type>
    public string PaymentId { get; set; }

    /// <summary>Specifies if the ONLYOFFICE newsletter is allowed or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Spam { get; set; }

    /// <summary>Tenant status</summary>
    /// <type>ASC.Core.Tenants;.TenantStatus, ASC.Core.Common</type>
    public TenantStatus Status { get; internal set; }

    /// <summary>The date and time when the tenant status was changed</summary>
    /// <type>System.DateTime, System</type>
    public DateTime StatusChangeDate { get; internal set; }

    /// <summary>Time zone</summary>
    /// <type>System.String, System</type>
    public string TimeZone { get; set; }

    /// <summary>List of trusted domains</summary>
    /// <type>System.Collections.Generic.List{System.String}, System.Collections.Generic</type>
    public List<string> TrustedDomains { get; set; }

    /// <summary>Trusted domains in the string format</summary>
    /// <type>System.String, System</type>
    public string TrustedDomainsRaw { get; set; }

    /// <summary>Trusted domains type</summary>
    /// <type>ASC.Core.Tenants.TenantTrustedDomainsType, ASC.Core.Common</type>
    public TenantTrustedDomainsType TrustedDomainsType { get; set; }

    /// <summary>Version</summary>
    /// <type>System.Int32, System</type>
    public int Version { get; set; }

    /// <summary>The date and time when the tenant version was changed</summary>
    /// <type>System.DateTime, System</type>
    public DateTime VersionChanged { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Tenant, TenantDto>()
            .ForMember(r => r.TenantId, opt => opt.MapFrom(src => src.Id))
            .ForMember(r => r.TenantAlias, opt => opt.MapFrom(src => src.Alias));
    }
}
