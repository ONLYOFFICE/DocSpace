using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.TenantDbContextNpgsql
{
    public partial class TenantDbContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "core_settings",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    id = table.Column<string>(maxLength: 128, nullable: false),
                    value = table.Column<byte[]>(nullable: false),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_settings_pkey", x => new { x.tenant, x.id });
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_settings",
                columns: new[] { "tenant", "id", "last_modified", "value" },
                values: new object[,]
                {
                    { -1, "CompanyWhiteLabelSettings", new DateTime(2020, 10, 1, 16, 42, 12, 312, DateTimeKind.Utc).AddTicks(4730), new byte[] { 48, 120, 70, 53, 52, 55, 48, 52, 56, 65, 52, 56, 54, 53, 49, 55, 49, 53, 56, 55, 68, 57, 67, 69, 66, 67, 56, 65, 52, 57, 54, 67, 54, 48, 49, 68, 57, 54, 48, 51, 49, 70, 50, 67, 49, 67, 51, 69, 57, 49, 54, 48, 51, 53, 51, 57, 52, 50, 69, 69, 55, 54, 53, 68, 65, 67, 68, 51, 49, 54, 70, 52, 66, 53, 70, 52, 50, 56, 57, 50, 52, 51, 54, 70, 67, 52, 65, 50, 49, 66, 57, 65, 54, 68, 70, 56, 70, 70, 68, 51, 66, 67, 52, 48, 51, 54, 66, 52, 55, 69, 51, 65, 53, 65, 49, 66, 52, 67, 56, 56, 49, 66, 50, 54, 54, 48, 57, 56, 54, 57, 70, 69, 66, 66, 54, 56, 52, 56, 66, 68, 56, 56, 67, 48, 50, 69, 69, 65, 67, 54, 65, 52, 67, 67, 66, 51, 69, 56, 70, 52, 48, 52, 50, 57, 48, 56, 49, 50, 70, 48, 69, 54, 69, 49, 50, 52, 65, 53, 53, 50, 66, 69, 56, 49, 65, 53, 56, 67, 54, 52, 66, 66, 56, 66, 68, 51, 67, 57, 65, 56, 67, 48, 69, 68, 69, 49, 70, 57, 52, 50, 49, 50, 56, 49, 68, 69, 48, 67, 55, 65, 70, 56, 50, 55, 51, 51, 67, 48, 66, 55, 53, 52, 69, 57, 55, 69, 70, 70, 70, 65, 53, 65, 55, 53, 54, 48, 55, 65, 57, 49, 57, 53, 55, 56, 57, 54, 67, 66, 69, 67, 70, 57, 53, 54, 51, 70, 67, 56, 51, 49, 51, 48, 48, 68, 67, 56, 69, 55, 67, 57, 51, 48, 65, 53, 53, 66, 50, 57, 56, 69, 66, 56, 50, 68, 54, 70, 54, 57, 69, 48, 69, 68, 54, 69, 52, 68, 56, 55, 53, 50, 54, 48, 55, 70, 49, 56, 56, 49, 70, 54, 49, 66, 48, 51, 50, 51, 48, 54, 69, 48, 70, 48, 54, 57, 65, 53, 70, 54, 57, 70, 48, 56, 54, 65, 49, 55, 55, 69, 66, 52, 49, 65, 67, 48, 54, 70, 56, 56, 57, 69, 66, 48, 66, 51, 57, 67, 66, 70, 68, 52, 66, 53, 67, 68, 66, 55, 54, 51, 69, 57, 57, 54, 53, 53, 52, 68, 69, 65, 68, 66, 57, 67, 55, 49, 67, 70, 51, 69, 70, 56, 54, 70, 52, 65, 48, 51, 53, 52, 65, 56, 54, 52, 65, 49, 48, 54, 51, 57, 68, 70, 68, 50, 57, 66, 53, 67, 54, 68, 53, 68, 67, 68, 65, 57, 68, 52, 66, 48, 57, 56, 56, 69, 69, 52, 48, 54, 57, 52, 56, 66, 67, 66, 53, 52, 67, 54, 65, 55, 48, 65, 68, 67, 54, 67, 48, 48, 53, 55, 55, 49, 55, 52, 50, 56, 53, 67, 69, 66, 67, 68, 55, 54 } },
                    { -1, "FullTextSearchSettings", new DateTime(2020, 10, 1, 16, 42, 12, 312, DateTimeKind.Utc).AddTicks(5433), new byte[] { 48, 120, 48, 56, 55, 56, 67, 70, 48, 53, 57, 57, 66, 53, 49, 55, 67, 65, 65, 50, 68, 51, 68, 65, 69, 68, 57, 68, 48, 54, 52, 67, 51, 69, 68, 67, 69, 69, 65, 70, 52, 51, 49, 70, 51, 53, 65, 54, 70, 54, 52, 50, 68, 67, 65, 68, 65, 48, 52, 56, 49, 55, 69, 51, 53, 49, 51, 50, 50, 55, 66, 66, 66, 49, 68, 69, 54, 69, 50, 66, 65, 66, 69, 66, 57, 69, 49, 48, 55, 55, 66, 50, 67, 70, 51, 49, 56, 67, 52, 56, 57, 56, 49, 52, 53, 52, 53, 69, 56, 55, 55, 53, 48, 49, 70, 54, 51, 51, 70, 66, 66, 69, 57, 52, 48, 50, 50, 67, 70, 67, 68, 68, 48, 50, 53, 66, 53, 51, 57, 53, 57, 55, 51, 65, 70, 53, 49, 48, 57, 52, 51, 52, 48, 56, 66, 66, 53, 54, 57, 54, 50, 69, 69, 51, 53, 68, 65, 51, 53, 70, 50, 70, 56, 51, 55, 52, 67, 70, 53, 70, 68, 49, 50, 54, 57, 53, 51, 53, 57, 52, 52, 57, 68, 55, 67, 69, 70, 66, 67, 50, 67, 55, 66, 68, 49, 49, 50, 65, 69, 53, 56, 55, 53, 50, 49, 55, 57, 65, 65, 50, 65, 53, 57, 69, 53, 69, 49, 55, 56, 48, 49, 69, 53, 56, 48, 67, 67, 67, 54, 48, 70, 65, 69, 67, 56, 69, 66, 68, 68, 51, 68, 54, 49, 50, 67, 52, 56, 56, 54, 54, 54, 54, 68, 57, 54, 68, 54, 67, 70, 48, 54, 48, 54, 48, 53, 69, 54, 52, 67, 57, 48, 65, 49, 70, 65, 65, 56, 48, 67, 48 } },
                    { -1, "SmtpSettings", new DateTime(2020, 10, 1, 16, 42, 12, 312, DateTimeKind.Utc).AddTicks(5448), new byte[] { 48, 120, 70, 48, 53, 50, 69, 48, 57, 48, 65, 49, 65, 51, 55, 53, 48, 68, 65, 68, 67, 68, 52, 69, 57, 57, 54, 49, 68, 65, 48, 52, 65, 65, 53, 49, 69, 70, 48, 49, 57, 55, 69, 50, 67, 48, 54, 50, 51, 67, 70, 49, 50, 67, 53, 56, 51, 56, 66, 70, 65, 52, 48, 65, 57, 66, 52, 56, 66, 65, 69, 70, 67, 66, 69, 51, 55, 49, 53, 56, 55, 55, 51, 49, 68, 55, 69, 51, 68, 67, 57, 69, 55, 67, 54, 48, 48, 57, 55, 52, 50, 70, 57, 69, 52, 49, 53, 68, 53, 54, 68, 66, 48, 70, 48, 65, 69, 48, 56, 69, 51, 50, 70, 56, 57, 48, 52, 66, 50, 67, 52, 52, 49, 67, 67, 54, 53, 55, 67, 54, 52, 53, 52, 51, 69, 65, 69, 69, 50, 54, 50, 48, 52, 52, 65, 50, 56, 66, 52, 51, 51, 53, 68, 67, 66, 48, 70, 48, 67, 52, 69, 57, 52, 48, 49, 68, 56, 57, 49, 70, 65, 48, 54, 51, 54, 57, 70, 57, 56, 52, 67, 65, 50, 68, 52, 55, 53, 67, 56, 54, 67, 50, 51, 55, 57, 49, 55, 57, 54, 49, 67, 53, 56, 50, 55, 55, 54, 57, 56, 51, 49, 53, 56, 53, 50, 51, 48, 65, 54, 54, 65, 67, 55, 55, 56, 55, 69, 54, 70, 66, 53, 54, 70, 68, 51, 69, 51, 55, 51, 56, 57, 50, 54, 55, 65, 52, 54, 65 } }
                });

            migrationBuilder.CreateTable(
                name: "core_user",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<Guid>(maxLength: 38, nullable: false),
                    tenant = table.Column<int>(nullable: false),
                    username = table.Column<string>(maxLength: 255, nullable: false),
                    firstname = table.Column<string>(maxLength: 64, nullable: false),
                    lastname = table.Column<string>(maxLength: 64, nullable: false),
                    sex = table.Column<bool>(nullable: true),
                    bithdate = table.Column<DateTime>(nullable: true),
                    status = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    activation_status = table.Column<int>(nullable: false),
                    email = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    workfromdate = table.Column<DateTime>(nullable: true),
                    terminateddate = table.Column<DateTime>(nullable: true),
                    title = table.Column<string>(maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    culture = table.Column<string>(maxLength: 20, nullable: true, defaultValueSql: "NULL"),
                    contacts = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    phone = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    phone_activation = table.Column<int>(nullable: false),
                    location = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    notes = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sid = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sso_name_id = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sso_session_id = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    removed = table.Column<bool>(nullable: false),
                    create_on = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_user", x => x.id);
                });

            migrationBuilder.InsertData(
               schema: "onlyoffice",
               table: "core_user",
               columns: new[] { "id", "firstname", "lastname", "username", "tenant", "email", "workfromdate", "last_modified", "status", "activation_status", "phone_activation", "removed" },
               values: new object[] { new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), "Administrator", "", "administrator", 1, " ", new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), 1, 0, 0, false });

            migrationBuilder.CreateTable(
                name: "tenants_forbiden",
                schema: "onlyoffice",
                columns: table => new
                {
                    address = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_forbiden_pkey", x => x.address);
                });

            migrationBuilder.CreateTable(
                name: "tenants_iprestrictions",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(nullable: false),
                    ip = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_iprestrictions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 255, nullable: false),
                    alias = table.Column<string>(maxLength: 100, nullable: false),
                    mappeddomain = table.Column<string>(maxLength: 100, nullable: true, defaultValueSql: "NULL"),
                    version = table.Column<int>(nullable: false, defaultValueSql: "2"),
                    Version_Changed = table.Column<DateTime>(nullable: true),
                    version_changed = table.Column<DateTime>(nullable: false),
                    language = table.Column<string>(fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "'en-US'"),
                    timezone = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    trusteddomains = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    trusteddomainsenabled = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    status = table.Column<int>(nullable: false),
                    statuschanged = table.Column<DateTime>(nullable: true),
                    creationdatetime = table.Column<DateTime>(nullable: false),
                    owner_id = table.Column<Guid>(maxLength: 38, nullable: false, defaultValueSql: "NULL"),
                    @public = table.Column<bool>(name: "public", nullable: false),
                    publicvisibleproducts = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    payment_id = table.Column<string>(maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    industry = table.Column<int>(nullable: true),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_version",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    version = table.Column<string>(maxLength: 64, nullable: false),
                    url = table.Column<string>(maxLength: 64, nullable: false),
                    default_version = table.Column<int>(nullable: false),
                    visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_version", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "core_usergroup",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    userid = table.Column<Guid>(maxLength: 38, nullable: false),
                    groupid = table.Column<Guid>(maxLength: 38, nullable: false),
                    ref_type = table.Column<int>(nullable: false),
                    removed = table.Column<bool>(nullable: false),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_usergroup_pkey", x => new { x.tenant, x.userid, x.groupid, x.ref_type });

                });
            migrationBuilder.InsertData(
               schema: "onlyoffice",
               table: "core_usergroup",
               columns: new[] { "tenant", "userid", "groupid", "ref_type", "last_modified", "removed" },
               values: new object[] { 1, new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), "cd84e66b-b803-40fc-99f9-b2969a54a1de", 0, new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), false });

            migrationBuilder.CreateTable(
                name: "core_usersecurity",
                schema: "onlyoffice",
                columns: table => new
                {
                    userid = table.Column<Guid>(maxLength: 38, nullable: false),
                    tenant = table.Column<int>(nullable: false),
                    pwdhash = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    pwdhashsha512 = table.Column<string>(maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    LastModified = table.Column<DateTime>(nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_usersecurity_pkey", x => x.userid);
                });

            migrationBuilder.InsertData(
               schema: "onlyoffice",
               table: "core_usersecurity",
               columns: new[] { "userid", "tenant", "pwdhash", "pwdhashsha512", "LastModified" },
               values: new object[] { new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 1, "vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=", "USubvPlB+ogq0Q1trcSupg==", new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218) });


            migrationBuilder.CreateTable(
                name: "tenants_partners",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    partner_id = table.Column<string>(maxLength: 36, nullable: true, defaultValueSql: "NULL"),
                    affiliate_id = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    campaign = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_partners_pkey", x => x.tenant_id);

                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_forbiden",
                column: "address",
                values: new object[]
                {
                    "controlpanel",
                    "localhost"
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "name", "owner_id", "public", "status", "statuschanged", "version_changed", "Version_Changed" },
                values: new object[] { 1, "localhost", new DateTime(2020, 9, 29, 10, 46, 46, 424, DateTimeKind.Utc).AddTicks(6218), null, "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), false, 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.CreateIndex(
                name: "email",
                schema: "onlyoffice",
                table: "core_user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "last_modified_core_user",
                schema: "onlyoffice",
                table: "core_user",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "username",
                schema: "onlyoffice",
                table: "core_user",
                columns: new[] { "username", "tenant" });

            migrationBuilder.CreateIndex(
                name: "last_modified_core_usergroup",
                schema: "onlyoffice",
                table: "core_usergroup",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "IX_core_usergroup_userid",
                schema: "onlyoffice",
                table: "core_usergroup",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "pwdhash",
                schema: "onlyoffice",
                table: "core_usersecurity",
                column: "pwdhash");

            migrationBuilder.CreateIndex(
                name: "tenant_core_usersecurity",
                schema: "onlyoffice",
                table: "core_usersecurity",
                column: "tenant");

            migrationBuilder.CreateIndex(
                name: "tenant_tenants_iprestrictions",
                schema: "onlyoffice",
                table: "tenants_iprestrictions",
                column: "tenant");

            migrationBuilder.CreateIndex(
                name: "alias",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "last_modified_tenants_tenants",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "mappeddomain",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "mappeddomain");

            migrationBuilder.CreateIndex(
                name: "version",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "core_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_usergroup",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_usersecurity",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_forbiden",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_iprestrictions",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_partners",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_version",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_user",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
