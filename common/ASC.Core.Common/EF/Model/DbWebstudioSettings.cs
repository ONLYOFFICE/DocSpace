using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("webstudio_settings")]
    public class DbWebstudioSettings : BaseEntity
    {
        public int TenantId { get; set; }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Data { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, Id, UserId };
        }
    }

    public static class WebstudioSettingsExtension
    {
        public static ModelBuilder MySqlAddWebstudioSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioSettings>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.Id, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("webstudio_settings");

                entity.HasIndex(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.TenantId).HasColumnName("TenantID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddWebstudioSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioSettings>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.Id, e.UserId })
                    .HasName("webstudio_settings_pkey");

                entity.ToTable("webstudio_settings", "onlyoffice");

                entity.HasIndex(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.TenantId).HasColumnName("TenantID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(64);

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasMaxLength(64);

                entity.Property(e => e.Data).IsRequired();
            });
            return modelBuilder;
        }
    }
}
