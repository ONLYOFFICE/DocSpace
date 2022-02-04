/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

using ASC.Common.Mapping;
using ASC.Core.Common.EF.Model;

using AutoMapper;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class Tenant : IMapFrom<DbTenant>
    {
        public const int DefaultTenant = -1;

        public int TenantId { get; internal set; }
        public string TenantAlias { get; set; }
        public string MappedDomain { get; set; }
        public int Version { get; set; }
        public DateTime VersionChanged { get; set; }
        public string HostedRegion { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public List<string> TrustedDomains { get; set; }
        public TenantTrustedDomainsType TrustedDomainsType { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedDateTime { get; internal set; }
        public DateTime LastModified { get; set; }
        public TenantStatus Status { get; internal set; }
        public DateTime StatusChangeDate { get; internal set; }
        public string PartnerId { get; set; }
        public string AffiliateId { get; set; }
        public string Campaign { get; set; }
        public string PaymentId { get; set; }
        public TenantIndustry Industry { get; set; }
        public bool Spam { get; set; }
        public bool Calls { get; set; }

        public static readonly string HostName = Dns.GetHostName().ToLowerInvariant();

        public Tenant()
        {
            TenantId = DefaultTenant;
            TimeZone = TimeZoneInfo.Utc.Id;
            Language = CultureInfo.CurrentCulture.Name;
            TrustedDomains = new List<string>();
            TrustedDomainsType = TenantTrustedDomainsType.None;
            CreatedDateTime = DateTime.UtcNow;
            Status = TenantStatus.Active;
            StatusChangeDate = DateTime.UtcNow;
            VersionChanged = DateTime.UtcNow;
            Industry = TenantIndustry.Other;
        }

        public Tenant(string alias)
            : this() => TenantAlias = alias.ToLowerInvariant();

        public Tenant(int id, string alias)
            : this(alias) => TenantId = id;

        public CultureInfo GetCulture() =>
            !string.IsNullOrEmpty(Language) 
            ? CultureInfo.GetCultureInfo(Language.Trim()) : CultureInfo.CurrentCulture;

        public void SetStatus(TenantStatus status)
        {
            Status = status;
            StatusChangeDate = DateTime.UtcNow;
        }

        public override bool Equals(object obj) =>
            obj is Tenant t && t.TenantId == TenantId;

        public override int GetHashCode() => TenantId;

        public override string ToString() => TenantAlias;

        public string GetTenantDomain(CoreSettings coreSettings, bool allowMappedDomain = true)
        {
            var baseHost = coreSettings.GetBaseDomain(HostedRegion);

            if (string.IsNullOrEmpty(baseHost) && !string.IsNullOrEmpty(HostedRegion))
                baseHost = HostedRegion;

            string result;
            if (baseHost == "localhost" || TenantAlias == "localhost")
            {
                //single tenant on local host
                TenantAlias = "localhost";
                result = HostName;
            }
            else result = string.Format("{0}.{1}", TenantAlias, baseHost).TrimEnd('.').ToLowerInvariant();

            if (!string.IsNullOrEmpty(MappedDomain) && allowMappedDomain)
            {
                if (MappedDomain.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
                    MappedDomain = MappedDomain.Substring(7);

                if (MappedDomain.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                    MappedDomain = MappedDomain.Substring(8);

                result = MappedDomain.ToLowerInvariant();
            }

            return result;
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DbTenant, Tenant>()
                .ForMember(dest => dest.TrustedDomains, opt => opt
                    .MapFrom(src => src.TrustedDomains != null
                    ? src.TrustedDomains.Split(new[] { '|' },
                        StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>()))
                .ForMember(dest => dest.Industry, opt => opt.NullSubstitute(TenantIndustry.Other));
        }

        internal string GetTrustedDomains()
        {
            TrustedDomains.RemoveAll(d => string.IsNullOrEmpty(d));
            if (TrustedDomains.Count == 0) return null;

            return string.Join("|", TrustedDomains.ToArray());
        }

        internal void SetTrustedDomains(string trustedDomains)
        {
            if (string.IsNullOrEmpty(trustedDomains)) TrustedDomains.Clear();
            else TrustedDomains.AddRange(trustedDomains.Split(new[] { '|' },
                StringSplitOptions.RemoveEmptyEntries));
        }
    }
}