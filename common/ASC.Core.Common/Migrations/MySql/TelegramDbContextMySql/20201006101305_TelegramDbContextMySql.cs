using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.TelegramDbContextMySql
{
    public partial class TelegramDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "telegram_users",
                columns: table => new
                {
                    portal_user_id = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant_id = table.Column<int>(nullable: false),
                    telegram_user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.portal_user_id });
                });

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
