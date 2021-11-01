using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
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
        public static ModelBuilderWrapper AddSubscriptionMethod(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddSubscriptionMethod, Provider.MySql)
                .Add(PgSqlAddSubscriptionMethod, Provider.Postgre)
                .HasData(
                new DbSubscriptionMethod { Source = "asc.web.studio", Action = "send_whats_new", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6504977c-75af-4691-9099-084d3ddeea04", Action = "new feed", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6a598c74-91ae-437d-a5f4-ad339bd11bb2", Action = "new post", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "853b6eb9-73ee-438d-9b09-8ffeedf36234", Action = "new topic in forum", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "9d51954f-db9b-4aed-94e3-ed70b914e101", Action = "new photo uploaded", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "28b10049-dd20-4f54-b986-873bc14ccfc7", Action = "new bookmark created", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "742cf945-cbbc-4a57-82d6-1600a12cf8ca", Action = "new wiki page", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "37620ae5-c40b-45ce-855a-39dd7d76a1fa", Action = "BirthdayReminder", Recipient = "abef62db-11a8-4673-9d32-ef1d8af19dc0", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6fe286a4-479e-4c25-a8d9-0156e332b0c0", Action = "sharedocument", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6fe286a4-479e-4c25-a8d9-0156e332b0c0", Action = "sharefolder", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6fe286a4-479e-4c25-a8d9-0156e332b0c0", Action = "updatedocument", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "invitetoproject", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "milestonedeadline", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "newcommentformessage", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "newcommentformilestone", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "newcommentfortask", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "projectcreaterequest", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "projecteditrequest", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "removefromproject", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "responsibleforproject", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "responsiblefortask", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "6045b68c-2c2e-42db-9e53-c272e814c4ad", Action = "taskclosed", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "40650da3-f7c1-424c-8c89-b9c115472e08", Action = "calendar_sharing", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "40650da3-f7c1-424c-8c89-b9c115472e08", Action = "event_alert", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "asc.web.studio", Action = "admin_notify", Recipient = "cd84e66b-b803-40fc-99f9-b2969a54a1de", Sender = "email.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "13ff36fb-0272-4887-b416-74f52b0d0b02", Action = "SetAccess", Recipient = "abef62db-11a8-4673-9d32-ef1d8af19dc0", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "13ff36fb-0272-4887-b416-74f52b0d0b02", Action = "ResponsibleForTask", Recipient = "abef62db-11a8-4673-9d32-ef1d8af19dc0", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "13ff36fb-0272-4887-b416-74f52b0d0b02", Action = "AddRelationshipEvent", Recipient = "abef62db-11a8-4673-9d32-ef1d8af19dc0", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "13ff36fb-0272-4887-b416-74f52b0d0b02", Action = "ExportCompleted", Recipient = "abef62db-11a8-4673-9d32-ef1d8af19dc0", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "13ff36fb-0272-4887-b416-74f52b0d0b02", Action = "CreateNewContact", Recipient = "abef62db-11a8-4673-9d32-ef1d8af19dc0", Sender = "email.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "13ff36fb-0272-4887-b416-74f52b0d0b02", Action = "ResponsibleForOpportunity", Recipient = "abef62db-11a8-4673-9d32-ef1d8af19dc0", Sender = "email.sender|messanger.sender", Tenant = -1 },
                new DbSubscriptionMethod { Source = "asc.web.studio", Action = "periodic_notify", Recipient = "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", Sender = "email.sender", Tenant = -1 }
                );

            return modelBuilder;
        }

        public static void MySqlAddSubscriptionMethod(this ModelBuilder modelBuilder)
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
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Action)
                    .HasColumnName("action")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Recipient)
                    .HasColumnName("recipient")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Sender)
                    .IsRequired()
                    .HasColumnName("sender")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddSubscriptionMethod(this ModelBuilder modelBuilder)
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
        }
    }
}
