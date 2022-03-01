namespace ASC.Core.Common.Migrations.PostgreSql.ResourceDbContextPostgreSql;

public partial class ResourceDbContextPostgreSql : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "res_authors",
            columns: table => new
            {
                login = table.Column<string>(type: "varchar(150)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                password = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                isAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                online = table.Column<bool>(type: "tinyint(1)", nullable: false),
                lastVisit = table.Column<DateTime>(type: "datetime", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.login);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "res_authorsfile",
            columns: table => new
            {
                authorLogin = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                fileid = table.Column<int>(type: "int", nullable: false),
                writeAccess = table.Column<bool>(type: "tinyint(1)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.authorLogin, x.fileid });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "res_authorslang",
            columns: table => new
            {
                authorLogin = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                cultureTitle = table.Column<string>(type: "varchar(20)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.authorLogin, x.cultureTitle });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "res_cultures",
            columns: table => new
            {
                title = table.Column<string>(type: "varchar(120)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                value = table.Column<string>(type: "varchar(120)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                available = table.Column<bool>(type: "tinyint(1)", nullable: false),
                creationDate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.title);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "res_data",
            columns: table => new
            {
                fileid = table.Column<int>(type: "int", nullable: false),
                title = table.Column<string>(type: "varchar(120)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                cultureTitle = table.Column<string>(type: "varchar(20)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                textValue = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                description = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                timeChanges = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                resourceType = table.Column<string>(type: "varchar(20)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                flag = table.Column<int>(type: "int", nullable: false),
                link = table.Column<string>(type: "varchar(120)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                authorLogin = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "'Console'", collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.fileid, x.cultureTitle, x.title });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "res_files",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                projectName = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                moduleName = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                resName = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                isLock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                lastUpdate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                creationDate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "'0000-00-00 00:00:00'")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_res_files", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "res_reserve",
            columns: table => new
            {
                fileid = table.Column<int>(type: "int", nullable: false),
                title = table.Column<string>(type: "varchar(120)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                cultureTitle = table.Column<string>(type: "varchar(20)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                textValue = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                flag = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.fileid, x.title, x.cultureTitle });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "res_authorsfile_FK2",
            table: "res_authorsfile",
            column: "fileid");

        migrationBuilder.CreateIndex(
            name: "res_authorslang_FK2",
            table: "res_authorslang",
            column: "cultureTitle");

        migrationBuilder.CreateIndex(
            name: "dateIndex",
            table: "res_data",
            column: "timeChanges");

        migrationBuilder.CreateIndex(
            name: "id",
            table: "res_data",
            column: "id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "resources_FK2",
            table: "res_data",
            column: "cultureTitle");

        migrationBuilder.CreateIndex(
            name: "resname",
            table: "res_files",
            column: "resName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "id",
            table: "res_reserve",
            column: "id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "resources_FK2",
            table: "res_reserve",
            column: "cultureTitle");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "res_authors");

        migrationBuilder.DropTable(
            name: "res_authorsfile");

        migrationBuilder.DropTable(
            name: "res_authorslang");

        migrationBuilder.DropTable(
            name: "res_cultures");

        migrationBuilder.DropTable(
            name: "res_data");

        migrationBuilder.DropTable(
            name: "res_files");

        migrationBuilder.DropTable(
            name: "res_reserve");
    }
}
