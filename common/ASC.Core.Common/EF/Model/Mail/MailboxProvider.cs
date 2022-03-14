using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Mail
{
    public class MailboxProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DisplayShortName { get; set; }
        public string Documentation { get; set; }
    }
    public static class MailboxProviderExtension
    {
        public static ModelBuilderWrapper AddMailboxProvider(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddMailboxProvider, Provider.MySql)
                .Add(PgSqlAddMailboxProvider, Provider.PostgreSql)
                .HasData(
                new MailboxProvider { Id = 1, Name = "1und1.de", DisplayName = "1&1", DisplayShortName = "1&1", Documentation = "http://hilfe-center.1und1.de/access/search/go.php?t=e698123" },
                new MailboxProvider { Id = 2, Name = "abc.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 3, Name = "agate.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 4, Name = "alice.it", DisplayName = "Alice Italy", DisplayShortName = "Alice", Documentation = "http://aiuto.alice.it/informazioni/clientemail/thunderbird.html" },
                new MailboxProvider { Id = 5, Name = "amail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 6, Name = "amber.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 7, Name = "aol.com", DisplayName = "AOL Mail", DisplayShortName = "AOL", Documentation = null },
                new MailboxProvider { Id = 8, Name = "apost.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 9, Name = "aqua.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 10, Name = "arcor.de", DisplayName = "Arcor", DisplayShortName = "Arcor", Documentation = null },
                new MailboxProvider { Id = 11, Name = "arubapec.it", DisplayName = "Aruba PEC", DisplayShortName = "Aruba", Documentation = "http://pec.aruba.it/guide_filmate.asp" },
                new MailboxProvider { Id = 12, Name = "att.net", DisplayName = "AT&T", DisplayShortName = "AT&T", Documentation = "http://www.att.com/esupport/article.jsp?sid=KB401570&ct=9000152" },
                new MailboxProvider { Id = 13, Name = "ballade.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 14, Name = "bay.wind.ne.jp", DisplayName = "?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 15, Name = "bb-niigata.jp", DisplayName = "BB????", DisplayShortName = "BB-NIIGATA", Documentation = null },
                new MailboxProvider { Id = 16, Name = "beige.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 17, Name = "biglobe.ne.jp", DisplayName = "Biglobe", DisplayShortName = "Biglobe", Documentation = null },
                new MailboxProvider { Id = 18, Name = "bigpond.com", DisplayName = "Telstra Bigpond", DisplayShortName = "Bigpond", Documentation = null },
                new MailboxProvider { Id = 19, Name = "blue.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 20, Name = "bluemail.ch", DisplayName = "bluewin.ch", DisplayShortName = "bluewin.ch", Documentation = "http://smtphelp.bluewin.ch/swisscomdtg/setup/?" },
                new MailboxProvider { Id = 21, Name = "bluewin.ch", DisplayName = "bluewin.ch", DisplayShortName = "bluewin.ch", Documentation = "http://smtphelp.bluewin.ch/swisscomdtg/setup/" },
                new MailboxProvider { Id = 22, Name = "bmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 23, Name = "bolero.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 24, Name = "bpost.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 25, Name = "broba.cc", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 26, Name = "brown.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 27, Name = "camel.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 28, Name = "cameo.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 29, Name = "cc9.ne.jp", DisplayName = "CC9???????????", DisplayShortName = "CC9", Documentation = null },
                new MailboxProvider { Id = 30, Name = "cek.ne.jp", DisplayName = "???????????", DisplayShortName = "CEK-Net", Documentation = null },
                new MailboxProvider { Id = 31, Name = "cgl.ucsf.edu", DisplayName = "UCSF CGL email", DisplayShortName = "CGL emai", Documentation = null },
                new MailboxProvider { Id = 32, Name = "charter.com", DisplayName = "Charter Commuications", DisplayShortName = "Charter", Documentation = null },
                new MailboxProvider { Id = 33, Name = "clio.ne.jp", DisplayName = "CLIO-Net??????", DisplayShortName = "CLIO-Net", Documentation = null },
                new MailboxProvider { Id = 34, Name = "cmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 35, Name = "co1.wind.ne.jp", DisplayName = "?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 36, Name = "co2.wind.ne.jp", DisplayName = "?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 37, Name = "co3.wind.ne.jp", DisplayName = "?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 38, Name = "cocoa.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 39, Name = "coda.plala.or.jp", DisplayName = "???", DisplayShortName = "Arcor", Documentation = null },
                new MailboxProvider { Id = 40, Name = "comcast.net", DisplayName = "???", DisplayShortName = "Comcast", Documentation = null },
                new MailboxProvider { Id = 41, Name = "concerto.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 42, Name = "coral.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 43, Name = "courante.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 44, Name = "cpost.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 45, Name = "cream.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 46, Name = "dan.wind.ne.jp", DisplayName = "???", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 47, Name = "dance.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 48, Name = "dd.iij4u.or.jp", DisplayName = "IIJ4U", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 49, Name = "df.eu", DisplayName = "domainFACTORY", DisplayShortName = "domainFACTORY", Documentation = "http://www.df.eu/de/service/df-faq/e-mail/mail-programme/" },
                new MailboxProvider { Id = 50, Name = "dmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 51, Name = "earthlink.net", DisplayName = "EarthLink", DisplayShortName = "EarthLink", Documentation = "http://support.earthlink.net/email/email-server-settings.php" },
                new MailboxProvider { Id = 52, Name = "ebony.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 53, Name = "email.it", DisplayName = "email.it", DisplayShortName = "email.it", Documentation = "http://www.email.it/ita/config/thunder.php" },
                new MailboxProvider { Id = 54, Name = "email.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 55, Name = "ewetel.de", DisplayName = "EWE Tel", DisplayShortName = "EWE Tel", Documentation = null },
                new MailboxProvider { Id = 56, Name = "fantasy.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 57, Name = "flamenco.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 58, Name = "fmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 59, Name = "francetelecom.fr", DisplayName = "France Telecom / Orange", DisplayShortName = "Orange", Documentation = null },
                new MailboxProvider { Id = 60, Name = "free.fr", DisplayName = "Free Telecom", DisplayShortName = "free.fr", Documentation = "http://www.free.fr/assistance/599-thunderbird.html" },
                new MailboxProvider { Id = 61, Name = "freenet.de", DisplayName = "Freenet Mail", DisplayShortName = "Freenet", Documentation = null },
                new MailboxProvider { Id = 62, Name = "fuga.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 63, Name = "gandi.net", DisplayName = "Gandi Mail", DisplayShortName = "Gandi", Documentation = null },
                new MailboxProvider { Id = 64, Name = "gmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 65, Name = "gmx.com", DisplayName = "GMX Freemail", DisplayShortName = "GMX", Documentation = null },
                new MailboxProvider { Id = 66, Name = "gmx.net", DisplayName = "GMX Freemail", DisplayShortName = "GMX", Documentation = null },
                new MailboxProvider { Id = 67, Name = "go.tvm.ne.jp", DisplayName = "????????????????????", DisplayShortName = "TVM-Net", Documentation = null },
                new MailboxProvider { Id = 68, Name = "goo.jp", DisplayName = "goo ????????", DisplayShortName = "goo", Documentation = null },
                new MailboxProvider { Id = 69, Name = "googlemail.com", DisplayName = "Google Mail", DisplayShortName = "GMail", Documentation = null },
                new MailboxProvider { Id = 70, Name = "grape.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 71, Name = "gray.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 72, Name = "hal.ne.jp", DisplayName = "?????????", DisplayShortName = "HAL", Documentation = null },
                new MailboxProvider { Id = 73, Name = "hana.or.jp", DisplayName = "????????", DisplayShortName = "????", Documentation = null },
                new MailboxProvider { Id = 74, Name = "hotmail.com", DisplayName = "Microsoft Live Hotmail", DisplayShortName = "Hotmail", Documentation = null },
                new MailboxProvider { Id = 75, Name = "i.softbank.jp", DisplayName = "SoftBank", DisplayShortName = "SoftBank", Documentation = null },
                new MailboxProvider { Id = 76, Name = "ic-net.or.jp", DisplayName = "IC-NET", DisplayShortName = "IC-NET", Documentation = null },
                new MailboxProvider { Id = 77, Name = "iijmio-mail.jp", DisplayName = "IIJmio ????????", DisplayShortName = "IIJmio", Documentation = null },
                new MailboxProvider { Id = 78, Name = "iiyama-catv.ne.jp", DisplayName = "???????i?????", DisplayShortName = "i?????", Documentation = null },
                new MailboxProvider { Id = 79, Name = "imail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 80, Name = "inbox.lt", DisplayName = "Inbox.lt", DisplayShortName = "Inbox.lt", Documentation = null },
                new MailboxProvider { Id = 81, Name = "inbox.lv", DisplayName = "Inbox.lv", DisplayShortName = "Inbox.lv", Documentation = null },
                new MailboxProvider { Id = 82, Name = "indigo.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 83, Name = "inet-shibata.or.jp", DisplayName = "??????????", DisplayShortName = "INET-SHIBATA", Documentation = null },
                new MailboxProvider { Id = 84, Name = "internode.on.net", DisplayName = "Internode", DisplayShortName = "Internode", Documentation = "http://www.internode.on.net/support/guides/email/secure_email/" },
                new MailboxProvider { Id = 85, Name = "ipax.at", DisplayName = "IPAX Internet Services", DisplayShortName = "IPAX", Documentation = null },
                new MailboxProvider { Id = 86, Name = "ivory.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 87, Name = "iwafune.ne.jp", DisplayName = "???????????", DisplayShortName = "IWAFUNE", Documentation = null },
                new MailboxProvider { Id = 88, Name = "jade.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 89, Name = "janis.or.jp", DisplayName = "Janis", DisplayShortName = "Janis", Documentation = null },
                new MailboxProvider { Id = 90, Name = "jet.ne.jp", DisplayName = "JETINTERNET", DisplayShortName = "JET", Documentation = null },
                new MailboxProvider { Id = 91, Name = "ji.jet.ne.jp", DisplayName = "JETINTERNET", DisplayShortName = "JET", Documentation = null },
                new MailboxProvider { Id = 92, Name = "jmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 93, Name = "kabelmail.de", DisplayName = "Kabel Deutschland", DisplayShortName = "Kabel D", Documentation = null },
                new MailboxProvider { Id = 94, Name = "kelcom.net", DisplayName = "KELCOM Internet", DisplayShortName = "KELCOM", Documentation = null },
                new MailboxProvider { Id = 95, Name = "khaki.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 96, Name = "kl.wind.ne.jp", DisplayName = "?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 97, Name = "kmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 98, Name = "kokuyou.ne.jp", DisplayName = "????????????", DisplayShortName = "?????", Documentation = null },
                new MailboxProvider { Id = 99, Name = "lapis.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 100, Name = "laposte.net", DisplayName = "LaPoste", DisplayShortName = "LaPoste", Documentation = "http://www.geckozone.org/forum/viewtopic.php?f=4&t=93118" },
                new MailboxProvider { Id = 101, Name = "lemon.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 102, Name = "libero.it", DisplayName = "Libero Mail", DisplayShortName = "Libero", Documentation = "http://aiuto.libero.it/mail/istruzioni/configura-mozilla-thunderbird-per-windows-a11.phtml" },
                new MailboxProvider { Id = 103, Name = "lilac.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 104, Name = "lime.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 105, Name = "mahoroba.ne.jp", DisplayName = "???????????", DisplayShortName = "????", Documentation = null },
                new MailboxProvider { Id = 106, Name = "mail.com", DisplayName = "mail.com", DisplayShortName = "mail.com", Documentation = null },
                new MailboxProvider { Id = 107, Name = "mail.dk", DisplayName = "TDC (DK)", DisplayShortName = "TDC", Documentation = null },
                new MailboxProvider { Id = 108, Name = "mail.iwafune.ne.jp", DisplayName = "???????????", DisplayShortName = "IWAFUNE", Documentation = null },
                new MailboxProvider { Id = 109, Name = "mail.ru", DisplayName = "mail.ru", DisplayShortName = "mail.ru", Documentation = null },
                new MailboxProvider { Id = 110, Name = "mail.telenor.dk", DisplayName = "Telenor Danmark", DisplayShortName = "Telenor", Documentation = null },
                new MailboxProvider { Id = 111, Name = "mail.wind.ne.jp", DisplayName = "?????????", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 112, Name = "maroon.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 113, Name = "me.com", DisplayName = "Apple iCloud", DisplayShortName = "Apple", Documentation = null },
                new MailboxProvider { Id = 114, Name = "minuet.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 115, Name = "ml.murakami.ne.jp", DisplayName = "???????????", DisplayShortName = "IWAFUNE", Documentation = null },
                new MailboxProvider { Id = 116, Name = "mnet.ne.jp", DisplayName = "Mnet ??? ????", DisplayShortName = "Mnet???", Documentation = null },
                new MailboxProvider { Id = 117, Name = "'mopera.net", DisplayName = "mopera U", DisplayShortName = "mopera U", Documentation = null },
                new MailboxProvider { Id = 118, Name = "mozilla.com", DisplayName = "Mozilla Corporation and Mozilla Foundation internal email addresses", DisplayShortName = "mozilla.com", Documentation = null },
                new MailboxProvider { Id = 119, Name = "mx1.tiki.ne.jp", DisplayName = "TikiTiki???????", DisplayShortName = "TikiTiki", Documentation = null },
                new MailboxProvider { Id = 120, Name = "navy.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 121, Name = "nctsoft.com", DisplayName = "nctsoft", DisplayShortName = "nct", Documentation = null },
                new MailboxProvider { Id = 122, Name = "nifty.com", DisplayName = "@nifty", DisplayShortName = "@nifty", Documentation = null },
                new MailboxProvider { Id = 123, Name = "nsat.jp", DisplayName = "BB????", DisplayShortName = "NSAT", Documentation = null },
                new MailboxProvider { Id = 124, Name = "o2.pl", DisplayName = "o2 Poczta", DisplayShortName = "o2", Documentation = null },
                new MailboxProvider { Id = 125, Name = "olive.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 126, Name = "onet.pl", DisplayName = "Poczta Onet", DisplayShortName = "Onet", Documentation = null },
                new MailboxProvider { Id = 127, Name = "opal.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 128, Name = "orange.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 129, Name = "orchid.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 130, Name = "ovh.net", DisplayName = "OVH", DisplayShortName = "OVH", Documentation = "http://guides.ovh.com/ConfigurationEmail" },
                new MailboxProvider { Id = 131, Name = "pal.kijimadaira.jp", DisplayName = "????FTTH", DisplayShortName = "????FTTH", Documentation = null },
                new MailboxProvider { Id = 132, Name = "palette.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 133, Name = "parabox.or.jp", DisplayName = "??????", DisplayShortName = "PARABOX", Documentation = null },
                new MailboxProvider { Id = 134, Name = "pdx.edu", DisplayName = "Portland State University Mail", DisplayShortName = "PSU Mail", Documentation = null },
                new MailboxProvider { Id = 135, Name = "peach.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 136, Name = "peoplepc.com", DisplayName = "PeoplePC", DisplayShortName = "PeoplePC", Documentation = null },
                new MailboxProvider { Id = 137, Name = "plum.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 138, Name = "po.dcn.ne.jp", DisplayName = "DCN???????????", DisplayShortName = "DCN", Documentation = null },
                new MailboxProvider { Id = 139, Name = "po.wind.ne.jp", DisplayName = "'?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 140, Name = "polka.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 141, Name = "pop.shibata.ne.jp", DisplayName = "??????????", DisplayShortName = "INET-SHIBATA", Documentation = null },
                new MailboxProvider { Id = 142, Name = "posteo.de", DisplayName = "Posteo", DisplayShortName = "Posteo", Documentation = null },
                new MailboxProvider { Id = 143, Name = "purple.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 144, Name = "qip.ru", DisplayName = "qip.ru", DisplayShortName = "qip.ru", Documentation = null },
                new MailboxProvider { Id = 145, Name = "rainbow.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 146, Name = "rambler.ru", DisplayName = "Rambler Mail", DisplayShortName = "Rambler", Documentation = null },
                new MailboxProvider { Id = 147, Name = "red.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 148, Name = "rmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 149, Name = "rondo.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 150, Name = "rose.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 151, Name = "rouge.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 152, Name = "rr.com", DisplayName = "RoadRunner", DisplayShortName = "RR", Documentation = "http://help.rr.com/HMSFaqs/e_emailserveraddys.aspx" },
                new MailboxProvider { Id = 153, Name = "ruby.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 154, Name = "sakunet.ne.jp", DisplayName = "?????????", DisplayShortName = "Saku-Net", Documentation = null },
                new MailboxProvider { Id = 155, Name = "sea.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 156, Name = "sepia.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 157, Name = "serenade.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 158, Name = "seznam.cz", DisplayName = "Seznam", DisplayShortName = "Seznam", Documentation = "http://napoveda.seznam.cz/cz/jake-jsou-adresy-pop3-a-smtp-serveru.html" },
                new MailboxProvider { Id = 159, Name = "sfr.fr", DisplayName = "SFR / Neuf", DisplayShortName = "SFR", Documentation = "http://assistance.sfr.fr/internet_neufbox-de-SFR/utiliser-email/parametrer-id-sfr/fc-505-50680" },
                new MailboxProvider { Id = 160, Name = "silk.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 161, Name = "silver.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 162, Name = "sky.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 163, Name = "skynet.be", DisplayName = "skynet", DisplayShortName = "skynet", Documentation = "http://support.en.belgacom.be/app/answers/detail/a_id/14337/kw/thunderbird" },
                new MailboxProvider { Id = 164, Name = "smail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 165, Name = "snow.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 166, Name = "so.wind.ne.jp", DisplayName = "?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 167, Name = "sonata.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 168, Name = "strato.de", DisplayName = "Strato", DisplayShortName = "Strato", Documentation = null },
                new MailboxProvider { Id = 169, Name = "studenti.univr.it", DisplayName = "Universita degli Studi di Verona", DisplayShortName = "UniVR", Documentation = null },
                new MailboxProvider { Id = 170, Name = "suite.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 171, Name = "sympatico.ca", DisplayName = "Sympatico Email", DisplayShortName = "Sympatico", Documentation = "http://internet.bell.ca/index.cfm?method=content.view&category_id=585&content_id=12767" },
                new MailboxProvider { Id = 172, Name = "symphony.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 173, Name = "t-online.de", DisplayName = "T-Online", DisplayShortName = "T-Online", Documentation = null },
                new MailboxProvider { Id = 174, Name = "taupe.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 175, Name = "terra.es", DisplayName = "Correo Terra", DisplayShortName = "Terra", Documentation = null },
                new MailboxProvider { Id = 176, Name = "tiki.ne.jp", DisplayName = "TikiTiki???????", DisplayShortName = "TikiTiki", Documentation = null },
                new MailboxProvider { Id = 177, Name = "tiscali.cz", DisplayName = "Tiscali", DisplayShortName = "Tiscali", Documentation = null },
                new MailboxProvider { Id = 178, Name = "tiscali.it", DisplayName = "Tiscali Italy", DisplayShortName = "Tiscali", Documentation = "http://assistenza.tiscali.it/tecnica/posta/configurazioni/" },
                new MailboxProvider { Id = 179, Name = "tmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 180, Name = "toccata.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 181, Name = "topaz.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 182, Name = "trio.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 183, Name = "umail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 184, Name = "umich.edu", DisplayName = "UM ITCS Email", DisplayShortName = "UM ITCS", Documentation = null },
                new MailboxProvider { Id = 185, Name = "upcmail.nl", DisplayName = "UPC Nederland", DisplayShortName = "UPC", Documentation = null },
                new MailboxProvider { Id = 186, Name = "verizon.net", DisplayName = "Verizon Online", DisplayShortName = "Verizon", Documentation = null },
                new MailboxProvider { Id = 187, Name = "versatel.de", DisplayName = "Versatel", DisplayShortName = "Versatel", Documentation = "http://www.versatel.de/hilfe/index_popup.php?einrichtung_email_programm" },
                new MailboxProvider { Id = 188, Name = "violet.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 189, Name = "vm.aikis.or.jp", DisplayName = "aikis", DisplayShortName = "aikis", Documentation = null },
                new MailboxProvider { Id = 190, Name = "vmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 191, Name = "vp.tiki.ne.jp", DisplayName = "TikiTiki???????", DisplayShortName = "TikiTiki", Documentation = null },
                new MailboxProvider { Id = 192, Name = "waltz.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 193, Name = "wave.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 194, Name = "web.de", DisplayName = "WEB.DE Freemail", DisplayShortName = "Web.de", Documentation = "http://hilfe.freemail.web.de/freemail/e-mail/pop3/thunderbird/" },
                new MailboxProvider { Id = 195, Name = "white.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 196, Name = "wildwestdomains.com", DisplayName = "Your WildWest domain", DisplayShortName = "WildWest", Documentation = null },
                new MailboxProvider { Id = 197, Name = "wine.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 198, Name = "wmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 199, Name = "wp.pl", DisplayName = "Poczta Wirtualna Polska", DisplayShortName = "Poczta WP", Documentation = null },
                new MailboxProvider { Id = 200, Name = "xmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 201, Name = "xp.wind.jp", DisplayName = "?????????", DisplayShortName = "wind", Documentation = null },
                new MailboxProvider { Id = 202, Name = "xpost.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 203, Name = "xs4all.nl", DisplayName = "XS4All", DisplayShortName = "XS4All", Documentation = null },
                new MailboxProvider { Id = 204, Name = "xtra.co.nz", DisplayName = "Yahoo! Mail", DisplayShortName = "Yahoo", Documentation = null },
                new MailboxProvider { Id = 205, Name = "yahoo.co.jp", DisplayName = "Yahoo! ???", DisplayShortName = "Yahoo! ??? ", Documentation = null },
                new MailboxProvider { Id = 206, Name = "yahoo.com", DisplayName = "Yahoo! Mail", DisplayShortName = "Yahoo", Documentation = null },
                new MailboxProvider { Id = 207, Name = "yandex.ru", DisplayName = "Yandex Mail", DisplayShortName = "Yandex", Documentation = null },
                new MailboxProvider { Id = 208, Name = "ybb.ne.jp", DisplayName = "Yahoo! BB", DisplayShortName = "Yahoo! BB", Documentation = null },
                new MailboxProvider { Id = 209, Name = "yellow.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 210, Name = "ymail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 211, Name = "ypost.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 212, Name = "ziggo.nl", DisplayName = "Ziggo", DisplayShortName = "Ziggo", Documentation = null },
                new MailboxProvider { Id = 213, Name = "zmail.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 214, Name = "zpost.plala.or.jp", DisplayName = "???", DisplayShortName = "???", Documentation = null },
                new MailboxProvider { Id = 215, Name = "avsmedia.com", DisplayName = "avsmedia.com", DisplayShortName = "avsmedia", Documentation = null },
                new MailboxProvider { Id = 216, Name = "avsmedia.net", DisplayName = "avsmedia.net", DisplayShortName = "avsmedia", Documentation = null },
                new MailboxProvider { Id = 218, Name = "ilearney.com", DisplayName = "ilearney.com", DisplayShortName = "ilearney.com", Documentation = null },
                new MailboxProvider { Id = 219, Name = "fpl -technology.com", DisplayName = "fpl-technology.com", DisplayShortName = "fpl-technology.com", Documentation = "http://fpl-technology.com" },
                new MailboxProvider { Id = 220, Name = "icloud.com", DisplayName = "Apple iCloud", DisplayShortName = "Apple", Documentation = null },
                new MailboxProvider { Id = 221, Name = "office365.com", DisplayName = "Microsoft Office 365", DisplayShortName = "Office365", Documentation = "https://products.office.com" });

            return modelBuilder;
        }

        public static void MySqlAddMailboxProvider(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailboxProvider>(entity =>
            {
                entity.ToTable("mail_mailbox_provider");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DisplayName)
                    .HasColumnName("display_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DisplayShortName)
                    .HasColumnName("display_short_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Documentation)
                    .HasColumnName("documentation")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddMailboxProvider(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailboxProvider>(entity =>
            {
                entity.ToTable("mail_mailbox_provider", "onlyoffice");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DisplayName)
                    .HasColumnName("display_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.DisplayShortName)
                    .HasColumnName("display_short_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Documentation)
                    .HasColumnName("documentation")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });
        }
    }
}
