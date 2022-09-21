using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.FilesDb
{
    public partial class FilesDbContext_Upgrade2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "options",
                schema: "onlyoffice",
                table: "files_security",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "subject_type",
                schema: "onlyoffice",
                table: "files_security",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "options",
                schema: "onlyoffice",
                table: "files_security");

            migrationBuilder.DropColumn(
                name: "subject_type",
                schema: "onlyoffice",
                table: "files_security");
        }
    }
}
