using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.NotifyDbContextNpgsql
{
    public partial class NotifyDbContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            _ = migrationBuilder.CreateTable(
                name: "notify_info",
                schema: "onlyoffice",
                columns: table => new
                {
                    notify_id = table.Column<int>(nullable: false),
                    state = table.Column<int>(nullable: false),
                    attempts = table.Column<int>(nullable: false),
                    modify_date = table.Column<DateTime>(nullable: false),
                    priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("notify_info_pkey", x => x.notify_id);
                });

            _ = migrationBuilder.CreateTable(
                name: "notify_queue",
                schema: "onlyoffice",
                columns: table => new
                {
                    notify_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant_id = table.Column<int>(nullable: false),
                    sender = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    reciever = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    subject = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    content_type = table.Column<string>(maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    content = table.Column<string>(nullable: true),
                    sender_type = table.Column<string>(maxLength: 64, nullable: true, defaultValueSql: "NULL"),
                    reply_to = table.Column<string>(maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    creation_date = table.Column<DateTime>(nullable: false),
                    attachments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("notify_queue_pkey", x => x.notify_id);
                });

            _ = migrationBuilder.AddColumn<string>(
                name: "auto_submitted",
                schema: "onlyoffice",
                table: "notify_queue",
                maxLength: 64,
                nullable: true,
                defaultValueSql: "NULL");

            _ = migrationBuilder.CreateIndex(
                name: "state",
                schema: "onlyoffice",
                table: "notify_info",
                column: "state");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "notify_info",
                schema: "onlyoffice");

            _ = migrationBuilder.DropTable(
                name: "notify_queue",
                schema: "onlyoffice");
        }
    }
}
