using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.CoreDb
{
    /// <inheritdoc />
    public partial class CoreDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2,
                column: "features",
                value: "audit,ldap,sso,whitelabel,thirdparty,audit,restore,total_size:107374182400,file_size:1024,manager:1");

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                column: "features",
                value: "trial,audit,ldap,sso,whitelabel,thirdparty,audit,restore,total_size:107374182400,file_size:100,manager:1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2,
                column: "features",
                value: "audit,ldap,sso,whitelabel,restore,thirdparty,audit,total_size:107374182400,file_size:1024,manager:1");

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                column: "features",
                value: "trial,audit,ldap,sso,whitelabel,restore,thirdparty,audit,total_size:107374182400,file_size:100,manager:1");
        }
    }
}
