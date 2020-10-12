using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.VoipDbContextMySql
{
    public partial class VoipDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "crm_contact",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<int>(nullable: false),
                    is_company = table.Column<bool>(nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    title = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    first_name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    last_name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    company_name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    industry = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    status_id = table.Column<int>(nullable: false),
                    company_id = table.Column<int>(nullable: false),
                    contact_type_id = table.Column<int>(nullable: false),
                    create_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    last_modifed_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    last_modifed_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    display_name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    is_shared = table.Column<bool>(nullable: false),
                    currency = table.Column<string>(type: "varchar(3)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_contact", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_voip_number",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    number = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    alias = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    settings = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_voip_number", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_voip_calls",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    parent_call_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    number_from = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    number_to = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    status = table.Column<int>(nullable: false),
                    answered_by = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "'00000000-0000-0000-0000-000000000000'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    dial_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    dial_duration = table.Column<int>(nullable: false),
                    record_sid = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    record_url = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    record_duration = table.Column<int>(nullable: false),
                    record_price = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    contact_id = table.Column<int>(nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    tenant_id = table.Column<int>(nullable: false),
                    CrmContactId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_voip_calls", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "create_on",
                table: "crm_contact",
                column: "create_on");

            migrationBuilder.CreateIndex(
                name: "last_modifed_on",
                table: "crm_contact",
                columns: new[] { "last_modifed_on", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "company_id",
                table: "crm_contact",
                columns: new[] { "tenant_id", "company_id" });

            migrationBuilder.CreateIndex(
                name: "display_name",
                table: "crm_contact",
                columns: new[] { "tenant_id", "display_name" });

            migrationBuilder.CreateIndex(
                name: "IX_crm_voip_calls_CrmContactId",
                table: "crm_voip_calls",
                column: "CrmContactId");

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "crm_voip_calls",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "parent_call_id",
                table: "crm_voip_calls",
                columns: new[] { "parent_call_id", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "crm_voip_number",
                column: "tenant_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "crm_voip_calls");

            migrationBuilder.DropTable(
                name: "crm_voip_number");

            migrationBuilder.DropTable(
                name: "crm_contact");
        }
    }
}
