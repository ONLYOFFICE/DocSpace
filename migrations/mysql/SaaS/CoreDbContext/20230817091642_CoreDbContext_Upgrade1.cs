using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.MySql.Migrations.CoreDb
{
    /// <inheritdoc />
    public partial class CoreDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "product_id" },
                values: new object[,]
                {
                    { -7, null, "non-profit,audit,ldap,sso,thirdparty,restore,oauth,contentsearch,total_size:2147483648,file_size:1024,manager:20", "nonprofit", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -7);
        }
    }
}
