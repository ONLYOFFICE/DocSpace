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
        public static ModelBuilderWrapper AddVoipNumber(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddVoipNumber, Provider.MySql)
                .Add(PgSqlAddVoipNumber, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddVoipNumber(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<VoipNumber>(entity =>
            {
                _ = entity.ToTable("crm_voip_number");

                _ = entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Alias)
                    .HasColumnName("alias")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnName("number")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Settings)
                    .HasColumnName("settings")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddVoipNumber(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<VoipNumber>(entity =>
            {
                _ = entity.ToTable("crm_voip_number", "onlyoffice");

                _ = entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id_crm_voip_number");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(50);

                _ = entity.Property(e => e.Alias)
                    .HasColumnName("alias")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnName("number")
                    .HasMaxLength(50);

                _ = entity.Property(e => e.Settings).HasColumnName("settings");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
    }
}
