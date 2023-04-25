using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.Backups
{
    /// <inheritdoc />
    public partial class BackupsContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "tenant_id",
                table: "backup_schedule",
                type: "integer",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[] { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_backup_backup_tenants_tenants_tenant_id",
                table: "backup_backup",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_backup_schedule_tenants_tenants_tenant_id",
                table: "backup_schedule",
                column: "tenant_id",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_backup_backup_tenants_tenants_tenant_id",
                table: "backup_backup");

            migrationBuilder.DropForeignKey(
                name: "FK_backup_schedule_tenants_tenants_tenant_id",
                table: "backup_schedule");

            migrationBuilder.DeleteData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                keyColumn: "id",
                keyValue: -1);

            migrationBuilder.AlterColumn<int>(
                name: "tenant_id",
                table: "backup_schedule",
                type: "integer",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
