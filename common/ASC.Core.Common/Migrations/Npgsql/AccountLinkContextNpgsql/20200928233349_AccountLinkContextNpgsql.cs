using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.Npgsql.AccountLinkContextNpgsql
{
    public partial class AccountLinkContextNpgsql : Migration
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
                    id = table.Column<string>(maxLength: 200, nullable: false),
                    uid = table.Column<string>(maxLength: 200, nullable: false),
                    provider = table.Column<string>(fixedLength: true, maxLength: 60, nullable: true, defaultValueSql: "NULL"),
                    profile = table.Column<string>(nullable: false),
                    linked = table.Column<DateTime>(nullable: false)
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
