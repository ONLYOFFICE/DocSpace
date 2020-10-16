using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.Npgsql.MailDbContextNpgsql
{
    public partial class MailDbContextNpgsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:onlyoffice.enum_dbip_location", "ipv4,ipv6")
                .Annotation("Npgsql:Enum:onlyoffice.enum_mail_mailbox_server", "pop3,imap,smtp");

            migrationBuilder.CreateTable(
                name: "mail_mailbox",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(nullable: false),
                    id_user = table.Column<string>(maxLength: 38, nullable: false),
                    address = table.Column<string>(maxLength: 255, nullable: false),
                    name = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    enabled = table.Column<short>(nullable: false, defaultValueSql: "'1'::smallint"),
                    is_removed = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    is_processed = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    is_server_mailbox = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    imap = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    user_online = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    is_default = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    msg_count_last = table.Column<int>(nullable: false),
                    size_last = table.Column<int>(nullable: false),
                    login_delay = table.Column<long>(nullable: false, defaultValueSql: "'30'::bigint"),
                    quota_error = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    imap_intervals = table.Column<string>(nullable: true),
                    begin_date = table.Column<DateTime>(nullable: false, defaultValueSql: "'1975-01-01 00:00:00'::timestamp without time zone"),
                    email_in_folder = table.Column<string>(nullable: true),
                    pop3_password = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    smtp_password = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    token_type = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint"),
                    token = table.Column<string>(nullable: true),
                    id_smtp_server = table.Column<int>(nullable: false),
                    id_in_server = table.Column<int>(nullable: false),
                    date_checked = table.Column<DateTime>(nullable: true),
                    date_user_checked = table.Column<DateTime>(nullable: true),
                    date_login_delay_expires = table.Column<DateTime>(nullable: false, defaultValueSql: "'1975-01-01 00:00:00'::timestamp without time zone"),
                    date_auth_error = table.Column<DateTime>(nullable: true),
                    date_created = table.Column<DateTime>(nullable: true),
                    date_modified = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_mailbox", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mail_mailbox_provider",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 255, nullable: false),
                    display_name = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    display_short_name = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    documentation = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_mailbox_provider", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mail_mailbox_server",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_provider = table.Column<int>(nullable: false),
                    hostname = table.Column<string>(type: "character varying", nullable: false),
                    port = table.Column<int>(nullable: false),
                    socket_type = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "'plain'::character varying"),
                    username = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    authentication = table.Column<string>(maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    is_user_data = table.Column<short>(nullable: false, defaultValueSql: "'0'::smallint")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_mailbox_server", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mail_server_server",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mx_record = table.Column<string>(maxLength: 128, nullable: false, defaultValueSql: "' '::character varying"),
                    connection_string = table.Column<string>(nullable: false),
                    server_type = table.Column<int>(nullable: false),
                    smtp_settings_id = table.Column<int>(nullable: false),
                    imap_settings_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_server_server", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "address_index",
                schema: "onlyoffice",
                table: "mail_mailbox",
                column: "address");

            migrationBuilder.CreateIndex(
                name: "main_mailbox_id_in_server_mail_mailbox_server_id",
                schema: "onlyoffice",
                table: "mail_mailbox",
                column: "id_in_server");

            migrationBuilder.CreateIndex(
                name: "main_mailbox_id_smtp_server_mail_mailbox_server_id",
                schema: "onlyoffice",
                table: "mail_mailbox",
                column: "id_smtp_server");

            migrationBuilder.CreateIndex(
                name: "date_login_delay_expires",
                schema: "onlyoffice",
                table: "mail_mailbox",
                columns: new[] { "date_checked", "date_login_delay_expires" });

            migrationBuilder.CreateIndex(
                name: "user_id_index",
                schema: "onlyoffice",
                table: "mail_mailbox",
                columns: new[] { "tenant", "id_user" });

            migrationBuilder.CreateIndex(
                name: "id_provider_mail_mailbox_server",
                schema: "onlyoffice",
                table: "mail_mailbox_server",
                column: "id_provider");

            migrationBuilder.CreateIndex(
                name: "mail_server_server_type_server_type_fk_id",
                schema: "onlyoffice",
                table: "mail_server_server",
                column: "server_type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mail_mailbox",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "mail_mailbox_provider",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "mail_mailbox_server",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "mail_server_server",
                schema: "onlyoffice");
        }
    }
}
