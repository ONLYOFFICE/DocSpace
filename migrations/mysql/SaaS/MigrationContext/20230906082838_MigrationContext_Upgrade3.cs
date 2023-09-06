using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class MigrationContext_Upgrade3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "tenant_activation_status_email",
                table: "core_user",
                columns: new[] { "tenant", "activation_status", "email" });

            migrationBuilder.CreateIndex(
                name: "tenant_activation_status_firstname",
                table: "core_user",
                columns: new[] { "tenant", "activation_status", "firstname" });

            migrationBuilder.CreateIndex(
                name: "tenant_activation_status_lastname",
                table: "core_user",
                columns: new[] { "tenant", "activation_status", "lastname" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "tenant_activation_status_email",
                table: "core_user");

            migrationBuilder.DropIndex(
                name: "tenant_activation_status_firstname",
                table: "core_user");

            migrationBuilder.DropIndex(
                name: "tenant_activation_status_lastname",
                table: "core_user");
        }
    }
}
