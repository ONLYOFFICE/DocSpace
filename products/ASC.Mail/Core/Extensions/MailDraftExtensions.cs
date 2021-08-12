/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using ASC.Common.Logging;
using ASC.Common.Web;
using ASC.Files.Core;
using ASC.Mail.Exceptions;
using ASC.Mail.Models;
using ASC.Mail.Models.Base;
using ASC.Mail.Storage;
using ASC.Mail.Utils;
using ASC.Web.Files.Services.WCFService;

using HtmlAgilityPack;

using MimeKit;

namespace ASC.Mail.Extensions
{
    public static class MailDraftExtensions
    {
        public static MailMessageData ToMailMessage(this MailComposeBase draft)
        {
            MailboxAddress fromVerified;

            if (string.IsNullOrEmpty(draft.From))
                throw new DraftException(DraftException.ErrorTypes.EmptyField, "Empty email address in {0} field",
                    DraftFieldTypes.From);

            if (!MailboxAddress.TryParse(ParserOptions.Default, draft.From, out fromVerified))
                throw new DraftException(DraftException.ErrorTypes.IncorrectField, "Incorrect email address",
                    DraftFieldTypes.From);

            if (string.IsNullOrEmpty(fromVerified.Name))
                fromVerified.Name = draft.Mailbox.Name;

            if (string.IsNullOrEmpty(draft.MimeMessageId))
                throw new ArgumentException("MimeMessageId");

            var messageItem = new MailMessageData
            {
                From = fromVerified.ToString(),
                FromEmail = fromVerified.Address,
                To = string.Join(", ", draft.To.ToArray()),
                Cc = draft.Cc != null ? string.Join(", ", draft.Cc.ToArray()) : "",
                Bcc = draft.Bcc != null ? string.Join(", ", draft.Bcc.ToArray()) : "",
                Subject = draft.Subject,
                Date = DateTime.UtcNow,
                Important = draft.Important,
                HtmlBody = draft.HtmlBody,
                Introduction = MailUtil.GetIntroduction(draft.HtmlBody),
                StreamId = draft.StreamId,
                TagIds = draft.Labels != null && draft.Labels.Count != 0 ? new List<int>(draft.Labels) : null,
                Size = draft.HtmlBody.Length,
                MimeReplyToId = draft.MimeReplyToId,
                MimeMessageId = draft.MimeMessageId,
                IsNew = false,
                Folder = draft.Folder,
                ChainId = draft.MimeMessageId,
                CalendarUid = draft.CalendarEventUid,
                CalendarEventIcs = draft.CalendarIcs,
                MailboxId = draft.Mailbox.MailBoxId
            };

            if (messageItem.Attachments == null)
            {
                messageItem.Attachments = new List<MailAttachmentData>();
            }

            draft.Attachments.ForEach(attachment =>
            {
                attachment.tenant = draft.Mailbox.TenantId;
                attachment.user = draft.Mailbox.UserId;
            });

            messageItem.Attachments.AddRange(draft.Attachments);

            messageItem.HasAttachments = messageItem.Attachments.Count > 0;

            return messageItem;
        }

        public static MimeMessage ToMimeMessage(this MailDraftData draft, StorageManager storageManager)
        {
            var mimeMessage = new MimeMessage
            {
                Date = DateTime.UtcNow,
                Subject = !string.IsNullOrEmpty(draft.Subject) ? draft.Subject : "",
                MessageId = draft.MimeMessageId
            };

            var from = MailboxAddress.Parse(ParserOptions.Default, draft.From);

            mimeMessage.From.Add(from);

            if (draft.To.Any())
                mimeMessage.To.AddRange(draft.To.ConvertAll(MailboxAddress.Parse));

            if (draft.Cc.Any())
                mimeMessage.Cc.AddRange(draft.Cc.ConvertAll(MailboxAddress.Parse));

            if (draft.Bcc.Any())
                mimeMessage.Bcc.AddRange(draft.Bcc.ConvertAll(MailboxAddress.Parse));

            if (draft.Important)
                mimeMessage.Importance = MessageImportance.High;

            if (!string.IsNullOrEmpty(draft.MimeReplyToId))
                mimeMessage.InReplyTo = draft.MimeReplyToId;

            mimeMessage.Body = ToMimeMessageBody(draft, storageManager);

            if (draft.IsAutogenerated)
            {
                mimeMessage.Headers.Add("Auto-Submitted", "auto-generated");
            }

            if (draft.IsAutoreplied)
            {
                mimeMessage.Headers.Add("Auto-Submitted", "auto-replied");
            }

            if (draft.RequestReceipt)
            {
                mimeMessage.Headers[HeaderId.ReturnReceiptTo] = from.ToString(true);
            }

            if (draft.RequestRead)
            {
                mimeMessage.Headers[HeaderId.DispositionNotificationTo] = from.ToString(true);
            }

            return mimeMessage;
        }

        private static MimePart ConvertToMimePart(MailAttachmentData mailAttachmentData, StorageManager storageManager, string contentId = null)
        {
            var contentType = ContentType.Parse(
                !string.IsNullOrEmpty(mailAttachmentData.contentType)
                    ? mailAttachmentData.contentType
                    : MimeMapping.GetMimeMapping(mailAttachmentData.fileName));

            var mimePart = new MimePart(contentType)
            {
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = mailAttachmentData.fileName
            };

            if (string.IsNullOrEmpty(contentId))
            {
                mimePart.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
            }
            else
            {
                mimePart.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
                mimePart.ContentId = contentId;
                mimePart.ContentType.Name = mailAttachmentData.fileName;
            }

            MemoryStream ms;

            if (mailAttachmentData.data == null)
            {
                var s3Key = MailStoragePathCombiner.GerStoredFilePath(mailAttachmentData);

                ms = new MemoryStream();

                using (var stream = storageManager
                    .GetDataStoreForAttachments(mailAttachmentData.tenant)
                    .GetReadStream(s3Key))
                {
                    stream.CopyTo(ms);
                }
            }
            else
            {
                ms = new MemoryStream(mailAttachmentData.data);
            }

            mimePart.Content = new MimeContent(ms);

            Parameter param;

            if (mimePart.ContentDisposition != null && mimePart.ContentDisposition.Parameters.TryGetValue("filename", out param))
                param.EncodingMethod = ParameterEncodingMethod.Rfc2047;

            if (mimePart.ContentType.Parameters.TryGetValue("name", out param))
                param.EncodingMethod = ParameterEncodingMethod.Rfc2047;

            return mimePart;
        }

        private static MimeEntity ToMimeMessageBody(MailDraftData draft, StorageManager storageManager)
        {
            string textBody;
            MailUtil.TryExtractTextFromHtml(draft.HtmlBody, out textBody);

            MultipartAlternative alternative = null;
            MimeEntity body = null;

            if (!string.IsNullOrEmpty(textBody))
            {
                var textPart = new TextPart("plain")
                {
                    Text = textBody,
                    ContentTransferEncoding = ContentEncoding.QuotedPrintable
                };

                if (!string.IsNullOrEmpty(draft.HtmlBody))
                {
                    alternative = new MultipartAlternative { textPart };
                    body = alternative;
                }
                else
                    body = textPart;
            }

            if (!string.IsNullOrEmpty(draft.HtmlBody))
            {
                var htmlPart = new TextPart("html")
                {
                    Text = draft.HtmlBody,
                    ContentTransferEncoding = ContentEncoding.QuotedPrintable
                };

                MimeEntity html;

                if (draft.AttachmentsEmbedded.Any())
                {
                    htmlPart.ContentTransferEncoding = ContentEncoding.Base64;

                    var related = new MultipartRelated
                    {
                        Root = htmlPart
                    };

                    related.Root.ContentId = null;

                    foreach (var emb in draft.AttachmentsEmbedded)
                    {
                        var linkedResource = ConvertToMimePart(emb, storageManager, emb.contentId);
                        related.Add(linkedResource);
                    }

                    html = related;
                }
                else
                    html = htmlPart;

                if (alternative != null)
                    alternative.Add(html);
                else
                    body = html;
            }

            if (!string.IsNullOrEmpty(draft.CalendarIcs))
            {
                var calendarPart = new TextPart("calendar")
                {
                    Text = draft.CalendarIcs,
                    ContentTransferEncoding = ContentEncoding.QuotedPrintable
                };

                calendarPart.ContentType.Parameters.Add("method", draft.CalendarMethod);

                if (alternative != null)
                    alternative.Add(calendarPart);
                else
                    body = calendarPart;
            }


            if (draft.Attachments.Any() || !string.IsNullOrEmpty(draft.CalendarIcs))
            {
                var mixed = new Multipart("mixed");

                if (body != null)
                    mixed.Add(body);

                foreach (var att in draft.Attachments)
                {
                    var attachment = ConvertToMimePart(att, storageManager);
                    mixed.Add(attachment);
                }

                if (!string.IsNullOrEmpty(draft.CalendarIcs))
                {
                    var filename = "calendar.ics";
                    switch (draft.CalendarMethod)
                    {
                        case DefineConstants.ICAL_REQUEST:
                            filename = "invite.ics";
                            break;
                        case DefineConstants.ICAL_REPLY:
                            filename = "reply.ics";
                            break;
                        case DefineConstants.ICAL_CANCEL:
                            filename = "cancel.ics";
                            break;
                    }

                    var contentType = new ContentType("application", "ics");
                    contentType.Parameters.Add("method", draft.CalendarMethod);
                    contentType.Parameters.Add("name", filename);

                    var calendarResource = new MimePart(contentType)
                    {
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = filename
                    };

                    var data = Encoding.UTF8.GetBytes(draft.CalendarIcs);

                    var ms = new MemoryStream(data);

                    calendarResource.Content = new MimeContent(ms);

                    mixed.Add(calendarResource);
                }

                body = mixed;
            }

            if (body != null)
                return body;

            return new TextPart("plain")
            {
                Text = string.Empty
            };
        }

        public static void ChangeAttachedFileLinksAddresses(this MailDraftData draft, FileStorageService<string> fileStorageService, ILog log = null)
        {
            if (log == null)
                log = new NullLog();

            var doc = new HtmlDocument();
            doc.LoadHtml(draft.HtmlBody);

            var linkNodes = doc.DocumentNode.SelectNodes("//a[contains(@class,'mailmessage-filelink-link')]");
            if (linkNodes == null) return;

            var setLinks = new List<Tuple<string, string>>();
            foreach (var linkNode in linkNodes)
            {
                var fileId = linkNode.Attributes["data-fileid"].Value;
                var objectId = "file_" + fileId;

                linkNode.Attributes["class"].Remove(); // 'mailmessage-filelink-link'
                linkNode.Attributes["data-fileid"].Remove(); // 'data-fileid'

                var setLink = setLinks.SingleOrDefault(x => x.Item1 == fileId);
                if (setLink != null)
                {
                    linkNode.SetAttributeValue("href", setLink.Item2);
                    log.InfoFormat("ChangeAttachedFileLinks() Change file link href: {0}", fileId);
                    continue;
                }

                var aceCollection = new AceCollection<string>
                {
                    Files = new List<string> { objectId },
                    Aces = new List<AceWrapper>
                            {
                                new AceWrapper
                                    {
                                        SubjectId = FileConstant.ShareLinkId,
                                        SubjectGroup = true,
                                        Share = draft.FileLinksShareMode
                                    }
                            }
                };

                fileStorageService.SetAceObject(aceCollection, false);
                log.InfoFormat("ChangeAttachedFileLinks() Set public accees to file: {0}", fileId);
                var sharedInfo =
                    fileStorageService.GetSharedInfo(new List<string> { objectId }, new List<string> { })
                                      .Find(r => r.SubjectId == FileConstant.ShareLinkId);
                linkNode.SetAttributeValue("href", sharedInfo.Link);
                log.InfoFormat("ChangeAttachedFileLinks() Change file link href: {0}", fileId);
                setLinks.Add(new Tuple<string, string>(fileId, sharedInfo.Link));
            }

            linkNodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'mailmessage-filelink')]");
            foreach (var linkNode in linkNodes)
            {
                linkNode.Attributes["class"].Remove();
            }

            draft.HtmlBody = doc.DocumentNode.OuterHtml;
        }

        public static List<string> GetEmbeddedAttachmentLinks(this MailComposeBase draft, StorageManager storageManager)
        {
            var links = new List<string>();

            var fckStorage = storageManager.GetDataStoreForCkImages(draft.Mailbox.TenantId);
            //todo: replace selector
            var currentMailFckeditorUrl = fckStorage.GetUri(StorageManager.CKEDITOR_IMAGES_DOMAIN, "").ToString();
            var currentMailAttachmentFolderUrl = MailStoragePathCombiner.GetMessageDirectory(draft.Mailbox.UserId,
                draft.StreamId);
            var currentUserStorageUrl = MailStoragePathCombiner.GetUserMailsDirectory(draft.Mailbox.UserId);
            var xpathQuery = StorageManager.GetXpathQueryForAttachmentsToResaving(currentMailFckeditorUrl,
                currentMailAttachmentFolderUrl,
                currentUserStorageUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(draft.HtmlBody);

            var linkNodes = doc.DocumentNode.SelectNodes(xpathQuery);

            if (linkNodes == null)
                return links;

            links.AddRange(linkNodes.Select(linkNode => linkNode.Attributes["src"].Value));

            return links;
        }

        public static void ChangeEmbeddedAttachmentLinks(this MailDraftData draft, ILog log = null)
        {
            if (log == null)
                log = new NullLog();

            var baseAttachmentFolder = MailStoragePathCombiner.GetMessageDirectory(draft.Mailbox.UserId, draft.StreamId);

            var doc = new HtmlDocument();
            doc.LoadHtml(draft.HtmlBody);
            var linkNodes = doc.DocumentNode.SelectNodes("//img[@src and (contains(@src,'" + baseAttachmentFolder + "'))]");
            if (linkNodes == null) return;

            foreach (var linkNode in linkNodes)
            {
                var link = linkNode.Attributes["src"].Value;
                log.InfoFormat("ChangeEmbededAttachmentLinks() Embeded attachment link for changing to cid: {0}", link);
                var fileLink = HttpUtility.UrlDecode(link.Substring(baseAttachmentFolder.Length));
                var fileName = Path.GetFileName(fileLink);

                var attach = CreateEmbbededAttachment(fileName, link, fileLink, draft.Mailbox.UserId, draft.Mailbox.TenantId, draft.Mailbox.MailBoxId, draft.StreamId);
                draft.AttachmentsEmbedded.Add(attach);
                linkNode.SetAttributeValue("src", "cid:" + attach.contentId);
                log.InfoFormat("ChangeEmbededAttachmentLinks() Attachment cid: {0}", attach.contentId);
            }
            draft.HtmlBody = doc.DocumentNode.OuterHtml;
        }

        public static void ChangeSmileLinks(this MailDraftData draft, ILog log = null)
        {
            if (log == null)
                log = new NullLog();

            var baseSmileUrl = MailStoragePathCombiner.GetEditorSmileBaseUrl();

            var doc = new HtmlDocument();
            doc.LoadHtml(draft.HtmlBody);
            var linkNodes = doc.DocumentNode.SelectNodes("//img[@src and (contains(@src,'" + baseSmileUrl + "'))]");
            if (linkNodes == null) return;

            foreach (var linkNode in linkNodes)
            {
                var link = linkNode.Attributes["src"].Value;

                log.InfoFormat("ChangeSmileLinks() Link to smile: {0}", link);

                var fileName = Path.GetFileName(link);

                var data = StorageManager.LoadLinkData(link, log);

                if (!data.Any())
                    continue;

                var attach = new MailAttachmentData
                {
                    fileName = fileName,
                    storedName = fileName,
                    contentId = link.GetMd5(),
                    data = data
                };

                log.InfoFormat("ChangeSmileLinks() Embedded smile contentId: {0}", attach.contentId);

                linkNode.SetAttributeValue("src", "cid:" + attach.contentId);

                if (draft.AttachmentsEmbedded.All(x => x.contentId != attach.contentId))
                {
                    draft.AttachmentsEmbedded.Add(attach);
                }
            }
            draft.HtmlBody = doc.DocumentNode.OuterHtml;
        }

        public static void ChangeUrlProxyLinks(this MailDraftData draft, ILog log = null)
        {
            if (log == null)
                log = new NullLog();

            try
            {
                draft.HtmlBody = HtmlSanitizer.RemoveProxyHttpUrls(draft.HtmlBody);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("ChangeUrlProxyLinks(): Exception: {0}", ex.ToString());
            }
        }

        public static void ChangeAttachedFileLinksImages(this MailDraftData draft, ILog log = null)
        {
            if (log == null)
                log = new NullLog();

            var baseSmileUrl = MailStoragePathCombiner.GetEditorImagesBaseUrl();

            var doc = new HtmlDocument();
            doc.LoadHtml(draft.HtmlBody);
            var linkNodes = doc.DocumentNode.SelectNodes("//img[@src and (contains(@src,'" + baseSmileUrl + "'))]");
            if (linkNodes == null) return;

            foreach (var linkNode in linkNodes)
            {
                var link = linkNode.Attributes["src"].Value;
                log.InfoFormat("ChangeAttachedFileLinksImages() Link to file link: {0}", link);

                var fileName = Path.GetFileName(link);

                var data = StorageManager.LoadLinkData(link, log);

                if (!data.Any())
                    continue;

                var attach = new MailAttachmentData
                {
                    fileName = fileName,
                    storedName = fileName,
                    contentId = link.GetMd5(),
                    data = data
                };

                log.InfoFormat("ChangeAttachedFileLinksImages() Embedded file link contentId: {0}", attach.contentId);
                linkNode.SetAttributeValue("src", "cid:" + attach.contentId);

                if (draft.AttachmentsEmbedded.All(x => x.contentId != attach.contentId))
                {
                    draft.AttachmentsEmbedded.Add(attach);
                }
            }

            draft.HtmlBody = doc.DocumentNode.OuterHtml;
        }

        public static void ChangeAllImagesLinksToEmbedded(this MailDraftData draft, ILog log = null)
        {
            if (log == null)
                log = new NullLog();

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(draft.HtmlBody);
                var linkNodes = doc.DocumentNode.SelectNodes("//img[@src]");
                if (linkNodes == null) return;

                foreach (var linkNode in linkNodes)
                {
                    var link = linkNode.Attributes["src"].Value;
                    log.InfoFormat("ChangeAllImagesLinksToEmbedded() Link to img link: {0}", link);

                    var fileName = Path.GetFileName(link);

                    var data = StorageManager.LoadLinkData(link, log);

                    if (!data.Any())
                        continue;

                    var attach = new MailAttachmentData
                    {
                        fileName = fileName,
                        storedName = fileName,
                        contentId = link.GetMd5(),
                        data = data
                    };

                    log.InfoFormat("ChangeAllImagesLinksToEmbedded() Embedded img link contentId: {0}", attach.contentId);
                    linkNode.SetAttributeValue("src", "cid:" + attach.contentId);

                    if (draft.AttachmentsEmbedded.All(x => x.contentId != attach.contentId))
                    {
                        draft.AttachmentsEmbedded.Add(attach);
                    }
                }

                draft.HtmlBody = doc.DocumentNode.OuterHtml;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("ChangeAllImagesLinksToEmbedded(): Exception: {0}", ex.ToString());
            }
        }

        private static MailAttachmentData CreateEmbbededAttachment(string fileName, string link, string fileLink, string user,
                                                        int tenant, int mailboxId, string streamId)
        {
            return new MailAttachmentData
            {
                fileName = fileName,
                storedName = fileName,
                contentId = link.GetMd5(),
                storedFileUrl = fileLink,
                streamId = streamId,
                user = user,
                tenant = tenant,
                mailboxId = mailboxId
            };
        }
    }
}
