namespace ASC.Feed.Models;

public class FeedAggregate : BaseEntity, IMapFrom<FeedRow>
{
    public string Id { get; set; }
    public int Tenant { get; set; }
    public string Product { get; set; }
    public string Module { get; set; }
    public Guid Author { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string GroupId { get; set; }
    public DateTime AggregateDate { get; set; }
    public string Json { get; set; }
    public string Keywords { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}
public static class FeedAggregateExtension
{
    public static ModelBuilderWrapper AddFeedAggregate(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddFeedAggregate, Provider.MySql)
            .Add(PgSqlAddFeedAggregate, Provider.PostgreSql);
        return modelBuilder;
    }
    public static void MySqlAddFeedAggregate(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedAggregate>(entity =>
        {
            entity.ToTable("feed_aggregate");

            entity.HasIndex(e => new { e.Tenant, e.AggregateDate })
                .HasDatabaseName("aggregated_date");

            entity.HasIndex(e => new { e.Tenant, e.ModifiedDate })
                .HasDatabaseName("modified_date");

            entity.HasIndex(e => new { e.Tenant, e.Product })
                .HasDatabaseName("product");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("varchar(88)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AggregateDate)
                .HasColumnName("aggregated_date")
                .HasColumnType("datetime");

            entity.Property(e => e.Author)
                .IsRequired()
                .HasColumnName("author")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("created_date")
                .HasColumnType("datetime");

            entity.Property(e => e.GroupId)
                .HasColumnName("group_id")
                .HasColumnType("varchar(70)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Json)
                .IsRequired()
                .HasColumnName("json")
                .HasColumnType("mediumtext")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Keywords)
                .HasColumnName("keywords")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ModifiedBy)
                .IsRequired()
                .HasColumnName("modified_by")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ModifiedDate)
                .HasColumnName("modified_date")
                .HasColumnType("datetime");

            entity.Property(e => e.Module)
                .IsRequired()
                .HasColumnName("module")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Product)
                .IsRequired()
                .HasColumnName("product")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Tenant).HasColumnName("tenant");
        });
    }
    public static void PgSqlAddFeedAggregate(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedAggregate>(entity =>
        {
            entity.ToTable("feed_aggregate", "onlyoffice");

            entity.HasIndex(e => new { e.Tenant, e.AggregateDate })
                .HasDatabaseName("aggregated_date");

            entity.HasIndex(e => new { e.Tenant, e.ModifiedDate })
                .HasDatabaseName("modified_date");

            entity.HasIndex(e => new { e.Tenant, e.Product })
                .HasDatabaseName("product");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasMaxLength(88);

            entity.Property(e => e.AggregateDate).HasColumnName("aggregated_date");

            entity.Property(e => e.Author)
                .IsRequired()
                .HasColumnName("author")
                .HasMaxLength(38)
                .IsFixedLength();

            entity.Property(e => e.CreatedDate).HasColumnName("created_date");

            entity.Property(e => e.GroupId)
                .HasColumnName("group_id")
                .HasMaxLength(70)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Json)
                .IsRequired()
                .HasColumnName("json");

            entity.Property(e => e.Keywords).HasColumnName("keywords");

            entity.Property(e => e.ModifiedBy)
                .IsRequired()
                .HasColumnName("modified_by")
                .HasMaxLength(38)
                .IsFixedLength();

            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");

            entity.Property(e => e.Module)
                .IsRequired()
                .HasColumnName("module")
                .HasMaxLength(50);

            entity.Property(e => e.Product)
                .IsRequired()
                .HasColumnName("product")
                .HasMaxLength(50);

            entity.Property(e => e.Tenant).HasColumnName("tenant");
        });
    }
}
