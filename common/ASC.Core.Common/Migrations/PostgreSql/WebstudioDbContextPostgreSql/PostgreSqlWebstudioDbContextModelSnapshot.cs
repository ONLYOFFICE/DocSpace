// <auto-generated />

namespace ASC.Core.Common.Migrations.PostgreSql.WebstudioDbContextPostgreSql;

[DbContext(typeof(PostgreSqlWebstudioDbContext))]
partial class PostgreSqlWebstudioDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("Relational:MaxIdentifierLength", 64)
            .HasAnnotation("ProductVersion", "5.0.10");

        modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioIndex", b =>
            {
                b.Property<string>("IndexName")
                    .HasColumnType("varchar(50)")
                    .HasColumnName("index_name")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<DateTime>("LastModified")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("timestamp")
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                b.HasKey("IndexName")
                    .HasName("PRIMARY");

                b.ToTable("webstudio_index");
            });

        modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioSettings", b =>
            {
                b.Property<int>("TenantId")
                    .HasColumnType("int")
                    .HasColumnName("TenantID");

                b.Property<string>("Id")
                    .HasColumnType("varchar(64)")
                    .HasColumnName("ID")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("UserId")
                    .HasColumnType("varchar(64)")
                    .HasColumnName("UserID")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("Data")
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.HasKey("TenantId", "Id", "UserId")
                    .HasName("PRIMARY");

                b.HasIndex("Id")
                    .HasDatabaseName("ID");

                b.ToTable("webstudio_settings");

                b.HasData(
                    new
                    {
                        TenantId = 1,
                        Id = "9a925891-1f92-4ed7-b277-d6f649739f06",
                        UserId = "00000000-0000-0000-0000-000000000000",
                        Data = "{\"Completed\":false}"
                    });
            });

        modelBuilder.Entity("ASC.Core.Common.EF.Model.DbWebstudioUserVisit", b =>
            {
                b.Property<int>("TenantId")
                    .HasColumnType("int")
                    .HasColumnName("tenantid");

                b.Property<DateTime>("VisitDate")
                    .HasColumnType("datetime")
                    .HasColumnName("visitdate");

                b.Property<string>("ProductId")
                    .HasColumnType("varchar(38)")
                    .HasColumnName("productid")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("UserId")
                    .HasColumnType("varchar(38)")
                    .HasColumnName("userid")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<DateTime>("FirstVisitTime")
                    .HasColumnType("datetime")
                    .HasColumnName("firstvisittime");

                b.Property<DateTime>("LastVisitTime")
                    .HasColumnType("datetime")
                    .HasColumnName("lastvisittime");

                b.Property<int>("VisitCount")
                    .HasColumnType("int")
                    .HasColumnName("visitcount");

                b.HasKey("TenantId", "VisitDate", "ProductId", "UserId")
                    .HasName("PRIMARY");

                b.HasIndex("VisitDate")
                    .HasDatabaseName("visitdate");

                b.ToTable("webstudio_uservisit");
            });
#pragma warning restore 612, 618
    }
}
