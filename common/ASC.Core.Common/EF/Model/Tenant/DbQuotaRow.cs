using System;
using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public class DbQuotaRow : BaseEntity
    {
        public int Tenant { get; set; }
        public string Path { get; set; }
        public long Counter { get; set; }
        public string Tag { get; set; }
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
            modelBuilder
                .Add(MySqlAddDbQuotaRow, Provider.MySql)
                .Add(PgSqlAddDbQuotaRow, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbQuotaRow(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbQuotaRow>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Path })
                    .HasName("PRIMARY");

                entity.ToTable("tenants_quotarow");

                entity.HasIndex(e => e.LastModified)
                    .HasDatabaseName("last_modified");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Counter).HasColumnName("counter");

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbQuotaRow(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbQuotaRow>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Path })
                    .HasName("tenants_quotarow_pkey");

                entity.ToTable("tenants_quotarow", "onlyoffice");

                entity.HasIndex(e => e.LastModified)
                    .HasDatabaseName("last_modified_tenants_quotarow");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .HasMaxLength(255);

                entity.Property(e => e.Counter)
                    .HasColumnName("counter")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(1024)
                    .HasDefaultValueSql("'0'");
            });
        }
    }
}
