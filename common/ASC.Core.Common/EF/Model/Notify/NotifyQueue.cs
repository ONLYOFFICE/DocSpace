using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("notify_queue")]
    public class NotifyQueue
    {
        [Key]
        [Column("notify_id")]
        public int NotifyId { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public string Subject { get; set; }

        [Column("content_type")]
        public string ContentType { get; set; }
        public string Content { get; set; }

        [Column("sender_type")]
        public string SenderType { get; set; }

        [Column("reply_to")]
        public string ReplyTo { get; set; }

        [Column("creation_date")]
        public DateTime CreationDate { get; set; }
        public string Attachments { get; set; }
    }
}
