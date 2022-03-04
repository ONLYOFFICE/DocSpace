namespace ASC.Feed.Models;

public class FeedLast : BaseEntity
{
    public string LastKey { get; set; }
    public DateTime LastDate { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { LastKey };
    }
}
public static class FeedLastExtension
{
    public static ModelBuilderWrapper AddFeedLast(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddFeedLast, Provider.MySql)
            .Add(PgSqlAddFeedLast, Provider.PostgreSql);
        return modelBuilder;
    }
    public static void MySqlAddFeedLast(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedLast>(entity =>
        {
            entity.HasKey(e => e.LastKey)
                .HasName("PRIMARY");

            entity.ToTable("feed_last");

            entity.Property(e => e.LastKey)
                .HasColumnName("last_key")
                .HasColumnType("varchar(128)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastDate)
                .HasColumnName("last_date")
                .HasColumnType("datetime");
        });
    }
    public static void PgSqlAddFeedLast(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedLast>(entity =>
        {
            entity.HasKey(e => e.LastKey)
                .HasName("feed_last_pkey");

            entity.ToTable("feed_last", "onlyoffice");

            entity.Property(e => e.LastKey)
                .HasColumnName("last_key")
                .HasMaxLength(128);

            entity.Property(e => e.LastDate).HasColumnName("last_date");
        });
    }
}
