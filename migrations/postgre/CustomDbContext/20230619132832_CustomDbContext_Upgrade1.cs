using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.CustomDb
{
    /// <inheritdoc />
    public partial class CustomDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dbip_location",
                schema: "onlyoffice");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:onlyoffice.enum_dbip_location", "ipv4,ipv6");

            migrationBuilder.CreateTable(
                name: "dbip_lookup",
                columns: table => new
                {
                    addrtype = table.Column<string>(name: "addr_type", type: "enum('ipv4','ipv6')", nullable: false),
                    ipstart = table.Column<byte[]>(name: "ip_start", type: "varbinary(16)", nullable: false),
                    ipend = table.Column<byte[]>(name: "ip_end", type: "varbinary(16)", nullable: false),
                    continent = table.Column<string>(type: "char(2)", nullable: false),
                    country = table.Column<string>(type: "char(2)", nullable: false),
                    stateprovcode = table.Column<string>(name: "stateprov_code", type: "varchar(15)", nullable: true),
                    stateprov = table.Column<string>(type: "varchar(80)", nullable: false),
                    district = table.Column<string>(type: "varchar(80)", nullable: false),
                    city = table.Column<string>(type: "varchar(80)", nullable: false),
                    zipcode = table.Column<string>(type: "varchar(20)", nullable: true),
                    latitude = table.Column<float>(type: "float", nullable: false),
                    longitude = table.Column<float>(type: "float", nullable: false),
                    geonameid = table.Column<int>(name: "geoname_id", type: "int(10)", nullable: true),
                    timezoneoffset = table.Column<float>(name: "timezone_offset", type: "float", nullable: false),
                    timezonename = table.Column<string>(name: "timezone_name", type: "varchar(64)", nullable: false),
                    weathercode = table.Column<string>(name: "weather_code", type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbip_lookup", x => new { x.addrtype, x.ipstart });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dbip_lookup");

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
                    city = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    district = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    geonameid = table.Column<int>(name: "geoname_id", type: "integer", nullable: true),
                    ipend = table.Column<string>(name: "ip_end", type: "character varying(39)", maxLength: 39, nullable: false),
                    ipstart = table.Column<string>(name: "ip_start", type: "character varying(39)", maxLength: 39, nullable: false),
                    latitude = table.Column<long>(type: "bigint", nullable: true),
                    longitude = table.Column<long>(type: "bigint", nullable: true),
                    processed = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    stateprov = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    timezonename = table.Column<string>(name: "timezone_name", type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    timezoneoffset = table.Column<double>(name: "timezone_offset", type: "double precision", nullable: true),
                    zipcode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbip_location", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ip_start",
                schema: "onlyoffice",
                table: "dbip_location",
                column: "ip_start");
        }
    }
}
