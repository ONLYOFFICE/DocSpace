using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASC.Migrations.PostgreSql.Migrations
{
    public partial class NotifyDbContextMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "notify_info",
                schema: "onlyoffice",
                columns: table => new
                {
                    notify_id = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    modify_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notify_info_pkey", x => x.notify_id);
                });

            migrationBuilder.CreateTable(
                name: "notify_queue",
                schema: "onlyoffice",
                columns: table => new
                {
                    notify_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    sender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    reciever = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    subject = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    content_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    content = table.Column<string>(type: "text", nullable: true),
                    sender_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    reply_to = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    attachments = table.Column<string>(type: "text", nullable: true),
                    auto_submitted = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("notify_queue_pkey", x => x.notify_id);
                });

            migrationBuilder.CreateIndex(
                name: "state",
                schema: "onlyoffice",
                table: "notify_info",
                column: "state");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notify_info",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "notify_queue",
                schema: "onlyoffice");
        }
    }
}
