using System;

using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public class DbTariff
    {
        public int Id { get; set; }
        public int Tenant { get; set; }
        public int Tariff { get; set; }
        public DateTime Stamp { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
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
                    .HasDatabaseName("tenant");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("int");

                entity.Property(e => e.Stamp)
                    .HasColumnName("stamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tariff).HasColumnName("tariff");

                entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
        public static void PgSqlAddDbTariff(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTariff>(entity =>
            {
                entity.ToTable("tenants_tariff", "onlyoffice");

                entity.HasIndex(e => e.Tenant)
                    .HasDatabaseName("tenant_tenants_tariff");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Stamp).HasColumnName("stamp");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Tariff).HasColumnName("tariff");

                entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
    }
}
