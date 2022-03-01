namespace ASC.Feed.Models;

public class FeedReaded : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Module { get; set; }
    public int Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Tenant, UserId, Module };
    }
}

public static class FeedReadedExtension
{
    public static ModelBuilderWrapper AddFeedReaded(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddFeedReaded, Provider.MySql)
            .Add(PgSqlAddFeedReaded, Provider.PostgreSql);
        return modelBuilder;
    }
    public static void MySqlAddFeedReaded(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedReaded>(entity =>
        {
            entity.HasKey(e => new { e.Tenant, e.UserId, e.Module })
                .HasName("PRIMARY");

            entity.ToTable("feed_readed");

            entity.Property(e => e.Tenant).HasColumnName("tenant_id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Module)
                .HasColumnName("module")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TimeStamp)
                .HasColumnName("timestamp")
                .HasColumnType("datetime");
        });
    }
    public static void PgSqlAddFeedReaded(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedReaded>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Tenant, e.Module })
                .HasName("feed_readed_pkey");

            entity.ToTable("feed_readed", "onlyoffice");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(38);

            entity.Property(e => e.Tenant).HasColumnName("tenant_id");

            entity.Property(e => e.Module)
                .HasColumnName("module")
                .HasMaxLength(50);

            entity.Property(e => e.TimeStamp).HasColumnName("timestamp");
        });
    }
}
