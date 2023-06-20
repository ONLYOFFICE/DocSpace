using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.NotifyDb
{
    /// <inheritdoc />
    public partial class NotifyDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "last_modified", "mappeddomain", "name", "owner_id", "payment_id", "status", "statuschanged", "timezone", "trusteddomains", "version_changed" },
                values: new object[] { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Web Office", "00000000-0000-0000-0000-000000000000", null, 1, null, null, null, null });

            migrationBuilder.InsertData(
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "last_modified", "mappeddomain", "name", "owner_id", "payment_id", "statuschanged", "timezone", "trusteddomains", "version_changed" },
                values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Web Office", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_notify_queue_tenant_id",
                table: "notify_queue",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_notify_queue_tenants_tenants_tenant_id",
                table: "notify_queue",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notify_queue_tenants_tenants_tenant_id",
                table: "notify_queue");

            migrationBuilder.DropIndex(
                name: "IX_notify_queue_tenant_id",
                table: "notify_queue");

            migrationBuilder.DeleteData(
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
