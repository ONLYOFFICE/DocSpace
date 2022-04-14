namespace ASC.EventBus.Extensions.Logger.Extensions;

public static class IntegrationEventLogExtension
{
    public static ModelBuilderWrapper AddIntegrationEventLog(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddIntegrationEventLog, Provider.MySql)
            .Add(PgSqlAddIntegrationEventLog, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddIntegrationEventLog(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IntegrationEventLogEntry>(entity =>
        {
            entity.ToTable("event_bus_integration_event_log");

            entity.HasKey(e => e.EventId)
                  .HasName("PRIMARY");

            entity.HasIndex(e => e.TenantId)
                  .HasDatabaseName("tenant_id");

            entity.Property(e => e.EventId)
                  .HasColumnName("event_id")
                  .HasColumnType("char(38)")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci")
                  .IsRequired();

            entity.Property(e => e.Content)
                  .HasColumnName("content")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci")
                  .IsRequired();

            entity.Property(e => e.CreateOn)
                  .HasColumnName("create_on")
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("char(38)")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci")
                  .IsRequired();

            entity.Property(e => e.State)
                  .HasColumnName("state")
                  .HasColumnType("int(11)")
                  .IsRequired();

            entity.Property(e => e.TimesSent)
                  .HasColumnName("times_sent")
                  .HasColumnType("int(11)")
                  .IsRequired();

            entity.Property(e => e.EventTypeName)
                  .HasColumnName("event_type_name")
                  .HasColumnType("varchar(255)")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci")
                  .IsRequired();

            entity.Property(e => e.TenantId)
                  .HasColumnName("tenant_id")
                  .HasColumnType("int(11)")
                  .IsRequired();
        });
    }

    public static void PgSqlAddIntegrationEventLog(this ModelBuilder modelBuilder)
    {
        throw new NotImplementedException();
    }
}
