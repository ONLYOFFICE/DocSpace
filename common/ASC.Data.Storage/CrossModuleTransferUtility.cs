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

namespace ASC.Data.Storage;

public class CrossModuleTransferUtility
{
    private readonly ILog _logger;
    private readonly IDataStore _source;
    private readonly IDataStore _destination;
    private readonly long _maxChunkUploadSize;
    private readonly int _chunkSize;
    private readonly IOptionsMonitor<ILog> _option;
    private readonly TempStream _tempStream;
    private readonly TempPath _tempPath;

    public CrossModuleTransferUtility(
        IOptionsMonitor<ILog> option,
        TempStream tempStream,
        TempPath tempPath,
        IDataStore source,
        IDataStore destination)
    {
        _logger = option.Get("ASC.CrossModuleTransferUtility");
        _option = option;
        _tempStream = tempStream;
        _tempPath = tempPath;
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _destination = destination ?? throw new ArgumentNullException(nameof(destination));
        _maxChunkUploadSize = 10 * 1024 * 1024;
        _chunkSize = 5 * 1024 * 1024;
    }

    public Task CopyFileAsync(string srcDomain, string srcPath, string destDomain, string destPath)
    {
        if (srcDomain == null)
        {
            throw new ArgumentNullException(nameof(srcDomain));
        }

        if (srcPath == null)
        {
            throw new ArgumentNullException(nameof(srcPath));
        }

        if (destDomain == null)
        {
            throw new ArgumentNullException(nameof(destDomain));
        }

        if (destPath == null)
        {
            throw new ArgumentNullException(nameof(destPath));
        }

        return InternalCopyFileAsync(srcDomain, srcPath, destDomain, destPath);
    }

    private async Task InternalCopyFileAsync(string srcDomain, string srcPath, string destDomain, string destPath)
    {
        using var stream = await _source.GetReadStreamAsync(srcDomain, srcPath);
        if (stream.Length < _maxChunkUploadSize)
        {
            await _destination.SaveAsync(destDomain, destPath, stream);
        }
        else
        {
            var session = new CommonChunkedUploadSession(stream.Length);
            var holder = new CommonChunkedUploadSessionHolder(_tempPath, _option, _destination, destDomain);
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
                _logger.Error("Copy File", ex);
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
