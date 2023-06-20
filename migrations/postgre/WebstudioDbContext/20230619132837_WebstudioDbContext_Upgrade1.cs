using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.WebstudioDb
{
    /// <inheritdoc />
    public partial class WebstudioDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_webstudio_settings_tenants_tenants_TenantID",
                schema: "onlyoffice",
                table: "webstudio_settings",
                column: "TenantID",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webstudio_uservisit_tenants_tenants_tenantid",
                schema: "onlyoffice",
                table: "webstudio_uservisit",
                column: "tenantid",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_webstudio_settings_tenants_tenants_TenantID",
                schema: "onlyoffice",
                table: "webstudio_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_webstudio_uservisit_tenants_tenants_tenantid",
                schema: "onlyoffice",
                table: "webstudio_uservisit");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);
        }
    }
}
