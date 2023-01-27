using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.Backups
{
    /// <inheritdoc />
    public partial class BackupsContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "removed",
                table: "backup_backup",
                type: "int",
                maxLength: 10,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "removed",
                table: "backup_backup");
        }
    }
}
