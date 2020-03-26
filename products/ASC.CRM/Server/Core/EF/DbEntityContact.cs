
using ASC.CRM.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_entity_contact")]
    public partial class DbEntityContact
    {
        [Key]
        [Column("entity_id", TypeName = "int(11)")]
        public int EntityId { get; set; }
        
        [Key]
        [Column("entity_type", TypeName = "int(11)")]
        public EntityType EntityType { get; set; }
        
        [Key]
        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }     
    }
       
}