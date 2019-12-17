using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("core_subscriptionmethod")]
    public class DbSubscriptionMethod : BaseEntity
    {
        public int Tenant { get; set; }
        public string Source { get; set; }
        public string Action { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Tenant, Source, Action, Recipient };
        }
    }

    public static class SubscriptionMethodExtension
    {
        public static ModelBuilder AddSubscriptionMethod(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbSubscriptionMethod>()
                .HasKey(c => new { c.Tenant, c.Source, c.Action, c.Recipient });

            return modelBuilder;
        }
    }
}
