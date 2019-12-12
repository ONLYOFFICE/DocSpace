using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("tenants_buttons")]
    public class DbButton
    {
        [Column("button_url")]
        public string ButtonUrl { get; set; }

        [Column("tariff_id")]
        public int TariffId { get; set; }

        [Column("partner_id")]
        public string PartnerId { get; set; }
    }
}
