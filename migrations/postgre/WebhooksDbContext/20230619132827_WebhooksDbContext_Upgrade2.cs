using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.Migrations.WebhooksDb
{
    /// <inheritdoc />
    public partial class WebhooksDbContextUpgrade2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "tenant_id",
                table: "webhooks_logs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int unsigned");

            migrationBuilder.AlterColumn<int>(
                name: "tenant_id",
                table: "webhooks_config",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int unsigned");

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[,]
                {
                    { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null },
                    { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_webhooks_config_tenants_tenants_tenant_id",
                table: "webhooks_config",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webhooks_logs_tenants_tenants_tenant_id",
                table: "webhooks_logs",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_webhooks_config_tenants_tenants_tenant_id",
                table: "webhooks_config");

            migrationBuilder.DropForeignKey(
                name: "FK_webhooks_logs_tenants_tenants_tenant_id",
                table: "webhooks_logs");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "tenant_id",
                table: "webhooks_logs",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "tenant_id",
                table: "webhooks_config",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
