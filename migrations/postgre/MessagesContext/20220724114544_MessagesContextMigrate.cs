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

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class MessagesContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "audit_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    initiator = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    target = table.Column<string>(type: "text", nullable: true),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    platform = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    page = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "login_events",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL::character varying"),
                    platform = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, defaultValueSql: "NULL"),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 38, nullable: false),
                    page = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true, defaultValueSql: "NULL"),
                    action = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_events", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "date",
                schema: "onlyoffice",
                table: "audit_events",
                columns: new[] { "tenant_id", "date" });

            migrationBuilder.CreateIndex(
                name: "date_login_events",
                schema: "onlyoffice",
                table: "login_events",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "tenant_id_login_events",
                schema: "onlyoffice",
                table: "login_events",
                columns: new[] { "user_id", "tenant_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_events",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "login_events",
                schema: "onlyoffice");
        }
    }
}
