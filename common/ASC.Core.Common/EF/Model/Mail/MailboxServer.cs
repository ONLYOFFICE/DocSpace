using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("mail_mailbox_server")]
    public class MailboxServer
    {
        public int Id { get; set; }

        [Column("id_provider")]
        public int IdProvider { get; set; }
        public string Type { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }

        [Column("socket_type")]
        public string SocketType { get; set; }
        public string UserName { get; set; }
        public string Authentication { get; set; }

        [Column("is_user_data")]
        public bool IsUserData { get; set; }
    }
    public static class MailboxServerExtension
    {
        public static ModelBuilderWrapper AddMailboxServer(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddMailboxServer, Provider.MySql)
                .Add(PgSqlAddMailboxServer, Provider.Postrge);
            return modelBuilder;
        }
        public static void MySqlAddMailboxServer(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailboxServer>(entity =>
            {
                entity.ToTable("mail_mailbox_server");

                entity.HasIndex(e => e.IdProvider)
                    .HasName("id_provider");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Authentication)
                    .HasColumnName("authentication")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Hostname)
                    .IsRequired()
                    .HasColumnName("hostname")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IdProvider).HasColumnName("id_provider");

                entity.Property(e => e.IsUserData).HasColumnName("is_user_data");

                entity.Property(e => e.Port).HasColumnName("port");

                entity.Property(e => e.SocketType)
                    .IsRequired()
                    .HasColumnName("socket_type")
                    .HasColumnType("enum('plain','SSL','STARTTLS')")
                    .HasDefaultValueSql("'plain'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("enum('pop3','imap','smtp')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserName)
                    .HasColumnName("username")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddMailboxServer(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum("onlyoffice", "en", new[] { "plain", "SSL", "STARTTLS" })
                .HasPostgresEnum("onlyoffice", "enum", new[] { "pop3", "imap", "smtp" });

            modelBuilder.Entity<MailboxServer>(entity =>
            {
                entity.ToTable("mail_mailbox_server", "onlyoffice");

                entity.HasIndex(e => e.IdProvider)
                    .HasName("id_provider_mail_mailbox_server");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("onlyoffice.enum");

                entity.Property(e => e.Authentication)
                    .HasColumnName("authentication")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Hostname)
                    .IsRequired()
                    .HasColumnName("hostname")
                    .HasColumnType("character varying");

                entity.Property(e => e.IdProvider).HasColumnName("id_provider");

                entity.Property(e => e.IsUserData)
                    .HasColumnName("is_user_data")
                    .HasDefaultValueSql("'0'::smallint");

                entity.Property(e => e.Port).HasColumnName("port");

                entity.Property(e => e.SocketType)
                    .IsRequired()
                    .HasColumnName("socket_type")
                    .HasColumnType("onlyoffice.en")
                    .HasDefaultValueSql("'plain'::text");

                entity.Property(e => e.UserName)
                    .HasColumnName("username")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");
            });
        }
    }
}
