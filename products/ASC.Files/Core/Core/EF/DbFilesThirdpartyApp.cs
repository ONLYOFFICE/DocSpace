using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_thirdparty_app")]
    public class DbFilesThirdpartyApp : BaseEntity, IDbFile
    {
        [Column("user_id")]
        public Guid UserId { get; set; }

        public string App { get; set; }

        public string Token { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("modified_on")]
        public DateTime ModifiedOn { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { UserId, App };
        }
    }

    public static class DbFilesThirdpartyAppExtension
    {
        public static ModelBuilderWrapper AddDbDbFilesThirdpartyApp(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbFilesThirdpartyApp, Provider.MySql)
                .Add(PgSqlAddDbFilesThirdpartyApp, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesThirdpartyApp(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesThirdpartyApp>(entity =>
            {
                _ = entity.HasKey(e => new { e.UserId, e.App })
                    .HasName("PRIMARY");

                _ = entity.ToTable("files_thirdparty_app");

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.App)
                    .HasColumnName("app")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modified_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
    }
        public static void PgSqlAddDbFilesThirdpartyApp(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesThirdpartyApp>(entity =>
            {
                _ = entity.HasKey(e => new { e.UserId, e.App })
                    .HasName("files_thirdparty_app_pkey");

                _ = entity.ToTable("files_thirdparty_app", "onlyoffice");

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(38);

                _ = entity.Property(e => e.App)
                    .HasColumnName("app")
                    .HasMaxLength(50);

                _ = entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modified_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.Token).HasColumnName("token");
            });
        }
    }
}
