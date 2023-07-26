using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class MigrationContextUpgrade2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "webplugins",
                schema: "onlyoffice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    license = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    author = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    homepage = table.Column<string>(name: "home_page", type: "character varying(255)", maxLength: 255, nullable: true),
                    pluginname = table.Column<string>(name: "plugin_name", type: "character varying(255)", maxLength: 255, nullable: false),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 36, nullable: false),
                    createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    system = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webplugins_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_webplugins_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "tenant_webplugins",
                schema: "onlyoffice",
                table: "webplugins",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webplugins",
                schema: "onlyoffice");
        }
    }
}
