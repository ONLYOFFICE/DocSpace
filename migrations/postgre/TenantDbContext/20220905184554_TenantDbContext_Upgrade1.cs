using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.TenantDb
{
    public partial class TenantDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "for_admin",
                schema: "onlyoffice",
                table: "tenants_iprestrictions",
                type: "TINYINT(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "for_admin",
                schema: "onlyoffice",
                table: "tenants_iprestrictions");
        }
    }
}
