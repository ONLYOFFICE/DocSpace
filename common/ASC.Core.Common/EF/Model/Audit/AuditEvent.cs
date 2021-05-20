using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class AuditEvent : MessageEvent
    {
        public string Initiator { get; set; }
        public string Target { get; set; }
    }
    public static class AuditEventExtension
    {
        public static ModelBuilderWrapper AddAuditEvent(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddAuditEvent, Provider.MySql)
                .Add(PgSqlAddAuditEvent, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddAuditEvent(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEvent>(entity =>
            {
                entity.ToTable("audit_events");

                entity.HasIndex(e => new { e.TenantId, e.Date })
                    .HasDatabaseName("date");

                entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Browser)
                    .HasColumnName("browser")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(20000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Initiator)
                    .HasColumnName("initiator")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .HasColumnType("varchar(300)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Platform)
                    .HasColumnName("platform")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Target)
                    .HasColumnName("target")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddAuditEvent(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEvent>(entity =>
            {
                entity.ToTable("audit_events", "onlyoffice");

                entity.HasIndex(e => new { e.TenantId, e.Date })
                    .HasDatabaseName("date");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Browser)
                    .HasColumnName("browser")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(20000)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Initiator)
                    .HasColumnName("initiator")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Ip)
                    .HasColumnName("ip")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .HasMaxLength(300)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Platform)
                    .HasColumnName("platform")
                    .HasMaxLength(200)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Target).HasColumnName("target");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(38)
                    .IsFixedLength()
                    .HasDefaultValueSql("NULL");
            });
        }
    }
}
