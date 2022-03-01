namespace ASC.Core.Common.Migrations.MySql.TenantDbContextMySql;

public partial class TenantDbContextMySql : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "core_settings",
            columns: table => new
            {
                tenant = table.Column<int>(type: "int", nullable: false),
                id = table.Column<string>(type: "varchar(128)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                value = table.Column<byte[]>(type: "mediumblob", nullable: false),
                last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenant, x.id });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "tenants_forbiden",
            columns: table => new
            {
                address = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.address);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "tenants_iprestrictions",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                tenant = table.Column<int>(type: "int", nullable: false),
                ip = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tenants_iprestrictions", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "tenants_partners",
            columns: table => new
            {
                tenant_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                partner_id = table.Column<string>(type: "varchar(36)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                affiliate_id = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                campaign = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
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

        migrationBuilder.CreateTable(
            name: "tenants_version",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                version = table.Column<string>(type: "varchar(64)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                url = table.Column<string>(type: "varchar(64)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                default_version = table.Column<int>(type: "int", nullable: false),
                visible = table.Column<bool>(type: "tinyint(1)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tenants_version", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.InsertData(
            table: "core_settings",
            columns: new[] { "id", "tenant", "value" },
            values: new object[,]
            {
                    { "CompanyWhiteLabelSettings", -1, new byte[] { 245, 71, 4, 138, 72, 101, 23, 21, 135, 217, 206, 188, 138, 73, 108, 96, 29, 150, 3, 31, 44, 28, 62, 145, 96, 53, 57, 66, 238, 118, 93, 172, 211, 22, 244, 181, 244, 40, 146, 67, 111, 196, 162, 27, 154, 109, 248, 255, 211, 188, 64, 54, 180, 126, 58, 90, 27, 76, 136, 27, 38, 96, 152, 105, 254, 187, 104, 72, 189, 136, 192, 46, 234, 198, 164, 204, 179, 232, 244, 4, 41, 8, 18, 240, 230, 225, 36, 165, 82, 190, 129, 165, 140, 100, 187, 139, 211, 201, 168, 192, 237, 225, 249, 66, 18, 129, 222, 12, 122, 248, 39, 51, 164, 188, 229, 21, 232, 86, 148, 196, 221, 167, 142, 34, 101, 43, 162, 137, 31, 206, 149, 120, 249, 114, 133, 168, 30, 18, 254, 223, 93, 101, 88, 97, 30, 58, 163, 224, 62, 173, 220, 170, 152, 40, 124, 100, 165, 81, 7, 87, 168, 129, 176, 12, 51, 69, 230, 252, 30, 34, 182, 7, 202, 45, 117, 60, 99, 241, 237, 148, 201, 35, 102, 219, 160, 228, 194, 230, 219, 22, 244, 74, 138, 176, 145, 0, 122, 167, 80, 93, 23, 228, 21, 48, 100, 60, 31, 250, 232, 34, 248, 249, 159, 210, 227, 12, 13, 239, 130, 223, 101, 196, 51, 36, 80, 127, 62, 92, 104, 228, 197, 226, 43, 232, 164, 12, 36, 66, 52, 133 } },
                    { "FullTextSearchSettings", -1, new byte[] { 8, 120, 207, 5, 153, 181, 23, 202, 162, 211, 218, 237, 157, 6, 76, 62, 220, 238, 175, 67, 31, 53, 166, 246, 66, 220, 173, 160, 72, 23, 227, 81, 50, 39, 187, 177, 222, 110, 43, 171, 235, 158, 16, 119, 178, 207, 49, 140, 72, 152, 20, 84, 94, 135, 117, 1, 246, 51, 251, 190, 148, 2, 44, 252, 221, 2, 91, 83, 149, 151, 58, 245, 16, 148, 52, 8, 187, 86, 150, 46, 227, 93, 163, 95, 47, 131, 116, 207, 95, 209, 38, 149, 53, 148, 73, 215, 206, 251, 194, 199, 189, 17, 42, 229, 135, 82, 23, 154, 162, 165, 158, 94, 23, 128, 30, 88, 12, 204, 96, 250, 236, 142, 189, 211, 214, 18, 196, 136, 102, 102, 217, 109, 108, 240, 96, 96, 94, 100, 201, 10, 31, 170, 128, 192 } },
                    { "SmtpSettings", -1, new byte[] { 240, 82, 224, 144, 161, 163, 117, 13, 173, 205, 78, 153, 97, 218, 4, 170, 81, 239, 1, 151, 226, 192, 98, 60, 241, 44, 88, 56, 191, 164, 10, 155, 72, 186, 239, 203, 227, 113, 88, 119, 49, 215, 227, 220, 158, 124, 96, 9, 116, 47, 158, 65, 93, 86, 219, 15, 10, 224, 142, 50, 248, 144, 75, 44, 68, 28, 198, 87, 198, 69, 67, 234, 238, 38, 32, 68, 162, 139, 67, 53, 220, 176, 240, 196, 233, 64, 29, 137, 31, 160, 99, 105, 249, 132, 202, 45, 71, 92, 134, 194, 55, 145, 121, 97, 197, 130, 119, 105, 131, 21, 133, 35, 10, 102, 172, 119, 135, 230, 251, 86, 253, 62, 55, 56, 146, 103, 164, 106 } }
            });

        migrationBuilder.InsertData(
            table: "tenants_forbiden",
            column: "address",
            values: new object[]
            {
                    "controlpanel",
                    "localhost"
            });

        migrationBuilder.InsertData(
            table: "tenants_tenants",
            columns: new[] { "id", "alias", "creationdatetime", "industry", "mappeddomain", "name", "owner_id", "payment_id", "status", "statuschanged", "timezone", "trusteddomains", "version_changed" },
            values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), null, null, "Web Office", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", null, 0, null, null, null, null });

        migrationBuilder.CreateIndex(
            name: "tenant",
            table: "tenants_iprestrictions",
            column: "tenant");

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
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "core_settings");

        migrationBuilder.DropTable(
            name: "tenants_forbiden");

        migrationBuilder.DropTable(
            name: "tenants_iprestrictions");

        migrationBuilder.DropTable(
            name: "tenants_partners");

        migrationBuilder.DropTable(
            name: "tenants_tenants");

        migrationBuilder.DropTable(
            name: "tenants_version");
    }
}
