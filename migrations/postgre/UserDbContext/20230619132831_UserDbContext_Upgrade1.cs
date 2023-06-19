using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class UserDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[,]
                {
                    { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null },
                    { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user",
                column: "tenant");

            migrationBuilder.AddForeignKey(
                name: "FK_core_acl_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_acl",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_group_tenants_tenants_tenant",
                table: "core_group",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_subscription_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_subscription",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_subscriptionmethod_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_subscriptionmethod",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_user_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_user",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_userdav_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "core_userdav",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_usergroup",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_userphoto_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_userphoto",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_core_usersecurity_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_usersecurity",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_core_acl_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_acl");

            migrationBuilder.DropForeignKey(
                name: "FK_core_group_tenants_tenants_tenant",
                table: "core_group");

            migrationBuilder.DropForeignKey(
                name: "FK_core_subscription_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_core_subscriptionmethod_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_subscriptionmethod");

            migrationBuilder.DropForeignKey(
                name: "FK_core_user_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_user");

            migrationBuilder.DropForeignKey(
                name: "FK_core_userdav_tenants_tenants_tenant_id",
                schema: "onlyoffice",
                table: "core_userdav");

            migrationBuilder.DropForeignKey(
                name: "FK_core_usergroup_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_usergroup");

            migrationBuilder.DropForeignKey(
                name: "FK_core_userphoto_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_userphoto");

            migrationBuilder.DropForeignKey(
                name: "FK_core_usersecurity_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "core_usersecurity");

            migrationBuilder.DropIndex(
                name: "IX_core_user_tenant",
                schema: "onlyoffice",
                table: "core_user");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
