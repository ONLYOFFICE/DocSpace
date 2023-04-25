using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class UserDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    alias = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    mappeddomain = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    version = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'2'"),
                    versionchanged = table.Column<DateTime>(name: "version_changed", type: "datetime", nullable: true),
                    language = table.Column<string>(type: "char(10)", nullable: false, defaultValueSql: "'en-US'", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    timezone = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    trusteddomains = table.Column<string>(type: "varchar(1024)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    trusteddomainsenabled = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    statuschanged = table.Column<DateTime>(type: "datetime", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ownerid = table.Column<string>(name: "owner_id", type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    paymentid = table.Column<string>(name: "payment_id", type: "varchar(38)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    industry = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'0'"),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp", nullable: false),
                    spam = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    calls = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.InsertData(
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "last_modified", "mappeddomain", "name", "owner_id", "payment_id", "status", "statuschanged", "timezone", "trusteddomains", "version_changed" },
                values: new object[] { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Web Office", "00000000-0000-0000-0000-000000000000", null, 1, null, null, null, null });

            migrationBuilder.InsertData(
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "last_modified", "mappeddomain", "name", "owner_id", "payment_id", "statuschanged", "timezone", "trusteddomains", "version_changed" },
                values: new object[] { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Web Office", "66faa6e4-f133-11ea-b126-00ffeec8b4ef", null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "alias",
                table: "tenants_tenants",
                column: "alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "last_modified",
                table: "tenants_tenants",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "mappeddomain",
                table: "tenants_tenants",
                column: "mappeddomain");

            migrationBuilder.CreateIndex(
                name: "version",
                table: "tenants_tenants",
                column: "version");

            migrationBuilder.AddForeignKey(
                name: "FK_core_acl_tenants_tenants_tenant",
                table: "core_acl",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_group_tenants_tenants_tenant",
                table: "core_group",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_subscription_tenants_tenants_tenant",
                table: "core_subscription",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_subscriptionmethod_tenants_tenants_tenant",
                table: "core_subscriptionmethod",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_user_tenants_tenants_tenant",
                table: "core_user",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_userdav_tenants_tenants_tenant_id",
                table: "core_userdav",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                table: "core_usergroup",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_userphoto_tenants_tenants_tenant",
                table: "core_userphoto",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_usersecurity_tenants_tenants_tenant",
                table: "core_usersecurity",
                column: "tenant",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_core_acl_tenants_tenants_tenant",
                table: "core_acl");

            migrationBuilder.DropForeignKey(
                name: "FK_core_group_tenants_tenants_tenant",
                table: "core_group");

            migrationBuilder.DropForeignKey(
                name: "FK_core_subscription_tenants_tenants_tenant",
                table: "core_subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_core_subscriptionmethod_tenants_tenants_tenant",
                table: "core_subscriptionmethod");

            migrationBuilder.DropForeignKey(
                name: "FK_core_user_tenants_tenants_tenant",
                table: "core_user");

            migrationBuilder.DropForeignKey(
                name: "FK_core_userdav_tenants_tenants_tenant_id",
                table: "core_userdav");

            migrationBuilder.DropForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                table: "core_usergroup");

            migrationBuilder.DropForeignKey(
                name: "FK_core_userphoto_tenants_tenants_tenant",
                table: "core_userphoto");

            migrationBuilder.DropForeignKey(
                name: "FK_core_usersecurity_tenants_tenants_tenant",
                table: "core_usersecurity");

            migrationBuilder.DropTable(
                name: "tenants_tenants");
        }
    }
}
