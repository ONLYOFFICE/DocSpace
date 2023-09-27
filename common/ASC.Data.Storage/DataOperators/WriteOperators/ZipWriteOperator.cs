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


public class ZipWriteOperator : IDataWriteOperator
{
    private readonly GZipOutputStream _gZipOutputStream;
    private readonly TarOutputStream _tarOutputStream;
    private readonly Stream _file;
    private readonly TempStream _tempStream;

    public bool NeedUpload
    {
        get
        {
            return true;
        }
    }

    public string Hash => throw new NotImplementedException();

    public string StoragePath => throw new NotImplementedException();

    public ZipWriteOperator(TempStream tempStream, string targetFile)
    {
        _tempStream = tempStream;
        _file = new FileStream(targetFile, FileMode.Create);
        _gZipOutputStream = new GZipOutputStream(_file);
        _tarOutputStream = new TarOutputStream(_gZipOutputStream, Encoding.UTF8);
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
        await using (var buffered = _tempStream.GetBuffered(stream))
        {
            var entry = TarEntry.CreateTarEntry(tarKey);
            entry.Size = buffered.Length;
            await _tarOutputStream.PutNextEntryAsync(entry, default);
            buffered.Position = 0;
            await buffered.CopyToAsync(_tarOutputStream);
            await _tarOutputStream.CloseEntryAsync(default);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _tarOutputStream.Close();
        await _tarOutputStream.DisposeAsync();
    }
}
