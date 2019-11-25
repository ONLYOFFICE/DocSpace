using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_subscriptionmethod")]
    public class SubscriptionMethod
    {
        public int Tenant { get; set; }
        public Guid Source { get; set; }
        public string Action { get; set; }
        public Guid Recipient { get; set; }
        public string Sender { get; set; }
    }
}
