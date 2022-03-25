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

using ASC.Notify.Engine;

using Context = ASC.Notify.Context;

namespace ASC.Files.Core.Services.NotifyService;

[Scope(Additional = typeof(NotifyClientExtension))]
public class NotifyClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NotifyEngine _notifyEngine;

    private Context NotifyContext { get; }

    public NotifyClient(IServiceProvider serviceProvider, Context notifyContext, NotifyEngine notifyEngine)
    {
        _serviceProvider = serviceProvider;
        NotifyContext = notifyContext;
        _notifyEngine = notifyEngine;
    }

    public void SendDocuSignComplete<T>(File<T> file, string sourceTitle)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
        var (notifySource, securityContext, filesLinkUtility, fileUtility, baseCommonLinkUtility, _, _, _, _) = scopeClass;
        var client = NotifyContext.RegisterClient(_notifyEngine, notifySource, scope);
        var recipient = notifySource.GetRecipientsProvider().GetRecipient(securityContext.CurrentAccount.ID.ToString());

        client.SendNoticeAsync(
            NotifyConstants.EventDocuSignComplete,
            file.UniqID,
            recipient,
            true,
            new TagValue(NotifyConstants.TagDocumentUrl, baseCommonLinkUtility.GetFullAbsolutePath(filesLinkUtility.GetFileWebPreviewUrl(fileUtility, file.Title, file.ID))),
            new TagValue(NotifyConstants.TagDocumentTitle, file.Title),
            new TagValue(NotifyConstants.TagMessage, sourceTitle)
            );
    }

    public void SendDocuSignStatus(string subject, string status)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
        var (notifySource, securityContext, _, _, _, _, _, _, _) = scopeClass;
        var client = NotifyContext.RegisterClient(_notifyEngine, notifySource, scope);

        var recipient = notifySource.GetRecipientsProvider().GetRecipient(securityContext.CurrentAccount.ID.ToString());

        client.SendNoticeAsync(
            NotifyConstants.EventDocuSignStatus,
            null,
            recipient,
            true,
            new TagValue(NotifyConstants.TagDocumentTitle, subject),
            new TagValue(NotifyConstants.TagMessage, status)
            );
    }

    public void SendMailMergeEnd(Guid userId, int countMails, int countError)
    {
        using var scope = _serviceProvider.CreateScope();
        var notifySource = scope.ServiceProvider.GetService<NotifySource>();
        var client = NotifyContext.RegisterClient(_notifyEngine, notifySource, scope);

        var recipient = notifySource.GetRecipientsProvider().GetRecipient(userId.ToString());

        client.SendNoticeAsync(
            NotifyConstants.EventMailMergeEnd,
            null,
            recipient,
            true,
            new TagValue(NotifyConstants.TagMailsCount, countMails),
            new TagValue(NotifyConstants.TagMessage, countError > 0 ? string.Format(FilesCommonResource.ErrorMassage_MailMergeCount, countError) : string.Empty)
            );
    }

    public async Task SendShareNoticeAsync<T>(FileEntry<T> fileEntry, Dictionary<Guid, FileShare> recipients, string message)
    {
        if (fileEntry == null || recipients.Count == 0)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
        var (notifySource, _, filesLinkUtility, fileUtility, baseCommonLinkUtility, daoFactory, pathProvider, userManager, tenantManager) = scopeClass;
        var client = NotifyContext.RegisterClient(_notifyEngine, notifySource, scope);
        var studioNotifyHelper = scope.ServiceProvider.GetService<StudioNotifyHelper>();

        var folderDao = daoFactory.GetFolderDao<T>();
        if (fileEntry.FileEntryType == FileEntryType.File && await folderDao.GetFolderAsync(((File<T>)fileEntry).FolderID) == null)
        {
            return;
        }

        var url = fileEntry.FileEntryType == FileEntryType.File
                      ? filesLinkUtility.GetFileWebPreviewUrl(fileUtility, fileEntry.Title, fileEntry.ID)
                      : await pathProvider.GetFolderUrlAsync((Folder<T>)fileEntry);

        var recipientsProvider = notifySource.GetRecipientsProvider();

        var action = fileEntry.FileEntryType == FileEntryType.File
        ? ((File<T>)fileEntry).Encrypted
            ? NotifyConstants.EventShareEncryptedDocument
            : NotifyConstants.EventShareDocument
        : NotifyConstants.EventShareFolder;


        foreach (var recipientPair in recipients)
        {
            var u = userManager.GetUsers(recipientPair.Key);
            var culture = string.IsNullOrEmpty(u.CultureName)
                              ? tenantManager.GetCurrentTenant().GetCulture()
                              : CultureInfo.GetCultureInfo(u.CultureName);

            var aceString = GetAccessString(recipientPair.Value, culture);
            var recipient = recipientsProvider.GetRecipient(u.Id.ToString());

            client.SendNoticeAsync(
                action,
                fileEntry.UniqID,
                recipient,
                true,
                new TagValue(NotifyConstants.TagDocumentTitle, fileEntry.Title),
                new TagValue(NotifyConstants.TagDocumentUrl, baseCommonLinkUtility.GetFullAbsolutePath(url)),
                new TagValue(NotifyConstants.TagAccessRights, aceString),
                new TagValue(NotifyConstants.TagMessage, message.HtmlEncode()),
                TagValues.Image(studioNotifyHelper, 0, "privacy.png")
                );
        }
    }

    public void SendEditorMentions<T>(FileEntry<T> file, string documentUrl, List<Guid> recipientIds, string message)
    {
        if (file == null || recipientIds.Count == 0)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<NotifyClientScope>();
        var (notifySource, _, _, _, baseCommonLinkUtility, _, _, userManager, _) = scopeClass;
        var client = NotifyContext.RegisterClient(_notifyEngine, notifySource, scope);

        var recipientsProvider = notifySource.GetRecipientsProvider();

        foreach (var recipientId in recipientIds)
        {
            var u = userManager.GetUsers(recipientId);

            var recipient = recipientsProvider.GetRecipient(u.Id.ToString());

            client.SendNoticeAsync(
                NotifyConstants.EventEditorMentions,
                file.UniqID,
                recipient,
                true,
                new TagValue(NotifyConstants.TagDocumentTitle, file.Title),
                new TagValue(NotifyConstants.TagDocumentUrl, baseCommonLinkUtility.GetFullAbsolutePath(documentUrl)),
                new TagValue(NotifyConstants.TagMessage, message.HtmlEncode())
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
    private readonly NotifySource _notifySource;
    private readonly SecurityContext _securityContext;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUtility _fileUtility;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly IDaoFactory _daoFactory;
    private readonly PathProvider _pathProvider;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;

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
        _notifySource = notifySource;
        _securityContext = securityContext;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _daoFactory = daoFactory;
        _pathProvider = pathProvider;
        _userManager = userManager;
        _tenantManager = tenantManager;
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
        notifySource = _notifySource;
        securityContext = _securityContext;
        filesLinkUtility = _filesLinkUtility;
        fileUtility = _fileUtility;
        baseCommonLinkUtility = _baseCommonLinkUtility;
        daoFactory = _daoFactory;
        pathProvider = _pathProvider;
        userManager = _userManager;
        tenantManager = _tenantManager;
    }
}

public static class NotifyClientExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<NotifyClientScope>();
    }
}
