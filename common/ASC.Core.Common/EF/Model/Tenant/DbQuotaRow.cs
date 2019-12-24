using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("tenants_quotarow")]
    public class DbQuotaRow : BaseEntity
    {
        public int Tenant { get; set; }
        public string Path { get; set; }
        public long Counter { get; set; }
        public string Tag { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Tenant, Path };
        }
    }

    public static class DbQuotaRowExtension
    {
        public static ModelBuilder AddDbQuotaRow(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbQuotaRow>()
                .HasKey(c => new { c.Tenant, c.Path });

            return modelBuilder;
        }
    }
}
