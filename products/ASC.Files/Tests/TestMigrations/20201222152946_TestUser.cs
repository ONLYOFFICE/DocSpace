using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.TenantDbContextMySql
{

    public partial class TestUser : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "core_user",
                keyColumn: "id",
                keyValue: "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
                columns: new[] { "last_modified", "status", "workfromdate" },
                values: new object[] { new DateTime(2020, 12, 22, 15, 29, 45, 332, DateTimeKind.Utc).AddTicks(2191), 1, new DateTime(2020, 12, 22, 15, 29, 45, 332, DateTimeKind.Utc).AddTicks(1292) });

            migrationBuilder.InsertData(
                table: "core_user",
                columns: new[] { "id", "activation_status", "bithdate", "contacts", "culture", "email", "firstname", "last_modified", "lastname", "location", "notes", "phone", "phone_activation", "removed", "sex", "sid", "sso_name_id", "sso_session_id", "status", "tenant", "terminateddate", "title", "username", "workfromdate" },
                values: new object[] { "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5", 0, null, null, null, "test@gmail.com", "Test", new DateTime(2020, 12, 22, 15, 29, 45, 332, DateTimeKind.Utc).AddTicks(3070), "User", null, null, null, 0, false, null, null, null, null, 1, 1, null, null, "TestUser", new DateTime(2020, 12, 22, 15, 29, 45, 332, DateTimeKind.Utc).AddTicks(3053) });

            migrationBuilder.UpdateData(
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: 1,
                column: "creationdatetime",
                value: new DateTime(2020, 12, 22, 15, 29, 45, 343, DateTimeKind.Utc).AddTicks(7822));

            migrationBuilder.InsertData(
                table: "core_usersecurity",
                columns: new[] { "userid", "pwdhash", "pwdhashsha512", "tenant" },
                values: new object[] { "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5", "vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=", "USubvPlB+ogq0Q1trcSupg==", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "core_usersecurity",
                keyColumn: "userid",
                keyValue: "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5");

            migrationBuilder.DeleteData(
                table: "core_user",
                keyColumn: "id",
                keyValue: "005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5");

            migrationBuilder.UpdateData(
                table: "core_user",
                keyColumn: "id",
                keyValue: "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
                columns: new[] { "last_modified", "status", "workfromdate" },
                values: new object[] { new DateTime(2020, 12, 22, 14, 54, 11, 741, DateTimeKind.Utc).AddTicks(4732), 1, new DateTime(2020, 12, 22, 14, 54, 11, 741, DateTimeKind.Utc).AddTicks(3715) });

            migrationBuilder.UpdateData(
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: 1,
                column: "creationdatetime",
                value: new DateTime(2020, 12, 22, 14, 54, 11, 753, DateTimeKind.Utc).AddTicks(774));
        }
    }
}
