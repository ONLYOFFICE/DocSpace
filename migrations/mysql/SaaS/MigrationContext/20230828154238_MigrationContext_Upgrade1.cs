using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.MySql.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class MigrationContext_Upgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenants_partners",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    partner_id = table.Column<string>(type: "varchar(36)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    affiliate_id = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    campaign = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant_id);
                    table.ForeignKey(
                        name: "FK_tenants_partners_tenants_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.UpdateData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2,
                column: "price",
                value: 15m);

            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "product_id" },
                values: new object[,]
                {
                    { -7, null, "non-profit,audit,ldap,sso,thirdparty,restore,oauth,contentsearch,total_size:2147483648,file_size:1024,manager:20", "nonprofit", null },
                    { -6, null, "audit,ldap,sso,whitelabel,thirdparty,restore,oauth,contentsearch,file_size:1024", "subscription", "1001" },
                    { -5, null, "manager:1", "admin1", "1005" },
                    { -4, null, "total_size:1073741824", "disk", "1004" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_partners");

            migrationBuilder.DeleteData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -4);

            migrationBuilder.UpdateData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2,
                column: "price",
                value: 30m);
        }
    }
}
