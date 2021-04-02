using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.DbContextMySql
{
    public partial class DbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dbip_location",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    addr_type = table.Column<string>(type: "enum('ipv4','ipv6')", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ip_start = table.Column<string>(type: "varchar(39)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ip_end = table.Column<string>(type: "varchar(39)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    country = table.Column<string>(type: "varchar(2)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    stateprov = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    district = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    city = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    zipcode = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    latitude = table.Column<long>(type: "bigint", nullable: false),
                    longitude = table.Column<long>(type: "bigint", nullable: false),
                    geoname_id = table.Column<int>(type: "int", nullable: false),
                    timezone_offset = table.Column<double>(type: "double", nullable: false),
                    timezone_name = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    processed = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbip_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mobile_app_install",
                columns: table => new
                {
                    user_email = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    app_type = table.Column<int>(type: "int", nullable: false),
                    registered_on = table.Column<DateTime>(type: "datetime", nullable: false),
                    last_sign = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.user_email, x.app_type });
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    Region = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Provider = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    connection_string = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regions", x => x.Region);
                });

            migrationBuilder.CreateIndex(
                name: "ip_start",
                table: "dbip_location",
                column: "ip_start");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dbip_location");

            migrationBuilder.DropTable(
                name: "mobile_app_install");

            migrationBuilder.DropTable(
                name: "regions");
        }
    }
}
