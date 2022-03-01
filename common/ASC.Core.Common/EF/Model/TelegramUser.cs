namespace ASC.Core.Common.EF.Model;

public class TelegramUser : BaseEntity
{
    public Guid PortalUserId { get; set; }
    public int TenantId { get; set; }
    public int TelegramUserId { get; set; }
    public override object[] GetKeys()
    {
        return new object[] { TenantId, PortalUserId };
    }
}

public static class TelegramUsersExtension
{
    public static ModelBuilderWrapper AddTelegramUsers(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddTelegramUsers, Provider.MySql)
            .Add(PgSqlAddTelegramUsers, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddTelegramUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.PortalUserId })
                .HasName("PRIMARY");

            entity.ToTable("telegram_users");

            entity.HasIndex(e => e.TelegramUserId)
                .HasDatabaseName("tgId");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.PortalUserId)
                .HasColumnName("portal_user_id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TelegramUserId).HasColumnName("telegram_user_id");
        });
    }
    public static void PgSqlAddTelegramUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.PortalUserId })
                .HasName("telegram_users_pkey");

            entity.ToTable("telegram_users", "onlyoffice");

            entity.HasIndex(e => e.TelegramUserId)
                .HasDatabaseName("tgId");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.PortalUserId)
                .HasColumnName("portal_user_id")
                .HasMaxLength(38);

            entity.Property(e => e.TelegramUserId).HasColumnName("telegram_user_id");
        });
    }
}
