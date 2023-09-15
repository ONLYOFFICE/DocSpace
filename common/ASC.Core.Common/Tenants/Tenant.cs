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

using Profile = AutoMapper.Profile;

namespace ASC.Core.Tenants;

public class Tenant : IMapFrom<DbTenant>
{
    public const int DefaultTenant = -1;

    public static readonly string HostName = Dns.GetHostName().ToLowerInvariant();
    public const string LocalHost = "localhost";

    private List<string> _domains;

    public Tenant()
    {
        Id = DefaultTenant;
        TimeZone = TimeZoneInfo.Utc.Id;
        Language = CultureInfo.CurrentCulture.Name;
        TrustedDomains = new List<string>();
        TrustedDomainsType = TenantTrustedDomainsType.None;
        CreationDateTime = DateTime.UtcNow;
        Status = TenantStatus.Active;
        StatusChangeDate = DateTime.UtcNow;
        VersionChanged = DateTime.UtcNow;
        Industry = TenantIndustry.Other;
    }

    public Tenant(string alias)
        : this()
    {
        Alias = alias.ToLowerInvariant();
    }

    public Tenant(int id, string alias)
        : this(alias)
    {
        Id = id;
    }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbTenant, Tenant>()
            .ForMember(r => r.TrustedDomainsType, opt => opt.MapFrom(src => src.TrustedDomainsEnabled))
            .ForMember(r => r.AffiliateId, opt => opt.MapFrom(src => src.Partner.AffiliateId))
            .ForMember(r => r.PartnerId, opt => opt.MapFrom(src => src.Partner.PartnerId))
            .ForMember(r => r.Campaign, opt => opt.MapFrom(src => src.Partner.Campaign));

        profile.CreateMap<TenantUserSecurity, Tenant>()
            .IncludeMembers(src => src.DbTenant);
    }

    public string AffiliateId { get; set; }
    public string Alias { get; set; }
    public bool Calls { get; set; }
    public string Campaign { get; set; }
    public DateTime CreationDateTime { get; internal set; }
    public string HostedRegion { get; set; }
    public int Id { get; internal set; }
    public TenantIndustry Industry { get; set; }
    public string Language { get; set; }
    public DateTime LastModified { get; set; }
    public string MappedDomain { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public string PartnerId { get; set; }
    public string PaymentId { get; set; }
    public bool Spam { get; set; }
    public TenantStatus Status { get; internal set; }
    public DateTime StatusChangeDate { get; internal set; }
    public string TimeZone { get; set; }
    public List<string> TrustedDomains
    {
        get
        {
            if (_domains.Count == 0 && !string.IsNullOrEmpty(TrustedDomainsRaw))
            {
                _domains = TrustedDomainsRaw.Split(new[] { '|' },
                    StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return _domains;
        }
        set => _domains = value;
    }

    public string TrustedDomainsRaw { get; set; }
    public TenantTrustedDomainsType TrustedDomainsType { get; set; }
    public int Version { get; set; }
    public DateTime VersionChanged { get; set; }
    public override bool Equals(object obj)
    {
        return obj is Tenant t && t.Id == Id;
    }

    public CultureInfo GetCulture() => !string.IsNullOrEmpty(Language) ? CultureInfo.GetCultureInfo(Language.Trim()) : CultureInfo.CurrentCulture;
    public override int GetHashCode()
    {
        return Id;
    }

    public string GetTenantDomain(CoreSettings coreSettings, bool allowMappedDomain = true)
    {
        var baseHost = coreSettings.GetBaseDomain(HostedRegion);

        if (string.IsNullOrEmpty(baseHost) && !string.IsNullOrEmpty(HostedRegion))
        {
            baseHost = HostedRegion;
        }

        string result;
        if (baseHost == "localhost" || Alias == "localhost")
        {
            //single tenant on local host
            Alias = "localhost";
            result = Alias;
        }
        else
        {
            result = $"{Alias}.{baseHost}".TrimEnd('.').ToLowerInvariant();
        }

        if (!string.IsNullOrEmpty(MappedDomain) && allowMappedDomain)
        {
            if (MappedDomain.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                MappedDomain = MappedDomain.Substring(7);
            }
            if (MappedDomain.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                MappedDomain = MappedDomain.Substring(8);
            }
            result = MappedDomain.ToLowerInvariant();
        }

        return result;
    }

    public void SetStatus(TenantStatus status)
    {
        Status = status;
        StatusChangeDate = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return Alias;
    }

    internal string GetTrustedDomains()
    {
        TrustedDomains.RemoveAll(d => string.IsNullOrEmpty(d));
        if (TrustedDomains.Count == 0)
        {
            return null;
        }

        return string.Join("|", TrustedDomains.ToArray());
    }

    internal void SetTrustedDomains(string trustedDomains)
    {
        if (string.IsNullOrEmpty(trustedDomains))
        {
            TrustedDomains.Clear();
        }
        else
        {
            TrustedDomains.AddRange(trustedDomains.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
