using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class TenantDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_core_settings_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_settings",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_iprestrictions_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_iprestrictions",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_core_settings_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_iprestrictions_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_iprestrictions");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);
        }
    }
}
