namespace ASC.Core.Common.EF.Model.Resource;

public class ResData
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public string Title { get; set; }
    public string CultureTitle { get; set; }
    public string TextValue { get; set; }
    public string Description { get; set; }
    public DateTime TimeChanges { get; set; }
    public string ResourceType { get; set; }
    public int Flag { get; set; }
    public string Link { get; set; }
    public string AuthorLogin { get; set; }
}

public static class ResDataExtension
{
    public static ModelBuilderWrapper AddResData(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddResData, Provider.MySql)
            .Add(PgSqlAddResData, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddResData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResData>(entity =>
        {
            entity.HasKey(e => new { e.FileId, e.CultureTitle, e.Title })
                .HasName("PRIMARY");

            entity.ToTable("res_data");

            entity.HasIndex(e => e.CultureTitle)
                .HasDatabaseName("resources_FK2");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("id")
                .IsUnique();

            entity.HasIndex(e => e.TimeChanges)
                .HasDatabaseName("dateIndex");

            entity.Property(e => e.FileId).HasColumnName("fileid");

            entity.Property(e => e.CultureTitle)
                .HasColumnName("cultureTitle")
                .HasColumnType("varchar(20)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(120)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AuthorLogin)
                .IsRequired()
                .HasColumnName("authorLogin")
                .HasColumnType("varchar(50)")
                .HasDefaultValueSql("'Console'")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Flag).HasColumnName("flag");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Link)
                .HasColumnName("link")
                .HasColumnType("varchar(120)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ResourceType)
                .HasColumnName("resourceType")
                .HasColumnType("varchar(20)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TextValue)
                .HasColumnName("textValue")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TimeChanges)
                .HasColumnName("timeChanges")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        });
    }
    public static void PgSqlAddResData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResData>(entity =>
        {
            entity.HasKey(e => new { e.FileId, e.CultureTitle, e.Title })
                .HasName("res_data_pkey");

            entity.ToTable("res_data", "onlyoffice");

            entity.HasIndex(e => e.CultureTitle)
                .HasDatabaseName("resources_FK2");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("id_res_data")
                .IsUnique();

            entity.HasIndex(e => e.TimeChanges)
                .HasDatabaseName("dateIndex");

            entity.Property(e => e.FileId).HasColumnName("fileid");

            entity.Property(e => e.CultureTitle)
                .HasColumnName("cultureTitle")
                .HasMaxLength(20);

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(120);

            entity.Property(e => e.AuthorLogin)
                .IsRequired()
                .HasColumnName("authorLogin")
                .HasMaxLength(50)
                .HasDefaultValueSql("'Console'");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.Flag).HasColumnName("flag");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Link)
                .HasColumnName("link")
                .HasMaxLength(120)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.ResourceType)
                .HasColumnName("resourceType")
                .HasMaxLength(20)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TextValue).HasColumnName("textValue");

            entity.Property(e => e.TimeChanges)
                .HasColumnName("timeChanges")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
