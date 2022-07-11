using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class DbTenantForbiden
    {
        public string Address { get; set; }
    }
    public static class DbTenantForbidenExtension
    {
        public static ModelBuilderWrapper AddDbTenantForbiden(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbTenantForbiden, Provider.MySql)
                .Add(PgSqlAddDbTenantForbiden, Provider.PostgreSql)
                .HasData(
                new DbTenantForbiden { Address = "controlpanel" },
                new DbTenantForbiden { Address = "localhost" }
                );
            return modelBuilder;
        }

        public static void MySqlAddDbTenantForbiden(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTenantForbiden>(entity =>
            {
                entity.HasKey(e => e.Address)
                    .HasName("PRIMARY");

                entity.ToTable("tenants_forbiden");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbTenantForbiden(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTenantForbiden>(entity =>
            {
                entity.HasKey(e => e.Address)
                    .HasName("tenants_forbiden_pkey");

                entity.ToTable("tenants_forbiden", "onlyoffice");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(50);
            });

        }
    }
}
