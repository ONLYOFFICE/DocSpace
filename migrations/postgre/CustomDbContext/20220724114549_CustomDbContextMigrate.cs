using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class CustomDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:onlyoffice.enum_dbip_location", "ipv4,ipv6");

            migrationBuilder.CreateTable(
                name: "dbip_location",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddrType = table.Column<string>(type: "text", nullable: true),
                    ip_start = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: false),
                    ip_end = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: false),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    stateprov = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    district = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    city = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    zipcode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    latitude = table.Column<long>(type: "bigint", nullable: true),
                    longitude = table.Column<long>(type: "bigint", nullable: true),
                    geoname_id = table.Column<int>(type: "integer", nullable: true),
                    timezone_offset = table.Column<double>(type: "double precision", nullable: true),
                    timezone_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    processed = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbip_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mobile_app_install",
                schema: "onlyoffice",
                columns: table => new
                {
                    user_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    app_type = table.Column<int>(type: "integer", nullable: false),
                    registered_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_sign = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mobile_app_install_pkey", x => new { x.user_email, x.app_type });
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Region = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    ConnectionString = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Region);
                });

            migrationBuilder.CreateIndex(
                name: "ip_start",
                schema: "onlyoffice",
                table: "dbip_location",
                column: "ip_start");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dbip_location",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "mobile_app_install",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
