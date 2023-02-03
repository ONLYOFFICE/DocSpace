using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class InstanceRegistrationContextMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hosting_instance_registration",
                columns: table => new
                {
                    instanceregistrationid = table.Column<string>(name: "instance_registration_id", type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    lastupdated = table.Column<DateTime>(name: "last_updated", type: "datetime", nullable: true),
                    workertypename = table.Column<string>(name: "worker_type_name", type: "varchar(255)", nullable: false, collation: "utf8_general_ci"),
                    isactive = table.Column<bool>(name: "is_active", type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.instanceregistrationid);
                });

            migrationBuilder.CreateIndex(
                name: "worker_type_name",
                table: "hosting_instance_registration",
                column: "worker_type_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hosting_instance_registration");
        }
    }
}
