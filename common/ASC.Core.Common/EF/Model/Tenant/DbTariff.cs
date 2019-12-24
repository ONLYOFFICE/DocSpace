using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("tenants_tariff")]
    public class DbTariff
    {
        public int Id { get; set; }
        public int Tenant { get; set; }
        public int Tariff { get; set; }
        public DateTime Stamp { get; set; }

        [Column("tariff_key")]
        public string TariffKey { get; set; }

        public string Comment { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }
    }
}
