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
                    { -1, "CompanyWhiteLabelSettings", new DateTime(2020, 10, 1, 16, 42, 12, 312, DateTimeKind.Utc).AddTicks(4730), new byte[] { 245, 71, 4, 138, 72, 101, 23, 21, 135, 217, 206, 188, 138, 73, 108, 96, 29, 150, 3, 31, 44, 28, 62, 145, 96, 53, 57, 66, 238, 118, 93, 172, 211, 22, 244, 181, 244, 40, 146, 67, 111, 196, 162, 27, 154, 109, 248, 255, 211, 188, 64, 54, 180, 126, 58, 90, 27, 76, 136, 27, 38, 96, 152, 105, 254, 187, 104, 72, 189, 136, 192, 46, 234, 198, 164, 204, 179, 232, 244, 4, 41, 8, 18, 240, 230, 225, 36, 165, 82, 190, 129, 165, 140, 100, 187, 139, 211, 201, 168, 192, 237, 225, 249, 66, 18, 129, 222, 12, 122, 248, 39, 51, 164, 188, 229, 21, 232, 86, 148, 196, 221, 167, 142, 34, 101, 43, 162, 137, 31, 206, 149, 120, 249, 114, 133, 168, 30, 18, 254, 223, 93, 101, 88, 97, 30, 58, 163, 224, 62, 173, 220, 170, 152, 40, 124, 100, 165, 81, 7, 87, 168, 129, 176, 12, 51, 69, 230, 252, 30, 34, 182, 7, 202, 45, 117, 60, 99, 241, 237, 148, 201, 35, 102, 219, 160, 228, 194, 230, 219, 22, 244, 74, 138, 176, 145, 0, 122, 167, 80, 93, 23, 228, 21, 48, 100, 60, 31, 250, 232, 34, 248, 249, 159, 210, 227, 12, 13, 239, 130, 223, 101, 196, 51, 36, 80, 127, 62, 92, 104, 228, 197, 226, 43, 232, 164, 12, 36, 66, 52, 133 } },
                    { -1, "FullTextSearchSettings", new DateTime(2020, 10, 1, 16, 42, 12, 312, DateTimeKind.Utc).AddTicks(5433), new byte[] { 8, 120, 207, 5, 153, 181, 23, 202, 162, 211, 218, 237, 157, 6, 76, 62, 220, 238, 175, 67, 31, 53, 166, 246, 66, 220, 173, 160, 72, 23, 227, 81, 50, 39, 187, 177, 222, 110, 43, 171, 235, 158, 16, 119, 178, 207, 49, 140, 72, 152, 20, 84, 94, 135, 117, 1, 246, 51, 251, 190, 148, 2, 44, 252, 221, 2, 91, 83, 149, 151, 58, 245, 16, 148, 52, 8, 187, 86, 150, 46, 227, 93, 163, 95, 47, 131, 116, 207, 95, 209, 38, 149, 53, 148, 73, 215, 206, 251, 194, 199, 189, 17, 42, 229, 135, 82, 23, 154, 162, 165, 158, 94, 23, 128, 30, 88, 12, 204, 96, 250, 236, 142, 189, 211, 214, 18, 196, 136, 102, 102, 217, 109, 108, 240, 96, 96, 94, 100, 201, 10, 31, 170, 128, 192 } },
                    { -1, "SmtpSettings", new DateTime(2020, 10, 1, 16, 42, 12, 312, DateTimeKind.Utc).AddTicks(5448), new byte[] { 240, 82, 224, 144, 161, 163, 117, 13, 173, 205, 78, 153, 97, 218, 4, 170, 81, 239, 1, 151, 226, 192, 98, 60, 241, 44, 88, 56, 191, 164, 10, 155, 72, 186, 239, 203, 227, 113, 88, 119, 49, 215, 227, 220, 158, 124, 96, 9, 116, 47, 158, 65, 93, 86, 219, 15, 10, 224, 142, 50, 248, 144, 75, 44, 68, 28, 198, 87, 198, 69, 67, 234, 238, 38, 32, 68, 162, 139, 67, 53, 220, 176, 240, 196, 233, 64, 29, 137, 31, 160, 99, 105, 249, 132, 202, 45, 71, 92, 134, 194, 55, 145, 121, 97, 197, 130, 119, 105, 131, 21, 133, 35, 10, 102, 172, 119, 135, 230, 251, 86, 253, 62, 55, 56, 146, 103, 164, 106 } }
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
