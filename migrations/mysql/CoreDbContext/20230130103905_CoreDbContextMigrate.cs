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

namespace ASC.Migrations.MySql.Migrations.CoreDb;

/// <inheritdoc />
public partial class CoreDbContextMigrate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "tenants_quota",
            columns: table => new
            {
                tenant = table.Column<int>(type: "int", nullable: false),
                name = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                description = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                features = table.Column<string>(type: "text", nullable: true)
                    .Annotation("MySql:CharSet", "utf8"),
                price = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValueSql: "'0.00'"),
                productid = table.Column<string>(name: "product_id", type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                visible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'0'")
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => x.tenant);
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "tenants_quotarow",
            columns: table => new
            {
                tenant = table.Column<int>(type: "int", nullable: false),
                path = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                userid = table.Column<Guid>(name: "user_id", type: "char(36)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                counter = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                tag = table.Column<string>(type: "varchar(1024)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenant, x.userid, x.path });
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "tenants_tariff",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                tenant = table.Column<int>(type: "int", nullable: false),
                stamp = table.Column<DateTime>(type: "datetime", nullable: false),
                customerid = table.Column<string>(name: "customer_id", type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                comment = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                    .Annotation("MySql:CharSet", "utf8"),
                createon = table.Column<DateTime>(name: "create_on", type: "timestamp", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tenants_tariff", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8");

        migrationBuilder.CreateTable(
            name: "tenants_tariffrow",
            columns: table => new
            {
                tariffid = table.Column<int>(name: "tariff_id", type: "int", nullable: false),
                quota = table.Column<int>(type: "int", nullable: false),
                tenant = table.Column<int>(type: "int", nullable: false),
                quantity = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.tenant, x.tariffid, x.quota });
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.InsertData(
            table: "tenants_quota",
            columns: new[] { "tenant", "description", "features", "name", "product_id" },
            values: new object[] { -3, null, "free,thirdparty,audit,total_size:2147483648,manager:1,room:12,usersInRoom:3", "startup", null });

        migrationBuilder.InsertData(
            table: "tenants_quota",
            columns: new[] { "tenant", "description", "features", "name", "price", "product_id", "visible" },
            values: new object[] { -2, null, "audit,ldap,sso,whitelabel,thirdparty,restore,total_size:107374182400,file_size:1024,manager:1", "admin", 30m, "1002", true });

        migrationBuilder.InsertData(
            table: "tenants_quota",
            columns: new[] { "tenant", "description", "features", "name", "product_id" },
            values: new object[] { -1, null, "trial,audit,ldap,sso,whitelabel,thirdparty,restore,total_size:107374182400,file_size:100,manager:1", "trial", null });

        migrationBuilder.CreateIndex(
            name: "last_modified",
            table: "tenants_quotarow",
            column: "last_modified");

        migrationBuilder.CreateIndex(
            name: "tenant",
            table: "tenants_tariff",
            column: "tenant");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "tenants_quota");

        migrationBuilder.DropTable(
            name: "tenants_quotarow");

        migrationBuilder.DropTable(
            name: "tenants_tariff");

        migrationBuilder.DropTable(
            name: "tenants_tariffrow");
    }
}
