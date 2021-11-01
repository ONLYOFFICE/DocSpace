using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.CoreDbContextMySql
{
    public partial class CoreDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "core_acl",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "int", nullable: false),
                    subject = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    action = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    @object = table.Column<string>(name: "object", type: "varchar(255)", nullable: false, defaultValueSql: "''", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    acetype = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.subject, x.action, x.@object });
                });

            migrationBuilder.CreateTable(
                name: "tenants_buttons",
                columns: table => new
                {
                    tariff_id = table.Column<int>(type: "int", nullable: false),
                    partner_id = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    button_url = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tariff_id, x.partner_id });
                });

            migrationBuilder.CreateTable(
                name: "tenants_quota",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    description = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    max_file_size = table.Column<long>(type: "bigint", nullable: false),
                    max_total_size = table.Column<long>(type: "bigint", nullable: false),
                    active_users = table.Column<int>(type: "int", nullable: false),
                    features = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    avangate_id = table.Column<string>(type: "varchar(128)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    visible = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant);
                });

            migrationBuilder.CreateTable(
                name: "tenants_quotarow",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "int", nullable: false),
                    path = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    counter = table.Column<long>(type: "bigint", nullable: false),
                    tag = table.Column<string>(type: "varchar(1024)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    last_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.path });
                });

            migrationBuilder.CreateTable(
                name: "tenants_tariff",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant = table.Column<int>(type: "int", nullable: false),
                    tariff = table.Column<int>(type: "int", nullable: false),
                    stamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    tariff_key = table.Column<string>(type: "varchar(64)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    comment = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    create_on = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "core_acl",
                columns: new[] { "action", "object", "subject", "tenant", "acetype" },
                values: new object[,]
                {
                    { "ef5e6790-f346-4b6e-b662-722bc28cb0db", "", "5d5b7260-f7f7-49f1-a1c9-95fbb6a12604", -1, 0 },
                    { "00e7dfc5-ac49-4fd3-a1d6-98d84e877ac4", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "14be970f-7af5-4590-8e81-ea32b5f7866d", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "18ecc94d-6afa-4994-8406-aee9dff12ce2", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "298530eb-435e-4dc6-a776-9abcd95c70e9", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "430eaf70-1886-483c-a746-1a18e3e6bb63", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "557d6503-633b-4490-a14c-6473147ce2b3", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "724cbb75-d1c9-451e-bae0-4de0db96b1f7", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "7cb5c0d1-d254-433f-abe3-ff23373ec631", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "91b29dcd-9430-4403-b17a-27d09189be88", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "a18480a4-6d18-4c71-84fa-789888791f45", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "b630d29b-1844-4bda-bbbe-cf5542df3559", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "c62a9e8d-b24c-4513-90aa-7ff0f8ba38eb", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "d7cdb020-288b-41e5-a857-597347618533", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "662f3db7-9bc8-42cf-84da-2765f563e9b0", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "f11e8f3f-46e6-4e55-90e3-09c22ec565bd", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "e0759a42-47f0-4763-a26a-d5aa665bec35", "", "712d9ec3-5d2b-4b13-824f-71f00191dcca", -1, 0 },
                    { "6f05c382-8bca-4469-9424-c807a98c40d7", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|1e04460243b54d7982f3fd6208a11960", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6743007c6f954d208c88a8601ce5e76d", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|e67be73df9ae4ce18fec1880cb518cb4", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|ea942538e68e49079394035336ee0ba8", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|32d24cb57ece46069c9419216ba42086", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|bf88953e3c434850a3fbb1e43ad53a3e", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|2a9230378b2d487b9a225ac0918acf3f", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|f4d98afdd336433287783c6945c81ea0", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|28b10049dd204f54b986873bc14ccfc7", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|3cfd481b46f24a4ab55cb8c0c9def02c", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6a598c7491ae437da5f4ad339bd11bb2", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|742cf945cbbc4a5782d61600a12cf8ca", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|853b6eb973ee438d9b098ffeedf36234", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "0d68b142-e20a-446e-a832-0d6b0b65a164", "", "bba32183-a14d-48ed-9d39-c6b4d8925fbf", -1, 0 },
                    { "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|46cfa73af32046cf8d5bcd82e1d67f26", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "f11e88d7-f185-4372-927c-d88008d2c483", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "e0759a42-47f0-4763-a26a-d5aa665bec35", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "f11e8f3f-46e6-4e55-90e3-09c22ec565bd", "", "5d5b7260-f7f7-49f1-a1c9-95fbb6a12604", -1, 0 },
                    { "088d5940-a80f-4403-9741-d610718ce95c", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "08d66144-e1c9-4065-9aa1-aa4bba0a7bc8", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "08d75c97-cf3f-494b-90d1-751c941fe2dd", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "0d1f72a8-63da-47ea-ae42-0900e4ac72a9", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "13e30b51-5b4d-40a5-8575-cb561899eeb1", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "19f658ae-722b-4cd8-8236-3ad150801d96", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "2c6552b3-b2e0-4a00-b8fd-13c161e337b1", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "388c29d3-c662-4a61-bf47-fc2f7094224a", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "40bf31f4-3132-4e76-8d5c-9828a89501a3", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "49ae8915-2b30-4348-ab74-b152279364fb", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "9018c001-24c2-44bf-a1db-d1121a570e74", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "948ad738-434b-4a88-8e38-7569d332910a", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "e37239bd-c5b5-4f1e-a9f8-3ceeac209615", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "9d75a568-52aa-49d8-ad43-473756cd8903", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "c426c349-9ad4-47cd-9b8f-99fc30675951", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "d11ebcb9-0e6e-45e6-a6d0-99c41d687598", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "d1f3b53d-d9e2-4259-80e7-d24380978395", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "d49f4e30-da10-4b39-bc6d-b41ef6e039d3", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "d852b66f-6719-45e1-8657-18f0bb791690", "", "abef62db-11a8-4673-9d32-ef1d8af19dc0", -1, 0 },
                    { "e0759a42-47f0-4763-a26a-d5aa665bec35", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "e37239bd-c5b5-4f1e-a9f8-3ceeac209615", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "fbc37705-a04c-40ad-a68c-ce2f0423f397", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "fcac42b8-9386-48eb-a938-d19b3c576912", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 },
                    { "13e30b51-5b4d-40a5-8575-cb561899eeb1", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "49ae8915-2b30-4348-ab74-b152279364fb", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "9018c001-24c2-44bf-a1db-d1121a570e74", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "d1f3b53d-d9e2-4259-80e7-d24380978395", "", "ba74ca02-873f-43dc-8470-8620c156bc67", -1, 0 },
                    { "a362fe79-684e-4d43-a599-65bc1f4e167f", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", -1, 0 }
                });

            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "active_users", "avangate_id", "description", "features", "max_file_size", "max_total_size", "name", "price", "visible" },
                values: new object[] { -1, 10000, "0", null, "domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption,privacyroom,restore", 102400L, 10995116277760L, "default", 0.00m, false });

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "tenants_quotarow",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "tenant",
                table: "tenants_tariff",
                column: "tenant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "core_acl");

            migrationBuilder.DropTable(
                name: "tenants_buttons");

            migrationBuilder.DropTable(
                name: "tenants_quota");

            migrationBuilder.DropTable(
                name: "tenants_quotarow");

            migrationBuilder.DropTable(
                name: "tenants_tariff");
        }
    }
}
