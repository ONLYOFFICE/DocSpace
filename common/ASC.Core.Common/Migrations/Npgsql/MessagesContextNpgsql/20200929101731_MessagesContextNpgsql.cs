using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.MessagesContextNpgsql
{
    public partial class MessagesContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "audit_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ip = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    platform = table.Column<string>(maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(nullable: false),
                    tenant_id = table.Column<int>(nullable: false),
                    user_id = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false, defaultValueSql: "NULL"),
                    page = table.Column<string>(maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(nullable: false),
                    description = table.Column<string>(maxLength: 20000, nullable: true, defaultValueSql: "NULL"),
                    initiator = table.Column<string>(maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    target = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "login_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ip = table.Column<string>(maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(maxLength: 200, nullable: true, defaultValueSql: "NULL::character varying"),
                    platform = table.Column<string>(maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(nullable: false),
                    tenant_id = table.Column<int>(nullable: false),
                    user_id = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false),
                    page = table.Column<string>(maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(nullable: false),
                    description = table.Column<string>(maxLength: 500, nullable: true, defaultValueSql: "NULL"),
                    login = table.Column<string>(maxLength: 200, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_partners",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    partner_id = table.Column<string>(nullable: true),
                    affiliate_id = table.Column<string>(nullable: true),
                    campaign = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_partners", x => x.tenant_id);
                    table.ForeignKey(
                        name: "FK_tenants_partners_tenants_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "date",
                schema: "onlyoffice",
                table: "audit_events",
                columns: new[] { "tenant_id", "date" });

            migrationBuilder.CreateIndex(
                name: "date_login_events",
                schema: "onlyoffice",
                table: "login_events",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "tenant_id_login_events",
                schema: "onlyoffice",
                table: "login_events",
                columns: new[] { "user_id", "tenant_id" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_partners");

            migrationBuilder.DropTable(
                name: "audit_events",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "login_events",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "webstudio_settings",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
