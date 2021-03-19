using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_projects")]
    public partial class DbProjects : IDbCrm
    {
        [Key]
        [Column("project_id", TypeName = "int(10)")]
        public int ProjectId { get; set; }

        [Key]
        [Column("contact_id", TypeName = "int(10)")]
        public int ContactId { get; set; }

        [Key]
        [Column("tenant_id", TypeName = "int(10)")]
        public int TenantId { get; set; }
    }
}