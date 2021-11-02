using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Mail
{
    public class ServerServer
    {
        public int Id { get; set; }
        public string MxRecord { get; set; }
        public string ConnectionString { get; set; }
        public int ServerType { get; set; }
        public int SmtpSettingsId { get; set; }
        public int ImapSettingsId { get; set; }
    }
    public static class ServerServerExtension
    {
        public static ModelBuilderWrapper AddServerServer(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddServerServer, Provider.MySql)
                .Add(PgSqlAddServerServer, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddServerServer(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServerServer>(entity =>
            {
                entity.ToTable("mail_server_server");

                entity.HasIndex(e => e.ServerType)
                    .HasDatabaseName("mail_server_server_type_server_type_fk_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConnectionString)
                    .IsRequired()
                    .HasColumnName("connection_string")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ImapSettingsId).HasColumnName("imap_settings_id");

                entity.Property(e => e.MxRecord)
                    .IsRequired()
                    .HasColumnName("mx_record")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ServerType).HasColumnName("server_type");

                entity.Property(e => e.SmtpSettingsId).HasColumnName("smtp_settings_id");
            });
        }
        public static void PgSqlAddServerServer(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServerServer>(entity =>
            {
                entity.ToTable("mail_server_server", "onlyoffice");

                entity.HasIndex(e => e.ServerType)
                    .HasDatabaseName("mail_server_server_type_server_type_fk_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConnectionString)
                    .IsRequired()
                    .HasColumnName("connection_string");

                entity.Property(e => e.ImapSettingsId).HasColumnName("imap_settings_id");

                entity.Property(e => e.MxRecord)
                    .IsRequired()
                    .HasColumnName("mx_record")
                    .HasMaxLength(128)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.ServerType).HasColumnName("server_type");

                entity.Property(e => e.SmtpSettingsId).HasColumnName("smtp_settings_id");
            });
        }
    }
}
