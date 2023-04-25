using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.FilesDb
{
    /// <inheritdoc />
    public partial class FilesDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[] { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_files_thirdparty_app_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_app",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_files_file_tenant_id",
                schema: "onlyoffice",
                table: "files_file",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_files_bunch_objects_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_bunch_objects",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_file_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_file",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_folder_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_folder",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_folder_tree_files_folder_folder_id",
                schema: "onlyoffice",
                table: "files_folder_tree",
                column: "folder_id",
                principalSchema: "onlyoffice",
                principalTable: "files_folder",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_link_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_link",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_properties_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_properties",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_security_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_security",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_tag_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_tag",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_tag_link_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_tag_link",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_thirdparty_account_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_account",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_thirdparty_app_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_app",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_files_thirdparty_id_mapping_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_id_mapping",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_files_bunch_objects_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_bunch_objects");

            migrationBuilder.DropForeignKey(
                name: "FK_files_file_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_file");

            migrationBuilder.DropForeignKey(
                name: "FK_files_folder_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_folder");

            migrationBuilder.DropForeignKey(
                name: "FK_files_folder_tree_files_folder_folder_id",
                schema: "onlyoffice",
                table: "files_folder_tree");

            migrationBuilder.DropForeignKey(
                name: "FK_files_link_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_link");

            migrationBuilder.DropForeignKey(
                name: "FK_files_properties_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_properties");

            migrationBuilder.DropForeignKey(
                name: "FK_files_security_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_security");

            migrationBuilder.DropForeignKey(
                name: "FK_files_tag_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_tag");

            migrationBuilder.DropForeignKey(
                name: "FK_files_tag_link_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_tag_link");

            migrationBuilder.DropForeignKey(
                name: "FK_files_thirdparty_account_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_account");

            migrationBuilder.DropForeignKey(
                name: "FK_files_thirdparty_app_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_app");

            migrationBuilder.DropForeignKey(
                name: "FK_files_thirdparty_id_mapping_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_id_mapping");

            migrationBuilder.DropIndex(
                name: "IX_files_thirdparty_app_tenant_id",
                schema: "onlyoffice",
                table: "files_thirdparty_app");

            migrationBuilder.DropIndex(
                name: "IX_files_file_tenant_id",
                schema: "onlyoffice",
                table: "files_file");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);
        }
    }
}
