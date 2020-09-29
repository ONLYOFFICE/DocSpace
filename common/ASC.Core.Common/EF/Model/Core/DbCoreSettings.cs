using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("core_settings")]
    public class DbCoreSettings : BaseEntity
    {
        public int Tenant { get; set; }
        public string Id { get; set; }
        public byte[] Value { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Tenant, Id };
        }
    }

    public static class CoreSettingsExtension
    {
        public static ModelBuilderWrapper AddCoreSettings(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddCoreSettings, Provider.MySql)
                .Add(PgSqlAddCoreSettings, Provider.Postrge);
            return modelBuilder;
        }
        public static void MySqlAddCoreSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCoreSettings>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Id })
                    .HasName("PRIMARY");

                entity.ToTable("core_settings");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("mediumblob");
            });
        }
        public static void PgSqlAddCoreSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCoreSettings>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Id })
                    .HasName("core_settings_pkey");

                entity.ToTable("core_settings", "onlyoffice");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(128);

                entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");
            });
        }
    }
}
