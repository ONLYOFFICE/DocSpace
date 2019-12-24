using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("core_subscription")]
    public class Subscription : BaseEntity
    {
        public int Tenant { get; set; }
        public string Source { get; set; }
        public string Action { get; set; }
        public string Recipient { get; set; }
        public string Object { get; set; }
        public bool Unsubscribed { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Tenant, Source, Action, Recipient, Object };
        }
    }

    public static class SubscriptionExtension
    {
        public static ModelBuilder AddSubscription(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .HasKey(c => new { c.Tenant, c.Source, c.Action, c.Recipient, c.Object });

            return modelBuilder;
        }
    }
}
