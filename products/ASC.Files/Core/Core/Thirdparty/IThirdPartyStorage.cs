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

namespace ASC.Files.Core.Core.Thirdparty;
public interface IThirdPartyStorage
{
    bool IsOpened { get; }

    public void Open(OAuth20Token token);
    public void Close();
    public Task<long> GetMaxUploadSizeAsync();
    public Task<bool> CheckAccessAsync();
    public Task<Stream> GetThumbnailAsync(string fileId, int width, int height);
}

public interface IThirdPartyItemStorage<TItem> : IThirdPartyStorage
{
    public Task<List<TItem>> GetItemsAsync(string folderId);
    public Task DeleteItemAsync(TItem item);
}

public interface IGoogleDriveItemStorage<TItem> : IThirdPartyItemStorage<TItem>
{
    public Task<List<TItem>> GetItemsAsync(string folderId, bool? folder);
}

public interface IThirdPartyFileStorage<TFile> : IThirdPartyStorage
{
    public Task<TFile> GetFileAsync(string fileId);
    public Task<TFile> CreateFileAsync(Stream fileStream, string title, string parentId);
    public Task<Stream> DownloadStreamAsync(TFile file, int offset = 0);
    public Task<TFile> MoveFileAsync(string fileId, string newFileName, string toFolderId);
    public Task<TFile> CopyFileAsync(string fileId, string newFileName, string toFolderId);
    public Task<TFile> RenameFileAsync(string fileId, string newName);
    public Task<TFile> SaveStreamAsync(string fileId, Stream fileStream);

}

public interface IThirdPartyFolderStorage<TFolder> : IThirdPartyStorage
{
    public Task<TFolder> GetFolderAsync(string folderId);
    public Task<TFolder> CreateFolderAsync(string title, string parentId);
    public Task<TFolder> MoveFolderAsync(string folderId, string newFolderName, string toFolderId);
    public Task<TFolder> CopyFolderAsync(string folderId, string newFolderName, string toFolderId);
    public Task<TFolder> RenameFolderAsync(string folderId, string newName);
}

[Transient]
public interface IThirdPartyStorage<TFile, TFolder, TItem> : IThirdPartyFileStorage<TFile>, IThirdPartyFolderStorage<TFolder>, IThirdPartyItemStorage<TItem>
{

}