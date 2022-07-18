// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Core.Common.EF.Model.Mail;

public class Mailbox
{
    public int Id { get; set; }
    public int Tenant { get; set; }
    public string IdUser { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public bool IsRemoved { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsServerMailbox { get; set; }
    public bool IsTeamlabMailbox { get; set; }
    public bool Imap { get; set; }
    public bool UserOnline { get; set; }
    public bool IsDefault { get; set; }
    public int MsgCountLast { get; set; }
    public int SizeLast { get; set; }
    public int LoginDelay { get; set; }
    public bool QuotaError { get; set; }
    public string ImapIntervals { get; set; }
    public DateTime BeginDate { get; set; }
    public string EmailInFolder { get; set; }
    public string Pop3Password { get; set; }
    public string SmtpPassword { get; set; }
    public int TokenType { get; set; }
    public string Token { get; set; }
    public int IdSmtpServer { get; set; }
    public int IdInServer { get; set; }
    public DateTime DateChecked { get; set; }
    public DateTime DateUserChecked { get; set; }
    public DateTime DateLoginDelayExpires { get; set; }
    public DateTime? DateAuthError { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
}

public static class MailboxExtension
{
    public static ModelBuilderWrapper AddMailbox(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddMailbox, Provider.MySql)
            .Add(PgSqlAddMailbox, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddMailbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mailbox>(entity =>
        {
            entity.ToTable("mail_mailbox");

            entity.HasIndex(e => e.Address)
                .HasDatabaseName("address_index");

            entity.HasIndex(e => e.IdInServer)
                .HasDatabaseName("main_mailbox_id_in_server_mail_mailbox_server_id");

            entity.HasIndex(e => e.IdSmtpServer)
                .HasDatabaseName("main_mailbox_id_smtp_server_mail_mailbox_server_id");

            entity.HasIndex(e => new { e.DateChecked, e.DateLoginDelayExpires })
                .HasDatabaseName("date_login_delay_expires");

            entity.HasIndex(e => new { e.Tenant, e.IdUser })
                .HasDatabaseName("user_id_index");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Address)
                .IsRequired()
                .HasColumnName("address")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

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
                .UseCollation("utf8_general_ci");

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
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Imap).HasColumnName("imap");

            entity.Property(e => e.ImapIntervals)
                .HasColumnName("imap_intervals")
                .HasColumnType("mediumtext")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

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
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Pop3Password)
                .HasColumnName("pop3_password")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.QuotaError).HasColumnName("quota_error");

            entity.Property(e => e.SizeLast).HasColumnName("size_last");

            entity.Property(e => e.SmtpPassword)
                .HasColumnName("smtp_password")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Tenant).HasColumnName("tenant");

            entity.Property(e => e.Token)
                .HasColumnName("token")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

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
                .HasDatabaseName("address_index");

            entity.HasIndex(e => e.IdInServer)
                .HasDatabaseName("main_mailbox_id_in_server_mail_mailbox_server_id");

            entity.HasIndex(e => e.IdSmtpServer)
                .HasDatabaseName("main_mailbox_id_smtp_server_mail_mailbox_server_id");

            entity.HasIndex(e => new { e.DateChecked, e.DateLoginDelayExpires })
                .HasDatabaseName("date_login_delay_expires");

            entity.HasIndex(e => new { e.Tenant, e.IdUser })
                .HasDatabaseName("user_id_index");

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
