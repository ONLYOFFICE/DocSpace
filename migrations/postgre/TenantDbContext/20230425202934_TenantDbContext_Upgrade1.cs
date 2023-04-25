using System;
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
            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[] { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null });

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
