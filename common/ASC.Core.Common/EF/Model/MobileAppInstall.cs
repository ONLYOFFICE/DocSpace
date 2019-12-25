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
        public static ModelBuilder AddMobileAppInstall(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MobileAppInstall>()
                .HasKey(c => new { c.UserEmail, c.AppType });

            return modelBuilder;
        }
    }
}
