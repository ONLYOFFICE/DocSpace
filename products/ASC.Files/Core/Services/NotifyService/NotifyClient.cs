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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Notify.Patterns;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Core.Notify;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Core.Services.NotifyService
{
    [Scope(Additional = typeof(NotifyClientExtension))]
    public class NotifyClient
    {
        private IServiceProvider ServiceProvider { get; }

        public NotifyClient(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void SendDocuSignComplete<T>(File<T> file, string sourceTitle)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
            var (notifySource, securityContext, filesLinkUtility, fileUtility, baseCommonLinkUtility, _, _, _, _) = scopeClass;
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(notifySource, scope);
            var recipient = notifySource.GetRecipientsProvider().GetRecipient(securityContext.CurrentAccount.ID.ToString());

            client.SendNoticeAsync(
                NotifyConstants.Event_DocuSignComplete,
                file.UniqID,
                recipient,
                true,
                new TagValue(NotifyConstants.Tag_DocumentUrl, baseCommonLinkUtility.GetFullAbsolutePath(filesLinkUtility.GetFileWebPreviewUrl(fileUtility, file.Title, file.ID))),
                new TagValue(NotifyConstants.Tag_DocumentTitle, file.Title),
                new TagValue(NotifyConstants.Tag_Message, sourceTitle)
                );
        }

        public void SendDocuSignStatus(string subject, string status)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
            var (notifySource, securityContext, _, _, _, _, _, _, _) = scopeClass;
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(notifySource, scope);

            var recipient = notifySource.GetRecipientsProvider().GetRecipient(securityContext.CurrentAccount.ID.ToString());

            client.SendNoticeAsync(
                NotifyConstants.Event_DocuSignStatus,
                null,
                recipient,
                true,
                new TagValue(NotifyConstants.Tag_DocumentTitle, subject),
                new TagValue(NotifyConstants.Tag_Message, status)
                );
        }

        public void SendMailMergeEnd(Guid userId, int countMails, int countError)
        {
            using var scope = ServiceProvider.CreateScope();
            var notifySource = scope.ServiceProvider.GetService<NotifySource>();
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(notifySource, scope);

            var recipient = notifySource.GetRecipientsProvider().GetRecipient(userId.ToString());

            client.SendNoticeAsync(
                NotifyConstants.Event_MailMergeEnd,
                null,
                recipient,
                true,
                new TagValue(NotifyConstants.Tag_MailsCount, countMails),
                new TagValue(NotifyConstants.Tag_Message, countError > 0 ? string.Format(FilesCommonResource.ErrorMassage_MailMergeCount, countError) : string.Empty)
                );
        }

        public async Task SendShareNoticeAsync<T>(FileEntry<T> fileEntry, Dictionary<Guid, FileShare> recipients, string message)
        {
            if (fileEntry == null || recipients.Count == 0) return;

            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
            var (notifySource, _, filesLinkUtility, fileUtility, baseCommonLinkUtility, daoFactory, pathProvider, userManager, tenantManager) = scopeClass;
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(notifySource, scope);
            var studioNotifyHelper = scope.ServiceProvider.GetService<StudioNotifyHelper>();

            var folderDao = daoFactory.GetFolderDao<T>();
            if (fileEntry.FileEntryType == FileEntryType.File && await folderDao.GetFolderAsync(((File<T>)fileEntry).FolderID) == null) return;

            var url = fileEntry.FileEntryType == FileEntryType.File
                          ? filesLinkUtility.GetFileWebPreviewUrl(fileUtility, fileEntry.Title, fileEntry.ID)
                          : await pathProvider.GetFolderUrlAsync((Folder<T>)fileEntry);

            var recipientsProvider = notifySource.GetRecipientsProvider();

            var action = fileEntry.FileEntryType == FileEntryType.File
            ? ((File<T>)fileEntry).Encrypted
                ? NotifyConstants.Event_ShareEncryptedDocument
                : NotifyConstants.Event_ShareDocument
            : NotifyConstants.Event_ShareFolder;


            foreach (var recipientPair in recipients)
            {
                var u = userManager.GetUsers(recipientPair.Key);
                var culture = string.IsNullOrEmpty(u.CultureName)
                                  ? tenantManager.GetCurrentTenant().GetCulture()
                                  : CultureInfo.GetCultureInfo(u.CultureName);

                var aceString = GetAccessString(recipientPair.Value, culture);
                var recipient = recipientsProvider.GetRecipient(u.ID.ToString());

                client.SendNoticeAsync(
                    action,
                    fileEntry.UniqID,
                    recipient,
                    true,
                    new TagValue(NotifyConstants.Tag_DocumentTitle, fileEntry.Title),
                    new TagValue(NotifyConstants.Tag_DocumentUrl, baseCommonLinkUtility.GetFullAbsolutePath(url)),
                    new TagValue(NotifyConstants.Tag_AccessRights, aceString),
                    new TagValue(NotifyConstants.Tag_Message, message.HtmlEncode()),
                    TagValues.Image(studioNotifyHelper, 0, "privacy.png")
                    );
            }
        }

        public void SendEditorMentions<T>(FileEntry<T> file, string documentUrl, List<Guid> recipientIds, string message)
        {
            if (file == null || recipientIds.Count == 0) return;

            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
            var (notifySource, _, _, _, baseCommonLinkUtility, _, _, userManager, _) = scopeClass;
            var client = WorkContext.NotifyContext.NotifyService.RegisterClient(notifySource, scope);

            var recipientsProvider = notifySource.GetRecipientsProvider();

            foreach (var recipientId in recipientIds)
            {
                var u = userManager.GetUsers(recipientId);

                var recipient = recipientsProvider.GetRecipient(u.ID.ToString());

                client.SendNoticeAsync(
                    NotifyConstants.Event_EditorMentions,
                    file.UniqID,
                    recipient,
                    true,
                    new TagValue(NotifyConstants.Tag_DocumentTitle, file.Title),
                    new TagValue(NotifyConstants.Tag_DocumentUrl, baseCommonLinkUtility.GetFullAbsolutePath(documentUrl)),
                    new TagValue(NotifyConstants.Tag_Message, message.HtmlEncode())
                    );
            }
        }

        private static string GetAccessString(FileShare fileShare, CultureInfo cultureInfo)
        {
            return fileShare switch
            {
                FileShare.Read => FilesCommonResource.ResourceManager.GetString("AceStatusEnum_Read", cultureInfo),
                FileShare.ReadWrite => FilesCommonResource.ResourceManager.GetString("AceStatusEnum_ReadWrite", cultureInfo),
                FileShare.CustomFilter => FilesCommonResource.ResourceManager.GetString("AceStatusEnum_CustomFilter", cultureInfo),
                FileShare.Review => FilesCommonResource.ResourceManager.GetString("AceStatusEnum_Review", cultureInfo),
                FileShare.FillForms => FilesCommonResource.ResourceManager.GetString("AceStatusEnum_FillForms", cultureInfo),
                FileShare.Comment => FilesCommonResource.ResourceManager.GetString("AceStatusEnum_Comment", cultureInfo),
                _ => string.Empty,
            };
        }
    }

    [Scope]
    public class NotifyClientScope
    {
        private NotifySource NotifySource { get; }
        private SecurityContext SecurityContext { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileUtility FileUtility { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private IDaoFactory DaoFactory { get; }
        private PathProvider PathProvider { get; }
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }

        public NotifyClientScope(NotifySource notifySource,
            SecurityContext securityContext,
            FilesLinkUtility filesLinkUtility,
            FileUtility fileUtility,
            BaseCommonLinkUtility baseCommonLinkUtility,
            IDaoFactory daoFactory,
            PathProvider pathProvider,
            UserManager userManager,
            TenantManager tenantManager)
        {
            NotifySource = notifySource;
            SecurityContext = securityContext;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            DaoFactory = daoFactory;
            PathProvider = pathProvider;
            UserManager = userManager;
            TenantManager = tenantManager;
        }

        public void Deconstruct(out NotifySource notifySource,
            out SecurityContext securityContext,
            out FilesLinkUtility filesLinkUtility,
            out FileUtility fileUtility,
            out BaseCommonLinkUtility baseCommonLinkUtility,
            out IDaoFactory daoFactory,
            out PathProvider pathProvider,
            out UserManager userManager,
            out TenantManager tenantManager)
        {
            notifySource = NotifySource;
            securityContext = SecurityContext;
            filesLinkUtility = FilesLinkUtility;
            fileUtility = FileUtility;
            baseCommonLinkUtility = BaseCommonLinkUtility;
            daoFactory = DaoFactory;
            pathProvider = PathProvider;
            userManager = UserManager;
            tenantManager = TenantManager;
        }
    }

    public static class NotifyClientExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<NotifyClientScope>();
        }
    }
}