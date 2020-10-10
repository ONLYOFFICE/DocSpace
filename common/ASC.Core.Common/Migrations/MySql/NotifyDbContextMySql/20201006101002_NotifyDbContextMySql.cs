using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.MySql.NotifyDbContextMySql
{
    public partial class NotifyDbContextMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "notify_info",
                columns: table => new
                {
                    notify_id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    state = table.Column<int>(nullable: false),
                    attempts = table.Column<int>(nullable: false),
                    modify_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => x.notify_id);
                });

            _ = migrationBuilder.CreateTable(
                name: "notify_queue",
                columns: table => new
                {
                    notify_id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<int>(nullable: false),
                    sender = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    reciever = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    subject = table.Column<string>(type: "varchar(1024)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    content_type = table.Column<string>(type: "varchar(64)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    sender_type = table.Column<string>(type: "varchar(64)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    reply_to = table.Column<string>(type: "varchar(1024)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    creation_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    attachments = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci"),
                    auto_submitted = table.Column<string>(type: "varchar(64)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PRIMARY", x => x.notify_id);
                });

            _ = migrationBuilder.CreateIndex(
                name: "state",
                table: "notify_info",
                column: "state");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "notify_info");

            _ = migrationBuilder.DropTable(
                name: "notify_queue");
        }
    }
}
