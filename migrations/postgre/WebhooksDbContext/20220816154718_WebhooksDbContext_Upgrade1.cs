using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.WebhooksDb
{
    public partial class WebhooksDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "event",
                table: "webhooks_logs",
                newName: "route");

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

            migrationBuilder.AlterColumn<string>(
                name: "response_payload",
                table: "webhooks_logs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "request_payload",
                table: "webhooks_logs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "json");

            migrationBuilder.AddColumn<string>(
                name: "method",
                table: "webhooks_logs",
                type: "varchar",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "method",
                table: "webhooks_logs");

            migrationBuilder.RenameColumn(
                name: "route",
                table: "webhooks_logs",
                newName: "event");

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
                name: "response_payload",
                table: "webhooks_logs",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "request_payload",
                table: "webhooks_logs",
                type: "json",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
