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

namespace ASC.Files.Core;

[DebuggerDisplay("{Id} into {FolderId}")]
public class ChunkedUploadSession<T> : CommonChunkedUploadSession
{
    public T FolderId { get; set; }
    public File<T> File { get; set; }
    public bool Encrypted { get; set; }
    public bool KeepVersion { get; set; }

    //hack for Backup bug 48873
    [NonSerialized]
    public bool CheckQuota = true;

    public ChunkedUploadSession(File<T> file, long bytesTotal) : base(bytesTotal)
    {
        File = file;
    }

    public override object Clone()
    {
        var clone = (ChunkedUploadSession<T>)MemberwiseClone();
        clone.File = (File<T>)File.Clone();

        return clone;
    }

    public override Stream Serialize()
    {
        var str = JsonSerializer.Serialize(this);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));

        return stream;
    }

    public static ChunkedUploadSession<T> Deserialize(Stream stream, FileHelper fileHelper)
    {
        var chunkedUploadSession = JsonSerializer.Deserialize<ChunkedUploadSession<T>>(stream);
        chunkedUploadSession.File.FileHelper = fileHelper; //TODO
        chunkedUploadSession.TransformItems();

        return chunkedUploadSession;
    }
}

[Scope]
public class ChunkedUploadSessionHelper
{
    private readonly ILogger<ChunkedUploadSessionHelper> _logger;
    private readonly EntryManager _entryManager;

    public ChunkedUploadSessionHelper(ILogger<ChunkedUploadSessionHelper> logger, EntryManager entryManager)
    {
        _entryManager = entryManager;
        _logger = logger;
    }

    public async Task<object> ToResponseObjectAsync<T>(ChunkedUploadSession<T> session, bool appendBreadCrumbs = false)
    {
        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(session.FolderId); //todo: check how?
        var pathFolder = appendBreadCrumbs
            ? breadCrumbs.Select(f =>
            {
                if (f == null)
                {
                    _logger.ErrorInUserInfoRequest(session.FolderId.ToString());

                    return default;
                }

                if (f is Folder<string> f1)
                {
                    return IdConverter.Convert<T>(f1.Id);
                }

                if (f is Folder<int> f2)
                {
                    return IdConverter.Convert<T>(f2.Id);
                }

                return IdConverter.Convert<T>(0);
            })
            : new List<T> { session.FolderId };

        return new
        {
            id = session.Id,
            path = pathFolder,
            created = session.Created,
            expired = session.Expired,
            location = session.Location,
            bytes_uploaded = session.BytesUploaded,
            bytes_total = session.BytesTotal
        };
    }
}
