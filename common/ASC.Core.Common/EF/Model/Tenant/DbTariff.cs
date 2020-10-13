using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("tenants_tariff")]
    public class DbTariff
    {
        public int Id { get; set; }
        public int Tenant { get; set; }
        public int Tariff { get; set; }
        public DateTime Stamp { get; set; }

        [Column("tariff_key")]
        public string TariffKey { get; set; }

        public string Comment { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }
    }
    public static class DbTariffExtension
    {
        public static ModelBuilderWrapper AddDbTariff(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbTariff, Provider.MySql)
                .Add(PgSqlAddDbTariff, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbTariff(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTariff>(entity =>
            {
                entity.ToTable("tenants_tariff");

                entity.HasIndex(e => e.Tenant)
                    .HasName("tenant");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Stamp)
                    .HasColumnName("stamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tariff).HasColumnName("tariff");

                entity.Property(e => e.TariffKey)
                    .HasColumnName("tariff_key")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
        public static void PgSqlAddDbTariff(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTariff>(entity =>
            {
                entity.ToTable("tenants_tariff", "onlyoffice");

                entity.HasIndex(e => e.Tenant)
                    .HasName("tenant_tenants_tariff");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Stamp).HasColumnName("stamp");

                entity.Property(e => e.Tariff).HasColumnName("tariff");

                entity.Property(e => e.TariffKey)
                    .HasColumnName("tariff_key")
                    .HasMaxLength(64)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
    }
}
