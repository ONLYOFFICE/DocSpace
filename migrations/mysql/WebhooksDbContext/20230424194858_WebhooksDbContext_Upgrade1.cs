using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.WebhooksDb
{
    /// <inheritdoc />
    public partial class WebhooksDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "webhook_id",
                table: "webhooks_logs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "route",
                table: "webhooks",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "webhook_id",
                table: "webhooks_logs");

            migrationBuilder.UpdateData(
                table: "webhooks",
                keyColumn: "route",
                keyValue: null,
                column: "route",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "route",
                table: "webhooks",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.UpdateData(
                table: "webhooks",
                keyColumn: "method",
                keyValue: null,
                column: "method",
                value: "");
        }
    }
}
