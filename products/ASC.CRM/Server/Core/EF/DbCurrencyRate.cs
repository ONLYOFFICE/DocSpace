using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.CRM.Core.EF
{
    [Table("crm_currency_rate")]
    public partial class DbCurrencyRate : IDbCrm
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }

        [Required]
        [Column("from_currency", TypeName = "varchar(255)")]
        public string FromCurrency { get; set; }

        [Required]
        [Column("to_currency", TypeName = "varchar(255)")]
        public string ToCurrency { get; set; }

        [Column("rate", TypeName = "decimal(10,2)")]
        public decimal Rate { get; set; }
        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }
        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }
        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }
        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid? LastModifedBy { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }
    }
}