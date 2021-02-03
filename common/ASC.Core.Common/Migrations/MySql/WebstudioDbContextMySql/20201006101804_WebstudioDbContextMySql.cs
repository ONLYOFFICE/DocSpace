using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.WebstudioDbContextMySql
{
    public partial class WebstudioDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "webstudio_index",
                columns: table => new
                {
                    index_name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.index_name);
                });

            migrationBuilder.CreateTable(
                name: "webstudio_settings",
                columns: table => new
                {
                    TenantID = table.Column<int>(nullable: false),
                    ID = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    UserID = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    Data = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.TenantID, x.ID, x.UserID });
                });

            migrationBuilder.CreateTable(
                name: "webstudio_uservisit",
                columns: table => new
                {
                    tenantid = table.Column<int>(nullable: false),
                    visitdate = table.Column<DateTime>(type: "datetime", nullable: false),
                    productid = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    userid = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    visitcount = table.Column<int>(nullable: false),
                    firstvisittime = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastvisittime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.visitdate, x.productid, x.userid });
                });

            migrationBuilder.InsertData(
                table: "webstudio_settings",
                columns: new[] { "TenantID", "ID", "UserID", "Data" },
                values: new object[,]
                {
                    { 1, "9a925891-1f92-4ed7-b277-d6f649739f06", "00000000-0000-0000-0000-000000000000", "{\"Completed\":false}" }
                });

            migrationBuilder.InsertData(
                table: "webstudio_uservisit",
                columns: new[] { "tenantid", "visitdate", "productid", "userid", "firstvisittime", "lastvisittime", "visitcount" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 10, 6, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(1620), "00000000-0000-0000-0000-000000000000", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2020, 10, 6, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(4248), new DateTime(2020, 10, 6, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(4835), 3 },
                    { 1, new DateTime(2020, 10, 7, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(5443), "00000000-0000-0000-0000-000000000000", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2020, 10, 6, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(5501), new DateTime(2020, 10, 6, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(5509), 2 },
                    { 1, new DateTime(2020, 10, 8, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(5519), "e67be73d-f9ae-4ce1-8fec-1880cb518cb4", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2020, 10, 6, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(5525), new DateTime(2020, 10, 6, 10, 18, 4, 448, DateTimeKind.Utc).AddTicks(5526), 1 }
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
