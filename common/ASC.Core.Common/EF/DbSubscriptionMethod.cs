using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_subscriptionmethod")]
    public class DbSubscriptionMethod
    {
        public int Tenant { get; set; }
        public string Source { get; set; }
        public string Action { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
    }
}
