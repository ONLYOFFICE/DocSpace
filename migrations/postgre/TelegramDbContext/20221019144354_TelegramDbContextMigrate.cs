using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class TelegramDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "telegram_users",
                schema: "onlyoffice",
                columns: table => new
                {
                    portal_user_id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("telegram_users_pkey", x => new { x.tenant_id, x.portal_user_id });
                });

            migrationBuilder.CreateIndex(
                name: "tgId",
                schema: "onlyoffice",
                table: "telegram_users",
                column: "telegram_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telegram_users",
                schema: "onlyoffice");
        }
    }
}
