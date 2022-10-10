using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.CoreDb
{
    public partial class CoreDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "tenants_quotarow");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "tenants_quotarow",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "tenants_quotarow",
                columns: new[] { "tenant", "user_id", "path" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "tenants_quotarow");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "tenants_quotarow");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "tenants_quotarow",
                columns: new[] { "tenant", "path" });
        }
    }
}
