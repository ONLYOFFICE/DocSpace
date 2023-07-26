using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class MigrationContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user");

            migrationBuilder.CreateIndex(
                name: "tenant_activation_status_email",
                schema: "onlyoffice",
                table: "core_user",
                columns: new[] { "tenant", "activation_status", "email" });

            migrationBuilder.CreateIndex(
                name: "tenant_activation_status_firstname",
                schema: "onlyoffice",
                table: "core_user",
                columns: new[] { "tenant", "activation_status", "firstname" });

            migrationBuilder.CreateIndex(
                name: "tenant_activation_status_lastname",
                schema: "onlyoffice",
                table: "core_user",
                columns: new[] { "tenant", "activation_status", "lastname" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "tenant_activation_status_email",
                schema: "onlyoffice",
                table: "core_user");

            migrationBuilder.DropIndex(
                name: "tenant_activation_status_firstname",
                schema: "onlyoffice",
                table: "core_user");

            migrationBuilder.DropIndex(
                name: "tenant_activation_status_lastname",
                schema: "onlyoffice",
                table: "core_user");

            migrationBuilder.CreateIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user",
                column: "tenant");
        }
    }
}
