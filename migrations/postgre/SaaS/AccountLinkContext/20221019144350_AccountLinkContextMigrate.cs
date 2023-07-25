using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class AccountLinkContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "account_links",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    uid = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    provider = table.Column<string>(type: "character(60)", fixedLength: true, maxLength: 60, nullable: true, defaultValueSql: "NULL"),
                    profile = table.Column<string>(type: "text", nullable: false),
                    linked = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_links_pkey", x => new { x.id, x.uid });
                });

            migrationBuilder.CreateIndex(
                name: "uid",
                schema: "onlyoffice",
                table: "account_links",
                column: "uid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_links",
                schema: "onlyoffice");
        }
    }
}
