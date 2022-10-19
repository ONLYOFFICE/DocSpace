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

namespace ASC.Migrations.PostgreSql.Migrations.CoreDb
{
    public partial class CoreDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                schema: "onlyoffice",
                table: "tenants_tariff");

            migrationBuilder.DropColumn(
                name: "tariff",
                schema: "onlyoffice",
                table: "tenants_tariff");

            migrationBuilder.DropColumn(
                name: "active_users",
                schema: "onlyoffice",
                table: "tenants_quota");

            migrationBuilder.DropColumn(
                name: "max_total_size",
                schema: "onlyoffice",
                table: "tenants_quota");

            migrationBuilder.DropColumn(
               name: "max_file_size",
               schema: "onlyoffice",
               table: "tenants_quota");

            migrationBuilder.RenameColumn(
                name: "avangate_id",
                schema: "onlyoffice",
                table: "tenants_quota",
                newName: "product_id");

            migrationBuilder.AddColumn<string>(
                name: "customer_id",
                schema: "onlyoffice",
                table: "tenants_tariff",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValueSql: "NULL");

            migrationBuilder.CreateTable(
                name: "TariffRows",
                columns: table => new
                {
                    TariffId = table.Column<int>(type: "integer", nullable: false),
                    Quota = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Tenant = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffRows", x => new { x.Tenant, x.TariffId, x.Quota });
                });

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                columns: new[] { "features", "name", "product_id" },
                values: new object[] { "trial,audit,ldap,sso,whitelabel,restore,total_size:107374182400,file_size:100,manager:1", "trial", null });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "visible" },
                values: new object[] { -3, null, "free,total_size:2147483648,manager:1,room:12,usersInRoom:3", "startup", false });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_quota",
                columns: new[] { "tenant", "description", "features", "name", "price", "product_id", "visible" },
                values: new object[] { -2, null, "audit,ldap,sso,whitelabel,restore,total_size:107374182400,file_size:1024,manager:1", "admin", 30.00m, "1002", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TariffRows");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -3);

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2);

            migrationBuilder.DropColumn(
                name: "customer_id",
                schema: "onlyoffice",
                table: "tenants_tariff");

            migrationBuilder.RenameColumn(
                name: "product_id",
                schema: "onlyoffice",
                table: "tenants_quota",
                newName: "avangate_id");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                schema: "onlyoffice",
                table: "tenants_tariff",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tariff",
                schema: "onlyoffice",
                table: "tenants_tariff",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "active_users",
                schema: "onlyoffice",
                table: "tenants_quota",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "max_total_size",
                schema: "onlyoffice",
                table: "tenants_quota",
                type: "bigint",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.AddColumn<long>(
                name: "max_file_size",
                schema: "onlyoffice",
                table: "tenants_quota",
                type: "bigint",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                columns: new[] { "active_users", "avangate_id", "features", "max_file_size", "max_total_size", "name" },
                values: new object[] { 10000, "0", "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default" });
        }
    }
}
