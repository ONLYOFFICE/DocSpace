using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.FeedDbContextMySql
{
    public partial class FeedDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "feed_aggregate",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(88)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant = table.Column<int>(nullable: false),
                    product = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    module = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    author = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    modified_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    modified_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    group_id = table.Column<string>(type: "varchar(70)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    aggregated_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    json = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    keywords = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_aggregate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feed_last",
                columns: table => new
                {
                    last_key = table.Column<string>(type: "varchar(128)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    last_date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.last_key);
                });

            migrationBuilder.CreateTable(
                name: "feed_readed",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    module = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant_id = table.Column<int>(nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.user_id, x.module });
                });

            migrationBuilder.CreateTable(
                name: "feed_users",
                columns: table => new
                {
                    feed_id = table.Column<string>(type: "varchar(88)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    user_id = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.feed_id, x.user_id });
                });

            migrationBuilder.CreateIndex(
                name: "aggregated_date",
                table: "feed_aggregate",
                columns: new[] { "tenant", "aggregated_date" });

            migrationBuilder.CreateIndex(
                name: "modified_date",
                table: "feed_aggregate",
                columns: new[] { "tenant", "modified_date" });

            migrationBuilder.CreateIndex(
                name: "product",
                table: "feed_aggregate",
                columns: new[] { "tenant", "product" });

            migrationBuilder.CreateIndex(
                name: "user_id",
                table: "feed_users",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feed_aggregate");

            migrationBuilder.DropTable(
                name: "feed_last");

            migrationBuilder.DropTable(
                name: "feed_readed");

            migrationBuilder.DropTable(
                name: "feed_users");
        }
    }
}
