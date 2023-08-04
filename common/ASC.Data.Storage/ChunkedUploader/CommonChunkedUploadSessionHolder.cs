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

namespace ASC.Core.ChunkedUploader;

public class CommonChunkedUploadSessionHolder
{
    public IDataStore DataStore { get; set; }

    public static readonly TimeSpan SlidingExpiration = TimeSpan.FromHours(12);
    private readonly TempPath _tempPath;
    private readonly string _domain;
    public long MaxChunkUploadSize;

    public const string StoragePath = "sessions";
    private readonly object _locker = new object();

    public CommonChunkedUploadSessionHolder(
        TempPath tempPath,
        IDataStore dataStore,
        string domain,
        long maxChunkUploadSize = 10 * 1024 * 1024)
    {
        _tempPath = tempPath;
        DataStore = dataStore;
        _domain = domain;
        MaxChunkUploadSize = maxChunkUploadSize;
    }

    public async Task StoreAsync(CommonChunkedUploadSession s)
    {
        await using var stream = s.Serialize();
        await DataStore.SavePrivateAsync(_domain, GetPathWithId(s.Id), stream, s.Expired);
    }

    public async Task RemoveAsync(CommonChunkedUploadSession s)
    {
        await DataStore.DeleteAsync(_domain, GetPathWithId(s.Id));
    }

    public async Task<Stream> GetStreamAsync(string sessionId)
    {
        return await DataStore.GetReadStreamAsync(_domain, GetPathWithId(sessionId));
    }

    public async ValueTask InitAsync(CommonChunkedUploadSession chunkedUploadSession)
    {
        if (chunkedUploadSession.BytesTotal < MaxChunkUploadSize && chunkedUploadSession.BytesTotal != -1)
        {
            chunkedUploadSession.UseChunks = false;
            return;
        }

        var tempPath = Guid.NewGuid().ToString();
        var uploadId = await DataStore.InitiateChunkedUploadAsync(_domain, tempPath);

        chunkedUploadSession.TempPath = tempPath;
        chunkedUploadSession.UploadId = uploadId;
    }

    public virtual async Task<string> FinalizeAsync(CommonChunkedUploadSession uploadSession)
    {
        var tempPath = uploadSession.TempPath;
        var uploadId = uploadSession.UploadId;
        var eTags = uploadSession.GetItemOrDefault<Dictionary<int, string>>("ETag");

        await DataStore.FinalizeChunkedUploadAsync(_domain, tempPath, uploadId, eTags);
        return Path.GetFileName(tempPath);
    }

    public async Task MoveAsync(CommonChunkedUploadSession chunkedUploadSession, string newPath, bool quotaCheckFileSize = true)
    {
        await DataStore.MoveAsync(_domain, chunkedUploadSession.TempPath, string.Empty, newPath, quotaCheckFileSize);
    }

    public async Task AbortAsync(CommonChunkedUploadSession uploadSession)
    {
        if (uploadSession.UseChunks)
        {
            var tempPath = uploadSession.TempPath;
            var uploadId = uploadSession.UploadId;

            await DataStore.AbortChunkedUploadAsync(_domain, tempPath, uploadId);
        }
        else if (!string.IsNullOrEmpty(uploadSession.ChunksBuffer))
        {
            File.Delete(uploadSession.ChunksBuffer);
        }
    }

    public virtual async Task<string> UploadChunkAsync(CommonChunkedUploadSession uploadSession, Stream stream, long length)
    {
        var tempPath = uploadSession.TempPath;
        var uploadId = uploadSession.UploadId;

        int chunkNumber;
        lock (_locker)
        {
            int.TryParse(uploadSession.GetItemOrDefault<string>("ChunksUploaded"), out chunkNumber);
            chunkNumber++;
            uploadSession.Items["ChunksUploaded"] = chunkNumber.ToString();
            uploadSession.BytesUploaded += length;
        }

        var eTag = await DataStore.UploadChunkAsync(_domain, tempPath, uploadId, stream, MaxChunkUploadSize, chunkNumber, length);

        lock (_locker)
        {
            var eTags = uploadSession.GetItemOrDefault<Dictionary<int, string>>("ETag") ?? new Dictionary<int, string>();
            eTags.Add(chunkNumber, eTag);
            uploadSession.Items["ETag"] = eTags;
        }
        return Path.GetFileName(tempPath);
    }

    public async Task<Stream> UploadSingleChunkAsync(CommonChunkedUploadSession uploadSession, Stream stream, long chunkLength)
    {
        if (uploadSession.BytesTotal == 0)
        {
            uploadSession.BytesTotal = chunkLength;
        }

        if (uploadSession.BytesTotal >= chunkLength)
        {
            //This is hack fixing strange behaviour of plupload in flash mode.

            if (string.IsNullOrEmpty(uploadSession.ChunksBuffer))
            {
                uploadSession.ChunksBuffer = _tempPath.GetTempFileName();
            }

            await using (var bufferStream = new FileStream(uploadSession.ChunksBuffer, FileMode.Append))
            {
                await stream.CopyToAsync(bufferStream);
            }

            uploadSession.BytesUploaded += chunkLength;

            if (uploadSession.BytesTotal == uploadSession.BytesUploaded)
            {
                return new FileStream(uploadSession.ChunksBuffer, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite,
                    4096, FileOptions.DeleteOnClose);
            }
        }

        return Stream.Null;
    }

    private string GetPathWithId(string id)
    {
        return CrossPlatform.PathCombine(StoragePath, id + ".session");
    }
}
