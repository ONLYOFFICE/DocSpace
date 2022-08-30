using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.CoreDb
{
    public partial class CoreDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                schema: "onlyoffice",
                table: "tenants_tariff");

            migrationBuilder.DropColumn(
                name: "tariff",
                schema: "onlyoffice",
                table: "tenants_tariff");

            migrationBuilder.DropColumn(
                name: "active_users",
                schema: "onlyoffice",
                table: "tenants_quota");

            migrationBuilder.DropColumn(
                name: "max_total_size",
                schema: "onlyoffice",
                table: "tenants_quota");

            migrationBuilder.RenameColumn(
                name: "avangate_id",
                schema: "onlyoffice",
                table: "tenants_quota",
                newName: "product_id");

            migrationBuilder.AddColumn<Guid>(
                name: "customer_id",
                schema: "onlyoffice",
                table: "tenants_tariff",
                type: "uuid",
                maxLength: 36,
                nullable: false,
                defaultValueSql: "NULL");

            migrationBuilder.CreateTable(
                name: "TariffRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TariffId = table.Column<int>(type: "integer", nullable: false),
                    Quota = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Tenant = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffRows", x => x.Id);
                });

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                columns: new[] { "features", "max_file_size", "name", "product_id" },
                values: new object[] { "trial,audit,ldap,sso,whitelabel,restore,total_size:10995116277760,admin:1", 100L, "trial", null });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "max_file_size", "name", "visible" },
                values: new object[] { -3, null, "free,audit,ldap,sso,restore,total_size:2147483648,admin:5,rooms:3", 100L, "startup", false });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "max_file_size", "name", "price", "product_id", "visible" },
                values: new object[] { -2, null, "audit,ldap,sso,whitelabel,restore,total_size:10995116277760,admin:1", 1024L, "admin", 30.00m, "1002", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TariffRows");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -3);

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2);

            migrationBuilder.DropColumn(
                name: "customer_id",
                schema: "onlyoffice",
                table: "tenants_tariff");

            migrationBuilder.RenameColumn(
                name: "product_id",
                schema: "onlyoffice",
                table: "tenants_quota",
                newName: "avangate_id");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                schema: "onlyoffice",
                table: "tenants_tariff",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tariff",
                schema: "onlyoffice",
                table: "tenants_tariff",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "active_users",
                schema: "onlyoffice",
                table: "tenants_quota",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "max_total_size",
                schema: "onlyoffice",
                table: "tenants_quota",
                type: "bigint",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                columns: new[] { "active_users", "avangate_id", "features", "max_file_size", "max_total_size", "name" },
                values: new object[] { 10000, "0", "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default" });
        }
    }
}
