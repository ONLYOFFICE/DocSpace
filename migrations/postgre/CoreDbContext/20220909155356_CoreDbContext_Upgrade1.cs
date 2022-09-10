using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.CoreDb
{
    public partial class CoreDbContext_Upgrade1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users_quotarow",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    counter = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "'0'"),
                    tag = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_userquotarow_pkey", x => new { x.tenant, x.UserId, x.path });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users_quotarow",
                schema: "onlyoffice");
        }
    }
}
