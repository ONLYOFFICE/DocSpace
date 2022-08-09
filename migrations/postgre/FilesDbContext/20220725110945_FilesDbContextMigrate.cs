using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class FilesDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "files_bunch_objects",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    right_node = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    left_node = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_bunch_objects_pkey", x => new { x.tenant_id, x.right_node });
                });

            migrationBuilder.CreateTable(
                name: "files_converts",
                schema: "onlyoffice",
                columns: table => new
                {
                    input = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    output = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_converts_pkey", x => new { x.input, x.output });
                });

            migrationBuilder.CreateTable(
                name: "files_file",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    version_group = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    current_version = table.Column<bool>(type: "boolean", nullable: false),
                    folder_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    content_length = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'::bigint"),
                    file_status = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    create_by = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    create_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    converted_type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, defaultValueSql: "NULL::character varying"),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    changes = table.Column<string>(type: "text", nullable: true),
                    encrypted = table.Column<bool>(type: "boolean", nullable: false),
                    forcesave = table.Column<int>(type: "integer", nullable: false),
                    thumb = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_file_pkey", x => new { x.id, x.tenant_id, x.version });
                });

            migrationBuilder.CreateTable(
                name: "files_folder",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    folder_type = table.Column<int>(type: "integer", nullable: false),
                    create_by = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    create_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    foldersCount = table.Column<int>(type: "integer", nullable: false),
                    filesCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_folder", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files_folder_tree",
                schema: "onlyoffice",
                columns: table => new
                {
                    folder_id = table.Column<int>(type: "integer", nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_folder_tree_pkey", x => new { x.parent_id, x.folder_id });
                });

            migrationBuilder.CreateTable(
                name: "files_link",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    linked_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    linked_for = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false, defaultValueSql: "NULL::bpchar")
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_link_pkey", x => new { x.tenant_id, x.source_id, x.linked_id });
                });

            migrationBuilder.CreateTable(
                name: "files_properties",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    entry_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_properties_pkey", x => new { x.tenant_id, x.entry_id });
                });

            migrationBuilder.CreateTable(
                name: "files_security",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    entry_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entry_type = table.Column<int>(type: "integer", nullable: false),
                    subject = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    owner = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    security = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_security_pkey", x => new { x.tenant_id, x.entry_id, x.entry_type, x.subject });
                });

            migrationBuilder.CreateTable(
                name: "files_tag",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    owner = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    flag = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files_tag_link",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: false),
                    entry_type = table.Column<int>(type: "integer", nullable: false),
                    entry_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    create_by = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: true, defaultValueSql: "NULL::bpchar"),
                    create_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tag_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_tag_link_pkey", x => new { x.tenant_id, x.tag_id, x.entry_type, x.entry_id });
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_account",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "'0'::character varying"),
                    customer_title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    token = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    folder_type = table.Column<int>(type: "integer", nullable: false),
                    room_type = table.Column<int>(type: "integer", nullable: false),
                    create_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    FolderId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_thirdparty_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_app",
                schema: "onlyoffice",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 38, nullable: false),
                    app = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    token = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_thirdparty_app_pkey", x => new { x.user_id, x.app });
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_id_mapping",
                schema: "onlyoffice",
                columns: table => new
                {
                    hash_id = table.Column<string>(type: "character(32)", fixedLength: true, maxLength: 32, nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_thirdparty_id_mapping_pkey", x => x.hash_id);
                });

            migrationBuilder.CreateTable(
                name: "tenants_quota",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "character varying", nullable: true),
                    max_file_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    max_total_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    active_users = table.Column<int>(type: "integer", nullable: false),
                    features = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValueSql: "0.00"),
                    avangate_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true, defaultValueSql: "NULL"),
                    visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_quota_pkey", x => x.tenant);
                });

            migrationBuilder.CreateTable(
                name: "tenants_tariff",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    tariff = table.Column<int>(type: "integer", nullable: false),
                    stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    create_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
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
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "active_users", "avangate_id", "description", "features", "max_file_size", "max_total_size", "name", "visible" },
                values: new object[] { -1, 10000, "0", null, "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default", false });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, null });

            migrationBuilder.CreateIndex(
                name: "left_node",
                schema: "onlyoffice",
                table: "files_bunch_objects",
                column: "left_node");

            migrationBuilder.CreateIndex(
                name: "folder_id",
                schema: "onlyoffice",
                table: "files_file",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "id",
                schema: "onlyoffice",
                table: "files_file",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "modified_on_files_file",
                schema: "onlyoffice",
                table: "files_file",
                column: "modified_on");

            migrationBuilder.CreateIndex(
                name: "modified_on_files_folder",
                schema: "onlyoffice",
                table: "files_folder",
                column: "modified_on");

            migrationBuilder.CreateIndex(
                name: "parent_id",
                schema: "onlyoffice",
                table: "files_folder",
                columns: new[] { "tenant_id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "folder_id_files_folder_tree",
                schema: "onlyoffice",
                table: "files_folder_tree",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "linked_for_files_link",
                schema: "onlyoffice",
                table: "files_link",
                columns: new[] { "tenant_id", "source_id", "linked_id", "linked_for" });

            migrationBuilder.CreateIndex(
                name: "owner",
                schema: "onlyoffice",
                table: "files_security",
                column: "owner");

            migrationBuilder.CreateIndex(
                name: "tenant_id_files_security",
                schema: "onlyoffice",
                table: "files_security",
                columns: new[] { "entry_id", "tenant_id", "entry_type", "owner" });

            migrationBuilder.CreateIndex(
                name: "name_files_tag",
                schema: "onlyoffice",
                table: "files_tag",
                columns: new[] { "tenant_id", "owner", "name", "flag" });

            migrationBuilder.CreateIndex(
                name: "create_on_files_tag_link",
                schema: "onlyoffice",
                table: "files_tag_link",
                column: "create_on");

            migrationBuilder.CreateIndex(
                name: "entry_id",
                schema: "onlyoffice",
                table: "files_tag_link",
                columns: new[] { "tenant_id", "entry_type", "entry_id" });

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_account",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "index_1",
                schema: "onlyoffice",
                table: "files_thirdparty_id_mapping",
                columns: new[] { "tenant_id", "hash_id" });

            migrationBuilder.CreateIndex(
                name: "tenant_tenants_tariff",
                schema: "onlyoffice",
                table: "tenants_tariff",
                column: "tenant");

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
                name: "files_bunch_objects",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_converts",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_file",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_folder",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_folder_tree",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_link",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_properties",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_security",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_tag",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_tag_link",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_thirdparty_account",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_thirdparty_app",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_thirdparty_id_mapping",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_quota",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tariff",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
