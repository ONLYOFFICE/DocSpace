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

using ResumableUploadSession = ASC.Files.Thirdparty.OneDrive.ResumableUploadSession;
using ResumableUploadSessionStatus = ASC.Files.Thirdparty.OneDrive.ResumableUploadSessionStatus;

namespace ASC.Files.Core.Core.Thirdparty.OneDrive;

[Scope]
internal class OneDriveFileDao : ThirdPartyFileDao<Item, Item, Item>
{
    private readonly SetupInfo _setupInfo;
    private readonly TempPath _tempPath;

    public OneDriveFileDao(UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        IDaoSelector<Item, Item, Item> daoSelector,
        CrossDao crossDao,
        IFileDao<int> fileDao,
        IDaoBase<Item, Item, Item> dao,
        SetupInfo setupInfo,
        TempPath tempPath) : base(userManager, dbContextFactory, daoSelector, crossDao, fileDao, dao)
    {
        _setupInfo = setupInfo;
        _tempPath = tempPath;
    }

    public override async Task<ChunkedUploadSession<string>> CreateUploadSessionAsync(File<string> file, long contentLength)
    {
        if (_setupInfo.ChunkUploadSize > contentLength && contentLength != -1)
        {
            return new ChunkedUploadSession<string>(RestoreIds(file), contentLength) { UseChunks = false };
        }

        var uploadSession = new ChunkedUploadSession<string>(file, contentLength);

        Item onedriveFile;
        if (file.Id != null)
        {
            onedriveFile = await Dao.GetFileAsync(file.Id);
        }
        else
        {
            var folder = await Dao.GetFolderAsync(file.ParentId);
            onedriveFile = new Item { Name = file.Title, ParentReference = new ItemReference { Id = folder.Id } };
        }

        var storage = (OneDriveStorage)await ProviderInfo.StorageAsync;
        var onedriveSession = await storage.CreateResumableSessionAsync(onedriveFile, contentLength);
        if (onedriveSession != null)
        {
            uploadSession.Items["OneDriveSession"] = onedriveSession;
        }
        else
        {
            uploadSession.Items["TempPath"] = _tempPath.GetTempFileName();
        }

        uploadSession.File = RestoreIds(uploadSession.File);

        return uploadSession;
    }

    public override async Task<File<string>> UploadChunkAsync(ChunkedUploadSession<string> uploadSession, Stream stream, long chunkLength)
    {
        if (!uploadSession.UseChunks)
        {
            if (uploadSession.BytesTotal == 0)
            {
                uploadSession.BytesTotal = chunkLength;
            }

            uploadSession.File = await SaveFileAsync(uploadSession.File, stream);
            uploadSession.BytesUploaded = chunkLength;

            return uploadSession.File;
        }

        if (uploadSession.Items.ContainsKey("OneDriveSession"))
        {
            var oneDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("OneDriveSession");
            var storage = (OneDriveStorage)await ProviderInfo.StorageAsync;
            await storage.TransferAsync(oneDriveSession, stream, chunkLength);
        }
        else
        {
            var tempPath = uploadSession.GetItemOrDefault<string>("TempPath");
            using var fs = new FileStream(tempPath, FileMode.Append);
            await stream.CopyToAsync(fs);
        }

        uploadSession.BytesUploaded += chunkLength;

        if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
        {
            uploadSession.File = await FinalizeUploadSessionAsync(uploadSession);
        }
        else
        {
            uploadSession.File = RestoreIds(uploadSession.File);
        }

        return uploadSession.File;
    }

    public override async Task<File<string>> FinalizeUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("OneDriveSession"))
        {
            var oneDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("OneDriveSession");

            await ProviderInfo.CacheResetAsync(oneDriveSession.FileId);
            var parentDriveId = oneDriveSession.FolderId;
            if (parentDriveId != null)
            {
                await ProviderInfo.CacheResetAsync(parentDriveId);
            }

            return Dao.ToFile(await Dao.GetFileAsync(oneDriveSession.FileId));
        }

        using var fs = new FileStream(uploadSession.GetItemOrDefault<string>("TempPath"), FileMode.Open, FileAccess.Read, System.IO.FileShare.None, 4096, FileOptions.DeleteOnClose);

        return await SaveFileAsync(uploadSession.File, fs);
    }

    public override async Task AbortUploadSessionAsync(ChunkedUploadSession<string> uploadSession)
    {
        if (uploadSession.Items.ContainsKey("OneDriveSession"))
        {
            var oneDriveSession = uploadSession.GetItemOrDefault<ResumableUploadSession>("OneDriveSession");

            if (oneDriveSession.Status != ResumableUploadSessionStatus.Completed)
            {
                var storage = (OneDriveStorage)await ProviderInfo.StorageAsync;
                await storage.CancelTransferAsync(oneDriveSession);

                oneDriveSession.Status = ResumableUploadSessionStatus.Aborted;
            }
        }
        else if (uploadSession.Items.ContainsKey("TempPath"))
        {
            System.IO.File.Delete(uploadSession.GetItemOrDefault<string>("TempPath"));
        }
    }
}