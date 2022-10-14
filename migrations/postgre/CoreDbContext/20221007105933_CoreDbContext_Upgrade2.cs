using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.CoreDb
{
    public partial class CoreDbContext_Upgrade2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users_quotarow",
                schema: "onlyoffice");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "onlyoffice",
                table: "tenants_quotarow",
                type: "uuid",
                maxLength: 36,
                nullable: false,
                defaultValueSql: "NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "onlyoffice",
                table: "tenants_quotarow");

            migrationBuilder.CreateTable(
                name: "users_quotarow",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    counter = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    tag = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_userquotarow_pkey", x => new { x.tenant, x.UserId, x.path });
                });
        }
    }
}
