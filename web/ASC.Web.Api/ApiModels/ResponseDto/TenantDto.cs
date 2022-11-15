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

public class TenantDto : IMapFrom<Tenant>
{
    public string AffiliateId { get; set; }
    public string TenantAlias { get; set; }
    public bool Calls { get; set; }
    public string Campaign { get; set; }
    public DateTime CreationDateTime { get; internal set; }
    public string HostedRegion { get; set; }
    public int TenantId { get; internal set; }
    public TenantIndustry Industry { get; set; }
    public string Language { get; set; }
    public DateTime LastModified { get; set; }
    public string MappedDomain { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public string PaymentId { get; set; }
    public bool Spam { get; set; }
    public TenantStatus Status { get; internal set; }
    public DateTime StatusChangeDate { get; internal set; }
    public string TimeZone { get; set; }
    public List<string> TrustedDomains { get; set; }
    public string TrustedDomainsRaw { get; set; }
    public TenantTrustedDomainsType TrustedDomainsType { get; set; }
    public int Version { get; set; }
    public DateTime VersionChanged { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Tenant, TenantDto>()
            .ForMember(r => r.TenantId, opt => opt.MapFrom(src => src.Id))
            .ForMember(r => r.TenantAlias, opt => opt.MapFrom(src => src.Alias));
    }
}
