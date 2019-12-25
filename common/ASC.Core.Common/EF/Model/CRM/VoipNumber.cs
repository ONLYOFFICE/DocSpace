using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("crm_voip_number")]
    public class VoipNumber
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Alias { get; set; }
        public string Settings { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }
    }
}
