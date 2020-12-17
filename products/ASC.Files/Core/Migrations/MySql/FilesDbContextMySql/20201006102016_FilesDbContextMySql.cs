using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Files.Core.Migrations.MySql.FilesDbContextMySql
{
    public partial class FilesDbContextMySql : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "files_bunch_objects",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    right_node = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    left_node = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.right_node });
                });

            migrationBuilder.CreateTable(
                name: "files_file",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    version = table.Column<int>(nullable: false),
                    tenant_id = table.Column<int>(nullable: false),
                    version_group = table.Column<int>(nullable: false, defaultValueSql: "'1'"),
                    current_version = table.Column<bool>(nullable: false),
                    folder_id = table.Column<int>(nullable: false),
                    title = table.Column<string>(type: "varchar(400)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    content_length = table.Column<long>(nullable: false),
                    file_status = table.Column<int>(nullable: false),
                    category = table.Column<int>(nullable: false),
                    create_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    modified_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    modified_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    converted_type = table.Column<string>(type: "varchar(10)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    comment = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    changes = table.Column<string>(type: "mediumtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    encrypted = table.Column<bool>(nullable: false),
                    forcesave = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.id, x.version });
                });

            migrationBuilder.CreateTable(
                name: "files_folder",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    parent_id = table.Column<int>(nullable: false),
                    title = table.Column<string>(type: "varchar(400)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    folder_type = table.Column<int>(nullable: false),
                    create_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    modified_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    modified_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    tenant_id = table.Column<int>(nullable: false),
                    foldersCount = table.Column<int>(nullable: false),
                    filesCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_folder", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files_folder_tree",
                columns: table => new
                {
                    folder_id = table.Column<int>(nullable: false),
                    parent_id = table.Column<int>(nullable: false),
                    level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.parent_id, x.folder_id });
                });

            migrationBuilder.CreateTable(
                name: "files_security",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    entry_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    entry_type = table.Column<int>(nullable: false),
                    subject = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    owner = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    security = table.Column<int>(nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.entry_id, x.entry_type, x.subject });
                });

            migrationBuilder.CreateTable(
                name: "files_tag",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    owner = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    flag = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files_tag_link",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    tag_id = table.Column<int>(nullable: false),
                    entry_type = table.Column<int>(nullable: false),
                    entry_id = table.Column<string>(type: "varchar(32)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    create_by = table.Column<string>(type: "char(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    tag_count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant_id, x.tag_id, x.entry_id, x.entry_type });
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_account",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    provider = table.Column<string>(type: "varchar(50)", nullable: false, defaultValueSql: "'0'")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    customer_title = table.Column<string>(type: "varchar(400)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    user_name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    password = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    token = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    user_id = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    folder_type = table.Column<int>(nullable: false),
                    create_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files_thirdparty_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_app",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    app = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    token = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant_id = table.Column<int>(nullable: false),
                    modified_on = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.user_id, x.app });
                });

            migrationBuilder.CreateTable(
                name: "files_thirdparty_id_mapping",
                columns: table => new
                {
                    hash_id = table.Column<string>(type: "char(32)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    tenant_id = table.Column<int>(nullable: false),
                    id = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.hash_id);
                });


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

        }
    }
}
