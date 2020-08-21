using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_forbiden")]
    public class DbTenantForbiden
    {
        [Key]
        public string Address { get; set; }
    }
    public static class DbTenantForbidenExtension
    {
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
                    .HasCollation("utf8_general_ci");
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
