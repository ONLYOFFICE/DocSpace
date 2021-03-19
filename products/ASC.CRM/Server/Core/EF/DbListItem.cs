using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.CRM.Core.Enums;

namespace ASC.CRM.Core.EF
{
    [Table("crm_list_item")]
    public partial class DbListItem : IDbCrm
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }

        [Required]
        [Column("title", TypeName = "varchar(255)")]
        public string Title { get; set; }

        [Column("sort_order", TypeName = "int(11)")]
        public int SortOrder { get; set; }

        [Column("color", TypeName = "varchar(255)")]
        public string Color { get; set; }

        [Column("additional_params", TypeName = "varchar(255)")]
        public string AdditionalParams { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [Column("list_type", TypeName = "int(255)")]
        public ListType ListType { get; set; }

        [Column("description", TypeName = "varchar(255)")]
        public string Description { get; set; }
    }
}