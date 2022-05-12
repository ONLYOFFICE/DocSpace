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

namespace ASC.Webhooks.Core.Migrations;

public partial class WebhooksDbContextMySql : Migration
{
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "webhooks_config");

        migrationBuilder.DropTable(
            name: "webhooks_logs");
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "webhooks_config",
            columns: table => new
            {
                config_id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                tenant_id = table.Column<uint>(type: "int unsigned", nullable: false),
                uri = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValueSql: "''")
                    .Annotation("MySql:CharSet", "utf8mb4"),
                secret_key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValueSql: "''")
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.config_id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "webhooks_logs",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                config_id = table.Column<int>(type: "int", nullable: false),
                uid = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                tenant_id = table.Column<uint>(type: "int unsigned", nullable: false),
                request_payload = table.Column<string>(type: "json", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                request_headers = table.Column<string>(type: "json", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                response_payload = table.Column<string>(type: "json", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                response_headers = table.Column<string>(type: "json", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                creation_time = table.Column<DateTime>(type: "datetime", nullable: false),
                @event = table.Column<string>(name: "event", type: "varchar(100)", maxLength: 100, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");
    }
}
