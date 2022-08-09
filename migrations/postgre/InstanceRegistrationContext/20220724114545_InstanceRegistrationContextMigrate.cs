using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class InstanceRegistrationContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hosting_instance_registration",
                columns: table => new
                {
                    instance_registration_id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    worker_type_name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    is_active = table.Column<bool>(type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.instance_registration_id);
                });

            migrationBuilder.CreateIndex(
                name: "worker_type_name",
                table: "hosting_instance_registration",
                column: "worker_type_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hosting_instance_registration");
        }
    }
}
