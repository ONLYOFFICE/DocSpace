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

namespace ASC.Data.Storage;

public class CrossModuleTransferUtility
{
    private readonly ILogger _logger;
    private readonly IDataStore _source;
    private readonly IDataStore _destination;
    private readonly long _maxChunkUploadSize;
    private readonly int _chunkSize;
    private readonly TempStream _tempStream;
    private readonly TempPath _tempPath;

    public CrossModuleTransferUtility(
        ILogger option,
        TempStream tempStream,
        TempPath tempPath,
        IDataStore source,
        IDataStore destination)
    {
        _logger = option;
        _tempStream = tempStream;
        _tempPath = tempPath;
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _destination = destination ?? throw new ArgumentNullException(nameof(destination));
        _maxChunkUploadSize = 10 * 1024 * 1024;
        _chunkSize = 5 * 1024 * 1024;
    }

    public async ValueTask CopyFileAsync(string srcDomain, string srcPath, string destDomain, string destPath)
    {
        ArgumentNullException.ThrowIfNull(srcDomain);
        ArgumentNullException.ThrowIfNull(srcPath);
        ArgumentNullException.ThrowIfNull(destDomain);
        ArgumentNullException.ThrowIfNull(destPath);

        await using var stream = await _source.GetReadStreamAsync(srcDomain, srcPath);
        if (stream.Length < _maxChunkUploadSize)
        {
            await _destination.SaveAsync(destDomain, destPath, stream);
        }
        else
        {
            var session = new CommonChunkedUploadSession(stream.Length);
            var holder = new CommonChunkedUploadSessionHolder(_tempPath, _destination, destDomain);
            await holder.InitAsync(session);
            try
            {
                Stream memstream = null;
                try
                {
                    while (GetStream(stream, out memstream))
                    {
                        memstream.Seek(0, SeekOrigin.Begin);
                        await holder.UploadChunkAsync(session, memstream, _chunkSize);
                        await memstream.DisposeAsync();
                    }
                }
                finally
                {
                    if (memstream != null)
                    {
                        await memstream.DisposeAsync();
                    }
                }

                await holder.FinalizeAsync(session);
                await _destination.MoveAsync(destDomain, session.TempPath, destDomain, destPath);
            }
            catch (Exception ex)
            {
                _logger.ErrorCopyFile(ex);
                await holder.AbortAsync(session);
            }
        }
    }

    private bool GetStream(Stream stream, out Stream memstream)
    {
        memstream = _tempStream.Create();
        var total = 0;
        int readed;
        const int portion = 2048;
        var buffer = new byte[portion];

        while ((readed = stream.Read(buffer, 0, portion)) > 0)
        {
            memstream.Write(buffer, 0, readed);
            total += readed;
            if (total >= _chunkSize)
            {
                break;
            }
        }

        return total > 0;
    }
}
