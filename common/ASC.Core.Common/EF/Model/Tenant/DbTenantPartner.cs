using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_partners")]
    public class DbTenantPartner
    {
        [Key]
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("partner_id")]
        public string PartnerId { get; set; }

        [Column("affiliate_id")]
        public string AffiliateId { get; set; }

        [Column("campaign")]
        public string Campaign { get; set; }

        public DbTenant Tenant { get; set; }
    }
    public static class DbTenantPartnerExtension
    {
        public static ModelBuilderWrapper AddDbTenantPartner(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbTenantPartner, Provider.MySql)
                .Add(PgSqlAddDbTenantPartner, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbTenantPartner(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbTenantPartner>(entity =>
            {
                _ = entity.HasKey(e => e.TenantId)
                    .HasName("PRIMARY");

                _ = entity.ToTable("tenants_partners");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.AffiliateId)
                    .HasColumnName("affiliate_id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Campaign)
                    .HasColumnName("campaign")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.PartnerId)
                    .HasColumnName("partner_id")
                    .HasColumnType("varchar(36)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

        }
        public static void PgSqlAddDbTenantPartner(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbTenantPartner>(entity =>
            {
                _ = entity.HasKey(e => e.TenantId)
                    .HasName("tenants_partners_pkey");

                _ = entity.ToTable("tenants_partners", "onlyoffice");

                _ = entity.Property(e => e.TenantId)
                    .HasColumnName("tenant_id")
                    .ValueGeneratedNever();

                _ = entity.Property(e => e.AffiliateId)
                    .HasColumnName("affiliate_id")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Campaign)
                    .HasColumnName("campaign")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.PartnerId)
                    .HasColumnName("partner_id")
                    .HasMaxLength(36)
                    .HasDefaultValueSql("NULL");
            });

        }
    }
}
