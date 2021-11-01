using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Nest;

namespace ASC.CRM.Core.EF
{
    [Table("crm_invoice")]
    public class DbInvoice : IDbCrm, ISearchItem
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }

        [Column("status", TypeName = "int(11)")]
        public InvoiceStatus Status { get; set; }

        [Required]
        [Column("number", TypeName = "varchar(255)")]
        public string Number { get; set; }

        [Column("issue_date", TypeName = "datetime")]
        public DateTime IssueDate { get; set; }

        [Column("template_type", TypeName = "int(11)")]
        public InvoiceTemplateType TemplateType { get; set; }

        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }

        [Column("consignee_id", TypeName = "int(11)")]
        public int ConsigneeId { get; set; }

        [Column("entity_type", TypeName = "int(11)")]
        public EntityType EntityType { get; set; }

        [Column("entity_id", TypeName = "int(11)")]
        public int EntityId { get; set; }

        [Column("due_date", TypeName = "datetime")]
        public DateTime DueDate { get; set; }

        [Required]
        [Column("language", TypeName = "varchar(255)")]
        public string Language { get; set; }

        [Required]
        [Column("currency", TypeName = "varchar(255)")]
        public string Currency { get; set; }

        [Column("exchange_rate", TypeName = "decimal(10,2)")]
        public decimal ExchangeRate { get; set; }

        [Required]
        [Column("purchase_order_number", TypeName = "varchar(255)")]
        public string PurchaseOrderNumber { get; set; }

        [Column("terms", TypeName = "text")]
        public string Terms { get; set; }

        [Column("description", TypeName = "text")]
        public string Description { get; set; }

        [Column("json_data", TypeName = "text")]
        public string JsonData { get; set; }

        [Column("file_id", TypeName = "int(11)")]
        public int FileId { get; set; }

        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }

        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid LastModifedBy { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get
            {
                return "crm_deal";
            }
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Description };
        }
    }
}