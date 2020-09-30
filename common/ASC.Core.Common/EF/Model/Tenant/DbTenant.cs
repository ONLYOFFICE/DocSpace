using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Tenants;

using Microsoft.EntityFrameworkCore;

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
        public DateTime? Version_Changed { get; set; }

        [NotMapped]
        public DateTime VersionChanged { get => Version_Changed ?? DateTime.MinValue; set => Version_Changed = value; }

        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string TrustedDomains { get; set; }
        public TenantTrustedDomainsType TrustedDomainsEnabled { get; set; }
        public TenantStatus Status { get; set; }

        public DateTime? StatusChanged { get; set; }

        //hack for DateTime?
        [NotMapped]
        public DateTime? StatusChangedHack { get { return StatusChanged; } set { StatusChanged = value; } }

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

    public static class DbTenantExtension
    {
        public static void AddDbTenant(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbTenant>()
                .HasOne(r => r.Partner)
                .WithOne(r => r.Tenant)
                .HasForeignKey<DbTenantPartner>(r => new { r.TenantId })
                .HasPrincipalKey<DbTenant>(r => new { r.Id });
        }
    }
}
