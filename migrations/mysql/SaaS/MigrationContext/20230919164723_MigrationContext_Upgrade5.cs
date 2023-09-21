using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class MigrationContext_Upgrade5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "short",
                table: "short_links",
                type: "char(15)",
                nullable: true,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(12)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<ulong>(
                name: "id",
                table: "short_links",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(19)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                table: "short_links",
                type: "int(10)",
                nullable: false,
                defaultValue: -1);

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
                oldType: "char(15)",
                oldNullable: true,
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "short_links",
                type: "bigint(19)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
