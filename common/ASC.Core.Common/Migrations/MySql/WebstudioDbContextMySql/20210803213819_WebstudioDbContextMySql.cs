using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.WebstudioDbContextMySql
{
    public partial class WebstudioDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "webstudio_index",
                columns: table => new
                {
                    index_name = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.index_name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "webstudio_settings",
                columns: table => new
                {
                    TenantID = table.Column<int>(type: "int", nullable: false),
                    ID = table.Column<string>(type: "varchar(64)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    UserID = table.Column<string>(type: "varchar(64)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Data = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.TenantID, x.ID, x.UserID });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "webstudio_uservisit",
                columns: table => new
                {
                    tenantid = table.Column<int>(type: "int", nullable: false),
                    visitdate = table.Column<DateTime>(type: "datetime", nullable: false),
                    productid = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    userid = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    visitcount = table.Column<int>(type: "int", nullable: false),
                    firstvisittime = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastvisittime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.visitdate, x.productid, x.userid });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "webstudio_settings",
                columns: new[] { "ID", "TenantID", "UserID", "Data" },
                values: new object[] { "9a925891-1f92-4ed7-b277-d6f649739f06", 1, "00000000-0000-0000-0000-000000000000", "{\"Completed\":false}" });

            migrationBuilder.InsertData(
                table: "webstudio_uservisit",
                columns: new[] { "productid", "tenantid", "userid", "visitdate", "firstvisittime", "lastvisittime", "visitcount" },
                values: new object[,]
                {
                    { "00000000-0000-0000-0000-000000000000", 1, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(1020), new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(3051), new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(4096), 3 },
                    { "00000000-0000-0000-0000-000000000000", 1, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(5161), new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(5205), new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(5208), 2 },
                    { "e67be73d-f9ae-4ce1-8fec-1880cb518cb4", 1, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(5210), new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(5223), new DateTime(2021, 8, 3, 21, 38, 18, 454, DateTimeKind.Utc).AddTicks(5225), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "ID",
                table: "webstudio_settings",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "visitdate",
                table: "webstudio_uservisit",
                column: "visitdate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webstudio_index");

            migrationBuilder.DropTable(
                name: "webstudio_settings");

            migrationBuilder.DropTable(
                name: "webstudio_uservisit");
        }
    }
}
