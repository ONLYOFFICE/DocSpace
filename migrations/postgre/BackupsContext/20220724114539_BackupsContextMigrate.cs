using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class BackupsContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "backup_backup",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char", maxLength: 38, nullable: false, collation: "utf8_general_ci"),
                    tenant_id = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    is_scheduled = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, collation: "utf8_general_ci"),
                    storage_type = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    storage_base_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci"),
                    storage_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    expires_on = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'0001-01-01 00:00:00'"),
                    storage_params = table.Column<string>(type: "text", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "backup_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "integer", maxLength: 10, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    backup_mail = table.Column<bool>(type: "boolean", maxLength: 10, nullable: false, defaultValueSql: "'0'"),
                    cron = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci"),
                    backups_stored = table.Column<int>(type: "integer", maxLength: 10, nullable: false),
                    storage_type = table.Column<int>(type: "integer", maxLength: 10, nullable: false),
                    storage_base_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci"),
                    last_backup_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    storage_params = table.Column<string>(type: "text", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant_id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mappeddomain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, defaultValueSql: "NULL"),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "2"),
                    version_changed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    language = table.Column<string>(type: "character(10)", fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "'en-US'"),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    trusteddomains = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    trusteddomainsenabled = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    status = table.Column<int>(type: "integer", nullable: false),
                    statuschanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    payment_id = table.Column<string>(type: "character varying(38)", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    industry = table.Column<int>(type: "integer", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, null });

            migrationBuilder.CreateIndex(
                name: "expires_on",
                table: "backup_backup",
                column: "expires_on");

            migrationBuilder.CreateIndex(
                name: "is_scheduled",
                table: "backup_backup",
                column: "is_scheduled");

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "backup_backup",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "alias",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "last_modified_tenants_tenants",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "mappeddomain",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "mappeddomain");

            migrationBuilder.CreateIndex(
                name: "version",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "backup_backup");

            migrationBuilder.DropTable(
                name: "backup_schedule");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
