using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations
{
    public partial class CoreDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tenants_buttons",
                columns: table => new
                {
                    tariff_id = table.Column<int>(type: "int", nullable: false),
                    partner_id = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    button_url = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tariff_id, x.partner_id });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "tenants_quota",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    description = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    max_file_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    max_total_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    active_users = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    features = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8"),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValueSql: "'0.00'"),
                    avangate_id = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    visible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "tenants_quotarow",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "int", nullable: false),
                    path = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    counter = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    tag = table.Column<string>(type: "varchar(1024)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.path });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "tenants_tariff",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant = table.Column<int>(type: "int", nullable: false),
                    tariff = table.Column<int>(type: "int", nullable: false),
                    stamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    comment = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    create_on = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "active_users", "avangate_id", "description", "features", "max_file_size", "max_total_size", "name" },
                values: new object[] { -1, 10000, "0", null, "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default" });

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "tenants_quotarow",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "tenant",
                table: "tenants_tariff",
                column: "tenant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_buttons");

            migrationBuilder.DropTable(
                name: "tenants_quota");

            migrationBuilder.DropTable(
                name: "tenants_quotarow");

            migrationBuilder.DropTable(
                name: "tenants_tariff");
        }
    }
}
