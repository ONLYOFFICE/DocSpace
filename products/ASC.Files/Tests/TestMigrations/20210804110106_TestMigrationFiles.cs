// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode


namespace ASC.Core.Common.Migrations;

public partial class TestFilesMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 1, 0, "my", 5, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 1, 1, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/my/66faa6e4-f133-11ea-b126-00ffeec8b4ef", "1" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 2, 0, "share", 6, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 2, 2, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/share/66faa6e4-f133-11ea-b126-00ffeec8b4ef", "2" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 3, 0, "favorites", 10, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 3, 3, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/favorites/66faa6e4-f133-11ea-b126-00ffeec8b4ef", "3" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 4, 0, "recent", 11, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 4, 4, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/recent/66faa6e4-f133-11ea-b126-00ffeec8b4ef", "4" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 5, 0, "trash", 3, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 5, 5, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/trash/66faa6e4-f133-11ea-b126-00ffeec8b4ef", "5" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 6, 0, "my", 5, "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 6, 6, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/my/005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", "6" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 7, 0, "share", 6, "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 7, 7, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/share/005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", "7" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 8, 0, "favorites", 10, "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 8, 8, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/favorites/005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", "8" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 9, 0, "recent", 11, "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 9, 9, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/recent/005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", "9" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 10, 0, "trash", 3, "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 10, 10, 0 });

        migrationBuilder.InsertData(
            table: "files_bunch_objects",
            columns: new[] { "tenant_id", "right_node", "left_node" },
            values: new object[] { 1, "files/trash/005bb3ff-7de3-47d2-9b3d-61b9ec8a76a", "10" });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 11, 1, "subfolder1", 0, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 11, 11, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 11, 1, 1 });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 12, 1, "subfolder2", 0, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 12, 12, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 12, 1, 1 });

        migrationBuilder.InsertData(
            table: "files_folder",
            columns: new[] { "id", "parent_id", "title", "folder_type", "create_by", "create_on", "modified_by", "modified_on", "tenant_id", "foldersCount", "filesCount" },
            values: new object[] { 13, 12, "subfolder21", 0, "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), 1, 0, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 13, 13, 0 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 13, 12, 1 });

        migrationBuilder.InsertData(
            table: "files_folder_tree",
            columns: new[] { "folder_id", "parent_id", "level" },
            values: new object[] { 13, 1, 2 });

        migrationBuilder.InsertData(
            table: "files_file",
            columns: new[] {
                "id", "version", "version_group", "current_version", "folder_id", "title", "content_length", "file_status", "category", "create_by",
                "create_on", "modified_by", "modified_on", "tenant_id", "comment",  "encrypted", "forcesave", "thumb"
            },
            values: new object[] { 1, 1, 1, true, 12, "New Document.docx", 6944, 0, 3, "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
            new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), "66faa6e4-f133-11ea-b126-00ffeec8b4ef", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842),
            1, "Created", false, 0, 0});
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
