using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations
{
    public partial class FilesDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "files_bunch_objects",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    right_node = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    left_node = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.right_node });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_converts",
                columns: table => new
                {
                    input = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    output = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.input, x.output });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_file",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    version_group = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    current_version = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'0'"),
                    folder_id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    title = table.Column<string>(type: "varchar(400)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    content_length = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    file_status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    category = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    create_by = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    modified_by = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    modified_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    converted_type = table.Column<string>(type: "varchar(10)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    comment = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    changes = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    encrypted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'0'"),
                    forcesave = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    thumb = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.id, x.version });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_folder",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    parent_id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    title = table.Column<string>(type: "varchar(400)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    folder_type = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    create_by = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    modified_by = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    modified_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    foldersCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    filesCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    @private = table.Column<bool>(name: "private", type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_folder", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_folder_tree",
                columns: table => new
                {
                    folder_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.parent_id, x.folder_id });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_link",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    source_id = table.Column<string>(type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    linked_id = table.Column<string>(type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    linked_for = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.source_id, x.linked_id });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_properties",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    entry_id = table.Column<string>(type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    data = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.entry_id });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "files_security",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    entry_id = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    entry_type = table.Column<int>(type: "int", nullable: false),
                    subject = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    subject_type = table.Column<int>(type: "int", nullable: false),
                    owner = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    security = table.Column<int>(type: "int", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    options = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.entry_id, x.entry_type, x.subject });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_tag",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    owner = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    flag = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_tag", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_tag_link",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    tag_id = table.Column<int>(type: "int", nullable: false),
                    entry_type = table.Column<int>(type: "int", nullable: false),
                    entry_id = table.Column<string>(type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    create_by = table.Column<string>(type: "char(38)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    tag_count = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.tag_id, x.entry_id, x.entry_type });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_thirdparty_account",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    provider = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "'0'", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    customer_title = table.Column<string>(type: "varchar(400)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    user_name = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    password = table.Column<string>(type: "varchar(512)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    token = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    user_id = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    folder_type = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    room_type = table.Column<int>(type: "int", nullable: false),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    folder_id = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    @private = table.Column<bool>(name: "private", type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_thirdparty_account", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_thirdparty_app",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    app = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    token = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    modified_on = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.user_id, x.app });
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "files_thirdparty_id_mapping",
                columns: table => new
                {
                    hash_id = table.Column<string>(type: "char(32)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.hash_id);
                })
                .Annotation("MySql:CharSet", "utf8");

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
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    statuschanged = table.Column<DateTime>(type: "datetime", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    owner_id = table.Column<string>(type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    payment_id = table.Column<string>(type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    industry = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    spam = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    calls = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.InsertData(
                table: "files_converts",
                columns: new[] { "input", "output" },
                values: new object[,]
                {
                    { ".csv", ".ods" },
                    { ".csv", ".ots" },
                    { ".csv", ".pdf" },
                    { ".csv", ".xlsm" },
                    { ".csv", ".xlsx" },
                    { ".csv", ".xltm" },
                    { ".csv", ".xltx" },
                    { ".doc", ".docm" },
                    { ".doc", ".docx" },
                    { ".doc", ".dotm" },
                    { ".doc", ".dotx" },
                    { ".doc", ".epub" },
                    { ".doc", ".fb2" },
                    { ".doc", ".html" },
                    { ".doc", ".odt" },
                    { ".doc", ".ott" },
                    { ".doc", ".pdf" },
                    { ".doc", ".rtf" },
                    { ".doc", ".txt" },
                    { ".docm", ".docx" },
                    { ".docm", ".dotm" },
                    { ".docm", ".dotx" },
                    { ".docm", ".epub" },
                    { ".docm", ".fb2" },
                    { ".docm", ".html" },
                    { ".docm", ".odt" },
                    { ".docm", ".ott" },
                    { ".docm", ".pdf" },
                    { ".docm", ".rtf" },
                    { ".docm", ".txt" },
                    { ".doct", ".docx" },
                    { ".docx", ".docm" },
                    { ".docx", ".docxf" },
                    { ".docx", ".dotm" },
                    { ".docx", ".dotx" },
                    { ".docx", ".epub" },
                    { ".docx", ".fb2" },
                    { ".docx", ".html" },
                    { ".docx", ".odt" },
                    { ".docx", ".ott" },
                    { ".docx", ".pdf" },
                    { ".docx", ".rtf" },
                    { ".docx", ".txt" },
                    { ".docxf", ".docx" },
                    { ".docxf", ".dotx" },
                    { ".docxf", ".epub" },
                    { ".docxf", ".fb2" },
                    { ".docxf", ".html" },
                    { ".docxf", ".odt" },
                    { ".docxf", ".oform" },
                    { ".docxf", ".ott" },
                    { ".docxf", ".pdf" },
                    { ".docxf", ".rtf" },
                    { ".docxf", ".txt" },
                    { ".dot", ".docm" },
                    { ".dot", ".docx" },
                    { ".dot", ".dotm" },
                    { ".dot", ".dotx" },
                    { ".dot", ".epub" },
                    { ".dot", ".fb2" },
                    { ".dot", ".html" },
                    { ".dot", ".odt" },
                    { ".dot", ".ott" },
                    { ".dot", ".pdf" },
                    { ".dot", ".rtf" },
                    { ".dot", ".txt" },
                    { ".dotm", ".docm" },
                    { ".dotm", ".docx" },
                    { ".dotm", ".dotx" },
                    { ".dotm", ".epub" },
                    { ".dotm", ".fb2" },
                    { ".dotm", ".html" },
                    { ".dotm", ".odt" },
                    { ".dotm", ".ott" },
                    { ".dotm", ".pdf" },
                    { ".dotm", ".rtf" },
                    { ".dotm", ".txt" },
                    { ".dotx", ".docm" },
                    { ".dotx", ".docx" },
                    { ".dotx", ".dotm" },
                    { ".dotx", ".epub" },
                    { ".dotx", ".fb2" },
                    { ".dotx", ".html" },
                    { ".dotx", ".odt" },
                    { ".dotx", ".ott" },
                    { ".dotx", ".pdf" },
                    { ".dotx", ".rtf" },
                    { ".dotx", ".txt" },
                    { ".epub", ".docm" },
                    { ".epub", ".docx" },
                    { ".epub", ".dotm" },
                    { ".epub", ".dotx" },
                    { ".epub", ".fb2" },
                    { ".epub", ".html" },
                    { ".epub", ".odt" },
                    { ".epub", ".ott" },
                    { ".epub", ".pdf" },
                    { ".epub", ".rtf" },
                    { ".epub", ".txt" },
                    { ".fb2", ".docm" },
                    { ".fb2", ".docx" },
                    { ".fb2", ".dotm" },
                    { ".fb2", ".dotx" },
                    { ".fb2", ".epub" },
                    { ".fb2", ".html" },
                    { ".fb2", ".odt" },
                    { ".fb2", ".ott" },
                    { ".fb2", ".pdf" },
                    { ".fb2", ".rtf" },
                    { ".fb2", ".txt" },
                    { ".fodp", ".odp" },
                    { ".fodp", ".otp" },
                    { ".fodp", ".pdf" },
                    { ".fodp", ".potm" },
                    { ".fodp", ".potx" },
                    { ".fodp", ".pptm" },
                    { ".fodp", ".pptx" },
                    { ".fods", ".csv" },
                    { ".fods", ".ods" },
                    { ".fods", ".ots" },
                    { ".fods", ".pdf" },
                    { ".fods", ".xlsm" },
                    { ".fods", ".xlsx" },
                    { ".fods", ".xltm" },
                    { ".fods", ".xltx" },
                    { ".fodt", ".docm" },
                    { ".fodt", ".docx" },
                    { ".fodt", ".dotm" },
                    { ".fodt", ".dotx" },
                    { ".fodt", ".epub" },
                    { ".fodt", ".fb2" },
                    { ".fodt", ".html" },
                    { ".fodt", ".odt" },
                    { ".fodt", ".ott" },
                    { ".fodt", ".pdf" },
                    { ".fodt", ".rtf" },
                    { ".fodt", ".txt" },
                    { ".html", ".docm" },
                    { ".html", ".docx" },
                    { ".html", ".dotm" },
                    { ".html", ".dotx" },
                    { ".html", ".epub" },
                    { ".html", ".fb2" },
                    { ".html", ".odt" },
                    { ".html", ".ott" },
                    { ".html", ".pdf" },
                    { ".html", ".rtf" },
                    { ".html", ".txt" },
                    { ".mht", ".docm" },
                    { ".mht", ".docx" },
                    { ".mht", ".dotm" },
                    { ".mht", ".dotx" },
                    { ".mht", ".epub" },
                    { ".mht", ".fb2" },
                    { ".mht", ".odt" },
                    { ".mht", ".ott" },
                    { ".mht", ".pdf" },
                    { ".mht", ".rtf" },
                    { ".mht", ".txt" },
                    { ".odp", ".otp" },
                    { ".odp", ".pdf" },
                    { ".odp", ".potm" },
                    { ".odp", ".potx" },
                    { ".odp", ".pptm" },
                    { ".odp", ".pptx" },
                    { ".ods", ".csv" },
                    { ".ods", ".ots" },
                    { ".ods", ".pdf" },
                    { ".ods", ".xlsm" },
                    { ".ods", ".xlsx" },
                    { ".ods", ".xltm" },
                    { ".ods", ".xltx" },
                    { ".odt", ".docm" },
                    { ".odt", ".docx" },
                    { ".odt", ".dotm" },
                    { ".odt", ".dotx" },
                    { ".odt", ".epub" },
                    { ".odt", ".fb2" },
                    { ".odt", ".html" },
                    { ".odt", ".ott" },
                    { ".odt", ".pdf" },
                    { ".odt", ".rtf" },
                    { ".odt", ".txt" },
                    { ".otp", ".odp" },
                    { ".otp", ".pdf" },
                    { ".otp", ".potm" },
                    { ".otp", ".potx" },
                    { ".otp", ".pptm" },
                    { ".otp", ".pptx" },
                    { ".ots", ".csv" },
                    { ".ots", ".ods" },
                    { ".ots", ".pdf" },
                    { ".ots", ".xlsm" },
                    { ".ots", ".xlsx" },
                    { ".ots", ".xltm" },
                    { ".ots", ".xltx" },
                    { ".ott", ".docm" },
                    { ".ott", ".docx" },
                    { ".ott", ".dotm" },
                    { ".ott", ".dotx" },
                    { ".ott", ".epub" },
                    { ".ott", ".fb2" },
                    { ".ott", ".html" },
                    { ".ott", ".odt" },
                    { ".ott", ".pdf" },
                    { ".ott", ".rtf" },
                    { ".ott", ".txt" },
                    { ".oxps", ".pdf" },
                    { ".pot", ".odp" },
                    { ".pot", ".otp" },
                    { ".pot", ".pdf" },
                    { ".pot", ".potm" },
                    { ".pot", ".potx" },
                    { ".pot", ".pptm" },
                    { ".pot", ".pptx" },
                    { ".potm", ".odp" },
                    { ".potm", ".otp" },
                    { ".potm", ".pdf" },
                    { ".potm", ".potx" },
                    { ".potm", ".pptm" },
                    { ".potm", ".pptx" },
                    { ".potx", ".odp" },
                    { ".potx", ".otp" },
                    { ".potx", ".pdf" },
                    { ".potx", ".potm" },
                    { ".potx", ".pptm" },
                    { ".potx", ".pptx" },
                    { ".pps", ".odp" },
                    { ".pps", ".otp" },
                    { ".pps", ".pdf" },
                    { ".pps", ".potm" },
                    { ".pps", ".potx" },
                    { ".pps", ".pptm" },
                    { ".pps", ".pptx" },
                    { ".ppsm", ".odp" },
                    { ".ppsm", ".otp" },
                    { ".ppsm", ".pdf" },
                    { ".ppsm", ".potm" },
                    { ".ppsm", ".potx" },
                    { ".ppsm", ".pptm" },
                    { ".ppsm", ".pptx" },
                    { ".ppsx", ".odp" },
                    { ".ppsx", ".otp" },
                    { ".ppsx", ".pdf" },
                    { ".ppsx", ".potm" },
                    { ".ppsx", ".potx" },
                    { ".ppsx", ".pptm" },
                    { ".ppsx", ".pptx" },
                    { ".ppt", ".odp" },
                    { ".ppt", ".otp" },
                    { ".ppt", ".pdf" },
                    { ".ppt", ".potm" },
                    { ".ppt", ".potx" },
                    { ".ppt", ".pptm" },
                    { ".ppt", ".pptx" },
                    { ".pptm", ".odp" },
                    { ".pptm", ".otp" },
                    { ".pptm", ".pdf" },
                    { ".pptm", ".potm" },
                    { ".pptm", ".potx" },
                    { ".pptm", ".pptx" },
                    { ".pptt", ".pptx" },
                    { ".pptx", ".odp" },
                    { ".pptx", ".otp" },
                    { ".pptx", ".pdf" },
                    { ".pptx", ".potm" },
                    { ".pptx", ".potx" },
                    { ".pptx", ".pptm" },
                    { ".rtf", ".docm" },
                    { ".rtf", ".docx" },
                    { ".rtf", ".dotm" },
                    { ".rtf", ".dotx" },
                    { ".rtf", ".epub" },
                    { ".rtf", ".fb2" },
                    { ".rtf", ".html" },
                    { ".rtf", ".odt" },
                    { ".rtf", ".ott" },
                    { ".rtf", ".pdf" },
                    { ".rtf", ".txt" },
                    { ".txt", ".docm" },
                    { ".txt", ".docx" },
                    { ".txt", ".dotm" },
                    { ".txt", ".dotx" },
                    { ".txt", ".epub" },
                    { ".txt", ".fb2" },
                    { ".txt", ".html" },
                    { ".txt", ".odt" },
                    { ".txt", ".ott" },
                    { ".txt", ".pdf" },
                    { ".txt", ".rtf" },
                    { ".xls", ".csv" },
                    { ".xls", ".ods" },
                    { ".xls", ".ots" },
                    { ".xls", ".pdf" },
                    { ".xls", ".xlsm" },
                    { ".xls", ".xlsx" },
                    { ".xls", ".xltm" },
                    { ".xls", ".xltx" },
                    { ".xlsm", ".csv" },
                    { ".xlsm", ".ods" },
                    { ".xlsm", ".ots" },
                    { ".xlsm", ".pdf" },
                    { ".xlsm", ".xlsx" },
                    { ".xlsm", ".xltm" },
                    { ".xlsm", ".xltx" },
                    { ".xlst", ".xlsx" },
                    { ".xlsx", ".csv" },
                    { ".xlsx", ".ods" },
                    { ".xlsx", ".ots" },
                    { ".xlsx", ".pdf" },
                    { ".xlsx", ".xlsm" },
                    { ".xlsx", ".xltm" },
                    { ".xlsx", ".xltx" },
                    { ".xlt", ".csv" },
                    { ".xlt", ".ods" },
                    { ".xlt", ".ots" },
                    { ".xlt", ".pdf" },
                    { ".xlt", ".xlsm" },
                    { ".xlt", ".xlsx" },
                    { ".xlt", ".xltm" },
                    { ".xlt", ".xltx" },
                    { ".xltm", ".csv" },
                    { ".xltm", ".ods" },
                    { ".xltm", ".ots" },
                    { ".xltm", ".pdf" },
                    { ".xltm", ".xlsm" },
                    { ".xltm", ".xlsx" },
                    { ".xltm", ".xltx" },
                    { ".xltx", ".csv" },
                    { ".xltx", ".ods" },
                    { ".xltx", ".ots" },
                    { ".xltx", ".pdf" },
                    { ".xltx", ".xlsm" },
                    { ".xltx", ".xlsx" },
                    { ".xltx", ".xltm" },
                    { ".xml", ".docm" },
                    { ".xml", ".docx" },
                    { ".xml", ".dotm" },
                    { ".xml", ".dotx" },
                    { ".xml", ".epub" },
                    { ".xml", ".fb2" },
                    { ".xml", ".html" },
                    { ".xml", ".odt" },
                    { ".xml", ".ott" },
                    { ".xml", ".pdf" },
                    { ".xml", ".rtf" },
                    { ".xml", ".txt" },
                    { ".xps", ".pdf" }
                });

            migrationBuilder.InsertData(
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "last_modified", "mappeddomain", "name", "owner_id", "payment_id", "statuschanged", "timezone", "trusteddomains", "version_changed" },
                values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Web Office", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "left_node",
                table: "files_bunch_objects",
                column: "left_node");

            migrationBuilder.CreateIndex(
                name: "folder_id",
                table: "files_file",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "id",
                table: "files_file",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "modified_on",
                table: "files_file",
                column: "modified_on");

            migrationBuilder.CreateIndex(
                name: "modified_on",
                table: "files_folder",
                column: "modified_on");

            migrationBuilder.CreateIndex(
                name: "parent_id",
                table: "files_folder",
                columns: new[] { "tenant_id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "folder_id",
                table: "files_folder_tree",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "linked_for",
                table: "files_link",
                columns: new[] { "tenant_id", "source_id", "linked_id", "linked_for" });

            migrationBuilder.CreateIndex(
                name: "owner",
                table: "files_security",
                column: "owner");

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "files_security",
                columns: new[] { "tenant_id", "entry_type", "entry_id", "owner" });

            migrationBuilder.CreateIndex(
                name: "name",
                table: "files_tag",
                columns: new[] { "tenant_id", "owner", "name", "flag" });

            migrationBuilder.CreateIndex(
                name: "create_on",
                table: "files_tag_link",
                column: "create_on");

            migrationBuilder.CreateIndex(
                name: "entry_id",
                table: "files_tag_link",
                columns: new[] { "tenant_id", "entry_id", "entry_type" });

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "files_thirdparty_account",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "index_1",
                table: "files_thirdparty_id_mapping",
                columns: new[] { "tenant_id", "hash_id" });

            migrationBuilder.CreateIndex(
                name: "alias",
                table: "tenants_tenants",
                column: "alias",
                unique: true);

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
                name: "files_bunch_objects");

            migrationBuilder.DropTable(
                name: "files_converts");

            migrationBuilder.DropTable(
                name: "files_file");

            migrationBuilder.DropTable(
                name: "files_folder");

            migrationBuilder.DropTable(
                name: "files_folder_tree");

            migrationBuilder.DropTable(
                name: "files_link");

            migrationBuilder.DropTable(
                name: "files_properties");

            migrationBuilder.DropTable(
                name: "files_security");

            migrationBuilder.DropTable(
                name: "files_tag");

            migrationBuilder.DropTable(
                name: "files_tag_link");

            migrationBuilder.DropTable(
                name: "files_thirdparty_account");

            migrationBuilder.DropTable(
                name: "files_thirdparty_app");

            migrationBuilder.DropTable(
                name: "files_thirdparty_id_mapping");

            migrationBuilder.DropTable(
                name: "tenants_tenants");
        }
    }
}
