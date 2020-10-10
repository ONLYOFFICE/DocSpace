using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.DbContextNpgsql
{
    public partial class DbContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            _ = migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:onlyoffice.enum_dbip_location", "ipv4,ipv6");

            _ = migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    Region = table.Column<string>(nullable: false),
                    Provider = table.Column<string>(nullable: true),
                    connection_string = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_regions", x => x.Region);
                });

            _ = migrationBuilder.CreateTable(
                name: "dbip_location",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    addr_type = table.Column<string>(nullable: true),
                    ip_start = table.Column<string>(maxLength: 39, nullable: false),
                    ip_end = table.Column<string>(maxLength: 39, nullable: false),
                    country = table.Column<string>(maxLength: 2, nullable: false),
                    stateprov = table.Column<string>(maxLength: 255, nullable: false),
                    district = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    city = table.Column<string>(maxLength: 255, nullable: false),
                    zipcode = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    latitude = table.Column<long>(nullable: false),
                    longitude = table.Column<long>(nullable: false),
                    geoname_id = table.Column<int>(nullable: false),
                    timezone_offset = table.Column<double>(nullable: false),
                    timezone_name = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    processed = table.Column<int>(nullable: false, defaultValueSql: "1")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_dbip_location", x => x.id);
                });

            _ = migrationBuilder.CreateTable(
                name: "mobile_app_install",
                schema: "onlyoffice",
                columns: table => new
                {
                    user_email = table.Column<string>(maxLength: 255, nullable: false),
                    app_type = table.Column<int>(nullable: false),
                    registered_on = table.Column<DateTime>(nullable: false),
                    last_sign = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("mobile_app_install_pkey", x => new { x.user_email, x.app_type });
                });

            _ = migrationBuilder.CreateIndex(
                name: "ip_start",
                schema: "onlyoffice",
                table: "dbip_location",
                column: "ip_start");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "regions");

            _ = migrationBuilder.DropTable(
                name: "dbip_location",
                schema: "onlyoffice");

            _ = migrationBuilder.DropTable(
                name: "mobile_app_install",
                schema: "onlyoffice");
        }
    }
}
