using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("mobile_app_install")]
    public class MobileAppInstall
    {
        [Column("user_email")]
        public string UserEmail { get; set; }

        [Column("app_type")]
        public int AppType { get; set; }

        [Column("registered_on")]
        public DateTime RegisteredOn { get; set; }

        [Column("last_sign")]
        public DateTime LastSign { get; set; }
    }

    public static class MobileAppInstallExtension
    {
        public static ModelBuilderWrapper AddMobileAppInstall(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddMobileAppInstall, Provider.MySql)
                .Add(PgSqlAddMobileAppInstall, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddMobileAppInstall(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<MobileAppInstall>(entity =>
            {
                _ = entity.HasKey(e => new { e.UserEmail, e.AppType })
                    .HasName("PRIMARY");

                _ = entity.ToTable("mobile_app_install");

                _ = entity.Property(e => e.UserEmail)
                    .HasColumnName("user_email")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.AppType).HasColumnName("app_type");

                _ = entity.Property(e => e.LastSign)
                    .HasColumnName("last_sign")
                    .HasColumnType("datetime");

                _ = entity.Property(e => e.RegisteredOn)
                    .HasColumnName("registered_on")
                    .HasColumnType("datetime");
            });
    }
        public static void PgSqlAddMobileAppInstall(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<MobileAppInstall>(entity =>
            {
                _ = entity.HasKey(e => new { e.UserEmail, e.AppType })
                    .HasName("mobile_app_install_pkey");

                _ = entity.ToTable("mobile_app_install", "onlyoffice");

                _ = entity.Property(e => e.UserEmail)
                    .HasColumnName("user_email")
                    .HasMaxLength(255);

                _ = entity.Property(e => e.AppType).HasColumnName("app_type");

                _ = entity.Property(e => e.LastSign).HasColumnName("last_sign");

                _ = entity.Property(e => e.RegisteredOn).HasColumnName("registered_on");
            });
        }
    }
}
