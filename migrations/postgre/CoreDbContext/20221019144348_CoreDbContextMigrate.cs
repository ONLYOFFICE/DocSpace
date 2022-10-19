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
                name: "tenants_quota",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "character varying", nullable: true),
                    features = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValueSql: "0.00"),
                    product_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true, defaultValueSql: "NULL"),
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
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false, defaultValueSql: "NULL")
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
                    stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    customer_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValueSql: "NULL"),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    create_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tariffrow",
                schema: "onlyoffice",
                columns: table => new
                {
                    tariff_id = table.Column<int>(type: "int", nullable: false),
                    quota = table.Column<int>(type: "int", nullable: false),
                    tenant = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.tariff_id, x.quota });
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "visible" },
                values: new object[] { -3, null, "free,total_size:2147483648,manager:1,room:12,usersInRoom:3", "startup", false });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "price", "product_id", "visible" },
                values: new object[] { -2, null, "audit,ldap,sso,whitelabel,restore,total_size:10995116277760,file_size:1024,manager:1", "admin", 30m, "1002", true });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "visible" },
                values: new object[] { -1, null, "trial,audit,ldap,sso,whitelabel,restore,total_size:10995116277760,file_size:100,manager:1", "trial", false });

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
                name: "tenants_quota",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_quotarow",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tariff",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tariffrow",
                schema: "onlyoffice");
        }
    }
}
