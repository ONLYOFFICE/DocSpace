using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class CustomDbContextMigrate : Migration
    {
        /// <inheritdoc />
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
                    ipstart = table.Column<string>(name: "ip_start", type: "character varying(39)", maxLength: 39, nullable: false),
                    ipend = table.Column<string>(name: "ip_end", type: "character varying(39)", maxLength: 39, nullable: false),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    stateprov = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    district = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    city = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    zipcode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    latitude = table.Column<long>(type: "bigint", nullable: true),
                    longitude = table.Column<long>(type: "bigint", nullable: true),
                    geonameid = table.Column<int>(name: "geoname_id", type: "integer", nullable: true),
                    timezoneoffset = table.Column<double>(name: "timezone_offset", type: "double precision", nullable: true),
                    timezonename = table.Column<string>(name: "timezone_name", type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
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
                    useremail = table.Column<string>(name: "user_email", type: "character varying(255)", maxLength: 255, nullable: false),
                    apptype = table.Column<int>(name: "app_type", type: "integer", nullable: false),
                    registeredon = table.Column<DateTime>(name: "registered_on", type: "timestamp with time zone", nullable: false),
                    lastsign = table.Column<DateTime>(name: "last_sign", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mobile_app_install_pkey", x => new { x.useremail, x.apptype });
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

        /// <inheritdoc />
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
