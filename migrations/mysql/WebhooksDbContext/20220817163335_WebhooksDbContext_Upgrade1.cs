using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.MySql.Migrations.WebhooksDb
{
    public partial class WebhooksDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "webhooks_logs",
                keyColumn: "uid",
                keyValue: null,
                column: "uid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "uid",
                table: "webhooks_logs",
                type: "varchar(36)",
                nullable: false,
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "webhooks_logs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<DateTime>(
                name: "delivery",
                table: "webhooks_logs",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "enabled",
                table: "webhooks_config",
                type: "tinyint(1)",
                nullable: false,
                defaultValueSql: "'1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "delivery",
                table: "webhooks_logs");

            migrationBuilder.DropColumn(
                name: "enabled",
                table: "webhooks_config");

            migrationBuilder.AlterColumn<string>(
                name: "uid",
                table: "webhooks_logs",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "webhooks_logs",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8");
        }
    }
}
