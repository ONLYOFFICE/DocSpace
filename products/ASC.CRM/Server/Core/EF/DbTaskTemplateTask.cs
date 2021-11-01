using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_task_template_task")]
    public partial class DbTaskTemplateTask : IDbCrm
    {
        [Key]
        [Column("task_id", TypeName = "int(10)")]
        public int TaskId { get; set; }

        [Key]
        [Column("task_template_id", TypeName = "int(10)")]
        public int TaskTemplateId { get; set; }

        [Key]
        [Column("tenant_id", TypeName = "int(10)")]
        public int TenantId { get; set; }
    }
}