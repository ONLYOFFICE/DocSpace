using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.FilesDb
{
    public partial class FilesDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FolderId",
                schema: "onlyoffice",
                table: "files_thirdparty_account",
                newName: "folder_id");

            migrationBuilder.AddColumn<bool>(
                name: "private",
                schema: "onlyoffice",
                table: "files_thirdparty_account",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "private",
                schema: "onlyoffice",
                table: "files_folder",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "private",
                schema: "onlyoffice",
                table: "files_thirdparty_account");

            migrationBuilder.DropColumn(
                name: "private",
                schema: "onlyoffice",
                table: "files_folder");

            migrationBuilder.RenameColumn(
                name: "folder_id",
                schema: "onlyoffice",
                table: "files_thirdparty_account",
                newName: "FolderId");
        }
    }
}
