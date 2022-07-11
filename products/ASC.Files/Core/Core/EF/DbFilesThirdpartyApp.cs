using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    public class DbFilesThirdpartyApp : BaseEntity, IDbFile
    {
        public Guid UserId { get; set; }
        public string App { get; set; }
        public string Token { get; set; }
        public int TenantId { get; set; }
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
            modelBuilder
                .Add(MySqlAddDbFilesThirdpartyApp, Provider.MySql)
                .Add(PgSqlAddDbFilesThirdpartyApp, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesThirdpartyApp(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesThirdpartyApp>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.App })
                    .HasName("PRIMARY");

                entity.ToTable("files_thirdparty_app");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.App)
                    .HasColumnName("app")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modified_on")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbFilesThirdpartyApp(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesThirdpartyApp>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.App })
                    .HasName("files_thirdparty_app_pkey");

                entity.ToTable("files_thirdparty_app", "onlyoffice");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(38);

                entity.Property(e => e.App)
                    .HasColumnName("app")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modified_on")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Token).HasColumnName("token");
            });
        }
    }
}
