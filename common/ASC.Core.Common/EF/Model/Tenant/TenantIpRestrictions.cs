using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_iprestrictions")]
    public class TenantIpRestrictions
    {
        public int Id { get; set; }
        public int Tenant { get; set; }
        public string Ip { get; set; }
    }
    public static class TenantIpRestrictionsExtension
    {
        public static ModelBuilderWrapper AddTenantIpRestrictions(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddTenantIpRestrictions, Provider.MySql)
                .Add(PgSqlAddTenantIpRestrictions, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTenantIpRestrictions(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<TenantIpRestrictions>(entity =>
            {
                _ = entity.ToTable("tenants_iprestrictions");

                _ = entity.HasIndex(e => e.Tenant)
                    .HasName("tenant");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("ip")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = _ = entity.Property(e => e.Tenant).HasColumnName("tenant");
            });

        }
        public static void PgSqlAddTenantIpRestrictions(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<TenantIpRestrictions>(entity =>
            {
                _ = entity.ToTable("tenants_iprestrictions", "onlyoffice");

                _ = entity.HasIndex(e => e.Tenant)
                    .HasName("tenant_tenants_iprestrictions");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("ip")
                    .HasMaxLength(50);

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");
            });

        }
    }
}
