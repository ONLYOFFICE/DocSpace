using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("tenants_buttons")]
    public class DbButton : BaseEntity
    {
        [Column("button_url")]
        public string ButtonUrl { get; set; }

        [Column("tariff_id")]
        public int TariffId { get; set; }

        [Column("partner_id")]
        public string PartnerId { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { TariffId, PartnerId };
        }
    }

    public static class DbButtonExtension
    {
        public static ModelBuilder AddDbButton(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbButton>()
                .HasKey(c => new { c.TariffId, c.PartnerId });

            return modelBuilder;
        }
    }
}
