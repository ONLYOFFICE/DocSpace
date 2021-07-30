﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ASC.Common;
using ASC.Core.Common.EF;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;
using ASC.Mail.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ASC.Mail.Core.Dao.Entities
{
    public static class Tables
    {
        public const string Mail = "mail";
        public const string Contact = "contact";
        public const string ContactInfo = "contact_info";
        public const string Tag = "tag";
        public const string UserFolder = "user_folder";

    }

    [Transient]
    [ElasticsearchType(RelationName = Tables.Mail)]
    [Table("mail_mail")]
    public partial class MailMail : BaseEntity, ISearchItemDocument
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("id_mailbox", TypeName = "int(11)")]
        public int IdMailbox { get; set; }
        [Required]
        [Column("id_user", TypeName = "varchar(255)")]
        public string IdUser { get; set; }
        [Column("tenant", TypeName = "int(11)")]
        public int TenantId { get; set; }
        [Column("uidl", TypeName = "varchar(255)")]
        public string Uidl { get; set; }
        [Column("md5", TypeName = "varchar(255)")]
        public string Md5 { get; set; }
        [Required]
        [Column("address", TypeName = "varchar(255)")]
        public string Address { get; set; }
        [Column("from_text", TypeName = "text")]
        public string FromText { get; set; }
        [Column("to_text", TypeName = "text")]
        public string ToText { get; set; }
        [Column("reply_to", TypeName = "text")]
        public string ReplyTo { get; set; }
        [Column("cc", TypeName = "text")]
        public string Cc { get; set; }
        [Column("bcc", TypeName = "text")]
        public string Bcc { get; set; }
        [Column("subject", TypeName = "text")]
        public string Subject { get; set; }
        [Required]
        [Column("introduction", TypeName = "varchar(255)")]
        public string Introduction { get; set; }
        [Column("importance")]
        public bool Importance { get; set; }
        [Column("date_received", TypeName = "datetime")]
        public DateTime DateReceived { get; set; }
        [Column("date_sent", TypeName = "datetime")]
        public DateTime DateSent { get; set; }
        [Column("size", TypeName = "int(11)")]
        public int Size { get; set; }
        [Column("attachments_count", TypeName = "int(11)")]
        public int AttachmentsCount { get; set; }
        [Column("unread", TypeName = "int(11)")]
        public bool Unread { get; set; }
        [Column("is_answered", TypeName = "int(11)")]
        public bool IsAnswered { get; set; }
        [Column("is_forwarded", TypeName = "int(11)")]
        public bool IsForwarded { get; set; }
        [Column("is_from_crm", TypeName = "int(11)")]
        public bool IsFromCrm { get; set; }
        [Column("is_from_tl", TypeName = "int(11)")]
        public bool IsFromTl { get; set; }
        [Column("is_text_body_only", TypeName = "int(11)")]
        public bool IsTextBodyOnly { get; set; }
        [Column("has_parse_error")]
        public bool HasParseError { get; set; }
        [Column("calendar_uid", TypeName = "varchar(255)")]
        public string CalendarUid { get; set; }
        [Required]
        [Column("stream", TypeName = "varchar(38)")]
        public string Stream { get; set; }
        [Column("folder", TypeName = "int(11)")]
        public int Folder { get; set; }
        [Column("folder_restore", TypeName = "int(11)")]
        public int FolderRestore { get; set; }
        [Column("spam", TypeName = "int(11)")]
        public bool Spam { get; set; }
        [Column("time_modified", TypeName = "timestamp")]
        public DateTime TimeModified { get; set; }
        [Column("is_removed")]
        public bool IsRemoved { get; set; }
        [Column("mime_message_id", TypeName = "varchar(255)")]
        public string MimeMessageId { get; set; }
        [Column("mime_in_reply_to", TypeName = "varchar(255)")]
        public string MimeInReplyTo { get; set; }
        [Column("chain_id", TypeName = "varchar(255)")]
        public string ChainId { get; set; }
        [Column("chain_date", TypeName = "datetime")]
        public DateTime ChainDate { get; set; }

        [Nested]
        [NotMapped]
        public ICollection<MailAttachment> Attachments { get; set; }

        [Nested]
        [NotMapped]
        public ICollection<MailUserFolder> UserFolders { get; set; }

        [Nested]
        [NotMapped]
        public ICollection<MailTag> Tags { get; set; }

        [Nested]
        [NotMapped]
        public bool HasAttachments { get; set; }

        [Nested]
        [NotMapped]
        public bool WithCalendar { get; set; }

        [NotMapped]
        public Document Document { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get => Tables.Mail;
        }

        public Expression<Func<ISearchItem, object[]>> SearchContentFields
        {
            get => (a) => new[] { Subject, FromText, ToText, Cc, Bcc, Document.Attachment.Content };
        }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            throw new NotImplementedException();
        }
    }

    public static class MailMailExtension
    {
        public static ModelBuilder AddMailMail(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailMail>(entity =>
            {
                entity.HasIndex(e => e.TimeModified)
                    .HasDatabaseName("time_modified");

                entity.HasIndex(e => new { e.IdMailbox, e.MimeMessageId })
                    .HasDatabaseName("mime_message_id");

                entity.HasIndex(e => new { e.Md5, e.IdMailbox })
                    .HasDatabaseName("md5");

                entity.HasIndex(e => new { e.Uidl, e.IdMailbox })
                    .HasDatabaseName("uidl");

                entity.HasIndex(e => new { e.ChainId, e.IdMailbox, e.Folder })
                    .HasDatabaseName("chain_index_folders");

                entity.HasIndex(e => new { e.TenantId, e.IdUser, e.Folder, e.ChainDate })
                    .HasDatabaseName("list_conversations");

                entity.HasIndex(e => new { e.TenantId, e.IdUser, e.Folder, e.DateSent })
                    .HasDatabaseName("list_messages");

                entity.Property(e => e.Address)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Bcc)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CalendarUid)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Cc)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ChainDate).HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.ChainId)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DateReceived).HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.DateSent).HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.Folder).HasDefaultValueSql("'1'");

                entity.Property(e => e.FolderRestore).HasDefaultValueSql("'1'");

                entity.Property(e => e.FromText)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IdUser)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Introduction)
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Md5)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MimeInReplyTo)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MimeMessageId)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ReplyTo)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Stream)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Subject)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TimeModified)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ToText)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Uidl)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasMany(m => m.Attachments)
                    .WithOne(a => a.Mail)
                    .HasForeignKey(a => a.IdMail);
            });

            return modelBuilder;
        }
    }
}