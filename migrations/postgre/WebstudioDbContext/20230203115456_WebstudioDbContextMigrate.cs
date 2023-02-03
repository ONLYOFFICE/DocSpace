using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class WebstudioDbContextMigrate : Migration
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
                name: "webstudio_index",
                schema: "onlyoffice",
                columns: table => new
                {
                    indexname = table.Column<string>(name: "index_name", type: "character varying(50)", maxLength: 50, nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_index_pkey", x => x.indexname);
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

            migrationBuilder.CreateTable(
                name: "webstudio_uservisit",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(type: "integer", nullable: false),
                    visitdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    productid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    userid = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    visitcount = table.Column<int>(type: "integer", nullable: false),
                    firstvisittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lastvisittime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_uservisit_pkey", x => new { x.tenantid, x.visitdate, x.productid, x.userid });
                    table.ForeignKey(
                        name: "FK_webstudio_uservisit_tenants_tenants_tenantid",
                        column: x => x.tenantid,
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
                table: "webstudio_settings",
                columns: new[] { "ID", "TenantID", "UserID", "Data" },
                values: new object[] { new Guid("9a925891-1f92-4ed7-b277-d6f649739f06"), 1, new Guid("00000000-0000-0000-0000-000000000000"), "{\"Completed\":false}" });

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

            migrationBuilder.CreateIndex(
                name: "visitdate",
                schema: "onlyoffice",
                table: "webstudio_uservisit",
                column: "visitdate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webstudio_index",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_uservisit",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
