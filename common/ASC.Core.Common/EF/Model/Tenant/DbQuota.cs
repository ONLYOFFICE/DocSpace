using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("tenants_quota")]
    public class DbQuota : BaseEntity
    {
        [Key]
        public int Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Column("max_file_size")]
        public long MaxFileSize { get; set; }

        [Column("max_total_size")]
        public long MaxTotalSize { get; set; }

        [Column("active_users")]
        public int ActiveUsers { get; set; }
        public string Features { get; set; }
        public decimal Price { get; set; }
        public decimal Price2 { get; set; }

        [Column("avangate_id")]
        public string AvangateId { get; set; }

        public bool Visible { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Tenant };
        }
    }
    public static class DbQuotaExtension
    {
        public static ModelBuilderWrapper AddDbQuota(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbQuota, Provider.MySql)
                .Add(PgSqlAddDbQuota, Provider.Postgre)
                .HasData(
                    new DbQuota { Tenant = -1, Name = "default", Description = null, MaxFileSize = 102400, MaxTotalSize = 10995116277760, ActiveUsers = 10000, Features = "docs,domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption", Price = decimal.Parse("0,00"), Price2 = decimal.Parse("0,00"), AvangateId = "0", Visible = false }
                );

            return modelBuilder;
        }

        public static void MySqlAddDbQuota(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbQuota>(entity =>
            {
                _ = entity.HasKey(e => e.Tenant)
                    .HasName("PRIMARY");

                _ = entity.ToTable("tenants_quota");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");

                _ = entity.Property(e => e.ActiveUsers).HasColumnName("active_users");

                _ = entity.Property(e => e.AvangateId)
                    .HasColumnName("avangate_id")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Features)
                    .HasColumnName("features")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.MaxFileSize).HasColumnName("max_file_size");

                _ = entity.Property(e => e.MaxTotalSize).HasColumnName("max_total_size");

                _ = entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2)");

                _ = entity.Property(e => e.Price2)
                    .HasColumnName("price2")
                    .HasColumnType("decimal(10,2)");

                _ = entity.Property(e => e.Visible).HasColumnName("visible");
            });
        }
        public static void PgSqlAddDbQuota(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbQuota>(entity =>
            {
                _ = entity.HasKey(e => e.Tenant)
                    .HasName("tenants_quota_pkey");

                _ = entity.ToTable("tenants_quota", "onlyoffice");

                _ = entity.Property(e => e.Tenant)
                    .HasColumnName("tenant")
                    .ValueGeneratedNever();

                _ = entity.Property(e => e.ActiveUsers).HasColumnName("active_users");

                _ = entity.Property(e => e.AvangateId)
                    .HasColumnName("avangate_id")
                    .HasMaxLength(128)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("character varying");

                _ = entity.Property(e => e.Features).HasColumnName("features");

                _ = entity.Property(e => e.MaxFileSize)
                    .HasColumnName("max_file_size")
                    .HasDefaultValueSql("'0'");

                _ = entity.Property(e => e.MaxTotalSize)
                    .HasColumnName("max_total_size")
                    .HasDefaultValueSql("'0'");

                _ = entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                _ = entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric(10,2)")
                    .HasDefaultValueSql("0.00");

                _ = entity.Property(e => e.Price2)
                    .HasColumnName("price2")
                    .HasColumnType("numeric(10,2)")
                    .HasDefaultValueSql("0.00");

                _ = entity.Property(e => e.Visible).HasColumnName("visible");
            });
        }
    }
}
