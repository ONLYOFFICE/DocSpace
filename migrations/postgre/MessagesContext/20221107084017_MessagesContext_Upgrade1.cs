using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.Messages
{
    public partial class MessagesContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "context",
                schema: "onlyoffice",
                table: "audit_events",
                type: "character varying(400)",
                maxLength: 400,
                nullable: true,
                defaultValueSql: "NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "context",
                schema: "onlyoffice",
                table: "audit_events");
        }
    }
}
