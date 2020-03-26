using ASC.CRM.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_contact")]
    public partial class DbContact : IDbCrm
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
                
        [Column("is_company")]
        public bool IsCompany { get; set; }
        
        [Column("notes", TypeName = "text")]
        public string Notes { get; set; }
        
        [Column("title", TypeName = "varchar(255)")]
        public string Title { get; set; }
        
        [Column("first_name", TypeName = "varchar(255)")]
        public string FirstName { get; set; }
        
        [Column("last_name", TypeName = "varchar(255)")]
        public string LastName { get; set; }
        
        [Column("company_name", TypeName = "varchar(255)")]
        public string CompanyName { get; set; }
        
        [Column("industry", TypeName = "varchar(255)")]
        public string Industry { get; set; }
        
        [Column("status_id", TypeName = "int(11)")]
        public int StatusId { get; set; }
        
        [Column("company_id", TypeName = "int(11)")]
        public int CompanyId { get; set; }
        
        [Column("contact_type_id", TypeName = "int(11)")]
        public int ContactTypeId { get; set; }
        
        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }
        
        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }
        
        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }
        
        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid LastModifedBy { get; set; }
        
        [Column("display_name", TypeName = "varchar(255)")]
        public string DisplayName { get; set; }

        [Column("is_shared", TypeName = "tinyint(4)")]
        public ShareType? IsShared { get; set; }

        [Column("currency", TypeName = "varchar(3)")]
        public string Currency { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

    }
}