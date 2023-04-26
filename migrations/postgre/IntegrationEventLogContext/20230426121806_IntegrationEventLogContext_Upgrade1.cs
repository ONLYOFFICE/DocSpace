using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations.IntegrationEventLog
{
    /// <inheritdoc />
    public partial class IntegrationEventLogContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbTenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    MappedDomain = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    VersionChanged = table.Column<DateTime>(name: "Version_Changed", type: "timestamp with time zone", nullable: true),
                    VersionChanged0 = table.Column<DateTime>(name: "VersionChanged", type: "timestamp with time zone", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    TrustedDomainsRaw = table.Column<string>(type: "text", nullable: true),
                    TrustedDomainsEnabled = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusChanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StatusChangedHack = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentId = table.Column<string>(type: "text", nullable: true),
                    Industry = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Spam = table.Column<bool>(type: "boolean", nullable: false),
                    Calls = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbTenant", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_event_bus_integration_event_log_DbTenant_tenant_id",
                table: "event_bus_integration_event_log",
                column: "tenant_id",
                principalTable: "DbTenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_bus_integration_event_log_DbTenant_tenant_id",
                table: "event_bus_integration_event_log");

            migrationBuilder.DropTable(
                name: "DbTenant");
        }
    }
}
