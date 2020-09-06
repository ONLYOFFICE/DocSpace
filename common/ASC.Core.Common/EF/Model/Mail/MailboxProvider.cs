using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Table("mail_mailbox_provider")]
    public class MailboxProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("display_short_name")]
        public string DisplayShortName { get; set; }
        public string Documentation { get; set; }
    }
    public static class MailboxProviderExtension
    {
        public static ModelBuilderWrapper AddMailboxProvider(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddMailboxProvider, Provider.MySql)
                .Add(PgSqlAddMailboxProvider, Provider.Postrge);
            return modelBuilder;
        }
        public static void MySqlAddMailboxProvider(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailboxProvider>(entity =>
            {
                entity.ToTable("mail_mailbox_provider");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DisplayName)
                    .HasColumnName("display_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DisplayShortName)
                    .HasColumnName("display_short_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Documentation)
                    .HasColumnName("documentation")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddMailboxProvider(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailboxProvider>(entity =>
            {
                entity.ToTable("mail_mailbox_provider", "onlyoffice");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DisplayName)
                    .HasColumnName("display_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.DisplayShortName)
                    .HasColumnName("display_short_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Documentation)
                    .HasColumnName("documentation")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });
        }
    }
}
