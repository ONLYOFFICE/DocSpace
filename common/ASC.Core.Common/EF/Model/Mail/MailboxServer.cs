// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Core.Common.EF.Model.Mail;

public class MailboxServer
{
    public int Id { get; set; }
    public int IdProvider { get; set; }
    public string Type { get; set; }
    public string Hostname { get; set; }
    public int Port { get; set; }
    public string SocketType { get; set; }
    public string UserName { get; set; }
    public string Authentication { get; set; }
    public bool IsUserData { get; set; }
}

public static class MailboxServerExtension
{
    public static ModelBuilderWrapper AddMailboxServer(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddMailboxServer, Provider.MySql)
            .Add(PgSqlAddMailboxServer, Provider.PostgreSql)
            .HasData(
            new MailboxServer { Id = 493, IdProvider = 1, Type = "imap", Hostname = "imap.1und1.de", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 494, IdProvider = 1, Type = "imap", Hostname = "imap.1und1.de", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 495, IdProvider = 1, Type = "pop3", Hostname = "pop.1und1.de", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 496, IdProvider = 1, Type = "pop3", Hostname = "pop.1und1.de", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 497, IdProvider = 1, Type = "smtp", Hostname = "smtp.1und1.de", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 498, IdProvider = 2, Type = "pop3", Hostname = "abc.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 499, IdProvider = 2, Type = "smtp", Hostname = "abc.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 500, IdProvider = 3, Type = "pop3", Hostname = "agate.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 501, IdProvider = 3, Type = "smtp", Hostname = "agate.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 502, IdProvider = 4, Type = "imap", Hostname = "in.alice.it", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 503, IdProvider = 4, Type = "pop3", Hostname = "in.alice.it", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 504, IdProvider = 4, Type = "smtp", Hostname = "out.alice.it", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 505, IdProvider = 5, Type = "pop3", Hostname = "amail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 506, IdProvider = 5, Type = "smtp", Hostname = "amail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 507, IdProvider = 6, Type = "pop3", Hostname = "amber.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 508, IdProvider = 6, Type = "smtp", Hostname = "amber.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 509, IdProvider = 7, Type = "imap", Hostname = "imap.aol.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 510, IdProvider = 7, Type = "imap", Hostname = "imap.aol.com", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 511, IdProvider = 7, Type = "pop3", Hostname = "pop.aol.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 512, IdProvider = 7, Type = "pop3", Hostname = "pop.aol.com", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 513, IdProvider = 7, Type = "smtp", Hostname = "smtp.aol.com", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 514, IdProvider = 8, Type = "pop3", Hostname = "apost.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 515, IdProvider = 8, Type = "smtp", Hostname = "apost.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 516, IdProvider = 9, Type = "pop3", Hostname = "aqua.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 517, IdProvider = 9, Type = "smtp", Hostname = "aqua.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 518, IdProvider = 10, Type = "imap", Hostname = "imap.arcor.de", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 519, IdProvider = 10, Type = "imap", Hostname = "imap.arcor.de", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 520, IdProvider = 10, Type = "pop3", Hostname = "pop3.arcor.de", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 521, IdProvider = 10, Type = "smtp", Hostname = "mail.arcor.de", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 522, IdProvider = 11, Type = "imap", Hostname = "imaps.pec.aruba.it", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 523, IdProvider = 11, Type = "pop3", Hostname = "pop3s.pec.aruba.it", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 524, IdProvider = 11, Type = "smtp", Hostname = "smtps.pec.aruba.it", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 525, IdProvider = 12, Type = "pop3", Hostname = "pop.att.yahoo.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 526, IdProvider = 12, Type = "smtp", Hostname = "smtp.att.yahoo.com", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 527, IdProvider = 215, Type = "pop3", Hostname = "mail.avsmedia.com", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%@%EMAILHOSTNAME%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 528, IdProvider = 215, Type = "smtp", Hostname = "mail.avsmedia.com", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 529, IdProvider = 216, Type = "pop3", Hostname = "78.40.187.62", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 530, IdProvider = 216, Type = "smtp", Hostname = "78.40.187.62", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 531, IdProvider = 13, Type = "pop3", Hostname = "ballade.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 532, IdProvider = 13, Type = "smtp", Hostname = "ballade.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 533, IdProvider = 14, Type = "pop3", Hostname = "bay.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 534, IdProvider = 14, Type = "smtp", Hostname = "bay.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 535, IdProvider = 15, Type = "pop3", Hostname = "pop.bb-niigata.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 536, IdProvider = 15, Type = "smtp", Hostname = "pop.bb-niigata.jp", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 537, IdProvider = 16, Type = "pop3", Hostname = "beige.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 538, IdProvider = 16, Type = "smtp", Hostname = "beige.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 539, IdProvider = 17, Type = "pop3", Hostname = "mail.biglobe.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 540, IdProvider = 17, Type = "smtp", Hostname = "mail.biglobe.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 541, IdProvider = 18, Type = "pop3", Hostname = "mail.bigpond.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 542, IdProvider = 18, Type = "smtp", Hostname = "mail.bigpond.com", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 543, IdProvider = 19, Type = "pop3", Hostname = "blue.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 544, IdProvider = 19, Type = "smtp", Hostname = "blue.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 545, IdProvider = 20, Type = "imap", Hostname = "imaps.bluewin.ch", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 546, IdProvider = 20, Type = "pop3", Hostname = "pop3s.bluewin.ch", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 547, IdProvider = 20, Type = "smtp", Hostname = "smtpauths.bluewin.ch", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 548, IdProvider = 21, Type = "imap", Hostname = "imaps.bluewin.ch", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 549, IdProvider = 21, Type = "pop3", Hostname = "pop3s.bluewin.ch", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 550, IdProvider = 21, Type = "smtp", Hostname = "smtpauths.bluewin.ch", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 551, IdProvider = 22, Type = "pop3", Hostname = "bmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 552, IdProvider = 22, Type = "smtp", Hostname = "bmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 553, IdProvider = 23, Type = "pop3", Hostname = "bolero.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 554, IdProvider = 23, Type = "smtp", Hostname = "bolero.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 555, IdProvider = 24, Type = "pop3", Hostname = "bpost.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 556, IdProvider = 24, Type = "smtp", Hostname = "bpost.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 557, IdProvider = 25, Type = "pop3", Hostname = "mail.broba.cc", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 558, IdProvider = 25, Type = "smtp", Hostname = "mail.broba.cc", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 559, IdProvider = 26, Type = "pop3", Hostname = "brown.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 560, IdProvider = 26, Type = "smtp", Hostname = "brown.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 561, IdProvider = 27, Type = "pop3", Hostname = "camel.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 562, IdProvider = 27, Type = "smtp", Hostname = "camel.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 563, IdProvider = 28, Type = "pop3", Hostname = "cameo.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 564, IdProvider = 28, Type = "smtp", Hostname = "cameo.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 565, IdProvider = 29, Type = "pop3", Hostname = "pop.cc9.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 566, IdProvider = 29, Type = "smtp", Hostname = "smtp.cc9.ne.jp", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 567, IdProvider = 30, Type = "pop3", Hostname = "mail.cek.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 568, IdProvider = 30, Type = "smtp", Hostname = "smtp.cek.ne.jp", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 569, IdProvider = 31, Type = "imap", Hostname = "plato.cgl.ucsf.edu", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 570, IdProvider = 31, Type = "smtp", Hostname = "plato.cgl.ucsf.edu", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 571, IdProvider = 32, Type = "imap", Hostname = "mobile.charter.net", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 572, IdProvider = 32, Type = "imap", Hostname = "imap.charter.net", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 573, IdProvider = 32, Type = "smtp", Hostname = "mobile.charter.net", Port = 587, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 574, IdProvider = 33, Type = "pop3", Hostname = "mail.clio.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 575, IdProvider = 33, Type = "smtp", Hostname = "mail.clio.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 576, IdProvider = 34, Type = "pop3", Hostname = "cmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 577, IdProvider = 34, Type = "smtp", Hostname = "cmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 578, IdProvider = 35, Type = "pop3", Hostname = "co1.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 579, IdProvider = 35, Type = "smtp", Hostname = "co1.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 580, IdProvider = 36, Type = "pop3", Hostname = "co2.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 581, IdProvider = 36, Type = "smtp", Hostname = "co2.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 582, IdProvider = 37, Type = "pop3", Hostname = "co3.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 583, IdProvider = 37, Type = "smtp", Hostname = "co3.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 584, IdProvider = 38, Type = "pop3", Hostname = "cocoa.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 585, IdProvider = 38, Type = "smtp", Hostname = "cocoa.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 586, IdProvider = 39, Type = "pop3", Hostname = "coda.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 587, IdProvider = 39, Type = "smtp", Hostname = "coda.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 588, IdProvider = 40, Type = "pop3", Hostname = "mail.comcast.net", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 589, IdProvider = 40, Type = "smtp", Hostname = "smtp.comcast.net", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 590, IdProvider = 41, Type = "pop3", Hostname = "concerto.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 591, IdProvider = 41, Type = "smtp", Hostname = "concerto.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 592, IdProvider = 42, Type = "pop3", Hostname = "coral.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 593, IdProvider = 42, Type = "smtp", Hostname = "coral.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 594, IdProvider = 43, Type = "pop3", Hostname = "courante.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 595, IdProvider = 43, Type = "smtp", Hostname = "courante.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 596, IdProvider = 44, Type = "pop3", Hostname = "cpost.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 597, IdProvider = 44, Type = "smtp", Hostname = "cpost.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 598, IdProvider = 45, Type = "pop3", Hostname = "cream.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 599, IdProvider = 45, Type = "smtp", Hostname = "cream.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 600, IdProvider = 46, Type = "pop3", Hostname = "dan.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 601, IdProvider = 46, Type = "smtp", Hostname = "dan.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 602, IdProvider = 47, Type = "pop3", Hostname = "dance.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 603, IdProvider = 47, Type = "smtp", Hostname = "dance.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 604, IdProvider = 48, Type = "pop3", Hostname = "mbox.iij4u.or.jp", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 605, IdProvider = 48, Type = "smtp", Hostname = "mbox.iij4u.or.jp", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 606, IdProvider = 49, Type = "imap", Hostname = "sslmailpool.ispgateway.de", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 607, IdProvider = 49, Type = "pop3", Hostname = "sslmailpool.ispgateway.de", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 608, IdProvider = 49, Type = "smtp", Hostname = "smtprelaypool.ispgateway.de", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 609, IdProvider = 50, Type = "pop3", Hostname = "dmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 610, IdProvider = 50, Type = "smtp", Hostname = "dmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 611, IdProvider = 51, Type = "imap", Hostname = "imap.earthlink.net", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 612, IdProvider = 51, Type = "pop3", Hostname = "pop.earthlink.net", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 613, IdProvider = 51, Type = "smtp", Hostname = "smtpauth.earthlink.net", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 614, IdProvider = 52, Type = "pop3", Hostname = "ebony.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 615, IdProvider = 52, Type = "smtp", Hostname = "ebony.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 616, IdProvider = 53, Type = "imap", Hostname = "imapmail.email.it", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 617, IdProvider = 53, Type = "pop3", Hostname = "popmail.email.it", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 618, IdProvider = 53, Type = "smtp", Hostname = "smtp.email.it", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 619, IdProvider = 54, Type = "pop3", Hostname = "email.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 620, IdProvider = 54, Type = "smtp", Hostname = "email.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 621, IdProvider = 55, Type = "pop3", Hostname = "pop3s-1.ewetel.net", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 622, IdProvider = 55, Type = "smtp", Hostname = "smtps-1.ewetel.net", Port = 25, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 623, IdProvider = 56, Type = "pop3", Hostname = "fantasy.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 624, IdProvider = 56, Type = "smtp", Hostname = "fantasy.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 625, IdProvider = 57, Type = "pop3", Hostname = "flamenco.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 626, IdProvider = 57, Type = "smtp", Hostname = "flamenco.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 627, IdProvider = 58, Type = "pop3", Hostname = "fmail.mail.plala.or.jp", Port = 993, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 628, IdProvider = 58, Type = "smtp", Hostname = "fmail.mail.plala.or.jp", Port = 143, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 629, IdProvider = 59, Type = "pop3", Hostname = "pop.orange.fr", Port = 995, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 630, IdProvider = 59, Type = "imap", Hostname = "imap.orange.fr", Port = 143, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 631, IdProvider = 59, Type = "smtp", Hostname = "smtp.orange.fr", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 632, IdProvider = 60, Type = "imap", Hostname = "imap.free.fr", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 633, IdProvider = 60, Type = "pop3", Hostname = "pop.free.fr", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 634, IdProvider = 60, Type = "smtp", Hostname = "smtp.free.fr", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 635, IdProvider = 61, Type = "imap", Hostname = "mx.freenet.de", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 636, IdProvider = 61, Type = "imap", Hostname = "mx.freenet.de", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 637, IdProvider = 61, Type = "pop3", Hostname = "mx.freenet.de", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 638, IdProvider = 61, Type = "pop3", Hostname = "mx.freenet.de", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 639, IdProvider = 61, Type = "smtp", Hostname = "mx.freenet.de", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 640, IdProvider = 62, Type = "pop3", Hostname = "fuga.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 641, IdProvider = 62, Type = "smtp", Hostname = "fuga.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 642, IdProvider = 63, Type = "imap", Hostname = "mail.gandi.net", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 643, IdProvider = 63, Type = "imap", Hostname = "mail.gandi.net", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 644, IdProvider = 63, Type = "pop3", Hostname = "mail.gandi.net", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 645, IdProvider = 63, Type = "pop3", Hostname = "mail.gandi.net", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 646, IdProvider = 63, Type = "smtp", Hostname = "mail.gandi.net", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 647, IdProvider = 64, Type = "pop3", Hostname = "gmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 648, IdProvider = 64, Type = "smtp", Hostname = "gmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 649, IdProvider = 65, Type = "imap", Hostname = "imap.gmx.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 650, IdProvider = 65, Type = "imap", Hostname = "imap.gmx.com", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 651, IdProvider = 65, Type = "pop3", Hostname = "pop.gmx.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 652, IdProvider = 65, Type = "pop3", Hostname = "pop.gmx.com", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 653, IdProvider = 65, Type = "smtp", Hostname = "mail.gmx.com", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 654, IdProvider = 66, Type = "imap", Hostname = "imap.gmx.net", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 655, IdProvider = 66, Type = "imap", Hostname = "imap.gmx.net", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 656, IdProvider = 66, Type = "pop3", Hostname = "pop.gmx.net", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 657, IdProvider = 66, Type = "pop3", Hostname = "pop.gmx.net", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 658, IdProvider = 66, Type = "smtp", Hostname = "mail.gmx.net", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 659, IdProvider = 67, Type = "pop3", Hostname = "go.tvm.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 660, IdProvider = 67, Type = "smtp", Hostname = "go.tvm.ne.jp", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 661, IdProvider = 68, Type = "pop3", Hostname = "pop.mail.goo.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 662, IdProvider = 68, Type = "smtp", Hostname = "smtp.mail.goo.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 663, IdProvider = 69, Type = "imap", Hostname = "imap.googlemail.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 664, IdProvider = 69, Type = "pop3", Hostname = "pop.googlemail.com", Port = 995, SocketType = "SSL", UserName = "recent:%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 665, IdProvider = 69, Type = "smtp", Hostname = "smtp.googlemail.com", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 666, IdProvider = 70, Type = "pop3", Hostname = "grape.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 667, IdProvider = 70, Type = "smtp", Hostname = "grape.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 668, IdProvider = 71, Type = "pop3", Hostname = "gray.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 669, IdProvider = 71, Type = "smtp", Hostname = "gray.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 670, IdProvider = 72, Type = "pop3", Hostname = "mail.hal.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 671, IdProvider = 72, Type = "smtp", Hostname = "mail.hal.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 672, IdProvider = 73, Type = "pop3", Hostname = "mail.hana.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 673, IdProvider = 73, Type = "smtp", Hostname = "mail.hana.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 674, IdProvider = 74, Type = "pop3", Hostname = "pop-mail.outlook.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 675, IdProvider = 74, Type = "smtp", Hostname = "smtp-mail.outlook.com", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 676, IdProvider = 75, Type = "imap", Hostname = "imap.softbank.jp", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 677, IdProvider = 75, Type = "smtp", Hostname = "smtp.softbank.jp", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 678, IdProvider = 76, Type = "pop3", Hostname = "mail.ic-net.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 679, IdProvider = 76, Type = "smtp", Hostname = "smtp.ic-net.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 680, IdProvider = 77, Type = "pop3", Hostname = "mbox.iijmio-mail.jp", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%.%EMAILDOMAIN%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 681, IdProvider = 77, Type = "smtp", Hostname = "mbox.iijmio-mail.jp", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%.%EMAILDOMAIN%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 682, IdProvider = 78, Type = "pop3", Hostname = "mail.iiyama-catv.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 683, IdProvider = 78, Type = "smtp", Hostname = "smtp.iiyama-catv.ne.jp", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 684, IdProvider = 79, Type = "pop3", Hostname = "imail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 685, IdProvider = 79, Type = "smtp", Hostname = "imail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 686, IdProvider = 80, Type = "pop3", Hostname = "mail.inbox.lt", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 687, IdProvider = 80, Type = "smtp", Hostname = "mail.inbox.lt", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 688, IdProvider = 81, Type = "pop3", Hostname = "mail.inbox.lv", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 689, IdProvider = 81, Type = "smtp", Hostname = "mail.inbox.lv", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 690, IdProvider = 82, Type = "pop3", Hostname = "indigo.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 691, IdProvider = 82, Type = "smtp", Hostname = "indigo.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 692, IdProvider = 83, Type = "pop3", Hostname = "po.inet-shibata.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 693, IdProvider = 83, Type = "smtp", Hostname = "po.inet-shibata.or.jp", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 694, IdProvider = 84, Type = "imap", Hostname = "mail.internode.on.net", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 695, IdProvider = 84, Type = "pop3", Hostname = "mail.internode.on.net", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 696, IdProvider = 84, Type = "smtp", Hostname = "mail.internode.on.net", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 697, IdProvider = 85, Type = "imap", Hostname = "mail.ipax.at", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 698, IdProvider = 85, Type = "smtp", Hostname = "mail.ipax.at", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 699, IdProvider = 86, Type = "pop3", Hostname = "ivory.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 700, IdProvider = 86, Type = "smtp", Hostname = "ivory.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 701, IdProvider = 87, Type = "pop3", Hostname = "po.iwafune.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 702, IdProvider = 87, Type = "smtp", Hostname = "po.iwafune.ne.jp", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 703, IdProvider = 88, Type = "pop3", Hostname = "jade.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 704, IdProvider = 88, Type = "smtp", Hostname = "jade.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 705, IdProvider = 89, Type = "pop3", Hostname = "mail.%EMAILDOMAIN%", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 706, IdProvider = 89, Type = "smtp", Hostname = "smtp.%EMAILDOMAIN%", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 707, IdProvider = 90, Type = "pop3", Hostname = "pop.jet.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 708, IdProvider = 90, Type = "imap", Hostname = "imap.jet.ne.jp", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 709, IdProvider = 90, Type = "smtp", Hostname = "smtp.jet.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 710, IdProvider = 91, Type = "pop3", Hostname = "pop02.jet.ne.jp", Port = 110, SocketType = "plain", UserName = "%%EMAILLOCALPART%%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 711, IdProvider = 91, Type = "smtp", Hostname = "smtp02.jet.ne.jp", Port = 587, SocketType = "plain", UserName = "%%EMAILLOCALPART%%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 712, IdProvider = 92, Type = "pop3", Hostname = "jmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 713, IdProvider = 92, Type = "smtp", Hostname = "jmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 714, IdProvider = 93, Type = "pop3", Hostname = "pop3.kabelmail.de", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 715, IdProvider = 93, Type = "smtp", Hostname = "smtp.kabelmail.de", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 716, IdProvider = 94, Type = "pop3", Hostname = "pop1.kelcom.net", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 717, IdProvider = 94, Type = "smtp", Hostname = "smtp.kelcom.net", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 718, IdProvider = 95, Type = "pop3", Hostname = "khaki.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 719, IdProvider = 95, Type = "smtp", Hostname = "khaki.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 720, IdProvider = 96, Type = "pop3", Hostname = "kl.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 721, IdProvider = 96, Type = "smtp", Hostname = "kl.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 722, IdProvider = 97, Type = "pop3", Hostname = "kmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 723, IdProvider = 97, Type = "smtp", Hostname = "kmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 724, IdProvider = 98, Type = "pop3", Hostname = "mail.kokuyou.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 725, IdProvider = 98, Type = "smtp", Hostname = "smtp.kokuyou.ne.jp", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 726, IdProvider = 99, Type = "pop3", Hostname = "lapis.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 727, IdProvider = 99, Type = "smtp", Hostname = "lapis.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 728, IdProvider = 100, Type = "imap", Hostname = "imap.laposte.net", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 729, IdProvider = 100, Type = "pop3", Hostname = "pop.laposte.net", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 730, IdProvider = 100, Type = "smtp", Hostname = "smtp.laposte.net", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 731, IdProvider = 101, Type = "pop3", Hostname = "lemon.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 732, IdProvider = 101, Type = "smtp", Hostname = "lemon.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 733, IdProvider = 102, Type = "imap", Hostname = "imapmail.libero.it", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 734, IdProvider = 102, Type = "pop3", Hostname = "popmail.libero.it", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 735, IdProvider = 102, Type = "smtp", Hostname = "smtp.libero.it", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 736, IdProvider = 103, Type = "pop3", Hostname = "lilac.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 737, IdProvider = 103, Type = "smtp", Hostname = "lilac.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 738, IdProvider = 104, Type = "pop3", Hostname = "lime.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 739, IdProvider = 104, Type = "smtp", Hostname = "lime.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 740, IdProvider = 105, Type = "pop3", Hostname = "mail.mahoroba.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 741, IdProvider = 105, Type = "smtp", Hostname = "mail.mahoroba.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 742, IdProvider = 106, Type = "imap", Hostname = "imap.mail.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 743, IdProvider = 106, Type = "imap", Hostname = "imap.mail.com", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 744, IdProvider = 106, Type = "pop3", Hostname = "pop.mail.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 745, IdProvider = 106, Type = "pop3", Hostname = "pop.mail.com", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 746, IdProvider = 106, Type = "smtp", Hostname = "smtp.mail.com", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 747, IdProvider = 107, Type = "pop3", Hostname = "pop3.mail.dk", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 748, IdProvider = 107, Type = "smtp", Hostname = "asmtp.mail.dk", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 749, IdProvider = 108, Type = "pop3", Hostname = "mail.iwafune.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 750, IdProvider = 108, Type = "smtp", Hostname = "mail.iwafune.ne.jp", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 751, IdProvider = 109, Type = "pop3", Hostname = "pop.mail.ru", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 752, IdProvider = 109, Type = "imap", Hostname = "imap.mail.ru", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 753, IdProvider = 109, Type = "smtp", Hostname = "smtp.mail.ru", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 754, IdProvider = 110, Type = "imap", Hostname = "mail.telenor.dk", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 755, IdProvider = 110, Type = "pop3", Hostname = "mail.telenor.dk", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 756, IdProvider = 110, Type = "smtp", Hostname = "mail.telenor.dk", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 757, IdProvider = 111, Type = "pop3", Hostname = "mail.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 758, IdProvider = 111, Type = "smtp", Hostname = "mail.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 759, IdProvider = 112, Type = "pop3", Hostname = "maroon.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 760, IdProvider = 112, Type = "smtp", Hostname = "maroon.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 761, IdProvider = 113, Type = "imap", Hostname = "imap.mail.me.com", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 762, IdProvider = 113, Type = "smtp", Hostname = "smtp.mail.me.com", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 763, IdProvider = 114, Type = "pop3", Hostname = "minuet.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 764, IdProvider = 114, Type = "smtp", Hostname = "minuet.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 765, IdProvider = 115, Type = "pop3", Hostname = "ml.murakami.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 766, IdProvider = 115, Type = "smtp", Hostname = "ml.murakami.ne.jp", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 767, IdProvider = 116, Type = "pop3", Hostname = "mail.mnet.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 768, IdProvider = 116, Type = "smtp", Hostname = "mail.mnet.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 769, IdProvider = 117, Type = "imap", Hostname = "mail.mopera.net", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 770, IdProvider = 117, Type = "imap", Hostname = "mail.mopera.net", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 771, IdProvider = 117, Type = "pop3", Hostname = "mail.mopera.net", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 772, IdProvider = 117, Type = "pop3", Hostname = "mail.mopera.net", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 773, IdProvider = 117, Type = "smtp", Hostname = "mail.mopera.net", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 774, IdProvider = 118, Type = "imap", Hostname = "mail.mozilla.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 775, IdProvider = 118, Type = "smtp", Hostname = "smtp.mozilla.org", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 776, IdProvider = 119, Type = "pop3", Hostname = "%EMAILDOMAIN%", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 777, IdProvider = 119, Type = "smtp", Hostname = "smtp-auth.tiki.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 778, IdProvider = 120, Type = "pop3", Hostname = "navy.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 779, IdProvider = 120, Type = "smtp", Hostname = "navy.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 780, IdProvider = 121, Type = "pop3", Hostname = "mail.nctsoft.com", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%@%EMAILHOSTNAME%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 781, IdProvider = 121, Type = "smtp", Hostname = "mail.nctsoft.com", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 782, IdProvider = 122, Type = "pop3", Hostname = "pop.nifty.com", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 783, IdProvider = 122, Type = "smtp", Hostname = "smtp.nifty.com", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 784, IdProvider = 123, Type = "pop3", Hostname = "mail.nsat.jp", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 785, IdProvider = 123, Type = "smtp", Hostname = "mail.nsat.jp", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 786, IdProvider = 124, Type = "pop3", Hostname = "poczta.o2.pl", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 787, IdProvider = 124, Type = "smtp", Hostname = "poczta.o2.pl", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 788, IdProvider = 125, Type = "pop3", Hostname = "olive.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 789, IdProvider = 125, Type = "smtp", Hostname = "olive.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 790, IdProvider = 126, Type = "pop3", Hostname = "pop3.poczta.onet.pl", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 791, IdProvider = 126, Type = "smtp", Hostname = "pop3.poczta.onet.pl", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 792, IdProvider = 127, Type = "pop3", Hostname = "opal.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 793, IdProvider = 127, Type = "smtp", Hostname = "opal.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 794, IdProvider = 128, Type = "pop3", Hostname = "orange.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 795, IdProvider = 128, Type = "smtp", Hostname = "orange.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 796, IdProvider = 129, Type = "pop3", Hostname = "orchid.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 797, IdProvider = 129, Type = "smtp", Hostname = "orchid.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 798, IdProvider = 130, Type = "imap", Hostname = "ssl0.ovh.net", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 799, IdProvider = 130, Type = "pop3", Hostname = "ssl0.ovh.net", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 800, IdProvider = 130, Type = "smtp", Hostname = "ssl0.ovh.net", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 801, IdProvider = 131, Type = "pop3", Hostname = "mail.pal.kijimadaira.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 802, IdProvider = 131, Type = "smtp", Hostname = "smtp.pal.kijimadaira.jp", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 803, IdProvider = 132, Type = "pop3", Hostname = "palette.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 804, IdProvider = 132, Type = "smtp", Hostname = "palette.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 805, IdProvider = 133, Type = "pop3", Hostname = "pop3.parabox.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 806, IdProvider = 134, Type = "smtp", Hostname = "smtp.parabox.or.jp", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 807, IdProvider = 134, Type = "imap", Hostname = "psumail.pdx.edu", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 808, IdProvider = 134, Type = "imap", Hostname = "psumail.pdx.edu", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 809, IdProvider = 134, Type = "smtp", Hostname = "mailhost.pdx.edu", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 810, IdProvider = 135, Type = "pop3", Hostname = "peach.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 811, IdProvider = 135, Type = "smtp", Hostname = "peach.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 812, IdProvider = 136, Type = "imap", Hostname = "imap.peoplepc.com", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 813, IdProvider = 136, Type = "pop3", Hostname = "pop.peoplepc.com", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 814, IdProvider = 136, Type = "smtp", Hostname = "smtpauth.peoplepc.com", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 815, IdProvider = 137, Type = "pop3", Hostname = "plum.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 816, IdProvider = 137, Type = "smtp", Hostname = "plum.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 817, IdProvider = 138, Type = "pop3", Hostname = "po.dcn.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 818, IdProvider = 138, Type = "smtp", Hostname = "po.dcn.ne.jp", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 819, IdProvider = 139, Type = "pop3", Hostname = "po.wind.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 820, IdProvider = 139, Type = "smtp", Hostname = "po.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 821, IdProvider = 140, Type = "pop3", Hostname = "polka.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 822, IdProvider = 140, Type = "smtp", Hostname = "polka.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 823, IdProvider = 141, Type = "pop3", Hostname = "%EMAILDOMAIN%", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 824, IdProvider = 141, Type = "smtp", Hostname = "%EMAILDOMAIN%", Port = 25, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 825, IdProvider = 142, Type = "imap", Hostname = "posteo.de", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 826, IdProvider = 142, Type = "smtp", Hostname = "posteo.de", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 827, IdProvider = 143, Type = "pop3", Hostname = "purple.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 828, IdProvider = 143, Type = "smtp", Hostname = "purple.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 829, IdProvider = 144, Type = "pop3", Hostname = "pop.qip.ru", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 830, IdProvider = 144, Type = "imap", Hostname = "imap.qip.ru", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 831, IdProvider = 144, Type = "smtp", Hostname = "smtp.qip.ru", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 832, IdProvider = 145, Type = "pop3", Hostname = "rainbow.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 833, IdProvider = 145, Type = "smtp", Hostname = "rainbow.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 834, IdProvider = 146, Type = "imap", Hostname = "imap.rambler.ru", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 835, IdProvider = 146, Type = "pop3", Hostname = "pop.rambler.ru", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 836, IdProvider = 146, Type = "smtp", Hostname = "smtp.rambler.ru", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 837, IdProvider = 147, Type = "pop3", Hostname = "red.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 838, IdProvider = 147, Type = "smtp", Hostname = "red.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 839, IdProvider = 148, Type = "pop3", Hostname = "rmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 840, IdProvider = 148, Type = "smtp", Hostname = "rmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 841, IdProvider = 149, Type = "pop3", Hostname = "rondo.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 842, IdProvider = 149, Type = "smtp", Hostname = "rondo.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 843, IdProvider = 150, Type = "pop3", Hostname = "rose.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 844, IdProvider = 150, Type = "smtp", Hostname = "rose.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 845, IdProvider = 151, Type = "pop3", Hostname = "rouge.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 846, IdProvider = 151, Type = "smtp", Hostname = "rouge.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 847, IdProvider = 152, Type = "pop3", Hostname = "pop-server.%EMAILDOMAIN%", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 848, IdProvider = 152, Type = "smtp", Hostname = "smtp-server.%EMAILDOMAIN%", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 849, IdProvider = 153, Type = "pop3", Hostname = "ruby.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 850, IdProvider = 153, Type = "smtp", Hostname = "ruby.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 851, IdProvider = 154, Type = "pop3", Hostname = "mail.sakunet.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 852, IdProvider = 154, Type = "smtp", Hostname = "smtp.sakunet.ne.jp", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 853, IdProvider = 155, Type = "pop3", Hostname = "sea.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 854, IdProvider = 155, Type = "smtp", Hostname = "sea.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 855, IdProvider = 156, Type = "pop3", Hostname = "sepia.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 856, IdProvider = 156, Type = "smtp", Hostname = "sepia.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 857, IdProvider = 157, Type = "pop3", Hostname = "serenade.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 858, IdProvider = 157, Type = "smtp", Hostname = "serenade.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 859, IdProvider = 158, Type = "pop3", Hostname = "pop3.seznam.cz", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 860, IdProvider = 158, Type = "smtp", Hostname = "smtp.seznam.cz", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 861, IdProvider = 159, Type = "imap", Hostname = "imap.sfr.fr", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 862, IdProvider = 159, Type = "pop3", Hostname = "pop.sfr.fr", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 863, IdProvider = 159, Type = "smtp", Hostname = "smtp.sfr.fr", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 864, IdProvider = 160, Type = "pop3", Hostname = "silk.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 865, IdProvider = 160, Type = "smtp", Hostname = "silk.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 866, IdProvider = 161, Type = "pop3", Hostname = "silver.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 867, IdProvider = 161, Type = "smtp", Hostname = "silver.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 868, IdProvider = 162, Type = "pop3", Hostname = "sky.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 869, IdProvider = 162, Type = "smtp", Hostname = "sky.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 870, IdProvider = 163, Type = "imap", Hostname = "imap.skynet.be", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 871, IdProvider = 163, Type = "pop3", Hostname = "pop.skynet.be", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 872, IdProvider = 163, Type = "smtp", Hostname = "relay.skynet.be", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 873, IdProvider = 164, Type = "pop3", Hostname = "smail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 874, IdProvider = 164, Type = "smtp", Hostname = "smail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 875, IdProvider = 165, Type = "pop3", Hostname = "snow.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 876, IdProvider = 165, Type = "smtp", Hostname = "snow.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 877, IdProvider = 166, Type = "imap", Hostname = "so.wind.ne.jp", Port = 143, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 878, IdProvider = 166, Type = "smtp", Hostname = "so.wind.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 879, IdProvider = 167, Type = "pop3", Hostname = "sonata.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 880, IdProvider = 167, Type = "smtp", Hostname = "sonata.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 881, IdProvider = 168, Type = "imap", Hostname = "imap.strato.de", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 882, IdProvider = 168, Type = "pop3", Hostname = "pop3.strato.de", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 883, IdProvider = 168, Type = "smtp", Hostname = "smtp.strato.de", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-encrypted", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 884, IdProvider = 169, Type = "pop3", Hostname = "mail.studenti.univr.it", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 885, IdProvider = 169, Type = "smtp", Hostname = "mail.studenti.univr.it", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 886, IdProvider = 170, Type = "pop3", Hostname = "suite.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 887, IdProvider = 170, Type = "smtp", Hostname = "suite.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 888, IdProvider = 171, Type = "pop3", Hostname = "pophm.sympatico.ca", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 889, IdProvider = 171, Type = "smtp", Hostname = "smtphm.sympatico.ca", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 890, IdProvider = 172, Type = "pop3", Hostname = "symphony.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 891, IdProvider = 172, Type = "smtp", Hostname = "symphony.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 892, IdProvider = 173, Type = "imap", Hostname = "secureimap.t-online.de", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 893, IdProvider = 173, Type = "pop3", Hostname = "securepop.t-online.de", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 894, IdProvider = 173, Type = "smtp", Hostname = "securesmtp.t-online.de", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 895, IdProvider = 174, Type = "pop3", Hostname = "taupe.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 896, IdProvider = 174, Type = "smtp", Hostname = "taupe.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 897, IdProvider = 175, Type = "imap", Hostname = "imap4.terra.es", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 898, IdProvider = 175, Type = "pop3", Hostname = "pop3.terra.es", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 899, IdProvider = 175, Type = "smtp", Hostname = "mailhost.terra.es", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 900, IdProvider = 176, Type = "pop3", Hostname = "mx.tiki.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 901, IdProvider = 176, Type = "smtp", Hostname = "smtp-auth.tiki.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 902, IdProvider = 177, Type = "pop3", Hostname = "pop3.mail.tiscali.cz", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 903, IdProvider = 177, Type = "smtp", Hostname = "smtp.mail.tiscali.cz", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 904, IdProvider = 178, Type = "imap", Hostname = "imap.tiscali.it", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 905, IdProvider = 178, Type = "pop3", Hostname = "pop.tiscali.it", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 906, IdProvider = 178, Type = "smtp", Hostname = "smtp.tiscali.it", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 907, IdProvider = 179, Type = "pop3", Hostname = "tmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 908, IdProvider = 179, Type = "smtp", Hostname = "tmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 909, IdProvider = 180, Type = "pop3", Hostname = "toccata.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 910, IdProvider = 180, Type = "smtp", Hostname = "toccata.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 911, IdProvider = 181, Type = "pop3", Hostname = "topaz.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 912, IdProvider = 181, Type = "smtp", Hostname = "topaz.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 913, IdProvider = 182, Type = "pop3", Hostname = "trio.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 914, IdProvider = 182, Type = "smtp", Hostname = "trio.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 915, IdProvider = 183, Type = "pop3", Hostname = "umail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 916, IdProvider = 183, Type = "smtp", Hostname = "umail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 917, IdProvider = 184, Type = "imap", Hostname = "mail.umich.edu", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 918, IdProvider = 184, Type = "smtp", Hostname = "smtp.mail.umich.edu", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 919, IdProvider = 185, Type = "pop3", Hostname = "pop3.upcmail.nl", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 920, IdProvider = 185, Type = "smtp", Hostname = "smtp.upcmail.nl", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 921, IdProvider = 186, Type = "pop3", Hostname = "incoming.verizon.net", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 922, IdProvider = 186, Type = "smtp", Hostname = "outgoing.verizon.net", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 923, IdProvider = 187, Type = "imap", Hostname = "mx.versatel.de", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 924, IdProvider = 187, Type = "pop3", Hostname = "mx.versatel.de", Port = 143, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 925, IdProvider = 187, Type = "smtp", Hostname = "smtp.versatel.de", Port = 25, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 926, IdProvider = 188, Type = "pop3", Hostname = "violet.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 927, IdProvider = 188, Type = "smtp", Hostname = "violet.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 928, IdProvider = 189, Type = "pop3", Hostname = "mail.aikis.or.jp", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 929, IdProvider = 189, Type = "smtp", Hostname = "mail.aikis.or.jp", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 930, IdProvider = 190, Type = "pop3", Hostname = "vmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 931, IdProvider = 190, Type = "smtp", Hostname = "vmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 932, IdProvider = 191, Type = "pop3", Hostname = "vp.tiki.ne.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 933, IdProvider = 191, Type = "smtp", Hostname = "vs.tiki.ne.jp", Port = 587, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 934, IdProvider = 192, Type = "pop3", Hostname = "waltz.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 935, IdProvider = 192, Type = "smtp", Hostname = "waltz.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 936, IdProvider = 193, Type = "pop3", Hostname = "wave.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 937, IdProvider = 193, Type = "smtp", Hostname = "wave.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 938, IdProvider = 194, Type = "imap", Hostname = "imap.web.de", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 939, IdProvider = 194, Type = "imap", Hostname = "imap.web.de", Port = 143, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 940, IdProvider = 194, Type = "pop3", Hostname = "pop3.web.de", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 941, IdProvider = 194, Type = "pop3", Hostname = "pop3.web.de", Port = 110, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 942, IdProvider = 194, Type = "smtp", Hostname = "smtp.web.de", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 943, IdProvider = 195, Type = "pop3", Hostname = "white.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 944, IdProvider = 195, Type = "smtp", Hostname = "white.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 945, IdProvider = 196, Type = "pop3", Hostname = "pop.secureserver.net", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 946, IdProvider = 196, Type = "imap", Hostname = "imap.secureserver.net", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 947, IdProvider = 196, Type = "smtp", Hostname = "smtpout.secureserver.net", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 948, IdProvider = 197, Type = "pop3", Hostname = "wine.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 949, IdProvider = 197, Type = "smtp", Hostname = "wine.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 950, IdProvider = 198, Type = "pop3", Hostname = "wmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 951, IdProvider = 198, Type = "smtp", Hostname = "wmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 952, IdProvider = 199, Type = "pop3", Hostname = "pop3.wp.pl", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 953, IdProvider = 199, Type = "smtp", Hostname = "smtp.wp.pl", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 954, IdProvider = 200, Type = "pop3", Hostname = "xmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 955, IdProvider = 200, Type = "smtp", Hostname = "xmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 956, IdProvider = 201, Type = "pop3", Hostname = "xp.wind.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 957, IdProvider = 201, Type = "smtp", Hostname = "xp.wind.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 958, IdProvider = 202, Type = "pop3", Hostname = "xpost.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 959, IdProvider = 202, Type = "smtp", Hostname = "xpost.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 960, IdProvider = 203, Type = "pop3", Hostname = "pops.xs4all.nl", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 961, IdProvider = 203, Type = "smtp", Hostname = "smtps.xs4all.nl", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 962, IdProvider = 204, Type = "pop3", Hostname = "pop3.xtra.co.nz", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 963, IdProvider = 204, Type = "smtp", Hostname = "send.xtra.co.nz", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 964, IdProvider = 204, Type = "pop3", Hostname = "pop.mail.yahoo.co.jp", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 965, IdProvider = 205, Type = "smtp", Hostname = "smtp.mail.yahoo.co.jp", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 966, IdProvider = 206, Type = "pop3", Hostname = "pop.mail.yahoo.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 967, IdProvider = 206, Type = "imap", Hostname = "imap.mail.yahoo.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 968, IdProvider = 206, Type = "smtp", Hostname = "smtp.mail.yahoo.com", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 969, IdProvider = 207, Type = "imap", Hostname = "imap.yandex.ru", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 970, IdProvider = 207, Type = "pop3", Hostname = "pop.yandex.ru", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 971, IdProvider = 207, Type = "smtp", Hostname = "smtp.yandex.ru", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 972, IdProvider = 208, Type = "pop3", Hostname = "ybbpop.mail.yahoo.co.jp", Port = 995, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 973, IdProvider = 208, Type = "smtp", Hostname = "ybbsmtp.mail.yahoo.co.jp", Port = 465, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 974, IdProvider = 209, Type = "pop3", Hostname = "yellow.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 975, IdProvider = 209, Type = "smtp", Hostname = "yellow.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 976, IdProvider = 210, Type = "pop3", Hostname = "ymail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 977, IdProvider = 210, Type = "smtp", Hostname = "ymail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 978, IdProvider = 211, Type = "pop3", Hostname = "ypost.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 979, IdProvider = 211, Type = "smtp", Hostname = "ypost.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 980, IdProvider = 212, Type = "pop3", Hostname = "pop.ziggo.nl", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 981, IdProvider = 212, Type = "smtp", Hostname = "smtp.ziggo.nl", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 982, IdProvider = 213, Type = "pop3", Hostname = "zmail.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 983, IdProvider = 213, Type = "smtp", Hostname = "zmail.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 984, IdProvider = 214, Type = "pop3", Hostname = "zpost.mail.plala.or.jp", Port = 110, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 985, IdProvider = 214, Type = "smtp", Hostname = "zpost.mail.plala.or.jp", Port = 587, SocketType = "plain", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 988, IdProvider = 218, Type = "pop3", Hostname = "mail.ilearney.com", Port = 110, SocketType = "plain", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 989, IdProvider = 218, Type = "smtp", Hostname = "mail.ilearney.com", Port = 25, SocketType = "plain", UserName = "", Authentication = "none", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 990, IdProvider = 219, Type = "pop3", Hostname = "pop3s.aruba.it", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 991, IdProvider = 219, Type = "imap", Hostname = "imaps.aruba.it", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 992, IdProvider = 219, Type = "smtp", Hostname = "smtps.aruba.it", Port = 465, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "password-cleartext", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 993, IdProvider = 74, Type = "imap", Hostname = "imap-mail.outlook.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 994, IdProvider = 220, Type = "imap", Hostname = "imap.mail.me.com", Port = 993, SocketType = "SSL", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 995, IdProvider = 220, Type = "smtp", Hostname = "smtp.mail.me.com", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILLOCALPART%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 996, IdProvider = 221, Type = "pop3", Hostname = "outlook.office365.com", Port = 995, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 997, IdProvider = 221, Type = "imap", Hostname = "outlook.office365.com", Port = 993, SocketType = "SSL", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") },
            new MailboxServer { Id = 998, IdProvider = 221, Type = "smtp", Hostname = "smtp.office365.com", Port = 587, SocketType = "STARTTLS", UserName = "%EMAILADDRESS%", Authentication = "", IsUserData = bool.Parse("false") });

        return modelBuilder;
    }

    public static void MySqlAddMailboxServer(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MailboxServer>(entity =>
        {
            entity.ToTable("mail_mailbox_server");

            entity.HasIndex(e => e.IdProvider)
                .HasDatabaseName("id_provider");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Authentication)
                .HasColumnName("authentication")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Hostname)
                .IsRequired()
                .HasColumnName("hostname")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.IdProvider).HasColumnName("id_provider");

            entity.Property(e => e.IsUserData).HasColumnName("is_user_data");

            entity.Property(e => e.Port).HasColumnName("port");

            entity.Property(e => e.SocketType)
                .IsRequired()
                .HasColumnName("socket_type")
                .HasColumnType("enum('plain','SSL','STARTTLS')")
                .HasDefaultValueSql("'plain'")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Type)
                .IsRequired()
                .HasColumnName("type")
                .HasColumnType("enum('pop3','imap','smtp')")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.UserName)
                .HasColumnName("username")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddMailboxServer(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("onlyoffice", "enum_mail_mailbox_server", new[] { "pop3", "imap", "smtp" });

        modelBuilder.Entity<MailboxServer>(entity =>
        {
            entity.ToTable("mail_mailbox_server", "onlyoffice");

            entity.HasIndex(e => e.IdProvider)
                .HasDatabaseName("id_provider_mail_mailbox_server");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Authentication)
                .HasColumnName("authentication")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Hostname)
                .IsRequired()
                .HasColumnName("hostname")
                .HasColumnType("character varying");

            entity.Property(e => e.IdProvider).HasColumnName("id_provider");

            entity.Property(e => e.IsUserData)
                .HasColumnName("is_user_data")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Port).HasColumnName("port");

            entity.Property(e => e.SocketType)
                .IsRequired()
                .HasColumnName("socket_type")
                .HasColumnType("character varying")
                .HasDefaultValueSql("'plain'");

            entity.Property(e => e.UserName)
                .HasColumnName("username")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");
        });
    }
}
