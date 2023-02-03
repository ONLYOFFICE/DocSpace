using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class MessagesContextMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mappeddomain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, defaultValueSql: "NULL"),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "2"),
                    versionchanged = table.Column<DateTime>(name: "version_changed", type: "timestamp with time zone", nullable: true),
                    language = table.Column<string>(type: "character(10)", fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "'en-US'"),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    trusteddomains = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    trusteddomainsenabled = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    status = table.Column<int>(type: "integer", nullable: false),
                    statuschanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ownerid = table.Column<Guid>(name: "owner_id", type: "uuid", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    paymentid = table.Column<string>(name: "payment_id", type: "character varying(38)", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    industry = table.Column<int>(type: "integer", nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    initiator = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    target = table.Column<string>(type: "text", nullable: true),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    platform = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", fixedLength: true, maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    page = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_events_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_user",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    firstname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    lastname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    sex = table.Column<bool>(type: "boolean", nullable: true),
                    bithdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    activationstatus = table.Column<int>(name: "activation_status", type: "integer", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    workfromdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    terminateddate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    culture = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "NULL"),
                    contacts = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    phone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    phoneactivation = table.Column<int>(name: "phone_activation", type: "integer", nullable: false),
                    location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    notes = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    sid = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    ssonameid = table.Column<string>(name: "sso_name_id", type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    ssosessionid = table.Column<string>(name: "sso_session_id", type: "character varying(512)", maxLength: 512, nullable: true, defaultValueSql: "NULL"),
                    removed = table.Column<bool>(type: "boolean", nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_core_user_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "core_usergroup",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    groupid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    reftype = table.Column<int>(name: "ref_type", type: "integer", nullable: false),
                    removed = table.Column<bool>(type: "boolean", nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("core_usergroup_pkey", x => new { x.tenant, x.userid, x.groupid, x.reftype });
                    table.ForeignKey(
                        name: "FK_core_usergroup_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "login_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL::character varying"),
                    platform = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    page = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_login_events_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webstudio_settings",
                schema: "onlyoffice",
                columns: table => new
                {
                    TenantID = table.Column<int>(type: "integer", nullable: false),
                    ID = table.Column<Guid>(type: "uuid", maxLength: 64, nullable: false),
                    UserID = table.Column<Guid>(type: "uuid", maxLength: 64, nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_settings_pkey", x => new { x.TenantID, x.ID, x.UserID });
                    table.ForeignKey(
                        name: "FK_webstudio_settings_tenants_tenants_TenantID",
                        column: x => x.TenantID,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[,]
                {
                    { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null },
                    { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, null }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_user",
                columns: new[] { "id", "activation_status", "bithdate", "create_on", "email", "firstname", "last_modified", "lastname", "phone_activation", "removed", "sex", "status", "tenant", "terminateddate", "username", "workfromdate" },
                values: new object[] { new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Administrator", new DateTime(2021, 3, 9, 9, 52, 55, 765, DateTimeKind.Utc).AddTicks(1420), "", 0, false, null, 1, 1, null, "administrator", new DateTime(2021, 3, 9, 9, 52, 55, 764, DateTimeKind.Utc).AddTicks(9157) });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "webstudio_settings",
                columns: new[] { "ID", "TenantID", "UserID", "Data" },
                values: new object[] { new Guid("9a925891-1f92-4ed7-b277-d6f649739f06"), 1, new Guid("00000000-0000-0000-0000-000000000000"), "{\"Completed\":false}" });

            migrationBuilder.CreateIndex(
                name: "date",
                schema: "onlyoffice",
                table: "audit_events",
                columns: new[] { "tenant_id", "date" });

            migrationBuilder.CreateIndex(
                name: "email",
                schema: "onlyoffice",
                table: "core_user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user",
                column: "tenant");

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
                name: "date_login_events",
                schema: "onlyoffice",
                table: "login_events",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_login_events_tenant_id",
                schema: "onlyoffice",
                table: "login_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "tenant_id_login_events",
                schema: "onlyoffice",
                table: "login_events",
                columns: new[] { "user_id", "tenant_id" });

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

            migrationBuilder.CreateIndex(
                name: "ID",
                schema: "onlyoffice",
                table: "webstudio_settings",
                column: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_events",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_user",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "core_usergroup",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "login_events",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
