using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("tenants_quotarow")]
    public class DbQuotaRow : BaseEntity
    {
        public int Tenant { get; set; }
        public string Path { get; set; }
        public long Counter { get; set; }
        public string Tag { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Tenant, Path };
        }
    }

    public static class DbQuotaRowExtension
    {
        public static ModelBuilderWrapper AddDbQuotaRow(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbQuotaRow, Provider.MySql)
                .Add(PgSqlAddDbQuotaRow, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbQuotaRow(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbQuotaRow>(entity =>
            {
                _ = entity.HasKey(e => new { e.Tenant, e.Path })
                    .HasName("PRIMARY");

                _ = entity.ToTable("tenants_quotarow");

                _ = entity.HasIndex(e => e.LastModified)
                    .HasName("last_modified");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");

                _ = entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Counter).HasColumnName("counter");

                _ = entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbQuotaRow(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbQuotaRow>(entity =>
            {
                _ = entity.HasKey(e => new { e.Tenant, e.Path })
                    .HasName("tenants_quotarow_pkey");

                _ = entity.ToTable("tenants_quotarow", "onlyoffice");

                _ = entity.HasIndex(e => e.LastModified)
                    .HasName("last_modified_tenants_quotarow");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");

                _ = entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .HasMaxLength(255);

                _ = entity.Property(e => e.Counter)
                    .HasColumnName("counter")
                    .HasDefaultValueSql("'0'");

                _ = entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(1024)
                    .HasDefaultValueSql("'0'");
            });
        }
    }
}
