using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;

using Nest;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ASC.CRM.Core.EF
{
    [Table("crm_contact_info")]
    public partial class DbContactInfo : IDbCrm, ISearchItem
    {
        public int Id { get; set; }

        [Required]
        [Column("data", TypeName = "text")]
        [Text(Analyzer = "whitespacecustom")]
        public string Data { get; set; }
        
        [Column("category", TypeName = "int(255)")]
        public int Category { get; set; }
        
        [Column("tenant_id", TypeName = "int(255)")]
        public int TenantId { get; set; }
        
        [Column("is_primary", TypeName = "tinyint(4)")]
        public bool IsPrimary { get; set; }
        
        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }
        
        [Column("type", TypeName = "int(255)")]
        public ContactInfoType Type { get; set; }
        
        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }
        
        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid LastModifedBy { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get
            {
                return "crm_field_value";
            }
        }

        [Ignore]
        public Expression<Func<ISearchItem, object[]>> SearchContentFields
        {
            get
            {
                return (a) => new[] { Value };
            }
        }


    }
}