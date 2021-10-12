using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASC.Core.Common.Migrations.PostgreSql.MailDbContextPostgreSql
{
    public partial class MailDbContextPostgreSql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "onlyoffice");

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccessToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GreyListingWhiteList",
                columns: table => new
                {
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GreyListingWhiteList", x => x.Comment);
                });

            migrationBuilder.CreateTable(
                name: "mail_mailbox",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant = table.Column<int>(type: "integer", nullable: false),
                    id_user = table.Column<string>(type: "character varying(38)", maxLength: 38, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'1'::smallint"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'0'"),
                    is_processed = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'0'"),
                    is_server_mailbox = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'0'"),
                    IsTeamlabMailbox = table.Column<bool>(type: "boolean", nullable: false),
                    imap = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'0'"),
                    user_online = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'0'"),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'0'"),
                    msg_count_last = table.Column<int>(type: "integer", nullable: false),
                    size_last = table.Column<int>(type: "integer", nullable: false),
                    login_delay = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "'30'"),
                    quota_error = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "'0'"),
                    imap_intervals = table.Column<string>(type: "text", nullable: true),
                    begin_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "'1975-01-01 00:00:00'"),
                    email_in_folder = table.Column<string>(type: "text", nullable: true),
                    pop3_password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    smtp_password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    token_type = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "'0'"),
                    token = table.Column<string>(type: "text", nullable: true),
                    id_smtp_server = table.Column<int>(type: "integer", nullable: false),
                    id_in_server = table.Column<int>(type: "integer", nullable: false),
                    date_checked = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_user_checked = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_login_delay_expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "'1975-01-01 00:00:00'"),
                    date_auth_error = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    display_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    display_short_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL"),
                    documentation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_mailbox_provider", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mail_server_server",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mx_record = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, defaultValueSql: "' '"),
                    connection_string = table.Column<string>(type: "text", nullable: false),
                    server_type = table.Column<int>(type: "integer", nullable: false),
                    smtp_settings_id = table.Column<int>(type: "integer", nullable: false),
                    imap_settings_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_server_server", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MailboxServer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProvider = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Hostname = table.Column<string>(type: "text", nullable: true),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    SocketType = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    Authentication = table.Column<string>(type: "text", nullable: true),
                    IsUserData = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailboxServer", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 1, "1&1", "1&1", "http://hilfe-center.1und1.de/access/search/go.php?t=e698123", "1und1.de" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 141, "??????????", "INET-SHIBATA", "pop.shibata.ne.jp" },
                    { 142, "Posteo", "Posteo", "posteo.de" },
                    { 143, "???", "???", "purple.plala.or.jp" },
                    { 144, "qip.ru", "qip.ru", "qip.ru" },
                    { 145, "???", "???", "rainbow.plala.or.jp" },
                    { 146, "Rambler Mail", "Rambler", "rambler.ru" },
                    { 147, "???", "???", "red.plala.or.jp" },
                    { 148, "???", "???", "rmail.plala.or.jp" },
                    { 149, "???", "???", "rondo.plala.or.jp" },
                    { 150, "???", "???", "rose.plala.or.jp" },
                    { 151, "???", "???", "rouge.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 152, "RoadRunner", "RR", "http://help.rr.com/HMSFaqs/e_emailserveraddys.aspx", "rr.com" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 153, "???", "???", "ruby.plala.or.jp" },
                    { 154, "?????????", "Saku-Net", "sakunet.ne.jp" },
                    { 155, "???", "???", "sea.plala.or.jp" },
                    { 156, "???", "???", "sepia.plala.or.jp" },
                    { 157, "???", "???", "serenade.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[,]
                {
                    { 158, "Seznam", "Seznam", "http://napoveda.seznam.cz/cz/jake-jsou-adresy-pop3-a-smtp-serveru.html", "seznam.cz" },
                    { 159, "SFR / Neuf", "SFR", "http://assistance.sfr.fr/internet_neufbox-de-SFR/utiliser-email/parametrer-id-sfr/fc-505-50680", "sfr.fr" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 160, "???", "???", "silk.plala.or.jp" },
                    { 161, "???", "???", "silver.plala.or.jp" },
                    { 162, "???", "???", "sky.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 163, "skynet", "skynet", "http://support.en.belgacom.be/app/answers/detail/a_id/14337/kw/thunderbird", "skynet.be" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 140, "???", "???", "polka.plala.or.jp" },
                    { 139, "'?????????", "wind", "po.wind.ne.jp" },
                    { 138, "DCN???????????", "DCN", "po.dcn.ne.jp" },
                    { 137, "???", "???", "plum.plala.or.jp" },
                    { 113, "Apple iCloud", "Apple", "me.com" },
                    { 114, "???", "???", "minuet.plala.or.jp" },
                    { 115, "???????????", "IWAFUNE", "ml.murakami.ne.jp" },
                    { 116, "Mnet ??? ????", "Mnet???", "mnet.ne.jp" },
                    { 117, "mopera U", "mopera U", "'mopera.net" },
                    { 118, "Mozilla Corporation and Mozilla Foundation internal email addresses", "mozilla.com", "mozilla.com" },
                    { 119, "TikiTiki???????", "TikiTiki", "mx1.tiki.ne.jp" },
                    { 120, "???", "???", "navy.plala.or.jp" },
                    { 121, "nctsoft", "nct", "nctsoft.com" },
                    { 122, "@nifty", "@nifty", "nifty.com" },
                    { 123, "BB????", "NSAT", "nsat.jp" },
                    { 164, "???", "???", "smail.plala.or.jp" },
                    { 124, "o2 Poczta", "o2", "o2.pl" },
                    { 126, "Poczta Onet", "Onet", "onet.pl" },
                    { 127, "???", "???", "opal.plala.or.jp" },
                    { 128, "???", "???", "orange.plala.or.jp" },
                    { 129, "???", "???", "orchid.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 130, "OVH", "OVH", "http://guides.ovh.com/ConfigurationEmail", "ovh.net" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 131, "????FTTH", "????FTTH", "pal.kijimadaira.jp" },
                    { 132, "???", "???", "palette.plala.or.jp" },
                    { 133, "??????", "PARABOX", "parabox.or.jp" },
                    { 134, "Portland State University Mail", "PSU Mail", "pdx.edu" },
                    { 135, "???", "???", "peach.plala.or.jp" },
                    { 136, "PeoplePC", "PeoplePC", "peoplepc.com" },
                    { 125, "???", "???", "olive.plala.or.jp" },
                    { 112, "???", "???", "maroon.plala.or.jp" },
                    { 165, "???", "???", "snow.plala.or.jp" },
                    { 167, "???", "???", "sonata.plala.or.jp" },
                    { 196, "Your WildWest domain", "WildWest", "wildwestdomains.com" },
                    { 197, "???", "???", "wine.plala.or.jp" },
                    { 198, "???", "???", "wmail.plala.or.jp" },
                    { 199, "Poczta Wirtualna Polska", "Poczta WP", "wp.pl" },
                    { 200, "???", "???", "xmail.plala.or.jp" },
                    { 201, "?????????", "wind", "xp.wind.jp" },
                    { 202, "???", "???", "xpost.plala.or.jp" },
                    { 203, "XS4All", "XS4All", "xs4all.nl" },
                    { 204, "Yahoo! Mail", "Yahoo", "xtra.co.nz" },
                    { 205, "Yahoo! ???", "Yahoo! ??? ", "yahoo.co.jp" },
                    { 206, "Yahoo! Mail", "Yahoo", "yahoo.com" },
                    { 207, "Yandex Mail", "Yandex", "yandex.ru" },
                    { 208, "Yahoo! BB", "Yahoo! BB", "ybb.ne.jp" },
                    { 209, "???", "???", "yellow.plala.or.jp" },
                    { 210, "???", "???", "ymail.plala.or.jp" },
                    { 211, "???", "???", "ypost.plala.or.jp" },
                    { 212, "Ziggo", "Ziggo", "ziggo.nl" },
                    { 213, "???", "???", "zmail.plala.or.jp" },
                    { 214, "???", "???", "zpost.plala.or.jp" },
                    { 215, "avsmedia.com", "avsmedia", "avsmedia.com" },
                    { 216, "avsmedia.net", "avsmedia", "avsmedia.net" },
                    { 218, "ilearney.com", "ilearney.com", "ilearney.com" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 219, "fpl-technology.com", "fpl-technology.com", "http://fpl-technology.com", "fpl -technology.com" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[] { 195, "???", "???", "white.plala.or.jp" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 194, "WEB.DE Freemail", "Web.de", "http://hilfe.freemail.web.de/freemail/e-mail/pop3/thunderbird/", "web.de" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 193, "???", "???", "wave.plala.or.jp" },
                    { 192, "???", "???", "waltz.plala.or.jp" },
                    { 168, "Strato", "Strato", "strato.de" },
                    { 169, "Universita degli Studi di Verona", "UniVR", "studenti.univr.it" },
                    { 170, "???", "???", "suite.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 171, "Sympatico Email", "Sympatico", "http://internet.bell.ca/index.cfm?method=content.view&category_id=585&content_id=12767", "sympatico.ca" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 172, "???", "???", "symphony.plala.or.jp" },
                    { 173, "T-Online", "T-Online", "t-online.de" },
                    { 174, "???", "???", "taupe.plala.or.jp" },
                    { 175, "Correo Terra", "Terra", "terra.es" },
                    { 176, "TikiTiki???????", "TikiTiki", "tiki.ne.jp" },
                    { 177, "Tiscali", "Tiscali", "tiscali.cz" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 178, "Tiscali Italy", "Tiscali", "http://assistenza.tiscali.it/tecnica/posta/configurazioni/", "tiscali.it" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 166, "?????????", "wind", "so.wind.ne.jp" },
                    { 179, "???", "???", "tmail.plala.or.jp" },
                    { 181, "???", "???", "topaz.plala.or.jp" },
                    { 182, "???", "???", "trio.plala.or.jp" },
                    { 183, "???", "???", "umail.plala.or.jp" },
                    { 184, "UM ITCS Email", "UM ITCS", "umich.edu" },
                    { 185, "UPC Nederland", "UPC", "upcmail.nl" },
                    { 186, "Verizon Online", "Verizon", "verizon.net" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 187, "Versatel", "Versatel", "http://www.versatel.de/hilfe/index_popup.php?einrichtung_email_programm", "versatel.de" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 188, "???", "???", "violet.plala.or.jp" },
                    { 189, "aikis", "aikis", "vm.aikis.or.jp" },
                    { 190, "???", "???", "vmail.plala.or.jp" },
                    { 191, "TikiTiki???????", "TikiTiki", "vp.tiki.ne.jp" },
                    { 180, "???", "???", "toccata.plala.or.jp" },
                    { 111, "?????????", "???", "mail.wind.ne.jp" },
                    { 110, "Telenor Danmark", "Telenor", "mail.telenor.dk" },
                    { 109, "mail.ru", "mail.ru", "mail.ru" },
                    { 30, "???????????", "CEK-Net", "cek.ne.jp" },
                    { 31, "UCSF CGL email", "CGL emai", "cgl.ucsf.edu" },
                    { 32, "Charter Commuications", "Charter", "charter.com" },
                    { 33, "CLIO-Net??????", "CLIO-Net", "clio.ne.jp" },
                    { 34, "???", "???", "cmail.plala.or.jp" },
                    { 35, "?????????", "wind", "co1.wind.ne.jp" },
                    { 36, "?????????", "wind", "co2.wind.ne.jp" },
                    { 37, "?????????", "wind", "co3.wind.ne.jp" },
                    { 38, "???", "???", "cocoa.plala.or.jp" },
                    { 39, "???", "Arcor", "coda.plala.or.jp" },
                    { 40, "???", "Comcast", "comcast.net" },
                    { 41, "???", "???", "concerto.plala.or.jp" },
                    { 42, "???", "???", "coral.plala.or.jp" },
                    { 43, "???", "???", "courante.plala.or.jp" },
                    { 44, "???", "???", "cpost.plala.or.jp" },
                    { 45, "???", "???", "cream.plala.or.jp" },
                    { 46, "???", "wind", "dan.wind.ne.jp" },
                    { 47, "???", "???", "dance.plala.or.jp" },
                    { 48, "IIJ4U", "???", "dd.iij4u.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 49, "domainFACTORY", "domainFACTORY", "http://www.df.eu/de/service/df-faq/e-mail/mail-programme/", "df.eu" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[] { 50, "???", "???", "dmail.plala.or.jp" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 51, "EarthLink", "EarthLink", "http://support.earthlink.net/email/email-server-settings.php", "earthlink.net" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 52, "???", "???", "ebony.plala.or.jp" },
                    { 29, "CC9???????????", "CC9", "cc9.ne.jp" },
                    { 28, "???", "???", "cameo.plala.or.jp" },
                    { 27, "???", "???", "camel.plala.or.jp" },
                    { 26, "???", "???", "brown.plala.or.jp" },
                    { 2, "???", "???", "abc.plala.or.jp" },
                    { 3, "???", "???", "agate.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 4, "Alice Italy", "Alice", "http://aiuto.alice.it/informazioni/clientemail/thunderbird.html", "alice.it" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 5, "???", "???", "amail.plala.or.jp" },
                    { 6, "???", "???", "amber.plala.or.jp" },
                    { 7, "AOL Mail", "AOL", "aol.com" },
                    { 8, "???", "???", "apost.plala.or.jp" },
                    { 9, "???", "???", "aqua.plala.or.jp" },
                    { 10, "Arcor", "Arcor", "arcor.de" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[,]
                {
                    { 11, "Aruba PEC", "Aruba", "http://pec.aruba.it/guide_filmate.asp", "arubapec.it" },
                    { 12, "AT&T", "AT&T", "http://www.att.com/esupport/article.jsp?sid=KB401570&ct=9000152", "att.net" },
                    { 53, "email.it", "email.it", "http://www.email.it/ita/config/thunder.php", "email.it" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 13, "???", "???", "ballade.plala.or.jp" },
                    { 15, "BB????", "BB-NIIGATA", "bb-niigata.jp" },
                    { 16, "???", "???", "beige.plala.or.jp" },
                    { 17, "Biglobe", "Biglobe", "biglobe.ne.jp" },
                    { 18, "Telstra Bigpond", "Bigpond", "bigpond.com" },
                    { 19, "???", "???", "blue.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[,]
                {
                    { 20, "bluewin.ch", "bluewin.ch", "http://smtphelp.bluewin.ch/swisscomdtg/setup/?", "bluemail.ch" },
                    { 21, "bluewin.ch", "bluewin.ch", "http://smtphelp.bluewin.ch/swisscomdtg/setup/", "bluewin.ch" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 22, "???", "???", "bmail.plala.or.jp" },
                    { 23, "???", "???", "bolero.plala.or.jp" },
                    { 24, "???", "???", "bpost.plala.or.jp" },
                    { 25, "???", "???", "broba.cc" },
                    { 14, "?????????", "wind", "bay.wind.ne.jp" },
                    { 54, "???", "???", "email.plala.or.jp" },
                    { 55, "EWE Tel", "EWE Tel", "ewetel.de" },
                    { 56, "???", "???", "fantasy.plala.or.jp" },
                    { 85, "IPAX Internet Services", "IPAX", "ipax.at" },
                    { 86, "???", "???", "ivory.plala.or.jp" },
                    { 87, "???????????", "IWAFUNE", "iwafune.ne.jp" },
                    { 88, "???", "???", "jade.plala.or.jp" },
                    { 89, "Janis", "Janis", "janis.or.jp" },
                    { 90, "JETINTERNET", "JET", "jet.ne.jp" },
                    { 91, "JETINTERNET", "JET", "ji.jet.ne.jp" },
                    { 92, "???", "???", "jmail.plala.or.jp" },
                    { 93, "Kabel Deutschland", "Kabel D", "kabelmail.de" },
                    { 94, "KELCOM Internet", "KELCOM", "kelcom.net" },
                    { 95, "???", "???", "khaki.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 84, "Internode", "Internode", "http://www.internode.on.net/support/guides/email/secure_email/", "internode.on.net" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 96, "?????????", "wind", "kl.wind.ne.jp" },
                    { 98, "????????????", "?????", "kokuyou.ne.jp" },
                    { 99, "???", "???", "lapis.plala.or.jp" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 100, "LaPoste", "LaPoste", "http://www.geckozone.org/forum/viewtopic.php?f=4&t=93118", "laposte.net" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[] { 101, "???", "???", "lemon.plala.or.jp" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 102, "Libero Mail", "Libero", "http://aiuto.libero.it/mail/istruzioni/configura-mozilla-thunderbird-per-windows-a11.phtml", "libero.it" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 103, "???", "???", "lilac.plala.or.jp" },
                    { 104, "???", "???", "lime.plala.or.jp" },
                    { 105, "???????????", "????", "mahoroba.ne.jp" },
                    { 106, "mail.com", "mail.com", "mail.com" },
                    { 107, "TDC (DK)", "TDC", "mail.dk" },
                    { 108, "???????????", "IWAFUNE", "mail.iwafune.ne.jp" },
                    { 97, "???", "???", "kmail.plala.or.jp" },
                    { 220, "Apple iCloud", "Apple", "icloud.com" },
                    { 83, "??????????", "INET-SHIBATA", "inet-shibata.or.jp" },
                    { 81, "Inbox.lv", "Inbox.lv", "inbox.lv" },
                    { 57, "???", "???", "flamenco.plala.or.jp" },
                    { 58, "???", "???", "fmail.plala.or.jp" },
                    { 59, "France Telecom / Orange", "Orange", "francetelecom.fr" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 60, "Free Telecom", "free.fr", "http://www.free.fr/assistance/599-thunderbird.html", "free.fr" });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "name" },
                values: new object[,]
                {
                    { 61, "Freenet Mail", "Freenet", "freenet.de" },
                    { 62, "???", "???", "fuga.plala.or.jp" },
                    { 63, "Gandi Mail", "Gandi", "gandi.net" },
                    { 64, "???", "???", "gmail.plala.or.jp" },
                    { 65, "GMX Freemail", "GMX", "gmx.com" },
                    { 66, "GMX Freemail", "GMX", "gmx.net" },
                    { 67, "????????????????????", "TVM-Net", "go.tvm.ne.jp" },
                    { 82, "???", "???", "indigo.plala.or.jp" },
                    { 68, "goo ????????", "goo", "goo.jp" },
                    { 70, "???", "???", "grape.plala.or.jp" },
                    { 71, "???", "???", "gray.plala.or.jp" },
                    { 72, "?????????", "HAL", "hal.ne.jp" },
                    { 73, "????????", "????", "hana.or.jp" },
                    { 74, "Microsoft Live Hotmail", "Hotmail", "hotmail.com" },
                    { 75, "SoftBank", "SoftBank", "i.softbank.jp" },
                    { 76, "IC-NET", "IC-NET", "ic-net.or.jp" },
                    { 77, "IIJmio ????????", "IIJmio", "iijmio-mail.jp" },
                    { 78, "???????i?????", "i?????", "iiyama-catv.ne.jp" },
                    { 79, "???", "???", "imail.plala.or.jp" },
                    { 80, "Inbox.lt", "Inbox.lt", "inbox.lt" },
                    { 69, "Google Mail", "GMail", "googlemail.com" }
                });

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[] { 221, "Microsoft Office 365", "Office365", "https://products.office.com", "office365.com" });

            migrationBuilder.CreateIndex(
                name: "address_index",
                schema: "onlyoffice",
                table: "mail_mailbox",
                column: "address");

            migrationBuilder.CreateIndex(
                name: "date_login_delay_expires",
                schema: "onlyoffice",
                table: "mail_mailbox",
                columns: new[] { "date_checked", "date_login_delay_expires" });

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
                name: "user_id_index",
                schema: "onlyoffice",
                table: "mail_mailbox",
                columns: new[] { "tenant", "id_user" });

            migrationBuilder.CreateIndex(
                name: "mail_server_server_type_server_type_fk_id",
                schema: "onlyoffice",
                table: "mail_server_server",
                column: "server_type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "GreyListingWhiteList");

            migrationBuilder.DropTable(
                name: "mail_mailbox",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "mail_mailbox_provider",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "mail_server_server",
                schema: "onlyoffice");

            migrationBuilder.DropTable(
                name: "MailboxServer");
        }
    }
}
