namespace ASC.MessagingSystem.Models;

public class LoginEvent : MessageEvent, IMapFrom<EventMessage>
{
    public string Login { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<EventMessage, LoginEvent>()
            .ConvertUsing<EventTypeConverter>();
    }
}

public static class LoginEventsExtension
{
    public static ModelBuilderWrapper AddLoginEvents(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddLoginEvents, Provider.MySql)
            .Add(PgSqlAddLoginEvents, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddLoginEvents(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginEvent>(entity =>
        {
            entity.ToTable("login_events");

            entity.HasIndex(e => e.Date)
                .HasDatabaseName("date");

            entity.HasIndex(e => new { e.TenantId, e.UserId })
                .HasDatabaseName("tenant_id");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Action).HasColumnName("action");

            entity.Property(e => e.Browser)
                .HasColumnName("browser")
                .HasColumnType("varchar(200)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Date)
                .HasColumnName("date")
                .HasColumnType("datetime");

            entity.Property(e => e.DescriptionRaw)
                .HasColumnName("description")
                .HasColumnType("varchar(500)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Ip)
                .HasColumnName("ip")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Login)
                .HasColumnName("login")
                .HasColumnType("varchar(200)")
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

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddLoginEvents(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginEvent>(entity =>
        {
            entity.ToTable("login_events", "onlyoffice");

            entity.HasIndex(e => e.Date)
                .HasDatabaseName("date_login_events");

            entity.HasIndex(e => new { e.UserId, e.TenantId })
                .HasDatabaseName("tenant_id_login_events");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Action).HasColumnName("action");

            entity.Property(e => e.Browser)
                .HasColumnName("browser")
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL::character varying");

            entity.Property(e => e.Date).HasColumnName("date");

            entity.Property(e => e.DescriptionRaw)
                .HasColumnName("description")
                .HasMaxLength(500)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Ip)
                .HasColumnName("ip")
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Login)
                .HasColumnName("login")
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Page)
                .HasColumnName("page")
                .HasMaxLength(300)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Platform)
                .HasColumnName("platform")
                .HasMaxLength(200)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id")
                .HasMaxLength(38)
                .IsFixedLength();
        });
    }
}
