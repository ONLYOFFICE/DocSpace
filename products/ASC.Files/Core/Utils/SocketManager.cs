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

namespace ASC.Web.Files.Utils;

public class SocketManager : SocketServiceClient
{
    private readonly FileDtoHelper _filesWrapperHelper;
    private readonly FolderDtoHelper _folderDtoHelper;
    private readonly TenantManager _tenantManager;

    public override string Hub => "files";

    public SocketManager(
        ILogger<SocketServiceClient> logger,
        IHttpClientFactory clientFactory,
        MachinePseudoKeys mashinePseudoKeys,
        IConfiguration configuration,
        FileDtoHelper filesWrapperHelper,
        TenantManager tenantManager,
        FolderDtoHelper folderDtoHelper) : base(logger, clientFactory, mashinePseudoKeys, configuration)
    {
        _filesWrapperHelper = filesWrapperHelper;
        _tenantManager = tenantManager;
        _folderDtoHelper = folderDtoHelper;
    }

    public async Task StartEditAsync<T>(T fileId)
    {
        var room = await GetFileRoomAsync(fileId);
        await MakeRequest("start-edit", new { room, fileId });
    }

    public async Task StopEditAsync<T>(T fileId)
    {
        var room = await GetFileRoomAsync(fileId);
        await MakeRequest("stop-edit", new { room, fileId });
    }

    public async Task CreateFileAsync<T>(File<T> file)
    {
        var room = await GetFolderRoomAsync(file.ParentId);

        var data = await SerializeFile(file);

        await MakeRequest("create-file", new { room, fileId = file.Id, data });
    }

    public async Task CreateFolderAsync<T>(Folder<T> folder)
    {
        var room = await GetFolderRoomAsync(folder.ParentId);

        var data = await SerializeFolder(folder);

        await MakeRequest("create-folder", new { room, folderId = folder.Id, data });
    }

    public async Task UpdateFileAsync<T>(File<T> file)
    {
        var room = await GetFolderRoomAsync(file.ParentId);

        var data = await SerializeFile(file);

        await MakeRequest("update-file", new { room, fileId = file.Id, data });
    }

    public async Task UpdateFolderAsync<T>(Folder<T> folder)
    {
        var room = await GetFolderRoomAsync(folder.ParentId);

        var data = await SerializeFolder(folder);

        await MakeRequest("update-folder", new { room, folderId = folder.Id, data });
    }

    public async Task DeleteFileAsync<T>(File<T> file)
    {
        var room = await GetFolderRoomAsync(file.ParentId);

        await MakeRequest("delete-file", new { room, fileId = file.Id });
    }

    public async Task DeleteFolder<T>(Folder<T> folder)
    {
        var room = await GetFolderRoomAsync(folder.ParentId);

        await MakeRequest("delete-folder", new { room, folderId = folder.Id });
    }

    public async Task ExecMarkAsNewFilesAsync(IEnumerable<Tag> tags)
    {
        var result = await tags.ToAsyncEnumerable()
            .SelectAwait(async tag => new
            {
                room = await GetFileRoomAsync(tag.EntryId, tag.Owner),
                fileId = tag.EntryId,
                count = tag.Count
            })
            .ToListAsync();

        SendNotAwaitableRequest("markasnew-file", result);
    }

    public async Task ExecMarkAsNewFoldersAsync(IEnumerable<Tag> tags)
    {
        var result = await tags.ToAsyncEnumerable()
            .SelectAwait(async tag => new
            {
                room = await GetFolderRoomAsync(tag.EntryId, tag.Owner),
                folderId = tag.EntryId,
                count = tag.Count
            })
            .ToListAsync();

        SendNotAwaitableRequest("markasnew-folder", result);
    }

    private async Task<string> GetFileRoomAsync<T>(T fileId, Guid? owner = null)
    {
        var tenantId = await _tenantManager.GetCurrentTenantIdAsync();
        var ownerData = owner.HasValue ? "-" + owner.Value : "";

        return $"{tenantId}-FILE-{fileId}{ownerData}";
    }

    private async Task<string> GetFolderRoomAsync<T>(T folderId, Guid? owner = null)
    {
        var tenantId = await _tenantManager.GetCurrentTenantIdAsync();
        var ownerData = owner.HasValue ? "-" + owner.Value : "";

        return $"{tenantId}-DIR-{folderId}{ownerData}";
    }

    private async Task<string> SerializeFile<T>(File<T> file)
    {
        return JsonSerializer.Serialize(await _filesWrapperHelper.GetAsync(file), typeof(FileDto<T>), FileEntryDtoContext.Default);
    }

    private async Task<string> SerializeFolder<T>(Folder<T> folder)
    {
        return JsonSerializer.Serialize(await _folderDtoHelper.GetAsync(folder), typeof(FolderDto<T>), FileEntryDtoContext.Default);
    }
}
