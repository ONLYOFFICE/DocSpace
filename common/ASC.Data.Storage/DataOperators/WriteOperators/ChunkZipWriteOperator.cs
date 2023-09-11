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

public class ChunkZipWriteOperator : IDataWriteOperator
{
    private readonly TarOutputStream _tarOutputStream;
    private readonly GZipOutputStream _gZipOutputStream;
    private readonly CommonChunkedUploadSession _chunkedUploadSession;
    private readonly CommonChunkedUploadSessionHolder _sessionHolder;
    private readonly SHA256 _sha;
    private Stream _fileStream;
    private readonly TempStream _tempStream;

    public string Hash { get; private set; }
    public string StoragePath { get; private set; }
    public bool NeedUpload
    {
        get
        {
            return false;
        }
    }

    public ChunkZipWriteOperator(TempStream tempStream,
        CommonChunkedUploadSession chunkedUploadSession,
        CommonChunkedUploadSessionHolder sessionHolder)
    {
        _tempStream = tempStream; 
        _chunkedUploadSession = chunkedUploadSession;
        _sessionHolder = sessionHolder;

        _fileStream = _tempStream.Create();
        _gZipOutputStream = new GZipOutputStream(_fileStream)
        {
            IsStreamOwner = false
        };
        _tarOutputStream = new TarOutputStream(_gZipOutputStream, Encoding.UTF8);
        _sha = SHA256.Create();
    }

    public async Task WriteEntryAsync(string tarKey, string domain, string path, IDataStore store)
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

    public async Task WriteEntryAsync(string tarKey, Stream stream)
    {
        if (_fileStream == null)
        {
            _fileStream = _tempStream.Create();
            _gZipOutputStream.baseOutputStream_ = _fileStream;
        }

        await using (var buffered = _tempStream.GetBuffered(stream))
        {
            var entry = TarEntry.CreateTarEntry(tarKey);
            entry.Size = buffered.Length;
            await _tarOutputStream.PutNextEntryAsync(entry, default);
            buffered.Position = 0;
            await buffered.CopyToAsync(_tarOutputStream);
            await _tarOutputStream.FlushAsync();
            await _tarOutputStream.CloseEntryAsync(default);
        }

        if (_fileStream.Length > _sessionHolder.MaxChunkUploadSize)
        {
            await UploadAsync(false);
        }
    }

    private async Task UploadAsync(bool last)
    {
        var chunkUploadSize = _sessionHolder.MaxChunkUploadSize;

        var buffer = new byte[chunkUploadSize];
        int bytesRead;
        _fileStream.Position = 0;
        while ((bytesRead = _fileStream.Read(buffer, 0, (int)chunkUploadSize)) > 0)
        {
            using (var theMemStream = new MemoryStream())
            {
                await theMemStream.WriteAsync(buffer, 0, bytesRead);
                theMemStream.Position = 0;
                if (bytesRead == chunkUploadSize || last)
                {
                    if (_fileStream.Position == _fileStream.Length && last)
                    {
                        _chunkedUploadSession.LastChunk = true;
                    }
                    
                    theMemStream.Position = 0;
                    StoragePath = await _sessionHolder.UploadChunkAsync(_chunkedUploadSession, theMemStream, theMemStream.Length);
                    _sha.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                }
                else
                {
                    await _fileStream.DisposeAsync();
                    _fileStream = _tempStream.Create();
                    _gZipOutputStream.baseOutputStream_ = _fileStream;

                    await theMemStream.CopyToAsync(_fileStream);
                    _fileStream.Flush();
                }
            }
        }
        if (last)
        {
            _sha.TransformFinalBlock(buffer, 0, 0);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _tarOutputStream.Close();
        _tarOutputStream.Dispose();

        await UploadAsync(true);
        _fileStream.Dispose();

        Hash = BitConverter.ToString(_sha.Hash).Replace("-", string.Empty);
        _sha.Dispose();
    }
}