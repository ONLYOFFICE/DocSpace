using System;
using System.ComponentModel.DataAnnotations.Schema;
using ASC.Core.Tenants;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_tenants")]
    public class DbTenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string MappedDomain { get; set; }
        public int Version { get; set; }

        [Column("version_changed")]
        public DateTime VersionChanged { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string TrustedDomains { get; set; }
        public TenantTrustedDomainsType TrustedDomainsEnabled { get; set; }
        public TenantStatus Status { get; set; }
        public DateTime StatusChanged { get; set; }
        public DateTime CreationDateTime { get; set; }

        [Column("owner_id")]
        public Guid OwnerId { get; set; }
        public bool Public { get; set; }
        public string PublicVisibleProducts { get; set; }

        [Column("payment_id")]
        public string PaymentId { get; set; }
        public TenantIndustry? Industry { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }
        public bool Spam { get; set; }
        public bool Calls { get; set; }

        public DbTenantPartner Partner { get; set; }
    }
}
