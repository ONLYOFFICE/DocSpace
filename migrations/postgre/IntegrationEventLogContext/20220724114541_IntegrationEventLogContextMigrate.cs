using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class IntegrationEventLogContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_bus_integration_event_log",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "char(38)", nullable: false, collation: "utf8_general_ci"),
                    event_type_name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    state = table.Column<int>(type: "int(11)", nullable: false),
                    times_sent = table.Column<int>(type: "int(11)", nullable: false),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    create_by = table.Column<Guid>(type: "char(38)", nullable: false, collation: "utf8_general_ci"),
                    content = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci"),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.event_id);
                });

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "event_bus_integration_event_log",
                column: "tenant_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_bus_integration_event_log");
        }
    }
}
