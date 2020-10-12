using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("mail_mailbox")]
    public class Mailbox
    {
        public int Id { get; set; }
        public int Tenant { get; set; }

        [Column("id_user")]
        public string IdUser { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        [Column("is_removed")]
        public bool IsRemoved { get; set; }

        [Column("is_processed")]
        public bool IsProcessed { get; set; }

        [Column("is_server_mailbox")]
        public bool IsServerMailbox { get; set; }

        [Column("is_teamlab_mailbox")]
        public bool IsTeamlabMailbox { get; set; }

        public bool Imap { get; set; }

        [Column("user_online")]
        public bool UserOnline { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("msg_count_last")]
        public int MsgCountLast { get; set; }

        [Column("size_last")]
        public int SizeLast { get; set; }

        [Column("login_delay")]
        public int LoginDelay { get; set; }

        [Column("quota_error")]
        public bool QuotaError { get; set; }

        [Column("imap_intervals")]
        public string ImapIntervals { get; set; }

        [Column("begin_date")]
        public DateTime BeginDate { get; set; }

        [Column("email_in_folder")]
        public string EmailInFolder { get; set; }

        [Column("pop3_password")]
        public string Pop3Password { get; set; }

        [Column("smtp_password")]
        public string SmtpPassword { get; set; }

        [Column("token_type")]
        public int TokenType { get; set; }
        public string Token { get; set; }

        [Column("id_smtp_server")]
        public int IdSmtpServer { get; set; }

        [Column("id_in_server")]
        public int IdInServer { get; set; }

        [Column("date_checked")]
        public DateTime DateChecked { get; set; }

        [Column("date_user_checked")]
        public DateTime DateUserChecked { get; set; }

        [Column("date_login_delay_expires")]
        public DateTime DateLoginDelayExpires { get; set; }

        [Column("date_auth_error")]
        public DateTime? DateAuthError { get; set; }

        [Column("date_created")]
        public DateTime DateCreated { get; set; }

        [Column("date_modified")]
        public DateTime DateModified { get; set; }
    }
    public static class MailboxExtension
    {
        public static ModelBuilderWrapper AddMailbox(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddMailbox, Provider.MySql)
                .Add(PgSqlAddMailbox, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddMailbox(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mailbox>(entity =>
            {
                entity.ToTable("mail_mailbox");

                entity.HasIndex(e => e.Address)
                    .HasName("address_index");

                entity.HasIndex(e => e.IdInServer)
                    .HasName("main_mailbox_id_in_server_mail_mailbox_server_id");

                entity.HasIndex(e => e.IdSmtpServer)
                    .HasName("main_mailbox_id_smtp_server_mail_mailbox_server_id");

                entity.HasIndex(e => new { e.DateChecked, e.DateLoginDelayExpires })
                    .HasName("date_login_delay_expires");

                entity.HasIndex(e => new { e.Tenant, e.IdUser })
                    .HasName("user_id_index");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BeginDate)
                    .HasColumnName("begin_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.DateAuthError)
                    .HasColumnName("date_auth_error")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateChecked)
                    .HasColumnName("date_checked")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("date_created")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateLoginDelayExpires)
                    .HasColumnName("date_login_delay_expires")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.DateModified)
                    .HasColumnName("date_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.DateUserChecked)
                    .HasColumnName("date_user_checked")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmailInFolder)
                    .HasColumnName("email_in_folder")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Enabled)
                    .HasColumnName("enabled")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.IdInServer).HasColumnName("id_in_server");

                entity.Property(e => e.IdSmtpServer).HasColumnName("id_smtp_server");

                entity.Property(e => e.IdUser)
                    .IsRequired()
                    .HasColumnName("id_user")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Imap).HasColumnName("imap");

                entity.Property(e => e.ImapIntervals)
                    .HasColumnName("imap_intervals")
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsDefault).HasColumnName("is_default");

                entity.Property(e => e.IsProcessed).HasColumnName("is_processed");

                entity.Property(e => e.IsRemoved).HasColumnName("is_removed");

                entity.Property(e => e.IsServerMailbox).HasColumnName("is_server_mailbox");

                entity.Property(e => e.LoginDelay)
                    .HasColumnName("login_delay")
                    .HasDefaultValueSql("'30'");

                entity.Property(e => e.MsgCountLast).HasColumnName("msg_count_last");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Pop3Password)
                    .HasColumnName("pop3_password")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.QuotaError).HasColumnName("quota_error");

                entity.Property(e => e.SizeLast).HasColumnName("size_last");

                entity.Property(e => e.SmtpPassword)
                    .HasColumnName("smtp_password")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TokenType).HasColumnName("token_type");

                entity.Property(e => e.UserOnline).HasColumnName("user_online");
            });
        }

        public static void PgSqlAddMailbox(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mailbox>(entity =>
            {
                entity.ToTable("mail_mailbox", "onlyoffice");

                entity.HasIndex(e => e.Address)
                    .HasName("address_index");

                entity.HasIndex(e => e.IdInServer)
                    .HasName("main_mailbox_id_in_server_mail_mailbox_server_id");

                entity.HasIndex(e => e.IdSmtpServer)
                    .HasName("main_mailbox_id_smtp_server_mail_mailbox_server_id");

                entity.HasIndex(e => new { e.DateChecked, e.DateLoginDelayExpires })
                    .HasName("date_login_delay_expires");

                entity.HasIndex(e => new { e.Tenant, e.IdUser })
                    .HasName("user_id_index");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasMaxLength(255);

                entity.Property(e => e.BeginDate)
                    .HasColumnName("begin_date")
                    .HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.DateAuthError).HasColumnName("date_auth_error");

                entity.Property(e => e.DateChecked).HasColumnName("date_checked");

                entity.Property(e => e.DateCreated).HasColumnName("date_created");

                entity.Property(e => e.DateLoginDelayExpires)
                    .HasColumnName("date_login_delay_expires")
                    .HasDefaultValueSql("'1975-01-01 00:00:00'");

                entity.Property(e => e.DateModified)
                    .HasColumnName("date_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DateUserChecked).HasColumnName("date_user_checked");

                entity.Property(e => e.EmailInFolder).HasColumnName("email_in_folder");

                entity.Property(e => e.Enabled)
                    .HasColumnName("enabled")
                    .HasDefaultValueSql("'1'::smallint");

                entity.Property(e => e.IdInServer).HasColumnName("id_in_server");

                entity.Property(e => e.IdSmtpServer).HasColumnName("id_smtp_server");

                entity.Property(e => e.IdUser)
                    .IsRequired()
                    .HasColumnName("id_user")
                    .HasMaxLength(38);

                entity.Property(e => e.Imap)
                    .HasColumnName("imap")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ImapIntervals).HasColumnName("imap_intervals");

                entity.Property(e => e.IsDefault)
                    .HasColumnName("is_default")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsProcessed)
                    .HasColumnName("is_processed")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsRemoved)
                    .HasColumnName("is_removed")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsServerMailbox)
                    .HasColumnName("is_server_mailbox")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LoginDelay)
                    .HasColumnName("login_delay")
                    .HasDefaultValueSql("'30'");

                entity.Property(e => e.MsgCountLast).HasColumnName("msg_count_last");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Pop3Password)
                    .HasColumnName("pop3_password")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.QuotaError)
                    .HasColumnName("quota_error")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SizeLast).HasColumnName("size_last");

                entity.Property(e => e.SmtpPassword)
                    .HasColumnName("smtp_password")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.TokenType)
                    .HasColumnName("token_type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UserOnline)
                    .HasColumnName("user_online")
                    .HasDefaultValueSql("'0'");
            });
        }
    }
}
