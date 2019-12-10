using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_iprestrictions")]
    public class TenantIpRestrictions
    {
        public int Id { get; set; }
        public int Tenant { get; set; }
        public string Ip { get; set; }
    }
}
