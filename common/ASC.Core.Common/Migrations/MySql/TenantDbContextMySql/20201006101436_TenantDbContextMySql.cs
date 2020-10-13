using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.TenantDbContextMySql
{
    public partial class TenantDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "core_settings",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    id = table.Column<string>(type: "varchar(128)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    value = table.Column<byte[]>(type: "mediumblob", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.id });
                });

            migrationBuilder.CreateTable(
                name: "core_user",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant = table.Column<int>(nullable: false),
                    username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    firstname = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    lastname = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    sex = table.Column<bool>(nullable: true),
                    bithdate = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<int>(nullable: false, defaultValueSql: "'1'"),
                    activation_status = table.Column<int>(nullable: false),
                    email = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    workfromdate = table.Column<DateTime>(type: "datetime", nullable: true),
                    terminateddate = table.Column<DateTime>(type: "datetime", nullable: true),
                    title = table.Column<string>(type: "varchar(64)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    culture = table.Column<string>(type: "varchar(20)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    contacts = table.Column<string>(type: "varchar(1024)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    phone = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    phone_activation = table.Column<int>(nullable: false),
                    location = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    notes = table.Column<string>(type: "varchar(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    sid = table.Column<string>(type: "varchar(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    sso_name_id = table.Column<string>(type: "varchar(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    sso_session_id = table.Column<string>(type: "varchar(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    removed = table.Column<bool>(nullable: false),
                    create_on = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_modified = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_forbiden",
                columns: table => new
                {
                    address = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.address);
                });

            migrationBuilder.CreateTable(
                name: "tenants_iprestrictions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant = table.Column<int>(nullable: false),
                    ip = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_iprestrictions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_version",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    version = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    url = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    default_version = table.Column<int>(nullable: false),
                    visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_version", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    alias = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    mappeddomain = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    version = table.Column<int>(nullable: false, defaultValueSql: "'2'"),
                    version_changed = table.Column<DateTime>(type: "datetime", nullable: false),
                    language = table.Column<string>(type: "char(10)", nullable: false, defaultValueSql: "'en-US'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    timezone = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    trusteddomains = table.Column<string>(type: "varchar(1024)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    trusteddomainsenabled = table.Column<int>(nullable: false, defaultValueSql: "'1'"),
                    status = table.Column<int>(nullable: false),
                    statuschanged = table.Column<DateTime>(type: "datetime", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    owner_id = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    @public = table.Column<bool>(name: "public", nullable: false),
                    publicvisibleproducts = table.Column<string>(type: "varchar(1024)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    payment_id = table.Column<string>(type: "varchar(38)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    industry = table.Column<int>(nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "core_usergroup",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    userid = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    groupid = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    ref_type = table.Column<int>(nullable: false),
                    removed = table.Column<bool>(nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.userid, x.groupid, x.ref_type });
                });

            migrationBuilder.CreateTable(
                name: "core_usersecurity",
                columns: table => new
                {
                    userid = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant = table.Column<int>(nullable: false),
                    pwdhash = table.Column<string>(type: "varchar(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    pwdhashsha512 = table.Column<string>(type: "varchar(512)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    LastModified = table.Column<DateTime>(type: "timestamp", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "tenants_partners",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    partner_id = table.Column<string>(type: "varchar(36)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    affiliate_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    campaign = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant_id);
                });

            migrationBuilder.InsertData(
                table: "core_settings",
                columns: new[] { "tenant", "id", "value" },
                values: new object[,]
                {
                    { -1, "CompanyWhiteLabelSettings", new byte[] { 48, 120, 70, 53, 52, 55, 48, 52, 56, 65, 52, 56, 54, 53, 49, 55, 49, 53, 56, 55, 68, 57, 67, 69, 66, 67, 56, 65, 52, 57, 54, 67, 54, 48, 49, 68, 57, 54, 48, 51, 49, 70, 50, 67, 49, 67, 51, 69, 57, 49, 54, 48, 51, 53, 51, 57, 52, 50, 69, 69, 55, 54, 53, 68, 65, 67, 68, 51, 49, 54, 70, 52, 66, 53, 70, 52, 50, 56, 57, 50, 52, 51, 54, 70, 67, 52, 65, 50, 49, 66, 57, 65, 54, 68, 70, 56, 70, 70, 68, 51, 66, 67, 52, 48, 51, 54, 66, 52, 55, 69, 51, 65, 53, 65, 49, 66, 52, 67, 56, 56, 49, 66, 50, 54, 54, 48, 57, 56, 54, 57, 70, 69, 66, 66, 54, 56, 52, 56, 66, 68, 56, 56, 67, 48, 50, 69, 69, 65, 67, 54, 65, 52, 67, 67, 66, 51, 69, 56, 70, 52, 48, 52, 50, 57, 48, 56, 49, 50, 70, 48, 69, 54, 69, 49, 50, 52, 65, 53, 53, 50, 66, 69, 56, 49, 65, 53, 56, 67, 54, 52, 66, 66, 56, 66, 68, 51, 67, 57, 65, 56, 67, 48, 69, 68, 69, 49, 70, 57, 52, 50, 49, 50, 56, 49, 68, 69, 48, 67, 55, 65, 70, 56, 50, 55, 51, 51, 67, 48, 66, 55, 53, 52, 69, 57, 55, 69, 70, 70, 70, 65, 53, 65, 55, 53, 54, 48, 55, 65, 57, 49, 57, 53, 55, 56, 57, 54, 67, 66, 69, 67, 70, 57, 53, 54, 51, 70, 67, 56, 51, 49, 51, 48, 48, 68, 67, 56, 69, 55, 67, 57, 51, 48, 65, 53, 53, 66, 50, 57, 56, 69, 66, 56, 50, 68, 54, 70, 54, 57, 69, 48, 69, 68, 54, 69, 52, 68, 56, 55, 53, 50, 54, 48, 55, 70, 49, 56, 56, 49, 70, 54, 49, 66, 48, 51, 50, 51, 48, 54, 69, 48, 70, 48, 54, 57, 65, 53, 70, 54, 57, 70, 48, 56, 54, 65, 49, 55, 55, 69, 66, 52, 49, 65, 67, 48, 54, 70, 56, 56, 57, 69, 66, 48, 66, 51, 57, 67, 66, 70, 68, 52, 66, 53, 67, 68, 66, 55, 54, 51, 69, 57, 57, 54, 53, 53, 52, 68, 69, 65, 68, 66, 57, 67, 55, 49, 67, 70, 51, 69, 70, 56, 54, 70, 52, 65, 48, 51, 53, 52, 65, 56, 54, 52, 65, 49, 48, 54, 51, 57, 68, 70, 68, 50, 57, 66, 53, 67, 54, 68, 53, 68, 67, 68, 65, 57, 68, 52, 66, 48, 57, 56, 56, 69, 69, 52, 48, 54, 57, 52, 56, 66, 67, 66, 53, 52, 67, 54, 65, 55, 48, 65, 68, 67, 54, 67, 48, 48, 53, 55, 55, 49, 55, 52, 50, 56, 53, 67, 69, 66, 67, 68, 55, 54 } },
                    { -1, "FullTextSearchSettings", new byte[] { 48, 120, 48, 56, 55, 56, 67, 70, 48, 53, 57, 57, 66, 53, 49, 55, 67, 65, 65, 50, 68, 51, 68, 65, 69, 68, 57, 68, 48, 54, 52, 67, 51, 69, 68, 67, 69, 69, 65, 70, 52, 51, 49, 70, 51, 53, 65, 54, 70, 54, 52, 50, 68, 67, 65, 68, 65, 48, 52, 56, 49, 55, 69, 51, 53, 49, 51, 50, 50, 55, 66, 66, 66, 49, 68, 69, 54, 69, 50, 66, 65, 66, 69, 66, 57, 69, 49, 48, 55, 55, 66, 50, 67, 70, 51, 49, 56, 67, 52, 56, 57, 56, 49, 52, 53, 52, 53, 69, 56, 55, 55, 53, 48, 49, 70, 54, 51, 51, 70, 66, 66, 69, 57, 52, 48, 50, 50, 67, 70, 67, 68, 68, 48, 50, 53, 66, 53, 51, 57, 53, 57, 55, 51, 65, 70, 53, 49, 48, 57, 52, 51, 52, 48, 56, 66, 66, 53, 54, 57, 54, 50, 69, 69, 51, 53, 68, 65, 51, 53, 70, 50, 70, 56, 51, 55, 52, 67, 70, 53, 70, 68, 49, 50, 54, 57, 53, 51, 53, 57, 52, 52, 57, 68, 55, 67, 69, 70, 66, 67, 50, 67, 55, 66, 68, 49, 49, 50, 65, 69, 53, 56, 55, 53, 50, 49, 55, 57, 65, 65, 50, 65, 53, 57, 69, 53, 69, 49, 55, 56, 48, 49, 69, 53, 56, 48, 67, 67, 67, 54, 48, 70, 65, 69, 67, 56, 69, 66, 68, 68, 51, 68, 54, 49, 50, 67, 52, 56, 56, 54, 54, 54, 54, 68, 57, 54, 68, 54, 67, 70, 48, 54, 48, 54, 48, 53, 69, 54, 52, 67, 57, 48, 65, 49, 70, 65, 65, 56, 48, 67, 48 } },
                    { -1, "SmtpSettings", new byte[] { 48, 120, 70, 48, 53, 50, 69, 48, 57, 48, 65, 49, 65, 51, 55, 53, 48, 68, 65, 68, 67, 68, 52, 69, 57, 57, 54, 49, 68, 65, 48, 52, 65, 65, 53, 49, 69, 70, 48, 49, 57, 55, 69, 50, 67, 48, 54, 50, 51, 67, 70, 49, 50, 67, 53, 56, 51, 56, 66, 70, 65, 52, 48, 65, 57, 66, 52, 56, 66, 65, 69, 70, 67, 66, 69, 51, 55, 49, 53, 56, 55, 55, 51, 49, 68, 55, 69, 51, 68, 67, 57, 69, 55, 67, 54, 48, 48, 57, 55, 52, 50, 70, 57, 69, 52, 49, 53, 68, 53, 54, 68, 66, 48, 70, 48, 65, 69, 48, 56, 69, 51, 50, 70, 56, 57, 48, 52, 66, 50, 67, 52, 52, 49, 67, 67, 54, 53, 55, 67, 54, 52, 53, 52, 51, 69, 65, 69, 69, 50, 54, 50, 48, 52, 52, 65, 50, 56, 66, 52, 51, 51, 53, 68, 67, 66, 48, 70, 48, 67, 52, 69, 57, 52, 48, 49, 68, 56, 57, 49, 70, 65, 48, 54, 51, 54, 57, 70, 57, 56, 52, 67, 65, 50, 68, 52, 55, 53, 67, 56, 54, 67, 50, 51, 55, 57, 49, 55, 57, 54, 49, 67, 53, 56, 50, 55, 55, 54, 57, 56, 51, 49, 53, 56, 53, 50, 51, 48, 65, 54, 54, 65, 67, 55, 55, 56, 55, 69, 54, 70, 66, 53, 54, 70, 68, 51, 69, 51, 55, 51, 56, 57, 50, 54, 55, 65, 52, 54, 65 } }
                });

            migrationBuilder.InsertData(
                table: "core_user",
                columns: new[] { "id", "activation_status", "bithdate", "contacts", "culture", "email", "firstname", "last_modified", "lastname", "location", "notes", "phone", "phone_activation", "removed", "sex", "sid", "sso_name_id", "sso_session_id", "status", "tenant", "terminateddate", "title", "username", "workfromdate" },
                values: new object[] { "66faa6e4-f133-11ea-b126-00ffeec8b4ef", 0, null, null, null, "", "Administrator", new DateTime(2020, 10, 6, 10, 14, 35, 587, DateTimeKind.Utc).AddTicks(7841), "", null, null, null, 0, false, null, null, null, null, 1, 1, null, null, "administrator", new DateTime(2020, 10, 6, 10, 14, 35, 587, DateTimeKind.Utc).AddTicks(6725) });

            migrationBuilder.InsertData(
                table: "tenants_forbiden",
                column: "address",
                values: new object[]
                {
                    "controlpanel",
                    "localhost"
                });

            migrationBuilder.InsertData(
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "mappeddomain", "name", "owner_id", "payment_id", "public", "publicvisibleproducts", "status", "statuschanged", "timezone", "trusteddomains", "version_changed" },
                values: new object[] { 1, "localhost", new DateTime(2020, 10, 6, 10, 14, 35, 606, DateTimeKind.Utc).AddTicks(4422), null, null, "Web Office", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", null, false, null, 0, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "core_usersecurity",
                columns: new[] { "userid", "pwdhash", "pwdhashsha512", "tenant" },
                values: new object[] { "66faa6e4-f133-11ea-b126-00ffeec8b4ef", "vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=", "USubvPlB+ogq0Q1trcSupg==", 1 });

            migrationBuilder.CreateIndex(
                name: "email",
                table: "core_user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "core_user",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "username",
                table: "core_user",
                columns: new[] { "tenant", "username" });

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "core_usergroup",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "IX_core_usergroup_userid",
                table: "core_usergroup",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "pwdhash",
                table: "core_usersecurity",
                column: "pwdhash");

            migrationBuilder.CreateIndex(
                name: "tenant",
                table: "core_usersecurity",
                column: "tenant");

            migrationBuilder.CreateIndex(
                name: "tenant",
                table: "tenants_iprestrictions",
                column: "tenant");

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "tenants_tenants",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "mappeddomain",
                table: "tenants_tenants",
                column: "mappeddomain");

            migrationBuilder.CreateIndex(
                name: "version",
                table: "tenants_tenants",
                column: "version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "core_settings");

            migrationBuilder.DropTable(
                name: "core_usergroup");

            migrationBuilder.DropTable(
                name: "core_usersecurity");

            migrationBuilder.DropTable(
                name: "tenants_forbiden");

            migrationBuilder.DropTable(
                name: "tenants_iprestrictions");

            migrationBuilder.DropTable(
                name: "tenants_partners");

            migrationBuilder.DropTable(
                name: "tenants_version");

            migrationBuilder.DropTable(
                name: "core_user");

            migrationBuilder.DropTable(
                name: "tenants_tenants");
        }
    }
}
