using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("audit_events")]
    public class AuditEvent : MessageEvent
    {
        public string Initiator { get; set; }
        public string Target { get; set; }
    }
    public static class AuditEventExtension
    {
        public static ModelBuilder MySqlAddAuditEvent(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEvent>(entity =>
            {
                entity.ToTable("audit_events");

                entity.HasIndex(e => new { e.TenantId, e.Date })
                    .HasName("date");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Browser)
                    .HasColumnName("browser")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(20000)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Initiator)
                    .HasColumnName("initiator")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .HasColumnType("varchar(300)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Platform)
                    .HasColumnName("platform")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Target)
                    .HasColumnName("target")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddAuditEvent(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEvent>(entity =>
            {
                entity.ToTable("audit_events", "onlyoffice");

                entity.HasIndex(e => new { e.TenantId, e.Date })
                    .HasName("date");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Browser)
                    .HasColumnName("browser")
                    .HasMaxLength(200);

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(20000);

                entity.Property(e => e.Initiator)
                    .HasColumnName("initiator")
                    .HasMaxLength(200);

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasMaxLength(50);

                entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .HasMaxLength(300);

                entity.Property(e => e.Platform)
                    .HasColumnName("platform")
                    .HasMaxLength(200);

                entity.Property(e => e.Target).HasColumnName("target");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(38)
                    .IsFixedLength();
            });
            return modelBuilder;
        }
    }
}
