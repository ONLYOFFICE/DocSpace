using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.MessagesContextMySql
{
    public partial class MessagesContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "login_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    login = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ip = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    browser = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    platform = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    date = table.Column<DateTime>(type: "datetime", nullable: false),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    page = table.Column<string>(type: "varchar(300)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    action = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_events", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "date",
                table: "login_events",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "login_events",
                columns: new[] { "tenant_id", "user_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "login_events");
        }
    }
}
