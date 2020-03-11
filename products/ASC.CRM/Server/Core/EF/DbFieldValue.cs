using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_field_value")]
    public partial class DbFieldValue
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("value", TypeName = "text")]
        public string Value { get; set; }
        [Column("entity_id", TypeName = "int(11)")]
        public int EntityId { get; set; }
        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }
        [Column("field_id", TypeName = "int(11)")]
        public int FieldId { get; set; }
        [Column("entity_type", TypeName = "int(10)")]
        public int EntityType { get; set; }
        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }
        [Column("last_modifed_by", TypeName = "char(38)")]
        public string LastModifedBy { get; set; }
    }
}