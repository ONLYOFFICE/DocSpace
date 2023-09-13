using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class MigrationContext_Upgrade4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "short",
                table: "short_links",
                type: "varchar(15)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(12)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                table: "short_links",
                type: "int(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "tenant_id",
                table: "short_links",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_short_links_tenants_tenants_tenant_id",
                table: "short_links",
                column: "tenant_id",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_short_links_tenants_tenants_tenant_id",
                table: "short_links");

            migrationBuilder.DropIndex(
                name: "tenant_id",
                table: "short_links");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "short_links");

            migrationBuilder.AlterColumn<string>(
                name: "short",
                table: "short_links",
                type: "varchar(12)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }
    }
}
