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

namespace ASC.Core.ChunkedUploader;

public class CommonChunkedUploadSessionHolder
{
    public IDataStore DataStore { get; set; }

    public static readonly TimeSpan SlidingExpiration = TimeSpan.FromHours(12);
    private readonly TempPath _tempPath;
    private readonly IOptionsMonitor<ILog> _option;
    private readonly string _domain;
    private readonly long _maxChunkUploadSize;

    private const string StoragePath = "sessions";

    public CommonChunkedUploadSessionHolder(
        TempPath tempPath,
        IOptionsMonitor<ILog> option,
        IDataStore dataStore,
        string domain,
        long maxChunkUploadSize = 10 * 1024 * 1024)
    {
        _tempPath = tempPath;
        _option = option;
        DataStore = dataStore;
        _domain = domain;
        _maxChunkUploadSize = maxChunkUploadSize;
    }

    public void DeleteExpired()
    {
        // clear old sessions
        try
        {
            DataStore.DeleteExpired(_domain, StoragePath, SlidingExpiration);
        }
        catch (Exception err)
        {
            _option.CurrentValue.Error(err);
        }
    }

    public void Store(CommonChunkedUploadSession s)
    {
        using var stream = s.Serialize();
        DataStore.SavePrivate(_domain, GetPathWithId(s.Id), stream, s.Expired);
    }

    public void Remove(CommonChunkedUploadSession s)
    {
        DataStore.Delete(_domain, GetPathWithId(s.Id));
    }

    public Stream GetStream(string sessionId)
    {
        return DataStore.GetReadStream(_domain, GetPathWithId(sessionId));
    }

    public void Init(CommonChunkedUploadSession chunkedUploadSession)
    {
        if (chunkedUploadSession.BytesTotal < _maxChunkUploadSize)
        {
            chunkedUploadSession.UseChunks = false;
            return;
        }

        var tempPath = Guid.NewGuid().ToString();
        var uploadId = DataStore.InitiateChunkedUpload(_domain, tempPath);

        chunkedUploadSession.TempPath = tempPath;
        chunkedUploadSession.UploadId = uploadId;
    }

    public void Finalize(CommonChunkedUploadSession uploadSession)
    {
        var tempPath = uploadSession.TempPath;
        var uploadId = uploadSession.UploadId;
        var eTags = uploadSession.GetItemOrDefault<List<string>>("ETag")
            .Select((x, i) => new KeyValuePair<int, string>(i + 1, x))
            .ToDictionary(x => x.Key, x => x.Value);

        DataStore.FinalizeChunkedUpload(_domain, tempPath, uploadId, eTags);
    }

    public void Move(CommonChunkedUploadSession chunkedUploadSession, string newPath, bool quotaCheckFileSize = true)
    {
        DataStore.Move(_domain, chunkedUploadSession.TempPath, string.Empty, newPath, quotaCheckFileSize);
    }

    public void Abort(CommonChunkedUploadSession uploadSession)
    {
        if (uploadSession.UseChunks)
        {
            var tempPath = uploadSession.TempPath;
            var uploadId = uploadSession.UploadId;

            DataStore.AbortChunkedUpload(_domain, tempPath, uploadId);
        }
        else if (!string.IsNullOrEmpty(uploadSession.ChunksBuffer))
        {
            File.Delete(uploadSession.ChunksBuffer);
        }
    }

    public void UploadChunk(CommonChunkedUploadSession uploadSession, Stream stream, long length)
    {
        var tempPath = uploadSession.TempPath;
        var uploadId = uploadSession.UploadId;
        var chunkNumber = uploadSession.GetItemOrDefault<int>("ChunksUploaded") + 1;

        var eTag = DataStore.UploadChunk(_domain, tempPath, uploadId, stream, _maxChunkUploadSize, chunkNumber, length);

        uploadSession.Items["ChunksUploaded"] = chunkNumber;
        uploadSession.BytesUploaded += length;

        var eTags = uploadSession.GetItemOrDefault<List<string>>("ETag") ?? new List<string>();
        eTags.Add(eTag);
        uploadSession.Items["ETag"] = eTags;
    }

    public Stream UploadSingleChunk(CommonChunkedUploadSession uploadSession, Stream stream, long chunkLength)
    {
        if (uploadSession.BytesTotal == 0)
            uploadSession.BytesTotal = chunkLength;

        if (uploadSession.BytesTotal >= chunkLength)
        {
            //This is hack fixing strange behaviour of plupload in flash mode.

            if (string.IsNullOrEmpty(uploadSession.ChunksBuffer))
            {
                uploadSession.ChunksBuffer = _tempPath.GetTempFileName();
            }

            using (var bufferStream = new FileStream(uploadSession.ChunksBuffer, FileMode.Append))
            {
                stream.CopyTo(bufferStream);
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
