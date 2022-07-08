using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Files.Core.Migrations.PostgreSql.FilesDbContextPostgreSql
{
    public partial class FilesDbContextPostgreSql : Migration
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
                name: "files_file",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    version_group = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    current_version = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    folder_id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    title = table.Column<string>(type: "varchar(400)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    content_length = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    file_status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    category = table.Column<int>(type: "int", nullable: false),
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
                    encrypted = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
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
                    filesCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'")
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
                name: "files_security",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "int", nullable: false),
                    entry_id = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    entry_type = table.Column<int>(type: "int", nullable: false),
                    subject = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    owner = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    security = table.Column<int>(type: "int", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp", nullable: false)
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
                    flag = table.Column<int>(type: "int", nullable: false)
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
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    tenant_id = table.Column<int>(type: "int", nullable: false)
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
                name: "tenants_quota",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    description = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    max_file_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    max_total_size = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    active_users = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    features = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValueSql: "'0.00'"),
                    avangate_id = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    visible = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "tenants_tariff",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant = table.Column<int>(type: "int", nullable: false),
                    tariff = table.Column<int>(type: "int", nullable: false),
                    stamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    comment = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    create_on = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
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
                    owner_id = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    payment_id = table.Column<string>(type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    industry = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    spam = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    calls = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "active_users", "avangate_id", "description", "features", "max_file_size", "max_total_size", "name" },
                values: new object[] { -1, 10000, "0", null, "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default" });

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
                name: "index_1",
                table: "files_thirdparty_id_mapping",
                columns: new[] { "tenant_id", "hash_id" });

            migrationBuilder.CreateIndex(
                name: "tenant",
                table: "tenants_tariff",
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
                name: "files_bunch_objects");

            migrationBuilder.DropTable(
                name: "files_file");

            migrationBuilder.DropTable(
                name: "files_folder");

            migrationBuilder.DropTable(
                name: "files_folder_tree");

            migrationBuilder.DropTable(
                name: "files_link");

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
                name: "tenants_quota");

            migrationBuilder.DropTable(
                name: "tenants_tariff");

            migrationBuilder.DropTable(
                name: "tenants_tenants");
        }
    }
}
