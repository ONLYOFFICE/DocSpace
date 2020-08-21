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

        public override object[] GetKeys()
        {
            return new object[] { Tenant, Source, Action, Recipient, Object };
        }
    }

    public static class SubscriptionExtension
    {
        public static ModelBuilder MySqlAddSubscription(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Source, e.Action, e.Recipient, e.Object })
                    .HasName("PRIMARY");

                entity.ToTable("core_subscription");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Source)
                    .HasColumnName("source")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Action)
                    .HasColumnName("action")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Recipient)
                    .HasColumnName("recipient")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Object)
                    .HasColumnName("object")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Unsubscribed).HasColumnName("unsubscribed");
            });
            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddSubscription(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Source, e.Action, e.Recipient, e.Object })
                    .HasName("core_subscription_pkey");

                entity.ToTable("core_subscription", "onlyoffice");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Source)
                    .HasColumnName("source")
                    .HasMaxLength(38);

                entity.Property(e => e.Action)
                    .HasColumnName("action")
                    .HasMaxLength(128);

                entity.Property(e => e.Recipient)
                    .HasColumnName("recipient")
                    .HasMaxLength(38);

                entity.Property(e => e.Object)
                    .HasColumnName("object")
                    .HasMaxLength(128);

                entity.Property(e => e.Unsubscribed).HasColumnName("unsubscribed");
            });
            return modelBuilder;
        }
    }
}
