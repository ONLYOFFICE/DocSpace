using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_subscription")]
    public class Subscription
    {
        public int Tenant { get; set; }
        public Guid Source { get; set; }
        public string Action { get; set; }
        public Guid Recipient { get; set; }
        public string Object { get; set; }
        public bool Unsubscribed { get; set; }
    }
}
