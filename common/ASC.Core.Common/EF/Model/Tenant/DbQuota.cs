using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public static ModelBuilder MySqlAddDbQuota(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbQuota>(entity =>
            {
                entity.HasKey(e => e.Tenant)
                    .HasName("PRIMARY");

                entity.ToTable("tenants_quota");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.ActiveUsers).HasColumnName("active_users");

                entity.Property(e => e.AvangateId)
                    .HasColumnName("avangate_id")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Features)
                    .HasColumnName("features")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MaxFileSize).HasColumnName("max_file_size");

                entity.Property(e => e.MaxTotalSize).HasColumnName("max_total_size");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Price2)
                    .HasColumnName("price2")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Visible).HasColumnName("visible");
            });

            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddDbQuota(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbQuota>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tenants_quota", "onlyoffice");

                entity.Property(e => e.ActiveUsers).HasColumnName("active_users");

                entity.Property(e => e.AvangateId)
                    .HasColumnName("avangate_id")
                    .HasMaxLength(128)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("character varying");

                entity.Property(e => e.Features).HasColumnName("features");

                entity.Property(e => e.MaxFileSize)
                    .HasColumnName("max_file_size")
                    .HasDefaultValueSql("'0'::bigint");

                entity.Property(e => e.MaxTotalSize)
                    .HasColumnName("max_total_size")
                    .HasDefaultValueSql("'0'::bigint");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric(10,2)")
                    .HasDefaultValueSql("0.00");

                entity.Property(e => e.Price2)
                    .HasColumnName("price2")
                    .HasColumnType("numeric(10,2)")
                    .HasDefaultValueSql("0.00");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Visible).HasColumnName("visible");
            });

            return modelBuilder;
        }
    }
}
