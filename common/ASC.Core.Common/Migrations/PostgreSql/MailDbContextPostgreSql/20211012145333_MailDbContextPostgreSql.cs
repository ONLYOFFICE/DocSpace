using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASC.Core.Common.Migrations.PostgreSql.MailDbContextPostgreSql
{
    public partial class MailDbContextPostgreSql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GreyListingWhiteList",
                columns: table => new
                {
                    Comment = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GreyListingWhiteList", x => x.Comment);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "mail_mailbox",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<string>(type: "varchar(38)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    address = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    name = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    enabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    is_removed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_processed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_server_mailbox = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsTeamlabMailbox = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    imap = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    user_online = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_default = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    msg_count_last = table.Column<int>(type: "int", nullable: false),
                    size_last = table.Column<int>(type: "int", nullable: false),
                    login_delay = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'30'"),
                    quota_error = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    imap_intervals = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    begin_date = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "'1975-01-01 00:00:00'"),
                    email_in_folder = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    pop3_password = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    smtp_password = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    token_type = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "text", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    id_smtp_server = table.Column<int>(type: "int", nullable: false),
                    id_in_server = table.Column<int>(type: "int", nullable: false),
                    date_checked = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_user_checked = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_login_delay_expires = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'1975-01-01 00:00:00'"),
                    date_auth_error = table.Column<DateTime>(type: "datetime", nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_modified = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_mailbox", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "mail_mailbox_provider",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    display_name = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    display_short_name = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    documentation = table.Column<string>(type: "varchar(255)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_mailbox_provider", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "mail_server_server",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mx_record = table.Column<string>(type: "varchar(128)", nullable: false, defaultValueSql: "''", collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    connection_string = table.Column<string>(type: "text", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    server_type = table.Column<int>(type: "int", nullable: false),
                    smtp_settings_id = table.Column<int>(type: "int", nullable: false),
                    imap_settings_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_server_server", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MailboxServer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdProvider = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hostname = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Port = table.Column<int>(type: "int", nullable: false),
                    SocketType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Authentication = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsUserData = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailboxServer", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "mail_mailbox_provider",
                columns: new[] { "id", "display_name", "display_short_name", "documentation", "name" },
                values: new object[,]
                {
                    { 1, "1&1", "1&1", "http://hilfe-center.1und1.de/access/search/go.php?t=e698123", "1und1.de" },
                    { 141, "??????????", "INET-SHIBATA", null, "pop.shibata.ne.jp" },
                    { 142, "Posteo", "Posteo", null, "posteo.de" },
                    { 143, "???", "???", null, "purple.plala.or.jp" },
                    { 144, "qip.ru", "qip.ru", null, "qip.ru" },
                    { 145, "???", "???", null, "rainbow.plala.or.jp" },
                    { 146, "Rambler Mail", "Rambler", null, "rambler.ru" },
                    { 147, "???", "???", null, "red.plala.or.jp" },
                    { 148, "???", "???", null, "rmail.plala.or.jp" },
                    { 149, "???", "???", null, "rondo.plala.or.jp" },
                    { 150, "???", "???", null, "rose.plala.or.jp" },
                    { 151, "???", "???", null, "rouge.plala.or.jp" },
                    { 152, "RoadRunner", "RR", "http://help.rr.com/HMSFaqs/e_emailserveraddys.aspx", "rr.com" },
                    { 153, "???", "???", null, "ruby.plala.or.jp" },
                    { 154, "?????????", "Saku-Net", null, "sakunet.ne.jp" },
                    { 155, "???", "???", null, "sea.plala.or.jp" },
                    { 156, "???", "???", null, "sepia.plala.or.jp" },
                    { 157, "???", "???", null, "serenade.plala.or.jp" },
                    { 158, "Seznam", "Seznam", "http://napoveda.seznam.cz/cz/jake-jsou-adresy-pop3-a-smtp-serveru.html", "seznam.cz" },
                    { 159, "SFR / Neuf", "SFR", "http://assistance.sfr.fr/internet_neufbox-de-SFR/utiliser-email/parametrer-id-sfr/fc-505-50680", "sfr.fr" },
                    { 160, "???", "???", null, "silk.plala.or.jp" },
                    { 161, "???", "???", null, "silver.plala.or.jp" },
                    { 162, "???", "???", null, "sky.plala.or.jp" },
                    { 163, "skynet", "skynet", "http://support.en.belgacom.be/app/answers/detail/a_id/14337/kw/thunderbird", "skynet.be" },
                    { 140, "???", "???", null, "polka.plala.or.jp" },
                    { 139, "'?????????", "wind", null, "po.wind.ne.jp" },
                    { 138, "DCN???????????", "DCN", null, "po.dcn.ne.jp" },
                    { 137, "???", "???", null, "plum.plala.or.jp" },
                    { 113, "Apple iCloud", "Apple", null, "me.com" },
                    { 114, "???", "???", null, "minuet.plala.or.jp" },
                    { 115, "???????????", "IWAFUNE", null, "ml.murakami.ne.jp" },
                    { 116, "Mnet ??? ????", "Mnet???", null, "mnet.ne.jp" },
                    { 117, "mopera U", "mopera U", null, "'mopera.net" },
                    { 118, "Mozilla Corporation and Mozilla Foundation internal email addresses", "mozilla.com", null, "mozilla.com" },
                    { 119, "TikiTiki???????", "TikiTiki", null, "mx1.tiki.ne.jp" },
                    { 120, "???", "???", null, "navy.plala.or.jp" },
                    { 121, "nctsoft", "nct", null, "nctsoft.com" },
                    { 122, "@nifty", "@nifty", null, "nifty.com" },
                    { 123, "BB????", "NSAT", null, "nsat.jp" },
                    { 164, "???", "???", null, "smail.plala.or.jp" },
                    { 124, "o2 Poczta", "o2", null, "o2.pl" },
                    { 126, "Poczta Onet", "Onet", null, "onet.pl" },
                    { 127, "???", "???", null, "opal.plala.or.jp" },
                    { 128, "???", "???", null, "orange.plala.or.jp" },
                    { 129, "???", "???", null, "orchid.plala.or.jp" },
                    { 130, "OVH", "OVH", "http://guides.ovh.com/ConfigurationEmail", "ovh.net" },
                    { 131, "????FTTH", "????FTTH", null, "pal.kijimadaira.jp" },
                    { 132, "???", "???", null, "palette.plala.or.jp" },
                    { 133, "??????", "PARABOX", null, "parabox.or.jp" },
                    { 134, "Portland State University Mail", "PSU Mail", null, "pdx.edu" },
                    { 135, "???", "???", null, "peach.plala.or.jp" },
                    { 136, "PeoplePC", "PeoplePC", null, "peoplepc.com" },
                    { 125, "???", "???", null, "olive.plala.or.jp" },
                    { 112, "???", "???", null, "maroon.plala.or.jp" },
                    { 165, "???", "???", null, "snow.plala.or.jp" },
                    { 167, "???", "???", null, "sonata.plala.or.jp" },
                    { 196, "Your WildWest domain", "WildWest", null, "wildwestdomains.com" },
                    { 197, "???", "???", null, "wine.plala.or.jp" },
                    { 198, "???", "???", null, "wmail.plala.or.jp" },
                    { 199, "Poczta Wirtualna Polska", "Poczta WP", null, "wp.pl" },
                    { 200, "???", "???", null, "xmail.plala.or.jp" },
                    { 201, "?????????", "wind", null, "xp.wind.jp" },
                    { 202, "???", "???", null, "xpost.plala.or.jp" },
                    { 203, "XS4All", "XS4All", null, "xs4all.nl" },
                    { 204, "Yahoo! Mail", "Yahoo", null, "xtra.co.nz" },
                    { 205, "Yahoo! ???", "Yahoo! ??? ", null, "yahoo.co.jp" },
                    { 206, "Yahoo! Mail", "Yahoo", null, "yahoo.com" },
                    { 207, "Yandex Mail", "Yandex", null, "yandex.ru" },
                    { 208, "Yahoo! BB", "Yahoo! BB", null, "ybb.ne.jp" },
                    { 209, "???", "???", null, "yellow.plala.or.jp" },
                    { 210, "???", "???", null, "ymail.plala.or.jp" },
                    { 211, "???", "???", null, "ypost.plala.or.jp" },
                    { 212, "Ziggo", "Ziggo", null, "ziggo.nl" },
                    { 213, "???", "???", null, "zmail.plala.or.jp" },
                    { 214, "???", "???", null, "zpost.plala.or.jp" },
                    { 215, "avsmedia.com", "avsmedia", null, "avsmedia.com" },
                    { 216, "avsmedia.net", "avsmedia", null, "avsmedia.net" },
                    { 218, "ilearney.com", "ilearney.com", null, "ilearney.com" },
                    { 219, "fpl-technology.com", "fpl-technology.com", "http://fpl-technology.com", "fpl -technology.com" },
                    { 195, "???", "???", null, "white.plala.or.jp" },
                    { 194, "WEB.DE Freemail", "Web.de", "http://hilfe.freemail.web.de/freemail/e-mail/pop3/thunderbird/", "web.de" },
                    { 193, "???", "???", null, "wave.plala.or.jp" },
                    { 192, "???", "???", null, "waltz.plala.or.jp" },
                    { 168, "Strato", "Strato", null, "strato.de" },
                    { 169, "Universita degli Studi di Verona", "UniVR", null, "studenti.univr.it" },
                    { 170, "???", "???", null, "suite.plala.or.jp" },
                    { 171, "Sympatico Email", "Sympatico", "http://internet.bell.ca/index.cfm?method=content.view&category_id=585&content_id=12767", "sympatico.ca" },
                    { 172, "???", "???", null, "symphony.plala.or.jp" },
                    { 173, "T-Online", "T-Online", null, "t-online.de" },
                    { 174, "???", "???", null, "taupe.plala.or.jp" },
                    { 175, "Correo Terra", "Terra", null, "terra.es" },
                    { 176, "TikiTiki???????", "TikiTiki", null, "tiki.ne.jp" },
                    { 177, "Tiscali", "Tiscali", null, "tiscali.cz" },
                    { 178, "Tiscali Italy", "Tiscali", "http://assistenza.tiscali.it/tecnica/posta/configurazioni/", "tiscali.it" },
                    { 166, "?????????", "wind", null, "so.wind.ne.jp" },
                    { 179, "???", "???", null, "tmail.plala.or.jp" },
                    { 181, "???", "???", null, "topaz.plala.or.jp" },
                    { 182, "???", "???", null, "trio.plala.or.jp" },
                    { 183, "???", "???", null, "umail.plala.or.jp" },
                    { 184, "UM ITCS Email", "UM ITCS", null, "umich.edu" },
                    { 185, "UPC Nederland", "UPC", null, "upcmail.nl" },
                    { 186, "Verizon Online", "Verizon", null, "verizon.net" },
                    { 187, "Versatel", "Versatel", "http://www.versatel.de/hilfe/index_popup.php?einrichtung_email_programm", "versatel.de" },
                    { 188, "???", "???", null, "violet.plala.or.jp" },
                    { 189, "aikis", "aikis", null, "vm.aikis.or.jp" },
                    { 190, "???", "???", null, "vmail.plala.or.jp" },
                    { 191, "TikiTiki???????", "TikiTiki", null, "vp.tiki.ne.jp" },
                    { 180, "???", "???", null, "toccata.plala.or.jp" },
                    { 111, "?????????", "???", null, "mail.wind.ne.jp" },
                    { 110, "Telenor Danmark", "Telenor", null, "mail.telenor.dk" },
                    { 109, "mail.ru", "mail.ru", null, "mail.ru" },
                    { 30, "???????????", "CEK-Net", null, "cek.ne.jp" },
                    { 31, "UCSF CGL email", "CGL emai", null, "cgl.ucsf.edu" },
                    { 32, "Charter Commuications", "Charter", null, "charter.com" },
                    { 33, "CLIO-Net??????", "CLIO-Net", null, "clio.ne.jp" },
                    { 34, "???", "???", null, "cmail.plala.or.jp" },
                    { 35, "?????????", "wind", null, "co1.wind.ne.jp" },
                    { 36, "?????????", "wind", null, "co2.wind.ne.jp" },
                    { 37, "?????????", "wind", null, "co3.wind.ne.jp" },
                    { 38, "???", "???", null, "cocoa.plala.or.jp" },
                    { 39, "???", "Arcor", null, "coda.plala.or.jp" },
                    { 40, "???", "Comcast", null, "comcast.net" },
                    { 41, "???", "???", null, "concerto.plala.or.jp" },
                    { 42, "???", "???", null, "coral.plala.or.jp" },
                    { 43, "???", "???", null, "courante.plala.or.jp" },
                    { 44, "???", "???", null, "cpost.plala.or.jp" },
                    { 45, "???", "???", null, "cream.plala.or.jp" },
                    { 46, "???", "wind", null, "dan.wind.ne.jp" },
                    { 47, "???", "???", null, "dance.plala.or.jp" },
                    { 48, "IIJ4U", "???", null, "dd.iij4u.or.jp" },
                    { 49, "domainFACTORY", "domainFACTORY", "http://www.df.eu/de/service/df-faq/e-mail/mail-programme/", "df.eu" },
                    { 50, "???", "???", null, "dmail.plala.or.jp" },
                    { 51, "EarthLink", "EarthLink", "http://support.earthlink.net/email/email-server-settings.php", "earthlink.net" },
                    { 52, "???", "???", null, "ebony.plala.or.jp" },
                    { 29, "CC9???????????", "CC9", null, "cc9.ne.jp" },
                    { 28, "???", "???", null, "cameo.plala.or.jp" },
                    { 27, "???", "???", null, "camel.plala.or.jp" },
                    { 26, "???", "???", null, "brown.plala.or.jp" },
                    { 2, "???", "???", null, "abc.plala.or.jp" },
                    { 3, "???", "???", null, "agate.plala.or.jp" },
                    { 4, "Alice Italy", "Alice", "http://aiuto.alice.it/informazioni/clientemail/thunderbird.html", "alice.it" },
                    { 5, "???", "???", null, "amail.plala.or.jp" },
                    { 6, "???", "???", null, "amber.plala.or.jp" },
                    { 7, "AOL Mail", "AOL", null, "aol.com" },
                    { 8, "???", "???", null, "apost.plala.or.jp" },
                    { 9, "???", "???", null, "aqua.plala.or.jp" },
                    { 10, "Arcor", "Arcor", null, "arcor.de" },
                    { 11, "Aruba PEC", "Aruba", "http://pec.aruba.it/guide_filmate.asp", "arubapec.it" },
                    { 12, "AT&T", "AT&T", "http://www.att.com/esupport/article.jsp?sid=KB401570&ct=9000152", "att.net" },
                    { 53, "email.it", "email.it", "http://www.email.it/ita/config/thunder.php", "email.it" },
                    { 13, "???", "???", null, "ballade.plala.or.jp" },
                    { 15, "BB????", "BB-NIIGATA", null, "bb-niigata.jp" },
                    { 16, "???", "???", null, "beige.plala.or.jp" },
                    { 17, "Biglobe", "Biglobe", null, "biglobe.ne.jp" },
                    { 18, "Telstra Bigpond", "Bigpond", null, "bigpond.com" },
                    { 19, "???", "???", null, "blue.plala.or.jp" },
                    { 20, "bluewin.ch", "bluewin.ch", "http://smtphelp.bluewin.ch/swisscomdtg/setup/?", "bluemail.ch" },
                    { 21, "bluewin.ch", "bluewin.ch", "http://smtphelp.bluewin.ch/swisscomdtg/setup/", "bluewin.ch" },
                    { 22, "???", "???", null, "bmail.plala.or.jp" },
                    { 23, "???", "???", null, "bolero.plala.or.jp" },
                    { 24, "???", "???", null, "bpost.plala.or.jp" },
                    { 25, "???", "???", null, "broba.cc" },
                    { 14, "?????????", "wind", null, "bay.wind.ne.jp" },
                    { 54, "???", "???", null, "email.plala.or.jp" },
                    { 55, "EWE Tel", "EWE Tel", null, "ewetel.de" },
                    { 56, "???", "???", null, "fantasy.plala.or.jp" },
                    { 85, "IPAX Internet Services", "IPAX", null, "ipax.at" },
                    { 86, "???", "???", null, "ivory.plala.or.jp" },
                    { 87, "???????????", "IWAFUNE", null, "iwafune.ne.jp" },
                    { 88, "???", "???", null, "jade.plala.or.jp" },
                    { 89, "Janis", "Janis", null, "janis.or.jp" },
                    { 90, "JETINTERNET", "JET", null, "jet.ne.jp" },
                    { 91, "JETINTERNET", "JET", null, "ji.jet.ne.jp" },
                    { 92, "???", "???", null, "jmail.plala.or.jp" },
                    { 93, "Kabel Deutschland", "Kabel D", null, "kabelmail.de" },
                    { 94, "KELCOM Internet", "KELCOM", null, "kelcom.net" },
                    { 95, "???", "???", null, "khaki.plala.or.jp" },
                    { 84, "Internode", "Internode", "http://www.internode.on.net/support/guides/email/secure_email/", "internode.on.net" },
                    { 96, "?????????", "wind", null, "kl.wind.ne.jp" },
                    { 98, "????????????", "?????", null, "kokuyou.ne.jp" },
                    { 99, "???", "???", null, "lapis.plala.or.jp" },
                    { 100, "LaPoste", "LaPoste", "http://www.geckozone.org/forum/viewtopic.php?f=4&t=93118", "laposte.net" },
                    { 101, "???", "???", null, "lemon.plala.or.jp" },
                    { 102, "Libero Mail", "Libero", "http://aiuto.libero.it/mail/istruzioni/configura-mozilla-thunderbird-per-windows-a11.phtml", "libero.it" },
                    { 103, "???", "???", null, "lilac.plala.or.jp" },
                    { 104, "???", "???", null, "lime.plala.or.jp" },
                    { 105, "???????????", "????", null, "mahoroba.ne.jp" },
                    { 106, "mail.com", "mail.com", null, "mail.com" },
                    { 107, "TDC (DK)", "TDC", null, "mail.dk" },
                    { 108, "???????????", "IWAFUNE", null, "mail.iwafune.ne.jp" },
                    { 97, "???", "???", null, "kmail.plala.or.jp" },
                    { 220, "Apple iCloud", "Apple", null, "icloud.com" },
                    { 83, "??????????", "INET-SHIBATA", null, "inet-shibata.or.jp" },
                    { 81, "Inbox.lv", "Inbox.lv", null, "inbox.lv" },
                    { 57, "???", "???", null, "flamenco.plala.or.jp" },
                    { 58, "???", "???", null, "fmail.plala.or.jp" },
                    { 59, "France Telecom / Orange", "Orange", null, "francetelecom.fr" },
                    { 60, "Free Telecom", "free.fr", "http://www.free.fr/assistance/599-thunderbird.html", "free.fr" },
                    { 61, "Freenet Mail", "Freenet", null, "freenet.de" },
                    { 62, "???", "???", null, "fuga.plala.or.jp" },
                    { 63, "Gandi Mail", "Gandi", null, "gandi.net" },
                    { 64, "???", "???", null, "gmail.plala.or.jp" },
                    { 65, "GMX Freemail", "GMX", null, "gmx.com" },
                    { 66, "GMX Freemail", "GMX", null, "gmx.net" },
                    { 67, "????????????????????", "TVM-Net", null, "go.tvm.ne.jp" },
                    { 82, "???", "???", null, "indigo.plala.or.jp" },
                    { 68, "goo ????????", "goo", null, "goo.jp" },
                    { 70, "???", "???", null, "grape.plala.or.jp" },
                    { 71, "???", "???", null, "gray.plala.or.jp" },
                    { 72, "?????????", "HAL", null, "hal.ne.jp" },
                    { 73, "????????", "????", null, "hana.or.jp" },
                    { 74, "Microsoft Live Hotmail", "Hotmail", null, "hotmail.com" },
                    { 75, "SoftBank", "SoftBank", null, "i.softbank.jp" },
                    { 76, "IC-NET", "IC-NET", null, "ic-net.or.jp" },
                    { 77, "IIJmio ????????", "IIJmio", null, "iijmio-mail.jp" },
                    { 78, "???????i?????", "i?????", null, "iiyama-catv.ne.jp" },
                    { 79, "???", "???", null, "imail.plala.or.jp" },
                    { 80, "Inbox.lt", "Inbox.lt", null, "inbox.lt" },
                    { 69, "Google Mail", "GMail", null, "googlemail.com" },
                    { 221, "Microsoft Office 365", "Office365", "https://products.office.com", "office365.com" }
                });

            migrationBuilder.CreateIndex(
                name: "address_index",
                table: "mail_mailbox",
                column: "address");

            migrationBuilder.CreateIndex(
                name: "date_login_delay_expires",
                table: "mail_mailbox",
                columns: new[] { "date_checked", "date_login_delay_expires" });

            migrationBuilder.CreateIndex(
                name: "main_mailbox_id_in_server_mail_mailbox_server_id",
                table: "mail_mailbox",
                column: "id_in_server");

            migrationBuilder.CreateIndex(
                name: "main_mailbox_id_smtp_server_mail_mailbox_server_id",
                table: "mail_mailbox",
                column: "id_smtp_server");

            migrationBuilder.CreateIndex(
                name: "user_id_index",
                table: "mail_mailbox",
                columns: new[] { "tenant", "id_user" });

            migrationBuilder.CreateIndex(
                name: "mail_server_server_type_server_type_fk_id",
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
                name: "mail_mailbox");

            migrationBuilder.DropTable(
                name: "mail_mailbox_provider");

            migrationBuilder.DropTable(
                name: "mail_server_server");

            migrationBuilder.DropTable(
                name: "MailboxServer");
        }
    }
}
