namespace ASC.Core.Common.EF;

public class DbGroup : BaseEntity, IMapFrom<Group>
{
    public int Tenant { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? ParentId { get; set; }
    public string Sid { get; set; }
    public bool Removed { get; set; }
    public DateTime LastModified { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}

public static class DbGroupExtension
{
    public static ModelBuilderWrapper AddDbGroup(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbGroup, Provider.MySql)
            .Add(PgSqlAddDbGroup, Provider.PostgreSql);

        return modelBuilder;
    }

    private static void MySqlAddDbGroup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbGroup>(entity =>
        {
            entity.ToTable("core_group");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.HasIndex(e => new { e.Tenant, e.ParentId })
                .HasDatabaseName("parentid");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CategoryId)
                .HasColumnName("categoryid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(128)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ParentId)
                .HasColumnName("parentid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Removed).HasColumnName("removed");

            entity.Property(e => e.Sid)
                .HasColumnName("sid")
                .HasColumnType("varchar(512)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Tenant).HasColumnName("tenant");
        });
    }
    private static void PgSqlAddDbGroup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbGroup>(entity =>
        {
            entity.ToTable("core_group");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.HasIndex(e => new { e.Tenant, e.ParentId })
                .HasDatabaseName("parentid");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasMaxLength(38);

            entity.Property(e => e.CategoryId)
                .HasColumnName("categoryid")
                .HasMaxLength(38)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(128);

            entity.Property(e => e.ParentId)
                .HasColumnName("parentid")
                .HasMaxLength(38)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Removed).HasColumnName("removed");

            entity.Property(e => e.Sid)
                .HasColumnName("sid")
                .HasMaxLength(512)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Tenant).HasColumnName("tenant");
        });
    }
}
