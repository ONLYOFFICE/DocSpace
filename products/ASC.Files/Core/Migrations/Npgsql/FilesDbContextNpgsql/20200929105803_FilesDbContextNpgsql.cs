using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Files.Core.Migrations.Npgsql.FilesDbContextNpgsql
{
    public partial class FilesDbContextNpgsql : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "encrypted_data",
                schema: "onlyoffice",
                columns: table => new
                {
                    public_key = table.Column<string>(fixedLength: true, maxLength: 64, nullable: false),
                    file_hash = table.Column<string>(fixedLength: true, maxLength: 66, nullable: false),
                    data = table.Column<string>(fixedLength: true, maxLength: 112, nullable: false),
                    create_on = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    tenant_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("encrypted_data_pkey", x => new { x.public_key, x.file_hash });
                });

            migrationBuilder.CreateTable(
                name: "files_bunch_objects",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    right_node = table.Column<string>(maxLength: 255, nullable: false),
                    left_node = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_bunch_objects_pkey", x => new { x.tenant_id, x.right_node });
                });

            migrationBuilder.CreateTable(
                name: "files_file",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    version = table.Column<int>(nullable: false),
                    tenant_id = table.Column<int>(nullable: false),
                    version_group = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    current_version = table.Column<bool>(nullable: false),
                    folder_id = table.Column<int>(nullable: false),
                    title = table.Column<string>(maxLength: 400, nullable: false),
                    content_length = table.Column<long>(nullable: false, defaultValueSql: "'0'::bigint"),
                    file_status = table.Column<int>(nullable: false),
                    category = table.Column<int>(nullable: false),
                    create_by = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false),
                    create_on = table.Column<DateTime>(nullable: false),
                    modified_by = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false),
                    modified_on = table.Column<DateTime>(nullable: false),
                    converted_type = table.Column<string>(maxLength: 10, nullable: true, defaultValueSql: "NULL::character varying"),
                    comment = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    changes = table.Column<string>(nullable: true),
                    encrypted = table.Column<bool>(nullable: false),
                    forcesave = table.Column<int>(nullable: false),
                    thumb = table.Column<int>(nullable: false)
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
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(nullable: false),
                    title = table.Column<string>(maxLength: 400, nullable: false),
                    folder_type = table.Column<int>(nullable: false),
                    create_by = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false),
                    create_on = table.Column<DateTime>(nullable: false),
                    modified_by = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false),
                    modified_on = table.Column<DateTime>(nullable: false),
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
                schema: "onlyoffice",
                columns: table => new
                {
                    folder_id = table.Column<int>(nullable: false),
                    parent_id = table.Column<int>(nullable: false),
                    level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_folder_tree_pkey", x => new { x.parent_id, x.folder_id });
                });

            migrationBuilder.CreateTable(
                name: "files_security",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant_id = table.Column<int>(nullable: false),
                    entry_id = table.Column<string>(maxLength: 50, nullable: false),
                    entry_type = table.Column<int>(nullable: false),
                    subject = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false),
                    owner = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false),
                    security = table.Column<int>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(maxLength: 255, nullable: false),
                    owner = table.Column<Guid>(maxLength: 38, nullable: false),
                    flag = table.Column<int>(nullable: false)
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
                    tenant_id = table.Column<int>(nullable: false),
                    tag_id = table.Column<int>(nullable: false),
                    entry_type = table.Column<int>(nullable: false),
                    entry_id = table.Column<string>(maxLength: 32, nullable: false),
                    create_by = table.Column<Guid>(fixedLength: true, maxLength: 38, nullable: false, defaultValueSql: "NULL"),
                    create_on = table.Column<DateTime>(nullable: false),
                    tag_count = table.Column<int>(nullable: false)
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
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider = table.Column<string>(maxLength: 50, nullable: false, defaultValueSql: "'0'::character varying"),
                    customer_title = table.Column<string>(maxLength: 400, nullable: false),
                    user_name = table.Column<string>(maxLength: 100, nullable: false),
                    password = table.Column<string>(maxLength: 512, nullable: false),
                    token = table.Column<string>(nullable: true),
                    user_id = table.Column<Guid>(maxLength: 38, nullable: false),
                    folder_type = table.Column<int>(nullable: false),
                    create_on = table.Column<DateTime>(nullable: false),
                    url = table.Column<string>(nullable: true),
                    tenant_id = table.Column<int>(nullable: false)
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
                    user_id = table.Column<Guid>(maxLength: 38, nullable: false),
                    app = table.Column<string>(maxLength: 50, nullable: false),
                    token = table.Column<string>(nullable: true),
                    tenant_id = table.Column<int>(nullable: false),
                    modified_on = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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
                    hash_id = table.Column<string>(fixedLength: true, maxLength: 32, nullable: false),
                    tenant_id = table.Column<int>(nullable: false),
                    id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("files_thirdparty_id_mapping_pkey", x => x.hash_id);
                });



            migrationBuilder.CreateIndex(
                name: "tenant_id_encrypted_data",
                schema: "onlyoffice",
                table: "encrypted_data",
                column: "tenant_id");

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
                name: "index_1",
                schema: "onlyoffice",
                table: "files_thirdparty_id_mapping",
                columns: new[] { "tenant_id", "hash_id" });


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_partners");

            migrationBuilder.DropTable(
                name: "encrypted_data",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "files_bunch_objects",
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
                name: "tenants_tenants",
                schema: "onlyoffice");
        }
    }
}
