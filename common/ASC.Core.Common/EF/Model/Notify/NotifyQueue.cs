using System;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class NotifyQueue
    {
        public int NotifyId { get; set; }
        public int TenantId { get; set; }
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public string Subject { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string SenderType { get; set; }
        public string ReplyTo { get; set; }
        public DateTime CreationDate { get; set; }
        public string Attachments { get; set; }
        public string AutoSubmitted { get; set; }
    }
    public static class NotifyQueueExtension
    {
        public static ModelBuilderWrapper AddNotifyQueue(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddNotifyQueue, Provider.MySql)
                .Add(PgSqlAddNotifyQueue, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddNotifyQueue(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyQueue>(entity =>
            {
                entity.HasKey(e => e.NotifyId)
                    .HasName("PRIMARY");

                entity.ToTable("notify_queue");

                entity.Property(e => e.NotifyId).HasColumnName("notify_id");

                entity.Property(e => e.Attachments)
                    .HasColumnName("attachments")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AutoSubmitted)
                    .HasColumnName("auto_submitted")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ContentType)
                    .HasColumnName("content_type")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creation_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Reciever)
                    .HasColumnName("reciever")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ReplyTo)
                    .HasColumnName("reply_to")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Sender)
                    .HasColumnName("sender")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.SenderType)
                    .HasColumnName("sender_type")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddNotifyQueue(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyQueue>(entity =>
            {
                entity.HasKey(e => e.NotifyId)
                    .HasName("notify_queue_pkey");

                entity.ToTable("notify_queue", "onlyoffice");

                entity.Property(e => e.NotifyId).HasColumnName("notify_id");

                entity.Property(e => e.Attachments).HasColumnName("attachments");

                entity.Property(e => e.AutoSubmitted)
                    .HasColumnName("auto_submitted")
                    .HasMaxLength(64)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.ContentType)
                    .HasColumnName("content_type")
                    .HasMaxLength(64)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CreationDate).HasColumnName("creation_date");

                entity.Property(e => e.Reciever)
                    .HasColumnName("reciever")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReplyTo)
                    .HasColumnName("reply_to")
                    .HasMaxLength(1024)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Sender)
                    .HasColumnName("sender")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.SenderType)
                    .HasColumnName("sender_type")
                    .HasMaxLength(64)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasMaxLength(1024)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
    }
}
