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

#nullable disable

namespace ASC.Migrations.MySql.Migrations.WebhooksDb
{
    /// <inheritdoc />
    public partial class WebhooksDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "uri",
                table: "webhooks_config",
                type: "text",
                nullable: true,
                defaultValueSql: "''",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<bool>(
                name: "ssl",
                table: "webhooks_config",
                type: "tinyint(1)",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.AlterColumn<string>(
                name: "route",
                table: "webhooks",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "method",
                table: "webhooks",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ssl",
                table: "webhooks_config");

            migrationBuilder.AlterColumn<string>(
                name: "uri",
                table: "webhooks_config",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldDefaultValueSql: "''",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.UpdateData(
                table: "webhooks",
                keyColumn: "route",
                keyValue: null,
                column: "route",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "route",
                table: "webhooks",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.UpdateData(
                table: "webhooks",
                keyColumn: "method",
                keyValue: null,
                column: "method",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "method",
                table: "webhooks",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }
    }
}
