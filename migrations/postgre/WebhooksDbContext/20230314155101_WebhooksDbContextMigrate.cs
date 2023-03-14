using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.WebhooksDb
{
    /// <inheritdoc />
    public partial class WebhooksDbContextMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "webhooks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    route = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "''"),
                    method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "webhooks_config",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    secretkey = table.Column<string>(name: "secret_key", type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''"),
                    tenantid = table.Column<int>(name: "tenant_id", type: "int unsigned", nullable: false),
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
                    configid = table.Column<int>(name: "config_id", type: "int", nullable: false),
                    creationtime = table.Column<DateTime>(name: "creation_time", type: "datetime", nullable: false),
                    webhookid = table.Column<int>(name: "webhook_id", type: "int", nullable: false),
                    requestheaders = table.Column<string>(name: "request_headers", type: "json", nullable: true),
                    requestpayload = table.Column<string>(name: "request_payload", type: "text", nullable: false),
                    responseheaders = table.Column<string>(name: "response_headers", type: "json", nullable: true),
                    responsepayload = table.Column<string>(name: "response_payload", type: "text", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    tenantid = table.Column<int>(name: "tenant_id", type: "int unsigned", nullable: false),
                    uid = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    delivery = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_webhooks_logs_webhooks_config_config_id",
                        column: x => x.configid,
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webhooks");

            migrationBuilder.DropTable(
                name: "webhooks_logs");

            migrationBuilder.DropTable(
                name: "webhooks_config");
        }
    }
}
