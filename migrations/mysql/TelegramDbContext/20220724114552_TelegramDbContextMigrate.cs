using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations
{
    public partial class TelegramDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "telegram_users",
                columns: table => new
                {
                    portal_user_id = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    telegram_user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.portal_user_id });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateIndex(
                name: "tgId",
                table: "telegram_users",
                column: "telegram_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telegram_users");
        }
    }
}
