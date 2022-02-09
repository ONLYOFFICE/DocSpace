﻿// <auto-generated />

namespace ASC.Core.Common.Migrations.MySql.VoipDbContextMySql;

[DbContext(typeof(MySqlVoipDbContext))]
partial class MySqlVoipDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("Relational:MaxIdentifierLength", 64)
            .HasAnnotation("ProductVersion", "5.0.10");

        modelBuilder.Entity("ASC.Core.Common.EF.Model.CrmContact", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasColumnName("id");

                b.Property<int>("CompanyId")
                    .HasColumnType("int")
                    .HasColumnName("company_id");

                b.Property<string>("CompanyName")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("company_name")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<int>("ContactTypeId")
                    .HasColumnType("int")
                    .HasColumnName("contact_type_id");

                b.Property<string>("CreateBy")
                    .IsRequired()
                    .HasColumnType("char(38)")
                    .HasColumnName("create_by")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<DateTime>("CreateOn")
                    .HasColumnType("datetime")
                    .HasColumnName("create_on");

                b.Property<string>("Currency")
                    .HasColumnType("varchar(3)")
                    .HasColumnName("currency")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("DisplayName")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("display_name")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("FirstName")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("first_name")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("Industry")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("industry")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<bool>("IsCompany")
                    .HasColumnType("tinyint(1)")
                    .HasColumnName("is_company");

                b.Property<bool>("IsShared")
                    .HasColumnType("tinyint(1)")
                    .HasColumnName("is_shared");

                b.Property<string>("LastModifedBy")
                    .IsRequired()
                    .HasColumnType("char(38)")
                    .HasColumnName("last_modifed_by")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<DateTime>("LastModifedOn")
                    .HasColumnType("datetime")
                    .HasColumnName("last_modifed_on");

                b.Property<string>("LastName")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("last_name")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("Notes")
                    .HasColumnType("text")
                    .HasColumnName("notes")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<int>("StatusId")
                    .HasColumnType("int")
                    .HasColumnName("status_id");

                b.Property<int>("TenantId")
                    .HasColumnType("int")
                    .HasColumnName("tenant_id");

                b.Property<string>("Title")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("title")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.HasKey("Id");

                b.HasIndex("CreateOn")
                    .HasDatabaseName("create_on");

                b.HasIndex("LastModifedOn", "TenantId")
                    .HasDatabaseName("last_modifed_on");

                b.HasIndex("TenantId", "CompanyId")
                    .HasDatabaseName("company_id");

                b.HasIndex("TenantId", "DisplayName")
                    .HasDatabaseName("display_name");

                b.ToTable("crm_contact");
            });

        modelBuilder.Entity("ASC.Core.Common.EF.Model.DbVoipCall", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("varchar(50)")
                    .HasColumnName("id")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("AnsweredBy")
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasColumnType("varchar(50)")
                    .HasColumnName("answered_by")
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<int>("ContactId")
                    .HasColumnType("int")
                    .HasColumnName("contact_id");

                b.Property<int?>("CrmContactId")
                    .HasColumnType("int");

                b.Property<DateTime>("DialDate")
                    .HasColumnType("datetime")
                    .HasColumnName("dial_date");

                b.Property<int>("DialDuration")
                    .HasColumnType("int")
                    .HasColumnName("dial_duration");

                b.Property<string>("NumberFrom")
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasColumnName("number_from")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("NumberTo")
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasColumnName("number_to")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("ParentCallId")
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasColumnName("parent_call_id")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<decimal>("Price")
                    .HasColumnType("decimal(10,4)")
                    .HasColumnName("price");

                b.Property<int>("RecordDuration")
                    .HasColumnType("int")
                    .HasColumnName("record_duration");

                b.Property<decimal>("RecordPrice")
                    .HasColumnType("decimal(10,4)")
                    .HasColumnName("record_price");

                b.Property<string>("RecordSid")
                    .HasColumnType("varchar(50)")
                    .HasColumnName("record_sid")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("RecordUrl")
                    .HasColumnType("text")
                    .HasColumnName("record_url")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<int>("Status")
                    .HasColumnType("int")
                    .HasColumnName("status");

                b.Property<int>("TenantId")
                    .HasColumnType("int")
                    .HasColumnName("tenant_id");

                b.HasKey("Id");

                b.HasIndex("CrmContactId");

                b.HasIndex("TenantId")
                    .HasDatabaseName("tenant_id");

                b.HasIndex("ParentCallId", "TenantId")
                    .HasDatabaseName("parent_call_id");

                b.ToTable("crm_voip_calls");
            });

        modelBuilder.Entity("ASC.Core.Common.EF.Model.VoipNumber", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("varchar(50)")
                    .HasColumnName("id")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("Alias")
                    .HasColumnType("varchar(255)")
                    .HasColumnName("alias")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("Number")
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasColumnName("number")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<string>("Settings")
                    .HasColumnType("text")
                    .HasColumnName("settings")
                    .UseCollation("utf8_general_ci")
                    .HasAnnotation("MySql:CharSet", "utf8");

                b.Property<int>("TenantId")
                    .HasColumnType("int")
                    .HasColumnName("tenant_id");

                b.HasKey("Id");

                b.HasIndex("TenantId")
                    .HasDatabaseName("tenant_id");

                b.ToTable("crm_voip_number");
            });

        modelBuilder.Entity("ASC.Core.Common.EF.Model.DbVoipCall", b =>
            {
                b.HasOne("ASC.Core.Common.EF.Model.CrmContact", "CrmContact")
                    .WithMany()
                    .HasForeignKey("CrmContactId");

                b.Navigation("CrmContact");
            });
#pragma warning restore 612, 618
    }
}
