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

public static class Extensions
{
    private const int BufferSize = 2048;//NOTE: set to 2048 to fit in minimum tcp window

    public static async Task<Stream> IronReadStreamAsync(this IDataStore store, TempStream tempStream, string domain, string path, int tryCount)
    {
        var ms = tempStream.Create();
        await IronReadToStreamAsync(store, domain, path, tryCount, ms);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    public static Task IronReadToStreamAsync(this IDataStore store, string domain, string path, int tryCount, Stream readTo)
    {
        if (tryCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tryCount), "Must be greater or equal 1.");
        }

        if (!readTo.CanWrite)
        {
            throw new ArgumentException("stream cannot be written", nameof(readTo));
        }


        return InternalIronReadToStreamAsync(store, domain, path, tryCount, readTo);
    }

    private static async Task InternalIronReadToStreamAsync(this IDataStore store, string domain, string path, int tryCount, Stream readTo)
    {
        var tryCurrent = 0;
        var offset = 0;

        while (tryCurrent < tryCount)
        {
            try
            {
                tryCurrent++;
                using var stream = await store.GetReadStreamAsync(domain, path, offset);
                var buffer = new byte[BufferSize];
                int readed;
                while ((readed = await stream.ReadAsync(buffer, 0, BufferSize)) > 0)
                {
                    await readTo.WriteAsync(buffer, 0, readed);
                    offset += readed;
                }
                break;
            }
            catch (Exception ex)
            {
                if (tryCurrent >= tryCount)
                {
                    throw new IOException("Can not read stream. Tries count: " + tryCurrent + ".", ex);
                }

                Thread.Sleep(tryCount * 50);
            }
        }
    }
}
