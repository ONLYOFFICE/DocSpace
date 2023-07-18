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

namespace ASC.Web.Files.Utils;

public class FilesChunkedUploadSessionHolder : CommonChunkedUploadSessionHolder
{
    private readonly IDaoFactory _daoFactory;

    public FilesChunkedUploadSessionHolder(IDaoFactory daoFactory, TempPath tempPath, IDataStore dataStore, string domain, long maxChunkUploadSize = 10485760)
        : base(tempPath, dataStore, domain, maxChunkUploadSize)
    {
        _daoFactory = daoFactory;
        TempDomain = FileConstant.StorageDomainTmp;
    }
    public override async Task<string> UploadChunkAsync(CommonChunkedUploadSession uploadSession, Stream stream, long length)
    {
        if (uploadSession is ChunkedUploadSession<int>)
        {
            return (await InternalUploadChunkAsync<int>(uploadSession, stream, length)).ToString();
        }
        else
        {
            return await InternalUploadChunkAsync<string>(uploadSession, stream, length);
        }
    }

    private async Task<T> InternalUploadChunkAsync<T>(CommonChunkedUploadSession uploadSession, Stream stream, long length)
    {
        var chunkedUploadSession = uploadSession as ChunkedUploadSession<T>;
        chunkedUploadSession.File.ContentLength += stream.Length;
        var fileDao = GetFileDao<T>();
        var file = await fileDao.UploadChunkAsync(chunkedUploadSession, stream, length);
        return file.Id;
    }

    public override async Task<string> FinalizeAsync(CommonChunkedUploadSession uploadSession)
    {
        if (uploadSession is ChunkedUploadSession<int>)
        {
            return (await InternalFinalizeAsync<int>(uploadSession)).ToString();
        }
        else
        {
            return await InternalFinalizeAsync<string>(uploadSession);
        }
    }

    private async Task<T> InternalFinalizeAsync<T>(CommonChunkedUploadSession commonChunkedUploadSession)
    {
        var chunkedUploadSession = commonChunkedUploadSession as ChunkedUploadSession<T>;
        chunkedUploadSession.BytesTotal = chunkedUploadSession.BytesUploaded;
        var fileDao = GetFileDao<T>();
        var file = await fileDao.FinalizeUploadSessionAsync(chunkedUploadSession);
        return file.Id;
    }

    private IFileDao<T> GetFileDao<T>()
    {
        return _daoFactory.GetFileDao<T>();
    }
}
