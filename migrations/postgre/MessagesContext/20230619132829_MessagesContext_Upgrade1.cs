using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.Messages
{
    /// <inheritdoc />
    public partial class MessagesContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_login_events_tenant_id",
                schema: "onlyoffice",
                table: "login_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user",
                column: "tenant");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_events_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "audit_events",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_usergroup",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_login_events_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "login_events",
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
                name: "FK_audit_events_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "audit_events");

            migrationBuilder.DropForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_usergroup");

            migrationBuilder.DropForeignKey(
                name: "FK_login_events_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "login_events");

            migrationBuilder.DropIndex(
                name: "IX_login_events_tenant_id",
                schema: "onlyoffice",
                table: "login_events");

            migrationBuilder.DropIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);
        }
    }
}
