using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations
{
    public partial class TestMigration : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "core_user",
                keyColumn: "id",
                keyValue: "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
                columns: new[] { "last_modified", "workfromdate" },
                values: new object[] { new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(1842), new DateTime(2021, 8, 4, 11, 1, 4, 512, DateTimeKind.Utc).AddTicks(9355) });

            migrationBuilder.InsertData(
                table: "core_user",
                columns: new[] { "id", "activation_status", "bithdate", "contacts", "culture", "email", "firstname", "last_modified", "lastname", "location", "notes", "phone", "phone_activation", "removed", "sex", "sid", "sso_name_id", "sso_session_id", "status", "tenant", "terminateddate", "title", "username", "workfromdate" },
                values: new object[] { "99223c7b-e3c9-11eb-9063-982cbc0ea1e5", 0, null, null, null, "test@gmail.com", "Test", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(2928), "User", null, null, null, 0, false, null, null, null, null, 1, 1, null, null, "TestUser", new DateTime(2021, 8, 4, 11, 1, 4, 513, DateTimeKind.Utc).AddTicks(2940) });

            migrationBuilder.InsertData(
                table: "core_usersecurity",
                columns: new[] { "userid", "pwdhash", "pwdhashsha512", "tenant" },
                values: new object[] { "99223c7b-e3c9-11eb-9063-982cbc0ea1e5", "vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=", "USubvPlB+ogq0Q1trcSupg==", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "core_usersecurity",
                keyColumn: "userid",
                keyValue: "99223c7b-e3c9-11eb-9063-982cbc0ea1e5");

            migrationBuilder.DeleteData(
                table: "core_user",
                keyColumn: "id",
                keyValue: "99223c7b-e3c9-11eb-9063-982cbc0ea1e5");

            migrationBuilder.UpdateData(
                table: "core_user",
                keyColumn: "id",
                keyValue: "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
                columns: new[] { "last_modified", "workfromdate" },
                values: new object[] { new DateTime(2021, 8, 3, 21, 35, 0, 522, DateTimeKind.Utc).AddTicks(6893), new DateTime(2021, 8, 3, 21, 35, 0, 522, DateTimeKind.Utc).AddTicks(5587) });
        }
    }
}
