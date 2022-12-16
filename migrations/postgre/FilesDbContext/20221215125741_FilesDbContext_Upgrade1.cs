using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.FilesDb
{
    public partial class FilesDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "has_logo",
                schema: "onlyoffice",
                table: "files_thirdparty_account",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_logo",
                schema: "onlyoffice",
                table: "files_folder",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_logo",
                schema: "onlyoffice",
                table: "files_thirdparty_account");

            migrationBuilder.DropColumn(
                name: "has_logo",
                schema: "onlyoffice",
                table: "files_folder");
        }
    }
}
