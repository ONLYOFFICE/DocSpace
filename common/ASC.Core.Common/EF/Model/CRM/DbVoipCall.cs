using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("crm_voip_calls")]
    public class DbVoipCall
    {
        public string Id { get; set; }

        [Column("parent_call_id")]
        public string ParentCallId { get; set; }

        [Column("number_from")]
        public string NumberFrom { get; set; }

        [Column("number_to")]
        public string NumberTo { get; set; }
        public int Status { get; set; }

        [Column("answered_by")]
        public Guid AnsweredBy { get; set; }

        [Column("dial_date")]
        public DateTime DialDate { get; set; }

        [Column("dial_duration")]
        public int DialDuration { get; set; }

        [Column("record_sid")]
        public string RecordSid { get; set; }

        [Column("record_url")]
        public string RecordUrl { get; set; }

        [Column("record_duration")]
        public int RecordDuration { get; set; }

        [Column("record_price")]
        public decimal RecordPrice { get; set; }

        [Column("contact_id")]
        public int ContactId { get; set; }

        public decimal Price { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        public CrmContact CrmContact { get; set; }
    }
}
