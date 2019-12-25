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

        internal override object[] GetKeys()
        {
            return new object[] { TenantId, Id, UserId };
        }
    }

    public static class WebstudioSettingsExtension
    {
        public static ModelBuilder AddWebstudioSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioSettings>()
                .HasKey(c => new { c.TenantId, c.Id, c.UserId });

            return modelBuilder;
        }
    }
}
