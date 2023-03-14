using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.WebhooksDb
{
    /// <inheritdoc />
    public partial class WebhooksDbContextMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "webhooks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    route = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8"),
                    method = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "webhooks_config",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8"),
                    secretkey = table.Column<string>(name: "secret_key", type: "varchar(50)", maxLength: 50, nullable: true, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8"),
                    tenantid = table.Column<uint>(name: "tenant_id", type: "int unsigned", nullable: false),
                    uri = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8"),
                    enabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "webhooks_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    configid = table.Column<int>(name: "config_id", type: "int", nullable: false),
                    creationtime = table.Column<DateTime>(name: "creation_time", type: "datetime", nullable: false),
                    webhookid = table.Column<int>(name: "webhook_id", type: "int", nullable: false),
                    requestheaders = table.Column<string>(name: "request_headers", type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8"),
                    requestpayload = table.Column<string>(name: "request_payload", type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    responseheaders = table.Column<string>(name: "response_headers", type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8"),
                    responsepayload = table.Column<string>(name: "response_payload", type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    status = table.Column<int>(type: "int", nullable: false),
                    tenantid = table.Column<uint>(name: "tenant_id", type: "int unsigned", nullable: false),
                    uid = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
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
                })
                .Annotation("MySql:CharSet", "utf8");

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
