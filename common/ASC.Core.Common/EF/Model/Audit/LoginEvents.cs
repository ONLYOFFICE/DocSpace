using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("login_events")]
    public class LoginEvents : MessageEvent
    {
        public string Login { get; set; }
    }
    public static class LoginEventsExtension
    {
        public static ModelBuilderWrapper AddLoginEvents(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddLoginEvents, Provider.MySql)
                .Add(PgSqlAddLoginEvents, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddLoginEvents(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<LoginEvents>(entity =>
            {
                _ = entity.ToTable("login_events");

                _ = entity.HasIndex(e => e.Date)
                    .HasName("date");

                _ = entity.HasIndex(e => new { e.TenantId, e.UserId })
                    .HasName("tenant_id");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.Action).HasColumnName("action");

                _ = entity.Property(e => e.Browser)
                    .HasColumnName("browser")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                _ = entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .HasColumnType("varchar(300)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Platform)
                    .HasColumnName("platform")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddLoginEvents(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<LoginEvents>(entity =>
            {
                _ = entity.ToTable("login_events", "onlyoffice");

                _ = entity.HasIndex(e => e.Date)
                    .HasName("date_login_events");

                _ = entity.HasIndex(e => new { e.UserId, e.TenantId })
                    .HasName("tenant_id_login_events");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.Action).HasColumnName("action");

                _ = entity.Property(e => e.Browser)
                    .HasColumnName("browser")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("NULL::character varying");

                _ = entity.Property(e => e.Date).HasColumnName("date");

                _ = entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .HasMaxLength(300)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.Platform)
                    .HasColumnName("platform")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasMaxLength(38)
                    .IsFixedLength();
            });
        }
    }
}
