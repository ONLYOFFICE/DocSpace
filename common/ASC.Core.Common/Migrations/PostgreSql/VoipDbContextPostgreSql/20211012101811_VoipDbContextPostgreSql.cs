using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.PostgreSql.VoipDbContextPostgreSql
{
    public partial class VoipDbContextPostgreSql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "crm_contact",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    is_company = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    first_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    last_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    company_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    industry = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    contact_type_id = table.Column<int>(type: "integer", nullable: false),
                    create_by = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    create_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_modifed_by = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false, defaultValueSql: "NULL"),
                    last_modifed_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NULL"),
                    display_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    is_shared = table.Column<bool>(type: "boolean", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_contact", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_voip_number",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    alias = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    settings = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_voip_number", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_voip_calls",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    parent_call_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    number_from = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    number_to = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    answered_by = table.Column<Guid>(type: "uuid", maxLength: 50, nullable: false, defaultValueSql: "'00000000-0000-0000-0000-000000000000'"),
                    dial_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    dial_duration = table.Column<int>(type: "integer", nullable: false),
                    record_sid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    record_url = table.Column<string>(type: "text", nullable: true),
                    record_duration = table.Column<int>(type: "integer", nullable: false),
                    record_price = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    contact_id = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,4)", nullable: false, defaultValueSql: "NULL"),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    CrmContactId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_voip_calls", x => x.id);
                    table.ForeignKey(
                        name: "FK_crm_voip_calls_crm_contact_CrmContactId",
                        column: x => x.CrmContactId,
                        principalSchema: "onlyoffice",
                        principalTable: "crm_contact",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "company_id",
                schema: "onlyoffice",
                table: "crm_contact",
                columns: new[] { "tenant_id", "company_id" });

            migrationBuilder.CreateIndex(
                name: "create_on_crm_contact",
                schema: "onlyoffice",
                table: "crm_contact",
                column: "create_on");

            migrationBuilder.CreateIndex(
                name: "display_name",
                schema: "onlyoffice",
                table: "crm_contact",
                columns: new[] { "tenant_id", "display_name" });

            migrationBuilder.CreateIndex(
                name: "last_modifed_on_crm_contact",
                schema: "onlyoffice",
                table: "crm_contact",
                columns: new[] { "last_modifed_on", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "IX_crm_voip_calls_CrmContactId",
                schema: "onlyoffice",
                table: "crm_voip_calls",
                column: "CrmContactId");

            migrationBuilder.CreateIndex(
                name: "parent_call_id",
                schema: "onlyoffice",
                table: "crm_voip_calls",
                columns: new[] { "parent_call_id", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "tenant_id_crm_voip_calls",
                schema: "onlyoffice",
                table: "crm_voip_calls",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "tenant_id_crm_voip_number",
                schema: "onlyoffice",
                table: "crm_voip_number",
                column: "tenant_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "crm_voip_calls",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "crm_voip_number",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "crm_contact",
                schema: "onlyoffice");
        }
    }
}
