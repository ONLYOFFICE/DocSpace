using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Files.Core.Migrations.MySql.FilesDbContextMySql
{
    public partial class FilesDbContextMySql_Upgrade1 : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "files_link",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    source_id = table.Column<string>(type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    linked_id = table.Column<string>(type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    linked_for = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.source_id, x.linked_id });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "linked_for",
                table: "files_link",
                columns: new[] { "tenant_id", "source_id", "linked_id", "linked_for" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "files_link");
        }
    }
}
