// <auto-generated />

namespace ASC.Core.Common.Migrations.MySql.TenantDbContextMySql
{
    [DbContext(typeof(MySqlTenantDbContext))]
    partial class MySqlTenantDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbCoreSettings", b =>
                {
                    b.Property<int>("Tenant")
                        .HasColumnType("int")
                        .HasColumnName("tenant");

                    b.Property<string>("Id")
                        .HasColumnType("varchar(128)")
                        .HasColumnName("id")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp")
                        .HasColumnName("last_modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<byte[]>("Value")
                        .IsRequired()
                        .HasColumnType("mediumblob")
                        .HasColumnName("value");

                    b.HasKey("Tenant", "Id")
                        .HasName("PRIMARY");

                    b.ToTable("core_settings");

                    b.HasData(
                        new
                        {
                            Tenant = -1,
                            Id = "CompanyWhiteLabelSettings",
                            LastModified = new DateTime(2021, 10, 26, 12, 11, 31, 504, DateTimeKind.Utc).AddTicks(1420),
                            Value = new byte[] { 245, 71, 4, 138, 72, 101, 23, 21, 135, 217, 206, 188, 138, 73, 108, 96, 29, 150, 3, 31, 44, 28, 62, 145, 96, 53, 57, 66, 238, 118, 93, 172, 211, 22, 244, 181, 244, 40, 146, 67, 111, 196, 162, 27, 154, 109, 248, 255, 211, 188, 64, 54, 180, 126, 58, 90, 27, 76, 136, 27, 38, 96, 152, 105, 254, 187, 104, 72, 189, 136, 192, 46, 234, 198, 164, 204, 179, 232, 244, 4, 41, 8, 18, 240, 230, 225, 36, 165, 82, 190, 129, 165, 140, 100, 187, 139, 211, 201, 168, 192, 237, 225, 249, 66, 18, 129, 222, 12, 122, 248, 39, 51, 164, 188, 229, 21, 232, 86, 148, 196, 221, 167, 142, 34, 101, 43, 162, 137, 31, 206, 149, 120, 249, 114, 133, 168, 30, 18, 254, 223, 93, 101, 88, 97, 30, 58, 163, 224, 62, 173, 220, 170, 152, 40, 124, 100, 165, 81, 7, 87, 168, 129, 176, 12, 51, 69, 230, 252, 30, 34, 182, 7, 202, 45, 117, 60, 99, 241, 237, 148, 201, 35, 102, 219, 160, 228, 194, 230, 219, 22, 244, 74, 138, 176, 145, 0, 122, 167, 80, 93, 23, 228, 21, 48, 100, 60, 31, 250, 232, 34, 248, 249, 159, 210, 227, 12, 13, 239, 130, 223, 101, 196, 51, 36, 80, 127, 62, 92, 104, 228, 197, 226, 43, 232, 164, 12, 36, 66, 52, 133 }
                        },
                        new
                        {
                            Tenant = -1,
                            Id = "FullTextSearchSettings",
                            LastModified = new DateTime(2021, 10, 26, 12, 11, 31, 504, DateTimeKind.Utc).AddTicks(1630),
                            Value = new byte[] { 8, 120, 207, 5, 153, 181, 23, 202, 162, 211, 218, 237, 157, 6, 76, 62, 220, 238, 175, 67, 31, 53, 166, 246, 66, 220, 173, 160, 72, 23, 227, 81, 50, 39, 187, 177, 222, 110, 43, 171, 235, 158, 16, 119, 178, 207, 49, 140, 72, 152, 20, 84, 94, 135, 117, 1, 246, 51, 251, 190, 148, 2, 44, 252, 221, 2, 91, 83, 149, 151, 58, 245, 16, 148, 52, 8, 187, 86, 150, 46, 227, 93, 163, 95, 47, 131, 116, 207, 95, 209, 38, 149, 53, 148, 73, 215, 206, 251, 194, 199, 189, 17, 42, 229, 135, 82, 23, 154, 162, 165, 158, 94, 23, 128, 30, 88, 12, 204, 96, 250, 236, 142, 189, 211, 214, 18, 196, 136, 102, 102, 217, 109, 108, 240, 96, 96, 94, 100, 201, 10, 31, 170, 128, 192 }
                        },
                        new
                        {
                            Tenant = -1,
                            Id = "SmtpSettings",
                            LastModified = new DateTime(2021, 10, 26, 12, 11, 31, 504, DateTimeKind.Utc).AddTicks(1635),
                            Value = new byte[] { 240, 82, 224, 144, 161, 163, 117, 13, 173, 205, 78, 153, 97, 218, 4, 170, 81, 239, 1, 151, 226, 192, 98, 60, 241, 44, 88, 56, 191, 164, 10, 155, 72, 186, 239, 203, 227, 113, 88, 119, 49, 215, 227, 220, 158, 124, 96, 9, 116, 47, 158, 65, 93, 86, 219, 15, 10, 224, 142, 50, 248, 144, 75, 44, 68, 28, 198, 87, 198, 69, 67, 234, 238, 38, 32, 68, 162, 139, 67, 53, 220, 176, 240, 196, 233, 64, 29, 137, 31, 160, 99, 105, 249, 132, 202, 45, 71, 92, 134, 194, 55, 145, 121, 97, 197, 130, 119, 105, 131, 21, 133, 35, 10, 102, 172, 119, 135, 230, 251, 86, 253, 62, 55, 56, 146, 103, 164, 106 }
                        });
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbTenant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("alias")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<bool>("Calls")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("calls")
                        .HasDefaultValueSql("true");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime")
                        .HasColumnName("creationdatetime");

                    b.Property<int?>("Industry")
                        .HasColumnType("int")
                        .HasColumnName("industry");

                    b.Property<string>("Language")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(10)")
                        .HasColumnName("language")
                        .HasDefaultValueSql("'en-US'")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<DateTime>("LastModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("last_modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("MappedDomain")
                        .HasColumnType("varchar(100)")
                        .HasColumnName("mappeddomain")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("varchar(38)")
                        .HasColumnName("owner_id")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("PaymentId")
                        .HasColumnType("varchar(38)")
                        .HasColumnName("payment_id")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<bool>("Spam")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("spam")
                        .HasDefaultValueSql("true");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<DateTime?>("StatusChanged")
                        .HasColumnType("datetime")
                        .HasColumnName("statuschanged");

                    b.Property<string>("TimeZone")
                        .HasColumnType("varchar(50)")
                        .HasColumnName("timezone")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("TrustedDomains")
                        .HasColumnType("varchar(1024)")
                        .HasColumnName("trusteddomains")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<int>("TrustedDomainsEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("trusteddomainsenabled")
                        .HasDefaultValueSql("'1'");

                    b.Property<int>("Version")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("version")
                        .HasDefaultValueSql("'2'");

                    b.Property<DateTime?>("Version_Changed")
                        .HasColumnType("datetime")
                        .HasColumnName("version_changed");

                    b.HasKey("Id");

                    b.HasIndex("LastModified")
                        .HasDatabaseName("last_modified");

                    b.HasIndex("MappedDomain")
                        .HasDatabaseName("mappeddomain");

                    b.HasIndex("Version")
                        .HasDatabaseName("version");

                    b.ToTable("tenants_tenants");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Alias = "localhost",
                            Calls = false,
                            CreationDateTime = new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317),
                            LastModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Web Office",
                            OwnerId = "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
                            Spam = false,
                            Status = 0,
                            TrustedDomainsEnabled = 0,
                            Version = 0
                        });
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbTenantForbiden", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("varchar(50)")
                        .HasColumnName("address")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.HasKey("Address")
                        .HasName("PRIMARY");

                    b.ToTable("tenants_forbiden");

                    b.HasData(
                        new
                        {
                            Address = "controlpanel"
                        },
                        new
                        {
                            Address = "localhost"
                        });
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbTenantPartner", b =>
                {
                    b.Property<int>("TenantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("tenant_id");

                    b.Property<string>("AffiliateId")
                        .HasColumnType("varchar(50)")
                        .HasColumnName("affiliate_id")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("Campaign")
                        .HasColumnType("varchar(50)")
                        .HasColumnName("campaign")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("PartnerId")
                        .HasColumnType("varchar(36)")
                        .HasColumnName("partner_id")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.HasKey("TenantId")
                        .HasName("PRIMARY");

                    b.ToTable("tenants_partners");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.DbTenantVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int>("DefaultVersion")
                        .HasColumnType("int")
                        .HasColumnName("default_version");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasColumnName("url")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasColumnName("version")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<bool>("Visible")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("visible");

                    b.HasKey("Id");

                    b.ToTable("tenants_version");
                });

            modelBuilder.Entity("ASC.Core.Common.EF.Model.TenantIpRestrictions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("ip")
                        .UseCollation("utf8_general_ci")
                        .HasAnnotation("MySql:CharSet", "utf8");

                    b.Property<int>("Tenant")
                        .HasColumnType("int")
                        .HasColumnName("tenant");

                    b.HasKey("Id");

                    b.HasIndex("Tenant")
                        .HasDatabaseName("tenant");

                    b.ToTable("tenants_iprestrictions");
                });
#pragma warning restore 612, 618
        }
    }
}
