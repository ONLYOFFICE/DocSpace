using ASC.CRM.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_task")]
    public partial class DbTask: IDbCrm
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        
        [Required]
        [Column("title", TypeName = "varchar(255)")]
        public string Title { get; set; }

        [Column("description", TypeName = "text")]
        public string Description { get; set; }

        [Column("deadline", TypeName = "datetime")]
        public DateTime Deadline { get; set; }

        [Required]
        [Column("responsible_id", TypeName = "char(38)")]
        public Guid ResponsibleId { get; set; }

        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }

        [Column("is_closed", TypeName = "int(1)")]
        public bool IsClosed { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [Column("entity_type", TypeName = "int(11)")]
        public EntityType EntityType { get; set; }

        [Column("entity_id", TypeName = "int(11)")]
        public int EntityId { get; set; }

        [Column("category_id", TypeName = "int(11)")]
        public int CategoryId { get; set; }

        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }

        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid? LastModifedBy { get; set; }

        [Column("alert_value", TypeName = "int(10)")]
        public int AlertValue { get; set; }

        [Column("exec_alert", TypeName = "int(10)")]
        public bool ExecAlert { get; set; }
    }
}