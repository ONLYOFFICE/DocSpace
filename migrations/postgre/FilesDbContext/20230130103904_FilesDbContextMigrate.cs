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

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.Migrations.FilesDb;

/// <inheritdoc />
public partial class FilesDbContextMigrate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "onlyoffice");

        migrationBuilder.CreateTable(
            name: "files_bunch_objects",
            schema: "onlyoffice",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                rightnode = table.Column<string>(name: "right_node", type: "character varying(255)", maxLength: 255, nullable: false),
                leftnode = table.Column<string>(name: "left_node", type: "character varying(255)", maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("files_bunch_objects_pkey", x => new { x.tenantid, x.rightnode });
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
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                versiongroup = table.Column<int>(name: "version_group", type: "integer", nullable: false, defaultValueSql: "1"),
                currentversion = table.Column<bool>(name: "current_version", type: "boolean", nullable: false),
                folderid = table.Column<int>(name: "folder_id", type: "integer", nullable: false),
                title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                contentlength = table.Column<long>(name: "content_length", type: "bigint", nullable: false, defaultValueSql: "'0'::bigint"),
                filestatus = table.Column<int>(name: "file_status", type: "integer", nullable: false),
                category = table.Column<int>(type: "integer", nullable: false),
                createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                modifiedby = table.Column<Guid>(name: "modified_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                modifiedon = table.Column<DateTime>(name: "modified_on", type: "timestamp with time zone", nullable: false),
                convertedtype = table.Column<string>(name: "converted_type", type: "character varying(10)", maxLength: 10, nullable: true, defaultValueSql: "NULL::character varying"),
                comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                changes = table.Column<string>(type: "text", nullable: true),
                encrypted = table.Column<bool>(type: "boolean", nullable: false),
                forcesave = table.Column<int>(type: "integer", nullable: false),
                thumb = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("files_file_pkey", x => new { x.id, x.tenantid, x.version });
            });

        migrationBuilder.CreateTable(
            name: "files_folder",
            schema: "onlyoffice",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                parentid = table.Column<int>(name: "parent_id", type: "integer", nullable: false),
                title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                foldertype = table.Column<int>(name: "folder_type", type: "integer", nullable: false),
                createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                modifiedby = table.Column<Guid>(name: "modified_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                modifiedon = table.Column<DateTime>(name: "modified_on", type: "timestamp with time zone", nullable: false),
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                foldersCount = table.Column<int>(type: "integer", nullable: false),
                filesCount = table.Column<int>(type: "integer", nullable: false),
                @private = table.Column<bool>(name: "private", type: "boolean", nullable: false),
                haslogo = table.Column<bool>(name: "has_logo", type: "boolean", nullable: false, defaultValue: false)
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
                folderid = table.Column<int>(name: "folder_id", type: "integer", nullable: false),
                parentid = table.Column<int>(name: "parent_id", type: "integer", nullable: false),
                level = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("files_folder_tree_pkey", x => new { x.parentid, x.folderid });
            });

        migrationBuilder.CreateTable(
            name: "files_link",
            schema: "onlyoffice",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                sourceid = table.Column<string>(name: "source_id", type: "character varying(32)", maxLength: 32, nullable: false),
                linkedid = table.Column<string>(name: "linked_id", type: "character varying(32)", maxLength: 32, nullable: false),
                linkedfor = table.Column<Guid>(name: "linked_for", type: "uuid", fixedLength: true, maxLength: 38, nullable: false, defaultValueSql: "NULL::bpchar")
            },
            constraints: table =>
            {
                table.PrimaryKey("files_link_pkey", x => new { x.tenantid, x.sourceid, x.linkedid });
            });

        migrationBuilder.CreateTable(
            name: "files_properties",
            schema: "onlyoffice",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                entryid = table.Column<string>(name: "entry_id", type: "character varying(50)", maxLength: 50, nullable: false),
                data = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("files_properties_pkey", x => new { x.tenantid, x.entryid });
            });

        migrationBuilder.CreateTable(
            name: "files_security",
            schema: "onlyoffice",
            columns: table => new
            {
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                entryid = table.Column<string>(name: "entry_id", type: "character varying(50)", maxLength: 50, nullable: false),
                entrytype = table.Column<int>(name: "entry_type", type: "integer", nullable: false),
                subject = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                subjecttype = table.Column<int>(name: "subject_type", type: "integer", nullable: false),
                owner = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                security = table.Column<int>(type: "integer", nullable: false),
                timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                options = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("files_security_pkey", x => new { x.tenantid, x.entryid, x.entrytype, x.subject });
            });

        migrationBuilder.CreateTable(
            name: "files_tag",
            schema: "onlyoffice",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
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
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                tagid = table.Column<int>(name: "tag_id", type: "integer", nullable: false),
                entrytype = table.Column<int>(name: "entry_type", type: "integer", nullable: false),
                entryid = table.Column<string>(name: "entry_id", type: "character varying(32)", maxLength: 32, nullable: false),
                createby = table.Column<Guid>(name: "create_by", type: "uuid", fixedLength: true, maxLength: 38, nullable: true, defaultValueSql: "NULL::bpchar"),
                createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: true),
                tagcount = table.Column<int>(name: "tag_count", type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("files_tag_link_pkey", x => new { x.tenantid, x.tagid, x.entrytype, x.entryid });
            });

        migrationBuilder.CreateTable(
            name: "files_thirdparty_account",
            schema: "onlyoffice",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "'0'::character varying"),
                customertitle = table.Column<string>(name: "customer_title", type: "character varying(400)", maxLength: 400, nullable: false),
                username = table.Column<string>(name: "user_name", type: "character varying(100)", maxLength: 100, nullable: false),
                password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                token = table.Column<string>(type: "text", nullable: true),
                userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 38, nullable: false),
                foldertype = table.Column<int>(name: "folder_type", type: "integer", nullable: false),
                roomtype = table.Column<int>(name: "room_type", type: "integer", nullable: false),
                createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false),
                url = table.Column<string>(type: "text", nullable: true),
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                folderid = table.Column<string>(name: "folder_id", type: "text", nullable: true),
                @private = table.Column<bool>(name: "private", type: "boolean", nullable: false),
                haslogo = table.Column<bool>(name: "has_logo", type: "boolean", nullable: false, defaultValue: false)
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
                userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 38, nullable: false),
                app = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                token = table.Column<string>(type: "text", nullable: true),
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                modifiedon = table.Column<DateTime>(name: "modified_on", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("files_thirdparty_app_pkey", x => new { x.userid, x.app });
            });

        migrationBuilder.CreateTable(
            name: "files_thirdparty_id_mapping",
            schema: "onlyoffice",
            columns: table => new
            {
                hashid = table.Column<string>(name: "hash_id", type: "character(32)", fixedLength: true, maxLength: 32, nullable: false),
                tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                id = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("files_thirdparty_id_mapping_pkey", x => x.hashid);
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
                versionchanged = table.Column<DateTime>(name: "version_changed", type: "timestamp with time zone", nullable: true),
                language = table.Column<string>(type: "character(10)", fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "'en-US'"),
                timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                trusteddomains = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                trusteddomainsenabled = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                status = table.Column<int>(type: "integer", nullable: false),
                statuschanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                creationdatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ownerid = table.Column<Guid>(name: "owner_id", type: "uuid", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                paymentid = table.Column<string>(name: "payment_id", type: "character varying(38)", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                industry = table.Column<int>(type: "integer", nullable: false),
                lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
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

    /// <inheritdoc />
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
            name: "tenants_tenants",
            schema: "onlyoffice");
    }
}
