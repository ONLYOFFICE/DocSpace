using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_partners")]
    public class DbTenantPartner
    {
        [Key]
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("partner_id")]
        public string PartnerId { get; set; }

        [Column("affiliate_id")]
        public string AffiliateId { get; set; }

        [Column("campaign")]
        public string Campaign { get; set; }

        public DbTenant Tenant { get; set; }
    }
}
