namespace ASC.MessagingSystem.Migrations.MySql.MessagesContextMySql;

public partial class MessagesContextMySql : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "audit_events",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                initiator = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                target = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                ip = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                browser = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                platform = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                date = table.Column<DateTime>(type: "datetime", nullable: false),
                tenant_id = table.Column<int>(type: "int", nullable: false),
                user_id = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                page = table.Column<string>(type: "varchar(300)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                action = table.Column<int>(type: "int", nullable: false),
                description = table.Column<string>(type: "varchar(20000)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_audit_events", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "login_events",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                login = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                ip = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                browser = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                platform = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                date = table.Column<DateTime>(type: "datetime", nullable: false),
                tenant_id = table.Column<int>(type: "int", nullable: false),
                user_id = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                page = table.Column<string>(type: "varchar(300)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                action = table.Column<int>(type: "int", nullable: false),
                description = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_login_events", x => x.id);
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

        migrationBuilder.CreateTable(
            name: "webstudio_settings",
            columns: table => new
            {
                TenantID = table.Column<int>(type: "int", nullable: false),
                ID = table.Column<string>(type: "varchar(64)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                UserID = table.Column<string>(type: "varchar(64)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                Data = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.TenantID, x.ID, x.UserID });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.InsertData(
            table: "tenants_tenants",
            columns: new[] { "id", "alias", "creationdatetime", "industry", "mappeddomain", "name", "owner_id", "payment_id", "status", "statuschanged", "timezone", "trusteddomains", "version_changed" },
            values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), null, null, "Web Office", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", null, 0, null, null, null, null });

        migrationBuilder.InsertData(
            table: "webstudio_settings",
            columns: new[] { "ID", "TenantID", "UserID", "Data" },
            values: new object[] { "9a925891-1f92-4ed7-b277-d6f649739f06", 1, "00000000-0000-0000-0000-000000000000", "{'Completed':false}" });

        migrationBuilder.CreateIndex(
            name: "date",
            table: "audit_events",
            columns: new[] { "tenant_id", "date" });

        migrationBuilder.CreateIndex(
            name: "date",
            table: "login_events",
            column: "date");

        migrationBuilder.CreateIndex(
            name: "tenant_id",
            table: "login_events",
            columns: new[] { "tenant_id", "user_id" });

        migrationBuilder.CreateIndex(
            name: "last_modified",
            table: "tenants_tenants",
            column: "last_modified");

        migrationBuilder.CreateIndex(
            name: "mappeddomain",
            table: "tenants_tenants",
            column: "mappeddomain");

        migrationBuilder.CreateIndex(
            name: "version",
            table: "tenants_tenants",
            column: "version");

        migrationBuilder.CreateIndex(
            name: "ID",
            table: "webstudio_settings",
            column: "ID");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "audit_events");

        migrationBuilder.DropTable(
            name: "login_events");

        migrationBuilder.DropTable(
            name: "tenants_tenants");

        migrationBuilder.DropTable(
            name: "webstudio_settings");
    }
}
