using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.CoreDb
{
    public partial class CoreDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                table: "tenants_tariff");

            migrationBuilder.DropColumn(
                name: "tariff",
                table: "tenants_tariff");

            migrationBuilder.DropColumn(
                name: "active_users",
                table: "tenants_quota");

            migrationBuilder.DropColumn(
                name: "max_total_size",
                table: "tenants_quota");

            migrationBuilder.RenameColumn(
                name: "avangate_id",
                table: "tenants_quota",
                newName: "product_id");

            migrationBuilder.AddColumn<string>(
                name: "customer_id",
                table: "tenants_tariff",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "tenants_tariffrow",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tariff_id = table.Column<int>(type: "int", nullable: false),
                    quota = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    tenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                columns: new[] { "features", "max_file_size", "name", "product_id" },
                values: new object[] { "trial,audit,ldap,sso,whitelabel,restore,total_size:10995116277760,admin:1", 100L, "trial", null });

            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "max_file_size", "name", "product_id" },
                values: new object[] { -3, null, "free,audit,ldap,sso,restore,total_size:2147483648,admin:5,rooms:3", 100L, "startup", null });

            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "max_file_size", "name", "price", "product_id", "visible" },
                values: new object[] { -2, null, "audit,ldap,sso,whitelabel,restore,total_size:10995116277760,admin:1", 1024L, "admin", 30.00m, "1002", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_tariffrow");

            migrationBuilder.DeleteData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2);

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "tenants_tariff");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "tenants_quota",
                newName: "avangate_id");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "tenants_tariff",
                type: "int",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.AddColumn<int>(
                name: "tariff",
                table: "tenants_tariff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "active_users",
                table: "tenants_quota",
                type: "int",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.AddColumn<long>(
                name: "max_total_size",
                table: "tenants_quota",
                type: "bigint",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.UpdateData(
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                columns: new[] { "active_users", "avangate_id", "features", "max_file_size", "max_total_size", "name" },
                values: new object[] { 10000, "0", "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default" });
        }
    }
}
