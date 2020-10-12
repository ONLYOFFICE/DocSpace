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
                    tenant = table.Column<int>(nullable: false),
                    subject = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    action = table.Column<string>(type: "varchar(38)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    @object = table.Column<string>(name: "object", type: "varchar(255)", nullable: false, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    acetype = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tenant, x.subject, x.action, x.@object });
                });

            migrationBuilder.CreateTable(
                name: "tenants_buttons",
                columns: table => new
                {
                    tariff_id = table.Column<int>(nullable: false),
                    partner_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    button_url = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.tariff_id, x.partner_id });
                });

            migrationBuilder.CreateTable(
                name: "tenants_quota",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(128)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    description = table.Column<string>(type: "varchar(128)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    max_file_size = table.Column<long>(nullable: false),
                    max_total_size = table.Column<long>(nullable: false),
                    active_users = table.Column<int>(nullable: false),
                    features = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    price2 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    avangate_id = table.Column<string>(type: "varchar(128)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.tenant);
                });

            migrationBuilder.CreateTable(
                name: "tenants_quotarow",
                columns: table => new
                {
                    tenant = table.Column<int>(nullable: false),
                    path = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    counter = table.Column<long>(nullable: false),
                    tag = table.Column<string>(type: "varchar(1024)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
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
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant = table.Column<int>(nullable: false),
                    tariff = table.Column<int>(nullable: false),
                    stamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    tariff_key = table.Column<string>(type: "varchar(64)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    comment = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    create_on = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tariff", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "core_acl",
                columns: new[] { "tenant", "subject", "action", "object", "acetype" },
                values: new object[,]
                {
                    { -1, "5d5b7260-f7f7-49f1-a1c9-95fbb6a12604", "ef5e6790-f346-4b6e-b662-722bc28cb0db", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "00e7dfc5-ac49-4fd3-a1d6-98d84e877ac4", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "14be970f-7af5-4590-8e81-ea32b5f7866d", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "18ecc94d-6afa-4994-8406-aee9dff12ce2", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "298530eb-435e-4dc6-a776-9abcd95c70e9", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "430eaf70-1886-483c-a746-1a18e3e6bb63", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "557d6503-633b-4490-a14c-6473147ce2b3", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "724cbb75-d1c9-451e-bae0-4de0db96b1f7", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "7cb5c0d1-d254-433f-abe3-ff23373ec631", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "91b29dcd-9430-4403-b17a-27d09189be88", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "a18480a4-6d18-4c71-84fa-789888791f45", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "b630d29b-1844-4bda-bbbe-cf5542df3559", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "c62a9e8d-b24c-4513-90aa-7ff0f8ba38eb", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "d7cdb020-288b-41e5-a857-597347618533", "", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "662f3db7-9bc8-42cf-84da-2765f563e9b0", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "f11e8f3f-46e6-4e55-90e3-09c22ec565bd", "", 0 },
                    { -1, "712d9ec3-5d2b-4b13-824f-71f00191dcca", "e0759a42-47f0-4763-a26a-d5aa665bec35", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "6f05c382-8bca-4469-9424-c807a98c40d7", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|1e04460243b54d7982f3fd6208a11960", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6743007c6f954d208c88a8601ce5e76d", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|e67be73df9ae4ce18fec1880cb518cb4", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|ea942538e68e49079394035336ee0ba8", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|32d24cb57ece46069c9419216ba42086", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|bf88953e3c434850a3fbb1e43ad53a3e", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|2a9230378b2d487b9a225ac0918acf3f", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|f4d98afdd336433287783c6945c81ea0", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|28b10049dd204f54b986873bc14ccfc7", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|3cfd481b46f24a4ab55cb8c0c9def02c", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6a598c7491ae437da5f4ad339bd11bb2", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|742cf945cbbc4a5782d61600a12cf8ca", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|853b6eb973ee438d9b098ffeedf36234", 0 },
                    { -1, "bba32183-a14d-48ed-9d39-c6b4d8925fbf", "0d68b142-e20a-446e-a832-0d6b0b65a164", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "77777777-32ae-425f-99b5-83176061d1ae", "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|46cfa73af32046cf8d5bcd82e1d67f26", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "f11e88d7-f185-4372-927c-d88008d2c483", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "e0759a42-47f0-4763-a26a-d5aa665bec35", "", 0 },
                    { -1, "5d5b7260-f7f7-49f1-a1c9-95fbb6a12604", "f11e8f3f-46e6-4e55-90e3-09c22ec565bd", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "088d5940-a80f-4403-9741-d610718ce95c", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "08d66144-e1c9-4065-9aa1-aa4bba0a7bc8", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "08d75c97-cf3f-494b-90d1-751c941fe2dd", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "0d1f72a8-63da-47ea-ae42-0900e4ac72a9", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "13e30b51-5b4d-40a5-8575-cb561899eeb1", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "19f658ae-722b-4cd8-8236-3ad150801d96", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "2c6552b3-b2e0-4a00-b8fd-13c161e337b1", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "388c29d3-c662-4a61-bf47-fc2f7094224a", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "40bf31f4-3132-4e76-8d5c-9828a89501a3", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "49ae8915-2b30-4348-ab74-b152279364fb", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "9018c001-24c2-44bf-a1db-d1121a570e74", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "948ad738-434b-4a88-8e38-7569d332910a", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "e37239bd-c5b5-4f1e-a9f8-3ceeac209615", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "9d75a568-52aa-49d8-ad43-473756cd8903", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "c426c349-9ad4-47cd-9b8f-99fc30675951", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "d11ebcb9-0e6e-45e6-a6d0-99c41d687598", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "d1f3b53d-d9e2-4259-80e7-d24380978395", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "d49f4e30-da10-4b39-bc6d-b41ef6e039d3", "", 0 },
                    { -1, "abef62db-11a8-4673-9d32-ef1d8af19dc0", "d852b66f-6719-45e1-8657-18f0bb791690", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "e0759a42-47f0-4763-a26a-d5aa665bec35", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "e37239bd-c5b5-4f1e-a9f8-3ceeac209615", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "fbc37705-a04c-40ad-a68c-ce2f0423f397", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "fcac42b8-9386-48eb-a938-d19b3c576912", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "13e30b51-5b4d-40a5-8575-cb561899eeb1", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "49ae8915-2b30-4348-ab74-b152279364fb", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "9018c001-24c2-44bf-a1db-d1121a570e74", "", 0 },
                    { -1, "ba74ca02-873f-43dc-8470-8620c156bc67", "d1f3b53d-d9e2-4259-80e7-d24380978395", "", 0 },
                    { -1, "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "a362fe79-684e-4d43-a599-65bc1f4e167f", "", 0 }
                });

            migrationBuilder.InsertData(
                table: "tenants_quota",
                columns: new[] { "tenant", "active_users", "avangate_id", "description", "features", "max_file_size", "max_total_size", "name", "price", "price2", "visible" },
                values: new object[] { -1, 10000, "0", null, "docs,domain,audit,controlpanel,healthcheck,ldap,sso,whitelabel,branding,ssbranding,update,support,portals:10000,discencryption", 102400L, 10995116277760L, "default", 0.00m, 0.00m, false });

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
