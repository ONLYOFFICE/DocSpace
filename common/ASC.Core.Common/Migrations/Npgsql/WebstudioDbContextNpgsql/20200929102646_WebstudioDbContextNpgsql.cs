using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.Npgsql.WebstudioDbContextNpgsql
{
    public partial class WebstudioDbContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            _ = migrationBuilder.CreateTable(
                name: "webstudio_index",
                schema: "onlyoffice",
                columns: table => new
                {
                    index_name = table.Column<string>(maxLength: 50, nullable: false),
                    last_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("webstudio_index_pkey", x => x.index_name);
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("webstudio_settings_pkey", x => new { x.TenantID, x.ID, x.UserID });
                });

            _ = migrationBuilder.CreateTable(
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
                    _ = table.PrimaryKey("webstudio_uservisit_pkey", x => new { x.tenantid, x.visitdate, x.productid, x.userid });
                });

            _ = migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "webstudio_settings",
                columns: new[] { "TenantID", "ID", "UserID", "Data" },
                values: new object[,] {
                    { 1, new Guid("9a925891-1f92-4ed7-b277-d6f649739f06"), new Guid("00000000-0000-0000-0000-000000000000"), "{'Analytics':true,'Completed':true}" },
                    { 1, new Guid("ab5b3c97-a972-475c-bb13-71936186c4e6"), new Guid("00000000-0000-0000-0000-000000000000"), "{'ColorThemeName':'pure - orange','FirstRequest':false}" }
                });

            _ = migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "webstudio_uservisit",
                columns: new[] { "tenantid", "visitdate", "productid", "userid", "firstvisittime", "lastvisittime", "visitcount" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 10, 1, 16, 53, 22, 198, DateTimeKind.Utc).AddTicks(5709), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), new DateTime(2020, 10, 1, 16, 53, 22, 198, DateTimeKind.Utc).AddTicks(9735), new DateTime(2020, 10, 1, 16, 53, 22, 199, DateTimeKind.Utc).AddTicks(777), 3 },
                    { 1, new DateTime(2020, 10, 1, 16, 53, 22, 199, DateTimeKind.Utc).AddTicks(1775), new Guid("00000000-0000-0000-0000-000000000000"), new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), new DateTime(2020, 10, 1, 16, 53, 22, 199, DateTimeKind.Utc).AddTicks(2002), new DateTime(2020, 10, 1, 16, 53, 22, 199, DateTimeKind.Utc).AddTicks(2025), 2 },
                    { 1, new DateTime(2020, 10, 1, 16, 53, 22, 199, DateTimeKind.Utc).AddTicks(2046), new Guid("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"), new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), new DateTime(2020, 10, 1, 16, 53, 22, 199, DateTimeKind.Utc).AddTicks(2061), new DateTime(2020, 10, 1, 16, 53, 22, 199, DateTimeKind.Utc).AddTicks(2065), 1 }
                });

            _ = migrationBuilder.CreateIndex(
                name: "ID",
                schema: "onlyoffice",
                table: "webstudio_settings",
                column: "ID");

            _ = migrationBuilder.CreateIndex(
                name: "visitdate",
                schema: "onlyoffice",
                table: "webstudio_uservisit",
                column: "visitdate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "webstudio_index",
                schema: "onlyoffice");

            _ = migrationBuilder.DropTable(
                name: "webstudio_settings",
                schema: "onlyoffice");

            _ = migrationBuilder.DropTable(
                name: "webstudio_uservisit",
                schema: "onlyoffice");
        }
    }
}
