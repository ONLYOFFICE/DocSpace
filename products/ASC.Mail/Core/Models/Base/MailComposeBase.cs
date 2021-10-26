/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System.Collections.Generic;
using System.Linq;

using ASC.Files.Core.Security;
using ASC.Mail.Enums;
using ASC.Mail.Exceptions;
using ASC.Mail.Utils;

namespace ASC.Mail.Models.Base
{
    public class MailComposeBase
    {
        public string CalendarEventUid { get; private set; }

        public string CalendarMethod { get; private set; }

        private readonly string _calendarIcs;

        public MailComposeBase(int id, MailBoxData mailBoxData, FolderType folder, string from, List<string> to, List<string> cc, List<string> bcc,
                         string subject, string mimeMessageId, string mimeReplyToId, bool important,
                         List<int> tags, string body, string streamId, List<MailAttachmentData> attachments, string calendarIcs = "")
        {
            Id = id;
            Mailbox = mailBoxData;
            Folder = folder;
            From = from;
            To = to ?? new List<string>();
            Cc = cc ?? new List<string>();
            Bcc = bcc ?? new List<string>();
            Subject = subject;
            MimeMessageId = mimeMessageId;
            MimeReplyToId = mimeReplyToId;
            Important = important;
            Labels = tags;
            HtmlBody = body ?? "";
            StreamId = streamId;

            var distinct = attachments == null ? new List<MailAttachmentData>() : attachments.Distinct().ToList();

            if (distinct.Sum(a => a.size) > DefineConstants.ATTACHMENTS_TOTAL_SIZE_LIMIT)
                throw new DraftException(DraftException.ErrorTypes.TotalSizeExceeded,
                    "Total size of all files exceeds limit!");

            Attachments = distinct;
            AttachmentsEmbedded = new List<MailAttachmentData>();

            if (string.IsNullOrEmpty(calendarIcs))
                return;

            _calendarIcs = calendarIcs;

            //TODO: Fix 
            var calendar = MailUtil.ParseValidCalendar(_calendarIcs);
            if (calendar != null)
            {
                CalendarMethod = calendar.Method;
                CalendarEventUid = calendar.Events[0].Uid;
            }
        }

        public int Id { get; set; }

        public MailBoxData Mailbox { get; set; }

        public FolderType Folder { get; set; }

        public List<string> To { get; set; }

        public List<string> Cc { get; set; }

        public List<string> Bcc { get; set; }

        public bool Important { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string HtmlBody { get; set; }

        public List<MailAttachmentData> Attachments { get; set; }

        public List<MailAttachmentData> AttachmentsEmbedded { get; set; }

        public string StreamId { get; set; }

        public int ReplyToId { get; set; }

        public List<int> Labels { get; set; }

        public string MimeMessageId { get; set; }

        public string MimeReplyToId { get; set; }

        public FileShare FileLinksShareMode { get; set; }

        public bool AccountChanged
        {
            get { return Mailbox.MailBoxId != PreviousMailboxId; }
        }

        public int PreviousMailboxId { get; set; }

        public string CalendarIcs
        {
            get { return _calendarIcs; }
        }


    }
}