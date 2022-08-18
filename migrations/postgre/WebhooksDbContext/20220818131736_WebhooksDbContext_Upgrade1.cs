using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.WebhooksDb
{
    public partial class WebhooksDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "config_id",
                table: "webhooks_config",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "uid",
                table: "webhooks_logs",
                type: "varchar",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "webhooks_logs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "delivery",
                table: "webhooks_logs",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "enabled",
                table: "webhooks_config",
                type: "boolean",
                nullable: false,
                defaultValueSql: "true");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "webhooks_config",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "delivery",
                table: "webhooks_logs");

            migrationBuilder.DropColumn(
                name: "enabled",
                table: "webhooks_config");

            migrationBuilder.DropColumn(
                name: "name",
                table: "webhooks_config");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "webhooks_config",
                newName: "config_id");

            migrationBuilder.AlterColumn<string>(
                name: "uid",
                table: "webhooks_logs",
                type: "varchar",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "webhooks_logs",
                type: "varchar",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
