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

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.MySql.Migrations.FilesDb;

/// <inheritdoc />
public partial class FilesDbContextMigrate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "files_bunch_objects",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                rightnode = table.Column<string>(name: "right_node", type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                leftnode = table.Column<string>(name: "left_node", type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.rightnode });
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
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                versiongroup = table.Column<int>(name: "version_group", type: "int", nullable: false, defaultValueSql: "'1'"),
                currentversion = table.Column<bool>(name: "current_version", type: "tinyint(1)", nullable: false, defaultValueSql: "'0'"),
                folderid = table.Column<int>(name: "folder_id", type: "int", nullable: false, defaultValueSql: "'0'"),
                title = table.Column<string>(type: "varchar(400)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                contentlength = table.Column<long>(name: "content_length", type: "bigint", nullable: false, defaultValueSql: "'0'"),
                filestatus = table.Column<int>(name: "file_status", type: "int", nullable: false, defaultValueSql: "'0'"),
                category = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                createby = table.Column<string>(name: "create_by", type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                createon = table.Column<DateTime>(name: "create_on", type: "datetime", nullable: false),
                modifiedby = table.Column<string>(name: "modified_by", type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                modifiedon = table.Column<DateTime>(name: "modified_on", type: "datetime", nullable: false),
                convertedtype = table.Column<string>(name: "converted_type", type: "varchar(10)", nullable: true, collation: "utf8_general_ci")
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
                table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.id, x.version });
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "files_folder",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                parentid = table.Column<int>(name: "parent_id", type: "int", nullable: false, defaultValueSql: "'0'"),
                title = table.Column<string>(type: "varchar(400)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                foldertype = table.Column<int>(name: "folder_type", type: "int", nullable: false, defaultValueSql: "'0'"),
                createby = table.Column<string>(name: "create_by", type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                createon = table.Column<DateTime>(name: "create_on", type: "datetime", nullable: false),
                modifiedby = table.Column<string>(name: "modified_by", type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                modifiedon = table.Column<DateTime>(name: "modified_on", type: "datetime", nullable: false),
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                foldersCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                filesCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                @private = table.Column<bool>(name: "private", type: "tinyint(1)", nullable: false, defaultValueSql: "'0'"),
                haslogo = table.Column<bool>(name: "has_logo", type: "tinyint(1)", nullable: false, defaultValue: false)
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
                folderid = table.Column<int>(name: "folder_id", type: "int", nullable: false),
                parentid = table.Column<int>(name: "parent_id", type: "int", nullable: false),
                level = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.parentid, x.folderid });
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "files_link",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                sourceid = table.Column<string>(name: "source_id", type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                linkedid = table.Column<string>(name: "linked_id", type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                linkedfor = table.Column<string>(name: "linked_for", type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.sourceid, x.linkedid });
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "files_properties",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                entryid = table.Column<string>(name: "entry_id", type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                data = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.entryid });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "files_security",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                entryid = table.Column<string>(name: "entry_id", type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                entrytype = table.Column<int>(name: "entry_type", type: "int", nullable: false),
                subject = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                subjecttype = table.Column<int>(name: "subject_type", type: "int", nullable: false),
                owner = table.Column<string>(type: "char(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                security = table.Column<int>(type: "int", nullable: false),
                timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                options = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.entryid, x.entrytype, x.subject });
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "files_tag",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
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
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                tagid = table.Column<int>(name: "tag_id", type: "int", nullable: false),
                entrytype = table.Column<int>(name: "entry_type", type: "int", nullable: false),
                entryid = table.Column<string>(name: "entry_id", type: "varchar(32)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                createby = table.Column<string>(name: "create_by", type: "char(38)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                createon = table.Column<DateTime>(name: "create_on", type: "datetime", nullable: true),
                tagcount = table.Column<int>(name: "tag_count", type: "int", nullable: false, defaultValueSql: "'0'")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenantid, x.tagid, x.entryid, x.entrytype });
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
                customertitle = table.Column<string>(name: "customer_title", type: "varchar(400)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                username = table.Column<string>(name: "user_name", type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                password = table.Column<string>(type: "varchar(512)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                token = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                userid = table.Column<string>(name: "user_id", type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                foldertype = table.Column<int>(name: "folder_type", type: "int", nullable: false, defaultValueSql: "'0'"),
                roomtype = table.Column<int>(name: "room_type", type: "int", nullable: false),
                createon = table.Column<DateTime>(name: "create_on", type: "datetime", nullable: false),
                url = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                folderid = table.Column<string>(name: "folder_id", type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                @private = table.Column<bool>(name: "private", type: "tinyint(1)", nullable: false),
                haslogo = table.Column<bool>(name: "has_logo", type: "tinyint(1)", nullable: false, defaultValue: false)
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
                userid = table.Column<string>(name: "user_id", type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                app = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                token = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                modifiedon = table.Column<DateTime>(name: "modified_on", type: "timestamp", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.userid, x.app });
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "files_thirdparty_id_mapping",
            columns: table => new
            {
                hashid = table.Column<string>(name: "hash_id", type: "char(32)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                tenantid = table.Column<int>(name: "tenant_id", type: "int", nullable: false),
                id = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.hashid);
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
                versionchanged = table.Column<DateTime>(name: "version_changed", type: "datetime", nullable: true),
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
                ownerid = table.Column<string>(name: "owner_id", type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                paymentid = table.Column<string>(name: "payment_id", type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                industry = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp", nullable: false),
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

    /// <inheritdoc />
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
