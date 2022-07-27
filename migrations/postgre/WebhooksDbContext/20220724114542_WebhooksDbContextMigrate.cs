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
                    config_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    secret_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''"),
                    tenant_id = table.Column<int>(type: "int unsigned", nullable: false),
                    uri = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.config_id);
                });

            migrationBuilder.CreateTable(
                name: "webhooks_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    config_id = table.Column<int>(type: "int", nullable: false),
                    creation_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    @event = table.Column<string>(name: "event", type: "varchar", maxLength: 100, nullable: true),
                    request_headers = table.Column<string>(type: "json", nullable: true),
                    request_payload = table.Column<string>(type: "json", nullable: false),
                    response_headers = table.Column<string>(type: "json", nullable: true),
                    response_payload = table.Column<string>(type: "json", nullable: true),
                    status = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    tenant_id = table.Column<int>(type: "int unsigned", nullable: false),
                    uid = table.Column<string>(type: "varchar", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webhooks_config");

            migrationBuilder.DropTable(
                name: "webhooks_logs");
        }
    }
}
