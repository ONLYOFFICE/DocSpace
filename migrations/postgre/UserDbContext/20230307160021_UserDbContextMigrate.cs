using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class UserDbContextMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "core_subscription",
                columns: new[] { "action", "object", "recipient", "source", "tenant", "unsubscribed" },
                values: new object[] { "rooms_activity", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "asc.web.studio", -1, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "core_subscription",
                keyColumns: new[] { "action", "object", "recipient", "source", "tenant" },
                keyValues: new object[] { "rooms_activity", "", "c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e", "asc.web.studio", -1 });
        }
    }
}
