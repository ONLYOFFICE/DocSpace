using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("core_settings")]
    public class DbCoreSettings
    {
        public int Tenant { get; set; }
        public string Id { get; set; }
        public byte[] Value { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }
    }

    public static class CoreSettingsExtension
    {
        public static void AddCoreSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCoreSettings>()
                .HasKey(c => new { c.Tenant, c.Id });
        }
    }
}
