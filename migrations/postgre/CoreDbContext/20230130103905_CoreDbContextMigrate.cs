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

namespace ASC.Migrations.PostgreSql.Migrations.CoreDb;

/// <inheritdoc />
public partial class CoreDbContextMigrate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "onlyoffice");

        migrationBuilder.CreateTable(
            name: "tenants_quota",
            schema: "onlyoffice",
            columns: table => new
            {
                tenant = table.Column<int>(type: "integer", nullable: false),
                name = table.Column<string>(type: "character varying", nullable: true),
                description = table.Column<string>(type: "character varying", nullable: true),
                features = table.Column<string>(type: "text", nullable: true),
                price = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValueSql: "0.00"),
                productid = table.Column<string>(name: "product_id", type: "character varying(128)", maxLength: 128, nullable: true, defaultValueSql: "NULL"),
                visible = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("tenants_quota_pkey", x => x.tenant);
            });

        migrationBuilder.CreateTable(
            name: "tenants_quotarow",
            schema: "onlyoffice",
            columns: table => new
            {
                tenant = table.Column<int>(type: "integer", nullable: false),
                path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                counter = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                tag = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "'0'"),
                lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                userid = table.Column<Guid>(name: "user_id", type: "uuid", maxLength: 36, nullable: false, defaultValueSql: "NULL")
            },
            constraints: table =>
            {
                table.PrimaryKey("tenants_quotarow_pkey", x => new { x.tenant, x.path });
            });

        migrationBuilder.CreateTable(
            name: "tenants_tariff",
            schema: "onlyoffice",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                tenant = table.Column<int>(type: "integer", nullable: false),
                stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                customerid = table.Column<string>(name: "customer_id", type: "character varying(255)", maxLength: 255, nullable: false, defaultValueSql: "NULL"),
                comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                createon = table.Column<DateTime>(name: "create_on", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tenants_tariff", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "tenants_tariffrow",
            schema: "onlyoffice",
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
            });

        migrationBuilder.InsertData(
            schema: "onlyoffice",
            table: "tenants_quota",
            columns: new[] { "tenant", "description", "features", "name", "visible" },
            values: new object[] { -3, null, "free,thirdparty,audit,total_size:2147483648,manager:1,room:12,usersInRoom:3", "startup", false });

        migrationBuilder.InsertData(
            schema: "onlyoffice",
            table: "tenants_quota",
            columns: new[] { "tenant", "description", "features", "name", "price", "product_id", "visible" },
            values: new object[] { -2, null, "audit,ldap,sso,whitelabel,thirdparty,restore,total_size:107374182400,file_size:1024,manager:1", "admin", 30m, "1002", true });

        migrationBuilder.InsertData(
            schema: "onlyoffice",
            table: "tenants_quota",
            columns: new[] { "tenant", "description", "features", "name", "visible" },
            values: new object[] { -1, null, "trial,audit,ldap,sso,whitelabel,thirdparty,restore,total_size:107374182400,file_size:100,manager:1", "trial", false });

        migrationBuilder.CreateIndex(
            name: "last_modified_tenants_quotarow",
            schema: "onlyoffice",
            table: "tenants_quotarow",
            column: "last_modified");

        migrationBuilder.CreateIndex(
            name: "tenant_tenants_tariff",
            schema: "onlyoffice",
            table: "tenants_tariff",
            column: "tenant");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "tenants_quota",
            schema: "onlyoffice");

        migrationBuilder.DropTable(
            name: "tenants_quotarow",
            schema: "onlyoffice");

        migrationBuilder.DropTable(
            name: "tenants_tariff",
            schema: "onlyoffice");

        migrationBuilder.DropTable(
            name: "tenants_tariffrow",
            schema: "onlyoffice");
    }
}
