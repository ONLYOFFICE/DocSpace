using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.FilesDb
{
    /// <inheritdoc />
    public partial class FilesDbContextUpgrade2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "tenant_id_parent_id_modified_on",
                table: "files_folder",
                columns: new[] { "tenant_id", "parent_id", "modified_on" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_parent_id_title",
                table: "files_folder",
                columns: new[] { "tenant_id", "parent_id", "title" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_folder_id_content_length",
                table: "files_file",
                columns: new[] { "tenant_id", "folder_id", "content_length" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_folder_id_modified_on",
                table: "files_file",
                columns: new[] { "tenant_id", "folder_id", "modified_on" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_folder_id_title",
                table: "files_file",
                columns: new[] { "tenant_id", "folder_id", "title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "tenant_id_parent_id_modified_on",
                table: "files_folder");

            migrationBuilder.DropIndex(
                name: "tenant_id_parent_id_title",
                table: "files_folder");

            migrationBuilder.DropIndex(
                name: "tenant_id_folder_id_content_length",
                table: "files_file");

            migrationBuilder.DropIndex(
                name: "tenant_id_folder_id_modified_on",
                table: "files_file");

            migrationBuilder.DropIndex(
                name: "tenant_id_folder_id_title",
                table: "files_file");
        }
    }
}
