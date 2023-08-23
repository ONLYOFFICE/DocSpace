using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Migrations.PostgreSql.SaaS.Migrations.Messages
{
    /// <inheritdoc />
    public partial class MessagesContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenants_partners",
                schema: "onlyoffice",
                columns: table => new
                {
                    tenantid = table.Column<int>(name: "tenant_id", type: "integer", nullable: false),
                    partnerid = table.Column<string>(name: "partner_id", type: "character varying(36)", maxLength: 36, nullable: true, defaultValueSql: "NULL"),
                    affiliateid = table.Column<string>(name: "affiliate_id", type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    campaign = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenants_partners_pkey", x => x.tenantid);
                    table.ForeignKey(
                        name: "FK_tenants_partners_tenants_tenants_tenant_id",
                        column: x => x.tenantid,
                        principalSchema: "onlyoffice",
                        principalTable: "tenants_tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants_partners",
                schema: "onlyoffice");
        }
    }
}
