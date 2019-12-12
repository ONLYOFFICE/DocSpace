using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("crm_contact")]
    public class CrmContact
    {
        public int Id { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("is_company")]
        public bool IsCompany { get; set; }
        public string Notes { get; set; }
        public string Title { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("company_name")]
        public string CompanyName { get; set; }
        public string Industry { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("contact_type_id")]
        public int ContactTypeId { get; set; }

        [Column("create_by")]
        public Guid CreateBy { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("last_modifed_by")]
        public Guid LastModifedBy { get; set; }

        [Column("last_modifed_on")]
        public DateTime LastModifedOn { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("is_shared")]
        public bool IsShared { get; set; }
        public string Currency { get; set; }
    }
}
