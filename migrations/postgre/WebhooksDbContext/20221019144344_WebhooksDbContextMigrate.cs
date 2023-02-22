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

namespace ASC.Migrations.PostgreSql.Migrations;

public partial class WebhooksDbContextMigrate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "webhooks",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                route = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, defaultValueSql: "''"),
                method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "''")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "webhooks_config",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                secret_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''"),
                tenant_id = table.Column<int>(type: "int unsigned", nullable: false),
                uri = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "''"),
                enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "webhooks_logs",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                config_id = table.Column<int>(type: "int", nullable: false),
                creation_time = table.Column<DateTime>(type: "datetime", nullable: false),
                webhook_id = table.Column<int>(type: "int", nullable: false),
                request_headers = table.Column<string>(type: "json", nullable: true),
                request_payload = table.Column<string>(type: "text", nullable: false),
                response_headers = table.Column<string>(type: "json", nullable: true),
                response_payload = table.Column<string>(type: "text", nullable: true),
                status = table.Column<int>(type: "int", nullable: false),
                tenant_id = table.Column<int>(type: "int unsigned", nullable: false),
                uid = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                delivery = table.Column<DateTime>(type: "datetime", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.id);
                table.ForeignKey(
                    name: "FK_webhooks_logs_webhooks_config_config_id",
                    column: x => x.config_id,
                    principalTable: "webhooks_config",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "tenant_id",
            table: "webhooks_config",
            column: "tenant_id");

        migrationBuilder.CreateIndex(
            name: "IX_webhooks_logs_config_id",
            table: "webhooks_logs",
            column: "config_id");

        migrationBuilder.CreateIndex(
            name: "tenant_id",
            table: "webhooks_logs",
            column: "tenant_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "webhooks");

        migrationBuilder.DropTable(
            name: "webhooks_logs");

        migrationBuilder.DropTable(
            name: "webhooks_config");
    }
}
