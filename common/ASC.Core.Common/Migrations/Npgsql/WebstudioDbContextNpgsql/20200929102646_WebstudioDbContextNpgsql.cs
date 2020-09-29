using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.Npgsql.WebstudioDbContextNpgsql
{
    public partial class WebstudioDbContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "webstudio_index",
                schema: "onlyoffice",
                columns: table => new
                {
                    index_name = table.Column<string>(maxLength: 50, nullable: false),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_index_pkey", x => x.index_name);
                });

            migrationBuilder.CreateTable(
                name: "webstudio_settings",
                schema: "onlyoffice",
                columns: table => new
                {
                    TenantID = table.Column<int>(nullable: false),
                    ID = table.Column<Guid>(maxLength: 64, nullable: false),
                    UserID = table.Column<Guid>(maxLength: 64, nullable: false),
                    Data = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_settings_pkey", x => new { x.TenantID, x.ID, x.UserID });
                });

            migrationBuilder.CreateTable(
                name: "webstudio_uservisit",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(nullable: false),
                    visitdate = table.Column<DateTime>(nullable: false),
                    productid = table.Column<Guid>(maxLength: 38, nullable: false),
                    userid = table.Column<Guid>(maxLength: 38, nullable: false),
                    visitcount = table.Column<int>(nullable: false),
                    firstvisittime = table.Column<DateTime>(nullable: false),
                    lastvisittime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webstudio_uservisit_pkey", x => new { x.tenantid, x.visitdate, x.productid, x.userid });
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "webstudio_settings",
                columns: new[] { "TenantID", "ID", "UserID", "Data" },
                values: new object[] { 1, new Guid("9a925891-1f92-4ed7-b277-d6f649739f06"), new Guid("00000000-0000-0000-0000-000000000000"), "{'Completed':false}" });

            migrationBuilder.CreateIndex(
                name: "ID",
                schema: "onlyoffice",
                table: "webstudio_settings",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "visitdate",
                schema: "onlyoffice",
                table: "webstudio_uservisit",
                column: "visitdate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webstudio_index",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_uservisit",
                schema: "onlyoffice");
        }
    }
}
