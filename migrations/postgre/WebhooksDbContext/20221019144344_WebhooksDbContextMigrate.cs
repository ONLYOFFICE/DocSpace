using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class WebhooksDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "webhooks_config",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    secret_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''"),
                    tenant_id = table.Column<int>(type: "int unsigned", nullable: false),
                    uri = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''"),
                    enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "webhooks_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    config_id = table.Column<int>(type: "int", nullable: false),
                    creation_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    method = table.Column<string>(type: "varchar", maxLength: 100, nullable: true),
                    route = table.Column<string>(type: "varchar", maxLength: 100, nullable: true),
                    request_headers = table.Column<string>(type: "json", nullable: true),
                    request_payload = table.Column<string>(type: "text", nullable: false),
                    response_headers = table.Column<string>(type: "json", nullable: true),
                    response_payload = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<int>(type: "int unsigned", nullable: false),
                    uid = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    delivery = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_webhooks_logs_webhooks_config_config_id",
                        column: x => x.config_id,
                        principalTable: "webhooks_config",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "webhooks_config",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_webhooks_logs_config_id",
                table: "webhooks_logs",
                column: "config_id");

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "webhooks_logs",
                column: "tenant_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webhooks_logs");

            migrationBuilder.DropTable(
                name: "webhooks_config");
        }
    }
}
