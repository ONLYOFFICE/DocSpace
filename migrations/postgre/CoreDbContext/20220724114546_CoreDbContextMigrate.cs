using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class CoreDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "tenants_buttons",
                schema: "onlyoffice",
                columns: table => new
                {
                    tariff_id = table.Column<int>(type: "integer", nullable: false),
                    partner_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    button_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_buttons_pkey", x => new { x.tariff_id, x.partner_id });
                });

            migrationBuilder.CreateTable(
                name: "tenants_quota",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "character varying", nullable: true),
                    max_file_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    max_total_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    active_users = table.Column<int>(type: "integer", nullable: false),
                    features = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValueSql: "0.00"),
                    avangate_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true, defaultValueSql: "NULL"),
                    visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_quota_pkey", x => x.tenant);
                });

            migrationBuilder.CreateTable(
                name: "tenants_quotarow",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    counter = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    tag = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "'0'"),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_quotarow_pkey", x => new { x.tenant, x.path });
                });

            migrationBuilder.CreateTable(
                name: "tenants_tariff",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    tariff = table.Column<int>(type: "integer", nullable: false),
                    stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    create_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "active_users", "avangate_id", "description", "features", "max_file_size", "max_total_size", "name", "visible" },
                values: new object[] { -1, 10000, "0", null, "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default", false });

            migrationBuilder.CreateIndex(
                name: "last_modified_tenants_quotarow",
                schema: "onlyoffice",
                table: "tenants_quotarow",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "tenant_tenants_tariff",
                schema: "onlyoffice",
                table: "tenants_tariff",
                column: "tenant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_buttons",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_quota",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_quotarow",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tariff",
                schema: "onlyoffice");
        }
    }
}
