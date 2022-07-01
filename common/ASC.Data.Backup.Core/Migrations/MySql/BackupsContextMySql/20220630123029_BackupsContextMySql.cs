using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Data.Backup.Core.Migrations.MySql.BackupsContextMySql
{
    public partial class BackupsContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "backup_backup",
                columns: table => new
                {
                    id = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    tenant_id = table.Column<int>(type: "int(10)", nullable: false),
                    is_scheduled = table.Column<int>(type: "int(10)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    hash = table.Column<string>(type: "varchar(64)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    storage_type = table.Column<int>(type: "int(10)", nullable: false),
                    storage_base_path = table.Column<string>(type: "varchar(255)", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    storage_path = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    expires_on = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'0001-01-01 00:00:00'"),
                    storage_params = table.Column<string>(type: "text", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "backup_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int(10)", nullable: false),
                    backup_mail = table.Column<int>(type: "int(10)", nullable: false, defaultValueSql: "'0'"),
                    cron = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    backups_stored = table.Column<int>(type: "int(10)", nullable: false),
                    storage_type = table.Column<int>(type: "int(10)", nullable: false),
                    storage_base_path = table.Column<string>(type: "varchar(255)", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    last_backup_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    storage_params = table.Column<string>(type: "text", nullable: true, defaultValueSql: "NULL", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    alias = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    mappeddomain = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    version = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'2'"),
                    version_changed = table.Column<DateTime>(type: "datetime", nullable: true),
                    language = table.Column<string>(type: "char(10)", nullable: false, defaultValueSql: "'en-US'", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    timezone = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    trusteddomains = table.Column<string>(type: "varchar(1024)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    trusteddomainsenabled = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    status = table.Column<int>(type: "int", nullable: false),
                    statuschanged = table.Column<DateTime>(type: "datetime", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    owner_id = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    payment_id = table.Column<string>(type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    industry = table.Column<int>(type: "int", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "mappeddomain", "name", "owner_id", "payment_id", "status", "statuschanged", "timezone", "trusteddomains", "version_changed" },
                values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), null, null, "Web Office", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", null, 0, null, null, null, null });

            migrationBuilder.CreateIndexSafely(
                name: "expires_on",
                table: "backup_backup",
                column: "expires_on");

            migrationBuilder.CreateIndexSafely(
                name: "is_scheduled",
                table: "backup_backup",
                column: "is_scheduled");

            migrationBuilder.CreateIndexSafely(
                name: "tenant_id",
                table: "backup_backup",
                column: "tenant_id");

            migrationBuilder.CreateIndexSafely(
                name: "last_modified",
                table: "tenants_tenants",
                column: "last_modified");

            migrationBuilder.CreateIndexSafely(
                name: "mappeddomain",
                table: "tenants_tenants",
                column: "mappeddomain");

            migrationBuilder.CreateIndexSafely(
                name: "version",
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
                name: "tenants_tenants");
        }
    }
}
