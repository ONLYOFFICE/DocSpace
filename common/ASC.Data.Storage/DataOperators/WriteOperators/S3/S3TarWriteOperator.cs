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

namespace ASC.Data.Storage.DataOperators;
public class S3TarWriteOperator : IDataWriteOperator
{
    private readonly CommonChunkedUploadSession _chunkedUploadSession;
    private readonly CommonChunkedUploadSessionHolder _sessionHolder;
    private readonly S3Storage _store;
    private readonly string _domain;
    private readonly string _key;

    public string Hash { get; private set; }
    public string StoragePath { get; private set; }
    public bool NeedUpload => false;

    public S3TarWriteOperator(CommonChunkedUploadSession chunkedUploadSession, CommonChunkedUploadSessionHolder sessionHolder)
    {
        _chunkedUploadSession = chunkedUploadSession;
        _sessionHolder = sessionHolder;
        _store = _sessionHolder.DataStore as S3Storage;

        _key = _chunkedUploadSession.TempPath;
        _domain = _sessionHolder.TempDomain;
    }

    public async Task WriteEntryAsync(string tarKey, string domain, string path, IDataStore store)
    {
        if (store is S3Storage) 
        {
            var s3Store = store as S3Storage;
            var fullPath = s3Store.MakePath(domain, path);

            await _store.ConcatFileAsync(fullPath, tarKey, _domain, _key);
        }
        else
        {
            Stream fileStream = null;
            await ActionInvoker.TryAsync(async () =>
            {
                fileStream = await store.GetReadStreamAsync(domain, path);
            }, 5, error => throw error);
            if (fileStream != null)
            {
                await WriteEntryAsync(tarKey, fileStream);
                fileStream.Dispose();
            }
        }
    }

    public async Task WriteEntryAsync(string tarKey, Stream stream)
    {
        await _store.ConcatFileStreamAsync(stream, tarKey, _domain, _key);
    }

    public async ValueTask DisposeAsync()
    {
        await _store.AddEndAsync(_domain ,_key);
        await _store.RemoveFirstBlockAsync(_domain ,_key);

        var contentLength = await _store.GetFileSizeAsync(_domain, _key);
        Hash = (await _store.GetFileEtagAsync(_domain, _key)).Trim('\"');

        (var uploadId, var eTags, var partNumber) = await _store.InitiateConcatAsync(_domain, _key, lastInit: true);

        _chunkedUploadSession.BytesUploaded = contentLength;
        _chunkedUploadSession.BytesTotal = contentLength;
        _chunkedUploadSession.UploadId = uploadId;
        _chunkedUploadSession.Items["ETag"] = eTags.ToDictionary(e => e.PartNumber, e => e.ETag);
        _chunkedUploadSession.Items["ChunksUploaded"] = (partNumber - 1).ToString();

        StoragePath = await _sessionHolder.FinalizeAsync(_chunkedUploadSession);
    }
}
