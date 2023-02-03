using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class FeedDbContextMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "feed_last",
                schema: "onlyoffice",
                columns: table => new
                {
                    lastkey = table.Column<string>(name: "last_key", type: "character varying(128)", maxLength: 128, nullable: false),
                    lastdate = table.Column<DateTime>(name: "last_date", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_last_pkey", x => x.lastkey);
                });

            migrationBuilder.CreateTable(
                name: "feed_users",
                schema: "onlyoffice",
                columns: table => new
                {
                    feedid = table.Column<string>(name: "feed_id", type: "character varying(88)", maxLength: 88, nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", fixedLength: true, maxLength: 38, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_users_pkey", x => new { x.feedid, x.userid });
                });

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
                name: "feed_aggregate",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(88)", maxLength: 88, nullable: false),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    product = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    author = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    modifiedby = table.Column<Guid>(name: "modified_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false),
                    modifieddate = table.Column<DateTime>(name: "modified_date", type: "timestamp with time zone", nullable: false),
                    groupid = table.Column<string>(name: "group_id", type: "character varying(70)", maxLength: 70, nullable: true, defaultValueSql: "NULL"),
                    aggregateddate = table.Column<DateTime>(name: "aggregated_date", type: "timestamp with time zone", nullable: false),
                    json = table.Column<string>(type: "text", nullable: false),
                    keywords = table.Column<string>(type: "text", nullable: true),
                    contextid = table.Column<string>(name: "context_id", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_aggregate", x => x.id);
                    table.ForeignKey(
                        name: "FK_feed_aggregate_tenants_tenants_tenant",
                        column: x => x.tenant,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feed_readed",
                schema: "onlyoffice",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 38, nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_readed_pkey", x => new { x.userid, x.tenantid, x.module });
                    table.ForeignKey(
                        name: "FK_feed_readed_tenants_tenants_tenant_id",
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

            migrationBuilder.CreateIndex(
                name: "aggregated_date",
                schema: "onlyoffice",
                table: "feed_aggregate",
                columns: new[] { "tenant", "aggregated_date" });

            migrationBuilder.CreateIndex(
                name: "modified_date",
                schema: "onlyoffice",
                table: "feed_aggregate",
                columns: new[] { "tenant", "modified_date" });

            migrationBuilder.CreateIndex(
                name: "product",
                schema: "onlyoffice",
                table: "feed_aggregate",
                columns: new[] { "tenant", "product" });

            migrationBuilder.CreateIndex(
                name: "IX_feed_readed_tenant_id",
                schema: "onlyoffice",
                table: "feed_readed",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "user_id_feed_users",
                schema: "onlyoffice",
                table: "feed_users",
                column: "user_id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feed_aggregate",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "feed_last",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "feed_readed",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "feed_users",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
