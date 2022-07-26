using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class FeedDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

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
                    modified_by = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    group_id = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true, defaultValueSql: "NULL"),
                    aggregated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    json = table.Column<string>(type: "text", nullable: false),
                    keywords = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_aggregate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feed_last",
                schema: "onlyoffice",
                columns: table => new
                {
                    last_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    last_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_last_pkey", x => x.last_key);
                });

            migrationBuilder.CreateTable(
                name: "feed_readed",
                schema: "onlyoffice",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_readed_pkey", x => new { x.user_id, x.tenant_id, x.module });
                });

            migrationBuilder.CreateTable(
                name: "feed_users",
                schema: "onlyoffice",
                columns: table => new
                {
                    feed_id = table.Column<string>(type: "character varying(88)", maxLength: 88, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feed_users_pkey", x => new { x.feed_id, x.user_id });
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
                name: "user_id_feed_users",
                schema: "onlyoffice",
                table: "feed_users",
                column: "user_id");
        }

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
        }
    }
}
