using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.ResourceDbContextMySql
{
    public partial class ResourceDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "res_authors",
                columns: table => new
                {
                    login = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    password = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    isAdmin = table.Column<bool>(nullable: false),
                    online = table.Column<bool>(nullable: false),
                    lastVisit = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => x.login);
                });

            _ = migrationBuilder.CreateTable(
                name: "res_authorsfile",
                columns: table => new
                {
                    authorLogin = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    fileid = table.Column<int>(nullable: false),
                    writeAccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => new { x.authorLogin, x.fileid });
                });

            _ = migrationBuilder.CreateTable(
                name: "res_authorslang",
                columns: table => new
                {
                    authorLogin = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    cultureTitle = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => new { x.authorLogin, x.cultureTitle });
                });

            _ = migrationBuilder.CreateTable(
                name: "res_cultures",
                columns: table => new
                {
                    title = table.Column<string>(type: "varchar(120)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    value = table.Column<string>(type: "varchar(120)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    available = table.Column<bool>(nullable: false),
                    creationDate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => x.title);
                });

            _ = migrationBuilder.CreateTable(
                name: "res_data",
                columns: table => new
                {
                    fileid = table.Column<int>(nullable: false),
                    title = table.Column<string>(type: "varchar(120)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    cultureTitle = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    textValue = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    timeChanges = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    resourceType = table.Column<string>(type: "varchar(20)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    flag = table.Column<int>(nullable: false),
                    link = table.Column<string>(type: "varchar(120)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    authorLogin = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "'Console'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => new { x.fileid, x.cultureTitle, x.title, x.id });
                });

            _ = migrationBuilder.CreateIndex(
                name: "id",
                table: "res_data",
                column: "id",
                unique: true);


            _ = migrationBuilder.CreateTable(
                name: "res_files",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    projectName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    moduleName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    resName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    isLock = table.Column<bool>(nullable: false),
                    lastUpdate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    creationDate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "'0000-00-00 00:00:00'")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_res_files", x => x.id);
                });

            _ = migrationBuilder.CreateTable(
                name: "res_reserve",
                columns: table => new
                {
                    fileid = table.Column<int>(nullable: false),
                    title = table.Column<string>(type: "varchar(120)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    cultureTitle = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    textValue = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    flag = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => new { x.fileid, x.title, x.cultureTitle, x.id });
                });

            _ = migrationBuilder.CreateIndex(
                name: "res_authorsfile_FK2",
                table: "res_authorsfile",
                column: "fileid");

            _ = migrationBuilder.CreateIndex(
                name: "res_authorslang_FK2",
                table: "res_authorslang",
                column: "cultureTitle");

            _ = migrationBuilder.CreateIndex(
                name: "resources_FK2",
                table: "res_data",
                column: "cultureTitle");

            _ = migrationBuilder.CreateIndex(
                name: "dateIndex",
                table: "res_data",
                column: "timeChanges");

            _ = migrationBuilder.CreateIndex(
                name: "resname",
                table: "res_files",
                column: "resName",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "resources_FK2",
                table: "res_reserve",
                column: "cultureTitle");

            _ = migrationBuilder.CreateIndex(
                name: "id",
                table: "res_reserve",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "res_authors");

            _ = migrationBuilder.DropTable(
                name: "res_authorsfile");

            _ = migrationBuilder.DropTable(
                name: "res_authorslang");

            _ = migrationBuilder.DropTable(
                name: "res_cultures");

            _ = migrationBuilder.DropTable(
                name: "res_data");

            _ = migrationBuilder.DropTable(
                name: "res_files");

            _ = migrationBuilder.DropTable(
                name: "res_reserve");
        }
    }
}
