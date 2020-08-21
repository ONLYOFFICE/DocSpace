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

        public override object[] GetKeys()
        {
            return new object[] { Tenant, Source, Action, Recipient };
        }
    }

    public static class SubscriptionMethodExtension
    {
        public static ModelBuilder MySqlAddSubscriptionMethod(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbSubscriptionMethod>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Source, e.Action, e.Recipient })
                    .HasName("PRIMARY");

                entity.ToTable("core_subscriptionmethod");

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

                entity.Property(e => e.Sender)
                    .IsRequired()
                    .HasColumnName("sender")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddSubscriptionMethod(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbSubscriptionMethod>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Source, e.Action, e.Recipient })
                    .HasName("core_subscriptionmethod_pkey");

                entity.ToTable("core_subscriptionmethod", "onlyoffice");

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

                entity.Property(e => e.Sender)
                    .IsRequired()
                    .HasColumnName("sender")
                    .HasMaxLength(1024);
            });
            return modelBuilder;
        }
    }
}
