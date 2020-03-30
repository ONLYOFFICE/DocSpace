using ASC.CRM.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_relationship_event")]
    public partial class DbRelationshipEvent : IDbCrm
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }

        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }
        
        [Column("content", TypeName = "text")]
        public string Content { get; set; }
        
        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }
        
        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }
        
        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }
        
        [Column("entity_type", TypeName = "int(11)")]
        public EntityType EntityType { get; set; }
        
        [Column("entity_id", TypeName = "int(11)")]
        public int EntityId { get; set; }

        [Column("category_id", TypeName = "int(11)")]
        public int CategoryId { get; set; }
        
        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid? LastModifedBy { get; set; }
        
        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }
        
        [Column("have_files", TypeName = "int(11)")]
        public bool HaveFiles { get; set; }
    }
}