using Microsoft.EntityFrameworkCore;

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

        [Column("auto_submitted")]
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
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AutoSubmitted)
                    .HasColumnName("auto_submitted")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContentType)
                    .HasColumnName("content_type")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creation_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Reciever)
                    .HasColumnName("reciever")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ReplyTo)
                    .HasColumnName("reply_to")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Sender)
                    .HasColumnName("sender")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SenderType)
                    .HasColumnName("sender_type")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

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
