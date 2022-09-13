using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.UserDb
{
    public partial class UserDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "quota_limit",
                table: "core_user",
                type: "bigint",
                nullable: false,
                defaultValueSql: "'-1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quota_limit",
                table: "core_user");
        }
    }
}
