using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.CRM.Core.Enums;

namespace ASC.CRM.Core.EF
{
    [Table("crm_tag")]
    public partial class DbTag : IDbCrm
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }

        [Required]
        [Column("title", TypeName = "varchar(255)")]
        public string Title { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [Column("entity_type", TypeName = "int(11)")]
        public EntityType EntityType { get; set; }
    }
}