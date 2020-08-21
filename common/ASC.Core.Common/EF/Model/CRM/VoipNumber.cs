using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("crm_voip_number")]
    public class VoipNumber
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Alias { get; set; }
        public string Settings { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }
    }
    public static class VoipNumberExtension
    {
        public static ModelBuilder MySqlAddVoipNumber(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VoipNumber>(entity =>
            {
                entity.ToTable("crm_voip_number");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Alias)
                    .HasColumnName("alias")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnName("number")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Settings)
                    .HasColumnName("settings")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });

            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddVoipNumber(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VoipNumber>(entity =>
            {
                entity.ToTable("crm_voip_number", "onlyoffice");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id_crm_voip_number");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(50);

                entity.Property(e => e.Alias)
                    .HasColumnName("alias")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnName("number")
                    .HasMaxLength(50);

                entity.Property(e => e.Settings).HasColumnName("settings");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });


            return modelBuilder;
        }
    }
}
