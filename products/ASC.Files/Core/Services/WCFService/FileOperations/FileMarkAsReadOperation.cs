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

namespace ASC.Web.Files.Services.WCFService.FileOperations;

class FileMarkAsReadOperationData<T> : FileOperationData<T>
{
    public IDictionary<string, StringValues> Headers { get; }

    public FileMarkAsReadOperationData(IEnumerable<T> folders, IEnumerable<T> files, Tenant tenant, IDictionary<string, StringValues> headers, ExternalShareData externalShareData, 
        bool holdResult = true) : base(folders, files, tenant, externalShareData, holdResult)
    {
        Headers = headers;
    }
}

[Transient]
class FileMarkAsReadOperation : ComposeFileOperation<FileMarkAsReadOperationData<string>, FileMarkAsReadOperationData<int>>
{
    public FileMarkAsReadOperation(IServiceProvider serviceProvider, FileOperation<FileMarkAsReadOperationData<string>, string> f1, FileOperation<FileMarkAsReadOperationData<int>, int> f2)
        : base(serviceProvider, f1, f2)
    {
        this[OpType] = (int)FileOperationType.MarkAsRead;
    }
}

class FileMarkAsReadOperation<T> : FileOperation<FileMarkAsReadOperationData<T>, T>
{
    private readonly IDictionary<string, StringValues> _headers;

    public FileMarkAsReadOperation(IServiceProvider serviceProvider, FileMarkAsReadOperationData<T> fileOperationData)
        : base(serviceProvider, fileOperationData)
    {
        _headers = fileOperationData.Headers;
        this[OpType] = (int)FileOperationType.MarkAsRead;
    }

    protected override int InitTotalProgressSteps()
    {
        return Files.Count + Folders.Count;
    }

    protected override async Task DoJob(IServiceScope scope)
    {
        var scopeClass = scope.ServiceProvider.GetService<FileMarkAsReadOperationScope>();
        var filesMessageService = scope.ServiceProvider.GetRequiredService<FilesMessageService>();
        var (fileMarker, globalFolder, daoFactory, settingsManager) = scopeClass;
        var entries = Enumerable.Empty<FileEntry<T>>();
        if (Folders.Count > 0)
        {
            entries = entries.Concat(await FolderDao.GetFoldersAsync(Folders).ToListAsync());
        }
        if (Files.Count > 0)
        {
            entries = entries.Concat(await FileDao.GetFilesAsync(Files).ToListAsync());
        }

        foreach (var entry in entries)
        {
            CancellationToken.ThrowIfCancellationRequested();

            await fileMarker.RemoveMarkAsNewAsync(entry, ((IAccount)(_principal ?? CustomSynchronizationContext.CurrentContext.CurrentPrincipal).Identity).ID);

            if (entry.FileEntryType == FileEntryType.File)
            {
                ProcessedFile(((File<T>)entry).Id);
                _ = filesMessageService.SendAsync(entry, _headers, MessageAction.FileMarkedAsRead, entry.Title);
            }
            else
            {
                ProcessedFolder(((Folder<T>)entry).Id);
                _ = filesMessageService.SendAsync(entry, _headers, MessageAction.FolderMarkedAsRead, entry.Title);
            }

            ProgressStep();
        }


        var rootIds = new List<int>
            {
                await globalFolder.GetFolderMyAsync(fileMarker, daoFactory),
                await globalFolder.GetFolderCommonAsync(fileMarker, daoFactory),
                await globalFolder.GetFolderShareAsync(daoFactory),
                await globalFolder.GetFolderProjectsAsync(daoFactory),
                await globalFolder.GetFolderVirtualRoomsAsync(daoFactory),
            };

        if (await PrivacyRoomSettings.GetEnabledAsync(settingsManager))
        {
            rootIds.Add(await globalFolder.GetFolderPrivacyAsync(daoFactory));
        }

        var newrootfolder = new List<string>();

        foreach (var r in rootIds.Where(id => id != 0))
        {
            var item = new KeyValuePair<int, int>(r, await fileMarker.GetRootFoldersIdMarkedAsNewAsync(r));
            newrootfolder.Add($"new_{{\"key\"? \"{item.Key}\", \"value\"? \"{item.Value}\"}}");
        }

        this[Res] += string.Join(SplitChar, newrootfolder.ToArray());
    }
}

[Scope]
public class FileMarkAsReadOperationScope
{
    private readonly FileMarker _fileMarker;
    private readonly GlobalFolder _globalFolder;
    private readonly IDaoFactory _daoFactory;
    private readonly SettingsManager _settingsManager;

    public FileMarkAsReadOperationScope(
        FileMarker fileMarker,
        GlobalFolder globalFolder,
        IDaoFactory daoFactory,
        SettingsManager settingsManager)
    {
        _fileMarker = fileMarker;
        _globalFolder = globalFolder;
        _daoFactory = daoFactory;
        _settingsManager = settingsManager;
    }

    public void Deconstruct(
        out FileMarker fileMarker,
        out GlobalFolder globalFolder,
        out IDaoFactory daoFactory,
        out SettingsManager settingsManager)
    {
        fileMarker = _fileMarker;
        globalFolder = _globalFolder;
        daoFactory = _daoFactory;
        settingsManager = _settingsManager;
    }
}
